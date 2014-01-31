using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using BattleIntel.Web.Models;
using DotNetOpenAuth.OpenId.Extensions;
using DotNetOpenAuth.OpenId.RelyingParty;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using BattleIntel.Core;

namespace BattleIntel.Web.Controllers
{

    [Authorize]
    public class AccountController : NHibernateController
    {
       private static OpenIdRelyingParty openid = new OpenIdRelyingParty();

       public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            var response = openid.GetResponse();
            if (response != null)
            {
                //check the response status
                switch (response.Status)
                {
                    //success status
                    case AuthenticationStatus.Authenticated:

                        string email = null;
                        var claimsResponse = response.GetExtension<ClaimsResponse>();
                        if (claimsResponse != null)
                        {
                            email = claimsResponse.Email;
                        }

                        var user = GetOrCreateUserForOpenId(response.ClaimedIdentifier, email);

                        bool rememberMe;
                        bool.TryParse(response.GetCallbackArgument("rememberMe"), out rememberMe);

                        SetUserAuthCookie(user, rememberMe);

                        string authReturnUrl = response.GetCallbackArgument("returnUrl");
                        if (Url.IsLocalUrl(authReturnUrl))
                        {
                            return Redirect(authReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }

                    case AuthenticationStatus.Canceled:
                        ModelState.AddModelError("err", "Canceled at provider");
                        break;

                    case AuthenticationStatus.Failed:
                        ModelState.AddModelError("err", response.Exception.Message);
                        break;
                }
            }
            
            return View(new LoginModel());
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            Identifier id;
            if (!Identifier.TryParse(model.OpenIdProvider, out id))
            {
                ModelState.AddModelError("OpenId", "Invalid Open Id");
                return View(model);
            }

            try
            {
                var authRequest = openid.CreateRequest(id);

                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    authRequest.AddCallbackArguments("returnUrl", returnUrl);
                }

                authRequest.AddCallbackArguments("rememberMe", model.RememberMe.ToString());

                authRequest.AddExtension(new ClaimsRequest 
                { 
                    Email = DemandLevel.Require
                });

                return authRequest.RedirectingResponse.AsActionResultMvc5();
            }
            catch (ProtocolException ex)
            {
                ModelState.AddModelError("err", ex.Message);
                return View(model);
            }
        }

        private User GetOrCreateUserForOpenId(string claimedIdentifier, string email)
        {
            var existing = Session.QueryOver<UserOpenId>()
                .Where(x => x.OpenIdentifier == claimedIdentifier)
                .Fetch(x => x.User).Eager
                .SingleOrDefault();

            if (existing != null)
            {
                existing.User.Email = email;
                return existing.User;
            }

            var newUser = new User
            {
                Name = "Anonymous",
                Email = email,
                JoinDateUTC = DateTime.UtcNow
            };

            Session.Save(newUser);
            Session.Save(new UserOpenId
            {
                User = newUser,
                OpenIdentifier = claimedIdentifier
            });

            return newUser;
        }

        private void SetUserAuthCookie(User user, bool isPersistent)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                user.Name,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                isPersistent,
                user.Id.ToString(),
                FormsAuthentication.FormsCookiePath);

            // Encrypt the ticket.
            string encTicket = FormsAuthentication.Encrypt(ticket);

            // Create the cookie.
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            throw new NotImplementedException();
        }
    }
}

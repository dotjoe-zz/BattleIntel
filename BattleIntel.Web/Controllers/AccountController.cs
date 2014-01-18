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
            return View(new LoginModel());
        }

        [AllowAnonymous]
        public ActionResult Authenticate(LoginModel model, string returnUrl)
        {
            var response = openid.GetResponse();
            if (response == null)
            {
                Identifier id;
                //make sure your users openid identifier is valid.
                if (Identifier.TryParse(model.OpenId, out id))
                {
                    try
                    {
                        HttpSession["RememberMe"] = model.RememberMe;

                        //request openid_identifier
                        return openid.CreateRequest(id)
                            .RedirectingResponse.AsActionResultMvc5();
                    }
                    catch (ProtocolException ex)
                    {
                        ModelState.AddModelError("err", ex.Message);
                        return View("Login", model);
                    }
                }
                else
                {
                    ModelState.AddModelError("OpenId", "Invalid identifier");
                    return View("Login", model);
                }
            }
            else
            {
                //check the response status
                switch (response.Status)
                {
                    //success status
                    case AuthenticationStatus.Authenticated:


                        bool rememberMe = HttpSession["RememberMe"] as bool? ?? false;
                        HttpSession.Remove("RememberMe");

                        var user = GetOrCreateRandomUserForOpenId(response.ClaimedIdentifier);

                        FormsAuthentication.SetAuthCookie(user.Username, rememberMe);

                        //TODO: lookup username or generate random username and redirect to an account creation page

                        return RedirectToAction("Index", "Home");

                    case AuthenticationStatus.Canceled:
                        ModelState.AddModelError("err", "Canceled at provider");
                        return View("Login", model);

                    case AuthenticationStatus.Failed:
                        ModelState.AddModelError("err", response.Exception.Message);
                        return View("Login", model);
                }
            }
            return new EmptyResult();
        }

        private User GetOrCreateRandomUserForOpenId(string claimedIdentifier)
        {
            var existing = Session.QueryOver<UserOpenId>()
                .Where(x => x.OpenIdentifier == claimedIdentifier)
                .Fetch(x => x.User).Eager
                .SingleOrDefault();

            if (existing != null) return existing.User;

            //TODO generate a real random username
            int lastUserId = Session.QueryOver<User>()
                .OrderBy(x => x.Id).Desc
                .Select(x => x.Id)
                .Take(1)
                .SingleOrDefault<int?>() ?? 0;

            var newUser = new User
            {
                Username = "Anonymous-" + (lastUserId + 1),
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

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
using AutoMapper;

namespace BattleIntel.Web.Controllers
{

    [Authorize]
    public class AccountController : NHibernateController
    {
        private static OpenIdRelyingParty openid = new OpenIdRelyingParty();

        [AllowAnonymous]
        public ActionResult Login()
        {
            var response = openid.GetResponse();
            if (response == null) return View(new LoginModel());
            
            //check the response status
            switch (response.Status)
            {
                //success status
                case AuthenticationStatus.Authenticated:

                    var userData = OpenIdUserData.CreateFromResponse(response);
                    var user = GetOrCreateUserForOpenId(response.ClaimedIdentifier, userData);

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

                OpenIdUserData.AddClaimsRequest(authRequest);

                return authRequest.RedirectingResponse.AsActionResultMvc5();
            }
            catch (ProtocolException ex)
            {
                ModelState.AddModelError("err", ex.Message);
                return View(model);
            }
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
        {
            var user = Session.Get<User>(UserData.Id);
            if (user == null) return HttpNotFound();

            var model = new UserIndexModel
            {
                User = Mapper.Map<UserModel>(user)
            };
            return View(model);
        }

        public ActionResult Edit()
        {
            var user = Session.Get<User>(UserData.Id);
            if (user == null) return HttpNotFound();

            var model = new UserEditModel
            {
                DisplayName = user.DisplayName
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(UserEditModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = Session.Get<User>(UserData.Id);
            if (user == null) return HttpNotFound();

            user.DisplayName = model.DisplayName;

            return RedirectToAction("Index");
        }

        private class OpenIdUserData
        {
            public string DisplayName { get; private set; }
            public string Email { get; private set; }

            public static void AddClaimsRequest(IAuthenticationRequest r)
            {
                r.AddExtension(new ClaimsRequest
                {
                    Email = DemandLevel.Require,
                    Nickname = DemandLevel.Require,
                    FullName = DemandLevel.Require
                });
            }

            public static OpenIdUserData CreateFromResponse(IAuthenticationResponse r)
            {
                var userData = new OpenIdUserData();

                var claimsResponse = r.GetExtension<ClaimsResponse>();
                if (claimsResponse != null)
                {
                    userData.Email = claimsResponse.Email;
                    userData.DisplayName = claimsResponse.Nickname ?? claimsResponse.FullName;
                }
                
                return userData;
            }
        }

        private User GetOrCreateUserForOpenId(string claimedIdentifier, OpenIdUserData u)
        {
            var existing = Session.QueryOver<UserOpenId>()
                .Where(x => x.OpenIdentifier == claimedIdentifier)
                .Fetch(x => x.User).Eager
                .SingleOrDefault();

            if (existing != null)
            {
                return existing.User;
            }

            var newUser = new User
            {
                DisplayName = u.DisplayName ?? "Anonymous",
                Email = u.Email,
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

        private void SetUserAuthCookie(User user, bool rememberMe)
        {
            Response.SetAuthCookie<UserDataModel>(user.Id.ToString(), rememberMe, new UserDataModel
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName
            });
        }

        
    }
}

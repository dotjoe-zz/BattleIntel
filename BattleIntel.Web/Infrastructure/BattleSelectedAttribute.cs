using BattleIntel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BattleIntel.Web
{
    public class BattleSelectedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.GetBattleCookieValue() == null)
            {
                filterContext.Result = new RedirectToRouteResult("Home", null);
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }
}
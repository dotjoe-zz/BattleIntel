using BattleIntel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BattleIntel.Web.Controllers
{
    public class HomeController : NHibernateController
    {
        public ActionResult Index()
        {
            var statsCollected = Session.QueryOver<BattleStat>().RowCount();
            ViewBag.Message = statsCollected + " total battle stats collected! Your Email is " + UserData.Email;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}

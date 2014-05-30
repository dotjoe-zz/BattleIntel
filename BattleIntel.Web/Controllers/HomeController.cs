using BattleIntel.Core;
using BattleIntel.Web.Models;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BattleIntel.Web.Controllers
{
    public class HomeController : NHibernateController
    {
        public ActionResult Index(bool? change)
        {
            if (change != true && SelectedBattle != null) return RedirectToAction("Index", "Teams");

            IntelReport reportsAlias = null;
            BattleHeader dto = null;

            var battles = Session.QueryOver<Battle>()
                .Left.JoinAlias(x => x.Reports, () => reportsAlias)
                .Where(() => reportsAlias.Id == null || reportsAlias.ReportStatsCount > 0) //ignore the chat and botmessages
                .SelectList(list => list
                    .SelectGroup(x => x.Id).WithAlias(() => dto.Id)
                    .SelectGroup(x => x.Name).WithAlias(() => dto.Name)
                    .SelectGroup(x => x.StartDateUTC).WithAlias(() => dto.StartDateUTC)
                    .SelectGroup(x => x.EndDateUTC).WithAlias(() => dto.EndDateUTC)
                    .SelectCountDistinct(() => reportsAlias.UserId).WithAlias(() => dto.NumUsersReporting)
                    .SelectCountDistinct(() => reportsAlias.MessageId).WithAlias(() => dto.NumReports)
                    .SelectCountDistinct(() => reportsAlias.Team.Id).WithAlias(() => dto.NumTeams)
                    .SelectSum(() => reportsAlias.NewStatsCount).WithAlias(() => dto.NumStats))
                .OrderBy(x => x.StartDateUTC).Desc
                .TransformUsing(Transformers.AliasToBean<BattleHeader>())
                .List<BattleHeader>();
                    
            return View(battles);
        }

        public ActionResult BattleSelect(int id)
        {
            var battle = Session.Get<Battle>(id);
            if (battle == null) return HttpNotFound();

            var cookie = new HttpCookie("Battle", id.ToString());
            cookie.Expires = DateTime.UtcNow.AddDays(7);
           
            Response.Cookies.Set(cookie);

            return RedirectToAction("Index", "Teams");
        }

        public ActionResult BattleReports(int id)
        {
            var battle = Session.Get<Battle>(id);
            if (battle == null) return HttpNotFound();

            Team teamAlias = null;
            IntelReportHeader dto = null;

            var reports = Session.QueryOver<IntelReport>()
                .Left.JoinAlias(x => x.Team, () => teamAlias)
                .Where(x => x.Battle.Id == id)
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => dto.Id)
                    .Select(x => x.UserName).WithAlias(() => dto.UserName)
                    .Select(x => x.CreateDateUTC).WithAlias(() => dto.CreateDateUTC)
                    .Select(x => x.Text).WithAlias(() => dto.Text)
                    .Select(x => x.ReportStatsCount).WithAlias(() => dto.ReportStatsCount)
                    .Select(x => x.NewStatsCount).WithAlias(() => dto.NewStatsCount)
                    .Select(() => teamAlias.Id).WithAlias(() => dto.TeamId)
                    .Select(() => teamAlias.Name).WithAlias(() => dto.TeamName))
                .TransformUsing(Transformers.AliasToBean<IntelReportHeader>())
                .List<IntelReportHeader>();

            return View(reports);
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

using BattleIntel.Core;
using BattleIntel.Core.Services;
using BattleIntel.Web.Models;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BattleIntel.Web.Controllers
{
    [BattleSelected]
    public class TeamsController : NHibernateController
    {
        // GET: Team
        public ActionResult Index(string sort)
        {
            var battle = Session.Get<Battle>(SelectedBattle.Id);
            if (battle == null) return HttpNotFound();

            ViewBag.Sort = sort;
            Team teamAlias = null;
            TeamIntelHeader dto = null;

            var q = Session.QueryOver<IntelReport>()
                .Inner.JoinAlias(x => x.Team, () => teamAlias)
                .Where(x => x.Battle.Id == battle.Id)
                .SelectList(list => list
                    .SelectGroup(() => teamAlias.Id).WithAlias(() => dto.Id)
                    .SelectGroup(() => teamAlias.Name).WithAlias(() => dto.Name)
                    .SelectCount(x => x.Id).WithAlias(() => dto.NumReports)
                    .SelectSum(x => x.NewStatsCount).WithAlias(() => dto.NumStats)
                    .SelectMax(x => x.CreateDateUTC).WithAlias(() => dto.MostRecentReportUTC));

            if(string.IsNullOrEmpty(sort))
            {
                q.OrderByAlias(() => teamAlias.Name).Asc();
            }
            else if (sort == "date")
            {
                q.UnderlyingCriteria.AddOrder(new Order("MostRecentReportUTC", false));
            }

            var teams = q
                .TransformUsing(Transformers.AliasToBean<TeamIntelHeader>())
                .List<TeamIntelHeader>();

            return View(teams);
        }

        public ActionResult Details(int id)
        {
            var team = Session.Get<Team>(id);
            if (team == null) return HttpNotFound();

            var model = new TeamIntelDetails
            {
                Id = team.Id,
                Name = team.Name
            };

            BattleStatMini statDto = null;

            model.Stats = Session.QueryOver<BattleStat>()
                .Where(x => x.Battle.Id == SelectedBattle.Id)
                .And(x => x.Team.Id == team.Id)
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => statDto.Id)
                    .Select(x => x.Stat.Level).WithAlias(() => statDto.Level)
                    .Select(x => x.Stat.Name).WithAlias(() => statDto.Name)
                    .Select(x => x.Stat.Defense).WithAlias(() => statDto.Defense)
                    .Select(x => x.Stat.AdditionalInfo).WithAlias(() => statDto.AdditionalInfo)
                    .Select(x => x.IsDeleted).WithAlias(() => statDto.IsDeleted))
                .OrderBy(x => x.Stat.Level).Desc
                .ThenBy(x => x.Stat.DefenseValue).Desc
                .ThenBy(x => x.Stat.Name).Asc
                .TransformUsing(Transformers.AliasToBean<BattleStatMini>())
                .List<BattleStatMini>();

            IntelReportMini reportDto = null;

            model.Reports = Session.QueryOver<IntelReport>()
                .Where(x => x.Battle.Id == SelectedBattle.Id)
                .And(x => x.Team.Id == team.Id)
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => reportDto.Id)
                    .Select(x => x.UserName).WithAlias(() => reportDto.UserName)
                    .Select(x => x.CreateDateUTC).WithAlias(() => reportDto.CreateDateUTC)
                    .Select(x => x.Text).WithAlias(() => reportDto.Text)
                    .Select(x => x.UpdatedText).WithAlias(() => reportDto.UpdatedText)
                    .Select(x => x.NewStatsCount).WithAlias(() => reportDto.NewStatsCount)
                    .Select(x => x.IsChat).WithAlias(() => reportDto.IsChat))
                .OrderBy(x => x.CreateDateUTC).Asc
                .TransformUsing(Transformers.AliasToBean<IntelReportMini>())
                .List<IntelReportMini>();

            return View(model);
        }

        [HttpPost]
        public ActionResult StatDelete(int id, int[] statsToDelete)
        {
            var stats = Session.QueryOver<BattleStat>()
                .Where(x => x.Battle.Id == SelectedBattle.Id)
                .And(x => x.Team.Id == id)
                .List<BattleStat>();

            foreach (var stat in stats)
            {
                stat.IsDeleted = (statsToDelete != null && statsToDelete.Contains(stat.Id));
            }

            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        public ActionResult UpdateReport(int id, string text, bool? isChat)
        {
            var report = Session.Get<IntelReport>(id);
            if (report == null) return HttpNotFound();

            if (isChat == true && !report.IsChat)
            {
                report.ClearStats();
                report.IsChat = true;
                return RedirectToAction("Index");
            }

            var currentText = report.UpdatedText ?? report.Text;
            if (currentText.Equals(text)) return RedirectToAction("Details", new { id = report.Team.Id }); //nothing updated

            report.UpdatedText = text;
            new IntelReportProcessor(Session, report.Battle).ReParseReportText(report);
            
            //team could have changed!
            return RedirectToAction("Details", new { id = report.Team.Id });
        }
    }
}
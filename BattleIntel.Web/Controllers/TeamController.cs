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
    [BattleSelected]
    public class TeamController : NHibernateController
    {
        // GET: Team
        public ActionResult Index()
        {
            var battle = Session.Get<Battle>(SelectedBattle.Id);
            if (battle == null) return HttpNotFound();

            Team teamAlias = null;
            TeamIntelHeader dto = null;

            var teams = Session.QueryOver<IntelReport>()
                .Inner.JoinAlias(x => x.Team, () => teamAlias)
                .Where(x => x.Battle.Id == battle.Id)
                .SelectList(list => list
                    .SelectGroup(() => teamAlias.Id).WithAlias(() => dto.Id)
                    .SelectGroup(() => teamAlias.Name).WithAlias(() => dto.Name)
                    .SelectCount(x => x.Id).WithAlias(() => dto.NumReports)
                    .SelectSum(x => x.NewStatsCount).WithAlias(() => dto.NumStats)
                    .SelectMax(x => x.CreateDateUTC).WithAlias(() => dto.MostRecentReportUTC))
                .OrderByAlias(() => teamAlias.Name).Asc
                .TransformUsing(Transformers.AliasToBean<TeamIntelHeader>())
                .List<TeamIntelHeader>();

            return View(teams);
        }
    }
}
using BattleIntel.Core.Db;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BattleIntel.Web.Controllers
{
    [NHibernateTransaction]
    public class NHibernateController : Controller
    {
        public HttpSessionStateBase HttpSession { get { return base.Session; } }
        public new ISession Session { get; set; }
	}

    /// <summary>
    /// Wrap NHibernateController action methods in a Transaction. Rollback on exception.
    /// </summary>
    public class NHibernateTransactionAttribute : FilterAttribute, IActionFilter
    {
        private static readonly ISessionFactory factory = NHibernateConfiguration.BuildSessionFactory();
        private ISession session;
        private ITransaction transaction;

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var nhController = filterContext.Controller as NHibernateController;
            if (nhController == null) throw new InvalidOperationException("NHibernateTransactionFilter can only be used with an NHibernateController.");

            session = factory.OpenSession();
            session.FlushMode = FlushMode.Commit;
            transaction = session.BeginTransaction();

            nhController.Session = session;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            using (session)
            using (transaction)
            {
                if (transaction.IsActive)
                {
                    if (filterContext.Exception != null)
                    {
                        transaction.Rollback();
                    }
                    else
                    {
                        transaction.Commit();
                    }
                }
            }
        }
    }
}
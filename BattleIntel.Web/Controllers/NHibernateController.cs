﻿using BattleIntel.Core.Db;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BattleIntel.Web.Controllers
{
    public class NHibernateController : Controller
    {
        public HttpSessionStateBase HttpSession { get { return base.Session; } }
        public new ISession Session { get; set; }

        private int? _CurrentUserId = null;
        public int CurrentUserId
        {
            get
            {
                if (_CurrentUserId.HasValue) return _CurrentUserId.Value;
                if (!User.Identity.IsAuthenticated) return 0;
                
                int userId = 0;

                if (User.Identity.IsAuthenticated &&
                    int.TryParse(((FormsIdentity)User.Identity).Ticket.UserData, out userId))
                {
                    _CurrentUserId = userId;
                }
                else
                {
                    _CurrentUserId = 0;
                }

                return _CurrentUserId.Value;
            }
        }
	}

    /// <summary>
    /// Wrap NHibernateController action methods in a Transaction. Rollback on exception.
    /// </summary>
    public class NHibernateTransactionFilter : IActionFilter
    {
        private readonly ISession session;
        private ITransaction transaction;

        public NHibernateTransactionFilter(ISession session)
        {
            this.session = session;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var nhController = filterContext.Controller as NHibernateController;
            if (nhController == null) throw new InvalidOperationException("NHibernateTransactionFilter can only be used with an NHibernateController.");

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
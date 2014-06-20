using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BattleIntel.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static HtmlString TimeAgo(this HtmlHelper helper, DateTime date)
        {
            return MvcHtmlString.Create(string.Format("<time class='timeago' datetime='{0}'>{1}</time>",
                date.ToString("o"), date.ToString("M")));
        }
    }
}
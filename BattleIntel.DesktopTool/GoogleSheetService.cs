using BattleIntel.Core;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleIntel.DesktopTool
{
    class GoogleSheetService
    {
        private SpreadsheetsService service;

        private GoogleSheetService() {}

        public static GoogleSheetService Init(string clientID, string clientSecret)
        {
            var parameters = new OAuth2Parameters
            {
                ClientId = clientID,
                ClientSecret = clientSecret,
                Scope = "https://spreadsheets.google.com/feeds",
                RedirectUri = "urn:ietf:wg:oauth:2.0:oob" //installed applications
            };

            var authUri = new Uri(OAuthUtil.CreateOAuth2AuthorizationUrl(parameters));
            using (var login = new GoogleLogin(authUri))
            {
                if (login.ShowDialog() != DialogResult.OK) return null; 
                parameters.AccessCode = login.AccessCode;
            }

            try
            {
                OAuthUtil.GetAccessToken(parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }

            var instance = new GoogleSheetService();
            instance.service = new SpreadsheetsService("BattleIntel");
            instance.service.RequestFactory = new GOAuth2RequestFactory(null, "BattleIntel", parameters);

            return instance;
        }

        public IList<StatContext> FindStatsByTeamName(string teamName)
        {
            throw new NotImplementedException();
        }

        public bool InsertStats(string teamName, IList<Stat> stats)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Where a list of stats came from.
    /// </summary>
    class StatContext
    {
        public string spreadsheetTitle { get; set; }
        public string spreadsheetHref { get; set; }
        public string sheetTitle { get; set; }
        public string sheetHref { get; set; }
        public string teamName { get; set; }
        public IList<Stat> stats { get; set; }
    }

    class SheetScope
    {
        public string spreadsheetHref { get; set; }
        public string sheetHref { get; set; }
    }
}

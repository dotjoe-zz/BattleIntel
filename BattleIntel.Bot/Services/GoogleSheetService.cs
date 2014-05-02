using BattleIntel.Core;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleIntel.Bot
{
    class GoogleSheetService
    {
        private SpreadsheetsService service;

        private GoogleSheetService() {}

        public static GoogleSheetService Init()
        {
            var parameters = new OAuth2Parameters
            {
                ClientId = ConfigurationManager.AppSettings.Get("Google-ClientID"),
                ClientSecret = ConfigurationManager.AppSettings.Get("Google-ClientSecret"),
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

        public IList<SpreadsheetModel> ListSpreadsheets()
        {
            var results = new List<SpreadsheetModel>();

            SpreadsheetFeed feed = service.Query(new SpreadsheetQuery());
            foreach (var entry in feed.Entries)
            {
                AtomLink wsLink = entry.Links.FindService(GDataSpreadsheetsNameTable.WorksheetRel, AtomLink.ATOM_TYPE);
                results.Add(new SpreadsheetModel
                {
                    Title = entry.Title.Text,
                    WorksheetsFeedURI = wsLink.HRef.Content
                });
            }

            return results;
        }

        public IList<WorksheetModel> ListWorksheets(string WorksheetsFeedURI)
        {
            var results = new List<WorksheetModel>();

            WorksheetFeed feed = service.Query(new WorksheetQuery(WorksheetsFeedURI));
            foreach (var entry in feed.Entries)
            {
                var cellsLink = entry.Links.FindService(GDataSpreadsheetsNameTable.CellRel, AtomLink.ATOM_TYPE);
                var listLink = entry.Links.FindService(GDataSpreadsheetsNameTable.ListRel, AtomLink.ATOM_TYPE);
                results.Add(new WorksheetModel 
                {
                    Title = entry.Title.Text,
                    CellsFeedURI = cellsLink.HRef.Content,
                    ListFeedURI = listLink.HRef.Content
                });
            }

            return results;
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

    class SpreadsheetModel
    {
        public string Title { get; set; }
        public string WorksheetsFeedURI { get; set; }
    }

    class WorksheetModel
    {
        public string Title { get; set; }
        public string CellsFeedURI { get; set; }
        public string ListFeedURI { get; set; }
    }
}

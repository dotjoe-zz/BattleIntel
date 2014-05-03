using BattleIntel.Core;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using System;
using System.Collections.Generic;

namespace GSheet
{
    using GSheet.Models;

    public class GSheetService
    {
        private SpreadsheetsService service;

        public GSheetService(OAuth2Parameters parameters) 
        {
            service = new SpreadsheetsService("BattleIntel");
            service.RequestFactory = new GOAuth2RequestFactory(null, "BattleIntel", parameters);
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
                    Url = entry.AlternateUri.Content,
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
}

namespace GSheet.Models
{
    public class SpreadsheetModel
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string WorksheetsFeedURI { get; set; }
    }

    public class WorksheetModel
    {
        public string Title { get; set; }
        public string CellsFeedURI { get; set; }
        public string ListFeedURI { get; set; }
    }
}

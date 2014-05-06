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

        public void MergeSheet(string listFeedURI, IList<IntelDataRow> rows)
        {
            //TODO might need to use CellFeed to ensure ColumnHeader's exist with correct names

            ListFeed feed = service.Query(new ListQuery(listFeedURI));

            int i = 0;
            //update existing rows
            for (; i < feed.Entries.Count && i < rows.Count; ++i)
            {
                ListEntry entry = feed.Entries[i] as ListEntry;
                IntelDataRow r = rows[i];

                //TODO are elements ordered by column or do need to use the column header name?
                //TODO check if we actually updated the value!! and skip if it has not changed
                entry.Elements[0].Value = r.Team;
                entry.Elements[1].Value = r.Stats;

                entry.Update();
            }

            //delete the remaining rows
            for (; i < feed.Entries.Count; ++i)
            {
                ListEntry entry = feed.Entries[i] as ListEntry;
                entry.Delete();
            }
            
            //or add the remaining intel
            for (; i < rows.Count; ++i)
            {
                var r = rows[i];
                var entry = new ListEntry();
                entry.Elements.Add(new ListEntry.Custom() { LocalName = "team", Value = r.Team });
                entry.Elements.Add(new ListEntry.Custom() { LocalName = "stats", Value = r.Stats });

                service.Insert(new Uri(listFeedURI), entry);
            }
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

    public class IntelDataRow
    {
        public string Team { get; set; }
        public string Stats { get; set; }
    }
}

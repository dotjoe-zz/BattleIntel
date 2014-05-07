using BattleIntel.Core;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void MergeSheet(string cellsFeedURI, string listFeedURI, IList<IntelDataRow> rows)
        {
            //make sure our column headers are set to known values since the list feed depends on them
            SetColumnHeaders(cellsFeedURI, "Team", "Stats");
            
            ListFeed feed = service.Query(new ListQuery(listFeedURI));

            //NOTE: LocalName is the ColumnHeader value except it MUST use lower case AND replace spaces with _
            string[][] values = 
            {
                new string[] {"team", ""},
                new string[] {"stats", ""}
            };

            int i = 0;
            //update existing rows
            for (; i < feed.Entries.Count && i < rows.Count; ++i)
            {
                ListEntry entry = (ListEntry)feed.Entries[i];
                IntelDataRow r = rows[i];
                values[0][1] = r.Team;
                values[1][1] = r.Stats;  

                bool update = false;

                foreach (ListEntry.Custom el in entry.Elements)
                foreach (var kv in values)
                {
                    if(el.LocalName == kv[0] && el.Value != kv[1])
                    {
                        update = true;
                        el.Value = kv[1];
                    }
                }
                
                if(update) entry.Update();
            }

            //delete the remaining rows
            for (; i < feed.Entries.Count; ++i)
            {
                ListEntry entry = (ListEntry)feed.Entries[i];
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

        private void SetColumnHeaders(string cellsFeedURI, params string[] headers)
        {
            CellFeed feed = service.Query(new CellQuery(cellsFeedURI));

            for(int i = 0; i < headers.Length; ++i)
            {
                CellEntry entry = new CellEntry
                {
                    Cell = new CellEntry.CellElement
                    {
                        InputValue = headers[i],
                        Row = 1,
                        Column = (uint)i + 1
                    }
                };
                service.Insert(new Uri(cellsFeedURI), entry);
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

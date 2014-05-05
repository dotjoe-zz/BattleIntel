using GSheet;
using GroupMe;
using GroupMe.Models;
using System;
using System.Configuration;
using System.Threading;
using System.Windows.Forms;
using GSheet.Models;

namespace BattleIntel.Bot
{
    class IntelBot
    {
        private readonly IIntelBotConsole console;

        private int? battleId;
        private Group intelRoom;
        private SpreadsheetModel spreadsheet;
        private WorksheetModel worksheet;

        private GroupMeService groupMe;
        private GSheetService google;

        public IntelBot(IIntelBotConsole console) 
        { 
            this.console = console;
        }

        public bool ConnectToBattle(IWin32Window owner)
        {
            using (var bs = new BattleSelector())
            {
                if (bs.ShowDialog(owner) == DialogResult.OK)
                {
                    battleId = bs.SelectedBattleId;
                    console.AppendLine(string.Format("Connected to BattleId: {0}", battleId));
                    return true;
                }
            }

            return false;
        }

        public bool ConnectToIntelRoom(IWin32Window owner)
        {
            if (groupMe == null)
            {
                groupMe = new GroupMeService(ConfigurationManager.AppSettings.Get("GroupMe-AccessToken"));
            }

            using (var gs = new GroupMeRoomSelector(groupMe))
            {
                if (gs.ShowDialog(owner) == DialogResult.OK)
                {
                    intelRoom = gs.SelectedGroup;
                    console.AppendLine(string.Format("Connected to Intel Room: {0} (id:{1})", intelRoom.name, intelRoom.id));
                    return true;
                }
            }

            return false;
        }

        public bool ConnectToGoogleSheet(IWin32Window owner)
        {
            if (google == null)
            {
                var auth = GoogleOAuth.Authorize();
                if (auth == null) return false;
                google = new GSheetService(auth);
            }

            using (var ws = new WorksheetSelector(google))
            {
                if (ws.ShowDialog() == DialogResult.OK)
                {
                    spreadsheet = ws.SelectedSpreadsheet;
                    worksheet = ws.SelectedWorksheet;
                    console.AppendLine(string.Format("Connected to Spreadsheet: {0}:{1} {2}", spreadsheet.Title, worksheet.Title, spreadsheet.Url));
                }
            }

            return false;
        }

        public bool ProcessIntel()
        {
            if (intelRoom == null || battleId == null)
            {
                console.AppendLine(string.Format("{0:G} BOT requires a battle AND intel room connection.", DateTime.Now));
                return false;
            }

            console.Append(string.Format("{0:G} checking for new intel...", DateTime.Now));

            Thread.Sleep(1000);
            //console.Append(string.Format("{0:G} parsing new intel...", DateTime.Now));

            //console.Append(string.Format("{0:G} writing new intel to sheet...", DateTime.Now));
            
            console.AppendLine("done");
            return true;
        }
    }

    interface IIntelBotConsole
    {
        void AppendLine(string s);
        void Append(string s);
    }
}

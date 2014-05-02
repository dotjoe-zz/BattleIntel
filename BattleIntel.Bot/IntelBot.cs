using BattleIntel.Core.Db;
using GroupMe;
using GroupMe.Responses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleIntel.Bot
{
    class IntelBot
    {
        private IIntelMessagingConsole console;
        private System.Timers.Timer timer;

        private GroupMeService groupMe;
        private Group intelRoom;
        private int? BattleId;

        public bool IsRunning { get; private set; }

        public IntelBot(IIntelMessagingConsole console) 
        { 
            this.console = console;

            this.timer = new System.Timers.Timer(10000);
            this.timer.Elapsed += timer_Elapsed;

            this.groupMe = new GroupMeService(ConfigurationManager.AppSettings.Get("GroupMe-AccessToken"));

            ConnectToBattle();
            ConnectToIntelRoom();
            ConnectToGoogleSheet();
        }

        private bool ConnectToBattle()
        {
            using (var bs = new BattleSelector())
            {
                if (bs.ShowDialog() == DialogResult.OK)
                {
                    BattleId = bs.SelectedBattleId;
                    console.AppendLine(string.Format("Connected to BattleId: {0}", BattleId));
                    return true;
                }
            }

            return false;
        }

        public bool ConnectToIntelRoom()
        {
            using (var gs = new GroupMeRoomSelector(groupMe))
            {
                if (gs.ShowDialog() == DialogResult.OK)
                {
                    intelRoom = gs.SelectedGroup;
                    console.AppendLine(string.Format("Connected to Intel Room: {0} (id:{1})", intelRoom.name, intelRoom.id));
                    return true;
                }
            }

            return false;
        }

        public bool ConnectToGoogleSheet()
        {
            return false;
        }

        private void ProcessIntel()
        {
            console.Append(string.Format("{0:G} checking for new intel...", DateTime.Now));

            //console.Append(string.Format("{0:G} parsing new intel...", DateTime.Now));

            //console.Append(string.Format("{0:G} writing new intel to sheet...", DateTime.Now));
        }

        public void Start()
        {
            console.AppendLine(string.Format("{0:G} Bot Starting", DateTime.Now));
            IsRunning = true;
            ProcessIntel(); //process intel right away, don't wait for first interval
            timer.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            timer.Stop();
            console.AppendLine(string.Format("{0:G} Bot Stopped", DateTime.Now));
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                timer.Stop(); //stop timing while checking for intel
                ProcessIntel();
            }
            finally
            {
                if (IsRunning) timer.Start(); //verify we didn't stop during the last intel check
            }
        }
    }

    interface IIntelMessagingConsole
    {
        void AppendLine(string s);
        void Append(string s);
    }
}

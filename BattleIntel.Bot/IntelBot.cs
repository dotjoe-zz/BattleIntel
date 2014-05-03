using GSheet;
using GroupMe;
using GroupMe.Models;
using System;
using System.Configuration;
using System.Threading;
using System.Windows.Forms;

namespace BattleIntel.Bot
{
    class IntelBot
    {
        private IIntelMessagingConsole console;
        private System.Timers.Timer timer;

        private int? battleId;
        private GroupMeService groupMe;
        private Group intelRoom;
        private GSheetService google;

        public double TimerInterval
        {
            get
            {
                return timer.Interval;
            }
            set
            {
                timer.Interval = value;
            }
        }
        public bool IsRunning { get; private set; }

        public IntelBot(IIntelMessagingConsole console) 
        { 
            this.console = console;

            this.TimerInterval = 60000;
            this.IsRunning = false;

            this.timer = new System.Timers.Timer(TimerInterval);
            this.timer.SynchronizingObject = console;
            this.timer.Elapsed += timer_Elapsed;

            this.groupMe = new GroupMeService(ConfigurationManager.AppSettings.Get("GroupMe-AccessToken"));
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

            var ss = google.ListSpreadsheets();
            foreach (var s in ss)
            {
                console.AppendLine(s.Title + ": " + s.Url);
            }

            return false;
        }

        private void ProcessIntel()
        {
            console.Append(string.Format("{0:G} checking for new intel...", DateTime.Now));

            Thread.Sleep(1000);
            //console.Append(string.Format("{0:G} parsing new intel...", DateTime.Now));

            //console.Append(string.Format("{0:G} writing new intel to sheet...", DateTime.Now));
            
            console.AppendLine("done");
        }

        private bool IsReadyToRun()
        {
            return intelRoom != null && battleId.HasValue;
        }

        public void Start()
        {
            if (!IsReadyToRun())
            {
                console.AppendLine(string.Format("{0:G} Cannot start Bot until you connect to a battle AND intel room.", DateTime.Now));
                return;
            }

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

    /// <summary>
    /// Used to send messages to the UI and also sync the timer workerthread to the UI thread.
    /// </summary>
    interface IIntelMessagingConsole : System.ComponentModel.ISynchronizeInvoke
    {
        void AppendLine(string s);
        void Append(string s);
    }
}

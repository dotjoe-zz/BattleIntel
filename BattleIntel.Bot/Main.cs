using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleIntel.Bot
{
    public partial class Main : Form, IIntelBotConsole
    {
        private IntelBot Bot;
        private System.Timers.Timer BotTimer;

        public Main()
        {
            InitializeComponent();

            BotTimer = new System.Timers.Timer();
            SetBotTimerInterval();
            BotTimer.Elapsed += Timer_Elapsed;
            BotTimer.SynchronizingObject = this; //keep Bot processing on UI thread

            Bot = new IntelBot(this);
            Bot.PostSheetURL = cbPostSheetUrl.Checked;
            SetStartStopStatus();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Bot.ConnectToBattle(this);
            Bot.ConnectToIntelRoom(this);
            Bot.ConnectToGoogleSheet(this);

            RefreshBotSettings();
            SetStartStopStatus();
        }

        private void RefreshBotSettings()
        {
            var settings = Bot.GetSettings();
            lblBattle.Text = settings.GetBattleDescription();
            lblBattleDates.Text = settings.GetBattleDatesDescription();
            lblGroupMeRoom.Text = settings.GetGroupDescription();
            lblSheet.Text = settings.GetSheetDescription();
            txtSheetURL.Text = settings.SpreadsheetURL;
        }

        private void SetStartStopStatus()
        {
            btnStart.Enabled = !BotTimer.Enabled;
            btnStop.Enabled = BotTimer.Enabled;
        }

        private void SetBotTimerInterval()
        {
            BotTimer.Interval = (double)(nupIntervalSeconds.Value * 1000);
        }

        private void RunBot()
        {
            try
            {
                pnlBotControls.Enabled = false;
                if (Bot.Process())
                {
                    BotTimer.Start();
                }
            }
            finally
            {
                SetStartStopStatus();
                pnlBotControls.Enabled = true;
            }
        }

    #region "Events"

        private void txtSheetURL_Enter(object sender, EventArgs e)
        {
            // Kick off SelectAll asyncronously so that it occurs after Click
            BeginInvoke((Action)delegate
            {
                txtSheetURL.SelectAll();
            });
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            RunBot();          
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            BotTimer.Stop();
            SetStartStopStatus();
        }

        void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BotTimer.Stop();
            RunBot();
        }

        private void cbPostSheetUrl_CheckedChanged(object sender, EventArgs e)
        {
            Bot.PostSheetURL = cbPostSheetUrl.Checked;
        }

        private void btnWriteSheet_Click(object sender, EventArgs e)
        {
            Bot.WriteSheet();
        }

        private void nupIntervalSeconds_ValueChanged(object sender, EventArgs e)
        {
            SetBotTimerInterval();
        }

    #endregion

    #region "IIntelBotConsole"

        public void AppendLine(string s)
        {
            Append(s + Environment.NewLine);
        }

        public void Append(string s)
        {
            txtConsole.AppendText(s);
        }

    #endregion

    }
}

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
            SetTimerInterval();
            BotTimer.Elapsed += Timer_Elapsed;
            BotTimer.SynchronizingObject = this; //keep Bot processing on UI thread

            Bot = new IntelBot(this);
            SetBotControlsStatus();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Bot.ConnectToBattle(this);
            Bot.ConnectToIntelRoom(this);
            Bot.ConnectToGoogleSheet(this);

            SetBotControlsStatus();
        }

        #region "Menu Items"

        private void battleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bot.ConnectToBattle(this);
        }

        private void groupMeRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bot.ConnectToIntelRoom(this);
        }

        private void googleSheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bot.ConnectToGoogleSheet(this);
        }

        #endregion

        #region "Bot Controls"

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;

            if (Bot.Process()) 
            { 
                BotTimer.Start();
            }

            SetBotControlsStatus();            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            BotTimer.Stop();
            SetBotControlsStatus();
        }

        void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BotTimer.Stop();
            if (Bot.Process())
            {
                BotTimer.Start();
            }
            else
            {
                SetBotControlsStatus();
            }
        }

        private void nupIntervalSeconds_ValueChanged(object sender, EventArgs e)
        {
            SetTimerInterval();
        }

        private void SetBotControlsStatus()
        {
            btnStart.Enabled = !BotTimer.Enabled;
            btnStop.Enabled = BotTimer.Enabled;
        }

        private void SetTimerInterval()
        {
            BotTimer.Interval = (double)(nupIntervalSeconds.Value * 1000);
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

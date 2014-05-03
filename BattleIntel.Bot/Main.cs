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
    public partial class Main : Form, IIntelMessagingConsole
    {
        private IntelBot Bot;

        public Main()
        {
            InitializeComponent();

            Bot = new IntelBot(this);
            SetBotControlsStatus();
            SetBotTimerInterval();
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
            Bot.Start();
            SetBotControlsStatus();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Bot.Stop();
            SetBotControlsStatus();
        }

        private void nupIntervalSeconds_ValueChanged(object sender, EventArgs e)
        {
            SetBotTimerInterval();
        }

        private void SetBotControlsStatus()
        {
            btnStart.Enabled = !Bot.IsRunning;
            btnStop.Enabled = Bot.IsRunning;
        }

        private void SetBotTimerInterval()
        {
            Bot.TimerInterval = (double)(nupIntervalSeconds.Value * 1000);
        }

        #endregion

        #region "IIntelMessagingConsole"

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

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
        }

        private void groupMeRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bot.ConnectToIntelRoom();
        }

        public void AppendLine(string s)
        {
           Append(s + Environment.NewLine);
        }

        public void Append(string s)
        {
            if (txtConsole.InvokeRequired)
            {
                txtConsole.Invoke((MethodInvoker)(() => txtConsole.AppendText(s)));
            }
            else 
            { 
                txtConsole.AppendText(s);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Bot.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Bot.Stop();
        }
    }
}

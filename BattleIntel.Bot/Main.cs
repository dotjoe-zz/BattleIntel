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

            Bot = IntelBot.Init(this);
        }

        private void groupMeRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bot.ConnectToIntelRoom();
        }

        public void AppendMessage(string s)
        {
            txtConsole.AppendText(s);
        }
    }
}

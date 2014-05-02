using GroupMe;
using GroupMe.Responses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleIntel.Bot
{
    class IntelBot
    {
        private IIntelMessagingConsole console;
        private GroupMeService groupMe;
        private Group intelRoom;

        private IntelBot() { }

        public static IntelBot Init(IIntelMessagingConsole console)
        {
            var instance = new IntelBot();

            instance.console = console;
            instance.groupMe = new GroupMeService(ConfigurationManager.AppSettings.Get("GroupMe-AccessToken"));

            instance.ConnectToIntelRoom();
            instance.ConnectToGoogleSheet();

            return instance;
        }

        public bool ConnectToIntelRoom()
        {
            using (var gs = new GroupSelector(groupMe))
            {
                if (gs.ShowDialog() == DialogResult.OK)
                {
                    intelRoom = gs.SelectedGroup;
                    console.AppendMessage(string.Format("Connected to Intel Room: {0} (id:{1})", intelRoom.name, intelRoom.id));
                    return true;
                }
            }

            return false;
        }

        public bool ConnectToGoogleSheet()
        {
            return false;
        }
    }

    interface IIntelMessagingConsole
    {
        void AppendMessage(string s);
    }
}

using GroupMe;
using GroupMe.Responses;
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
    public partial class GroupMeRoomSelector : Form
    {
        private GroupMeService service;
        private int page = 1;
        private int perPage = 20;

        public Group SelectedGroup { get; private set; }

        public GroupMeRoomSelector(GroupMeService service)
        {
            this.service = service;
            InitializeComponent();
        }

        private void GroupSelector_Load(object sender, EventArgs e)
        {
            LoadGroups();
        }

        private void btnLoadMore_Click(object sender, EventArgs e)
        {
            page++;
            LoadGroups();
        }

        private void LoadGroups()
        {
            var groups = service.GroupsIndex(page: page, per_page: perPage);

            foreach (var g in groups)
            {
                listBox1.Items.Add(g);
            }

            btnLoadMore.Enabled = (perPage == groups.Count());
            statusLabel.Text = listBox1.Items.Count + " active group(s)";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            SelectedGroup = listBox1.SelectedItem as GroupMe.Responses.Group;

            if (SelectedGroup == null)
            {
                MessageBox.Show(this, "Please select a group.");
                return;
            }
            
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}

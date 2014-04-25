using BattleIntel.Core;
using BattleIntel.DesktopTool.GroupMeForms;
using GroupMe;
using GroupMe.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleIntel.DesktopTool
{
    public partial class StatParserForm : Form
    {
        private IList<Stat> Stats;
        private GroupMeService groupMe;
        private Group intelSourceGroup;

        public StatParserForm()
        {
            InitializeComponent();

            cbCopyForSheetMode.DataSource = Enum.GetValues(typeof(CopyForSheetMode)).Cast<CopyForSheetMode>()
                .Select(x => new { id = (int)x, description = x.ToString() })
                .ToList();
            cbCopyForSheetMode.DisplayMember = "description";
            cbCopyForSheetMode.ValueMember = "id";
            cbCopyForSheetMode.SelectedValue = (int)CopyForSheetMode.Cell;

            groupMe = new GroupMeService(ConfigurationManager.AppSettings.Get("GroupMe-AccessToken"));
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText()) return;
            var lines = GetClipboardLines();
            if (!lines.Any()) return;

            txtTeamName.Text = ConsumeTeamName(ref lines);
            txtTeamStats.Text = string.Join(Environment.NewLine, lines.ToArray());
        }

        private void btnAppend_Click(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText()) return;
            var lines = GetClipboardLines();
            if (!lines.Any()) return;

            //discard an appended team name
            ConsumeTeamName(ref lines);
            
            //append to the stats
            txtTeamStats.Text += Environment.NewLine + string.Join(Environment.NewLine, lines.ToArray());
        }

        private IEnumerable<string> GetClipboardLines()
        {
            return Clipboard.GetText(TextDataFormat.UnicodeText)
                .Trim('"') //trim the quotations from a possible cell copy
                .Split('\n')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim());
        }

        private string ConsumeTeamName(ref IEnumerable<string> lines)
        {
            //check if we copied from a spreadsheet having the team name on every line separated by a tab.
            if (lines.Count() == lines.Where(x => x.Contains('\t')).Count())
            {
                //use it if every line has the same team name
                var teamNames = lines.Select(x => x.Split('\t').First().Trim()).Distinct(StringComparer.OrdinalIgnoreCase);
                if (teamNames.Count() == 1)
                {
                    //remove the team name from all the lines
                    lines = lines.Select(x => string.Join(" ", x.Split('\t').Skip(1).ToArray()));
                    return teamNames.First();
                }
            }
            else
            {
                //check for team name on the first line
                var firstLineStat = Stat.Parse(lines.First());
                if (firstLineStat.Defense == null)
                {
                    lines = lines.Skip(1);
                    return firstLineStat.RawInput;
                }
            }

            return string.Empty;
        }

        private void txtTeamStats_TextChanged(object sender, EventArgs e)
        {
            Stats = txtTeamStats.Lines
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => Stat.Parse(x.Trim()))
                    .Distinct()
                    .OrderByDescending(x => x.Level)
                    .ThenByDescending(x => x.DefenseValue)
                    .ThenBy(x => x.Name)
                    .ToList();

            txtTeamStatsOutput.Text = String.Join(Environment.NewLine, Stats.Select(x => x.ToLine()).ToArray());
        }

        private void btnCopyForScout_Click(object sender, EventArgs e)
        {
            //team name on first line followed by stats
            var teamName = "";
            if(!string.IsNullOrWhiteSpace(txtTeamName.Text))
            {
                teamName = txtTeamName.Text.Trim() + Environment.NewLine;
            }

            //just use the stats ouput text
            var toClip = teamName + txtTeamStatsOutput.Text;
            if (string.IsNullOrEmpty(toClip)) return;
            
            Clipboard.SetText(toClip);
        }

        private void btnCopyForSheet_Click(object sender, EventArgs e)
        {
            if (Stats == null || Stats.Count == 0) return;

            //default team name to a single space
            var teamName = " "; 
            if(!string.IsNullOrWhiteSpace(txtTeamName.Text))
            {
                teamName = txtTeamName.Text.Trim();
            }

            string toClip = null;
            var copyMode = GetCopyForSheetMode();

            if (copyMode == CopyForSheetMode.Cell)
            {
                //repeat the teamName above the stats
                toClip = string.Format("{0}\t\"{0}\n{1}\"", 
                    teamName,  
                    string.Join("\n", Stats.Select(x => x.ToLine())));
            }
            else if (copyMode == CopyForSheetMode.TwoColumns)
            {
                toClip = string.Join(Environment.NewLine, Stats.Select(x => teamName + "\t" + x.ToLine()));
            }
            else if (copyMode == CopyForSheetMode.MultiColumns)
            {
                toClip = string.Join(Environment.NewLine, Stats.Select(x => teamName + "\t" + x.ToLine("\t")));
            }

            if (string.IsNullOrEmpty(toClip)) return;
            
            Clipboard.SetText(toClip);
        }

        private CopyForSheetMode GetCopyForSheetMode()
        {
            return (CopyForSheetMode)cbCopyForSheetMode.SelectedValue;
        }

        private enum CopyForSheetMode
        {
            Cell = 0,
            TwoColumns = 1,
            MultiColumns = 2
        }

        private void groupmeRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var gs = new GroupSelector(groupMe))
            {
                if (gs.ShowDialog() == DialogResult.OK)
                {
                    intelSourceGroup = gs.SelectedGroup;
                }
            }
            UpdateConnectStatusLabel();

            if (intelSourceGroup != null)
            {
                var m = groupMe.PostGroupMessage(intelSourceGroup.id, "Arms Bot say Hi! lol");
                MessageBox.Show(m.name + ": " + m.text);

                var messages = groupMe.GroupMessages(intelSourceGroup.id);
                MessageBox.Show(string.Join("\n", messages.Select(x => x.name + ": " + x.text)));
            }

        }

        private void googleSheetToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void UpdateConnectStatusLabel()
        {
            connectStatus.Text = "Listening: " + intelSourceGroup.name;
        }
    }
}

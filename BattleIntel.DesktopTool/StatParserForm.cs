using BattleIntel.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BattleIntel.DesktopTool
{
    public partial class StatParserForm : Form
    {
        private IList<Stat> Stats;

        public StatParserForm()
        {
            InitializeComponent();

            cbCopyForSheetMode.DataSource = Enum.GetValues(typeof(CopyForSheetMode)).Cast<CopyForSheetMode>()
                .Select(x => new { id = (int)x, description = x.ToString() })
                .ToList();
            cbCopyForSheetMode.DisplayMember = "description";
            cbCopyForSheetMode.ValueMember = "id";
            cbCopyForSheetMode.SelectedValue = (int)CopyForSheetMode.Cell;
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText()) return;
            var lines = GetClipboardLines();
            if (!lines.Any()) return;

            string teamName;
            lines = lines.RemoveTeamName(out teamName);
            txtTeamName.Text = teamName;
            txtTeamStats.Text = string.Join(Environment.NewLine, lines.ToArray());
        }

        private void btnAppend_Click(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText()) return;
            var lines = GetClipboardLines();
            if (!lines.Any()) return;

            //discard an appended team name
            string teamName;
            lines = lines.RemoveTeamName(out teamName);
            
            //append to the stats
            txtTeamStats.Text += Environment.NewLine + string.Join(Environment.NewLine, lines.ToArray());
        }

        private IEnumerable<string> GetClipboardLines()
        {
            return Clipboard.GetText(TextDataFormat.UnicodeText).SplitToNonEmptyLines();
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
    }
}

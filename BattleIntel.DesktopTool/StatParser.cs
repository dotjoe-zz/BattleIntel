using BattleIntel.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleIntel.DesktopTool
{
    public partial class StatParser : Form
    {
        public StatParser()
        {
            InitializeComponent();
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                var t = Clipboard.GetText(TextDataFormat.UnicodeText);

                //check for team name on the first line
                var lines = t.Split('\n')
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim());

                if (!lines.Any()) return;

                //check for a team name on every line separated with tab (i.e. copied from sheet)
                if (lines.Count() == lines.Where(x => x.Contains('\t')).Count())
                {
                    txtTeamName.Text = lines.First().Split('\t').First().Trim();
                    lines = lines.Select(x => string.Join(" ", x.Split('\t').Skip(1).ToArray()));
                }
                else
                {
                    //check for team name on the first line
                    var firstLineStat = BattleStat.Parse(lines.First());
                    if (firstLineStat.Level == null && firstLineStat.Defense == null)
                    {
                        txtTeamName.Text = firstLineStat.Name;
                        lines = lines.Skip(1);
                    }
                    else
                    {
                        txtTeamName.Text = "";
                    }
                }
                
                txtTeamStats.Text = string.Join("\r\n", lines.ToArray());
            }
        }

        private void btnAppend_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                //append to the stats
                txtTeamStats.Text += "\r\n" + Clipboard.GetText(TextDataFormat.UnicodeText);
            }
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            var t = txtTeamStats.Text;

            var stats = t.Split('\n')
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => BattleStat.Parse(x.Trim()))
                    .OrderByDescending(x => x.Level)
                    .ThenBy(x => x.Name)
                    .ThenBy(x => x.Defense);

            txtTeamStatsOutput.Text = String.Join("\r\n", stats.Select(x => x.ToString()).ToArray());
        }

        private void btnCopyForScout_Click(object sender, EventArgs e)
        {
            //team name on first line followed by stats
            var teamName = "";
            if(!string.IsNullOrWhiteSpace(txtTeamName.Text))
            {
                teamName = txtTeamName.Text.Trim() + "\r\n";
            }

            Clipboard.SetText(teamName + txtTeamStatsOutput.Text);

        }

        private void btnCopyForSheet_Click(object sender, EventArgs e)
        {
            var teamName = "";
            if(!string.IsNullOrWhiteSpace(txtTeamName.Text))
            {
                teamName = txtTeamName.Text.Trim() + "\t";
            }

            //team name on every line with a tab separator
            var lines = txtTeamStatsOutput.Text.Split('\n')
                .Select(x => teamName + x.Trim());

            Clipboard.SetText(string.Join("\r\n", lines.ToArray()));
        }
    }
}

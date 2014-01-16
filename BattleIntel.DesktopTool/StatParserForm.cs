﻿using BattleIntel.Core;
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
    public partial class StatParserForm : Form
    {
        public StatParserForm()
        {
            InitializeComponent();
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText()) return;

            var lines = Clipboard.GetText(TextDataFormat.UnicodeText).Split('\n')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim());

            if (!lines.Any()) return;

            txtTeamName.Text = "";

            //check if we copied from a spreadsheet having the team name on every line separated by a tab.
            if (lines.Count() == lines.Where(x => x.Contains('\t')).Count())
            {
                //use it if every line has the same team name
                var teamNames = lines.Select(x => x.Split('\t').First().Trim()).Distinct(StringComparer.OrdinalIgnoreCase);
                if (teamNames.Count() == 1)
                {
                    txtTeamName.Text = teamNames.First();
                    //remove the team name from all the lines
                    lines = lines.Select(x => string.Join(" ", x.Split('\t').Skip(1).ToArray()));
                }
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
            }

            txtTeamStats.Text = string.Join(Environment.NewLine, lines.ToArray());
        }

        private void btnAppend_Click(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText()) return;
            
            //append to the stats
            txtTeamStats.Text += Environment.NewLine + Clipboard.GetText(TextDataFormat.UnicodeText);
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            var parsedStatLines = txtTeamStats.Lines
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => BattleStat.Parse(x.Trim()))
                    .Distinct()
                    .OrderByDescending(x => x.Level)
                    .ThenBy(x => x.Name)
                    .ThenBy(x => x.Defense)
                    .Select(x => x.ToString());

            txtTeamStatsOutput.Text = String.Join(Environment.NewLine, parsedStatLines.ToArray());
        }

        private void btnCopyForScout_Click(object sender, EventArgs e)
        {
            //team name on first line followed by stats
            var teamName = "";
            if(!string.IsNullOrWhiteSpace(txtTeamName.Text))
            {
                teamName = txtTeamName.Text.Trim() + Environment.NewLine;
            }

            var toClip = teamName + txtTeamStatsOutput.Text;
            if (string.IsNullOrEmpty(toClip)) return;
            
            Clipboard.SetText(toClip);
        }

        private void btnCopyForSheet_Click(object sender, EventArgs e)
        {
            //team name on every line with a tab separator
            var teamName = "";
            if(!string.IsNullOrWhiteSpace(txtTeamName.Text))
            {
                teamName = txtTeamName.Text.Trim() + "\t";
            }
            
            var lines = txtTeamStatsOutput.Lines
                .Select(x => teamName + x.Trim());

            var toClip = string.Join(Environment.NewLine, lines.ToArray());
            if (string.IsNullOrEmpty(toClip)) return;
            
            Clipboard.SetText(toClip);
        }
    }
}
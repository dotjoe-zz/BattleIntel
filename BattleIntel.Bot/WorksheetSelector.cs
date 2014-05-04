using GSheet;
using GSheet.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleIntel.Bot
{
    public partial class WorksheetSelector : Form
    {
        private readonly GSheetService google;

        public SpreadsheetModel SelectedSpreadsheet;
        public WorksheetModel SelectedWorksheet;
        
        public WorksheetSelector(GSheetService google)
        {
            InitializeComponent();

            this.google = google;
        }

        private void WorksheetSelector_Load(object sender, EventArgs e)
        {
            var spreadsheetNodes = google.ListSpreadsheets()
                .Select(x => 
                {
                    var n = new TreeNode
                    {
                        Text = x.Title,
                        Tag = x
                    };
                    n.Nodes.Add(new TreeNode { Text = "Loading Worksheets...", Tag = "placeholder" });
                    return n;
                }).ToArray();

            treeView1.Nodes.AddRange(spreadsheetNodes);
        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Nodes.Count == 1 && (e.Node.Nodes[0].Tag as string) == "placeholder")
            {
                var spreadsheet = e.Node.Tag as SpreadsheetModel;

                ThreadPool.QueueUserWorkItem(state =>
                {
                    var worksheetNodes = google.ListWorksheets(spreadsheet.WorksheetsFeedURI)
                        .Select(x => new TreeNode
                        {
                            Text = x.Title,
                            Tag = x
                        }).ToArray();

                    treeView1.BeginInvoke((Action)delegate
                    {
                        e.Node.Nodes.RemoveAt(0); //remove the placeholder
                        e.Node.Nodes.AddRange(worksheetNodes);
                    });
                });
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var n = treeView1.SelectedNode;
            if (n != null) SelectedWorksheet = n.Tag as WorksheetModel;

            if (SelectedWorksheet == null)
            {
                MessageBox.Show("Please select a worksheet");
                return;
            }
            else
            {
                SelectedSpreadsheet = n.Parent.Tag as SpreadsheetModel;
            }
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}

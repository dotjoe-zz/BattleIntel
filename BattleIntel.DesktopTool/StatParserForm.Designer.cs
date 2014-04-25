namespace BattleIntel.DesktopTool
{
    partial class StatParserForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnAppend = new System.Windows.Forms.Button();
            this.txtTeamName = new System.Windows.Forms.TextBox();
            this.btnPaste = new System.Windows.Forms.Button();
            this.txtTeamStats = new System.Windows.Forms.TextBox();
            this.cbCopyForSheetMode = new System.Windows.Forms.ComboBox();
            this.btnCopyForSheet = new System.Windows.Forms.Button();
            this.btnCopyForScout = new System.Windows.Forms.Button();
            this.txtTeamStatsOutput = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.groupmeRoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.googleSheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectStatus = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnAppend);
            this.splitContainer1.Panel1.Controls.Add(this.txtTeamName);
            this.splitContainer1.Panel1.Controls.Add(this.btnPaste);
            this.splitContainer1.Panel1.Controls.Add(this.txtTeamStats);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.cbCopyForSheetMode);
            this.splitContainer1.Panel2.Controls.Add(this.btnCopyForSheet);
            this.splitContainer1.Panel2.Controls.Add(this.btnCopyForScout);
            this.splitContainer1.Panel2.Controls.Add(this.txtTeamStatsOutput);
            this.splitContainer1.Size = new System.Drawing.Size(434, 403);
            this.splitContainer1.SplitterDistance = 215;
            this.splitContainer1.TabIndex = 0;
            // 
            // btnAppend
            // 
            this.btnAppend.Location = new System.Drawing.Point(53, 3);
            this.btnAppend.Name = "btnAppend";
            this.btnAppend.Size = new System.Drawing.Size(53, 23);
            this.btnAppend.TabIndex = 3;
            this.btnAppend.Text = "Append";
            this.btnAppend.UseVisualStyleBackColor = true;
            this.btnAppend.Click += new System.EventHandler(this.btnAppend_Click);
            // 
            // txtTeamName
            // 
            this.txtTeamName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTeamName.Location = new System.Drawing.Point(3, 31);
            this.txtTeamName.Name = "txtTeamName";
            this.txtTeamName.Size = new System.Drawing.Size(209, 20);
            this.txtTeamName.TabIndex = 0;
            this.toolTip1.SetToolTip(this.txtTeamName, "Team Name (optional)");
            // 
            // btnPaste
            // 
            this.btnPaste.Location = new System.Drawing.Point(3, 3);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(44, 23);
            this.btnPaste.TabIndex = 2;
            this.btnPaste.Text = "Paste";
            this.btnPaste.UseVisualStyleBackColor = true;
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // txtTeamStats
            // 
            this.txtTeamStats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTeamStats.Location = new System.Drawing.Point(3, 58);
            this.txtTeamStats.Multiline = true;
            this.txtTeamStats.Name = "txtTeamStats";
            this.txtTeamStats.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTeamStats.Size = new System.Drawing.Size(209, 342);
            this.txtTeamStats.TabIndex = 1;
            this.txtTeamStats.TextChanged += new System.EventHandler(this.txtTeamStats_TextChanged);
            // 
            // cbCopyForSheetMode
            // 
            this.cbCopyForSheetMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCopyForSheetMode.FormattingEnabled = true;
            this.cbCopyForSheetMode.Location = new System.Drawing.Point(97, 31);
            this.cbCopyForSheetMode.Name = "cbCopyForSheetMode";
            this.cbCopyForSheetMode.Size = new System.Drawing.Size(114, 21);
            this.cbCopyForSheetMode.TabIndex = 3;
            // 
            // btnCopyForSheet
            // 
            this.btnCopyForSheet.Location = new System.Drawing.Point(3, 29);
            this.btnCopyForSheet.Name = "btnCopyForSheet";
            this.btnCopyForSheet.Size = new System.Drawing.Size(88, 23);
            this.btnCopyForSheet.TabIndex = 1;
            this.btnCopyForSheet.Text = "Copy for Sheet";
            this.btnCopyForSheet.UseVisualStyleBackColor = true;
            this.btnCopyForSheet.Click += new System.EventHandler(this.btnCopyForSheet_Click);
            // 
            // btnCopyForScout
            // 
            this.btnCopyForScout.Location = new System.Drawing.Point(3, 3);
            this.btnCopyForScout.Name = "btnCopyForScout";
            this.btnCopyForScout.Size = new System.Drawing.Size(88, 23);
            this.btnCopyForScout.TabIndex = 0;
            this.btnCopyForScout.Text = "Copy for Scout";
            this.btnCopyForScout.UseVisualStyleBackColor = true;
            this.btnCopyForScout.Click += new System.EventHandler(this.btnCopyForScout_Click);
            // 
            // txtTeamStatsOutput
            // 
            this.txtTeamStatsOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTeamStatsOutput.Location = new System.Drawing.Point(3, 58);
            this.txtTeamStatsOutput.Multiline = true;
            this.txtTeamStatsOutput.Name = "txtTeamStatsOutput";
            this.txtTeamStatsOutput.ReadOnly = true;
            this.txtTeamStatsOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTeamStatsOutput.Size = new System.Drawing.Size(209, 342);
            this.txtTeamStatsOutput.TabIndex = 2;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.connectStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 406);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(434, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.googleSheetToolStripMenuItem,
            this.groupmeRoomToolStripMenuItem});
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(65, 20);
            this.toolStripDropDownButton1.Text = "Connect";
            // 
            // groupmeRoomToolStripMenuItem
            // 
            this.groupmeRoomToolStripMenuItem.Name = "groupmeRoomToolStripMenuItem";
            this.groupmeRoomToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.groupmeRoomToolStripMenuItem.Text = "Groupme Room";
            this.groupmeRoomToolStripMenuItem.Click += new System.EventHandler(this.groupmeRoomToolStripMenuItem_Click);
            // 
            // googleSheetToolStripMenuItem
            // 
            this.googleSheetToolStripMenuItem.Name = "googleSheetToolStripMenuItem";
            this.googleSheetToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.googleSheetToolStripMenuItem.Text = "Google Sheet";
            this.googleSheetToolStripMenuItem.Click += new System.EventHandler(this.googleSheetToolStripMenuItem_Click);
            // 
            // connectStatus
            // 
            this.connectStatus.Name = "connectStatus";
            this.connectStatus.Size = new System.Drawing.Size(88, 17);
            this.connectStatus.Text = "Not Connected";
            // 
            // StatParserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 428);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(450, 420);
            this.Name = "StatParserForm";
            this.Text = "Stat Parser v0.4";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtTeamName;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.TextBox txtTeamStats;
        private System.Windows.Forms.Button btnCopyForSheet;
        private System.Windows.Forms.Button btnCopyForScout;
        private System.Windows.Forms.TextBox txtTeamStatsOutput;
        private System.Windows.Forms.Button btnAppend;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox cbCopyForSheetMode;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem googleSheetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem groupmeRoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel connectStatus;
    }
}
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
            this.btnParse = new System.Windows.Forms.Button();
            this.txtTeamName = new System.Windows.Forms.TextBox();
            this.btnPaste = new System.Windows.Forms.Button();
            this.txtTeamStats = new System.Windows.Forms.TextBox();
            this.cbCopyForSheetMode = new System.Windows.Forms.ComboBox();
            this.btnCopyForSheet = new System.Windows.Forms.Button();
            this.btnCopyForScout = new System.Windows.Forms.Button();
            this.txtTeamStatsOutput = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnAppend);
            this.splitContainer1.Panel1.Controls.Add(this.btnParse);
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
            this.splitContainer1.Size = new System.Drawing.Size(434, 381);
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
            // btnParse
            // 
            this.btnParse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnParse.Location = new System.Drawing.Point(158, 3);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(54, 23);
            this.btnParse.TabIndex = 4;
            this.btnParse.Text = "Parse";
            this.btnParse.UseVisualStyleBackColor = true;
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
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
            this.txtTeamStats.Size = new System.Drawing.Size(209, 320);
            this.txtTeamStats.TabIndex = 1;
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
            this.txtTeamStatsOutput.Size = new System.Drawing.Size(209, 320);
            this.txtTeamStatsOutput.TabIndex = 2;
            // 
            // StatParserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 381);
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(450, 420);
            this.Name = "StatParserForm";
            this.Text = "Stat Parser";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnParse;
        private System.Windows.Forms.TextBox txtTeamName;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.TextBox txtTeamStats;
        private System.Windows.Forms.Button btnCopyForSheet;
        private System.Windows.Forms.Button btnCopyForScout;
        private System.Windows.Forms.TextBox txtTeamStatsOutput;
        private System.Windows.Forms.Button btnAppend;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox cbCopyForSheetMode;
    }
}
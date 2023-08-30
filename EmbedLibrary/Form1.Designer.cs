namespace EmbedLibrary
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lbLibrarys = new System.Windows.Forms.ListBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLatestRelease = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSourceCode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.chkAutomatically = new System.Windows.Forms.CheckBox();
            this.chkWritePdb = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbLibrarys
            // 
            this.lbLibrarys.AllowDrop = true;
            this.lbLibrarys.FormattingEnabled = true;
            this.lbLibrarys.Location = new System.Drawing.Point(12, 76);
            this.lbLibrarys.Name = "lbLibrarys";
            this.lbLibrarys.ScrollAlwaysVisible = true;
            this.lbLibrarys.Size = new System.Drawing.Size(323, 160);
            this.lbLibrarys.TabIndex = 0;
            this.lbLibrarys.DragDrop += new System.Windows.Forms.DragEventHandler(this.lbLibrarys_DragDrop);
            this.lbLibrarys.DragEnter += new System.Windows.Forms.DragEventHandler(this.lbLibrarys_DragEnter);
            // 
            // txtName
            // 
            this.txtName.AllowDrop = true;
            this.txtName.Location = new System.Drawing.Point(12, 27);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(242, 20);
            this.txtName.TabIndex = 1;
            this.txtName.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtName_DragDrop);
            this.txtName.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtName_DragEnter);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(260, 25);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Enabled = false;
            this.btnAdd.Location = new System.Drawing.Point(12, 242);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnClear
            // 
            this.btnClear.Enabled = false;
            this.btnClear.Location = new System.Drawing.Point(93, 242);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 4;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(260, 242);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 5;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(347, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuLatestRelease,
            this.menuSourceCode,
            this.toolStripSeparator1,
            this.menuAbout});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // menuLatestRelease
            // 
            this.menuLatestRelease.Name = "menuLatestRelease";
            this.menuLatestRelease.Size = new System.Drawing.Size(180, 22);
            this.menuLatestRelease.Text = "Latest Release";
            this.menuLatestRelease.Click += new System.EventHandler(this.menuLatestRelease_Click);
            // 
            // menuSourceCode
            // 
            this.menuSourceCode.Name = "menuSourceCode";
            this.menuSourceCode.Size = new System.Drawing.Size(180, 22);
            this.menuSourceCode.Text = "Source Code";
            this.menuSourceCode.Click += new System.EventHandler(this.menuSourceCode_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // menuAbout
            // 
            this.menuAbout.Name = "menuAbout";
            this.menuAbout.Size = new System.Drawing.Size(180, 22);
            this.menuAbout.Text = "About";
            this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
            // 
            // chkAutomatically
            // 
            this.chkAutomatically.AutoSize = true;
            this.chkAutomatically.Checked = true;
            this.chkAutomatically.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutomatically.Location = new System.Drawing.Point(12, 53);
            this.chkAutomatically.Name = "chkAutomatically";
            this.chkAutomatically.Size = new System.Drawing.Size(88, 17);
            this.chkAutomatically.TabIndex = 7;
            this.chkAutomatically.Text = "Automatically";
            this.chkAutomatically.UseVisualStyleBackColor = true;
            this.chkAutomatically.CheckedChanged += new System.EventHandler(this.chkAutomatically_CheckedChanged);
            // 
            // chkWritePdb
            // 
            this.chkWritePdb.AutoSize = true;
            this.chkWritePdb.Checked = true;
            this.chkWritePdb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWritePdb.Location = new System.Drawing.Point(106, 53);
            this.chkWritePdb.Name = "chkWritePdb";
            this.chkWritePdb.Size = new System.Drawing.Size(73, 17);
            this.chkWritePdb.TabIndex = 8;
            this.chkWritePdb.Text = "Write Pdb";
            this.chkWritePdb.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 277);
            this.Controls.Add(this.chkWritePdb);
            this.Controls.Add(this.chkAutomatically);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lbLibrarys);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Embed Library .Net";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbLibrarys;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuLatestRelease;
        private System.Windows.Forms.ToolStripMenuItem menuSourceCode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuAbout;
        private System.Windows.Forms.CheckBox chkAutomatically;
        private System.Windows.Forms.CheckBox chkWritePdb;
    }
}


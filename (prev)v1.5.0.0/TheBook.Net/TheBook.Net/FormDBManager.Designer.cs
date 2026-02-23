namespace TheBook.Net
{
    partial class FormDBManager
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDBManager));
            tabControlMain = new TabControl();
            tabPageNonVHDDB = new TabPage();
            pictureBox1 = new PictureBox();
            buttonUpgradeNVDB = new Button();
            buttonCloseNVDB = new Button();
            buttonLaunchNVDB = new Button();
            buttonOpenNVDB = new Button();
            checkBoxWriteLock = new CheckBox();
            buttonNewNonVHDDB = new Button();
            txtBoxDBPath = new TextBox();
            label10 = new Label();
            menuStripMain = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripMenuItem();
            toolStripMenuItem4 = new ToolStripMenuItem();
            toolStripMenuItem5 = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            tabControlMain.SuspendLayout();
            tabPageNonVHDDB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            menuStripMain.SuspendLayout();
            SuspendLayout();
            // 
            // tabControlMain
            // 
            tabControlMain.Controls.Add(tabPageNonVHDDB);
            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.Location = new Point(0, 29);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(784, 232);
            tabControlMain.TabIndex = 0;
            // 
            // tabPageNonVHDDB
            // 
            tabPageNonVHDDB.Controls.Add(pictureBox1);
            tabPageNonVHDDB.Controls.Add(buttonUpgradeNVDB);
            tabPageNonVHDDB.Controls.Add(buttonCloseNVDB);
            tabPageNonVHDDB.Controls.Add(buttonLaunchNVDB);
            tabPageNonVHDDB.Controls.Add(buttonOpenNVDB);
            tabPageNonVHDDB.Controls.Add(checkBoxWriteLock);
            tabPageNonVHDDB.Controls.Add(buttonNewNonVHDDB);
            tabPageNonVHDDB.Controls.Add(txtBoxDBPath);
            tabPageNonVHDDB.Controls.Add(label10);
            tabPageNonVHDDB.Location = new Point(4, 30);
            tabPageNonVHDDB.Name = "tabPageNonVHDDB";
            tabPageNonVHDDB.Size = new Size(776, 198);
            tabPageNonVHDDB.TabIndex = 4;
            tabPageNonVHDDB.Text = "local database";
            tabPageNonVHDDB.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(8, 130);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(64, 64);
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.TabIndex = 25;
            pictureBox1.TabStop = false;
            // 
            // buttonUpgradeNVDB
            // 
            buttonUpgradeNVDB.Location = new Point(321, 130);
            buttonUpgradeNVDB.Name = "buttonUpgradeNVDB";
            buttonUpgradeNVDB.Size = new Size(150, 50);
            buttonUpgradeNVDB.TabIndex = 7;
            buttonUpgradeNVDB.Text = "upgrade/fix";
            buttonUpgradeNVDB.UseVisualStyleBackColor = true;
            buttonUpgradeNVDB.Click += buttonURNVDB_Click;
            // 
            // buttonCloseNVDB
            // 
            buttonCloseNVDB.Location = new Point(477, 130);
            buttonCloseNVDB.Name = "buttonCloseNVDB";
            buttonCloseNVDB.Size = new Size(110, 50);
            buttonCloseNVDB.TabIndex = 6;
            buttonCloseNVDB.Text = "close db";
            buttonCloseNVDB.UseVisualStyleBackColor = true;
            buttonCloseNVDB.Click += buttonCloseNVDB_Click;
            // 
            // buttonLaunchNVDB
            // 
            buttonLaunchNVDB.Location = new Point(593, 124);
            buttonLaunchNVDB.Name = "buttonLaunchNVDB";
            buttonLaunchNVDB.Size = new Size(150, 50);
            buttonLaunchNVDB.TabIndex = 5;
            buttonLaunchNVDB.Text = "launch";
            buttonLaunchNVDB.UseVisualStyleBackColor = true;
            buttonLaunchNVDB.Click += buttonLaunchNVDB_Click;
            // 
            // buttonOpenNVDB
            // 
            buttonOpenNVDB.Location = new Point(593, 68);
            buttonOpenNVDB.Name = "buttonOpenNVDB";
            buttonOpenNVDB.Size = new Size(150, 50);
            buttonOpenNVDB.TabIndex = 4;
            buttonOpenNVDB.Text = "open db";
            buttonOpenNVDB.UseVisualStyleBackColor = true;
            buttonOpenNVDB.Click += buttonOpenNVDB_Click;
            // 
            // checkBoxWriteLock
            // 
            checkBoxWriteLock.AutoSize = true;
            checkBoxWriteLock.Checked = true;
            checkBoxWriteLock.CheckState = CheckState.Checked;
            checkBoxWriteLock.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBoxWriteLock.Location = new Point(437, 2);
            checkBoxWriteLock.Name = "checkBoxWriteLock";
            checkBoxWriteLock.Size = new Size(150, 21);
            checkBoxWriteLock.TabIndex = 1;
            checkBoxWriteLock.Text = "read only / write lock";
            checkBoxWriteLock.UseVisualStyleBackColor = true;
            checkBoxWriteLock.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // buttonNewNonVHDDB
            // 
            buttonNewNonVHDDB.Location = new Point(593, 12);
            buttonNewNonVHDDB.Name = "buttonNewNonVHDDB";
            buttonNewNonVHDDB.Size = new Size(150, 50);
            buttonNewNonVHDDB.TabIndex = 3;
            buttonNewNonVHDDB.Text = "new db";
            buttonNewNonVHDDB.UseVisualStyleBackColor = true;
            buttonNewNonVHDDB.Click += buttonNewNonVHDDB_Click;
            // 
            // txtBoxDBPath
            // 
            txtBoxDBPath.Location = new Point(3, 24);
            txtBoxDBPath.Multiline = true;
            txtBoxDBPath.Name = "txtBoxDBPath";
            txtBoxDBPath.ReadOnly = true;
            txtBoxDBPath.Size = new Size(584, 100);
            txtBoxDBPath.TabIndex = 2;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(3, 0);
            label10.Name = "label10";
            label10.Size = new Size(146, 21);
            label10.TabIndex = 18;
            label10.Text = "local database path:";
            // 
            // menuStripMain
            // 
            menuStripMain.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            menuStripMain.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStripMain.Location = new Point(0, 0);
            menuStripMain.Name = "menuStripMain";
            menuStripMain.Size = new Size(784, 29);
            menuStripMain.TabIndex = 1;
            menuStripMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem2, toolStripMenuItem3, toolStripMenuItem4, toolStripMenuItem5, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 25);
            fileToolStripMenuItem.Text = "&File";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.ShortcutKeys = Keys.Alt | Keys.N;
            toolStripMenuItem1.Size = new Size(211, 26);
            toolStripMenuItem1.Text = "New DB";
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.ShortcutKeys = Keys.Alt | Keys.O;
            toolStripMenuItem2.Size = new Size(211, 26);
            toolStripMenuItem2.Text = "Open DB";
            toolStripMenuItem2.Click += toolStripMenuItem2_Click;
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.ShortcutKeys = Keys.Alt | Keys.F5;
            toolStripMenuItem3.Size = new Size(211, 26);
            toolStripMenuItem3.Text = "Launch DB";
            toolStripMenuItem3.Click += toolStripMenuItem3_Click;
            // 
            // toolStripMenuItem4
            // 
            toolStripMenuItem4.Name = "toolStripMenuItem4";
            toolStripMenuItem4.ShortcutKeys = Keys.Alt | Keys.F11;
            toolStripMenuItem4.Size = new Size(211, 26);
            toolStripMenuItem4.Text = "Close DB";
            toolStripMenuItem4.Click += toolStripMenuItem4_Click;
            // 
            // toolStripMenuItem5
            // 
            toolStripMenuItem5.Name = "toolStripMenuItem5";
            toolStripMenuItem5.Size = new Size(211, 26);
            toolStripMenuItem5.Text = "Upgrade/Fix DB";
            toolStripMenuItem5.Click += toolStripMenuItem5_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(208, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(211, 26);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // FormDBManager
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(784, 261);
            Controls.Add(tabControlMain);
            Controls.Add(menuStripMain);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStripMain;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormDBManager";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "hub and database manager";
            FormClosing += FormDBManager_FormClosing;
            Load += Form1_Load;
            tabControlMain.ResumeLayout(false);
            tabPageNonVHDDB.ResumeLayout(false);
            tabPageNonVHDDB.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            menuStripMain.ResumeLayout(false);
            menuStripMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TabControl tabControlMain;
        private MenuStrip menuStripMain;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private RadioButton radioButton3;
        private RadioButton radioButton4;
        private TextBox txtBoxDBPath;
        private Label label10;
        private TabPage tabPageNonVHDDB;
        private CheckBox checkBoxWriteLock;
        private Button buttonNewNonVHDDB;
        private Button buttonOpenNVDB;
        private Button buttonLaunchNVDB;
        private Button buttonCloseNVDB;
        private Button buttonUpgradeNVDB;
        private PictureBox pictureBox1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem toolStripMenuItem5;
        private ToolStripSeparator toolStripSeparator1;
    }
}

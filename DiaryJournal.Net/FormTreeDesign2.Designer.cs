namespace DiaryJournal.Net
{
    partial class FormTreeDesign2
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
            buttonOK = new Button();
            buttonCancel = new Button();
            splitContainer1 = new SplitContainer();
            splitContainerHInner1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            textboxPath = new TextBox();
            txtEntryTitle = new TextBox();
            splitContainer3 = new SplitContainer();
            lvCurrentPath = new ListView();
            columnHeader9 = new ColumnHeader();
            columnHeader11 = new ColumnHeader();
            lvChildren = new ListView();
            columnHeader12 = new ColumnHeader();
            columnHeader13 = new ColumnHeader();
            lvChildsChildren = new ListView();
            columnHeader14 = new ColumnHeader();
            columnHeader15 = new ColumnHeader();
            buttonReload = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerHInner1).BeginInit();
            splitContainerHInner1.Panel1.SuspendLayout();
            splitContainerHInner1.Panel2.SuspendLayout();
            splitContainerHInner1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            SuspendLayout();
            // 
            // buttonOK
            // 
            buttonOK.DialogResult = DialogResult.OK;
            buttonOK.Location = new Point(922, 30);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(150, 30);
            buttonOK.TabIndex = 1;
            buttonOK.Text = "&OK";
            buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(766, 30);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(150, 30);
            buttonCancel.TabIndex = 2;
            buttonCancel.Text = "&Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainerHInner1);
            splitContainer1.Panel1MinSize = 400;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(buttonReload);
            splitContainer1.Panel2.Controls.Add(buttonOK);
            splitContainer1.Panel2.Controls.Add(buttonCancel);
            splitContainer1.Panel2MinSize = 60;
            splitContainer1.Size = new Size(1084, 661);
            splitContainer1.SplitterDistance = 585;
            splitContainer1.TabIndex = 4;
            // 
            // splitContainerHInner1
            // 
            splitContainerHInner1.BorderStyle = BorderStyle.FixedSingle;
            splitContainerHInner1.Dock = DockStyle.Fill;
            splitContainerHInner1.Location = new Point(0, 0);
            splitContainerHInner1.MinimumSize = new Size(60, 400);
            splitContainerHInner1.Name = "splitContainerHInner1";
            splitContainerHInner1.Orientation = Orientation.Horizontal;
            // 
            // splitContainerHInner1.Panel1
            // 
            splitContainerHInner1.Panel1.Controls.Add(splitContainer2);
            splitContainerHInner1.Panel1MinSize = 60;
            // 
            // splitContainerHInner1.Panel2
            // 
            splitContainerHInner1.Panel2.Controls.Add(lvChildsChildren);
            splitContainerHInner1.Panel2MinSize = 250;
            splitContainerHInner1.Size = new Size(1084, 585);
            splitContainerHInner1.SplitterDistance = 330;
            splitContainerHInner1.SplitterWidth = 5;
            splitContainerHInner1.TabIndex = 1;
            // 
            // splitContainer2
            // 
            splitContainer2.BorderStyle = BorderStyle.FixedSingle;
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.FixedPanel = FixedPanel.Panel1;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(textboxPath);
            splitContainer2.Panel1.Controls.Add(txtEntryTitle);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(splitContainer3);
            splitContainer2.Size = new Size(1084, 330);
            splitContainer2.SplitterWidth = 5;
            splitContainer2.TabIndex = 0;
            // 
            // textboxPath
            // 
            textboxPath.Dock = DockStyle.Top;
            textboxPath.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textboxPath.Location = new Point(0, 22);
            textboxPath.Name = "textboxPath";
            textboxPath.Size = new Size(1082, 27);
            textboxPath.TabIndex = 2;
            textboxPath.Text = "\\0";
            // 
            // txtEntryTitle
            // 
            txtEntryTitle.Dock = DockStyle.Top;
            txtEntryTitle.Font = new Font("Segoe UI", 8.25F);
            txtEntryTitle.Location = new Point(0, 0);
            txtEntryTitle.Name = "txtEntryTitle";
            txtEntryTitle.Size = new Size(1082, 22);
            txtEntryTitle.TabIndex = 1;
            txtEntryTitle.Text = "unique title (mandatory for identification)";
            // 
            // splitContainer3
            // 
            splitContainer3.Dock = DockStyle.Fill;
            splitContainer3.Location = new Point(0, 0);
            splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(lvCurrentPath);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(lvChildren);
            splitContainer3.Size = new Size(1082, 273);
            splitContainer3.SplitterDistance = 550;
            splitContainer3.TabIndex = 120;
            // 
            // lvCurrentPath
            // 
            lvCurrentPath.CheckBoxes = true;
            lvCurrentPath.Columns.AddRange(new ColumnHeader[] { columnHeader9, columnHeader11 });
            lvCurrentPath.Dock = DockStyle.Fill;
            lvCurrentPath.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lvCurrentPath.FullRowSelect = true;
            lvCurrentPath.GridLines = true;
            lvCurrentPath.Location = new Point(0, 0);
            lvCurrentPath.Name = "lvCurrentPath";
            lvCurrentPath.Size = new Size(550, 273);
            lvCurrentPath.TabIndex = 118;
            lvCurrentPath.UseCompatibleStateImageBehavior = false;
            lvCurrentPath.View = View.Details;
            lvCurrentPath.VirtualListSize = 100000;
            lvCurrentPath.SelectedIndexChanged += lvCurrentPath_SelectedIndexChanged;
            lvCurrentPath.Click += lvCurrentPath_Click;
            lvCurrentPath.DoubleClick += lvCurrentPath_DoubleClick;
            // 
            // columnHeader9
            // 
            columnHeader9.Text = "index";
            columnHeader9.Width = 70;
            // 
            // columnHeader11
            // 
            columnHeader11.Text = "nodes";
            columnHeader11.Width = 2500;
            // 
            // lvChildren
            // 
            lvChildren.CheckBoxes = true;
            lvChildren.Columns.AddRange(new ColumnHeader[] { columnHeader12, columnHeader13 });
            lvChildren.Dock = DockStyle.Fill;
            lvChildren.Font = new Font("Segoe UI", 9.75F);
            lvChildren.FullRowSelect = true;
            lvChildren.GridLines = true;
            lvChildren.Location = new Point(0, 0);
            lvChildren.Name = "lvChildren";
            lvChildren.Size = new Size(528, 273);
            lvChildren.TabIndex = 119;
            lvChildren.UseCompatibleStateImageBehavior = false;
            lvChildren.View = View.Details;
            lvChildren.VirtualListSize = 100000;
            lvChildren.SelectedIndexChanged += lvChildren_SelectedIndexChanged;
            lvChildren.Click += lvChildren_Click;
            lvChildren.DoubleClick += lvChildren_DoubleClick;
            // 
            // columnHeader12
            // 
            columnHeader12.Text = "index";
            columnHeader12.Width = 70;
            // 
            // columnHeader13
            // 
            columnHeader13.Text = "children nodes";
            columnHeader13.Width = 2500;
            // 
            // lvChildsChildren
            // 
            lvChildsChildren.CheckBoxes = true;
            lvChildsChildren.Columns.AddRange(new ColumnHeader[] { columnHeader14, columnHeader15 });
            lvChildsChildren.Dock = DockStyle.Fill;
            lvChildsChildren.Font = new Font("Segoe UI", 9.75F);
            lvChildsChildren.FullRowSelect = true;
            lvChildsChildren.GridLines = true;
            lvChildsChildren.Location = new Point(0, 0);
            lvChildsChildren.Name = "lvChildsChildren";
            lvChildsChildren.Size = new Size(1082, 248);
            lvChildsChildren.TabIndex = 120;
            lvChildsChildren.UseCompatibleStateImageBehavior = false;
            lvChildsChildren.View = View.Details;
            lvChildsChildren.VirtualListSize = 100000;
            lvChildsChildren.DoubleClick += lvChildsChildren_DoubleClick;
            // 
            // columnHeader14
            // 
            columnHeader14.Text = "index";
            columnHeader14.Width = 70;
            // 
            // columnHeader15
            // 
            columnHeader15.Text = "grand children nodes";
            columnHeader15.Width = 2500;
            // 
            // buttonReload
            // 
            buttonReload.Location = new Point(610, 30);
            buttonReload.Name = "buttonReload";
            buttonReload.Size = new Size(150, 30);
            buttonReload.TabIndex = 3;
            buttonReload.Text = "&Reload";
            buttonReload.UseVisualStyleBackColor = true;
            buttonReload.Click += buttonReload_Click;
            // 
            // FormTreeDesign2
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(1084, 661);
            Controls.Add(splitContainer1);
            Font = new Font("Segoe UI", 11.25F);
            Margin = new Padding(3, 4, 3, 4);
            Name = "FormTreeDesign2";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "tree";
            Load += FormTreeDesign2_Load;
            Shown += FormTreeDesign2_Shown;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainerHInner1.Panel1.ResumeLayout(false);
            splitContainerHInner1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerHInner1).EndInit();
            splitContainerHInner1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel1.PerformLayout();
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button buttonOK;
        private Button buttonCancel;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainerHInner1;
        private SplitContainer splitContainer2;
        private TextBox textboxPath;
        private TextBox txtEntryTitle;
        private SplitContainer splitContainer3;
        private ListView lvCurrentPath;
        private ColumnHeader columnHeader9;
        private ColumnHeader columnHeader11;
        private ListView lvChildren;
        private ColumnHeader columnHeader12;
        private ColumnHeader columnHeader13;
        private ListView lvChildsChildren;
        private ColumnHeader columnHeader14;
        private ColumnHeader columnHeader15;
        private Button buttonReload;
    }
}
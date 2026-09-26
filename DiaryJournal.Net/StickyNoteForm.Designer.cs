namespace DiaryJournal.Net
{
    partial class StickyNoteForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StickyNoteForm));
            toolStrip1 = new ToolStrip();
            tsButtonIncSize = new ToolStripButton();
            tsButtonDecSize = new ToolStripButton();
            rtb = new RichTextBox();
            menuRtb = new ContextMenuStrip(components);
            toolStripMenuItem23 = new ToolStripMenuItem();
            copyToolStripMenuItem2 = new ToolStripMenuItem();
            pasteToolStripMenuItem2 = new ToolStripMenuItem();
            toolStripSeparator39 = new ToolStripSeparator();
            selectAllToolStripMenuItem = new ToolStripMenuItem();
            toolStrip1.SuspendLayout();
            menuRtb.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { tsButtonIncSize, tsButtonDecSize });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(384, 25);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // tsButtonIncSize
            // 
            tsButtonIncSize.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsButtonIncSize.Image = (Image)resources.GetObject("tsButtonIncSize.Image");
            tsButtonIncSize.ImageTransparentColor = Color.Magenta;
            tsButtonIncSize.Name = "tsButtonIncSize";
            tsButtonIncSize.Size = new Size(76, 22);
            tsButtonIncSize.Text = "increase size";
            tsButtonIncSize.Click += tsButtonIncSize_Click;
            // 
            // tsButtonDecSize
            // 
            tsButtonDecSize.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsButtonDecSize.Image = (Image)resources.GetObject("tsButtonDecSize.Image");
            tsButtonDecSize.ImageTransparentColor = Color.Magenta;
            tsButtonDecSize.Name = "tsButtonDecSize";
            tsButtonDecSize.Size = new Size(79, 22);
            tsButtonDecSize.Text = "decrease size";
            tsButtonDecSize.Click += tsButtonDecSize_Click;
            // 
            // rtb
            // 
            rtb.BackColor = Color.Gold;
            rtb.ContextMenuStrip = menuRtb;
            rtb.Dock = DockStyle.Fill;
            rtb.Font = new Font("Segoe UI", 48F, FontStyle.Regular, GraphicsUnit.Point, 0);
            rtb.Location = new Point(0, 25);
            rtb.Name = "rtb";
            rtb.Size = new Size(384, 436);
            rtb.TabIndex = 1;
            rtb.Text = "";
            // 
            // menuRtb
            // 
            menuRtb.Items.AddRange(new ToolStripItem[] { toolStripMenuItem23, copyToolStripMenuItem2, pasteToolStripMenuItem2, toolStripSeparator39, selectAllToolStripMenuItem });
            menuRtb.Name = "menuRtbSearch";
            menuRtb.Size = new Size(120, 98);
            // 
            // toolStripMenuItem23
            // 
            toolStripMenuItem23.Name = "toolStripMenuItem23";
            toolStripMenuItem23.Size = new Size(119, 22);
            toolStripMenuItem23.Text = "cut";
            toolStripMenuItem23.Click += toolStripMenuItem23_Click;
            // 
            // copyToolStripMenuItem2
            // 
            copyToolStripMenuItem2.Name = "copyToolStripMenuItem2";
            copyToolStripMenuItem2.Size = new Size(119, 22);
            copyToolStripMenuItem2.Text = "copy";
            copyToolStripMenuItem2.Click += copyToolStripMenuItem2_Click;
            // 
            // pasteToolStripMenuItem2
            // 
            pasteToolStripMenuItem2.Name = "pasteToolStripMenuItem2";
            pasteToolStripMenuItem2.Size = new Size(119, 22);
            pasteToolStripMenuItem2.Text = "paste";
            pasteToolStripMenuItem2.Click += pasteToolStripMenuItem2_Click;
            // 
            // toolStripSeparator39
            // 
            toolStripSeparator39.Name = "toolStripSeparator39";
            toolStripSeparator39.Size = new Size(116, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            selectAllToolStripMenuItem.Size = new Size(119, 22);
            selectAllToolStripMenuItem.Text = "select all";
            selectAllToolStripMenuItem.Click += selectAllToolStripMenuItem_Click;
            // 
            // StickyNoteForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(384, 461);
            Controls.Add(rtb);
            Controls.Add(toolStrip1);
            Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(5);
            Name = "StickyNoteForm";
            Text = "your sticky notes";
            TopMost = true;
            Load += StickyNoteForm_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            menuRtb.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStrip1;
        private RichTextBox rtb;
        private ToolStripButton tsButtonIncSize;
        private ToolStripButton tsButtonDecSize;
        private ContextMenuStrip menuRtb;
        private ToolStripMenuItem toolStripMenuItem23;
        private ToolStripMenuItem copyToolStripMenuItem2;
        private ToolStripMenuItem pasteToolStripMenuItem2;
        private ToolStripSeparator toolStripSeparator39;
        private ToolStripMenuItem selectAllToolStripMenuItem;
    }
}
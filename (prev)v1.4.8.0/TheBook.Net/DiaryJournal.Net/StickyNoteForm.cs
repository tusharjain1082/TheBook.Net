using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiaryJournal.Net
{
    public partial class StickyNoteForm : Form
    {
        public String notes = "";

        public StickyNoteForm()
        {
            InitializeComponent();
        }

        private void StickyNoteForm_Load(object sender, EventArgs e)
        {
            rtb.Text = notes;
        }

        private void toolStripMenuItem23_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)cms.SourceControl;
            rtb.Cut();

        }

        private void copyToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)cms.SourceControl;
            rtb.Copy();

        }

        private void pasteToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)cms.SourceControl;
            rtb.Paste();

        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)cms.SourceControl;
            rtb.SelectAll();

        }

        private void tsButtonIncSize_Click(object sender, EventArgs e)
        {
            rtb.Font = new Font(rtb.Font.Name, rtb.Font.Size + 2, rtb.Font.Style);
        }

        private void tsButtonDecSize_Click(object sender, EventArgs e)
        {
            if (rtb.Font.Size > 8)
                rtb.Font = new Font(rtb.Font.Name, rtb.Font.Size - 2, rtb.Font.Style);

        }
    }
}

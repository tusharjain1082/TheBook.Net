#define UNICODE

using AngleSharp.Dom;
using HtmlAgilityPack;
using MarkupConverter;
using PdfSharp.Drawing;
using RtfPipe;
using RtfPipe.Model;
using RtfPipe.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TheBook.Net.Core;

namespace DiaryJournal.Net
{

    public partial class FormJournalDesign1 : Form
    {

        public bool stateChanged = false;
        public textFormatting formatting = null;

        public String xamlState0 = "";
        public String xamlState1 = "";
        public String xamlState2 = "";
        public String xamlState3 = "";
        public String xamlState4 = "";
        public String xamlState5 = "";
        public String xamlState6 = "";
        public String xamlState7 = "";
        public String xamlState8 = "";
        public String xamlState9 = "";

        public String xamlState = "";
        public String previousXamlState = "";
        public FormFind? myFormFind = null;
        public TemplateFormat templateFormats = new TemplateFormat();

        // TheBook.Net
        public OpenFSDBContext? dbCtx = null;

        public Form? myParentForm = null;

        bool properExit = false;
        public List<myNode> allNodes = new List<myNode>();
        PrintRichTextBoxEx printerRtb = new PrintRichTextBoxEx();

        //public List<myNode>? RootSystemNodes = null;
        public List<RegisterItem>? RootSystemNodesRegistry = null;
        public RegisterItem? currentPathItem = null;
        public RegisterItem? emptySlotsItem = null;

        System.Windows.Controls.WpfRichTextBoxEx rtbEntry = new System.Windows.Controls.WpfRichTextBoxEx();
        System.Windows.Controls.PrintDialog pd = new System.Windows.Controls.PrintDialog();
        //        System.Windows.Controls.Viewbox viewboxPrimary = new System.Windows.Controls.Viewbox();
        ElementHost host1 = new ElementHost();
        ElementHost host2 = new ElementHost();

        // delegates
        public delegate void __toggleFormDelegate(bool toggle);
        public __toggleFormDelegate toggleForm;
        public delegate void __showMessageBoxDelegate(String text, String title, MessageBoxButtons buttons, MessageBoxIcon icon);
        public __showMessageBoxDelegate showMessageBox;
        public delegate void __updateTotalEntriesStatusDelegate(long totalEntries);
        public __updateTotalEntriesStatusDelegate updateTotalEntriesStatus;
        public delegate void __resetLVSearchDelegate();
        public __resetLVSearchDelegate resetLVSearch;
        public delegate void __LvSearchUpdateDelegate(bool set);
        public __LvSearchUpdateDelegate LvSearchUpdate;
        public delegate void __saveEntryDeletage();
        public __saveEntryDeletage saveEntry;
        public delegate void __hideFormDelegate(bool toggle);
        public __hideFormDelegate hideForm;
        public delegate void __showFormDelegate();
        public __showFormDelegate showForm;
        public delegate void __updateSearchProgressPathDelegate(String path);
        public __updateSearchProgressPathDelegate updateSearchProgressPath;
        public delegate void __gotoEntryByAttributeDelegate(bool lm, bool lc, bool byID, UInt32 id);
        public __gotoEntryByAttributeDelegate gotoEntryByAttribute;
        public delegate bool __processSearchDelegate();
        public __processSearchDelegate processSearch;
        public delegate TreeNode? __TVTreeAddItemDelegate(RegisterItem? item, TreeNode? tvParentNode);
        public __TVTreeAddItemDelegate TVTreeAddItem;
        public delegate void __configureStatusDelegate(RegisterItem? currentItem);
        public __configureStatusDelegate configureStatus;
        public delegate void __loadSelectedEntryDelegate(UInt32 id);
        public __loadSelectedEntryDelegate loadSelectedEntry;
        public delegate void __expandAllLineageDelegate(List<TreeNode?>? tvlineage);
        public __expandAllLineageDelegate ExpandAllLineage;

        public FormJournalDesign1()
        {
            InitializeComponent();
        }

        /*
        private void FormJournal_KeyDown(object sender, KeyEventArgs e)
        {
            if (!dbCtx.idle)
            {
                // meaning busy, do not process any menu shortcut key
                e.Handled = true;
                MessageBox.Show("test");
                return;
            }
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // If the form is busy, return true to indicate the key is "handled"
            // and should not be processed further.
            if (!dbCtx.idle)
            {
                // meaning busy, do not process any menu shortcut key
                MessageBox.Show("test");
                return true;
            }

            // idle
            return base.ProcessCmdKey(ref msg, keyData);
        }
        */

        protected override bool ProcessDialogKey(Keys keyData)
        {
            //if (isProcessing && keyData == (Keys.Control | Keys.S))
            // {
            //     return true; // Consume the key and do nothing
            //}
            if (!dbCtx.idle)
                return true;

            return base.ProcessDialogKey(keyData);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // List specific shortcuts to disable
            /*
            if (keyData == (Keys.Control | Keys.S))
            {
                return true; // Consume the key so the menu doesn't see it
            }

            // To disable ALL Alt-key combinations (menu mnemonics)
            if ((keyData & Keys.Alt) == Keys.Alt)
            {
                return true;
            }
            */
            if (!dbCtx.idle)
                return true;

            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void FormJournalDesign1_Load(object sender, EventArgs e)
        {
            // Add a reference to the NuGet package System.Text.Encoding.CodePages for .Net core only
            // important initialization for RtfPipe Library:
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // auto config and init
            CoreFramework.autoCreateInitLocalPaths();

            // initialize all formatting config
            formatting = new textFormatting();

            host1.Dock = DockStyle.Fill;
            rtbEntry.Effect = null;
            host1.AutoSize = true;
            host1.Child = rtbEntry;
            rtbEntry.HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
            rtbEntry.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;

            splitContainer5.Panel2.Controls.Add(host1);

            rtbEntry.SnapsToDevicePixels = true;
            rtbEntry.UseLayoutRounding = true;
            formatting.formatLineSpacing(rtbEntry, 0.5);
            formatting.formatLineSpacing(rtbEntry.dummy, 0.5);

            //this.KeyDown += FormJournal_KeyDown;
            rtbEntry.TextChanged += rtbEntry_TextChanged;
            rtbEntry.SelectionChanged += RtbEntry_SelectionChanged;
            tabControlJournal.Selected += TabControlJournal_Selected;
            cmbFonts.SelectedIndexChanged += CmbFonts_SelectedIndexChanged;
            cmbSize.SelectedIndexChanged += CmbSize_SelectedIndexChanged;
            lvSearch.DoubleClick += LvSearch_DoubleClick;
            this.Shown += FormJournalDesign1_Shown;
            this.FormClosing += FormJournalDesign1_FormClosing;
            cmbSize.DropDownClosed += CmbSize_DropDownClosed;
            cmbFonts.DropDownClosed += CmbFonts_DropDownClosed;

            cmbFonts.MouseUp += CmbFonts_MouseUp;
            cmbSize.MouseUp += CmbSize_MouseUp;

            String strDateTimeTemplate = DiaryJournal.Net.Properties.Resources.BuildDateTime;
            DateTime buildDateTime = DateTime.Parse(strDateTimeTemplate);
            String strBuildDateTime = buildDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            System.Version version = Assembly.GetExecutingAssembly().GetName().Version;
            this.Text = $"Tushar Jain's TheBook.Net-Version{Application.ProductVersion}::DiaryJournal.Net-Version{version}, uses .Net 10.0, Compiled/Built on: " + strBuildDateTime;

            for (int size = 6; size <= 300; size++)
                cmbSize.Items.Add(size);

            // setup delegates
            toggleForm = new __toggleFormDelegate(__toggleForm);
            showMessageBox = new __showMessageBoxDelegate(__showMessageBox);
            updateTotalEntriesStatus = new __updateTotalEntriesStatusDelegate(__updateTotalEntriesStatus);
            resetLVSearch = new __resetLVSearchDelegate(__resetLVSearch);
            LvSearchUpdate = new __LvSearchUpdateDelegate(__LvSearchUpdate);
            saveEntry = new __saveEntryDeletage(__saveEntry);
            hideForm = new __hideFormDelegate(__hideForm);
            showForm = new __showFormDelegate(__showForm);
            updateSearchProgressPath = new __updateSearchProgressPathDelegate(__updateSearchProgressPath);
            gotoEntryByAttribute = new __gotoEntryByAttributeDelegate(__gotoEntryByAttribute);
            processSearch = new __processSearchDelegate(__processSearch);
            TVTreeAddItem = new __TVTreeAddItemDelegate(__TVTreeAddItem);
            configureStatus = new __configureStatusDelegate(__configureStatus);
            loadSelectedEntry = new __loadSelectedEntryDelegate(__loadSelectedEntry);
            ExpandAllLineage = new __expandAllLineageDelegate(__expandAllLineage);

            // now load config file and setup
            dbCtx.config = new myConfig();
            if (!File.Exists(myConfigMethods.getConfigPathFile()))
                myConfigMethods.toYamlFile(dbCtx.config, myConfigMethods.getConfigPathFile());
            else
                dbCtx.config = myConfigMethods.fromYamlFile(myConfigMethods.getConfigPathFile());

            applyConfig();

            splitContainerH.Cursor = Cursors.Default;

            cmbFonts.Items.AddRange(formatting.fontNames.ToArray());

            // config
            resetRtb(rtbEntry);

            // listing all web colors in toolstrip drop down split buttons so that the user can select them.
            var webColors = myCommonMethods1.getWebColors();// typeof(Color));
            foreach (System.Drawing.Color knownColor in webColors)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = knownColor.ToString();
                item.BackColor = knownColor; //Color.FromKnownColor(knownColor);
                item.Tag = knownColor;
                tssplitbuttonFontColors.DropDownItems.Add(item);
                item.Click += ToolStripFontColorMenuItem_Click;

                ToolStripMenuItem item2 = new ToolStripMenuItem();
                item2.Text = knownColor.ToString();
                item2.BackColor = knownColor; //Color.FromKnownColor(knownColor);
                item2.Tag = knownColor;
                tssplitbuttonBackColors.DropDownItems.Add(item2);
                item2.Click += ToolStripBackColorMenuItem_Click;
            }

            dtpickerSearchFrom.Value = DateTime.Now;
            dtpickerSearchThrough.Value = DateTime.Now;
            dtpickerSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerSearchThroughTime.Value = DateTime.Parse("23:59:59");
            dtpickerCDSearchFrom.Value = DateTime.Now;
            dtpickerCDSearchThrough.Value = DateTime.Now;
            dtpickerCDSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerCDSearchThroughTime.Value = DateTime.Parse("23:59:59");
            dtpickerMDSearchFrom.Value = DateTime.Now;
            dtpickerMDSearchThrough.Value = DateTime.Now;
            dtpickerMDSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerMDSearchThroughTime.Value = DateTime.Parse("23:59:59");

            // configure
            //rtbEntry.HideSelection = rtbViewEntry.HideSelection = false;

            // configure print document and printer stuff
            printerRtb.printDoc = pdRtbEntry;
            pdRtbEntry.BeginPrint += new PrintEventHandler(printerRtb.printDoc_BeginPrint);
            pdRtbEntry.PrintPage += new PrintPageEventHandler(printerRtb.printDoc_PrintPage);
            pdRtbEntry.EndPrint += new PrintEventHandler(printerRtb.printDoc_EndPrint);

            foreach (ToolStripItem item in msJournal.Items)
            {
                foreach (ToolStripMenuItem menuitem in myCommonMethods1.GetAllItems(item))
                {
                    menuitem.Tag = menuitem.ShortcutKeys;
                }
            }
        }

        private void CmbFonts_DropDownClosed(object? sender, EventArgs e)
        {
            ToolStripComboBox comboBox = (ToolStripComboBox)sender;
            if (comboBox.SelectedItem == null)
                return;

            formatting.formatFont(rtbEntry, (String)comboBox.SelectedItem);
        }

        private void CmbSize_MouseUp(object? sender, MouseEventArgs e)
        {
        }

        public void __updateSearchProgressPath(String path)
        {
            txtSearchProgressFullPath.Text = path;
            txtSearchProgressFullPath.Update();
            this.Update();
        }

        private void CmbFonts_MouseUp(object? sender, MouseEventArgs e)
        {
        }

        private void RtbEntry_SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            tsbuttonUnderline.Checked = rtbEntry.Underline;
            tsbuttonStrikeout.Checked = rtbEntry.Strikeout;
            tsbuttonBold.Checked = rtbEntry.Bold;
            tsbuttonItalics.Checked = rtbEntry.Italic;

            int caretPosition = rtbEntry.SelectionStartOffset;
            int lineIndex = rtbEntry.LineIndex;
            tsslabelCaretPosition.Text = caretPosition.ToString();
            tsslabelLineIndex.Text = lineIndex.ToString();

            formatting.selStartIndex = rtbEntry.SelectionStartOffset;
            formatting.selLength = rtbEntry.SelectionLength;
            if (rtbEntry.SelectionFont != null)
            {
                int index = cmbFonts.FindString(rtbEntry.SelectionFont.Name);
                cmbFonts.SelectedIndex = index;
            }
            else
            {
                cmbFonts.SelectedItem = null;
            }

            if (rtbEntry.SelectionFont != null)
            {
                double doubleSelFontSize = rtbEntry.SelectionFontSize;
                int intSelFontSize = (int)doubleSelFontSize;
                cmbSize.SelectedIndex = cmbSize.FindString(intSelFontSize.ToString());
            }

            if (rtbEntry.SelectionAlignment == System.Windows.TextAlignment.Left)
                tsbuttonLeftJustify.Checked = true;
            else
                tsbuttonLeftJustify.Checked = false;

            if (rtbEntry.SelectionAlignment == System.Windows.TextAlignment.Right)
                tsbuttonRightJustify.Checked = true;
            else
                tsbuttonRightJustify.Checked = false;

            if (rtbEntry.SelectionAlignment == System.Windows.TextAlignment.Justify)
                tsbuttonJustify.Checked = true;
            else
                tsbuttonJustify.Checked = false;

            if (rtbEntry.SelectionAlignment == System.Windows.TextAlignment.Center)
                tsbuttonCenterJustify.Checked = true;
            else
                tsbuttonCenterJustify.Checked = false;

            if (rtbEntry.SelectionBullets)
                tsbuttonBullets.Checked = true;
            else
                tsbuttonBullets.Checked = false;

            if (rtbEntry.SelectionNumbering)
                tsbuttonNumberedList.Checked = true;
            else
                tsbuttonNumberedList.Checked = false;


            if (rtbEntry.SelectionIndent > 0)
            {
                tsbuttonIndentLeft.Checked = false;
                tsbuttonIndentRight.Checked = true;
            }
            else
            {
                tsbuttonIndentLeft.Checked = true;
                tsbuttonIndentRight.Checked = false;
            }
        }

        public void __showForm()
        {
            this.Activate();
            this.Focus();
            this.BringToFront();
            this.Show();
        }
        // Determine whether one node is a parent 
        // or ancestor of a second node.
        private bool ContainsNode(TreeNode node1, TreeNode node2)
        {
            // Check the parent node of the second node.
            if (node2 == null) return false;
            if (node2.Parent == null) return false;
            if (node2.Parent.Equals(node1)) return true;

            // If the parent node is not null or equal to the first node, 
            // call the ContainsNode method recursively using the parent of 
            // the second node.
            return ContainsNode(node1, node2.Parent);
        }
        private void FormJournalDesign1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (!properExit)
            {
                //                var res = MessageBox.Show(this, "you cannot directly close this form. please click on exit button to close the form.", "error",
                //                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                //e.Cancel = true;
            }
            else
            {
                //e.Cancel = false;
            }

            // first save the entry, then exit
            if (stateChanged)
            {
                if (MessageBox.Show(this, "do you wish to save the currently active changed entry?", "question",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    saveEntry();
            }

            try
            {
                reset();
            }
            catch { }

            try
            {
                List<Control>? list = ControlHelper.GetAllChildControls(this); //ControlHelper.GetAll(this, typeof(Control)))
                if (list != null)
                {
                    foreach (Control c in list)
                        c.Dispose();
                }
                this.Controls.Clear();
            }
            catch { }

            if (this.myParentForm != null)
                this.myParentForm.Show();

            try
            {
                base.OnClosing(e);
            }
            catch { }

        }

        private void FormJournalDesign1_Shown(object? sender, EventArgs e)
        {
            // if vhd db is not opened then force exit
            if (dbCtx == null)
            {
                MessageBox.Show(this, "you cannot directly open this journal application. please load properly through the database manager application TheBook.Net.exe file.", "error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            loadDB();
            this.BringToFront();
            this.Activate();
            this.Focus();
            this.Height = this.Height + 1;

        }

        public void applyConfig()
        {
            rtbViewEntry.RightMargin = dbCtx.config.cmbCfgRtbViewEntryRMValue;
            int index = cmbCfgRtbViewEntryRM.FindString(rtbViewEntry.RightMargin.ToString());
            cmbCfgRtbViewEntryRM.SelectedIndex = index;
            radCfgLMNode.Checked = dbCtx.config.radCfgLMNode;
            radCfgLCNode.Checked = dbCtx.config.radCfgLCNode;
        }

        public void viewEntry(RegisterItem item)
        {
            String rtf = "";
            byte[]? xamlbytesOut = null;
            item.loadNode(dbCtx, ref rtf, ref xamlbytesOut, true);
            if (dbCtx.dbEntryType == EntryType.Xaml)
                rtf = xamlEntry.toRtf(xamlbytesOut);

            rtbViewEntry.Rtf = rtf;
            tabControlJournal.SelectedIndex = tabControlJournal.TabPages.IndexOfKey("TabPageViewEntry");

            // now setup caret config
            rtbViewEntry.Select(item.node.chapter.caretIndex, 0);
            if (item.node.chapter.caretSelectionLength != 0)
                rtbViewEntry.Select(item.node.chapter.caretIndex, item.node.chapter.caretSelectionLength);

            rtbViewEntry.ScrollToCaret();

        }
        public void OpenSearchedEntry(UInt32 id)
        {
            RegisterItem? item = Register.LoadSetupRegisterItem(dbCtx, id, true, false, false, false, false, false);
            if (item == null) return; // error abort

            if (item.node.chapter.IsDeleted)
            {
                viewEntry(item);
            }
            else
            {
                tabControlJournal.SelectedIndex = 0;
                __gotoEntryById(item.Id);
            }
        }

        public bool getSelectedListViewNodeId(System.Windows.Forms.ListView lv, out UInt32 idOut)
        {
            if (!dbCtx.isDBOpen())
            {
                idOut = 0;
                return false;
            }

            if (lv.SelectedItems.Count == 0)
            {
                idOut = 0;
                return false;
            }
            System.Windows.Forms.ListViewItem listViewItem = lv.SelectedItems[0];
            UInt32 id = UInt32.Parse(listViewItem.Name);

            idOut = id;
            return true;
        }

        private void LvSearch_DoubleClick(object? sender, EventArgs e)
        {

            UInt32 id = 0;
            if (getSelectedListViewNodeId(lvSearch, out id))
                OpenSearchedEntry(id);
        }

        private void ToolStripFontColorMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            if (menuItem == null)
                return;

            System.Drawing.Color color = (System.Drawing.Color)menuItem.Tag;
            if (color == null)
                return;

            formatting.formatFontColor(rtbEntry, color);
        }

        private void ToolStripBackColorMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            if (menuItem == null)
                return;

            System.Drawing.Color color = (System.Drawing.Color)menuItem.Tag;
            if (color == null)
                return;

            formatting.formatBackColor(rtbEntry, color);
        }

        public void resetRtb(System.Windows.Controls.WpfRichTextBoxEx rtb, bool clear = true, bool resetSaveState = false)
        {

            // config
            if (clear)
            {
                // configure rtb document
                configureRtbDocument(rtbEntry, myConfig.defaultDocumentWidth, true);

                rtb.Rtf = "";
                //tushar 01 May 2025: rtb.SelectionFont = new System.Drawing.Font("Times New Roman", 24.0f, FontStyle.Regular);
            }

            if (resetSaveState)
            {
                stateChanged = false;
                tsslblStateChanged.Text = " ";
                tsbuttonSave.Checked = false;
                tsbuttonSave.BackColor = SystemColors.Control;
                tsslblStateChanged.BackColor = SystemColors.Control;

            }
        }

        private void CmbSize_SelectedIndexChanged(object? sender, EventArgs e)
        {
        }

        private void CmbFonts_SelectedIndexChanged(object? sender, EventArgs e)
        {
        }

        private void TabControlJournal_Selected(object? sender, TabControlEventArgs e)
        {
        }

        public void setupNowEntry()
        {
            setupNewEntry(DateTime.Now, false);
        }

        public RegisterItem? setupNewSpecialEntry(DateTime dateTime, NodeType nodeType = NodeType.Template,
            NodeType parentSystemNodeType = NodeType.Templates, String title = "")
        {
            // first save the entry
            saveEntry();

            // reload latest state
            RegisterItem? parent = entryMethodsNewDesign.findRegistrySystemNodeItemByType(this.RootSystemNodesRegistry, parentSystemNodeType);
            parent = Register.LoadSetupRegisterItem(dbCtx, parent.Id, false, false, false, false, true, true);
            if (parent == null) return null;

            // now create registry and node
            myNode node = new myNode();
            node.chapter.parentId = parent.Id;
            node.chapter.chapterDateTime = dateTime;
            node.chapter.nodeType = nodeType;
            node.chapter.Title = title;
            byte[]? xamlbytes = null;
            RegisterItem? item = Register.Insert(dbCtx, parent.Id, emptySlotsItem, node, "", xamlbytes);
            if (item != null)
            {
                TreeNode? tvItem = null;
                TreeNode? tvParent = null;
                List<TreeNode?> tvLineage = null;
                List<RegisterItem?> lineage = null;

                // first add new node in tree view
                getTreeNodesLineage(item, true, ref tvParent, ref tvItem, ref lineage, ref tvLineage);

                // reconfigure labels
                reloadTreeNodeConfigLabel(tvParent);

                // finally reload path and set this new item in editor
                tvTree.SelectedNode = tvItem;

                MessageBox.Show("special node created", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("error, special node not created", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return item;
        }
        public void setupNewEntry(DateTime dateTime, bool hasParent, UInt32 parentId = 0, NodeType nodeType = NodeType.Entry,
            String title = "")
        {
            if (!dbCtx.isDBOpen())
                return;

            if (dbCtx.readOnly)
            {
                MessageBox.Show("error entry not created", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // firstly save entry
            __saveEntry();

            // create load calendar nodes
            RegisterItem? year = null;
            RegisterItem? month = null;
            // create-load year node
            if (!entryMethodsNewDesign.DBCreateLoadYearSystemNode(dbCtx, RootSystemNodesRegistry, emptySlotsItem, dateTime, ref year))
            {
                MessageBox.Show("error entry not created", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // create-load month node
            if (!entryMethodsNewDesign.DBCreateLoadMonthSystemNode(dbCtx, emptySlotsItem, year, dateTime, ref month))
            {
                MessageBox.Show("error entry not created", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!hasParent)
            {
                // parent not required means month parent id
                parentId = month.node.chapter.Id;
            }

            myNode node = new myNode();
            node.chapter.parentId = parentId;
            node.chapter.chapterDateTime = dateTime;
            node.chapter.creationDateTime = DateTime.Now;
            node.chapter.modificationDateTime = DateTime.Now;
            node.chapter.nodeType = nodeType;
            node.chapter.Title = title;

            RegisterItem? parent = Register.LoadSetupRegisterItem(dbCtx, parentId, true, false, false, false, false, false);
            if (parent == null) return; // parent not found or critical error

            // validations
            if (parent.childrenCount >= Register.default_maxChildrenNodes)
            {
                MessageBox.Show($"error entry not created. maximum direct new children create/insert limit [{Register.default_maxChildrenNodes}] reached for/in this target parent.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // now create registry and node
            byte[]? xamlbytes = null;
            RegisterItem? item = Register.Insert(dbCtx, parent.Id, emptySlotsItem, node, "", xamlbytes);
            if (item != null)
            {
                TreeNode? tvItem = null;
                TreeNode? tvParent = null;
                List<TreeNode?> tvLineage = null;
                List<RegisterItem?> lineage = null;

                // first add new node in tree view
                getTreeNodesLineage(item, true, ref tvParent, ref tvItem, ref lineage, ref tvLineage);

                // reconfigure labels
                reloadTreeNodeConfigLabel(tvParent);

                // finally reload path and set this new item in editor
                tvTree.SelectedNode = tvItem;

                MessageBox.Show("entry node created", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                MessageBox.Show("error entry not created", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TheJournalImportrtfEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doImportRtfEntries();
        }
        public void doImportRtfEntries()
        {
            if (!dbCtx.isDBOpen())
                return;

            if (dbCtx.readOnly)
            {
                MessageBox.Show("error. database read-only.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            browseFolder.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Application.StartupPath;
            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            __importTheJournalRtfEntries(browseFolder.SelectedPath);
        }
        private void toolStripMenuItem70_Click(object sender, EventArgs e)
        {
            doImportRtfEntries();
        }
        public bool ForceSetupNewEntrySilent(DateTime dateTime, ref RegisterItem? outItem, Chapter? chapter = null,
            UInt32 parentId = 0, bool useParentId = false, NodeType nodeType = NodeType.Entry,
            String title = "", String? rtf = "", byte[]? xamlbytes = null)
        {
            if (!dbCtx.isDBOpen())
                return false;

            if (dbCtx.readOnly)
                return false;

            // firstly save entry
            __saveEntry();

            // create load calendar nodes
            RegisterItem? year = null;
            RegisterItem? month = null;
            // create-load year node
            if (!entryMethodsNewDesign.DBCreateLoadYearSystemNode(dbCtx, RootSystemNodesRegistry, emptySlotsItem, dateTime, ref year))
                return false;

            // create-load month node
            if (!entryMethodsNewDesign.DBCreateLoadMonthSystemNode(dbCtx, emptySlotsItem, year, dateTime, ref month))
                return false;

            myNode node = new myNode();
            if (chapter != null) node.chapter = chapter;

            if (useParentId)
                node.chapter.parentId = parentId;
            else
                node.chapter.parentId = month.Id;

            node.chapter.chapterDateTime = dateTime;
            node.chapter.creationDateTime = DateTime.Now;
            node.chapter.modificationDateTime = DateTime.Now;
            node.chapter.nodeType = nodeType;
            node.chapter.Title = title;

            RegisterItem? parent = Register.LoadSetupRegisterItem(dbCtx, node.chapter.parentId, true, false, false, false, false, false);
            if (parent == null)
                return false; // parent not found or critical error

            // now create registry and node

            RegisterItem? item = Register.Insert(dbCtx, parent.Id, emptySlotsItem, node, rtf, xamlbytes);
            if (item != null)
            {
                TreeNode? tvItem = null;
                TreeNode? tvParent = null;
                List<TreeNode?> tvLineage = null;
                List<RegisterItem?> lineage = null;

                // first add new node in tree view
                getTreeNodesLineage(item, true, ref tvParent, ref tvItem, ref lineage, ref tvLineage);

                // reconfigure labels
                reloadTreeNodeConfigLabel(tvParent);

                outItem = item;

                // success
                return true;
            }
            else
            {
                outItem = null;
                return false; // critical error
            }
        }

        public void __importTheJournalRtfEntries(String path)
        {
            if (!dbCtx.isDBOpen())
                return;

            // first save the entry
            saveEntry();

            __toggleForm(false);
            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // first initialize
            IEnumerable<FileInfo> files = myCommonMethods1.EnumerateFiles(path, EntryType.Rtf);
            Int32 index = 0;
            Int32 total = files.Count();
            if (total <= 0) return;

            // first create set top node
            // do not use set node
            /*
            RegisterItem? setNode = null;
            if (!ForceSetupNewEntrySilent(DateTime.Now, ref setNode, null, 0, NodeType.Set, importSetName, "", null))
            {
                MessageBox.Show("error, set node not created", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // error set node not created
            }

            // reload
            setNode = Register.LoadSetupRegisterItem(dbCtx, setNode.Id, true, false, false, false, true, true);
            */

            // load set node
            //TreeNode? tvSetNodeItem = null;
            //TreeNode? tvSetNodeParent = null;
            //List<TreeNode?> tvSetNodeLineage = null;
            //List<RegisterItem?> setNodelineage = null;

            // first add new node in tree view
            //getTreeNodesLineage(setNode, true, ref tvSetNodeParent, ref tvSetNodeItem, ref setNodelineage, ref tvSetNodeLineage);

            // reconfigure labels
            //reloadTreeNodeConfigLabel(tvParent);

            foreach (FileInfo file in files)
            {
                // 2nd load rtf from file
                String? rtf = File.ReadAllText(file.FullName);

                Chapter? chapter = theJournalMethods.convertFilenameToChapter(file.FullName);
                if (chapter == null)
                    continue;

                byte[]? xaml = null;
                if (dbCtx.dbEntryType == EntryType.Xaml)
                {
                    xamlEntry.dummy.Rtf = rtf;
                    xaml = xamlEntry.dummy.XamlBytes;
                }

                // 3rd import rtf and file as new node into set top node
                // create file
                RegisterItem? child = null;
                if (!ForceSetupNewEntrySilent(chapter.chapterDateTime, ref child, chapter, 0, false, NodeType.Entry, chapter.Title, rtf, xaml))
                    continue; // error set node not created

                // update progress
                formOperation.updateProgressBar(index, total);
                formOperation.updateFilesStatus(index++, total);

            }

            formOperation.close();
            __toggleForm(true);

            // reset reload to root
            tvTree.SelectedNode = tvTree.Nodes.Find("0", false).FirstOrDefault();

            MessageBox.Show($"{index} entries imported of total {total} entries.", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        public void __showMessageBox(String text, String title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            MessageBox.Show(this, text, title, buttons, icon);
        }

        public void __toggleForm(bool toggle)
        {
            // 1. Initial call to start the process
            foreach (ToolStripItem item in msJournal.Items)
            {

                foreach (ToolStripMenuItem menuitem in myCommonMethods1.GetAllItems(item))
                {
                    menuitem.Enabled = toggle;
                    //menuitem.Visible = toggle;
                    if (toggle)
                    {
                        menuitem.ShortcutKeys = (Keys)menuitem.Tag;
                    }
                    else
                    {
                        menuitem.ShortcutKeys = Keys.None;
                        //menuitem.Invalidate();
                    }
                }

                item.Enabled = toggle;
                //item.Visible = toggle;
            }

            //this.Update();

            foreach (Control obj in this.Controls)
                obj.Enabled = toggle;

            if (toggle)
            {
                //this.Controls.Add(msJournal);
                tvTree.EndUpdate();
            }
            else
            {
                //this.Controls.Remove(msJournal);
                tvTree.BeginUpdate();
            }


            // Example: Processing messages during a heavy loop
            // working
            for (int i = 0; i < 10000; i++)
            {
                // Process all pending messages (repaints, clicks, etc.)
                Application.DoEvents();
            }
            dbCtx.idle = toggle;
            //msJournal.Enabled = toggle;
            //this.Enabled = toggle;
        }
        public void __hideForm(bool toggle)
        {
            if (toggle)
                this.Hide();
            else
                this.Show();
        }

        public void __updateTotalEntriesStatus(long totalEntries)
        {
            labelEntries.Text = totalEntries.ToString();
        }

        public bool loadDB()
        {
            // reset everything
            reset();

            // setup ui
            txtDBFile.Text = dbCtx.dbBasePath;

            if (radCfgLMNode.Checked)
                textboxPath.Text = dbCtx.dbConfig.lastModifiedEntry.ToString();
            else if (radCfgLCNode.Checked)
                textboxPath.Text = dbCtx.dbConfig.latestCreatedEntry.ToString();

            if (dbCtx.readOnly)
                MessageBox.Show("warning: database is write locked. to write in it, please disable write lock in database manager form.", "warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // operations status form
            this.Enabled = false;
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // first create any missing root system node
            List<RegisterItem>? registry = null;
            RegisterItem? emptySlotsItem = null;
            if (!entryMethodsNewDesign.DBCreateDBCore(dbCtx, ref emptySlotsItem, false))
            {
                this.Enabled = true;
                formOperation.close();
                return false; // critical error, abort
            }
            // now load all root system nodes
            if (!entryMethodsNewDesign.DBLoadRootSystemNodes(dbCtx, ref registry, ref emptySlotsItem))
            {
                this.Enabled = true;
                formOperation.close();
                return false; // critical error, abort
            }

            // setup
            this.RootSystemNodesRegistry = registry;
            this.emptySlotsItem = emptySlotsItem;

            this.Enabled = true;
            formOperation.close();

            // now finally present the demanded nodes in user interface according to configuration
            currentPathItem = null;
            reload();

            return true;
        }
        public void reload()
        {
            // first save current entry
            saveEntry();

            // first empty the tree view control
            tvTree.Nodes.Clear();

            //if (currentPathItem != null)
            //    currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, false, false);
            //else
            //    currentPathItem = Register.LoadSetupRegisterItem(dbCtx, 0, true, false, false, false, false, false);

            // phase 1 - firstly get the registry node item
            List<RegisterItem>? registry = null;
            if (!Register.LoadFullPath(dbCtx, textboxPath.Text, ref registry, true))
                return; // critical error abort

            RegisterItem? item = registry.LastOrDefault();
            currentPathItem = item;

            TreeNode? tvItem = null;
            TreeNode? tvParent = null;
            List<TreeNode?> tvLineage = null;
            List<RegisterItem?> lineage = null;

            // first add new node in tree view
            getTreeNodesLineage(currentPathItem, true, ref tvParent, ref tvItem, ref lineage, ref tvLineage);

            // final setup
            tvTree.SelectedNode = tvItem;
        }

        public bool __findTreeNode(TreeNode parent, String strId, out TreeNode? treeNodeOut)
        {
            TreeNode[] nodes = parent.Nodes.Find(strId, false);
            if (nodes.Length != 0)
            {
                treeNodeOut = nodes.FirstOrDefault();
                return true;
            }
            treeNodeOut = null;
            return false;
        }

        public bool __findTreeNode(String strId, out TreeNode? treeNodeOut)
        {
            TreeNode[] nodes = tvTree.Nodes.Find(strId, true);
            if (nodes.Length != 0)
            {
                treeNodeOut = nodes.FirstOrDefault();
                return true;
            }
            treeNodeOut = null;
            return false;
        }
        public void __expandTreeNode(String strId)
        {
            TreeNode[] nodes = tvTree.Nodes.Find(strId, true);
            if (nodes.Length != 0)
            {
                TreeNode? treeNode = nodes.FirstOrDefault();
                treeNode.Expand();
            }
        }


        public TreeNode? __TVTreeAddItem(RegisterItem? item, TreeNode? tvParentNode)
        {
            if (item == null) return null;

            TreeNode? tnode = null;
            if (tvParentNode != null)
            {
                if (__findTreeNode(tvParentNode, item.Id.ToString(), out tnode)) return tnode; // exists so return existent node
            }
            else
            {
                if (__findTreeNode(item.Id.ToString(), out tnode)) return tnode; // exists so return existent node
            }

            String? rtf = "";
            byte[]? xaml = null;

            // load node config if required
            if (item.node == null)
                item.loadNode(dbCtx, ref rtf, ref xaml, false);

            // not exists, so create
            String label = entryMethods.getEntryLabel(item.node, false);
            String formatted = $"{label} ({item.childrenCount} children)";
            tnode = new TreeNode(formatted);
            tnode.Name = item.Id.ToString();
            if (item.Id == 0)
            {
                // item is root so add root tree node
                tvTree.Nodes.Add(tnode);
            }
            else
            {
                // item is not root but local node
                if (tvParentNode == null)
                {
                    // find parent and create child in it
                    if (!__findTreeNode(item.parentId.ToString(), out tvParentNode)) return null; // parent not exists so return error
                }
                if (tvParentNode == null)
                    return null; // error no parent

                // parent exists, so create child tree node
                if (tvParentNode.Nodes.Add(tnode) < 0) return null; // error tree node not inserted
            }
            return tnode;
        }

        // this method fetches the lineage of tree nodes without exaustive search

        public bool getTreeNodesLineage(RegisterItem? item, bool autoCreate, ref TreeNode? outTvParent,
            ref TreeNode? outTreeNode, ref List<RegisterItem>? outLineage, ref List<TreeNode?>? outTvLineage)
        {
            // phase 1 - firstly get the registry node item
            List<RegisterItem>? registry = null;

            if (!Register.LoadFullPath(dbCtx, item, ref registry, true, false, false, false, false))
                return false; // critical error abort

            if (registry.Count == 0) return false;

            // all mandatory validations
            if (item.nodeType == NodeType.EmptySlot) return false;

            // add all lineage
            List<TreeNode?>? tvlineage = new List<TreeNode?>();
            TreeNode? tvNode = null;

            // phase 1 - fetch tree nodes
            foreach (RegisterItem? lineageitem in registry)
            {
                if (tvNode != null)
                {
                    // local tree node with parent
                    __findTreeNode(tvNode, lineageitem.Id.ToString(), out tvNode);
                }
                else
                {
                    // root tree node so search in tree view control
                    __findTreeNode(lineageitem.Id.ToString(), out tvNode);
                }


                // add found parent tree node 
                tvlineage.Add(tvNode);
            }

            // phase 2 - create missing tree nodes if required
            List<TreeNode?>? tvlineageFinal = new List<TreeNode?>();
            tvNode = null;
            int index = 0;
            foreach (TreeNode? node in tvlineage)
            {
                if (node != null)
                {
                    // set this node as next node's parent
                    tvNode = node;
                }
                else
                {
                    // auto create all tree nodes if required
                    if (autoCreate)
                    {
                        tvNode = TVTreeAddItem(registry[index], tvNode);
                    }
                }

                if (tvNode != null)
                {
                    // get parent of current input item
                    UInt32 Id = UInt32.Parse(tvNode.Name);

                    if (item.parentId == Id)
                        outTvParent = tvNode;

                    // get current item
                    if (item.Id == Id)
                        outTreeNode = tvNode;
                }

                tvlineageFinal.Add(tvNode);
                index++;
            }

            // set
            outLineage = registry;
            outTvLineage = tvlineageFinal;
            return true;
        }
        public bool reloadPath(String path, bool usePath, RegisterItem? item)
        {
            
            // phase 1 - firstly get the registry node item
            List<RegisterItem>? registry = null;

            if (usePath)
            {
                if (!Register.LoadFullPath(dbCtx, path, ref registry, true))
                    return false; // critical error abort
            }
            else
            {
                if (!Register.LoadFullPath(dbCtx, item, ref registry, true, true, false, false, false))
                    return false; // critical error abort
            }

            if (registry.Count == 0) return false;

            // phase 2 - first at the top place, add parent child relation list items

            RegisterItem? currentItem = registry.LastOrDefault();

            // all mandatory validations
            if (currentItem.nodeType == NodeType.EmptySlot) return false;

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // add all lineage
            List<TreeNode?>? tvlineage = new List<TreeNode?>();
            TreeNode? parent = null;
            foreach (RegisterItem? lineageitem in registry)
            {
                parent = (TreeNode?)this.Invoke(TVTreeAddItem, lineageitem, parent);
                tvlineage.Add(parent);
            }

            this.Invoke(configureStatus, currentItem);

            // load all children in current path
            reloadCurrentTVTreeNodeChildren();

            this.Invoke(ExpandAllLineage, tvlineage);

            // finally load the entry into rtb
            this.Invoke(loadSelectedEntry, currentItem.Id);

            formOperation.close();

            return true;
        }

        public void __expandAllLineage(List<TreeNode?>? tvlineage)
        {
            // expand all lineage
            //tvCurrentItem.EnsureVisible();
            foreach (TreeNode? tvitem in tvlineage)
                tvitem.Expand();

        }
        public void __configureStatus(RegisterItem? currentItem)
        {
            // finally configure everything
            String rtf = "";
            byte[]? xaml = null;
            if (currentItem.node == null) currentItem.loadNode(dbCtx, ref rtf, ref xaml, false);
            bool readOnly = dbCtx.readOnly;
            currentPathItem = currentItem;
            txtEntryTitle.Text = currentItem.node.chapter.Title;
            Int64 emptySlots = -1;
            Int64 registerFileSize = -1;
            labelEntries.Text = $"used slots:{Register.Count(dbCtx, this.emptySlotsItem,
                ref emptySlots, ref registerFileSize).ToString()}";
            labelUnusedSlots.Text = $"unused slots:{emptySlots.ToString()}";
            List<RegisterItem>? ancestors = null;
            Register.Lineage(dbCtx, currentItem, ref ancestors, true);//, true);
            String path = Register.LineageFullPath(ancestors);//, currentItem);//, true);
            textboxPath.Text = path;
            labelRWLock.Text = $"write-lock:{readOnly}";
        }
        public bool reloadCurrentTVTreeNodeChildren()
        {
            if (currentPathItem == null) return false;
            if (currentPathItem.nodeType == NodeType.EmptySlot) return false;

            // load all children of this selected node
            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, false, false, false, false, true, true);

            // lineage feature
            TreeNode? tvItem = null;
            TreeNode? tvParent = null;
            List<TreeNode?> tvLineage = null;
            List<RegisterItem?> lineage = null;
            getTreeNodesLineage(currentPathItem, true, ref tvParent, ref tvItem, ref lineage, ref tvLineage);

            RegisterItem? child = currentPathItem.children.Next();
            while (child != null)
            {
                if (child.domainType != DomainType.HiddenCore && child.nodeType != NodeType.EmptySlot)
                    this.Invoke(TVTreeAddItem, child, tvItem);

                child = currentPathItem.children.Next();
            }
            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, true, true);

            return true;
        }

        public bool reloadTreeNodeConfigLabel(TreeNode tvNode)
        {
            RegisterItem? item = Register.LoadSetupRegisterItem(dbCtx, UInt32.Parse(tvNode.Name), true, false, false, false, true, true);
            if (item == null) return false; // node not existent abort with error

            // change tree node label and config
            String label = entryMethods.getEntryLabel(item.node, false);
            String formatted = $"{label} ({item.childrenCount} children)";
            tvNode.Text = formatted;
            return true;
        }

        public void reset()
        {
            this.Invoke(toggleForm, false);

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // reset user interface
            resetRtb(rtbEntry);
            labelEntries.Text = "0";
            txtEntryTitle.Text = "unique title (mandatory for identification)";
            tsslblStateChanged.Text = " ";
            stateChanged = false;
            txtDBFile.Text = "";
            tsbuttonSave.Checked = false;
            tsbuttonSave.BackColor = SystemColors.Control;
            tsslblStateChanged.BackColor = SystemColors.Control;

            // close the db
            //cfg.ctx0.close();
            //dbCtx.close();

            this.Invoke(toggleForm, true);

            formOperation.close();

        }

        public void __loadSelectedEntry(UInt32 id)
        {
            timerSetRtbEntry.Tag = id;
            timerSetRtbEntry.Enabled = true;
            timerSetRtbEntry.Start();
        }

        private void newEntryNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setupNowEntry();
        }

        private void saveEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveEntry();
        }

        public void __rotateInsertState()
        {
            if (!dbCtx.isDBOpen())
                return;

            //todoif (tvEntries.SelectedNode == null)
            //   return;

            String newXamlState = rtbEntry.Xaml;
            xamlState0 = xamlState1;
            xamlState1 = xamlState2;
            xamlState2 = xamlState3;
            xamlState3 = xamlState4;
            xamlState4 = xamlState5;
            xamlState5 = xamlState6;
            xamlState6 = xamlState7;
            xamlState7 = xamlState8;
            xamlState8 = xamlState9;
            xamlState9 = newXamlState;
        }

        public void __saveEntry()
        {
            if (!dbCtx.isDBOpen())
                return;

            if (dbCtx.readOnly)
            {
                MessageBox.Show("error. database read-only.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (currentPathItem == null) return;
            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, true, true);
            if (currentPathItem == null) return;

            // save current caret position
            entryMethods.DBUpdateCaretConfig(dbCtx, currentPathItem.node, rtbEntry.SelectionStartOffset, rtbEntry.SelectionLength);

            // check if body was changed
            if (!stateChanged)
                return;

            // first rotate and save new state for emergency restore
            __rotateInsertState();

            // save the entry
            entryMethods.DBUpdateNodeOFSDB(dbCtx, currentPathItem.node, rtbEntry.Rtf, rtbEntry.XamlBytes, true, true, true);

            // update db config
            dbCtx.dbConfig.lastModifiedEntry = currentPathItem.Id;
            DatabaseConfig.toYamlFile(dbCtx.dbConfig, dbCtx.dbConfigFile);

            stateChanged = false;
            tsslblStateChanged.Text = " ";
            tsslabelLMD.Text = currentPathItem.node.chapter.modificationDateTime.ToString("HH:mm:ss:fff, dddd, dd MMMM yyyy");
            tsbuttonSave.Checked = false;
            tsbuttonSave.BackColor = SystemColors.Control;
            tsslblStateChanged.BackColor = SystemColors.Control;
        }
        public void undortbEntry()
        {
            rtbEntry.Undo();
        }
        public void redortbEntry()
        {
            rtbEntry.Redo();
        }
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undortbEntry();
        }
        private void undoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            undortbEntry();
        }
        public void changeEntryTitle()
        {
            if (currentPathItem == null) return;
            if (currentPathItem.Id == 0) return;

            if (dbCtx.readOnly)
            {
                MessageBox.Show("error. database read-only.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, false, false);

            // system node title cannot be modified
            if (currentPathItem.node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return;

            string? input = currentPathItem.node.chapter.Title;
            if (userInterface.ShowInputDialog("input title for entry", ref input) != DialogResult.OK)
                return;

            if (!entryMethods.DBUpdateNodeTitle(dbCtx, currentPathItem.node, input)) return;

            TreeNode? tvItem = null;
            TreeNode? tvParent = null;
            List<TreeNode?> tvLineage = null;
            List<RegisterItem?> lineage = null;

            // first add new node in tree view
            getTreeNodesLineage(currentPathItem, true, ref tvParent, ref tvItem, ref lineage, ref tvLineage);

            // reconfigure labels
            reloadTreeNodeConfigLabel(tvItem);

        }

        private void entryTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeEntryTitle();
        }

        private void titleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeEntryTitle();
        }

        public void pastertbEntry()
        {
            if (Clipboard.ContainsImage())
            {
                // The clipboard contains an image
                //Console.WriteLine("Image detected!");

                //String tmpfile = Path.Combine(dbCtx.dbTmpPath, Guid.NewGuid().ToString());

                // correction made on 10:12 AM, 02 September 2025
                BitmapSource? src = myCommonMethods1.GetWPFImageFromClipboard();
                //BitmapEncoder encoder = new JpegBitmapEncoder();
                //encoder.Frames.Add(BitmapFrame.Create(src));
                //using (var fileStream = new FileStream(tmpfile, FileMode.Create))
                //{
                //    encoder.Save(fileStream);
                //}

                //Uri uri = new Uri(tmpfile, UriKind.Absolute);
                //BitmapImage bitmapImg = new BitmapImage(uri);

                BitmapImage? bmp = myCommonMethods1.ToBitmapImage(src);
                formatting.formatInsertImageWpf(rtbEntry, bmp);
            }
            else
            {
                // local data
                rtbEntry.Paste();
            }
        }
        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            pastertbEntry();
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pastertbEntry();
        }

        private void rtbEntry_TextChanged(object? sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            //todo if (tvEntries.SelectedNode == null)
            //    return;

            stateChanged = true;
            tsslblStateChanged.Text = "*";
            tsslblStateChanged.BackColor = System.Drawing.Color.Red;
            tsbuttonSave.BackColor = System.Drawing.Color.Red;
            //            tsbuttonSave.Checked = true;
        }
        public void __gotoEntryById(UInt32 id)
        {
            RegisterItem? item = Register.LoadSetupRegisterItem(dbCtx, id, false, false, false, false, true, true);

            // first get old original tree view nodes 
            TreeNode? tvItem = null;
            TreeNode? tvParent = null;
            List<TreeNode?> tvLineage = null;
            List<RegisterItem?> lineage = null;

            // first get old tree nodes
            getTreeNodesLineage(item, true, ref tvParent, ref tvItem, ref lineage, ref tvLineage);

            // load tree view node and it's entry
            tvTree.SelectedNode = tvItem;
        }

        public void __gotoEntryByAttribute(bool lm, bool lc, bool byID, UInt32 id)
        {
            if (lm)
                __gotoEntryById(dbCtx.dbConfig.lastModifiedEntry);
            else if (lc)
                __gotoEntryById(dbCtx.dbConfig.latestCreatedEntry);
            else if (byID)
                __gotoEntryById(id);
        }

        public void copyrtbEntry()
        {
            rtbEntry.Copy();
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            copyrtbEntry();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyrtbEntry();
        }

        public void copyAllrtbEntry()
        {
            rtbEntry.SelectAll();
            rtbEntry.Copy();
        }

        private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyAllrtbEntry();
        }

        private void copyAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            copyAllrtbEntry();
        }

        public void cutrtbEntry()
        {
            rtbEntry.Cut();
        }

        public void cutAllrtbEntry()
        {
            rtbEntry.SelectAll();
            rtbEntry.Cut();
        }

        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cutrtbEntry();
        }

        private void cutAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cutAllrtbEntry();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cutrtbEntry();
        }

        private void cutAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cutAllrtbEntry();
        }

        private void newEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newEntry();
        }

        public void newEntry()
        {
            if (!dbCtx.isDBOpen())
                return;

            DateTime inputDate = DateTime.Now;
            if (userInterface.ShowDateTimeDialog("new entry's date and time", ref inputDate, null) != DialogResult.OK)
                return;

            setupNewEntry(inputDate, false);
        }


        private void tsbuttonSave_Click(object sender, EventArgs e)
        {
            saveEntry();
        }

        private void cmbFonts_Click(object sender, EventArgs e)
        {
        }


        private void tsbuttonBold_Click(object sender, EventArgs e)
        {
            formatting.formatBold(rtbEntry, (ToolStripButton)sender);
        }

        private void tsbuttonItalics_Click(object sender, EventArgs e)
        {
            formatting.formatItalics(rtbEntry, (ToolStripButton)sender);
        }

        private void tsbuttonUnderline_Click(object sender, EventArgs e)
        {
            formatting.formatUnderline(rtbEntry, (ToolStripButton)sender);
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            doExportEntries(EntryType.Html);
        }

        public void doExportEntries(EntryType entryType)
        {
            if (!dbCtx.isDBOpen()) return;

            if (currentPathItem == null) return;

            // firstly save entry
            __saveEntry();

            if (currentPathItem.nodeType == NodeType.EmptySlot) return;

            browseFolder.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            __toggleForm(false);
            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // export tree
            entryMethods.DBExportNodeTree(dbCtx, currentPathItem.Id, browseFolder.SelectedPath, entryType, formOperation);

            formOperation.close();
            __toggleForm(true);

            MessageBox.Show("node tree exported as documents.", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        private void tsbuttonStrikeout_Click(object sender, EventArgs e)
        {
            formatting.formatStrikeout(rtbEntry, (ToolStripButton)sender);
        }

        private void tsbuttonLeftJustify_Click(object sender, EventArgs e)
        {
            formatting.formatLeftJustify(rtbEntry, (ToolStripButton)sender);
        }

        private void tsbuttonRightJustify_Click(object sender, EventArgs e)
        {
            formatting.formatRightJustify(rtbEntry, (ToolStripButton)sender);
        }

        private void tsbuttonJustify_Click(object sender, EventArgs e)
        {
            formatting.formatJustify(rtbEntry, (ToolStripButton)sender);
        }

        private void tsbuttonCenterJustify_Click(object sender, EventArgs e)
        {
            formatting.formatCenterJustify(rtbEntry, (ToolStripButton)sender);
        }

        private void tsbuttonNewEntry_Click(object sender, EventArgs e)
        {
            newEntry();
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            redortbEntry();
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            redortbEntry();
        }

        private void buttonApplyConfig1_Click(object sender, EventArgs e)
        {
            int rtbViewEntryRightMargin = 0;
            if (!int.TryParse(cmbCfgRtbViewEntryRM.Text, out rtbViewEntryRightMargin))
            {
                MessageBox.Show("error configuration. retry after correcting it. aborted.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // first save config which is allowed to be saved while db is loaded.
            rtbViewEntry.RightMargin = rtbViewEntryRightMargin;
            dbCtx.config.cmbCfgRtbViewEntryRMValue = rtbViewEntryRightMargin;

            // now set other 1st level config
            dbCtx.config.radCfgLMNode = radCfgLMNode.Checked;
            dbCtx.config.radCfgLCNode = radCfgLCNode.Checked;
            myConfigMethods.toYamlFile(dbCtx.config, myConfigMethods.getConfigPathFile());
            MessageBox.Show("applied configuration.", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {

            CustomFontDialog fontDialog = new CustomFontDialog();
            fontDialog.font = rtbEntry.SelectionFont;
            fontDialog.size = (int)rtbEntry.SelectionFontSize;
            fontDialog.bold = rtbEntry.Bold;
            fontDialog.italic = rtbEntry.Italic;
            fontDialog.underline = rtbEntry.Underline;
            fontDialog.strikeout = rtbEntry.Strikeout;
            fontDialog.fontColor = rtbEntry.SelectionColor;
            fontDialog.fontBackColor = rtbEntry.SelectionBackColor;
            fontDialog.editor = true;

            if (fontDialog.ShowDialog() != DialogResult.OK)
                return;

            rtbEntry.SelectionFont = fontDialog.font;
            rtbEntry.SelectionColor = fontDialog.fontColor;
            rtbEntry.SelectionBackColor = fontDialog.fontBackColor;
            rtbEntry.SelectionFontSize = fontDialog.size;
            rtbEntry.Bold = fontDialog.bold;
            rtbEntry.Italic = fontDialog.italic;
            rtbEntry.toggleSelectionAllDecorations(rtbEntry.Selection, fontDialog.underline, fontDialog.strikeout);
        }

        private void cmbSize_Click(object sender, EventArgs e)
        {
        }

        private void CmbSize_DropDownClosed(object? sender, EventArgs e)
        {
            if (cmbSize.SelectedItem == null)
                return;

            int value = (int)cmbSize.SelectedItem;

            formatting.formatFontSize(rtbEntry, value);
        }

        private void tsbuttonBullets_Click(object sender, EventArgs e)
        {
            formatting.formatBullets(rtbEntry);

        }

        private void tsbuttonNumberedList_Click(object sender, EventArgs e)
        {
            formatting.formatNumberedList(rtbEntry);

        }

        private void toolStripContainer2_ContentPanel_Load(object sender, EventArgs e)
        {

        }

        private void tsbuttonIndentLeft_Click(object sender, EventArgs e)
        {
            formatting.formatIndentLeft(rtbEntry, tsbuttonIndentLeft, tsbuttonIndentRight);
        }

        private void tsbuttonIndentRight_Click(object sender, EventArgs e)
        {
            formatting.formatIndentRight(rtbEntry, tsbuttonIndentLeft, tsbuttonIndentRight);
        }

        private void lineSpace1_Click(object sender, EventArgs e)
        {
            formatting.formatLineSpacing(rtbEntry, 1.0);
        }

        private void lineSpace1pt5_Click(object sender, EventArgs e)
        {
            formatting.formatLineSpacing(rtbEntry, 1.5);

        }

        private void lineSpace2_Click(object sender, EventArgs e)
        {
            formatting.formatLineSpacing(rtbEntry, 2.0);

        }
        private void toolStripMenuItem60_Click(object sender, EventArgs e)
        {
            formatting.formatLineSpacing(rtbEntry, 4.0);
        }

        private void toolStripMenuItem61_Click(object sender, EventArgs e)
        {
            formatting.formatLineSpacing(rtbEntry, 6.0);

        }
        private void toolStripMenuItem62_Click(object sender, EventArgs e)
        {
            formatting.formatLineSpacing(rtbEntry, 10.0);
        }

        private void toolStripMenuItem63_Click(object sender, EventArgs e)
        {
            formatting.formatLineSpacing(rtbEntry, 14.0);
        }

        private void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertTable();
        }

        public void insertTable()
        {
            formatting.formatInsertTable(rtbEntry);
        }

        private void tsbuttonStore_Click(object sender, EventArgs e)
        {
            store();
        }

        public void store()
        {
            String xaml = rtbEntry.Xaml;
            if (xaml == "") return;

            xamlState = xaml;
        }


        public void restore()
        {
            if (xamlState == "") return;

            previousXamlState = rtbEntry.Xaml;
            rtbEntry.Xaml = xamlState;
        }
        public void undoRestore()
        {
            if (previousXamlState == "") return;

            rtbEntry.Xaml = previousXamlState;
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restore();
        }

        private void restoreToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            restore();
        }

        private void toolStripMenuItem17_Click(object sender, EventArgs e)
        {
            store();
        }

        private void toolStripMenuItem18_Click(object sender, EventArgs e)
        {
            store();
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {

        }

        private void tsbuttonIncreaseFontSize_Click(object sender, EventArgs e)
        {
            formatting.formatIncreaseFontSize(rtbEntry);
        }

        private void tsbuttonDecreaseFontSize_Click(object sender, EventArgs e)
        {
            formatting.formatDecreaseFontSize(rtbEntry);
        }

        private void toolStripMenuItem20_Click(object sender, EventArgs e)
        {
            saveEntry();
            loadDB();
        }

        private void forceSetBoldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatForceSetUnsetBold(rtbEntry, (ToolStripMenuItem)sender, true);
        }

        private void forceUnsetBoldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatForceSetUnsetBold(rtbEntry, (ToolStripMenuItem)sender, false);
        }

        private void forceSetItalicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatForceSetUnsetItalics(rtbEntry, (ToolStripMenuItem)sender, true);
        }

        private void forceUnsetItalicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatForceSetUnsetItalics(rtbEntry, (ToolStripMenuItem)sender, false);
        }

        private void forceSetUnderlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatForceSetUnsetUnderline(rtbEntry, (ToolStripMenuItem)sender, true);
        }

        private void forceUnsetUnderlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatForceSetUnsetUnderline(rtbEntry, (ToolStripMenuItem)sender, false);
        }

        private void forceSetStrikeoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatForceSetUnsetStrikeout(rtbEntry, (ToolStripMenuItem)sender, true);
        }

        private void forceUnsetStrikeoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatForceSetUnsetStrikeout(rtbEntry, (ToolStripMenuItem)sender, false);
        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertImage();
        }

        private void tsbuttonInsertImage_Click(object sender, EventArgs e)
        {
            insertImage();
        }

        public void insertImage()
        {
            ofdFile.Filter = @"*.bmp;*.jpg;*.jpeg;*.gif;*.tiff;*.png|*.bmp;*.jpg;*.jpeg;*.gif;*.tiff;*.png|all files *.*|*.*";
            ofdFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Application.StartupPath;
            if (ofdFile.ShowDialog() != DialogResult.OK)
                return;

            // correction made on 10:12 AM, 02 September 2025
            Uri uri = new Uri(ofdFile.FileName, UriKind.Absolute);
            BitmapImage bitmapImg = new BitmapImage(uri);
            formatting.formatInsertImageWpf(rtbEntry, bitmapImg);

        }

        private void tsbuttonUndo_Click(object sender, EventArgs e)
        {
            undortbEntry();
        }

        private void tsbuttonRedo_Click(object sender, EventArgs e)
        {
            redortbEntry();
        }

        private void tsbuttonRestore_Click(object sender, EventArgs e)
        {
            restore();
        }
        private void undoRestorereadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undoRestore();
        }

        private void tsbuttonUndoRestore_Click(object sender, EventArgs e)
        {
            undoRestore();
        }

        private void toolStripMenuItem22_Click(object sender, EventArgs e)
        {
            doNewChildEntry();
        }

        public void doNewChildEntry()
        {
            if (!dbCtx.isDBOpen())
                return;

            if (currentPathItem != null)
                setupNewEntry(DateTime.Now, true, currentPathItem.Id);
        }
        public void doNewLabelEntry(bool root = false)
        {
            if (!dbCtx.isDBOpen())
                return;

            string? input = "";
            if (userInterface.ShowInputDialog("input entry label (required)", ref input) != DialogResult.OK)
                return;

            if (input.Length <= 0)
                return;

            if (currentPathItem != null)
                setupNewEntry(DateTime.Now, true, currentPathItem.Id, NodeType.Label, input);

        }

        private void newChildEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doNewChildEntry();
        }

        private void highlightCheckedEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            highlightCheckedEntries(false);
        }

        public void highlightEntry(TreeNode treeNode, ref myNode? node)
        {
            // todo
            /*
            if (!dbCtx.isDBOpen())
                return;

            CustomFontDialog fontDialog = new CustomFontDialog();

            if (treeNode.NodeFont == null)
                treeNode.NodeFont = cfg.tvEntriesFont;//new System.Drawing.Font("Arial", 8);

            fontDialog.font = treeNode.NodeFont;
            fontDialog.fontColor = treeNode.ForeColor;
            fontDialog.fontBackColor = treeNode.BackColor;
            fontDialog.size = (int)treeNode.NodeFont.Size;
            fontDialog.bold = treeNode.NodeFont.Bold;
            fontDialog.italic = treeNode.NodeFont.Italic;
            fontDialog.underline = treeNode.NodeFont.Underline;
            fontDialog.strikeout = treeNode.NodeFont.Strikeout;
            fontDialog.editor = false;
            if (fontDialog.ShowDialog() != DialogResult.OK)
                return;

            entryMethods.setEntryHighlightFontComplete(dbCtx, ref node, fontDialog.fontColor, fontDialog.fontBackColor, fontDialog.font);
            loadNodeHighlight(dbCtx, treeNode, ref node);
            */
        }

        // this method sets the highlights and font for a given tree node
        public static void loadNodeHighlight(OpenFSDBContext? ctx, TreeNode treeNode, ref myNode? node)
        {
            /*
            TreeNode tmpNode = new TreeNode();
            tmpNode.NodeFont = ctx.config.tvEntriesFont;// new System.Drawing.Font("Arial", 8, FontStyle.Regular);

            if (node.chapter.HLFont.Length >= 1)
                treeNode.NodeFont = myCommonMethods1.StringToFont(node.chapter.HLFont);
            else
                treeNode.NodeFont = tmpNode.NodeFont;

            if (node.chapter.HLFontColor.Length >= 1)
                treeNode.ForeColor = myCommonMethods1.StringToColor(node.chapter.HLFontColor);
            else
                treeNode.ForeColor = tmpNode.ForeColor;

            if (node.chapter.HLBackColor.Length >= 1)
                treeNode.BackColor = myCommonMethods1.StringToColor(node.chapter.HLBackColor);
            else
                treeNode.BackColor = tmpNode.BackColor;
            */
        }

        public void loadNodeHighlight(TreeNode treeNode)
        {
            // first find the node
            UInt32 id = UInt32.Parse(treeNode.Name);
            myNode? node = entryMethods.FindNodeInList(allNodes, id);
            if (node == null)
                return;

            if (node.chapter.HLFont.Length >= 1)
                treeNode.NodeFont = myCommonMethods1.StringToFont(node.chapter.HLFont);

            if (node.chapter.HLFontColor.Length >= 1)
                treeNode.ForeColor = myCommonMethods1.StringToColor(node.chapter.HLFontColor);

            if (node.chapter.HLBackColor.Length >= 1)
                treeNode.BackColor = myCommonMethods1.StringToColor(node.chapter.HLBackColor);
        }


        public void highlightCheckedEntries(bool reset)
        {
            /* todo

            if (!dbCtx.isDBOpen())
                return;

            if (tvEntries.Nodes.Count <= 0)
                return;

            CustomFontDialog fontDialog = new CustomFontDialog();
            // create a temporary tree node and extract it's default values
            TreeNode treeNode = new TreeNode();
            fontDialog.font = treeNode.NodeFont;
            fontDialog.fontColor = treeNode.ForeColor;
            fontDialog.fontBackColor = treeNode.BackColor;
            if (treeNode.NodeFont == null)
                treeNode.NodeFont = cfg.tvEntriesFont;// new System.Drawing.Font("Arial", 8);
            fontDialog.size = (int)treeNode.NodeFont.Size;
            fontDialog.bold = treeNode.NodeFont.Bold;
            fontDialog.italic = treeNode.NodeFont.Italic;
            fontDialog.underline = treeNode.NodeFont.Underline;
            fontDialog.strikeout = treeNode.NodeFont.Strikeout;
            fontDialog.editor = false;

            if (!reset)
            {
                // if user selected a tree node it should be the default value
                if (tvEntries.SelectedNode != null)
                {
                    fontDialog.fontColor = tvEntries.SelectedNode.ForeColor;
                    fontDialog.fontBackColor = tvEntries.SelectedNode.BackColor;
                    if (tvEntries.SelectedNode.NodeFont != null)
                    {
                        fontDialog.font = tvEntries.SelectedNode.NodeFont;
                        fontDialog.size = (int)tvEntries.SelectedNode.NodeFont.Size;
                        fontDialog.bold = tvEntries.SelectedNode.NodeFont.Bold;
                        fontDialog.italic = tvEntries.SelectedNode.NodeFont.Italic;
                        fontDialog.underline = tvEntries.SelectedNode.NodeFont.Underline;
                        fontDialog.strikeout = tvEntries.SelectedNode.NodeFont.Strikeout;
                    }
                }
                if (fontDialog.ShowDialog() != DialogResult.OK)
                    return;
            }

            TreeNodeCollection rootNodes = tvEntries.Nodes;
            foreach (TreeNode rootNode in rootNodes)
            {
                Queue<TreeNode> queue = new Queue<TreeNode>();
                queue.Enqueue(rootNode);
                while (queue.Count > 0)
                {
                    // the first node is dequeued and processed first, then it's children are processed level by level.
                    TreeNode currentNode = queue.Dequeue();

                    // get children of this node.
                    TreeNodeCollection children = currentNode.Nodes;

                    // add all children is queue, they will be processed in this same way in this same place: 1st parent node, 2nd children nodes.
                    foreach (TreeNode childNode in children)
                        queue.Enqueue(childNode);

                    if (!currentNode.Checked)
                        continue;

                    highlightTreeViewNode(currentNode, fontDialog.font, fontDialog.fontColor, fontDialog.fontBackColor, reset);
                }
            }

            //if (cfg.ctx0.isDBOpen())
            //    SingleFileDB.Checkpoint(cfg.ctx0);
            */
        }

        public void highlightTreeViewNode(TreeNode treeNode, System.Drawing.Font font, System.Drawing.Color color, System.Drawing.Color backColor, bool reset)
        {
            UInt32 id = UInt32.Parse(treeNode.Name);
            myNode? node = entryMethods.FindNodeInList(allNodes, id);
            if (node == null)
                return;

            treeNode.ForeColor = color;
            treeNode.BackColor = backColor;
            treeNode.NodeFont = font;
            if (reset)
                entryMethods.setEntryClearHighlight(dbCtx, node);
            else
                entryMethods.setEntryHighlightFontComplete(dbCtx, node, color, backColor, font);

            loadNodeHighlight(treeNode);

        }
        private void highlightSelectedEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* todo
            if (!dbCtx.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            Int64 id = Int64.Parse(tvEntries.SelectedNode.Name);
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return;

            highlightEntry(tvEntries.SelectedNode, ref node);
            */
        }

        public void highlightEntryBackColor(TreeNode treeNode)
        {
            if (!dbCtx.isDBOpen())
                return;

            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = treeNode.BackColor;

            if (colorDialog.ShowDialog() != DialogResult.OK)
                return;

            UInt32 id = UInt32.Parse(treeNode.Name);
            myNode? node = entryMethods.FindNodeInList(allNodes, id);
            if (node == null)
                return;

            entryMethods.setEntryHighlightBackColor(dbCtx, node, colorDialog.Color);
            loadNodeHighlight(treeNode);
        }

        private void tsbuttonSearch_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            saveEntry();

            __resetLVSearch();

            bgWorkerSearch.RunWorkerAsync();
        }

        public bool __processSearch()
        {
            // init search
            List<UInt32> locations = lvItemsRegisterItemIds(lvSearchWhere);
            /*
             if (locations.Count == 0)
            {
                // no location provided, auto set to root 0
                RegisterItem? root = Register.LoadSetupRegisterItem(dbCtx, 0, false, false, false, false, false, false);
                if (root == null) return; // error abort
                locations.Add(root);
            }
            */

            bool result = journalSearchFramework.searchEntries(dbCtx, this, null, txtSearchProgressFullPath, lvSearch, tsSearchProgressBar,
              dtpickerSearchFrom.Value, dtpickerSearchFromTime.Value, dtpickerSearchThrough.Value, dtpickerSearchThroughTime.Value, chkSearchUseDateRange.Checked,
              dtpickerCDSearchFrom.Value, dtpickerCDSearchFromTime.Value, dtpickerCDSearchThrough.Value, dtpickerCDSearchThroughTime.Value, chkSearchUseCreationDateRange.Checked,
              dtpickerMDSearchFrom.Value, dtpickerMDSearchFromTime.Value, dtpickerMDSearchThrough.Value, dtpickerMDSearchThroughTime.Value, chkSearchUseModificationDateRange.Checked,
              rtbSearch.Text, rtbSearchReplace.Text, chkSearchAll.Checked,
              chkSearchMatchCase.Checked, chkSearchMatchWholeWord.Checked,
              chkSearchReplace.Checked, chkSearchReplaceTitle.Checked, chkSearchEmptyString.Checked, locations);

            reload();
            //return result;
            return true;
        }


        private void bgWorkerSearch_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            this.Invoke(toggleForm, false);
            this.Invoke(LvSearchUpdate, true);
            bool searchResult = (bool)this.Invoke(processSearch);
            this.Invoke(LvSearchUpdate, false);
            this.Invoke(toggleForm, true);

        }

        public List<UInt32> lvItemsRegisterItemIds(ListView lv)
        {
            List<UInt32> list = lv.Items.Cast<ListViewItem>()
                                       .Select(c => (UInt32)c.Tag)
                                       .ToList();

            return list;
        }
        public void __resetLVSearch()
        {
            lvSearch.Items.Clear();
        }


        public void __LvSearchUpdate(bool set)
        {
            if (set)
                lvSearch.BeginUpdate();
            else
                lvSearch.EndUpdate();
        }

        private void lvSearch_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tsbuttonOpenSearchedEntry_Click(object sender, EventArgs e)
        {
            UInt32 id = 0;
            if (getSelectedListViewNodeId(lvSearch, out id))
                OpenSearchedEntry(id);
        }

        private void toolStripSearch_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        public void checkAllSearchedItems(bool check)
        {
            foreach (System.Windows.Forms.ListViewItem listViewItem in lvSearch.Items)
                listViewItem.Checked = check;
        }

        private void tsbuttonChkAllSearchedEntries_Click(object sender, EventArgs e)
        {
            checkAllSearchedItems(true);
        }

        private void tsbuttonUnchkAllSearchedEntries_Click(object sender, EventArgs e)
        {
            checkAllSearchedItems(false);
        }

        private void tsbuttonResetSearch_Click(object sender, EventArgs e)
        {
            __resetLVSearch();
            rtbSearch.Clear();
            rtbSearchReplace.Clear();
            dtpickerSearchFrom.Value = DateTime.Now;
            dtpickerSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerSearchThrough.Value = DateTime.Now;
            dtpickerSearchThroughTime.Value = DateTime.Parse("23:59:59");
            dtpickerCDSearchFrom.Value = DateTime.Now;
            dtpickerCDSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerCDSearchThrough.Value = DateTime.Now;
            dtpickerCDSearchThroughTime.Value = DateTime.Parse("23:59:59");
            dtpickerMDSearchFrom.Value = DateTime.Now;
            dtpickerMDSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerMDSearchThrough.Value = DateTime.Now;
            dtpickerMDSearchThroughTime.Value = DateTime.Parse("23:59:59");
            lvSearchWhere.Tag = null;
            lvSearchWhere.Items.Clear();
        }

        public void deleteSearchedList()
        {
            if (!dbCtx.isDBOpen())
                return;

            saveEntry();

            if (MessageBox.Show("warning: are you sure you want to purge checked nodes? all of their descendants trees will also be forever purged along with them!\n" +
            "there is no need to purge anything! you can recycle and reuse the nodes! you have 8+ million slots!", "warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            List<myNode>? worklist = new List<myNode>();
            worklist.AddRange(allNodes);
            foreach (System.Windows.Forms.ListViewItem listViewItem in lvSearch.CheckedItems)
            {
                // delete checked nodes
                UInt32 id = UInt32.Parse(listViewItem.Name);
                RegisterItem? item = Register.LoadSetupRegisterItem(dbCtx, id, false, false, false, false, true, true);
                if (item == null) continue;
                this.emptySlotsItem = Register.LoadSetupRegisterItem(dbCtx, this.emptySlotsItem.Id, false, false, false, false, true, true);
                item.tree.Delete(item, emptySlotsItem);
            }

            this.Invoke(toggleForm, false);
            formOperation.close();

            tvTree.SelectedNode = tvTree.Nodes.Find("0", false).FirstOrDefault();
        }

        private void tsbuttonDeleteSearchedEntry_Click(object sender, EventArgs e)
        {
            deleteSearchedList();
        }

        private void copyToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)cms.SourceControl;
            rtb.Copy();
        }

        private void rtbSearchPattern_TextChanged(object sender, EventArgs e)
        {

        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)cms.SourceControl;
            rtb.SelectAll();

        }

        private void pasteToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)cms.SourceControl;
            rtb.Paste();
        }

        private void toolStripMenuItem23_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)cms.SourceControl;
            rtb.Cut();

        }

        private void increaseFontSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatIncreaseFontSize(rtbEntry);
        }

        private void decreaseFontSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatDecreaseFontSize(rtbEntry);
        }

        private void toolStripMenuItem24_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            findAndReplace();
        }

        public void findAndReplace()
        {
            if (!dbCtx.isDBOpen())
                return;

            if (myFormFind != null)
                return;

            myFormFind = new FormFind();
            // todo tushar: myFormFind.myParentForm = this;
            myFormFind.rtb = rtbEntry;
            myFormFind.rtbSearchPattern.Text = rtbEntry.Selection.Text;
            myFormFind.Show(this);
        }

        private void searchAllEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlJournal.SelectedIndex = tabControlJournal.TabPages.IndexOfKey("TabPageSearch");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (stateChanged)
            {
                if (MessageBox.Show(this, "do you wish to save the currently active changed entry?", "question",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    saveEntry();
            }

            this.Close();
            if (this.myParentForm != null)
                this.myParentForm.Show();

        }


        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            if (menuItem.Checked)
                menuItem.Checked = false;
            else
                menuItem.Checked = true;

            userInterface.GoFullscreen(this, menuItem.Checked);

        }

        private void buttonSearchResetDates_Click(object sender, EventArgs e)
        {
            dtpickerSearchFrom.Value = DateTime.Now;
            dtpickerSearchThrough.Value = DateTime.Now;
            dtpickerSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerSearchThroughTime.Value = DateTime.Parse("23:59:59");
        }

        private void toolStripMenuItem33_Click(object sender, EventArgs e)
        {
        }

        private void bgWorkerSingleDBToFSDB_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
        }

        private void newLabelNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doNewLabelEntry();
        }

        public void __exportEntries(EntryType entryType, bool loadOperationForm)
        {
            /* todo
            // first save the entry
            this.Invoke(saveEntry);

            if (!dbCtx.isDBOpen())
                return;

            this.Invoke(toggleForm, false);

            // operations status form
            FormOperation? formOperation = null;
            if (loadOperationForm)
                formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // first get the total number of chapters which exist in db
            long allNodesCount = allNodes.LongCount();

            // now get the custom checked root nodes
            List<myNode> checkedNodes = (List<myNode>)this.Invoke(getHighestCheckedTreeViewItemsDBNodes, tvEntries);

            // load tree document object model structure
            myTreeDom treeDom = new myTreeDom();
            treeDom.buildCustomTree(ref allNodes, ref checkedNodes, true, false);
            List<myTreeDomNode> tree = treeDom.ToList();
            long nodesCount = tree.LongCount();
            Int64 exportIndex = 0;

            foreach (myTreeDomNode listedNode in tree)
            {
                // load the rtf from current node
                myNode? node = listedNode.self;

                // straightaway export the entry
                // tushar original 15 February 2024: entryMethods.exportEntry(dbCtx, ref node, browseFolder.SelectedPath, false, exportIndex, entryType);
                entryMethods.exportEntry(dbCtx, ref node, browseFolder.SelectedPath, false, exportIndex, entryType);
                //entryMethods.exportEntry(dbCtx, ref node, browseFolder.SelectedPath, false, node.chapter.Id, entryType);

                // update ui
                if (loadOperationForm)
                {
                    formOperation.updateProgressBar(exportIndex, nodesCount);
                    formOperation.updateFilesStatus(exportIndex, nodesCount);
                }

                // update
                exportIndex++;
            }

            // entire tree structure export completed. now final update and exit.
            this.Invoke(toggleForm, true);

            if (loadOperationForm)
                formOperation.close();

            this.Invoke(showMessageBox, "total entries exported:" + exportIndex, "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            */
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            doExportEntries(EntryType.Txt);
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            doExportEntries(EntryType.Rtf);
        }

        private void promoteNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            if (currentPathItem == null) return;
            if (currentPathItem.Id == 0) return;

            if (dbCtx.readOnly)
            {
                MessageBox.Show("error. database read-only.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // firstly save entry
            __saveEntry();

            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, true, true);

            // skip if this is system node, we cannot change it
            if (currentPathItem.node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return;

            // first get old original tree view nodes 
            TreeNode? tvOldItem = null;
            TreeNode? tvOldParent = null;
            TreeNode? tvNewItem = null;
            TreeNode? tvNewParent = null;
            List<TreeNode?> tvLineage = null;
            List<RegisterItem?> lineage = null;

            // first get old tree nodes
            getTreeNodesLineage(currentPathItem, true, ref tvOldParent, ref tvOldItem, ref lineage, ref tvLineage);

            // update node in db
            if (!entryMethods.DBPromoteNodeOFSDB(dbCtx, currentPathItem.node)) return; // error abort

            // get current register item latest state
            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, true, true);

            // now setup new tree view nodes
            getTreeNodesLineage(currentPathItem, true, ref tvNewParent, ref tvNewItem, ref lineage, ref tvLineage);
            // reconfigure labels
            reloadTreeNodeConfigLabel(tvNewParent);

            // now remove tree node from old place
            tvOldParent.Nodes.Remove(tvOldItem);
            // reconfigure labels
            reloadTreeNodeConfigLabel(tvOldParent);

            // get current register item latest state
            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, true, true);

            // finally reload path
            tvTree.SelectedNode = tvNewItem;
        }

        private void moveNodeToRootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveNode(0);
        }

        private void promoteNodeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void moveNodeToRootToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }
        public void MoveNode(UInt32 destId)
        {
            if (!dbCtx.isDBOpen())
                return;

            if (currentPathItem == null) return;
            if (currentPathItem.Id <= 0) return;
            if (currentPathItem.Id == destId) return;
            if (currentPathItem.parentId == destId) return;

            if (dbCtx.readOnly)
            {
                MessageBox.Show("error. database read-only.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // firstly save entry
            __saveEntry();

            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, true, true);

            // skip if this is system node, we cannot change it
            if (currentPathItem.node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return;

            // first get old original tree view nodes 
            TreeNode? tvOldItem = null;
            TreeNode? tvOldParent = null;
            TreeNode? tvNewItem = null;
            TreeNode? tvNewParent = null;
            List<TreeNode?> tvLineage = null;
            List<RegisterItem?> lineage = null;

            // first get old tree nodes
            getTreeNodesLineage(currentPathItem, true, ref tvOldParent, ref tvOldItem, ref lineage, ref tvLineage);

            // validate if destination is not descendant of this item
            RegisterItem? destItem = Register.LoadSetupRegisterItem(dbCtx, destId, false, false, false, false, false, false);
            if (destItem == null) return; // error destination item does not exists
            if (Register.IsDescendantOfAncestor(dbCtx, destItem, currentPathItem)) return; // error destination item is descendant
            //RegisterItem? result = lineage.Find(item => item.Id == destId);

            // get current register item latest state
            RegisterItem? parent = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.parentId, true, false, false, false, true, true);
            if (parent == null) return;
            RegisterItem? dst = Register.LoadSetupRegisterItem(dbCtx, destId, true, false, false, false, true, true);
            if (dst == null) return;

            // move from parent to another location
            if (!parent.children.Move(currentPathItem, dst))
            {
                MessageBox.Show("error, node not moved. destination children limit reached or some other error.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // get current register item latest state
            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, true, true);

            // now setup new tree view nodes
            getTreeNodesLineage(currentPathItem, true, ref tvNewParent, ref tvNewItem, ref lineage, ref tvLineage);
            // reconfigure labels
            reloadTreeNodeConfigLabel(tvNewParent);

            // now remove tree node from old place
            tvOldParent.Nodes.Remove(tvOldItem);
            // reconfigure labels
            reloadTreeNodeConfigLabel(tvOldParent);

            // get current register item latest state
            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, true, true);

            // finally reload path
            tvTree.SelectedNode = tvNewItem;


        }
        public void CloneNode(UInt32 destId)
        {
            if (!dbCtx.isDBOpen())
                return;

            if (currentPathItem == null) return;
            if (currentPathItem.Id <= 0) return;

            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, true, true);

            // skip if this is system node, we cannot change it
            if (currentPathItem.node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return;

            // first get old original tree view nodes 
            TreeNode? tvOldItem = null;
            TreeNode? tvOldParent = null;
            TreeNode? tvNewItem = null;
            TreeNode? tvNewParent = null;
            List<TreeNode?> tvLineage = null;
            List<RegisterItem?> lineage = null;

            // first get old tree nodes
            getTreeNodesLineage(currentPathItem, true, ref tvOldParent, ref tvOldItem, ref lineage, ref tvLineage);

            RegisterItem? clone = null;
            if (!cloneEntry(currentPathItem.Id, destId, out clone)) // clone at some location
            {
                MessageBox.Show(this, "error occured while cloning the current node.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // setup
            currentPathItem = clone;

            // get current register item latest state
            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, true, true);

            // now setup new tree view nodes
            getTreeNodesLineage(currentPathItem, true, ref tvNewParent, ref tvNewItem, ref lineage, ref tvLineage);
            // reconfigure labels
            reloadTreeNodeConfigLabel(tvNewParent);

            // get current register item latest state
            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, true, true);

            // finally reload path
            tvTree.SelectedNode = tvNewItem;
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            if (currentPathItem == null) return;
            if (currentPathItem.Id <= 0) return;

            if (dbCtx.readOnly)
            {
                MessageBox.Show("error. database read-only.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, false, false);

            // skip if this is system node, we cannot change it
            if (currentPathItem.node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return;

            FormTreeDesign2 form = new FormTreeDesign2();
            form.ctx = dbCtx;
            if (form.ShowDialog() != DialogResult.OK) return;
            if (form.selectedPathItem == null) return;

            MoveNode(form.selectedPathItem.Id);
        }

        private void moveNodeToToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void exportEntryAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* todo
            TreeNode? treeNode = tvEntries.SelectedNode;
            if (treeNode == null)
                return;

            Int64 id = Int64.Parse(treeNode.Name);

            doExportCustomEntry(id);
            */
        }

        public RegisterItem? doExportCustomEntry(UInt32 id)
        {
            // get latest state register item
            RegisterItem? item = Register.LoadSetupRegisterItem(dbCtx, id, true, false, false, false, false, false);
            if (item == null) return null;

            sfdFile.Filter = "*.rtf|*.rtf|*.xaml|*.xaml|*.txt|*.txt|*.html|*.html|*.pdf|*.pdf";
            sfdFile.FilterIndex = 1;
            sfdFile.Title = "save as";
            sfdFile.FileName = entryMethods.getEntryLabel(item.node, true, true);

            if (sfdFile.ShowDialog() != DialogResult.OK)
                return null;

            if (sfdFile.FileName.Length <= 0)
                return null;

            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            EntryType entryType = EntryType.Rtf;
            entryMethods.getEntryTypeFormatsByFileName(sfdFile.FileName, ref entryType, ref ext, ref extComplete, ref extSearchPattern);

            // straightaway export the entry
            if (entryMethods.exportEntry(dbCtx, ref item.node, sfdFile.FileName, true, 0, entryType))
                MessageBox.Show(this, "entry exported", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);

            return item;
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            if (currentPathItem == null) return;

            doExportCustomEntry(currentPathItem.Id);
        }

        private void timeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatInsertTime(rtbEntry, DateTime.Now);
        }

        private void dateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatInsertDate(rtbEntry, DateTime.Now);
        }

        private void toolStripMenuItem25_Click(object sender, EventArgs e)
        {
            /* todo
            if (!dbCtx.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            Int64 id = Int64.Parse(tvEntries.SelectedNode.Name);
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null) return;

            clearHighlight(tvEntries.SelectedNode, ref node);
            */
        }

        public void clearHighlight(TreeNode treeNode, ref myNode? node)
        {
            if (!dbCtx.isDBOpen())
                return;

            entryMethods.setEntryClearHighlight(dbCtx, node);
            loadNodeHighlight(dbCtx, treeNode, ref node);
        }

        private void toolStripMenuItem26_Click(object sender, EventArgs e)
        {
            highlightCheckedEntries(true);
        }

        public void LVMarkUnmark(ListView lv, bool checkmark = true, bool selected = true, bool checkItems = false, bool selectItems = false)
        {
            lv.BeginUpdate();
            foreach (ListViewItem item in lv.Items)
            {
                if (item.SubItems[1].Text == @"\") { continue; }
                else if (item.SubItems[1].Text == @".") { continue; }
                else if (item.SubItems[1].Text == @"..") { continue; }

                if (checkmark)
                {
                    if (checkItems)
                        item.Checked = true;
                    else
                        item.Checked = false;

                }
                if (selected)
                {
                    if (selectItems)
                        item.Selected = true;
                    else
                        item.Selected = false;
                }
            }
            lv.EndUpdate();
        }

        private void checkAllNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void uncheckAllNodesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem27_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            DateTime inputDate = DateTime.Now;
            if (userInterface.ShowDateTimeDialog("custom time", ref inputDate, null) != DialogResult.OK)
                return;

            formatting.formatInsertTime(rtbEntry, inputDate);

        }

        private void toolStripMenuItem28_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            DateTime inputDate = DateTime.Now;
            if (userInterface.ShowDateTimeDialog("custom date", ref inputDate, null) != DialogResult.OK)
                return;

            formatting.formatInsertDate(rtbEntry, inputDate);


        }

        private void toolStripMenuItem29_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            DateTime inputDate = DateTime.Now;
            if (userInterface.ShowDateTimeDialog("custom date and time", ref inputDate, null) != DialogResult.OK)
                return;

            formatting.formatInsertDateTime(rtbEntry, inputDate);


        }
        private void toolStripMenuItem30_Click(object sender, EventArgs e)
        {
            formatting.formatForceRemoveAllFormatting(rtbEntry);
        }

        private void toolStripMenuItem32_Click(object sender, EventArgs e)
        {
            doNewLabelEntry();
        }

        private void toolStripMenuItem33_Click_1(object sender, EventArgs e)
        {
            doNewLabelEntry(true);
        }
        private void toolStripMenuItem34_Click(object sender, EventArgs e)
        {
            doExportEntries(EntryType.Pdf);
        }
        private void toolStripMenuItem38_Click(object sender, EventArgs e)
        {
            doExportEntries(EntryType.Xaml);

        }

        private void closeAllTreeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void expandAllTreeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void closeSelectedNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void expandSelectedNodeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void buttonSearchResetCDates_Click(object sender, EventArgs e)
        {
            dtpickerCDSearchFrom.Value = DateTime.Now;
            dtpickerCDSearchThrough.Value = DateTime.Now;
            dtpickerCDSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerCDSearchThroughTime.Value = DateTime.Parse("23:59:59");
        }

        private void buttonSearchResetMDates_Click(object sender, EventArgs e)
        {
            dtpickerMDSearchFrom.Value = DateTime.Now;
            dtpickerMDSearchThrough.Value = DateTime.Now;
            dtpickerMDSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerMDSearchThroughTime.Value = DateTime.Parse("23:59:59");
        }

        private void toolStripMenuItem37_Click(object sender, EventArgs e)
        {
            setupCommonDateTime();
        }
        public void setupCommonDateTime()
        {
            if (!dbCtx.isDBOpen())
                return;

            if (currentPathItem == null) return;
            if (currentPathItem.Id == 0) return;

            if (dbCtx.readOnly)
            {
                MessageBox.Show("error. database read-only.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, false, false);

            DateTime inputDate = currentPathItem.node.chapter.chapterDateTime;
            if (userInterface.ShowDateTimeDialog("edit entry's date and time", ref inputDate, null) != DialogResult.OK)
                return;

            // update node in db
            RegisterItem? item = entryMethods.DBSetNodeCommonDateTimeOFSDB(dbCtx, currentPathItem.Id, inputDate);
            if (item == null)
            {
                MessageBox.Show("error occured while changing node common date time", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TreeNode? tvItem = null;
            TreeNode? tvParent = null;
            List<TreeNode?> tvLineage = null;
            List<RegisterItem?> lineage = null;

            // first add new node in tree view
            getTreeNodesLineage(currentPathItem, true, ref tvParent, ref tvItem, ref lineage, ref tvLineage);

            // reconfigure labels
            reloadTreeNodeConfigLabel(tvItem);
        }

        private void entryCommonDateAndTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        public void doSortTreeNodeFirstLevelChildren(TreeView? tv, TreeNode? treeNode, ref FormSortOptions? form)
        {
        }
        public void doSortTreeNodeRecursive(TreeView? tv, TreeNode? treeNode, ref FormSortOptions? form)
        {
        }

        private void toolStripMenuItem40_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem41_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem42_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem43_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem44_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem45_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem46_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem47_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem48_Click(object sender, EventArgs e)
        {
        }

        private void exportAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.ListView lv = (System.Windows.Forms.ListView)cms.SourceControl;
            if (lv == null) return;
            if (lv.SelectedItems.Count == 0) return;
            System.Windows.Forms.ListViewItem lvitem = lv.SelectedItems[0];

            // load node
            UInt32 id = UInt32.Parse(lvitem.Name);
            doExportCustomEntry(id);

        }

        private void lvTrashCan_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem49_Click(object sender, EventArgs e)
        {
            doImportNonCalendarRtfEntries(false);
        }
        public void doImportNonCalendarRtfEntries(bool alienEntries)
        {
            if (!dbCtx.isDBOpen())
                return;

            if (dbCtx.readOnly)
            {
                MessageBox.Show("error. database read-only.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            browseFolder.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Application.StartupPath;
            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            string? input = "the journal non-calendar set";
            if (alienEntries)
                input = "other non-calendar import set";

            if (userInterface.ShowInputDialog("input new clone set name/title", ref input) != DialogResult.OK)
                return;

            if (input.Length <= 0)
                return;


            __importTheJournalNonCalendarRtfEntriesNew(browseFolder.SelectedPath, input);

        }

        public void __importTheJournalNonCalendarRtfEntriesNew(String path, String importSetName)
        {
            if (!dbCtx.isDBOpen())
                return;

            // first save the entry
            saveEntry();

            // first initialize
            IEnumerable<FileInfo> files = myCommonMethods1.EnumerateFiles(path, EntryType.Rtf);
            Int32 index = 0;
            Int32 total = files.Count();
            if (total <= 0) return;

            // first create set top node
            RegisterItem? setNode = null;
            if (!ForceSetupNewEntrySilent(DateTime.Now, ref setNode, null, 0, true, NodeType.Set, importSetName, "", null))
            {
                MessageBox.Show("error, set node not created", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // error set node not created
            }

            __toggleForm(false);
            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // reload
            setNode = Register.LoadSetupRegisterItem(dbCtx, setNode.Id, true, false, false, false, true, true);

            // this set's own context session work list
            List<myNode> setList = new List<myNode>();

            foreach (FileInfo file in files)
            {
                // 2nd load rtf from file
                String? rtf = File.ReadAllText(file.FullName);

                // notice: any tree node level in entire tree cannot have more than 1 nodes with exactly same name/title.
                // this leads to destruction of nodes of same name and level while importing.
                // so you must ensure there is no duplicate named/titled node in any tree level.
                // if there is any duplicate node, you must give it a unique name/title which differs in all children of a tree node level.
                // also, each and every entry which is to be imported, must have a unique name/title in it's filename.

                // reload empty slots register
                emptySlotsItem = Register.LoadSetupRegisterItem(dbCtx, emptySlotsItem.Id, false, false, false, false, true, true);

                // auto create direct all nodes line by node names which exist in the file name itself.
                List<String> nodeNames = theJournalMethods.partitionEntryFileIntoNodes(file.FullName);
                List<myNode> nodesLine = theJournalMethods.initNodesLineTJNC(dbCtx, ref setList, ref nodeNames, setNode.node.chapter.Id, emptySlotsItem);
                myNode? targetNode = nodesLine.Last();

                byte[]? xaml = null;
                if (dbCtx.dbEntryType == EntryType.Xaml)
                {
                    xamlEntry.dummy.Rtf = rtf;
                    xaml = xamlEntry.dummy.XamlBytes;
                }

                // finally write body of the node in db
                entryMethods.DBUpdateNodeOFSDB(dbCtx, targetNode, rtf, xaml, true, false, false, EntryType.Default);

                // update progress
                formOperation.updateProgressBar(index, total);
                formOperation.updateFilesStatus(index++, total);

            }

            formOperation.close();
            __toggleForm(true);

            // reset reload to root
            tvTree.SelectedNode = tvTree.Nodes.Find("0", false).FirstOrDefault();

            MessageBox.Show($"{index} entries imported of total {total} entries.", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        public void doNewCustomNode(NodeType nodeType, bool root = false)
        {
            if (!dbCtx.isDBOpen())
                return;

            string? input = "";
            if (userInterface.ShowInputDialog("input node title (required)", ref input) != DialogResult.OK)
                return;

            DateTime inputDate = DateTime.Now;
            if (userInterface.ShowDateTimeDialog("new node's date and time", ref inputDate, null) != DialogResult.OK)
                return;

            if (input.Length <= 0)
                return;

            /* todo
            if (!root)
            {
                if (tvEntries.SelectedNode == null)
                    return;

                Int64 parentId = Int64.Parse(tvEntries.SelectedNode.Name);
                setupNewEntry(inputDate, parentId, nodeType, input);
            }
            else
            {
                setupNewEntry(inputDate, -1, nodeType, input);
            }
            */
        }

        private void toolStripMenuItem52_Click(object sender, EventArgs e)
        {
            printEntry(rtbEntry, false);
        }

        public void printEntry(System.Windows.Controls.WpfRichTextBoxEx? rtb, bool printPreview = false)
        {
            // first save the entry
            this.Invoke(saveEntry);

            if (rtb == null) return;

            // init and configure
            printerRtb.Clear();
            printerRtb.Rtf = rtb.Rtf;

            // first page setup
            if (pdlgRtbEntry.ShowDialog() != DialogResult.OK) return;

            // 2nd print preview dialog
            if (printPreview)
                if (ppDlgRtbEntry.ShowDialog() != DialogResult.OK) return;

            // finally print
            //if ((pd.ShowDialog() == true))
            // {
            //     pd.PrintDocument((((IDocumentPaginatorSource)rtbEntry.Document).DocumentPaginator),
            //        "Print Document");
            // }

            printerRtb.PrintRichTextContents();
        }

        private void toolStripMenuItem53_Click(object sender, EventArgs e)
        {
            printEntry(rtbEntry, true);
        }

        /// <summary>
        /// Gets the text pointer at the given character offset.
        /// Each line break will count as 2 chars.
        /// </summary>
        /// <param name="richTextBox">The rich text box.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The TextPointer at the given character offset</returns>
        public static TextPointer GetTextPointerAtOffset(System.Windows.Controls.RichTextBox richTextBox, int offset)
        {
            var navigator = richTextBox.Document.ContentStart;
            int cnt = 0;

            while (navigator.CompareTo(richTextBox.Document.ContentEnd) < 0)
            {
                switch (navigator.GetPointerContext(LogicalDirection.Forward))
                {
                    case TextPointerContext.ElementStart:
                        break;
                    case TextPointerContext.ElementEnd:
                        if (navigator.GetAdjacentElement(LogicalDirection.Forward) is Paragraph)
                            cnt += 2;
                        break;
                    case TextPointerContext.EmbeddedElement:
                        // TODO: Find out what to do here?
                        cnt++;
                        break;
                    case TextPointerContext.Text:
                        int runLength = navigator.GetTextRunLength(LogicalDirection.Forward);

                        if (runLength > 0 && runLength + cnt < offset)
                        {
                            cnt += runLength;
                            navigator = navigator.GetPositionAtOffset(runLength);
                            if (cnt > offset)
                                break;
                            continue;
                        }
                        cnt++;
                        break;
                }

                if (cnt > offset)
                    break;

                navigator = navigator.GetPositionAtOffset(1, LogicalDirection.Forward);

            } // End while.

            return navigator;
        }

        private void timerSetRtbEntry_Tick(object sender, EventArgs e)
        {
            timerSetRtbEntry.Stop();
            timerSetRtbEntry.Enabled = false;
            if (currentPathItem == null) return;

            msJournal.Enabled = false;
            this.Enabled = false;

            // configure rtb document
            configureRtbDocument(rtbEntry, currentPathItem.node.chapter.documentWidth, true);

            // create a new flowdocument and set it
            Object? data = null;
            data = entryMethods.DBLoadNodeData(dbCtx, currentPathItem.Id, currentPathItem.node.DirectorySectionID);

            // configure richtextbox
            try
            {
                if (data is String)
                    rtbEntry.Rtf = (String)data;
                else
                    rtbEntry.XamlBytes = (byte[])data;
            }
            catch (Exception)
            {
                MessageBox.Show("error loading this entry's rtf. rtf corrupted and or contains invalid data.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            resetRtb(rtbEntry, false, true);

            List<RegisterItem>? ancestors = null;
            Register.Lineage(dbCtx, currentPathItem, ref ancestors, true);//, true);
            String path = Register.LineageFullPath(ancestors);//, currentItem);//, true);

            String label = entryMethods.getEntryLabel(currentPathItem.node, false);
            String formatted = $"{path}::{label} ({currentPathItem.childrenCount} children)";
            txtEntryTitle.Text = formatted;//currentPathItem.node.chapter.Title;

            tsslabelLMD.Text = currentPathItem.node.chapter.modificationDateTime.ToString("HH:mm:ss:fff, dddd, dd MMMM yyyy");
            tsslabelID.Text = currentPathItem.node.chapter.Id.ToString();
            tsslabelPID.Text = currentPathItem.node.chapter.parentId.ToString();

            this.Enabled = true;
            msJournal.Enabled = true;

            this.ActiveControl = host1;
            host1.Focus();
            rtbEntry.Focus();

            try
            {
                // now setup caret config
                rtbEntry.SelectionStartOffset = currentPathItem.node.chapter.caretIndex;
                if (currentPathItem.node.chapter.caretSelectionLength != 0)
                    rtbEntry.SelectionLength = currentPathItem.node.chapter.caretSelectionLength;

                // show caret position in the middle of richtextbox
                if (rtbEntry.Selection != null)
                {
                    if (rtbEntry.Selection.Start != null)
                    {
                        if (rtbEntry.Selection.Start.Paragraph != null)
                        {
                            if (rtbEntry.Selection.Start.Paragraph.NextBlock != null)
                            {
                                Block selNextBlock = rtbEntry.Selection.Start.Paragraph.NextBlock;
                                if (selNextBlock != null)
                                {
                                    var characterRect = selNextBlock.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                                    rtbEntry.ScrollToVerticalOffset(rtbEntry.VerticalOffset + characterRect.Top - rtbEntry.ActualHeight / 2d);
                                }
                            }
                        }
                    }
                }
            }
            catch { }

        }
        public void configureRtbDocument(System.Windows.Controls.WpfRichTextBoxEx rtb, int width, bool clear)
        {
            // create a new flowdocument and set it with new width
            List<Block> blocks = rtbEntry.Document.Blocks.ToList();
            rtbEntry.Document.Blocks.Clear();
            rtbEntry.SpellCheck.IsEnabled = false;
            FlowDocument flowDoc = new FlowDocument();// new Paragraph(new Run("")));
            flowDoc.PageWidth = (double)width;
            flowDoc.ColumnWidth = 999999.0;
            flowDoc.FontFamily = new System.Windows.Media.FontFamily("Times New Roman");
            flowDoc.FontSize = 24.0; /// original 14.0;
            rtbEntry.Document = flowDoc;
            if (!clear)
                rtbEntry.Document.Blocks.AddRange(blocks);
        }
        private void insertColumnLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatInsertTableColumn(rtbEntry, true);
        }

        private void insertColumnRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatInsertTableColumn(rtbEntry, false);
        }

        private void insertRowAboveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatInsertTableRow(rtbEntry, true);
        }

        private void insertRowBelowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatInsertTableRow(rtbEntry, false);
        }

        private void toolStripMenuItem59_Click(object sender, EventArgs e)
        {
            formatting.formatDeleteTableColumn(rtbEntry);
        }

        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatDeleteTableRow(rtbEntry);

        }

        private void toolStripMenuItem64_Click(object sender, EventArgs e)
        {
            formatting.formatDeleteTable(rtbEntry);
        }

        private void columnWidthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatSetColumnWidth(rtbEntry, false);
        }

        private void toolStripSeparator20_Click(object sender, EventArgs e)
        {

        }

        private void formatRowsCellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatCells(rtbEntry);
        }

        private void removeFormattingOfCellsOfRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatCellsRemoveAllFormatting(rtbEntry);
        }

        private void formatColumnsAndTheirContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatColumns(rtbEntry, false);
        }

        private void removeFormattingOfEntireTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatTable(rtbEntry, true);
        }

        private void toolStripMenuItem58_Click(object sender, EventArgs e)
        {
            formatting.formatTable(rtbEntry, false);
        }

        private void toolStripMenuItem65_Click(object sender, EventArgs e)
        {
            formatting.formatSetColumnWidth(rtbEntry, true);
        }

        private void selectTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.selectTable(rtbEntry);
        }

        private void toolStripMenuItem66_Click(object sender, EventArgs e)
        {
            formatting.formatRows(rtbEntry, false);
        }

        private void toolStripMenuItem67_Click(object sender, EventArgs e)
        {
            formatting.formatRows(rtbEntry, true);
        }

        private void toolStripMenuItem68_Click(object sender, EventArgs e)
        {
            formatting.formatColumns(rtbEntry, true);
        }

        private void toolStripMenuItem69_Click(object sender, EventArgs e)
        {
            doImportNonCalendarRtfEntries(true);
        }

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {
            rtbEntry.SelectAll();
        }

        private void paragraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatInsertParagraph(rtbEntry);
        }

        private void recoverSavedEntryFromDbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            if (currentPathItem == null) return;

            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, true, true);

            // configure rtb document
            configureRtbDocument(rtbEntry, currentPathItem.node.chapter.documentWidth, true);

            // create a new flowdocument and set it
            Object? data = null;
            data = entryMethods.DBLoadNodeData(dbCtx, currentPathItem.Id, currentPathItem.node.DirectorySectionID);

            // configure richtextbox
            try
            {
                if (data is String)
                    rtbEntry.Rtf = (String)data;
                else
                    rtbEntry.XamlBytes = (byte[])data;
            }
            catch (Exception)
            {
                MessageBox.Show("error loading this entry's rtf. rtf corrupted and or contains invalid data.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            resetRtb(rtbEntry, false, true);

            List<RegisterItem>? ancestors = null;
            Register.Lineage(dbCtx, currentPathItem, ref ancestors, true);//, true);
            String path = Register.LineageFullPath(ancestors);//, currentItem);//, true);

            String label = entryMethods.getEntryLabel(currentPathItem.node, false);
            String formatted = $"{path}::{label} ({currentPathItem.childrenCount} children)";
            txtEntryTitle.Text = formatted;//currentPathItem.node.chapter.Title;

            tsslabelLMD.Text = currentPathItem.node.chapter.modificationDateTime.ToString("HH:mm:ss:fff, dddd, dd MMMM yyyy");
            tsslabelID.Text = currentPathItem.node.chapter.Id.ToString();
            tsslabelPID.Text = currentPathItem.node.chapter.parentId.ToString();

            try
            {
                // now setup caret config
                rtbEntry.SelectionStartOffset = currentPathItem.node.chapter.caretIndex;
                if (currentPathItem.node.chapter.caretSelectionLength != 0)
                    rtbEntry.SelectionLength = currentPathItem.node.chapter.caretSelectionLength;

                // show caret position in the middle of richtextbox
                if (rtbEntry.Selection != null)
                {
                    if (rtbEntry.Selection.Start != null)
                    {
                        if (rtbEntry.Selection.Start.Paragraph != null)
                        {
                            if (rtbEntry.Selection.Start.Paragraph.NextBlock != null)
                            {
                                Block selNextBlock = rtbEntry.Selection.Start.Paragraph.NextBlock;
                                if (selNextBlock != null)
                                {
                                    var characterRect = selNextBlock.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                                    rtbEntry.ScrollToVerticalOffset(rtbEntry.VerticalOffset + characterRect.Top - rtbEntry.ActualHeight / 2d);
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }
        private void configureEntrysWidthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            if (dbCtx.readOnly)
            {
                MessageBox.Show("error. database read-only.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // first save the current entry
            saveEntry();

            if (currentPathItem == null) return;

            // skip if this is system node, we cannot change it
            if (currentPathItem.node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return;

            configureEntryWidth(rtbEntry, currentPathItem.Id);


        }

        public void configureEntryWidth(System.Windows.Controls.WpfRichTextBoxEx rtb, UInt32 id)
        {
            // prepare input box
            List<Object> valueStrings = new List<Object>();
            int width = myConfig.defaultDocumentWidth;
            for (int i = 0; i < 60; i++)
            {
                valueStrings.Add((Object)width.ToString());
                width += 100;
            }

            // get latest state register item
            RegisterItem? item = Register.LoadSetupRegisterItem(dbCtx, id, true, false, false, false, false, false);
            if (item == null) return;

            // ask user
            String input = item.node.chapter.documentWidth.ToString();
            if (userInterface.ShowListInputDialog("configure entry document width", ref input, ref valueStrings, -1) != DialogResult.OK)
                return;

            // check validity
            int value = -1;
            if (!int.TryParse(input, out value))
                return;

            // firstly set document width in db
            if (!entryMethods.DBSetNodeDocumentWidthOFSDB(dbCtx, item.node, value))
                return;

            // finally setup document width
            // configure rtb document
            configureRtbDocument(rtbEntry, item.node.chapter.documentWidth, false);

            //setup
            currentPathItem = item;
            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, true, true);

            // first get old original tree view nodes 
            TreeNode? tvOldItem = null;
            TreeNode? tvOldParent = null;
            List<TreeNode?> tvLineage = null;
            List<RegisterItem?> lineage = null;

            // first get old tree nodes
            getTreeNodesLineage(currentPathItem, true, ref tvOldParent, ref tvOldItem, ref lineage, ref tvLineage);

            // reload
            tvTree.SelectedNode = tvOldItem;
        }

        private void buttonSearchLocation_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            FormTreeDesign2 form = new FormTreeDesign2();
            form.ctx = dbCtx;
            if (form.ShowDialog() != DialogResult.OK) return;
            if (form.selectedPathItem == null) return;

            // now check if selected node's ancestor does not already exists in the list. if exists, abort
            /* original bug - we cannot add 2 or more different locations in the same tree
            foreach (ListViewItem lvitem in lvSearchWhere.Items)
            {
                RegisterItem? item = (RegisterItem?)lvitem.Tag;
                if (Register.ExistsSomewhereInAncestoralChain(dbCtx, VhdCtx.dbCtx.dbNodeTreeRegistryFile, item, form.selectedPathItem))
                    return; // item's ancestral chain already exists in the list so we take that and do not insert a duplicate
            }
            */
            // this node's ancestral chain and this node does not exists in the list. insert this selected node into list
            LVSearchWhereAddItem(form.selectedPathItem);

        }
        public void LVSearchWhereAddItem(RegisterItem? registryItem)
        {
            if (registryItem == null) return;
            List<RegisterItem>? ancestors = null;
            Register.Lineage(dbCtx, registryItem, ref ancestors, true);
            String path = Register.LineageFullPath(ancestors);//, currentItem);//, true);
            ListViewItem item = new ListViewItem(path);
            item.Tag = registryItem.Id;

            String rtf = "";
            byte[]? xamlbytesOut = null;
            registryItem.loadNode(dbCtx, ref rtf, ref xamlbytesOut, false);
            String label = entryMethods.getEntryLabel(registryItem.node, false);
            String formatted = $"{label} ({registryItem.childrenCount} children)";
            item.SubItems.Add(formatted);

            lvSearchWhere.Items.Add(item);
        }

        private void buttonSearchLocationReset_Click(object sender, EventArgs e)
        {
            lvSearchWhere.Tag = null;
            lvSearchWhere.Items.Clear();
        }

        private void toolStripMenuItem71_Click(object sender, EventArgs e)
        {
            formatting.formatSelectTableRow(rtbEntry);
        }

        private void toolStripMenuItem72_Click(object sender, EventArgs e)
        {
            formatting.formatStrikeout(rtbEntry, (ToolStripButton)tsbuttonStrikeout);
        }

        private void buttonRemoveSearchLocation_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvSearchWhere.CheckedItems)
                item.Remove();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Version version = Assembly.GetExecutingAssembly().GetName().Version;
            String text = $"Tushar Jain's TheBook.Net-Version{Application.ProductVersion}::DiaryJournal.Net-Version{version}" +
                "\nan open source free writing sotware a must for books and stories writers, notes keeping, books texts and stories collection and scribing, diary and journal software, .Net 10.0 and Visual Studio 2026.\n" +
                "Copyright © 2022 - 2027, Tushar Jain\noriginal sole developer: Tushar Jain";
            MessageBox.Show(text, "about", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void chkSearchReplace_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSearchReplace.Checked)
            {
                if (MessageBox.Show("warning: please confirm if you want to replace all matching items?", "warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    chkSearchReplace.Checked = false;
            }
        }

        private void toolStripMenuItem73_Click(object sender, EventArgs e)
        {
            formatting.formatPasteRawText(rtbEntry);
        }

        private void convertEntryToRawTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatConvertToRawText(rtbEntry);
        }

        private void toolStripMenuItem74_Click(object sender, EventArgs e)
        {
            formatting.cutParagraphRaw(rtbEntry);
        }

        private void cloneEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dbCtx.readOnly)
            {
                MessageBox.Show("error. database read-only.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // firstly save entry
            __saveEntry();

            if (currentPathItem == null) return;
            if (currentPathItem.Id == 0) return;

            // get current register item latest state
            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, true, true);

            CloneNode(currentPathItem.parentId);
        }

        public bool cloneEntry(UInt32 id, UInt32 locationId, out RegisterItem? clone)
        {
            if (!dbCtx.isDBOpen())
            {
                clone = null;
                return false; // error
            }

            // validations
            RegisterItem? parent = null;
            Register.FindNode(dbCtx, locationId, ref parent);
            if (parent.childrenCount >= Register.default_maxChildrenNodes)
            {
                MessageBox.Show($"error entry not created. maximum direct new children create/insert limit [{Register.default_maxChildrenNodes}] reached for/in this target parent.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                clone = null;
                return false; // error
            }

            RegisterItem? cloneItem = entryMethods.DBCloneNodeOFSDB(dbCtx, id, locationId, ref emptySlotsItem);
            if (cloneItem == null)
            {
                clone = null;
                return false; // error
            }

            // update db config file
            dbCtx.dbConfig.latestCreatedEntry = cloneItem.Id;
            DatabaseConfig.toYamlFile(dbCtx.dbConfig, dbCtx.dbConfigFile);

            clone = cloneItem;
            return true;
        }

        private void cloneToOtherLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dbCtx.readOnly)
            {
                MessageBox.Show("error. database read-only.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // firstly save entry
            __saveEntry();

            if (currentPathItem == null) return;
            if (currentPathItem.Id == 0) return;

            // get current register item latest state
            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, true, true);

            // skip if this is system node, we cannot change it
            if (currentPathItem.node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return;

            FormTreeDesign2 form = new FormTreeDesign2();
            form.ctx = dbCtx;
            if (form.ShowDialog() != DialogResult.OK) return;
            if (form.selectedPathItem == null) return;

            CloneNode(form.selectedPathItem.Id);
        }

        private void toolStripMenuItem75_Click(object sender, EventArgs e)
        {
            if (dbCtx.readOnly)
            {
                MessageBox.Show("error. database read-only.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // firstly save entry
            __saveEntry();

            if (currentPathItem == null) return;
            if (currentPathItem.Id == 0) return;

            // get current register item latest state
            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, true, true);

            CloneNode(0);
        }

        private void buttonResetConfig1_Click(object sender, EventArgs e)
        {
            cmbCfgRtbViewEntryRM.SelectedIndex = cmbCfgRtbViewEntryRM.FindString(myConfig.default_cmbCfgRtbViewEntryRMValue.ToString());
        }
        private void gotoLatestEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            __gotoEntryByAttribute(false, true, false, 0);

        }

        private void gotoLatestLastModifiedEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            __gotoEntryByAttribute(true, false, false, 0);
        }


        private void gotoEntryByIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string? input = "0";
            if (userInterface.ShowInputDialog("input entry id", ref input) != DialogResult.OK)
                return;

            // check validity
            UInt32 value = 0;
            if (!UInt32.TryParse(input, out value))
                return;

            if (value < 0) return;

            // goto custom entry
            __gotoEntryByAttribute(false, false, true, value);

        }
        public void restoreState(int index)
        {
            switch (index)
            {
                case 0:
                    rtbEntry.Xaml = xamlState0;
                    break;
                case 1:
                    rtbEntry.Xaml = xamlState1;
                    break;
                case 2:
                    rtbEntry.Xaml = xamlState2;
                    break;
                case 3:
                    rtbEntry.Xaml = xamlState3;
                    break;
                case 4:
                    rtbEntry.Xaml = xamlState4;
                    break;
                case 5:
                    rtbEntry.Xaml = xamlState5;
                    break;
                case 6:
                    rtbEntry.Xaml = xamlState6;
                    break;
                case 7:
                    rtbEntry.Xaml = xamlState7;
                    break;
                case 8:
                    rtbEntry.Xaml = xamlState8;
                    break;
                case 9:
                    rtbEntry.Xaml = xamlState9;
                    break;

            }
        }

        private void state0ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(0);
        }

        private void state1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(1);

        }

        private void state2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(2);

        }

        private void state3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(3);

        }

        private void state4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(4);

        }

        private void state5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(5);

        }

        private void state6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(6);

        }

        private void state7ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(7);

        }

        private void state8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(8);

        }

        private void state9latestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(9);

        }

        private void copyDbToLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            if (browseFolder.ShowDialog() != DialogResult.OK) return;

            // firstly save entry
            saveEntry();

            __toggleForm(false);

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // now copy/move db
            if (entryMethods.DBCopyDatabaseOFSDB(dbCtx, browseFolder.SelectedPath, false, false, false, formOperation))
            {
                formOperation.close();
                MessageBox.Show("successfully copied/moved db! please manually reload the db.", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                formOperation.close();
                MessageBox.Show("error occured!", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            __toggleForm(true);
        }
        private void toolStripMenuItem19_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            if (browseFolder.ShowDialog() != DialogResult.OK) return;

            // firstly save entry
            saveEntry();

            __toggleForm(false);

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // now copy/move db
            if (entryMethods.DBCopyDatabaseOFSDB(dbCtx, browseFolder.SelectedPath, false, true, false, formOperation))
            {
                formOperation.close();
                MessageBox.Show("successfully copied/moved db! please manually reload the db.", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                formOperation.close();
                MessageBox.Show("error occured!", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            __toggleForm(true);

        }

        private void insertACodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // first save the entry
            saveEntry();

            if (!dbCtx.isDBOpen())
                return;

            // let the user choose and insert a single template code into the template entry
            FormList form = new FormList();
            form.checkMultipleItems = false;
            form.listType = FormList.ListType.TemplateCode;
            form.allItems = templateFormats.findAllTemplateCodeItems();
            if (form.ShowDialog(this) != DialogResult.OK) return;
            if (form.outSelectedItem == null) return;
            TemplateFormat.TemplateCodeItem item = (TemplateFormat.TemplateCodeItem)form.outSelectedItem;
            formatting.formatInsertString(rtbEntry, item.value);
        }

        private void toolStripMenuItem81_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            if (dbCtx.readOnly)
            {
                MessageBox.Show("error. database read-only.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // first save the entry
            saveEntry();

            string? input = "";
            if (userInterface.ShowInputDialog("input title for new template entry (mandatory)", ref input) != DialogResult.OK)
                return;

            // template must have a name/title
            if (input.Length <= 0) return;

            RegisterItem? item = setupNewSpecialEntry(DateTime.Now, NodeType.Template, NodeType.Templates, input);

        }

        private void toolStripMenuItem82_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            // first save the entry
            saveEntry();

            // reload latest state
            RegisterItem? parent = entryMethodsNewDesign.findRegistrySystemNodeItemByType(this.RootSystemNodesRegistry, NodeType.Templates);
            parent = Register.LoadSetupRegisterItem(dbCtx, parent.Id, true, false, false, false, true, true);
            if (parent == null) return;

            String? rtf = "";
            byte[]? xamlbytes = null;

            List<RegisterItem>? desc = null;
            parent.tree.GetDescendantTreeSequence(ref desc, NodeType.Template);
            foreach (RegisterItem obj in desc)
                obj.loadNode(dbCtx, ref rtf, ref xamlbytes, false);

            // show all templates list form and let the user choose a single template
            FormNodeList form = new FormNodeList();
            form.registry = desc;//parent.childrenList;
            form.nodeTypes.Add(NodeType.Template);
            form.checkMultipleNodes = false;
            form.listDeletedNodes = false;
            if (form.ShowDialog(this) != DialogResult.OK) return;
            if (form.outSelectedNode == null) return;

            // load rtf data of the selected template from it's db node
            RegisterItem? item = form.outSelectedNode;
            item.loadNode(dbCtx, ref rtf, ref xamlbytes, true);

            // initialize an rtb control and set the rtf into it for processing
            if (dbCtx.dbEntryType == EntryType.Xaml)
                xamlEntry.dummy.XamlBytes = xamlbytes;
            else
                xamlEntry.dummy.Rtf = rtf;

            // now fetch all template codes
            List<Object> codes = templateFormats.findAllTemplateCodeItems();

            // now transform all codes
            foreach (Object? codeItem in codes)
            {
                if (codeItem == null) continue;
                TemplateFormat.TemplateCodeItem code = (TemplateFormat.TemplateCodeItem)codeItem;
                templateFormats.transform(xamlEntry.dummy, code);
            }

            // finally insert the transformed rtf into our primary rtb control at the selection location
            rtf = xamlEntry.dummy.Rtf; // WpfRtbMethods.ToRtf(rtb);
            rtbEntry.SelectedRtf = rtf;
            MessageBox.Show("template inserted", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void convertToRawTextAllNodesRecursiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            if (MessageBox.Show("warning: are you sure you want to convert body of all nodes to raw text? this will remove all richtext formatting and media!", "warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            // firstly save entry
            __saveEntry();

            this.Invoke(toggleForm, false);

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // now process
            UInt32 processed = 0;
            entryMethods.DBNodesConvertToRawTextOFSDB(dbCtx, out processed, formOperation);

            this.Invoke(toggleForm, true);
            formOperation.close();
        }


        private async void pasteHtmlFromWebBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            // save entry just in case application crashes from a bug
            saveEntry();

            /*
             * warning: experimental, contains crash bugs in backend source code library.
             *
             */
            //String xaml = System.Windows.Forms.Clipboard.GetText(System.Windows.Forms.TextDataFormat.Html);

            //object obj = Clipboard.GetDataObject();
            //rtbEntry.Paste();
            //String xaml = System.Windows.Forms.Clipboard.GetText(System.Windows.Forms.TextDataFormat.Text);
            String html = System.Windows.Forms.Clipboard.GetText(System.Windows.Forms.TextDataFormat.Html);
            if (html == "") return;
            if (html.IndexOf("<html>") == -1) return;

            //            html = String.Format(@"<!DOCTYPE html><html><body>{0}", html.Split("<!--StartFragment-->")[1]);
            //html = String.Format(@"<html><body>{0}", html.Split("<!--StartFragment-->")[1]);
            //html = String.Format("{0}</body></html> ", html.Split("<!--EndFragment-->")[0]);
            //html = String.Format("<html>{0}", html.Split("<html>")[1]);
            //html = String.Format("{0}</html>", html.Split("</html>")[0]);

            bool result = myCommonMethods1.CheckForInternetConnection(5000);
            if (!result)
                MessageBox.Show("error ping failed.", "error");


            //var config = Configuration.Default;
            //var context = BrowsingContext.New(config);
            //var document = await context.OpenAsync(req => req.Content(html));
            //var elements = document.Body.QuerySelectorAll("img");
            //html = document.ToHtml();



            //original HtmlAgilityPack
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(html);
            HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes("//img");
            List<Image> images = new List<Image>();

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    /*
                    var src = "";
                    if (node.Attributes.Contains(@"src"))
                        src = node.Attributes[@"src"].Value;
                    else if (node.Attributes.Contains(@"data-src"))
                        src = node.Attributes[@"data-src"].Value;

                    //if (src.StartsWith("/"))
                    //    node.SetAttributeValue("src", "www.abc.xyz" + src);


                    // download image
                    Bitmap? bitmap = myCommonMethods1.DownloadImage(src, "", null);
                    if (bitmap != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            // set jpeg image base64 encoded blob into src
                            bitmap.Save(ms, bitmap.RawFormat);
                            images.Add(bitmap);
                            //String data = String.Format("data:image/jpeg;base64,{0}", Convert.ToBase64String(ms.ToArray()));
                            //node.SetAttributeValue("src", data);
                        }
                    }
                    //var newHtml = htmlDoc.DocumentNode.WriteTo();
                    */
                }
            }
            html = htmlDoc.DocumentNode.WriteTo();


            HtmlToXamlDocumentOptions options = new HtmlToXamlDocumentOptions();
            options.IsRootSection = false;
            String xaml = HtmlToXamlConverter.ConvertHtmlToXaml(html, new HtmlToXamlContext(options));

            try
            {
                //byte[] bytes = Encoding.UTF8.GetBytes(xaml);
                //FlowDocument resultDoc = (FlowDocument)XamlReader.Parse(xaml);
                //FlowDocument resultDoc = System.Windows.Controls.WpfRichTextBoxEx.SetXaml(xaml);
                String encXaml = System.Windows.Controls.WpfRichTextBoxEx.GetPreparedEncodedXaml(rtbEntry.dummy, xaml);
                //rtbEntry.SelectedRtf = System.Windows.Controls.WpfRichTextBoxEx.ToRtf(resultDoc);
                rtbEntry.SelectedXaml = encXaml;

            }
            catch { }

            //var xaml = HtmlToXamlConverter.ConvertHtmlToXaml(html, new HtmlToXamlContext(new HtmlToXamlDocumentOptions()));
            //rtbEntry.Paste();
            //rtbEntry.XamlPlain = xaml;
            //HtmlToXamlContext context = new HtmlToXamlContext(new HtmlToXamlDocumentOptions());
            //cxt.Options.IsRootSection = true;
            //XDocument xdoc = HtmlToXamlConverter.ConvertHtmlToXamlDocument(html, cxt);

            //html = String.Format("{0}", html.Split("<!--StartFragment-->")[1]);
            //html = String.Format("{0}", html.Split("<!--EndFragment-->")[0]);

            // From String
            //var doc = new HtmlAgilityPack.HtmlDocument();
            //doc.LoadHtml(html);
            //var htmlBody = doc.DocumentNode.SelectSingleNode("//body");
            //Console.WriteLine(htmlBody.OuterHtml);            //FileStream file = new FileStream("C:\\tmp.html", FileMode.Create);
            //doc.Save(file, Encoding.UTF8);
            // file.Flush();
            //file.Close();
            // file.Position = 0;

            //String html2 = Encoding.UTF8.GetString(ms.ToArray());
            //WebBrowser browser = new WebBrowser();
            //browser.Dock = DockStyle.Fill;
            //browser.Show();
            //browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);
            //MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(html));
            //browser.NavigateToStream(ms);
            //browser.DocumentText = "0";
            //browser.Navigate("about:blank");
            //browser.Document.OpenNew(true);
            //browser.Document.Write(html);
            ///browser.NavigateToString(html);
            //browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);
            //this.Controls.Clear();
            //this.Controls.Add(browser);
            //browser.DocumentStream = file;//ms;

            //Thread.Sleep(7000);
            //File.WriteAllText(@"C:\\tmp.html", browser.Document.Body.Parent.InnerHtml, Encoding.GetEncoding(browser.Document.Encoding));

            //browser.Refresh();
            //browser.sele
            //browser.Document.ExecCommand("SelectAll", false, null);
            //browser.Document.ExecCommand("Copy", false, null);
            //rtbEntry.Paste();

            /* original
            //String html2 = Encoding.UTF8.GetString(ms.ToArray());
            WebBrowser browser = new WebBrowser();
            //MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(html));
            //browser.NavigateToStream(ms);
            browser.DocumentText = "0";
            browser.Document.OpenNew(true);
            browser.Document.Write(html);
            this.Controls.Clear();
            this.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
            browser.Show();
            //browser.DocumentStream = file;//ms;
            browser.Refresh();
            browser.Document.ExecCommand("SelectAll", false, null);
            browser.Document.ExecCommand("Copy", false, null);
            rtbEntry.Paste();
            */


            //var xaml = HtmlToXamlConverter.ConvertHtmlToXaml(html, new HtmlToXamlContext(new HtmlToXamlDocumentOptions()));
            //HtmlToXamlContext context = new HtmlToXamlContext(new HtmlToXamlDocumentOptions());
            //cxt.Options.IsRootSection = true;
            //XDocument xdoc = HtmlToXamlConverter.ConvertHtmlToXamlDocument(html, cxt);


            //var xaml = HtmlToXamlConverter.ConvertHtmlToXaml(html, context);

            //var converter = new Converter();
            //var domRootElement = converter.Convert(html, asFlowDocument: true, cssStyleSheetProvider)

            //rtbEntry.XamlPlain = xaml;

            //System.Windows.Controls.WebBrowser webBrowser = new System.Windows.Controls.WebBrowser();
            //webBrowser.docu
            /*
            var webBrowser = new WebBrowser();
            webBrowser.CreateControl(); // only if needed
            webBrowser.docu = html;
            //while (webBrowser.DocumentText != *yourhtmlstring *)
             //   Application.DoEvents();
            webBrowser.Document.ExecCommand("SelectAll", false, null);
            webBrowser.Document.ExecCommand("Copy", false, null);
            rtbEntry.Paste();
            //*yourRichTextControl *.Paste();
            */
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbEntry.Document.Blocks.Clear();
        }

        private void toolStripMenuItem54_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "rtf files|*.rtf";
            if (ofd.ShowDialog() != DialogResult.OK) return;

            rtbViewEntry.LoadFile(ofd.FileName);
            tabControlJournal.SelectedTab = tabPageViewEntry;
        }

        private void ofdFile_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void toolStripMenuItem56_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            // save entry just in case application crashes from a bug
            saveEntry();

            OpenFileSystemDB.autoClearLocalPaths(dbCtx, false, true);
        }

        private void toolStripMenuItem85_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            // save entry just in case application crashes from a bug
            saveEntry();

            OpenFileSystemDB.autoClearLocalPaths(dbCtx, true, false);
        }
        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            if (myParentForm.Visible)
                myParentForm.Hide();
            else
                myParentForm.Show();
        }

        private void lvChildren_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tsbuttonReload_Click(object sender, EventArgs e)
        {
            reload();
        }

        private void textboxPath_Click(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.SelectAll();
            textBox.Focus();
        }

        private void lvCurrentPath_Click(object sender, EventArgs e)
        {
            //todo: if (lvCurrentPath.SelectedItems.Count == 0) return;
            //todo: ListViewItem selectedItem = lvCurrentPath.SelectedItems[0];
            //todo: UInt32 id = (UInt32)selectedItem.Tag;
            //todo: reloadChildren(id, lvChildren);
        }

        private void lvCurrentPath_KeyPressed(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                // Enter key pressed
                //todo: if (lvCurrentPath.SelectedItems.Count == 0) return;
                //todo: ListViewItem selectedItem = lvCurrentPath.SelectedItems[0];
                //todo: UInt32 id = (UInt32)selectedItem.Tag;
                //todo: reloadChildren(id, lvChildren);
            }
        }
        private void lvCurrentPath_DoubleClick(object sender, EventArgs e)
        {
            //todo: if (lvCurrentPath.SelectedItems.Count == 0) return;
            //todo: ListViewItem selectedItem = lvCurrentPath.SelectedItems[0];
            //todo: UInt32 id = (UInt32)selectedItem.Tag;
            //todo: reloadPath(id.ToString(), true, null);
        }

        private void lvChildren_Click(object sender, EventArgs e)
        {
            //todo: if (lvChildren.SelectedItems.Count == 0) return;
            //todo: ListViewItem selectedItem = lvChildren.SelectedItems[0];
            //todo: UInt32 id = (UInt32)selectedItem.Tag;
            //todo: reloadChildren(id, lvChildsChildren);

        }
        private void lvChildren_KeyPressed(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                // Enter key pressed
                //todo: if (lvChildren.SelectedItems.Count == 0) return;
                //todo: ListViewItem selectedItem = lvChildren.SelectedItems[0];
                //todo: UInt32 id = (UInt32)selectedItem.Tag;
                //todo: reloadChildren(id, lvChildsChildren);
            }
        }

        private void lvChildren_DoubleClick(object sender, EventArgs e)
        {
            //todo: if (lvChildren.SelectedItems.Count == 0) return;
            //todo: ListViewItem selectedItem = lvChildren.SelectedItems[0];
            //todo: UInt32 id = (UInt32)selectedItem.Tag;
            //todo: reloadPath(id.ToString(), true, null);
        }
        private void lvChildsChildren_DoubleClick(object sender, EventArgs e)
        {
            //todo: if (lvChildsChildren.SelectedItems.Count == 0) return;
            //todo: ListViewItem selectedItem = lvChildsChildren.SelectedItems[0];
            //todo: UInt32 id = (UInt32)selectedItem.Tag;
            //todo: reloadPath(id.ToString(), true, null);
        }
        private void lvSiblings_Click(object sender, EventArgs e)
        {
        }

        private void lvSiblings_DoubleClick(object sender, EventArgs e)
        {
            saveEntry();
            //todo: if (lvSiblings.SelectedItems.Count == 0) return;
            //todo: ListViewItem selectedItem = lvSiblings.SelectedItems[0];
            //todo: UInt32 id = (UInt32)selectedItem.Tag;
            //todo: reloadPath(id.ToString(), true, null);
        }

        private void lvSiblings_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lvCurrentPath_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FormJournalDesign1_SizeChanged(object sender, EventArgs e)
        {
            // top horizontal split container for toolbox
            splitContainerH.SplitterDistance = 30;

            // horizontal split container for top path and title panel and lower navigation panel
            splitContainer1.SplitterDistance = 30;

            // navigation vertical split container for parent to children navigation panels
            splitContainer3.SplitterDistance = splitContainer3.Size.Width / 3;

            // text box for entry title
            txtEntryTitle.Width = this.ClientSize.Width;
        }

        private void FormJournalDesign1_Resize(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem31_Click(object sender, EventArgs e)
        {
            CopyDB(true);
        }
        public void CopyDB(bool toXaml)
        {
            if (!dbCtx.isDBOpen())
                return;

            if (browseFolder.ShowDialog() != DialogResult.OK) return;

            // firstly save entry
            saveEntry();

            __toggleForm(false);

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // now copy/move db
            if (entryMethods.DBCopyDatabaseOFSDB(dbCtx, browseFolder.SelectedPath, true, false, toXaml, formOperation))
            {
                formOperation.close();
                MessageBox.Show("successfully copied/moved db! please manually reload the db.", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                formOperation.close();
                MessageBox.Show("error occured!", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            __toggleForm(true);

        }
        private void insertXamlFileIntoSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "xaml files|*.xaml";
            if (ofd.ShowDialog() != DialogResult.OK) return;

            byte[] bytes = File.ReadAllBytes(ofd.FileName);
            rtbEntry.SelectedXamlBytes = bytes;
        }

        public void convertDB(bool toXaml)
        {
            if (!dbCtx.isDBOpen())
                return;

            if (MessageBox.Show("warning: are you sure you want to convert current db?", "warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            // firstly save entry
            __saveEntry();

            __toggleForm(false);

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // now process
            UInt32 processed = 0;
            entryMethods.DBConvertOFSDB(dbCtx, toXaml, out processed, formOperation);

            formOperation.close();
            __toggleForm(true);

        }
        private void toolStripMenuItem50_Click(object sender, EventArgs e)
        {
            convertDB(true);
        }

        private void toolStripMenuItem36_Click(object sender, EventArgs e)
        {
            convertDB(false);
        }

        private void toolStripMenuItem51_Click(object sender, EventArgs e)
        {
            CopyDB(false);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            myCommonMethods1.OpenFolderInExplorer(dbCtx.dbBasePath);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (currentPathItem == null) return;
            entryMethods.DBExploreEntryFileOFSDB(dbCtx, currentPathItem.sectionId, currentPathItem.Id);
        }

        private void showSelectedTextInStickyNotesFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String notes = rtbEntry.Selection.Text;
            StickyNoteForm obj = new StickyNoteForm();
            obj.notes = notes;
            obj.Show();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (currentPathItem == null) return;

            List<Object> found = new List<Object>();
            FormList form = new FormList();
            form.checkMultipleItems = false;
            form.listType = FormList.ListType.UInt32;
            form.allItems = found;

            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, false, false, true, false, true, true);
            currentPathItem.tree.GetDescendantTreeSequence(ref currentPathItem.treeList);

            // add this top parent first
            found.Add(currentPathItem.Id);

            // iterate and add all tree sequence of this top parent
            foreach (RegisterItem item in currentPathItem.treeList)
                found.Add(item.Id);

            // let the user choose and insert a single template code into the template entry
            form.Text = $"total descendants of current node: {found.Count}";
            if (form.ShowDialog(this) != DialogResult.OK) return;

            UInt32 id = (UInt32)form.outSelectedItem;
            __gotoEntryById(id);

        }

        private void tsbuttonDeleteEntry_Click(object sender, EventArgs e)
        {
            deleteNodes();
        }
        private void purgeCheckedNodesTreesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteNodes();
        }

        public void deleteNodes()
        {
            if (currentPathItem == null) return;

            if (!dbCtx.isDBOpen())
                return;

            if (dbCtx.readOnly)
            {
                MessageBox.Show("error db write locked.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, true, true);

            if (currentPathItem == null) return;
            if (currentPathItem.Id == 0) return;
            if (currentPathItem.specialNodeType == SpecialNodeType.SystemNode) return;
            if (currentPathItem.nodeType == NodeType.EmptySlot) return;

            // firstly save entry
            __saveEntry();

            if (MessageBox.Show("warning: are you sure you want to purge checked nodes? all of their descendants trees will also be forever purged along with them!\n" +
                "there is no need to purge anything! you can recycle and reuse the nodes! you have 8+ million slots!", "warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            __toggleForm(false);
            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // first get old original tree view nodes 
            TreeNode? tvOldItem = null;
            TreeNode? tvOldParent = null;
            List<TreeNode?> tvLineage = null;
            List<RegisterItem?> lineage = null;

            // first get old tree nodes
            getTreeNodesLineage(currentPathItem, true, ref tvOldParent, ref tvOldItem, ref lineage, ref tvLineage);

            // delete
            this.emptySlotsItem = Register.LoadSetupRegisterItem(dbCtx, this.emptySlotsItem.Id, false, false, false, false, true, true);
            currentPathItem.tree.Delete(currentPathItem, emptySlotsItem);

            // now remove tree node from old place
            tvOldParent.Nodes.Remove(tvOldItem);
            // reconfigure labels
            reloadTreeNodeConfigLabel(tvOldParent);

            // finally reload path
            tvTree.SelectedNode = tvOldParent;

            formOperation.close();
            __toggleForm(true);

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen()) return;

            // firstly save entry
            __saveEntry();

            this.Invoke(toggleForm, false);

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            List<Object> found = new List<Object>();
            foreach (OpenFSDBSection section in dbCtx.dbSections.sections)
            {
                List<myNode> list = new List<myNode>();
                if (!OpenFileSystemDB.findSectionNodes(dbCtx, section.sectionId, ref list)) continue;
                // iterate and process all entries in this section
                foreach (myNode? listNode in list)
                {
                    myNode? node = listNode;
                    if (node == null) continue;
                    RegisterItem? parent = Register.LoadSetupRegisterItem(dbCtx, node.chapter.parentId, true, false, false, false, false, false);
                    if (parent == null)
                    {
                        found.Add(node.chapter.Id);
                        continue;
                    }
                    if (parent.nodeType == NodeType.EmptySlot)
                    {
                        found.Add(node.chapter.Id);
                        continue;
                    }
                }
            }

            this.Invoke(toggleForm, true);
            formOperation.close();

            // let the user choose orphan nodes to restore
            FormList form = new FormList();
            form.Text = "choose orphan nodes to restore them in any location";
            form.checkMultipleItems = true;
            form.listType = FormList.ListType.UInt32;
            form.allItems = found;
            if (form.ShowDialog(this) != DialogResult.OK) return;
            if (form.outCheckedItems.Count == 0) return;

            FormTreeDesign2 form2 = new FormTreeDesign2();
            form2.Text = "select destination location to restore the orphaned nodes";
            form2.ctx = dbCtx;
            if (form2.ShowDialog() != DialogResult.OK) return;
            if (form2.selectedPathItem == null) return;

            foreach (Object id in form.outCheckedItems)
            {
                String? rtf = "";
                byte[]? xaml = null;
                myNode? node = entryMethods.DBSearchNodeOFSDB(dbCtx, (UInt32)id, ref rtf, ref xaml, true);
                if (node == null) continue;

                node.chapter.parentId = form2.selectedPathItem.Id;

                // move node 
                entryMethods.DBSetNodeParent(dbCtx, node, form2.selectedPathItem.Id);

                RegisterItem? item = Register.LoadSetupRegisterItem(dbCtx, (UInt32)id, false, false, false, false, true, true);
                if (item == null) continue;

                item.sectionId = node.DirectorySectionID;
                item.domainType = node.chapter.domainType;
                item.specialNodeType = node.chapter.specialNodeType;
                item.parentId = form2.selectedPathItem.Id;
                item.nodeType = node.chapter.nodeType;
                item.node = node;
                Register.UpdateNode(dbCtx, item, form2.selectedPathItem.Id, true, node.DirectorySectionID, true, 0, true);

                item = Register.LoadSetupRegisterItem(dbCtx, (UInt32)id, false, false, false, false, true, true);

                RegisterItem? parent = Register.LoadSetupRegisterItem(dbCtx, form2.selectedPathItem.Id, false, false, false, false, true, true);
                if (parent == null) continue;

                parent.children.Add(item);

                item = Register.LoadSetupRegisterItem(dbCtx, (UInt32)id, false, false, false, false, true, true);
                parent = Register.LoadSetupRegisterItem(dbCtx, form2.selectedPathItem.Id, false, false, false, false, true, true);

                parent.tree.Add(item);
            }

            // get current register item latest state
            //RegisterItem? currentItem = Register.LoadSetupRegisterItem(dbCtx, currentPathItem.Id, true, false, false, false, false, false);
            //if (currentItem == null) return;

            //setup
            //currentPathItem = currentItem;

            // update form
            reloadPath("", false, currentPathItem);

        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            List<Object> found = new List<Object>();
            FormList form = new FormList();
            form.checkMultipleItems = false;
            form.listType = FormList.ListType.String;
            form.allItems = found;

            //Stream? s = Register.RegisterCopyToMemory(dbCtx.dbNodeTreeRegistryFile);
            UInt32 id = 0;
            while (id < Register.default_totalPreallocatedNodes)
            {
                RegisterItem? item = null;
                if (Register.FindNode(dbCtx.regFileStream, id++, ref item) < 0) break;
                if (item == null) break;

                if (item.domainType == DomainType.EmptySlot)
                    continue;

                if (item.nextId == 0 && item.prevId == 0)
                    found.Add($"item:{item.Id}=>next:{item.nextId} and prev:{item.prevId}");
                else if (item.nextId == 0)
                    found.Add($"item:{item.Id}=>next:{item.nextId} and prev:{item.prevId}");
                else if (item.prevId == 0)
                    found.Add($"item:{item.Id}=>next:{item.nextId} and prev:{item.prevId}");
            }

            //s.Close();
            //s.Dispose();

            // let the user choose and insert a single template code into the template entry
            form.Text = $"total nodes where tree sequence was broken or where dead end exists: {found.Count}";
            if (form.ShowDialog(this) != DialogResult.OK) return;

            id = (UInt32)form.outSelectedItem;
            __gotoEntryById(id);

        }

        private void toolStripMenuItem55_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen()) return;

            if (dbCtx.readOnly)
            {
                MessageBox.Show("error db write locked.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (currentPathItem == null) return;

            // firstly save entry
            __saveEntry();

            if (currentPathItem.nodeType == NodeType.EmptySlot) return;

            browseFolder.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            __toggleForm(false);
            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // import
            entryMethods.DBImportDocumentsOFSDB(dbCtx, currentPathItem.Id, emptySlotsItem, browseFolder.SelectedPath, formOperation);

            formOperation.close();
            __toggleForm(true);

            // reset reload to root
            tvTree.SelectedNode = tvTree.Nodes.Find("0", false).FirstOrDefault();
        }

        private void testTrashDbOnlyCreateTestNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "warning: create 100,000+ trash test nodes. this is only meant for trash test database not your official database. do you wish to continue?", "question",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;


            if (!dbCtx.isDBOpen()) return;

            if (dbCtx.readOnly)
            {
                MessageBox.Show("error db write locked.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // firstly save entry
            __saveEntry();

            List<RegisterItem> tops = new List<RegisterItem>();

            __toggleForm(false);

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);


            // first create top ancestors
            for (int i = 0; i < 10000; i++)
            {
                // now create registry and node
                myNode node = new myNode();
                node.chapter.parentId = 0;
                node.chapter.chapterDateTime = DateTime.Now;
                node.chapter.nodeType = NodeType.Entry;
                node.chapter.Title = i.ToString();
                byte[]? xamlbytes = null;
                RegisterItem? item = Register.Insert(dbCtx, 0, emptySlotsItem, node, "", xamlbytes, false, false);
                if (item != null)
                    tops.Add(item);

                formOperation.updateProgressBar(i, 10000);
                formOperation.updateFilesStatus(i, 10000);
            }
            UInt32 index = 0;

            // now create rest of trees: 300,000 or more garbage nodes
            for (int i = 0; i < 10; i++)
            {
                foreach (RegisterItem? top in tops)
                {
                    // now create registry and node
                    myNode node = new myNode();
                    node.chapter.parentId = top.Id;
                    node.chapter.chapterDateTime = DateTime.Now;
                    node.chapter.nodeType = NodeType.Entry;
                    node.chapter.Title = index++.ToString();
                    byte[]? xamlbytes = null;
                    RegisterItem? item = Register.Insert(dbCtx, top.Id, emptySlotsItem, node, "", xamlbytes, false, false);

                    formOperation.updateProgressBar(index, (10000 * 10));
                    formOperation.updateFilesStatus(index, (10000 * 10));

                }
            }

            // finally commit all config
            dbCtx.writeUsedSlotsFile();
            DatabaseConfig.toYamlFile(dbCtx.dbConfig, dbCtx.dbConfigFile);

            formOperation.close();

            __toggleForm(true);

        }

        private void textboxPath_TextChanged(object sender, EventArgs e)
        {

        }

        private void tvTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvTree.SelectedNode == null) return;

            // first save current entry
            saveEntry();

            // set current path item this selected node
            RegisterItem? item = Register.LoadSetupRegisterItem(dbCtx, UInt32.Parse(tvTree.SelectedNode.Name), false, false, false, false, true, true);
            currentPathItem = item;

            // disable form and menu
            __toggleForm(false);
            
            reloadPath("", false, currentPathItem);

            // enable form and menu
            __toggleForm(true);

        }

    }
}
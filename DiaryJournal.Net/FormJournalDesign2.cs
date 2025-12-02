#define UNICODE

using AngleSharp.Dom;
using HtmlAgilityPack;
using MarkupConverter;
using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp.Drawing;
using RtfPipe;
using RtfPipe.Model;
using RtfPipe.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Xml.Linq;
using TheBook.Net.Core;
using static DiaryJournal.Net.FindReplaceFramework;
using static System.Windows.Forms.DataFormats;

namespace DiaryJournal.Net
{

    public partial class FormJournalDesign2 : Form
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

        public myConfig cfg = new myConfig();

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
        public delegate void __gotoEntryByAttributeDelegate(bool lm, bool lc, bool byID, Int64 id);
        public __gotoEntryByAttributeDelegate gotoEntryByAttribute;
        public delegate bool __processSearchDelegate();
        public __processSearchDelegate processSearch;


        public FormJournalDesign2()
        {
            InitializeComponent();
        }

        private void FormJournalDesign2_Load(object sender, EventArgs e)
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

            rtbEntry.TextChanged += rtbEntry_TextChanged;
            rtbEntry.SelectionChanged += RtbEntry_SelectionChanged;
            tabControlJournal.Selected += TabControlJournal_Selected;
            cmbFonts.SelectedIndexChanged += CmbFonts_SelectedIndexChanged;
            cmbSize.SelectedIndexChanged += CmbSize_SelectedIndexChanged;
            lvSearch.DoubleClick += LvSearch_DoubleClick;
            this.Shown += FormJournalDesign2_Shown;
            this.FormClosing += FormJournalDesign2_FormClosing;
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

            // now load config file and setup
            dbCtx.config = cfg;
            myConfigMethods.autoCreateLoadConfigFile(ref dbCtx.config, false);
            applyConfig();

            splitContainerH.Cursor = Cursors.Default;
            splitContainerHInner1.Cursor = Cursors.Default;
            splitContainerSearch.Cursor = Cursors.Default;

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
            dtpickerDDSearchFrom.Value = DateTime.Now;
            dtpickerDDSearchThrough.Value = DateTime.Now;
            dtpickerDDSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerDDSearchThroughTime.Value = DateTime.Parse("23:59:59");

            // configure
            //rtbEntry.HideSelection = rtbViewEntry.HideSelection = false;

            // configure print document and printer stuff
            printerRtb.printDoc = pdRtbEntry;
            pdRtbEntry.BeginPrint += new PrintEventHandler(printerRtb.printDoc_BeginPrint);
            pdRtbEntry.PrintPage += new PrintPageEventHandler(printerRtb.printDoc_PrintPage);
            pdRtbEntry.EndPrint += new PrintEventHandler(printerRtb.printDoc_EndPrint);

            lvCurrentPath.KeyPress += new System.Windows.Forms.KeyPressEventHandler(lvCurrentPath_KeyPressed);
            lvChildren.KeyPress += new System.Windows.Forms.KeyPressEventHandler(lvChildren_KeyPressed);

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
        private void FormJournalDesign2_FormClosing(object? sender, FormClosingEventArgs e)
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

        private void FormJournalDesign2_Shown(object? sender, EventArgs e)
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
            chkCfgAutoLoadCreateDefaultDB.Checked = cfg.chkCfgAutoLoadCreateDefaultDB;
            rtbViewEntry.RightMargin = cfg.cmbCfgRtbViewEntryRMValue;
            int index = cmbCfgRtbViewEntryRM.FindString(rtbViewEntry.RightMargin.ToString());
            cmbCfgRtbViewEntryRM.SelectedIndex = index;
            radCfgLMNode.Checked = cfg.radCfgLMNode;
            radCfgLCNode.Checked = cfg.radCfgLCNode;
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
        public void OpenSearchedEntry(Int64 id)
        {
            RegisterItem? item = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, id, true, false, false, false, false, false);
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

        public long getSelectedListViewNodeId(System.Windows.Forms.ListView lv)
        {
            if (!dbCtx.isDBOpen())
                return -1;

            if (lv.SelectedItems.Count == 0)
                return -1;

            System.Windows.Forms.ListViewItem listViewItem = lv.SelectedItems[0];
            Int64 id = Int64.Parse(listViewItem.Name);

            return id;
        }

        private void LvSearch_DoubleClick(object? sender, EventArgs e)
        {
            Int64 id = getSelectedListViewNodeId(lvSearch);
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
            setupNewEntry(DateTime.Now, -1);
        }

        public RegisterItem? setupNewSpecialEntry(DateTime dateTime, NodeType nodeType = NodeType.Template,
            NodeType parentSystemNodeType = NodeType.Templates, String title = "")
        {
            // first save the entry
            saveEntry();

            // reload latest state
            RegisterItem? parent = entryMethodsNewDesign.findRegistrySystemNodeItemByType(this.RootSystemNodesRegistry, parentSystemNodeType);
            parent = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, parent.Id, false, false, false, false, true, true);
            if (parent == null) return null;

            // now create registry and node
            myNode node = new myNode();
            node.chapter.parentId = parent.Id;
            node.chapter.chapterDateTime = dateTime;
            node.chapter.nodeType = nodeType;
            node.chapter.Title = title;
            byte[]? xamlbytes = null;
            RegisterItem? item = Register.Insert(dbCtx, dbCtx.dbNodeTreeRegistryFile, parent.Id, emptySlotsItem, node, "", xamlbytes);
            if (item != null)
            {
                reloadPath("", false, item);
                MessageBox.Show("special node created", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("error, special node not created", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return item;
        }
        public void setupNewEntry(DateTime dateTime, Int64 parentId = 0, NodeType nodeType = NodeType.Entry,
            String title = "")
        {
            if (!dbCtx.isDBOpen())
                return;

            if (dbCtx.readOnly)
            {
                MessageBox.Show("error entry not created", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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

            if (parentId > 0)
            {
                // a valid parent id
            }
            else if (parentId == 0)
            {
                // root node demanded through 0, set parent to 0 meaning invalid
                parentId = 0;
            }
            else
            {
                // -1 auto parent means month parent id
                parentId = month.node.chapter.Id;
            }

            myNode node = new myNode();
            node.chapter.parentId = parentId;
            node.chapter.chapterDateTime = dateTime;
            node.chapter.creationDateTime = DateTime.Now;
            node.chapter.modificationDateTime = DateTime.Now;
            node.chapter.nodeType = nodeType;
            node.chapter.Title = title;

            RegisterItem? parent = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, parentId, true, false, false, false, false, false);
            if (parent == null) return; // parent not found or critical error

            // validations
            if (parent.childrenCount >= Register.default_maxChildrenNodes)
            {
                reloadPath("", false, parent);
                MessageBox.Show($"error entry not created. maximum direct new children create/insert limit [{Register.default_maxChildrenNodes}] reached for/in this target parent.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // now create registry and node
            byte[]? xamlbytes = null;
            RegisterItem? item = Register.Insert(dbCtx, dbCtx.dbNodeTreeRegistryFile, parent.Id, emptySlotsItem, node, "", xamlbytes);
            if (item != null)
            {
                reloadPath("", false, item);
                MessageBox.Show("entry node created", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("error entry not created", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void TheJournalImportrtfEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doImportRtfEntries(false);
        }
        public void doImportRtfEntries(bool alienEntries)
        {
            if (!dbCtx.isDBOpen())
                return;

            browseFolder.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Application.StartupPath;
            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            string? input = "the journal set";
            if (alienEntries)
                input = "other calendar import set";

            if (userInterface.ShowInputDialog("input new clone set name/title", ref input) != DialogResult.OK)
                return;

            if (input.Length <= 0)
                return;

            __importTheJournalRtfEntries(browseFolder.SelectedPath, input, true);
        }
        private void toolStripMenuItem70_Click(object sender, EventArgs e)
        {
            doImportRtfEntries(true);
        }
        public void __importTheJournalRtfEntries(String path, String importSetName, bool loadOperationForm)
        {
            /* todo tushar: upgrade to latest 02 January 2025
            // first save the entry
            this.Invoke(saveEntry);

            if (!dbCtx.isDBOpen())
                return;

            this.Invoke(toggleForm, false);

            // operations status form
            FormOperation? formOperation = null;
            if (loadOperationForm)
                formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            System.Windows.Forms.RichTextBox richTextBox = new System.Windows.Forms.RichTextBox();

            IEnumerable<FileInfo> files = myCommonMethods1.EnumerateFiles(path, EntryType.Rtf);
            long index = 0;
            long total = files.LongCount();

            // first we need to create a set node. we cannot import anything at all without a set node.
            myNode? setNode = entryMethods.createSetNode(dbCtx, importSetName, DateTime.Now);

            // import set node
            entryMethods.DBCreateNode(dbCtx, ref setNode, "", false, false, false, false, true, true);

            // this set's own context session work list
            List<myNode> setList = new List<myNode>();

            foreach (FileInfo file in files)
            {
                // everything inside the set and the set node is virtual and relative, not based on anything outside the set and set node.

                Chapter? chapter = theJournalMethods.convertFilenameToChapter(file.FullName);
                if (chapter == null)
                    continue;

                // get rtf and update
                // richtextbox automatically cleans and fixes a corrupted rtf and makes it valid.
                // so we first load the imported rtf into a richtextbox object, then retrieve the cleaned and fixed
                // rtf from it and only then store it in db.
                String rtf = File.ReadAllText(file.FullName);
                rtf = theJournalMethods.fixTheJournalRtfEntry(rtf);
                richTextBox.Rtf = rtf;
                rtf = richTextBox.Rtf;

                // by default all entries imported from "The Journal" are root entries aligned in Year and Month Nodes.
                // entry's properties
                chapter.nodeType = NodeType.Entry;

                // initialize calender nodes
                myNode? yearNode = null;
                myNode? monthNode = null;

                // initialize set's own virtual calendar nodes
                entryMethods.initCalenderNodesNonSystemSet(dbCtx, ref setList, ref setNode, chapter.chapterDateTime.Year,
                    chapter.chapterDateTime.Month, out yearNode, out monthNode, true);

                // now setup the chapter with the year and month config
                chapter.parentId = monthNode.chapter.Id;

                // create new node into db
                myNode? newNode = new myNode(ref chapter);
                newNode = entryMethods.DBNewNode(dbCtx,
                    SpecialNodeType.None, NodeType.Entry, DomainType.Journal,
                    ref newNode, true, true, true, chapter.chapterDateTime, chapter.parentId, true, chapter.Title, rtf,
                    true, true, false);

                if (newNode == null)
                    continue;

                // node op success, load the node into the global context session work list
                setList.Add(newNode);

                // update ui
                if (loadOperationForm)
                {
                    formOperation.updateProgressBar(index, total);
                    formOperation.updateFilesStatus(index, total);
                }

                // update
                index++;
            }

            // checkpoint
            entryMethods.DBCheckpoint(ref cfg);

            // finally update the db index in file.
            entryMethods.DBWriteIndexing(ref cfg);

            // now first add set node
            allNodes.Add(setNode);

            // now add all set list into the global session work list
            allNodes.AddRange(setList);

            this.Invoke(toggleForm, true);

            if (loadOperationForm)
                formOperation.close();

            //todo mySystemNodes? systemNodes = null;
            //reloadAll(true, true, true, true, ref systemNodes);
            */

        }

        public void __showMessageBox(String text, String title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            MessageBox.Show(this, text, title, buttons, icon);
        }

        public void __toggleForm(bool toggle)
        {
            this.Enabled = toggle;
            //this.Show();
            //this.BringToFront();
            //this.Focus();
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

            dbCtx.config = cfg;

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
            //List<myNode>? nodes = null;
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
            //this.RootSystemNodes = nodes;
            this.RootSystemNodesRegistry = registry;
            this.emptySlotsItem = emptySlotsItem;

            //List<RegisterItem> items = null;
            //emptySlotsItem.children.NextCache(1000, null, ref items);
            this.Enabled = true;
            formOperation.close();

            // now finally present the demanded nodes in user interface according to configuration
            reload();

            return true;
        }
        public void reload()
        {
            reloadPath(textboxPath.Text, true, null);
        }

        public void LVCurrentPathAddItem(RegisterItem? registryItem, int index, String name)
        {
            if (registryItem == null) return;
            ListViewItem item = new ListViewItem(index.ToString());
            //item.Name = registryItem.Id.ToString();
            item.Tag = registryItem.Id;//registryItem;
            item.SubItems.Add(name);
            lvCurrentPath.Items.Add(item);
        }
        public void LVChildrenAddItem(ListView lv, RegisterItem? registryItem, int index, String name)
        {
            if (registryItem == null) return;
            ListViewItem item = new ListViewItem(index.ToString());
            //item.Name = registryItem.Id.ToString();
            item.Tag = registryItem.Id;//registryItem;
            item.SubItems.Add(name);
            lv.Items.Add(item);
        }
        public void LVSiblingsAddItem(ListView lv, RegisterItem? registryItem, int index, String name)
        {
            if (registryItem == null) return;
            ListViewItem item = new ListViewItem(index.ToString());
            //item.Name = registryItem.Id.ToString();
            item.Tag = registryItem.Id;//registryItem;
            item.SubItems.Add(name);
            lv.Items.Add(item);
        }

        public bool reloadPath(String path, bool usePath, RegisterItem? item)
        {
            // phase 1 - firstly get the registry node item
            List<RegisterItem>? registry = null;

            if (usePath)
            {
                if (!Register.LoadFullPath(dbCtx, dbCtx.dbNodeTreeRegistryFile, path, ref registry, true))
                    return false; // critical error abort
            }
            else
            {
                if (!Register.LoadFullPath(dbCtx, dbCtx.dbNodeTreeRegistryFile, item, ref registry, true, false, false, false, false))
                    return false; // critical error abort
            }

            if (registry.Count == 0) return false;

            // operations status form
            //this.Enabled = false;
            //FormOperation? formOperation = null;
            //formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // phase 2 - first at the top place, add parent child relation list items

            int dirIndex = 0;
            // first add root path navigator
            RegisterItem? rootItem = registry.FirstOrDefault();
            // add current path as "." so that we can double click it to reload this current path with any changes.
            RegisterItem? currentItem = registry.LastOrDefault();

            lvCurrentPath.Items.Clear();

            // all mandatory validations
            if (currentItem.nodeType == NodeType.EmptySlot) return false;

            lvCurrentPath.BeginUpdate();

            LVCurrentPathAddItem(rootItem, dirIndex++, @"\");
            LVCurrentPathAddItem(currentItem, dirIndex++, ".");
            // now add parent if there is a parent with name ".." to climb to parent
            if (currentItem.Id != 0)
            {
                // this node is normal child node and has a parent other than root 0 id, so we must add ".." as the parent node link
                // if root 0 id then we cannot insert ".." parent node link.
                RegisterItem? parentItem = registry.Find(x => x.Id == currentItem.parentId);
                LVCurrentPathAddItem(parentItem, dirIndex++, "..");
            }

            // reload empty slots system node
            currentItem = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, currentItem.Id, true, false, true, false, true, true);

            // load registry item's node config from entry file
            String rtf = "";
            byte[]? xamlbytesOut = null;
            currentItem = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, currentItem.Id, true, false, false, false, true, true);
            RegisterItem? child = currentItem.children.Next();
            while (child != null)
            {
                if (child.domainType != DomainType.HiddenCore && child.nodeType != NodeType.EmptySlot)
                {
                    xamlbytesOut = null;
                    child.loadNode(dbCtx, ref rtf, ref xamlbytesOut, false);
                    String label = entryMethods.getEntryLabel(child.node, false);
                    String formatted = $"{label} ({child.childrenCount} children)";
                    LVCurrentPathAddItem(child, dirIndex++, formatted);
                }
                child = currentItem.children.Next();
            }
            lvCurrentPath.EndUpdate();

            // configure siblings listview
            reloadChildren(currentItem.parentId, lvSiblings);

            // finally configure everything
            String fsName = "";
            long freeSpace = 0;
            long partitionSize = 0;
            long usedSpace = 0;
            bool readOnly = dbCtx.readOnly;
            currentPathItem = currentItem;
            txtEntryTitle.Text = currentItem.node.chapter.Title;
            Int64 emptySlots = -1;
            Int64 registerFileSize = -1;
            labelEntries.Text = $"used slots:{Register.Count(dbCtx, dbCtx.dbNodeTreeRegistryFile, this.emptySlotsItem,
                ref emptySlots, ref registerFileSize).ToString()}";
            labelUnusedSlots.Text = $"unused slots:{emptySlots.ToString()}";
            List<RegisterItem>? ancestors = null;
            Register.Lineage(dbCtx, dbCtx.dbNodeTreeRegistryFile, currentItem, ref ancestors, true);//, true);
            path = Register.LineageFullPath(ancestors);//, currentItem);//, true);
            textboxPath.Text = path;

            labelRWLock.Text = $"write-lock:{readOnly}";

            // finally load the entry into rtb
            loadSelectedEntry(currentItem.Id);

            //this.Enabled = true;
            //formOperation.close();
            return true;
        }

        public bool reloadChildren(Int64 id, ListView lv)
        {
            // phase 1 - add all subdirectories which exist in currently selected/doubleclicked path

            // phase 1 - firstly get the registry node item
            List<RegisterItem>? registry = null;
            if (!Register.LoadFullPath(dbCtx, dbCtx.dbNodeTreeRegistryFile, id, ref registry, true))
                return false; // critical error abort

            if (registry.Count == 0) return false;

            // phase 2 - first at the top place, add parent child relation list items

            lv.BeginUpdate();
            lv.Items.Clear();

            int dirIndex = 0;
            // first add root path navigator
            RegisterItem? rootItem = registry.FirstOrDefault();
            LVChildrenAddItem(lv, rootItem, dirIndex++, @"\");
            // add current path as "." so that we can double click it to reload this current path with any changes.
            RegisterItem? currentItem = registry.LastOrDefault();
            LVChildrenAddItem(lv, currentItem, dirIndex++, ".");
            // now add parent if there is a parent with name ".." to climb to parent
            if (currentItem.Id != 0)
            {
                // this node is normal child node and has a parent other than root 0 id, so we must add ".." as the parent node link
                // if root 0 id then we cannot insert ".." parent node link.
                RegisterItem? parentItem = registry.Find(x => x.Id == currentItem.parentId);
                LVChildrenAddItem(lv, parentItem, dirIndex++, "..");
            }

            String rtf = "";
            byte[]? xamlbytesOut = null;
            RegisterItem? child = currentItem.children.Next();
            while (child != null)
            {
                if (child.domainType != DomainType.HiddenCore && child.nodeType != NodeType.EmptySlot)
                {
                    child.loadNode(dbCtx, ref rtf, ref xamlbytesOut, false);
                    String label = entryMethods.getEntryLabel(child.node, false);
                    String formatted = $"{label} ({child.childrenCount} children)";
                    LVChildrenAddItem(lv, child, dirIndex++, formatted);
                }
                child = currentItem.children.Next();
            }
            lv.EndUpdate();
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

        public void loadSelectedEntry(Int64 id)
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

            if (currentPathItem == null) return;

            // save current caret position
            entryMethods.DBUpdateCaretConfig(dbCtx, currentPathItem.node, rtbEntry.SelectionStartOffset, rtbEntry.SelectionLength);

            // check if body was changed
            if (!stateChanged)
                return;

            if (dbCtx.readOnly)
            {
                MessageBox.Show("cannot save entry. database read-only.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // first rotate and save new state for emergency restore
            __rotateInsertState();

            // save the entry
            entryMethods.DBUpdateNodeOFSDB(dbCtx, currentPathItem.node, rtbEntry.Rtf, rtbEntry.XamlBytes, true, true, true);

            // update db config
            dbCtx.dbConfig.lastModifiedEntry = currentPathItem.Id;
            DatabaseConfig.toXmlFile(dbCtx, dbCtx.dbConfig, dbCtx.dbConfigFile);

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

            // system node title cannot be modified
            if (currentPathItem.node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return;

            string? input = currentPathItem.node.chapter.Title;
            if (userInterface.ShowInputDialog("input title for entry", ref input) != DialogResult.OK)
                return;

            setupEntryTitle(input);
        }

        private void entryTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeEntryTitle();
        }

        public void setupEntryTitle(String title)
        {
            if (currentPathItem == null) return;
            if (currentPathItem.Id == 0) return;

            if (!entryMethods.DBUpdateNodeTitle(dbCtx, currentPathItem.node, title)) return;

            // update form
            reloadPath("", false, currentPathItem);
        }

        private void titleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeEntryTitle();
        }

        public void pastertbEntry()
        {
            rtbEntry.Paste();
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
        public void __gotoEntryById(Int64 id)
        {
            reloadPath(id.ToString(), true, null);
        }

        public void __gotoEntryByAttribute(bool lm, bool lc, bool byID, Int64 id)
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

            setupNewEntry(inputDate, -1);
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
            doExportCheckedEntries(EntryType.Html);
        }

        public void doExportCheckedEntries(EntryType entryType)
        {
            if (!dbCtx.isDBOpen())
                return;

            browseFolder.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Application.StartupPath;
            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            if (!dbCtx.isDBOpen()) return;

            // firstly save entry
            __saveEntry();

            this.Invoke(toggleForm, false);

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            foreach (ListViewItem lvItem in lvCurrentPath.CheckedItems)
            {
                if ((lvItem.SubItems[1].Text == @"\") || (lvItem.SubItems[1].Text == @".") || (lvItem.SubItems[1].Text == @"..")) continue;
                entryMethods.DBExportNodeTree(dbCtx, (Int64)lvItem.Tag, browseFolder.SelectedPath, entryType, formOperation);
            }

            this.Invoke(toggleForm, true);
            formOperation.close();

            // update form
            reloadPath("", false, currentPathItem);
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
            String err = "error configuration. retry after correcting it. aborted.";
            int rtbViewEntryRightMargin = 0;
            if (!int.TryParse(cmbCfgRtbViewEntryRM.Text, out rtbViewEntryRightMargin))
            {
                MessageBox.Show(err, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // first save config which is allowed to be saved while db is loaded.
            rtbViewEntry.RightMargin = rtbViewEntryRightMargin;
            cfg.cmbCfgRtbViewEntryRMValue = rtbViewEntryRightMargin;

            // now set other 1st level config
            cfg.radCfgLMNode = radCfgLMNode.Checked;
            cfg.radCfgLCNode = radCfgLCNode.Checked;
            cfg.chkCfgAutoLoadCreateDefaultDB = chkCfgAutoLoadCreateDefaultDB.Checked;
            myConfigMethods.saveConfigFile(myConfigMethods.getConfigPathFile(), ref cfg, false);
            MessageBox.Show("applied all levels of configurations.", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                setupNewEntry(DateTime.Now, currentPathItem.Id);
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
                setupNewEntry(DateTime.Now, currentPathItem.Id, NodeType.Label, input);

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
            Int64 id = Int64.Parse(treeNode.Name);
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
            Int64 id = Int64.Parse(treeNode.Name);
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

            Int64 id = Int64.Parse(treeNode.Name);
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
            List<Int64> locations = lvItemsRegisterItemIds(lvSearchWhere);
            /*
             if (locations.Count == 0)
            {
                // no location provided, auto set to root 0
                RegisterItem? root = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, 0, false, false, false, false, false, false);
                if (root == null) return; // error abort
                locations.Add(root);
            }
            */

            bool result = journalSearchFramework.searchEntries(dbCtx, this, txtSearchProgressFullPath, lvSearch, tsSearchProgressBar,
                dtpickerSearchFrom.Value, dtpickerSearchFromTime.Value, dtpickerSearchThrough.Value, dtpickerSearchThroughTime.Value, chkSearchUseDateRange.Checked,
                dtpickerCDSearchFrom.Value, dtpickerCDSearchFromTime.Value, dtpickerCDSearchThrough.Value, dtpickerCDSearchThroughTime.Value, chkSearchUseCreationDateRange.Checked,
                dtpickerMDSearchFrom.Value, dtpickerMDSearchFromTime.Value, dtpickerMDSearchThrough.Value, dtpickerMDSearchThroughTime.Value, chkSearchUseModificationDateRange.Checked,
                dtpickerDDSearchFrom.Value, dtpickerDDSearchFromTime.Value, dtpickerDDSearchThrough.Value, dtpickerDDSearchThroughTime.Value, chkSearchUseDeletionDateRange.Checked,
                rtbSearch.Text, rtbSearchReplace.Text, chkSearchAll.Checked,
                chkSearchTrashCan.Checked, chkSearchMatchCase.Checked, chkSearchMatchWholeWord.Checked,
                chkSearchReplace.Checked, chkSearchReplaceTitle.Checked, chkSearchEmptyString.Checked, locations);

            reload();
            return result;
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

        public List<Int64> lvItemsRegisterItemIds(ListView lv)
        {
            List<Int64> list = lv.Items.Cast<ListViewItem>()
                                       .Select(c => (Int64)c.Tag)
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
            Int64 id = getSelectedListViewNodeId(lvSearch);
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
            dtpickerDDSearchFrom.Value = DateTime.Now;
            dtpickerDDSearchThrough.Value = DateTime.Now;
            dtpickerDDSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerDDSearchThroughTime.Value = DateTime.Parse("23:59:59");
            lvSearchWhere.Tag = null;
            lvSearchWhere.Items.Clear();
        }

        public void deleteSearchedList()
        {
            if (!dbCtx.isDBOpen())
                return;

            saveEntry();

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            List<myNode>? worklist = new List<myNode>();
            worklist.AddRange(allNodes);
            foreach (System.Windows.Forms.ListViewItem listViewItem in lvSearch.CheckedItems)
            {
                // delete checked nodes
                Int64 id = Int64.Parse(listViewItem.Name);
                myNode? node = entryMethods.FindNodeInList(allNodes, id);
                if (node == null)
                    return;

                entryMethods.DBDeleteOrPurgeNodeRecursive(dbCtx, ref worklist, node, true, false, false);

                listViewItem.Checked = false;
                listViewItem.Selected = true;
            }

            formOperation.close();

            // auto create/load all nodes
            //todo mySystemNodes? systemNodes = null;
            //reloadAll(true, true, true, true, ref systemNodes);

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
            myFormFind.myParentForm = this;
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

        public void exportSet(DatabaseType dbType, bool checkedNodesSet)
        {
            /* todo
            // firstly save entry
            saveEntry();

            if (!dbCtx.isDBOpen())
                return;

            String destPath = "";
            String dbName = "";
            if (userInterface.ShowInputDialog("input set name/title", ref dbName) != DialogResult.OK) return;
            if (dbName.Length <= 0) return;

            // destination set db
            switch (dbType)
            {
                case DatabaseType.OpenFSDB:
                    if (browseFolder.ShowDialog(this) != DialogResult.OK)
                        return;

                    destPath = browseFolder.SelectedPath;

                    break;

                case DatabaseType.SingleFileDB:
                    sfdDB.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    if (sfdDB.ShowDialog() != DialogResult.OK)
                        return;

                    destPath = sfdDB.FileName;

                    break;
                default:
                    return;
            }

            this.Invoke(toggleForm, false);

            // what to export
            List<myNode>? selectedNodes = null;
            if (checkedNodesSet)
                selectedNodes = (List<myNode>)this.Invoke(getHighestCheckedTreeViewItemsDBNodes, tvEntries);
            else
                selectedNodes = getRootNodes();

            // export selected nodes set
            entryMethods.ExportSet(this, ref cfg, ref allNodes, ref selectedNodes, dbName, destPath, dbType, true);

            // reload without saving currently selected node because it was saved previously.
            loadDB();

            this.Invoke(toggleForm, true);
            */
        }

        public void importSet(DatabaseType dbType)
        {
            // todo everything

            // firstly save entry
            saveEntry();

            if (!dbCtx.isDBOpen())
                return;

            // source db
            String dbName = "";
            //myConfig? cfgSrc = entryMethods.DBSelectOpenLoadDB(dbType, ref dbName, null);
            //i//f (cfgSrc == null) return;

            // import source set db
            //this.Invoke(toggleForm, false);
            //entryMethods.ImportSet(this, ref cfgSrc, ref cfg, dbName, ref allNodes, dbType, true);

            // reload without saving currently selected node because it was saved previously.
            loadDB();

            this.Invoke(toggleForm, true);

        }

        private void newLabelNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doNewLabelEntry();
        }

        public void gotoTreeNodeByDBNode(myNode? node)
        {
            /* todo
            if (node == null)
                return;

            String path = String.Format(@"{0}", node.chapter.Id);
            TreeNode[] matchingNodes = tvEntries.Nodes.Find(path, true);
            if (matchingNodes.Length <= 0)
                return;

            TreeNode treeNode = matchingNodes[0];

            tvEntries.Focus();
            tvEntries.SelectedNode = treeNode;
            treeNode.ExpandAll();
            treeNode.EnsureVisible();
            */
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
            doExportCheckedEntries(EntryType.Txt);
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            doExportCheckedEntries(EntryType.Rtf);
        }

        private void promoteNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            if (currentPathItem == null) return;
            if (currentPathItem.Id == 0) return;

            // skip if this is system node, we cannot change it
            if (currentPathItem.node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return;

            // update node in db
            entryMethods.DBPromoteNodeOFSDB(dbCtx, currentPathItem.node);

            // get current register item latest state
            RegisterItem? currentItem = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, currentPathItem.Id, true, false, false, false, false, false);
            if (currentItem == null) return;

            //setup
            currentPathItem = currentItem;

            // update form
            reloadPath("", false, currentPathItem);

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
        public void MoveNode(Int64 destId)
        {
            if (!dbCtx.isDBOpen())
                return;

            if (currentPathItem == null) return;
            if (currentPathItem.Id <= 0) return;
            if (currentPathItem.Id == destId) return;

            // skip if this is system node, we cannot change it
            if (currentPathItem.node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return;

            if (currentPathItem.Id == destId)
                return;

            // move node 
            entryMethods.DBMoveNodeOFSDB(dbCtx, currentPathItem.Id, destId);

            // get current register item latest state
            RegisterItem? currentItem = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, currentPathItem.Id, true, false, false, false, false, false);
            if (currentItem == null) return;

            //setup
            currentPathItem = currentItem;

            // update form
            reloadPath("", false, currentPathItem);
        }
        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            if (currentPathItem == null) return;
            if (currentPathItem.Id <= 0) return;

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

        public RegisterItem? doExportCustomEntry(Int64 id)
        {
            // get latest state register item
            RegisterItem? item = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, id, true, false, false, false, false, false);
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

        private void checkmarkAllNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LVMarkUnmark(lvCurrentPath, true, false, true, false);
        }

        private void uncheckAllNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LVMarkUnmark(lvCurrentPath, true, false, false, false);
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
            doExportCheckedEntries(EntryType.Pdf);
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

            // setup
            currentPathItem = item;

            // update form
            reloadPath("", false, currentPathItem);
        }

        private void entryCommonDateAndTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        private void toolStripMenuItem38_Click(object sender, EventArgs e)
        {
        }

        public void doSortTreeNodeFirstLevelChildren(TreeView? tv, TreeNode? treeNode, ref FormSortOptions? form)
        {
        }
        public void doSortTreeNodeRecursive(TreeView? tv, TreeNode? treeNode, ref FormSortOptions? form)
        {
        }

        private void sortNodesChildrenRecursivelyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void sortFirstLevelRootNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void sortAllRootNodesRecursivelyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void sortCheckedNodesFirstChildrenToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void sortCheckedNodesRecursivelyToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void buttonSearchResetDDates_Click(object sender, EventArgs e)
        {
            dtpickerDDSearchFrom.Value = DateTime.Now;
            dtpickerDDSearchThrough.Value = DateTime.Now;
            dtpickerDDSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerDDSearchThroughTime.Value = DateTime.Parse("23:59:59");
        }
        private void toolStripMenuItem46_Click(object sender, EventArgs e)
        {
            exportSet(DatabaseType.OpenFSDB, false);
        }

        private void toolStripMenuItem47_Click(object sender, EventArgs e)
        {
            exportSet(DatabaseType.OpenFSDB, true);
        }

        private void toolStripMenuItem48_Click(object sender, EventArgs e)
        {
            importSet(DatabaseType.OpenFSDB);
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
            Int64 id = Int64.Parse(lvitem.Name);
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


            __importTheJournalNonCalendarRtfEntriesNew(browseFolder.SelectedPath, input, true);

        }

        public void __importTheJournalNonCalendarRtfEntriesNew(String path, String importSetName, bool loadOperationForm)
        {
            // first save the entry
            this.Invoke(saveEntry);

            if (!dbCtx.isDBOpen())
                return;

            this.Invoke(toggleForm, false);

            // operations status form
            FormOperation? formOperation = null;
            if (loadOperationForm)
                formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            //AdvRichTextBox rtb = new AdvRichTextBox();
            //rtb.WordWrap = false;
            //rtb.Multiline = true;
            //rtb.SuspendLayout();
            //rtb.BeginUpdate();

            IEnumerable<String> files = Directory.EnumerateFiles(path, "*.rtf");
            long index = 0;
            long total = files.LongCount();

            // first we need to create a set node. we cannot import anything at all without a set node.
            myNode? setNode = entryMethods.createSetNode(dbCtx, importSetName, DateTime.Now);

            // import set node
            byte[]? xamlbytes = null;
            //entryMethods.DBCreateNode(dbCtx, ref setNode, "", xamlbytes, false, false, false, false, true, true);

            // this set's own context session work list
            List<myNode> setList = new List<myNode>();

            foreach (String file in files)
            {
                // notice: any tree node level in entire tree cannot have more than 1 nodes with exactly same name/title.
                // this leads to destruction of nodes of same name and level while importing.
                // so you must ensure there is no duplicate named/titled node in any tree level.
                // if there is any duplicate node, you must give it a unique name/title which differs in all children of a tree node level.
                // also, each and every entry which is to be imported, must have a unique name/title in it's filename.

                // auto create direct all nodes line by node names which exist in the file name itself.
                // todo List<String> nodeNames = theJournalMethods.partitionEntryFileIntoNodes(file);
                // todo List<myNode> nodesLine = theJournalMethods.initNodesLineTJNC(dbCtx, ref setList, ref nodeNames, setNode.chapter.Id);
                // todo myNode? targetNode = nodesLine.Last();

                // get rtf and update
                // richtextbox automatically cleans and fixes a corrupted rtf and makes it valid.
                // so we first load the imported rtf into a richtextbox object, then retrieve the cleaned and fixed
                // rtf from it and only then store it in db.
                String rtf = File.ReadAllText(file);
                // todo rtf = theJournalMethods.fixTheJournalRtfEntry(rtf);
                xamlEntry.dummy.Rtf = rtf;
                //rtb.Rtf = rtf;
                //rtf = rtb.Rtf;

                // finally write body of the node in db
                //entryMethods.DBUpdateNodeOFSDB(dbCtx, ref targetNode, xamlEntry.dummy.Rtf, xamlEntry.dummy.XamlBytes, true, true);

                // update ui
                if (loadOperationForm)
                {
                    formOperation.updateProgressBar(index, total);
                    formOperation.updateFilesStatus(index, total);
                }

                // update
                index++;
            }

            // finally update the db index in file.
            //entryMethods.DBWriteIndexing(dbCtx);

            // now first add set node
            allNodes.Add(setNode);

            // now add all set list into the global session work list
            allNodes.AddRange(setList);

            this.Invoke(toggleForm, true);

            if (loadOperationForm)
                formOperation.close();

            //todo mySystemNodes? systemNodes = null;
            //reloadAll(true, true, true, true, ref systemNodes);

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
            Register.Lineage(dbCtx, dbCtx.dbNodeTreeRegistryFile, currentPathItem, ref ancestors, true);//, true);
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

            this.Enabled = false;

            resetRtb(rtbEntry, true, true);
            String? rtf = "";
            byte[]? xamlbytesOut = null;
            currentPathItem.loadNode(dbCtx, ref rtf, ref xamlbytesOut, true);
            if (dbCtx.dbEntryType == EntryType.Xaml)
                rtbEntry.XamlBytes = xamlbytesOut;
            else
                rtbEntry.Rtf = rtf;

            resetRtb(rtbEntry, false, true);

            txtEntryTitle.Text = currentPathItem.node.chapter.Title;
            tsslabelLMD.Text = currentPathItem.node.chapter.modificationDateTime.ToString("HH:mm:ss:fff, dddd, dd MMMM yyyy");
            tsslabelID.Text = currentPathItem.node.chapter.Id.ToString();
            tsslabelPID.Text = currentPathItem.node.chapter.parentId.ToString();

            this.Enabled = true;

            // now setup caret config
            rtbEntry.SelectionStartOffset = currentPathItem.node.chapter.caretIndex;
            if (currentPathItem.node.chapter.caretSelectionLength != 0)
                rtbEntry.SelectionLength = currentPathItem.node.chapter.caretSelectionLength;
        }
        private void configureEntrysWidthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // first save the current entry
            saveEntry();

            if (!dbCtx.isDBOpen())
                return;

            if (currentPathItem == null) return;

            // skip if this is system node, we cannot change it
            if (currentPathItem.node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return;

            configureEntryWidth(rtbEntry, currentPathItem.Id);


        }

        public void configureEntryWidth(System.Windows.Controls.WpfRichTextBoxEx rtb, Int64 id)
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
            RegisterItem? item = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, id, true, false, false, false, false, false);
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

            // update form
            reloadPath("", false, currentPathItem);
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
            Register.Lineage(dbCtx, dbCtx.dbNodeTreeRegistryFile, registryItem, ref ancestors, true);
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
            // firstly save entry
            __saveEntry();

            if (currentPathItem == null) return;
            if (currentPathItem.Id == 0) return;

            // skip if this is system node, we cannot change it
            if (currentPathItem.node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return;

            if (!cloneEntry(currentPathItem.Id, currentPathItem.parentId))
            {
                MessageBox.Show(this, "error occured while cloning the current node.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool cloneEntry(Int64 id, Int64 locationId)
        {
            if (!dbCtx.isDBOpen())
                return false; // error


            // validations
            RegisterItem? parent = null;
            Register.FindNode(dbCtx, dbCtx.dbNodeTreeRegistryFile, locationId, ref parent);
            if (parent.childrenCount >= Register.default_maxChildrenNodes)
            {
                MessageBox.Show($"error entry not created. maximum direct new children create/insert limit [{Register.default_maxChildrenNodes}] reached for/in this target parent.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            RegisterItem? cloneItem = entryMethods.DBCloneNodeOFSDB(dbCtx, id, locationId, ref emptySlotsItem);
            if (cloneItem == null) return false;

            //setup
            currentPathItem = cloneItem;

            // update db config file
            dbCtx.dbConfig.latestCreatedEntry = cloneItem.Id;
            DatabaseConfig.toXmlFile(dbCtx, dbCtx.dbConfig, dbCtx.dbConfigFile);

            // update form
            reloadPath("", false, currentPathItem);

            return true;
        }

        private void cloneAtParentLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // firstly save entry
            __saveEntry();

            if (currentPathItem == null) return;
            if (currentPathItem.Id == 0) return;
            if (currentPathItem.parentId == 0) return;

            // skip if this is system node, we cannot change it
            if (currentPathItem.node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return;

            RegisterItem? parent = null;
            if (Register.FindNode(dbCtx, dbCtx.dbNodeTreeRegistryFile, currentPathItem.parentId, ref parent) < 0) return;
            if (parent == null) return;

            if (!cloneEntry(currentPathItem.Id, parent.parentId))
                MessageBox.Show(this, "error occured while cloning the current node.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void cloneToOtherLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // firstly save entry
            __saveEntry();

            if (currentPathItem == null) return;
            if (currentPathItem.Id == 0) return;

            // skip if this is system node, we cannot change it
            if (currentPathItem.node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return;

            FormTreeDesign2 form = new FormTreeDesign2();
            form.ctx = dbCtx;
            if (form.ShowDialog() != DialogResult.OK) return;
            if (form.selectedPathItem == null) return;

            if (!cloneEntry(currentPathItem.Id, form.selectedPathItem.Id))
                MessageBox.Show(this, "error occured while cloning the current node.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);


        }

        private void toolStripMenuItem75_Click(object sender, EventArgs e)
        {
            // firstly save entry
            __saveEntry();

            if (currentPathItem == null) return;
            if (currentPathItem.Id == 0) return;

            // skip if this is system node, we cannot change it
            if (currentPathItem.node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return;

            if (!cloneEntry(currentPathItem.Id, 0)) // clone at root location
                MessageBox.Show(this, "error occured while cloning the current node.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void buttonResetConfig1_Click(object sender, EventArgs e)
        {
            cmbCfgRtbViewEntryRM.SelectedIndex = cmbCfgRtbViewEntryRM.FindString(myConfig.default_cmbCfgRtbViewEntryRMValue.ToString());
        }
        private void gotoLatestEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            __gotoEntryByAttribute(false, true, false, -1);

        }

        private void gotoLatestLastModifiedEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            __gotoEntryByAttribute(true, false, false, -1);
        }


        private void gotoEntryByIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string? input = "0";
            if (userInterface.ShowInputDialog("input entry id", ref input) != DialogResult.OK)
                return;

            // check validity
            Int64 value = -1;
            if (!Int64.TryParse(input, out value))
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
                return;
            }
        }
        private void toolStripMenuItem19_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

            if (browseFolder.ShowDialog() != DialogResult.OK) return;

            // firstly save entry
            saveEntry();

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
                return;
            }

            formOperation.close();

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
            form.allItems = cfg.templateFormat.findAllTemplateCodeItems();
            if (form.ShowDialog(this) != DialogResult.OK) return;
            if (form.outSelectedItem == null) return;
            TemplateFormat.TemplateCodeItem item = (TemplateFormat.TemplateCodeItem)form.outSelectedItem;
            formatting.formatInsertString(rtbEntry, item.value);
        }

        private void toolStripMenuItem81_Click(object sender, EventArgs e)
        {
            if (!dbCtx.isDBOpen())
                return;

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
            parent = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, parent.Id, true, false, true, true, true, true);
            if (parent == null) return;

            // show all templates list form and let the user choose a single template
            FormNodeList form = new FormNodeList();
            form.registry = parent.childrenList;
            form.nodeTypes.Add(NodeType.Template);
            form.checkMultipleNodes = false;
            form.listDeletedNodes = false;
            if (form.ShowDialog(this) != DialogResult.OK) return;
            if (form.outSelectedNode == null) return;

            // load rtf data of the selected template from it's db node
            RegisterItem? item = form.outSelectedNode;
            String? rtf = "";
            byte[]? xamlbytes = null;
            item.loadNode(dbCtx, ref rtf, ref xamlbytes, true);

            // initialize an rtb control and set the rtf into it for processing
            if (dbCtx.dbEntryType == EntryType.Xaml)
                xamlEntry.dummy.XamlBytes = xamlbytes;
            else
                xamlEntry.dummy.Rtf = rtf;

            // now fetch all template codes
            List<Object> codes = cfg.templateFormat.findAllTemplateCodeItems();

            // now transform all codes
            foreach (Object? codeItem in codes)
            {
                if (codeItem == null) continue;
                TemplateFormat.TemplateCodeItem code = (TemplateFormat.TemplateCodeItem)codeItem;
                cfg.templateFormat.transform(xamlEntry.dummy, code);
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
            long processed = 0;
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
            if (lvCurrentPath.SelectedItems.Count == 0) return;
            ListViewItem selectedItem = lvCurrentPath.SelectedItems[0];
            Int64 id = (Int64)selectedItem.Tag;
            reloadChildren(id, lvChildren);
        }

        private void lvCurrentPath_KeyPressed(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                // Enter key pressed
                if (lvCurrentPath.SelectedItems.Count == 0) return;
                ListViewItem selectedItem = lvCurrentPath.SelectedItems[0];
                Int64 id = (Int64)selectedItem.Tag;
                reloadChildren(id, lvChildren);
            }
        }
        private void lvCurrentPath_DoubleClick(object sender, EventArgs e)
        {
            if (lvCurrentPath.SelectedItems.Count == 0) return;
            ListViewItem selectedItem = lvCurrentPath.SelectedItems[0];
            Int64 id = (Int64)selectedItem.Tag;
            reloadPath(id.ToString(), true, null);
        }

        private void lvChildren_Click(object sender, EventArgs e)
        {
            if (lvChildren.SelectedItems.Count == 0) return;
            ListViewItem selectedItem = lvChildren.SelectedItems[0];
            Int64 id = (Int64)selectedItem.Tag;
            reloadChildren(id, lvChildsChildren);

        }
        private void lvChildren_KeyPressed(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                // Enter key pressed
                if (lvChildren.SelectedItems.Count == 0) return;
                ListViewItem selectedItem = lvChildren.SelectedItems[0];
                Int64 id = (Int64)selectedItem.Tag;
                reloadChildren(id, lvChildsChildren);
            }
        }

        private void lvChildren_DoubleClick(object sender, EventArgs e)
        {
            if (lvChildren.SelectedItems.Count == 0) return;
            ListViewItem selectedItem = lvChildren.SelectedItems[0];
            Int64 id = (Int64)selectedItem.Tag;
            reloadPath(id.ToString(), true, null);
        }
        private void lvChildsChildren_DoubleClick(object sender, EventArgs e)
        {
            if (lvChildsChildren.SelectedItems.Count == 0) return;
            ListViewItem selectedItem = lvChildsChildren.SelectedItems[0];
            Int64 id = (Int64)selectedItem.Tag;
            reloadPath(id.ToString(), true, null);
        }
        private void lvSiblings_Click(object sender, EventArgs e)
        {
        }

        private void lvSiblings_DoubleClick(object sender, EventArgs e)
        {
            saveEntry();
            if (lvSiblings.SelectedItems.Count == 0) return;
            ListViewItem selectedItem = lvSiblings.SelectedItems[0];
            Int64 id = (Int64)selectedItem.Tag;
            reloadPath(id.ToString(), true, null);
        }

        private void lvSiblings_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lvCurrentPath_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FormJournalDesign2_SizeChanged(object sender, EventArgs e)
        {
            // top horizontal split container for toolbox
            splitContainerH.SplitterDistance = 30;

            // horizontal split container for top path and title panel and lower navigation panel
            splitContainer1.SplitterDistance = 30;

            // central horizontal split container for journal navigation and editor
            splitContainerHInner1.SplitterDistance = splitContainerHInner1.Size.Height / 3;

            // navigation vertical split container for parent to children navigation panels
            splitContainer3.SplitterDistance = splitContainer3.Size.Width / 3;

            // rtbedit vertical split container for editor
            splitContainer4.SplitterDistance = splitContainer4.Size.Width / 3;

            // navigation vertical split container for children and grand children navigation panels
            splitContainer2.SplitterDistance = splitContainer2.Size.Width / 2;

            // text box for entry title
            txtEntryTitle.Width = this.ClientSize.Width;
        }

        private void FormJournalDesign2_Resize(object sender, EventArgs e)
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
                return;
            }

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

            this.Invoke(toggleForm, false);

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // now process
            long processed = 0;
            entryMethods.DBConvertOFSDB(dbCtx, toXaml, out processed, formOperation);

            this.Invoke(toggleForm, true);
            formOperation.close();

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
            form.listType = FormList.ListType.Int64;
            form.allItems = found;

            currentPathItem = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, currentPathItem.Id, false, false, true, false, true, true);
            currentPathItem.tree.GetDescendantTreeSequence(ref currentPathItem.treeList);

            // iterate
            foreach (RegisterItem item in currentPathItem.treeList)
                found.Add(item.Id);

            // let the user choose and insert a single template code into the template entry
            form.Text = $"total descendants of current node: {found.LongCount()}";
            if (form.ShowDialog(this) != DialogResult.OK) return;

            Int64 id = (Int64)form.outSelectedItem;
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

            if (MessageBox.Show("warning: are you sure you want to purge checked nodes? all of their descendants trees will also be forever purged along with them!\n" +
                "there is no need to purge anything! you can recycle and reuse the nodes! you have 8+ million slots!", "warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            // firstly save entry
            __saveEntry();

            this.Invoke(toggleForm, false);

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            foreach (ListViewItem lvItem in lvCurrentPath.CheckedItems)
            {
                if ((lvItem.SubItems[1].Text == @"\") || (lvItem.SubItems[1].Text == @".") || (lvItem.SubItems[1].Text == @"..")) continue;
                RegisterItem? regitem = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, (Int64)lvItem.Tag, false, false, false, false, true, true);
                this.emptySlotsItem = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, this.emptySlotsItem.Id, false, false, false, false, true, true);
                RegisterItem? parent = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, regitem.parentId, false, false, false, false, true, true);
                parent.tree.Delete(regitem, emptySlotsItem);
            }

            this.Invoke(toggleForm, true);
            formOperation.close();

            // reload to correct sync
            reloadPath("", false, currentPathItem);
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
                    RegisterItem? parent = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, node.chapter.parentId, false, false, false, false, true, true);

                    if (parent.nodeType == NodeType.EmptySlot)
                        found.Add(node.chapter.Id);


                }
            }

            this.Invoke(toggleForm, true);
            formOperation.close();

            // let the user choose orphan nodes to restore
            FormList form = new FormList();
            form.Text = "choose orphan nodes to restore them in any location";
            form.checkMultipleItems = true;
            form.listType = FormList.ListType.Int64;
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
                myNode? node = entryMethods.DBSearchNodeOFSDB(dbCtx, (Int64)id, ref rtf, ref xaml, true);
                if (node == null) continue;

                node.chapter.parentId = form2.selectedPathItem.Id;

                // move node 
                entryMethods.DBSetNodeParent(dbCtx, node, form2.selectedPathItem.Id);

                RegisterItem? item = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, (Int64)id, false, false, false, false, true, true);
                if (item == null) continue;

                item.sectionId = node.DirectorySectionID;
                item.domainType = node.chapter.domainType;
                item.specialNodeType = node.chapter.specialNodeType;
                item.parentId = form2.selectedPathItem.Id;
                item.nodeType = node.chapter.nodeType;
                item.node = node;
                Register.UpdateNode(dbCtx, dbCtx.dbNodeTreeRegistryFile, item, form2.selectedPathItem.Id, true, node.DirectorySectionID, true, 0, true);

                item = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, (Int64)id, false, false, false, false, true, true);

                RegisterItem? parent = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, form2.selectedPathItem.Id, false, false, false, false, true, true);
                if (parent == null) continue;

                parent.children.Add(item);

                item = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, (Int64)id, false, false, false, false, true, true);
                parent = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, form2.selectedPathItem.Id, false, false, false, false, true, true);

                parent.tree.Add(item);
            }

            // get current register item latest state
            //RegisterItem? currentItem = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, currentPathItem.Id, true, false, false, false, false, false);
            //if (currentItem == null) return;

            //setup
            //currentPathItem = currentItem;

            // update form
            reloadPath("", false, currentPathItem);

        }
    }
}
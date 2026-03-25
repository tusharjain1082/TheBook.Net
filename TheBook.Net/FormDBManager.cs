using System.Windows.Forms;
using System;
using System.Runtime.InteropServices;
using TheBook.Net.Core;
using System.ComponentModel;
using DiaryJournal.Net;
using System.IO;
using System.Threading;
using System.Reflection;

namespace TheBook.Net
{
    public partial class FormDBManager : Form
    {
        public OpenFSDBContext? dbCtx = null;

        public String recentDBPath = "";

        public DiaryJournal.Net.FormJournalDesign2 journalForm = null;

        public FormDBManager()
        {
            InitializeComponent();
        }

        private void FormDBManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseDB();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            // auto create local paths - mandatory
            CoreFramework.autoCreateInitLocalPaths();

            String strDateTimeTemplate = TheBook.Net.Properties.Resources.BuildDateTime;
            DateTime buildDateTime = DateTime.Parse(strDateTimeTemplate);
            String strBuildDateTime = buildDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            System.Version version = Assembly.GetExecutingAssembly().GetName().Version;
            this.Text = $"Tushar Jain's TheBook.Net Version {version} Compiled/Built on: {strBuildDateTime}::- {this.Text}";
        }

        private void buttonNewNonVHDDB_Click(object sender, EventArgs e)
        {
            newDB();
        }
        public void newDB()
        {
            if (txtBoxDBPath.Text.Length != 0) return;
            if (dbCtx != null) return; // we cannot proceed when one session is active.

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.AddToRecent = true;
            fbd.AutoUpgradeEnabled = true;
            fbd.InitialDirectory = CoreFramework.myDBPath;
            if (fbd.ShowDialog() != DialogResult.OK) return;

            string? input = "myJournal";
            if (userInterface.ShowInputDialog("input title/name/identity for database", ref input) != DialogResult.OK)
                return;

            bool isXamlDB = false;
            if (MessageBox.Show("would you like to create xaml db? no means db with rtf entries.",
                "warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) isXamlDB = true;

            OpenFSDBContext? ctx = new OpenFSDBContext();

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);
            if (OpenFileSystemDB.PrepareDB(ctx, fbd.SelectedPath, input, isXamlDB))
            {
                formOperation.close();
                recentDBPath = ctx.dbBasePath;
                MessageBox.Show("new local db created successfully at your selected path.",
                    "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                formOperation.close();
                MessageBox.Show("error. aborted", "error");
            }
            // finally close the context
            ctx.close();
        }
        private void buttonOpenNVDB_Click(object sender, EventArgs e)
        {
            openDB();
        }

        public void openDB()
        {
            if (txtBoxDBPath.Text.Length != 0) return;

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.AddToRecent = true;
            fbd.AutoUpgradeEnabled = true;

            if (recentDBPath != "")
                fbd.InitialDirectory = recentDBPath;
            else
                fbd.InitialDirectory = CoreFramework.myDBPath;

            if (fbd.ShowDialog() != DialogResult.OK) return;

            OpenFSDBContext? ctx = new OpenFSDBContext();

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);
            if (OpenFileSystemDB.LoadVHDFileDB(ctx, fbd.SelectedPath, checkBoxWriteLock.Checked))
            {
                txtBoxDBPath.Text = fbd.SelectedPath;
                dbCtx = ctx;
                formOperation.close();
                recentDBPath = fbd.SelectedPath;
                MessageBox.Show("db loaded successfully at your selected path.",
                    "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                formOperation.close();
                MessageBox.Show("error. aborted", "error");
            }
        }
        private void buttonLaunchNVDB_Click(object sender, EventArgs e)
        {
            launchDB();
        }

        public void launchDB()
        {
            if (txtBoxDBPath.Text.Length == 0) return;
            if (dbCtx == null) return;

            // reload empty slots system node
            //RegisterItem? root = Register.LoadSetupRegisterItem(dbCtx, dbCtx.dbNodeTreeRegistryFile, 0, true, false, true, false, true, true, true);

            // setup and launch journal
            if (journalForm == null)
            {
                journalForm = new DiaryJournal.Net.FormJournalDesign2();
                journalForm.dbCtx = dbCtx; // ctx;
                journalForm.myParentForm = this;
                journalForm.Show();
            }
            else
            {
                if (!journalForm.IsDisposed)
                {
                    journalForm.Show();
                }
                else
                {
                    journalForm = new DiaryJournal.Net.FormJournalDesign2();
                    journalForm.dbCtx = dbCtx; // ctx;
                    journalForm.myParentForm = this;
                    journalForm.Show();
                }
            }
            this.Hide();
        }

        public void CloseDB()
        {
            if (txtBoxDBPath.Text.Length == 0) return;

            txtBoxDBPath.Text = "";
            if (dbCtx != null)
            {
                dbCtx.writeUsedSlotsFile();
                DatabaseConfig.toYamlFile(dbCtx.dbConfig, dbCtx.dbConfigFile);
                dbCtx.close();
                dbCtx = null;
            }
            GC.Collect();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseDB();
            Application.Exit();
        }

        private void buttonCloseNVDB_Click(object sender, EventArgs e)
        {
            CloseDB();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (txtBoxDBPath.Text.Length == 0) return;
            if (dbCtx == null) return;

            // set/unset write lock in current session context
            dbCtx.readOnly = checkBoxWriteLock.Checked;
            MessageBox.Show("successfully set/unset write lock.", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void buttonURNVDB_Click(object sender, EventArgs e)
        {
            upgradeFixDB();
        }

        public void upgradeFixDB()
        {
            if (txtBoxDBPath.Text.Length == 0) return;
            if (dbCtx == null) return;
            if (dbCtx.readOnly) return;
            
            if (MessageBox.Show("warning: are you sure you want to upgrade/fix this db? please save a backup copy somewhere first!", "warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            // now reconfigure

            // create new used slots file
            dbCtx.usedSlots = 0;
            dbCtx.writeUsedSlotsFile();

            // create register and 1st node which is root node
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "phase 1 of 4: DBCreateRegister() - please wait. doing operation...", 0, 100, 0, 0);
            if (!OpenFileSystemDB.DBCreateRegister(dbCtx, true))
            {
                formOperation.close();
                MessageBox.Show("phase 1 DBCreateRegister() was not completed.", "failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            formOperation.close();

            // phase 3 - build this version's register
            formOperation = null;
            formOperation = FormOperation.showForm(this, "phase 2 of 4: DBBuildOFSDBRegister() - please wait. doing operation...", 0, 100, 0, 0);
            if (!entryMethodsNewDesign.DBBuildOFSDBRegister(dbCtx, formOperation))
            {
                MessageBox.Show("phase 2 DBBuildOFSDBRegister() was not completed.", "failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            formOperation.close();

            // phase 3 - we create all missing system nodes

            formOperation = null;
            formOperation = FormOperation.showForm(this, "phase 3 of 4: DBCreateDBCore() - please wait. doing operation...", 0, 100, 0, 0);
            // first create any missing root system node
            List<RegisterItem>? registry = null;
            RegisterItem? emptySlotsItem = null;
            if (!entryMethodsNewDesign.DBCreateDBCore(dbCtx, ref emptySlotsItem, true))
            {
                MessageBox.Show("phase 3 DBCreateDBCore() was not completed.", "failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            formOperation.close();

            // now load all root system nodes
            formOperation = null;
            formOperation = FormOperation.showForm(this, "phase 4 of 4: DBLoadRootSystemNodes() - please wait. doing operation...", 0, 100, 0, 0);
            if (!entryMethodsNewDesign.DBLoadRootSystemNodes(dbCtx, ref registry, ref emptySlotsItem))
            {
                MessageBox.Show("phase 4 DBLoadRootSystemNodes() load system nodes was not completed.", "failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            formOperation.close();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            newDB();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            openDB();

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            launchDB();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            CloseDB();

        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            upgradeFixDB();
        }
    }
}

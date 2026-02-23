using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheBook.Net;

namespace DiaryJournal.Net
{
    public partial class FormSelectDB : Form
    {

        public bool OpenFileSystemDB = false;
        public DatabaseType selectedDBType = DatabaseType.OpenFSDB;


        public FormSelectDB()
        {
            InitializeComponent();
        }

        private void FormSelectDB_Load(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            OpenFileSystemDB = radOFSDB.Checked;

            if (OpenFileSystemDB) selectedDBType = DatabaseType.OpenFSDB;

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }
    }
}

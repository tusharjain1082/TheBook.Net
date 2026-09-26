using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using TheBook.Net;
using TheBook.Net.Core;

namespace DiaryJournal.Net
{
    public partial class FormNodeList : Form
    {
        public List<RegisterItem> registry = new List<RegisterItem>();
        public List<NodeType> nodeTypes = new List<NodeType>();
        public bool checkMultipleNodes = true;
        public bool listDeletedNodes = false;

        public List<RegisterItem> outCheckedNodes = new List<RegisterItem>();
        public RegisterItem? outSelectedNode = null;

        public FormNodeList()
        {
            InitializeComponent();
        }

        private void FormNodeList_Load(object sender, EventArgs e)
        {
            lvNodeList.MultiSelect = checkMultipleNodes;
            lvNodeList.CheckBoxes = checkMultipleNodes;
            buttonAll.Enabled = buttonNone.Enabled = checkMultipleNodes;

            refillList();
        }
        private void buttonOK_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.ListViewItem listViewItem in lvNodeList.CheckedItems)
            {
                RegisterItem? node = (RegisterItem?)listViewItem.Tag;
                if (node == null) continue;
                outCheckedNodes.Add(node);
            }

            if (lvNodeList.SelectedItems.Count > 0) 
            {
                RegisterItem? node = (RegisterItem?)lvNodeList.SelectedItems[0].Tag;
                outSelectedNode = node;
            }
        }

        public void refillList()
        {
            lvNodeList.Items.Clear();

            foreach (RegisterItem? listedItem in registry)
            {
                RegisterItem? item = listedItem;
                if (item == null) continue;

                foreach (NodeType? type in nodeTypes)
                {
                    if (type == null) continue;

                    // check if this node is required, if not then skip
                    if (type != NodeType.AnyOrAll)
                    {
                        // a node type is exclusively given, so the node must be the same node type, else skip this node
                        if (item.node.chapter.nodeType != type)
                            continue;
                    }

                    // if deleted nodes are not wanted then skip them
                    if ((!listDeletedNodes) && (item.node.chapter.IsDeleted))
                        continue;

                    // node is required by node type, so add it in listview control
                    System.Windows.Forms.ListViewItem lvitem = new System.Windows.Forms.ListViewItem();
                    lvitem.Name = item.node.chapter.Id.ToString();
                    lvitem.Tag = item;
                    lvitem.Text = item.node.chapter.Title;

                    // dates
                    String chapterDateTime = item.node.chapter.chapterDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
                    lvitem.SubItems.Add(chapterDateTime);
                    lvitem.SubItems.Add(item.node.chapter.creationDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
                    lvitem.SubItems.Add(item.node.chapter.modificationDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
                    lvitem.SubItems.Add(item.node.chapter.deletionDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss"));

                    // deleted status
                    if (item.node.chapter.IsDeleted)
                        lvitem.SubItems.Add("trash can");
                    else
                        lvitem.SubItems.Add("common");

                    // special node type
                    lvitem.SubItems.Add(item.node.chapter.specialNodeType.ToString());

                    // node type
                    lvitem.SubItems.Add(item.node.chapter.nodeType.ToString());

                    // node's parent id
                    lvitem.SubItems.Add(item.node.chapter.parentId.ToString());

                    // node's id
                    lvitem.SubItems.Add(item.node.chapter.Id.ToString());

                    // other details
                    lvitem.SubItems.Add(item.node.chapter.Title);
                    lvNodeList.Items.Add(lvitem);
                }
            }

        }

        private void buttonAll_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.ListViewItem listViewItem in lvNodeList.Items)
            {
                listViewItem.Checked = true;
                listViewItem.Selected = true;
            }
        }

        private void buttonNone_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.ListViewItem listViewItem in lvNodeList.Items)
            {
                listViewItem.Checked = false;
                listViewItem.Selected = false;
            }
        }
    }
}

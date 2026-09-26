using TheBook.Net.Core;

namespace DiaryJournal.Net
{
    public partial class FormTreeDesign2 : Form
    {
        // TheBook.Net
        public OpenFSDBContext? ctx = null;
        public RegisterItem? selectedPathItem = null;

        public FormTreeDesign2()
        {
            InitializeComponent();
        }

        private void FormTreeDesign2_Shown(object sender, EventArgs e)
        {
            reload();
        }

        private void FormTreeDesign2_Load(object sender, EventArgs e)
        {
            lvCurrentPath.KeyPress += new System.Windows.Forms.KeyPressEventHandler(lvCurrentPath_KeyPressed);
            lvChildren.KeyPress += new System.Windows.Forms.KeyPressEventHandler(lvChildren_KeyPressed);

        }
        public void reload()
        {
            reloadPath(textboxPath.Text, true, null);
        }

        public void LVCurrentPathAddItem(RegisterItem? registryItem, int index, String name)
        {
            if (registryItem == null) return;
            ListViewItem item = new ListViewItem(index.ToString());
            item.Tag = registryItem.Id;
            item.SubItems.Add(name);
            lvCurrentPath.Items.Add(item);
        }
        public void LVChildrenAddItem(ListView lv, RegisterItem? registryItem, int index, String name)
        {
            if (registryItem == null) return;
            ListViewItem item = new ListViewItem(index.ToString());
            item.Tag = registryItem.Id;
            item.SubItems.Add(name);
            lv.Items.Add(item);
        }

        public bool reloadPath(String path, bool usePath, RegisterItem? item)
        {
            // phase 1 - firstly get the registry node item
            List<RegisterItem>? registry = null;

            if (usePath)
            {
                if (!Register.LoadFullPath(ctx, path, ref registry, true))
                    return false; // critical error abort
            }
            else
            {
                if (!Register.LoadFullPath(ctx, item, ref registry, true, false, false, false, false))
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

            // load registry item's node config from entry file
            String rtf = "";
            byte[]? xamlbytesOut = null;
            currentItem.loadNode(ctx, ref rtf, ref xamlbytesOut, false);
            currentItem.children = new ChildrenRegister();
            ChildrenRegister.Initialize(ctx, currentItem.children, currentItem);
            RegisterItem? child = currentItem.children.Next();
            while (child != null)
            {
                if (child.domainType != DomainType.HiddenCore && child.nodeType != NodeType.EmptySlot)
                {
                    child.loadNode(ctx, ref rtf, ref xamlbytesOut, false);
                    String label = entryMethods.getEntryLabel(child.node, false);
                    String formatted = $"{label} ({child.childrenCount} children)";
                    LVCurrentPathAddItem(child, dirIndex++, formatted);
                }
                child = currentItem.children.Next();
            }
            lvCurrentPath.EndUpdate();

            // finally configure everything
            selectedPathItem = currentItem;
            txtEntryTitle.Text = currentItem.node.chapter.Title;
            List<RegisterItem>? ancestors = null;
            Register.Lineage(ctx, currentItem, ref ancestors, true);//, true);
            path = Register.LineageFullPath(ancestors);//, currentItem);//, true);
            textboxPath.Text = path;

            //this.Enabled = true;
            //formOperation.close();
            return true;
        }

        public bool reloadChildren(UInt32 id, ListView lv)
        {
            // phase 1 - add all subdirectories which exist in currently selected/doubleclicked path

            // phase 1 - firstly get the registry node item
            List<RegisterItem>? registry = null;
            if (!Register.LoadFullPath(ctx, id, ref registry, true))
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
                    child.loadNode(ctx, ref rtf, ref xamlbytesOut, false);
                    String label = entryMethods.getEntryLabel(child.node, false);
                    String formatted = $"{label} ({child.childrenCount} children)";
                    LVChildrenAddItem(lv, child, dirIndex++, formatted);
                }
                child = currentItem.children.Next();
            }
            lv.EndUpdate();
            //this.Enabled = true;
            //formOperation.close();
            return true;
        }

        private void lvCurrentPath_DoubleClick(object sender, EventArgs e)
        {
            if (lvCurrentPath.SelectedItems.Count == 0) return;
            ListViewItem selectedItem = lvCurrentPath.SelectedItems[0];
            UInt32 id = (UInt32)selectedItem.Tag;
            reloadPath(id.ToString(), true, null);
        }

        private void lvCurrentPath_Click(object sender, EventArgs e)
        {
            if (lvCurrentPath.SelectedItems.Count == 0) return;
            ListViewItem selectedItem = lvCurrentPath.SelectedItems[0];
            UInt32 id = (UInt32)selectedItem.Tag;
            reloadChildren(id, lvChildren);
        }
        private void lvCurrentPath_KeyPressed(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                // Enter key pressed
                if (lvCurrentPath.SelectedItems.Count == 0) return;
                ListViewItem selectedItem = lvCurrentPath.SelectedItems[0];
                UInt32 id = (UInt32)selectedItem.Tag;
                reloadChildren(id, lvChildren);
            }
        }

        private void lvChildren_Click(object sender, EventArgs e)
        {
            if (lvChildren.SelectedItems.Count == 0) return;
            ListViewItem selectedItem = lvChildren.SelectedItems[0];
            UInt32 id = (UInt32)selectedItem.Tag;
            reloadChildren(id, lvChildsChildren);
        }

        private void lvChildren_DoubleClick(object sender, EventArgs e)
        {
            if (lvChildren.SelectedItems.Count == 0) return;
            ListViewItem selectedItem = lvChildren.SelectedItems[0];
            UInt32 id = (UInt32)selectedItem.Tag;
            reloadPath(id.ToString(), true, null);
        }
        private void lvChildren_KeyPressed(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                // Enter key pressed
                if (lvChildren.SelectedItems.Count == 0) return;
                ListViewItem selectedItem = lvChildren.SelectedItems[0];
                UInt32 id = (UInt32)selectedItem.Tag;
                reloadChildren(id, lvChildsChildren);
            }
        }

        private void lvChildsChildren_DoubleClick(object sender, EventArgs e)
        {
            if (lvChildsChildren.SelectedItems.Count == 0) return;
            ListViewItem selectedItem = lvChildsChildren.SelectedItems[0];
            UInt32 id = (UInt32)selectedItem.Tag;
            reloadPath(id.ToString(), true, null);
        }

        private void buttonReload_Click(object sender, EventArgs e)
        {
            reload();
        }

        private void lvCurrentPath_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lvChildren_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

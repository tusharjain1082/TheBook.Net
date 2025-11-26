using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using DiaryJournal.Net;
using RtfPipe;
using RtfPipe.Tokens;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace TheBook.Net.Core
{
    public static class entryMethodsNewDesign
    {
        public static bool DBCreateLoadYearSystemNode(OpenFSDBContext? ctx, List<RegisterItem> registry, RegisterItem emptySlots, DateTime year, ref RegisterItem? itemOut)
        {
            if (registry.Count == 0) return false; // no system node present so return error

            // assuming all system nodes configs were loaded from files
            String rtf = "";
            RegisterItem? journalNode = findRegistrySystemNodeItemByType(registry, NodeType.Journal);
            journalNode = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, journalNode.Id, true, false, false, false, true, true);
            if (journalNode == null) return false; // journal system node not existent abort with error

            // phase 1: get registry for this parent node
            List<RegisterItem> journalNodeRegistry = null;
            if (!journalNode.children.GetChildren(ref journalNodeRegistry, true))
                return false; // critical error abort with error

            // phase 2: find the required node, if nonexistent create it, then return it
            RegisterItem? item = findRegistryYearSystemNodeItem(journalNodeRegistry, year);
            if (item != null)
            {
                // registry item found, return it
                itemOut = item;
                return true;
            }
            else
            {
                if (ctx.readOnly) return false;

                // registry item nonexistent, create and return it
                myNode? node = new myNode(true);
                DateTime datetime = new DateTime(year.Year, 1, 1, 0, 0, 0, 0);
                String label = datetime.ToString("yyyy");
                node.chapter.nodeType = NodeType.Year;
                node.chapter.specialNodeType = SpecialNodeType.SystemNode;
                node.chapter.parentId = journalNode.node.chapter.Id;
                node.chapter.chapterDateTime = datetime;
                node.chapter.creationDateTime = DateTime.Now;
                node.chapter.modificationDateTime = DateTime.Now;
                node.chapter.Title = label;

                // finally insert and configure empty slots register also
                byte[]? xamlbytes = null;
                item = Register.Insert(ctx, ctx.dbNodeTreeRegistryFile, journalNode.Id, emptySlots, node, "", xamlbytes);
                // done
            }
            // done
            itemOut = item;
            return true;
        }
        public static bool DBCreateLoadMonthSystemNode(OpenFSDBContext? ctx, RegisterItem emptySlots, RegisterItem? yearItem, DateTime month, ref RegisterItem? itemOut)
        {
            // first get latest state of the parent node
            yearItem = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, yearItem.Id, false, false, false, false, true, true);
            if (yearItem == null)
                return false; // critical error

            String rtf = "";
            byte[]? xamlbytesOut = null;
            if (yearItem.node == null) yearItem.loadNode(ctx, ref rtf, ref xamlbytesOut, false);

            // phase 1: get registry for this parent node
            List<RegisterItem> parentNodeRegistry = null;
            if (!yearItem.children.GetChildren(ref parentNodeRegistry, true))
                return false; // critical error abort with error

            // phase 2: find the required node, if nonexistent create it, then return it
            RegisterItem? item = findRegistryMonthSystemNodeItem(parentNodeRegistry, month);
            if (item != null)
            {
                // registry item found, return it
                itemOut = item;
                return true;
            }
            else
            {
                if (ctx.readOnly) return false;

                // registry item nonexistent, create and return it
                myNode? node = new myNode(true);
                DateTime datetime = new DateTime(yearItem.node.chapter.chapterDateTime.Year, month.Month, 1, 0, 0, 0, 0);
                String label = datetime.ToString("MMMM");
                node.chapter.nodeType = NodeType.Month;
                node.chapter.specialNodeType = SpecialNodeType.SystemNode;
                node.chapter.parentId = yearItem.node.chapter.Id;
                node.chapter.chapterDateTime = datetime;
                node.chapter.creationDateTime = DateTime.Now;
                node.chapter.modificationDateTime = DateTime.Now;
                node.chapter.Title = label;

                // finally insert and configure empty slots register also
                item = Register.Insert(ctx, ctx.dbNodeTreeRegistryFile, yearItem.Id, emptySlots, node, "", xamlbytesOut);
                // done
            }
            // done
            itemOut = item;
            return true;
        }
        // finds a system node by it's name/type from the source registry
        public static RegisterItem? findRegistryEmptySlotSystemNodeItem(List<RegisterItem>? registry)
        {
            foreach (RegisterItem? item in registry)
            {
                if (item.node == null) continue;

                if (item.node.chapter.specialNodeType != SpecialNodeType.SystemNode)
                    continue; // not system node so skip

                if (item.node.chapter.nodeType != NodeType.EmptySlot)
                    continue; // not the wanted node name so skip

                // this is the system node which is being wanted, so return it
                return item;
            }
            return null;
        }
        // finds a system node by it's name/type from the source registry
        public static RegisterItem? findRegistryMonthSystemNodeItem(List<RegisterItem>? registry, DateTime month)
        {
            foreach (RegisterItem? item in registry)
            {
                if (item.node == null) continue;

                if (item.node.chapter.specialNodeType != SpecialNodeType.SystemNode)
                    continue; // not system node so skip

                //String name = CoreFramework.convertToString(item.node.chapter.nodeType);

                if (item.node.chapter.nodeType != NodeType.Month)//(systemNodeName != name)
                    continue; // not the wanted node name so skip

                // check to find if this is the right node
                if (item.node.chapter.chapterDateTime.Month != month.Month)
                    continue; // this is not demanded year, so skip

                // this is the system node which is being wanted, so return it
                return item;
            }
            return null;
        }
        // finds a system node by it's name/type from the source registry
        public static RegisterItem? findRegistryYearSystemNodeItem(List<RegisterItem>? registry, DateTime year)
        {
            foreach (RegisterItem? item in registry)
            {
                if (item.node == null) continue;

                if (item.node.chapter.specialNodeType != SpecialNodeType.SystemNode)
                    continue; // not system node so skip

                //String name = CoreFramework.convertToString(item.node.chapter.nodeType);

                if (item.node.chapter.nodeType != NodeType.Year)//(systemNodeName != name)
                    continue; // not the wanted node name so skip

                // check to find if this is the right node
                if (item.node.chapter.chapterDateTime.Year != year.Year)
                    continue; // this is not demanded year, so skip

                // this is the system node which is being wanted, so return it
                return item;
            }
            return null;
        }
        // finds a system node by it's name/type from the source registry
        public static RegisterItem? findRegistrySystemNodeItemByType(List<RegisterItem>? registry, NodeType type)
        {
            foreach (RegisterItem? item in registry)
            {
                if (item.node == null) continue;

                if (item.node.chapter.specialNodeType != SpecialNodeType.SystemNode)
                    continue; // not system node so skip

                //String name = CoreFramework.convertToString(item.node.chapter.nodeType);

                if (item.node.chapter.nodeType != type)//(systemNodeName != name)
                    continue; // not the wanted node name so skip

                // this is the system node which is being wanted, so return it
                return item;
            }
            return null;
        }
        public static bool DBAutoCreateLoadSystemNode(OpenFSDBContext? ctx, List<myNode> nodes, ref RegisterItem? emptySlots,
            NodeType nodeType, ref RegisterItem? itemOut)
        {
            // phase 1 - find the node

            // first get parent node
            //RegisterItem? parent = Register.LoadSetupRegisterItem(ctx, ctx.dbCtx.dbNodeTreeRegistryFile, 0, false, false, false, false, true, true);
            //if (parent == null) return false; // critical error loading parent so abort with error

            String name = CoreFramework.convertToString(nodeType);

            // skip this system node if it exists in db
            myNode? node = entryMethods.findSystemNodeByName(nodes, name);
            if (node != null)
            {
                // already exists so get it from register and return it directly
                itemOut = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, node.chapter.Id, false, false, false, false, true, true);
                return true; // system node already exists, so skip
            }

            // node does not exists, so create it and return it
            // phase 2 create registry entry and the system node because it does not exists

            // this node does not exists in db, so auto create it
            myNode newNode = new myNode(true);
            newNode.chapter.specialNodeType = SpecialNodeType.SystemNode;
            newNode.chapter.nodeType = nodeType;
            newNode.chapter.chapterDateTime = DateTime.Now;
            newNode.chapter.Title = name;

            // finally update the register add this node
            if (newNode.chapter.nodeType == NodeType.EmptySlot)
                newNode.chapter.domainType = DomainType.HiddenCore;

            // create directly into Register 
            byte[]? xamlbytes = null;
            RegisterItem? item = Register.Insert(ctx, ctx.dbNodeTreeRegistryFile, 0, emptySlots, newNode, "", xamlbytes);
            if (item == null) return false; // critical error return error

            itemOut = item;
            return true;
        }
        // this method creates any missing root system node
        public static bool DBCreateDBCore(OpenFSDBContext? ctx, ref RegisterItem? emptySlots, bool rebuildEmptySlotsRegister)
        {
            // phase 1: - find emptyslots system node

            RegisterItem? root = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, 0, true, false, false, false, true, true);
            if (root == null) return false;

            List<RegisterItem>? registry = null;
            Register.FindRegisterItems(ctx, ctx.dbNodeTreeRegistryFile, 0, NodeType.EmptySlot, true,
                SpecialNodeType.SystemNode, true, DomainType.AnyOrAll, false, ref registry, true);

            if (registry.Count == 0)
            {
                // empty slots system node does not exists so first create it.

                if (ctx.readOnly)
                {
                    MessageBox.Show("warning: cannot create system nodes because database is write locked.", "warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // this node does not exists in db, so auto create it
                myNode newNode = new myNode(true);
                newNode.chapter.Id = 1;
                newNode.chapter.specialNodeType = SpecialNodeType.SystemNode;
                newNode.chapter.nodeType = NodeType.EmptySlot;
                newNode.chapter.chapterDateTime = DateTime.Now;
                newNode.chapter.creationDateTime = DateTime.Now;
                newNode.chapter.modificationDateTime = DateTime.Now;
                newNode.chapter.Title = NodeType.EmptySlot.convertToString();
                newNode.chapter.domainType = DomainType.HiddenCore;

                // create new empty slots system node in root
                byte[]? xamlbytes = null;
                if (!OpenFileSystemDB.createNode(ctx, newNode, "", xamlbytes, true, true, true, false))
                    return false; // critical error

                // create directly into Register 
                emptySlots = root.children.Add(newNode, ref root);
                if (emptySlots == null) return false;
            }
            else
            {
                // empty slots system node exists so use it
                emptySlots = registry.FirstOrDefault();
            }

            // reload empty slots system node
            emptySlots = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, emptySlots.Id, true, false, false, false, true, true);

            // we build a new empty slots register
            if (!ctx.readOnly)
            {
                if (rebuildEmptySlotsRegister)
                    Register.BuildEmptySlotsRegister(ctx, ctx.dbNodeTreeRegistryFile, emptySlots);
            }

            // reload empty slots system node
            emptySlots = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, emptySlots.Id, true, false, false, false, true, true);

            // phase 2: - find all existing system nodes

            registry = null;
            if (!Register.FindRegisterItems(ctx, ctx.dbNodeTreeRegistryFile, 0,
                NodeType.AnyOrAll, false, SpecialNodeType.SystemNode, true, DomainType.AnyOrAll,
                false, ref registry, true))
                return false; // critical error abort with error

            List<myNode> rootNodes = registry.Select(c => c.node).ToList();

            // phase 3: - create all system nodes which do not exist

            foreach (String systemNodeName in mySystemNodes.SystemNodesNames)
            {
                // phase 1 find system node by name

                NodeType nodeType = CoreFramework.convertToEnum<NodeType>(systemNodeName);

                // skip this system node if it exists in db
                myNode? found = entryMethods.findSystemNodeByName(rootNodes, systemNodeName);
                if (found != null) continue; // system node already exists, so skip
                if (nodeType == NodeType.EmptySlot) continue; // cannot recreate empty slots system node so skip

                // phase 2 create registry entry and the system node because it does not exists

                // this node does not exists in db, so auto create it
                myNode newNode = new myNode(true);
                newNode.chapter.specialNodeType = SpecialNodeType.SystemNode;
                newNode.chapter.nodeType = nodeType;
                newNode.chapter.chapterDateTime = DateTime.Now;
                newNode.chapter.creationDateTime = DateTime.Now;
                newNode.chapter.modificationDateTime = DateTime.Now;
                newNode.chapter.Title = systemNodeName;

                if (ctx.readOnly)
                {
                    MessageBox.Show("warning: cannot create system nodes because database is write locked.", "warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // phase 3 insert this new system node directly into Register 
                RegisterItem? item = Register.Insert(ctx, ctx.dbNodeTreeRegistryFile, 0, emptySlots, newNode, "", Array.Empty<byte>());
                if (item == null) return false; // critical error while creating new system node abort with error
            }
            return true;
        }
        // load node from registry item
        public static bool DBFindLoadNode(OpenFSDBContext? ctx,
            RegisterItem item, ref String rtfOut, bool loadData, ref myNode? nodeOut, ref byte[]? xamlbytesOut)
        {
            String rtf = "";
            myNode? node = OpenFileSystemDB.findLoadNode(ctx, item.DirectorySectionId, item.Id, ref rtf, ref xamlbytesOut, loadData);
            if (node == null) return false;

            // node found and loaded, return it
            rtfOut = rtf;
            nodeOut = node;
            return true;
        }
        // finds all system nodes from the list and makes new lists
        public static bool DBFindSystemNodes(myConfig cfg, List<myNode> nodes,
            List<RegisterItem> registry,
            ref List<myNode> listNodesOut,
            ref List<RegisterItem> registryOut)
        {
            listNodesOut = new List<myNode>();
            registryOut = new List<RegisterItem>();

            foreach (myNode node in nodes)
            {
                if (node.chapter.specialNodeType != SpecialNodeType.SystemNode)
                    continue; // not system node so skip

                RegisterItem? registryItem = registry.Find(x => x.Id == node.chapter.Id);
                if (registryItem == null) return false; // critical error node was not found in registry abort with error

                listNodesOut.Add(node);
                registryOut.Add(registryItem);
            }
            return true;
        }

        // this method is the 2nd step in which the method loads the root system nodes
        public static bool DBLoadRootSystemNodes(OpenFSDBContext? ctx, ref List<RegisterItem>? registryOut, ref RegisterItem? emptySlotsItemOut)
        {

            // phase 1: - find system nodes
            List<RegisterItem> registry = null;
            Register.FindRegisterItems(ctx, ctx.dbNodeTreeRegistryFile, 0, NodeType.AnyOrAll, false,
                SpecialNodeType.SystemNode, true, DomainType.AnyOrAll, false, ref registry, true);

            // phase 2: check if any system node is still missing, abort with error if so

            if (registry.Count < mySystemNodes.SystemNodesNames.Count)
                return false; // critical error abort with error

            // now intitialize all items
            foreach (RegisterItem? item in registry)
                Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, item, true, false, false, false, true, true);

            // get empty slots system node item
            emptySlotsItemOut = entryMethodsNewDesign.findRegistrySystemNodeItemByType(registry, NodeType.EmptySlot);

            // success, return the root nodes list
            registryOut = registry;
            return true;
        }

        // this method generates register file
        public static bool DBBuildOFSDBRegister(OpenFSDBContext? ctx, FormOperation? formop = null)
        {

            // get total number of entries
            Int64 total = OpenFSDBSections.getTotalEntriesAllSections(ctx);
            Int64 index = 0;
            Int64 totalValid = 0;

            // phase 1 - write all node configurations into register by their ids

            // iterate through all sections
            foreach (OpenFSDBSection section in ctx.dbSections.sections)
            {
                List<myNode> list = new List<myNode>();
                if (!OpenFileSystemDB.findSectionNodes(ctx, section.sectionId, ref list)) continue;
                // iterate and process all entries in this section
                foreach (myNode? node in list)
                {
                    // build register item
                    RegisterItem item = RegisterItem.convertFromMyNode(node);
                    // write register item into register by item id
                    if (!Register.UpdateNode(ctx, ctx.dbNodeTreeRegistryFile, item, 0, false, 0, false, 0, false))
                        return false; // critical error, we cannot proceed further, abort with error

                }
                index += list.Count;
                totalValid += list.Count;
                if (formop != null)
                {
                    formop.updateProgressBar(index, total);
                    formop.updateFilesStatus(index, total);
                }
            }

            // phase 2 - configure descendant lineage tree configuration in all applicable nodes

            // iterate through all sections
            index = 0;
            foreach (OpenFSDBSection section in ctx.dbSections.sections)
            {
                List<myNode> list = new List<myNode>();
                if (!OpenFileSystemDB.findSectionNodes(ctx, section.sectionId, ref list)) continue;
                // iterate and process all entries in this section
                foreach (myNode? node in list)
                {
                    if (node.chapter.Id == 0) continue; // root is skipped

                    // this is not root but parent and or child node, so load both current item and it's parent and insert the item into their registers.
                    RegisterItem? item = null;
                    Register.FindNode(ctx, ctx.dbNodeTreeRegistryFile, node.chapter.Id, ref item);
                    if (item == null) return false; // critical error abort operations

                    // get top root ancestor of this node
                    //List<RegisterItem>? lineage = null;
                    //Register.Lineage(ctx, ctx.dbCtx.dbNodeTreeRegistryFile, item, ref lineage, true, false, false);
                    //RegisterItem? rootAncestor = lineage.FirstOrDefault();
                    RegisterItem? rootAncestor = null;
                    //if (rootAncestor != null)
                    if (Register.LineageRootAncestor(ctx, ctx.dbNodeTreeRegistryFile, item, ref rootAncestor, false, false))
                    {
                        // root ancestor found, this means this node is local tree child node and has a valid root ancestor, so configure root ancestor
                        // configure root Ancestor
                        item.treeRootId = rootAncestor.Id;
                        // write register item into register by item id
                        if (!Register.UpdateNode(ctx, ctx.dbNodeTreeRegistryFile, item, 0, false, 0, false, 0, false))
                            return false; // critical error, we cannot proceed further, abort with error
                    }
                }
                index += list.Count;
                //totalValid += list.Count;
                if (formop != null)
                {
                    formop.updateProgressBar(index, total);
                    formop.updateFilesStatus(index, total);
                }
            }

            // phase 3 - build both registers children register and lineage tree register

            // iterate all valid slots by their valid nodes files which exist in sections of the db
            // iterate through all sections
            index = 0;
            foreach (OpenFSDBSection section in ctx.dbSections.sections)
            {
                List<myNode> list = new List<myNode>();
                if (!OpenFileSystemDB.findSectionNodes(ctx, section.sectionId, ref list)) continue;
                // iterate and process all entries in this section
                foreach (myNode? node in list)
                {
                    if (node.chapter.Id == 0) continue; // root is skipped

                    // this is not root but parent and or child node, so load both current item and it's parent and insert the item into their registers.
                    RegisterItem? item = null;
                    Register.FindNode(ctx, ctx.dbNodeTreeRegistryFile, node.chapter.Id, ref item);
                    if (item == null) return false; // critical error abort operations

                    RegisterItem? parent = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, item.parentId, false, false, false, false, true, true);
                    if (parent == null) return false; // critical error abort operations

                    // now add the item into parent's both registers

                    // add to children's register
                    if (!parent.children.Add(item, parent)) return false; // critical error abort operations

                    // reload latest states
                    Register.FindNode(ctx, ctx.dbNodeTreeRegistryFile, node.chapter.Id, ref item);
                    if (item == null) return false; // critical error abort operations
                    parent = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, item.parentId, false, false, false, false, true, true);
                    if (parent == null) return false; // critical error abort operations

                    // add to root ancestor's tree lineage register
                    if (parent.Id != 0)
                    {
                        // if parent is not root then it's a root ancestor or a local tree child node, so in both cases we autoadd to root ancestor only.
                        // we cannot add to root itself there is no lineage tree register in root 0.
                        if (!parent.tree.Add(item))
                            return false; // critical error abort operations
                    }
                }

                // update                
                index += list.Count;
                if (formop != null)
                {
                    formop.updateProgressBar(index, totalValid);
                    formop.updateFilesStatus(index, totalValid);
                }
            }
            return true;
        }

    }
}

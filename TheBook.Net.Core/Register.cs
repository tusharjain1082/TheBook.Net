using DiaryJournal.Net;
using Microsoft.Win32;
using RtfPipe;
using RtfPipe.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace TheBook.Net.Core
{
    /* [CAUTION]
     * 05:49 PM, 03 January 2025: i discovered that = 
     * integral synchronization is mandatory in every step. we change lib and we change item, in both changes in every step
     * intregral synchronization by reloading the latest state register item from registers are mandatory. if even a single 
     * integral synchronization is missing then integral corruption will happen and the register and db will be integrally corrupted.
     * when we add item to lib, when we remove item from lib, both item and lib are integrally changed, 
     * so we are required to reload both of them their latest states from register.
    // step 1 =
    lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
    item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
    lib.children.Remove(item);
    // step 2 =
    item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
    lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
    item.tree.MoveDescendants(item, false, lib.tree.rootAncestor);
    // step 3 =
    item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
    lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
    lib.children.Add(item, lib);
    // step 4 =
    lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
    item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
    lib.tree.Add(item);
    // step 5 =
    item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
    lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
    List<RegisterItem> libtree = null;
    lib.tree.GetWholeTree(ref libtree, false);
    item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
    libchildren = null;
    lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
    lib.children.GetChildren(ref libchildren, false);
    List<RegisterItem> itemtree = null;
    item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
    item.tree.GetWholeTree(ref itemtree, false);
    List<RegisterItem> itemchildren = null;
    item.children.GetChildren(ref itemchildren, false);

    // step 1 =
    lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
    item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
    lib.children.Remove(item);
    // step 2 =
    //lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true);
    //item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false);
    //lib.tree.Remove(item);
    // step 3 =
    lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
    item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
    BuildCustomLineageStructure(ctx, file, item);
    // we cannot move descendants to root because we cannot put tree in there.
    //item.tree.MoveDescendants(item, false, root.tree.rootAncestor);
    // step 4 =
    item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
    root = LoadSetupRegisterItem(ctx, file, root.Id, false, false, false, false, true, true);
    root.children.Add(item, root);
    // step 5 =
    item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
    root = LoadSetupRegisterItem(ctx, file, root.Id, false, false, false, false, true, true);
    List<RegisterItem> rootchildren = null;
    root.children.GetChildren(ref rootchildren, false);
    item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
    item.tree.GetWholeTree(ref itemtree, false);
    // step 5 =
    item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
    root = LoadSetupRegisterItem(ctx, file, root.Id, false, false, false, false, true, true);
    root.children.Remove(item);
    // step 5 =
    lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
    item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
    root = LoadSetupRegisterItem(ctx, file, root.Id, false, false, false, false, true, true);
    item.tree.MoveDescendants(item, true, lib.tree.rootAncestor);
    // step 5 =
    lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
    item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
    lib.children.Add(item, lib);
    // step 5 =
    item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
    lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
    lib.tree.GetWholeTree(ref libtree, false);
    item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
    lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
    lib.children.GetChildren(ref libchildren, false);
    return;
     */

    public class RegisterItem
    {
        /* map = 
         * first 8 bytes which is Int64 is reserved for flags or some function. every byte is flag enum.
         * */

        public Int64 position = 0;

        // 36 bytes binary block is made of all these elements =
        public const int blockSize = ((sizeof(Int64) * 14) + 4);
        public Int64 Id = 0;
        public Int64 parentId = 0;
        public Int64 DirectorySectionId = 0;
        public Int64 firstChildId = 0;
        public Int64 lastChildId = 0;
        public Int64 nextSiblingId = 0;
        public Int64 previousSiblingId = 0;
        public Int64 childrenCount = 0;
        public Int64 treeRootId = 0;
        public Int64 treeHeadId = 0;
        public Int64 treeTailId = 0;
        public Int64 nextDescendantId = 0;
        public Int64 previousDescendantId = 0;
        public Int64 descendantsCount = 0;
        public NodeType nodeType = NodeType.Entry;
        public SpecialNodeType specialNodeType = SpecialNodeType.None;
        public DomainType domainType = DomainType.Journal;
        public RegisterItemFlags1 flags1 = RegisterItemFlags1.None;


        public myNode? node = null;
        public ChildrenRegister? children = null;
        public String rtf = "";
        public byte[]? xamlbytes = Array.Empty<byte>();
        public List<RegisterItem>? childrenList = null;

        public LineageRegister? tree = null;

        public RegisterItem()
        {

        }
        public RegisterItem(Int64 position, Int64 id, Int64 parentid, Int64 sectionid, Int64 childrenCount,
            Int64 firstChildId, Int64 lastChildId, Int64 nextSiblingId, Int64 previousSiblingId, NodeType nodeType,
            SpecialNodeType specialNodeType, DomainType domainType, RegisterItemFlags1 flags1,
            Int64 treeRootId, Int64 treeHeadId, Int64 treeTailId,
            Int64 nextDescendantId, Int64 prevDescendantId, Int64 descendantsCount)
        {
            this.position = position;
            this.Id = id;
            this.parentId = parentid;
            this.DirectorySectionId = sectionid;
            this.firstChildId = firstChildId;
            this.lastChildId = lastChildId;
            this.nextSiblingId = nextSiblingId;
            this.previousSiblingId = previousSiblingId;
            this.childrenCount = childrenCount;
            this.treeRootId = treeRootId;
            this.treeHeadId = treeHeadId;
            this.treeTailId = treeTailId;
            this.nextDescendantId = nextDescendantId;
            this.previousDescendantId = prevDescendantId;
            this.descendantsCount = descendantsCount;
            this.nodeType = nodeType;
            this.specialNodeType = specialNodeType;
            this.domainType = domainType;
            this.flags1 = flags1;
        }

        public void CopyFrom(RegisterItem? item)
        {
            this.position = item.position;
            this.Id = item.Id;
            this.parentId = item.parentId;
            this.DirectorySectionId = item.DirectorySectionId;
            this.firstChildId = item.firstChildId;
            this.lastChildId = item.lastChildId;
            this.nextSiblingId = item.nextSiblingId;
            this.previousSiblingId = item.previousSiblingId;
            this.childrenCount = item.childrenCount;
            this.treeRootId = item.treeRootId;
            this.treeHeadId = item.treeHeadId;
            this.treeTailId = item.treeTailId;
            this.nextDescendantId = item.nextDescendantId;
            this.previousDescendantId = item.previousDescendantId;
            this.descendantsCount = item.descendantsCount;
            this.nodeType = item.nodeType;
            this.specialNodeType = item.specialNodeType;
            this.domainType = item.domainType;
            this.flags1 = item.flags1;
        }
        public void CopyFrom(myNode node, bool copyId, bool copyParentId, bool copySectionId)
        {
            if (copyId) this.Id = node.chapter.Id;
            if (copyParentId) this.parentId = node.chapter.parentId;
            if (copySectionId) this.DirectorySectionId = node.DirectorySectionID;
            this.nodeType = node.chapter.nodeType;
            this.specialNodeType = node.chapter.specialNodeType;
            this.domainType = node.chapter.domainType;
        }
        #region "framework"

        // load the node file and it's configuration into this collection's myNode object
        public bool loadNode(OpenFSDBContext? ctx, ref String? rtfOut, ref byte[]? xamlbytesOut, bool loadData)
        {

            myNode? node = null;
            String? rtf = "";
            if (!entryMethodsNewDesign.DBFindLoadNode(ctx, this, ref rtf, loadData, ref node, ref xamlbytesOut))
                return false; // critical error abort with error

            if (node == null) return false;

            rtfOut = rtf;
            this.node = node;

            return true;
        }

        public static RegisterItem? convertFromMyNode(myNode? node)
        {
            if (node == null) return null;
            if (node.chapter == null) return null;
            RegisterItem item = new RegisterItem();
            item.Id = node.chapter.Id;
            item.parentId = node.chapter.parentId;
            item.DirectorySectionId = node.DirectorySectionID;
            item.nodeType = node.chapter.nodeType;
            item.specialNodeType = node.chapter.specialNodeType;
            item.domainType = node.chapter.domainType;
            return item;
        }
        public static byte[]? convertToBytesFromMyNode(myNode? node)
        {
            if (node == null) return null;
            if (node.chapter == null) return null;
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write((Int64)0);//node.chapter.Id);
            bw.Write(node.chapter.parentId);
            bw.Write(node.DirectorySectionID);
            bw.Write((Int64)0);
            bw.Write((Int64)0);
            bw.Write((Int64)0);
            bw.Write((Int64)0);
            bw.Write((Int64)0);
            bw.Write((Int64)0);
            bw.Write((Int64)0);
            bw.Write((Int64)0);
            bw.Write((Int64)0);
            bw.Write((Int64)0);
            bw.Write((Int64)0);
            bw.Write((byte)node.chapter.nodeType);
            bw.Write((byte)node.chapter.specialNodeType);
            bw.Write((byte)node.chapter.domainType);
            bw.Write((byte)0);
            return ms.ToArray();
        }
        public static byte[] convertToBytes(Int64 id, Int64 parentId, Int64 DirectorySectionId, Int64 childrenCount,
            Int64 firstChildId, Int64 lastChildId, Int64 nextSiblingId, Int64 previousSiblingId, NodeType nodeType,
            SpecialNodeType specialNodeType, DomainType domainType, RegisterItemFlags1 flags1,
            Int64 treeRootId, Int64 treeHeadId, Int64 treeTailId,
            Int64 nextDescendantId, Int64 prevDescendantId, Int64 descendantsCount)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write((Int64)0);// id);
            bw.Write(parentId);
            bw.Write(DirectorySectionId);
            bw.Write(firstChildId);
            bw.Write(lastChildId);
            bw.Write(nextSiblingId);
            bw.Write(previousSiblingId);
            bw.Write(childrenCount);
            bw.Write(treeRootId);
            bw.Write(treeHeadId);
            bw.Write(treeTailId);
            bw.Write(nextDescendantId);
            bw.Write(prevDescendantId);
            bw.Write(descendantsCount);
            bw.Write((byte)nodeType);
            bw.Write((byte)specialNodeType);
            bw.Write((byte)domainType);
            bw.Write((byte)flags1);
            return ms.ToArray();
        }
        public static byte[] convertToBytes(RegisterItem item)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write((Int64)0);// item.Id);
            bw.Write(item.parentId);
            bw.Write(item.DirectorySectionId);
            bw.Write(item.firstChildId);
            bw.Write(item.lastChildId);
            bw.Write(item.nextSiblingId);
            bw.Write(item.previousSiblingId);
            bw.Write(item.childrenCount);
            bw.Write(item.treeRootId);
            bw.Write(item.treeHeadId);
            bw.Write(item.treeTailId);
            bw.Write(item.nextDescendantId);
            bw.Write(item.previousDescendantId);
            bw.Write(item.descendantsCount);
            bw.Write((byte)item.nodeType);
            bw.Write((byte)item.specialNodeType);
            bw.Write((byte)item.domainType);
            bw.Write((byte)item.flags1);
            return ms.ToArray();
        }
        public static RegisterItem? convertFromBytesStream(Stream s)
        {
            BinaryReader br = new BinaryReader(s);
            //try
            //{
            //if (br.PeekChar() == -1) return null; // end of stream
            //   br.ReadByte();
            //  s.Position--;
            // }
            //catch
            // {
            //     return null;
            // }
            if (s.Position >= s.Length) return null;
            RegisterItem item = new RegisterItem();
            item.position = s.Position;
            //item.Id = br.ReadInt64();
            br.ReadInt64();
            item.parentId = br.ReadInt64();
            item.DirectorySectionId = br.ReadInt64();
            item.firstChildId = br.ReadInt64();
            item.lastChildId = br.ReadInt64();
            item.nextSiblingId = br.ReadInt64();
            item.previousSiblingId = br.ReadInt64();
            item.childrenCount = br.ReadInt64();
            item.treeRootId = br.ReadInt64();
            item.treeHeadId = br.ReadInt64();
            item.treeTailId = br.ReadInt64();
            item.nextDescendantId = br.ReadInt64();
            item.previousDescendantId = br.ReadInt64();
            item.descendantsCount = br.ReadInt64();
            item.nodeType = (NodeType)br.ReadByte();
            item.specialNodeType = (SpecialNodeType)br.ReadByte();
            item.domainType = (DomainType)br.ReadByte();
            item.flags1 = (RegisterItemFlags1)br.ReadByte();
            return item; // valid node is found
        }
        public static RegisterItem? Root(Stream s)
        {
            Int64 pos = s.Position;
            s.Position = 0;
            RegisterItem? item = convertFromBytesStream(s);
            if (item == null) return null;
            s.Position = pos;
            return item; // valid node is found
        }


        public static RegisterItem? convertFromBytes(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            BinaryReader br = new BinaryReader(ms);
            RegisterItem item = new RegisterItem();
            //item.Id = br.ReadInt64();
            br.ReadInt64();
            item.parentId = br.ReadInt64();
            item.DirectorySectionId = br.ReadInt64();
            item.firstChildId = br.ReadInt64();
            item.lastChildId = br.ReadInt64();
            item.nextSiblingId = br.ReadInt64();
            item.previousSiblingId = br.ReadInt64();
            item.childrenCount = br.ReadInt64();
            item.treeRootId = br.ReadInt64();
            item.treeHeadId = br.ReadInt64();
            item.treeTailId = br.ReadInt64();
            item.nextDescendantId = br.ReadInt64();
            item.previousDescendantId = br.ReadInt64();
            item.descendantsCount = br.ReadInt64();
            item.nodeType = (NodeType)br.ReadByte();
            item.specialNodeType = (SpecialNodeType)br.ReadByte();
            item.domainType = (DomainType)br.ReadByte();
            item.flags1 = (RegisterItemFlags1)br.ReadByte();
            return item;
        }
        public bool InitializeSystem(OpenFSDBContext? ctx, String registerFile, bool loadChildrenRegister, bool loadLineageRegister)
        {
            if (loadChildrenRegister)
            {
                // initialize children register
                this.children = new ChildrenRegister();
                ChildrenRegister.Initialize(ctx, registerFile, this.children, this);
            }

            if (loadLineageRegister)
            {
                // initialize lineage register
                this.tree = new LineageRegister();
                LineageRegister.Initialize(ctx, registerFile, this.tree, this);
            }

            return true;
        }
        #endregion
    }

    public class Register
    {
        public const Int64 default_RootNodeId = 0;
        //public const int default_totalPreallocatedNodes = 10000001; // 10 million or 1 crore slots/nodes; //8000001; // 8 million slots/nodes; 3 million slots/nodes //1000001; // 3 million nodes + 1 root node preallocated in register
        public const int default_totalPreallocatedNodes = 8000001; // 8 million slots/nodes; 3 million slots/nodes //1000001; // 3 million nodes + 1 root node preallocated in register
        public const int default_maxChildrenNodes = 200; // 200 direct children; max new direct children created/inserted in any parent node throughout db.
        public MemoryStream? registerStream = null;
        public String file = "";
        public bool loadRegisterCopyToMemory(String file)
        {
            if (!File.Exists(file)) return false;
            byte[] bytes = File.ReadAllBytes(file);
            this.registerStream = new MemoryStream(bytes);
            this.registerStream.Position = 0;
            this.file = file;
            return true;
        }

        public static void writeZeros(Stream s, Int64 totalSize)
        {
            s.Position = 0;

            double double_totalBlocks = (double)totalSize / (double)1048576;
            long ltotalBlocks_floor = (long)Math.Floor(double_totalBlocks);
            long ltotalBlocks_ceiling = (long)Math.Ceiling(double_totalBlocks);
            long lProcessingBytes = 0;

            // crc32 vector by vector
            long totalVectors = 1;
            long stride = totalVectors * 1048576;
            lProcessingBytes = ltotalBlocks_floor * 1048576;
            long size = lProcessingBytes;
            while (size > 0)
            {
                // process entire vector of defined block size, if more vectors exist, process them as one by one
                byte[] buffer = new byte[stride];
                s.Write(buffer);
                s.Flush();
                size -= stride;
            }

            // now only some bytes remain which were not aligned to the buffer size, so process them off in a single instance
            long lremainingBytes = totalSize - lProcessingBytes;
            if (lremainingBytes > 0)
            {
                byte[] buffer = new byte[lremainingBytes];
                s.Write(buffer);
                s.Flush();
            }
        }

        public static bool toFile(Register registry, String file)
        {
            using (Stream? s = new FileStream(file, FileMode.CreateNew))
                return toFile(registry, s);
        }

        public static bool toFile(Register registry, Stream s)
        {
            try
            {
                // initialize and preallocate zeroed file size
                Int64 size = RegisterItem.blockSize * default_totalPreallocatedNodes;
                byte[] buffer = new byte[size];
                MemoryStream ms = new MemoryStream(buffer);
                RegisterItem emptySlotItem = new RegisterItem();
                emptySlotItem.domainType = DomainType.EmptySlot;
                emptySlotItem.specialNodeType = SpecialNodeType.EmptySlot;
                emptySlotItem.nodeType = NodeType.EmptySlot;
                emptySlotItem.flags1 = RegisterItemFlags1.None;
                byte[] blockBytes = RegisterItem.convertToBytes(emptySlotItem);
                for (Int64 id = 0; id < default_totalPreallocatedNodes; id++)
                {
                    Int64 pos = RegisterItem.blockSize * id;
                    ms.Position = pos;
                    ms.Write(blockBytes);
                    ms.Flush();
                }
                // finally save the file in vhd file
                s.Position = 0;
                s.Write(buffer);
                s.Flush();
                //writeZeros(s, size);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region "framework"

        // this method finds a first empty slot
        public static RegisterItem? FindFirstEmptySlot(OpenFSDBContext? ctx, String file, ref RegisterItem? emptySlots, bool deleteEmptySlot)
        {
            // first get the latest state of empty slots register by id
            emptySlots = LoadSetupRegisterItem(ctx, file, emptySlots.Id, false, false, false, false, true, true);
            if (emptySlots == null) return null;
            // return the first empty slot in chain of the system node
            RegisterItem? found = emptySlots.children.First();
            if (found == null) return null;
            if (deleteEmptySlot) emptySlots.children.Delete(found, ref emptySlots, true);
            return found;
        }
        // this method deletes the empty slot by id in parent node
        public static bool DeleteEmptySlot(OpenFSDBContext? ctx, String file, ref RegisterItem emptySlots, Int64 id)
        {
            if (ctx.readOnly) return false;

            // first get the latest state of empty slots register by id
            //RegisterItem? emptySlots = LoadSetupRegisterItem(ctx, file, emptySlotsId, false, false, false);
            //if (emptySlots == null) return false;

            // now get the item by id
            RegisterItem? item = LoadSetupRegisterItem(ctx, file, id, false, false, false, false, false, false);
            if (item == null) return false;

            return emptySlots.children.Delete(item, ref emptySlots, true);
        }

        // this method finds the node
        public static Int64 FindNode(OpenFSDBContext? ctx, String file, Int64 id, ref RegisterItem? itemOut)
        {
            // not vhd so process local
            try
            {
                using (Stream? s = new FileStream(file, FileMode.Open))
                    return FindNode(s, id, ref itemOut);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        // this method finds the node
        public static Int64 FindNode(Stream s, Int64 id, ref RegisterItem? itemOut)
        {
            // new change
            Int64 offset = RegisterItem.blockSize * id;
            s.Position = offset;
            RegisterItem? item = RegisterItem.convertFromBytesStream(s);
            if (item == null) return -1; // end of stream so break
            //if (item.Id != id) return -1; // error correct node not found
            
            item.Id = id;

            // valid item            
            itemOut = item;
            return offset;
        }
        // this method inserts a node in the registry
        public static Int64 InsertNode(Stream s, RegisterItem item)
        {
            // configure stream position
            Int64 pos = RegisterItem.blockSize * item.Id;
            if (s.Length < pos) s.SetLength(pos);
            s.Position = pos;

            // convert the node item to bytes and write it
            try
            {
                BinaryWriter bw = new BinaryWriter(s);
                byte[] bytes = RegisterItem.convertToBytes(item);
                bw.Write(bytes);
                item.position = pos;
                return pos;
            }
            catch
            {
                return -1;
            }
        }
        // move parent to another parent with all true descendants move along in recursion
        public static bool Move(OpenFSDBContext? ctx, String registerFile, Int64 Id, Int64 destId)
        {
            /* intelligent decision making is to be used here.
             * this is phase 1 - we decide through destination node and then source node. 
             * there are 3 types of nodes design we used in Registers: (1) Root (2) Root's Child Ancestor Adam (3) local lineage tree's node. 
             * if destination node is root, and this node is root, we do not do anything.
             * if desintation node is root, and this node's parent is root, then this node is already Root Ancestor Adam in Root, so we do not do anything.
             * if desintation node is root, and this node is local tree node, then we first form this node as Adam, and it's descendants as it's tree, in the last
             * we move this node physically as Adam into the root.
             * if desintation node is not root and destination node's parent is root, then destination node is Root Ancestor Adam, then if this node 
             * is Root Ancestor Adam we move all it's tree and itself into the desination Root Ancestor. if both nodes are same id then we do not do anything.
             * if destination node is Root Ancestor Adam and is different from this node and or this node's Root Ancestor, and this node is either Root Ancestor 
             * or a local tree node, then we move this node and it's descendants into the destination node and it's Root Ancestor.
             * if destination node is Root Ancestor Adam, and this node is local tree node, and both Root Ancestors are different, then we move this node and
             * it's descendants into destination node and it's Root Ancestor Adam.
             * if destination node is Root Ancestor Adam, and this node is local tree node, and both's Root Ancestors are same then we move this node
             * and all it's descendants into the destination node.
             * if source node is local tree child and destination node is root, then we make source node a root ancestor and migrate all it's descendants into it,
             * then we remove the source node from it's parent and insert it into root as it's child node.
             * local tree child to root ancestor is root ancestor's tree and root ancestor's children path.
             * local tree child to local tree child is parent to child register.
             * root to local tree child is migration of root ancestor and all it's tree into tree of destination root ancestor and then removal of root ancestor from
             * root and insertion into the destination parent as child.
             * in 2nd phase which is last - we straightforwardly remove this node from it's parent if required, then we move it into destination node's children register.
             * if destination node is local child node, and this node is local child node, if root ancestors are different then we move all descendants and 
             * this node into destination root ancestor then we finally physically move this node from previous parent into destination node.
             * we also change and update myNode entry config file otherwise it would result in integral failure.
             */

            if (ctx.readOnly) return false;

            // load both nodes
            if (Id == destId) return false; // cannot move on the same item
            RegisterItem? item = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, Id, false, false, false, false, true, true);
            if (item == null) return false;
            RegisterItem? parent = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, item.parentId, false, false, false, false, true, true);
            RegisterItem? dest = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, destId, false, false, false, false, true, true);
            if (dest == null) return false;

            // validations
            if (dest.childrenCount >= Register.default_maxChildrenNodes)
                return false;

            // we decide intelligently in primary conditions and in them multiple secondary conditions
            if (dest.Id == 0)
            {
                // means we are moving the source item to root
                if (item.Id == 0)
                {
                    // means source item is root, so no operation
                    return false;
                }
                else if (item.Id > 0 && item.parentId == 0)
                {
                    // means source item is root child ancestor Adam, and it is already in root, so no operation
                    return false;
                }
                else if (item.Id > 0 && item.parentId > 0)
                {
                    // means source item is common local tree node, so we need to construct and establish source node as root ancestor Adam and migrate it to root
                    // we cannot move descendants tree to root because we cannot put tree in there. we move descendants tree into root ancestor Adam node

                    // if destination is same where item is located, abort with error
                    if (item.parentId == dest.Id) return false;

                    if (!BuildCustomLineageStructure(ctx, registerFile, item)) return false;
                    // now remove the item from it's parent
                    // reload latest states
                    item = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, Id, false, false, false, false, true, true);
                    parent = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, item.parentId, false, false, false, false, true, true);
                    if (item == null) return false;
                    if (parent == null) return false;
                    // now remove the item from it's previous parent
                    if (!parent.children.Remove(item)) return false;
                    // reload latest states
                    item = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, Id, false, false, false, false, true, true);
                    dest = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, destId, false, false, false, false, true, true);
                    // tushar [ new edit: 04-12-am 07-January-2025 ] reconfigure item
                    item.treeRootId = 0;
                    // finally insert the item into destination node
                    if (!dest.children.Add(item, dest)) return false;
                    // reload latest states
                    item = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, Id, false, false, false, false, true, true);
                    // now finally physically update node files
                    String rtf = "";
                    myNode? node = entryMethods.DBFindLoadNodeOFSDB(ctx, item.Id, ref rtf, false, item.DirectorySectionId);
                    if (node == null) return false;
                    node.chapter.parentId = item.parentId;
                    byte[]? xamlbytes = null;
                    entryMethods.DBUpdateNodeOFSDB(ctx, node, "", xamlbytes, false, false, false);
                }
                else
                {
                    // means error or invalid source item so abort with error
                    return false;
                }

            }
            else if ((dest.Id > 0 && dest.parentId == 0) || (dest.Id > 0 && dest.parentId > 0))
            {
                // means we are moving the source item into a root ancestor Adam which is direct child of root
                // otherwise means the 2nd option that we are moving the source item into a local tree child as it's child and descendant
                if (item.Id == 0)
                {
                    // means source item is root, so no operation
                    return false;
                }
                else if ((item.Id > 0 && item.parentId == 0) || (item.Id > 0 && item.parentId > 0))
                {
                    // means source item is root child ancestor Adam, and it is in root, so we demote it from root ancestor and migrate it and all it's 
                    // descendants into destination root ancestor, then remove it from root and insert it in root ancestor's children register, and lastly
                    // update the node
                    // or otherwise means the 2nd option that the source item is a local tree child node, so the same, we use the same methods to move the 
                    // source node into destination node.
                    // whether destination node is root ancestor Adam, or local tree child node, we use the same methods to move the source node and it's descendants.

                    // if destination root ancestor or destination node is same as this item, abort with error
                    if (item.Id == dest.Id) return false;

                    // if destination root ancestor or destination node is already parent of this item, abort with error
                    if (item.parentId == dest.Id) return false;

                    // if it is same root ancestor where item is located, then we do not move the tree everything remains unchanged but only item is
                    // physically moved to the destination parent node.
                    if (!item.tree.MoveDescendants(item, true, dest.tree.rootAncestor)) return false;
                    // reload latest states
                    item = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, Id, false, false, false, false, true, true);
                    parent = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, item.parentId, false, false, false, false, true, true);
                    if (item == null) return false;
                    if (parent == null) return false;
                    // now remove the item from it's previous parent
                    if (!parent.children.Remove(item)) return false;
                    // reload latest states
                    item = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, Id, false, false, false, false, true, true);
                    dest = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, destId, false, false, false, false, true, true);
                    // finally insert the item into destination node
                    if (!dest.children.Add(item, dest)) return false;
                    // reload latest states
                    item = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, Id, false, false, false, false, true, true);
                    // now finally physically update node files
                    String rtf = "";
                    myNode? node = entryMethods.DBFindLoadNodeOFSDB(ctx, item.Id, ref rtf, false, item.DirectorySectionId);
                    if (node == null) return false;
                    node.chapter.parentId = item.parentId;
                    byte[]? xamlbytes = null;
                    entryMethods.DBUpdateNodeOFSDB(ctx, node, "", xamlbytes, false, false, false);
                }
                else
                {
                    // means error or invalid source item so abort with error
                    return false;
                }
            }
            else
            {
                // means error or invalid destination node so abort with error
                return false;
            }

            return true;
        }

        // this method inserts a node in the registry and writes the node files in db path
        public static RegisterItem? Insert(OpenFSDBContext? ctx, String registerFile, Int64 parentId,
            RegisterItem? emptySlotsItem, myNode? node, String? rtf, byte[]? xamlbytes)
        {
            if (ctx.readOnly) return null;

            // first get latest state of the parent node
            RegisterItem? parent = LoadSetupRegisterItem(ctx, registerFile, parentId, false, false, false, false, true, true);
            if (parent == null) return null;

            // validations
            if (parent.childrenCount >= Register.default_maxChildrenNodes)
                return null;

            // 2nd setup item
            node.chapter.parentId = parent.Id;
            //DomainType domainType = node.chapter.domainType;
            if (node.chapter.nodeType == NodeType.EmptySlot) node.chapter.domainType = DomainType.HiddenCore;

            if (emptySlotsItem == null) return null; // no empty slots register so abort with error

            //// 3rd get the first empty slot and delete it from empty slots register because it is not empty anymore as it should be allocated
            // empty slots system node is set as param, so go through it
            RegisterItem? firstEmptySlot = FindFirstEmptySlot(ctx, registerFile, ref emptySlotsItem, false);
            if (firstEmptySlot != null)
            {
                // found an empty slot which is under the current db index, so we use this empty slot and do not increment the db index
                // db index is incremented only when all empty slots have been used upto the current db index so we use current db index and increment it
                // configure
                node.chapter.Id = firstEmptySlot.Id;
                // free the empty slot from it's parent register
                if (emptySlotsItem != null)
                {
                    if (!DeleteEmptySlot(ctx, registerFile, ref emptySlotsItem, node.chapter.Id)) // if empty slots is passed then delete the empty slot from it
                        return null; // error
                }
                // now create node at first empty slot
                if (!entryMethods.DBCreateNodeOFSDB(ctx, node, rtf, xamlbytes, false, false, false, false))
                    return null; // error
            }
            else
            {
                // critical error no empty slot found meaning register is full, so we need to enlarge the register
                // todo tushar 07:32 PM, 29 December 2024 = do enlarge the register and create and register the empty slots into it and in empty slots
                // system node and then save and reload the register and finally create the node in the newly created first empty slot
                return null;
            }
            // finally update the register add this node
            // todo tushar: lineage root head tail etc.
            RegisterItem? item = new RegisterItem(0, node.chapter.Id,
                node.chapter.parentId, node.DirectorySectionID, 0, 0, 0, 0, 0,
                node.chapter.nodeType, node.chapter.specialNodeType, node.chapter.domainType, RegisterItemFlags1.None, 0, 0, 0, 0, 0, 0);

            // add to parent's children register
            if (!parent.children.Add(item, parent))
                return null;

            // initialize the node item
            item.node = node;
            item.children = new ChildrenRegister();
            if (!ChildrenRegister.Initialize(ctx, ctx.dbNodeTreeRegistryFile, item.children, item))
                return null; // critical error

            // first get latest state of the parent node
            parent = LoadSetupRegisterItem(ctx, registerFile, parentId, false, false, false, false, true, true);
            if (parent == null) return null;

            // root cannot have lineage chain because of integrity issues while manipulating trees
            // so only root child or Adam has lineage tree
            if (parent.Id == 0)
            {
                // parent is root so this item is root child node
                item.tree = new LineageRegister();
                if (!LineageRegister.Initialize(ctx, ctx.dbNodeTreeRegistryFile, item.tree, item))
                    return null; // critical error

            }
            else
            {
                // todo tushar 01-January-2025: bug is here, parent object is expired and is overwritten with corruption at the back
                // in parentchildren.Add() function because the parent object is not integrally synchronized with latest correct config below.
                // parent is not root so add to parent's lineage
                // add to parent's root ancestor's descendants register
                if (!parent.tree.Add(item))
                    return null;

                // initialize root child lineage tree into item
                item.tree = new LineageRegister();
                if (!LineageRegister.Initialize(ctx, ctx.dbNodeTreeRegistryFile, item.tree, item))
                    return null; // critical error

            }
            if (item != null)
            {
                ctx.dbConfig.latestCreatedEntry = item.Id;
                DatabaseConfig.toXmlFile(ctx, ctx.dbConfig, ctx.dbConfigFile);
            }
            return item;
        }

        // this method deletes a node recursively
        public static bool Delete(OpenFSDBContext? ctx, String registerFile, Int64 Id, ref RegisterItem? emptySlots,
            bool withoutEmptySlotsRegister)
        {
            if (ctx.readOnly) return false;

            // reload latest states
            if (Id <= 0) return false; // error invalid node
            RegisterItem? item = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, Id, false, false, false, false, true, true);
            if (item == null) return false;

            // we decide intelligently in primary conditions and in them multiple secondary conditions
            if (item.Id == 0)
            {
                // means this is root node, so abort with error we cannot delete root
            }
            else if ((item.Id > 0 && item.parentId == 0) || (item.Id > 0 && item.parentId > 0))
            {
                // means this is either root ancestor Adam node, or otherwise a local tree child node or local node
                return item.tree.DeleteDescendants(item, true, ref emptySlots, withoutEmptySlotsRegister);
            }
            else
            {
                // means error or invalid destination node so abort with error
                return false;
            }
            return true;
        }

        // this method inserts a node in the registry
        public static Int64 InsertNode(OpenFSDBContext? ctx, String file, RegisterItem item)
        {
            if (ctx.readOnly) return -1;
            if (!File.Exists(file)) return -1;

            try
            {
                using (Stream? s = new FileStream(file, FileMode.Open))
                {
                    return InsertNode(s, item);
                }
            }
            catch
            {
                return -1;
            }

        }
        // this method overwrites with empty slot and registers it with empty slots system node
        public static bool WriteEmptySlot(OpenFSDBContext? ctx, String file,
            ref RegisterItem emptySlotsParentItem, Int64 id, ref RegisterItem? itemOut)
        {
            if (ctx.readOnly) return false;
            if (!File.Exists(file)) return false;

            RegisterItem? emptySlotItem = new RegisterItem();
            emptySlotItem.Id = id;
            emptySlotItem.parentId = emptySlotsParentItem.Id;
            emptySlotItem.nodeType = NodeType.EmptySlot;
            emptySlotItem.specialNodeType = SpecialNodeType.EmptySlot;
            emptySlotItem.domainType = DomainType.EmptySlot;

            // convert the node item to bytes and write it
            // add this empty slot into empty slots parent's register. this also overwrites the old with new item
            if (!emptySlotsParentItem.children.Add(emptySlotItem, emptySlotsParentItem)) return false;
            itemOut = emptySlotItem;
            return true;
        }

        // this method deletes a node in the registry
        public static bool DeleteNode(OpenFSDBContext? ctx, String file, RegisterItem item)
        {
            if (ctx.readOnly) return false;
            if (!File.Exists(file)) return false;

            try
            {
                using (Stream? s = new FileStream(file, FileMode.Open))
                {
                    return DeleteNode(s, item);
                }
            }
            catch
            {
                return false;
            }
        }
        // this method deletes a node in the registry
        public static bool DeleteNode(Stream s, RegisterItem? item)
        {
            if (item == null) return false;
            return DeleteNode(s, item.Id);
        }

        // this method deletes a node in the registry
        public static bool DeleteNode(Stream s, Int64 id)
        {
            try
            {
                Int64 pos = RegisterItem.blockSize * id;
                s.Position = pos;
                RegisterItem default0Item = new RegisterItem();
                default0Item.nodeType = NodeType.EmptySlot;
                default0Item.specialNodeType = SpecialNodeType.EmptySlot;
                default0Item.domainType = DomainType.EmptySlot;
                BinaryWriter bw = new BinaryWriter(s);
                bw.Write(RegisterItem.convertToBytes(default0Item));
                return true;
            }
            catch
            {
                return false;
            }
        }
        // this method updates a node in the registry
        public static bool UpdateNode(OpenFSDBContext? ctx, String file,
            RegisterItem? item, Int64 parentId, bool useParentId, Int64 DirectorySectionId, bool useDirectorySectionId,
            Int32 childrenCount, bool useChildrenCount)
        {
            if (ctx.readOnly) return false;
            if (!File.Exists(file)) return false;

            try
            {
                using (Stream? s = new FileStream(file, FileMode.Open))
                {
                    return UpdateNode(s, item, parentId, useParentId, DirectorySectionId, useDirectorySectionId,
                        childrenCount, useChildrenCount);
                }
            }
            catch
            {
                return false;
            }
        }

        // this method updates a node in the registry
        public static bool UpdateNode(Stream s, RegisterItem? item, Int64 parentId, bool useParentId, Int64 DirectorySectionId, bool useDirectorySectionId,
            Int32 childrenCount, bool useChildrenCount)
        {
            if (item == null) return false;

            if (useParentId)
                item.parentId = parentId;

            if (useDirectorySectionId)
                item.DirectorySectionId = DirectorySectionId;

            if (useChildrenCount)
                item.childrenCount = childrenCount;

            // convert the node item to bytes and write it
            try
            {
                Int64 pos = RegisterItem.blockSize * item.Id;
                s.Position = pos;
                BinaryWriter bw = new BinaryWriter(s);
                byte[] bytes = RegisterItem.convertToBytes(item);
                bw.Write(bytes);
                return true;
            }
            catch
            {
                return false;
            }
        }
        // gets all parents right to the root
        public static bool Lineage(OpenFSDBContext? ctx, String file,
            RegisterItem item, ref List<RegisterItem>? listOut, bool topFirst, bool addCurrentItem = true, bool addRoot = true)
        {
            if (!File.Exists(file)) return false;

            try
            {
                using (Stream? s = new FileStream(file, FileMode.Open))
                {
                    return Lineage(s, item, ref listOut, topFirst, addCurrentItem, addRoot);
                }
            }
            catch
            {
                return false;
            }
        }
        // gets all parents right to the root
        public static bool Lineage(OpenFSDBContext? ctx, String file,
            RegisterItem item, ref List<RegisterItem>? listOut, bool topFirst, bool loadNode, bool loadRtf,
            bool loadChildren, bool loadChildrenConfigs)
        {
            List<RegisterItem> list = new List<RegisterItem>();

            // first add the target item
            list.Add(item);

            listOut = list;

            // this item is root, it does not has any parent so abort
            if (item.Id == 0) return true;

            // this item is not root but a child item, so get all lineage right to the root
            // ascending order list from bottom to top parents
            while (true)
            {
                item = LoadSetupRegisterItem(ctx, file, item.parentId, loadNode, loadRtf, loadChildren, loadChildrenConfigs, false, false);
                if (item == null) break;

                // finally add this parent to the list
                list.Add(item);

                if (item.Id == 0) break; // this is root so there is nothing more so we abort
            }

            // make descending order list: parent then child highest is first lowest is last, if required
            if (topFirst) list.Reverse();

            return true;
        }
        // checks if item descends from any of the ancestor right to the root ancestor
        public static bool ExistsSomewhereInAncestoralChain(OpenFSDBContext? ctx, String file, RegisterItem? item, RegisterItem? target)
        {
            List<RegisterItem>? targetLineage = null;
            Register.Lineage(ctx, file, target, ref targetLineage, true, true, false);
            List<RegisterItem>? itemLineage = null;
            Register.Lineage(ctx, file, item, ref itemLineage, true, true, false);

            foreach (RegisterItem? ancestor in itemLineage)
            {
                RegisterItem? found = targetLineage.Find(x => x.Id == ancestor.Id);
                if (found != null)
                {
                    // yes current item is descendant in target's ancestoral chain
                    return true;
                }
            }
            // no current item is not a descendant in target's ancestral chain
            return false;
        }

        // gets the root ancestor
        public static bool LineageRootAncestor(OpenFSDBContext? ctx, String file,
            RegisterItem item, ref RegisterItem? rootAncestorOut, bool loadNode, bool loadRtf)
        {
            // this item is root, it does not has any parent so abort with error
            if (item.Id == 0) return false;
            // this item is root ancestor itself so abort with error
            if (item.parentId == 0) return false;

            // this item is not root but a child item, so get all lineage right to the root
            // ascending order list from bottom to top parents
            while (true)
            {
                FindNode(ctx, file, item.parentId, ref item);
                if (item == null) break; // parent item not found
                if (item.Id == 0) break; // this is root so there is nothing more so we abort
                if (item.parentId == 0)
                {
                    // this item is root ancestor
                    byte[]? xamlbytesOut = null;
                    if (loadNode) item.loadNode(ctx, ref item.rtf, ref xamlbytesOut, loadRtf);
                    rootAncestorOut = item;
                    return true;
                }
            }
            return false;
        }


        // finds an ancestor by id in lineage list
        public static bool FindLineageAncestor(List<RegisterItem> list, Int64 id)
        {
            foreach (RegisterItem? ancestor in list)
            {
                if (ancestor.Id == id) return true;
            }
            return false;
        }
        public static bool LineageRemoveRoot(List<RegisterItem> list)
        {
            RegisterItem? root = null;
            foreach (RegisterItem? ancestor in list)
            {
                if (ancestor.Id == 0)
                {
                    // root found
                    root = ancestor;
                    break;
                }
            }
            if (root != null) list.Remove(root);
            return true;
        }
        public static bool LineageRemoveAncestor(List<RegisterItem> list, Int64 id)
        {
            RegisterItem? item = null;
            foreach (RegisterItem? ancestor in list)
            {
                if (ancestor.Id == id)
                {
                    // root found
                    item = ancestor;
                    break;
                }
            }
            if (item != null) list.Remove(item);
            return true;
        }
        // gets all parents right to the root
        public static bool Lineage(Stream s, RegisterItem? item, ref List<RegisterItem>? listOut, bool topFirst, bool addCurrentItem, bool addRoot)
        {
            List<RegisterItem> list = new List<RegisterItem>();

            // first add the target item
            if (addCurrentItem) list.Add(item);

            listOut = list;

            // this item is root, it does not has any parent so abort
            if (item.Id == 0) return true;

            // this item is not root but a child item, so get all lineage right to the root
            // ascending order list from bottom to top parents
            while (true)
            {
                s.Position = 0;
                Int64 offset = FindNode(s, item.parentId, ref item);
                if (offset < 0) break;

                // check integrity
                // tushar new addition: 19 January 2025 07:46 pm
                if (item.domainType == DomainType.EmptySlot)
                    return false; // critical error, integrity of ancestoral chain invalid/corrupted, error abort

                // finally add this parent to the list
                if (addRoot)
                {
                    // always add root
                    list.Add(item);
                }
                else
                {
                    // do not add root but all other items
                    if (item.Id != 0) list.Add(item);
                }

                if (item.Id == 0) break; // this is root so there is nothing more so we abort
            }

            // make descending order list: parent then child highest is first lowest is last, if required
            if (topFirst) list.Reverse();

            return true;
        }
        // checks ancestral integrity
        public static bool LineageIntegrity(OpenFSDBContext? ctx, String file,
            RegisterItem item)
        {
            if (item.domainType == DomainType.EmptySlot)
                return false; // critical error, integrity of ancestoral chain invalid/corrupted, error abort

            // this item is root, it does not has any parent so abort
            if (item.Id == 0) return true;

            // this item is not root but a child item, so get all lineage right to the root
            // ascending order list from bottom to top parents
            while (true)
            {
                item = LoadSetupRegisterItem(ctx, file, item.parentId, true, false, false, false, false, false);
                if (item == null) break;

                // check integrity
                // tushar new addition: 19 January 2025 07:46 pm
                if (item.node == null)
                    return false; // node entry files not found meaning integral failure abort with error
                if (item.domainType == DomainType.EmptySlot)
                    return false; // critical error, integrity of ancestoral chain invalid/corrupted, error abort

                if (item.Id == 0) break; // this is root so there is nothing more so we abort
            }
            return true;
        }

        // gets full path string
        public static String LineageFullPath(List<RegisterItem> list)
        {
            String path = @"\";
            foreach (RegisterItem item in list)
                path = Path.Combine(path, item.Id.ToString());

            return path;
        }

        // converts full path into ancestor node ids
        public static bool LineageFromFullPath(String path, ref List<Int64> listOut)
        {
            if (path == "") return false;
            List<Int64> list = new List<Int64>();
            String[] ancestors = path.Split(@"\");
            if (ancestors.Count() == 0) return false; // now a valid path

            foreach (String ancestor in ancestors)
            {
                if (ancestor == "") continue;

                Int64 value = 0;
                if (!Int64.TryParse(ancestor, out value)) return false; // invalid path or garbage
                list.Add(value);
            }
            listOut = list;
            return true;
        }

        // converts full path into ancestor node items
        public static bool LineageFromFullPath(Stream s, String path, ref List<RegisterItem>? listOut)
        {
            if (path == "") return false;
            List<RegisterItem> list = new List<RegisterItem>();
            String[] ancestors = path.Split(@"\");
            if (ancestors.Count() == 0) return false; // now a valid path

            foreach (String ancestor in ancestors)
            {
                if (ancestor == "") continue;

                Int64 id = 0;
                if (!Int64.TryParse(ancestor, out id)) return false; // invalid path or garbage

                s.Position = 0;
                RegisterItem? item = null;
                Int64 offset = FindNode(s, id, ref item);
                if (offset < 0) break;

                list.Add(item);
            }
            listOut = list;
            return true;
        }

        // loads all registry node items from path
        public static bool LoadFullPath(OpenFSDBContext? ctx, String file, String path, ref List<RegisterItem>? listOut, bool topFirst)
        {
            if (!File.Exists(file)) return false;

            try
            {
                using (Stream? s = new FileStream(file, FileMode.Open))
                {
                    return LoadFullPath(s, path, ref listOut, topFirst);
                }
            }
            catch
            {
                return false;
            }
        }
        // loads all registry node items from path
        public static bool LoadFullPath(OpenFSDBContext? ctx, String file, RegisterItem item, ref List<RegisterItem>? listOut, bool topFirst,
            bool loadNode, bool loadRtf, bool loadChildren, bool loadChildrenConfigs)
        {
            if (!File.Exists(file)) return false;
            return Lineage(ctx, file, item, ref listOut, topFirst, loadNode, loadRtf, loadChildren, loadChildrenConfigs);
        }

        // loads all registry node items from path
        public static bool LoadFullPath(Stream s, String path, ref List<RegisterItem>? listOut, bool topFirst)
        {
            List<RegisterItem> list = new List<RegisterItem>();

            // phase 1 - get all lineage ids

            // get all ancestors ids
            List<Int64> lineageItems = null;
            if (!LineageFromFullPath(path, ref lineageItems)) return false;
            if (lineageItems.Count == 0) return false;

            // phase 2 - get the lowest item by id from the register

            // now get the lowest child's offset and item
            Int64 lowestChildId = lineageItems.LastOrDefault();
            RegisterItem? item = null;
            s.Position = 0;
            Int64 offset = FindNode(s, lowestChildId, ref item);
            if (item == null) return false; // error node not found or something wrong abort with error

            // phase 3 - get lineage

            // now directly get lineage
            if (!Lineage(s, item, ref list, topFirst, true, true)) return false; // if error return error

            // success
            listOut = list;
            return true;
        }

        // loads full path items from a target item id
        public static bool LoadFullPath(Stream s, RegisterItem item, ref List<RegisterItem>? listOut, bool topFirst)
        {
            List<RegisterItem>? list = null;

            // now directly get lineage
            if (!Lineage(s, item, ref list, topFirst, true, true)) return false; // if error return error

            // success
            listOut = list;
            return true;
        }
        // loads full path items from a target item id
        public static bool LoadFullPath(OpenFSDBContext? ctx, String file, Int64 id, ref List<RegisterItem>? listOut, bool topFirst)
        {
            // first get the item by id
            RegisterItem? item = LoadSetupRegisterItem(ctx, file, id, false, false, false, false, true, true);
            if (item == null) return false;

            List<RegisterItem>? list = null;

            // now directly get lineage
            if (!Lineage(ctx, file, item, ref list, topFirst)) return false; // if error return error

            // success
            listOut = list;
            return true;
        }
        // gets register file's size
        public static Int64 RegisterFileSize(OpenFSDBContext? ctx, String file)
        {
            if (!File.Exists(file)) return -1;

            try
            {
                using (Stream? s = new FileStream(file, FileMode.Open))
                {
                    return s.Length;
                }
            }
            catch
            {
                return -1;
            }
        }

        // returns the total number of entries
        public static Int64 Count(OpenFSDBContext? ctx, String file,
            RegisterItem? emptySlotsItem, ref Int64 emptySlotsCountOut, ref Int64 regfileSizeOut)
        {
            // first get the latest state of empty slots register by id
            emptySlotsItem = LoadSetupRegisterItem(ctx, file, emptySlotsItem.Id, false, false, false, false, false, false);
            if (emptySlotsItem == null) return -1;

            Int64 emptySlotsCount = emptySlotsItem.childrenCount;
            Int64 registerFileSize = RegisterFileSize(ctx, file);
            emptySlotsCountOut = emptySlotsCount;
            regfileSizeOut = registerFileSize;
            Int64 count = registerFileSize / RegisterItem.blockSize;
            count -= emptySlotsCount;
            return count;
        }
        // returns the total number of entries which validly exist in all root ancestors
        public static Int64 CountValidEntries(OpenFSDBContext? ctx, String file)
        {
            // first get the latest state of root register by id
            RegisterItem? root = LoadSetupRegisterItem(ctx, file, 0, false, false, false, false, false, false);
            if (root == null) return -1;

            Int64 nextSiblingId = -1;

            RegisterItem? current = null;

            if (root.firstChildId == 0) return 1; // there are no children except root, so return 1

            // get the head
            if (Register.FindNode(ctx, file, root.firstChildId, ref current) < 0)
                return -1; // end of chain or error, abort

            if (current == null)
                return -1; // end of chain or error, abort

            nextSiblingId = current.Id;

            Int64 count = 1; // increment root node's counter

            while (true)
            {
                // load item from registry
                RegisterItem? nextItem = null;
                Int64 nextOffset = Register.FindNode(ctx, file, nextSiblingId, ref nextItem);
                if (nextOffset < 0) break; // error or end of stream or end of register so break
                if (nextItem == null) break; // no more items so break

                // update count
                count += nextItem.descendantsCount; // total descendants of this root ancestor
                count += 1; // increment by 1 which is this root ancestor

                // configure
                nextSiblingId = nextItem.nextSiblingId;

                // check if end of children register reached
                if (nextItem.nextSiblingId == 0)
                    break;
            }
            return count;
        }
        // we get system nodes
        public static bool FindSystemNodeRegisterItem(OpenFSDBContext? ctx, String file,
            RegisterItem? parent, NodeType type, ref RegisterItem? itemOut)
        {
            // now get the required register item

            Int64 nextSiblingId = -1;

            RegisterItem? current = null;

            if (parent.firstChildId == 0) return true; // there are no children so abort and return empty list

            // get the head
            if (Register.FindNode(ctx, file, parent.firstChildId, ref current) < 0)
                return false; // end of chain or error or no children present in parent, abort

            if (current == null)
                return false; // there is no child node present so abort

            nextSiblingId = current.Id;

            while (true)
            {
                // load item from registry
                RegisterItem? nextItem = null;
                Int64 nextOffset = Register.FindNode(ctx, file, nextSiblingId, ref nextItem);
                if (nextOffset < 0) break; // error or end of stream or end of register so break
                if (nextItem == null) break; // no more items so break

                // configure
                nextSiblingId = nextItem.nextSiblingId;

                if (nextItem.specialNodeType == SpecialNodeType.SystemNode)
                {
                    if (nextItem.nodeType == type)
                    {
                        // found match return it directly
                        itemOut = nextItem;
                        return true;
                    }
                }

                // item not found continue if required
                // check if end of children register reached
                if (nextItem.nextSiblingId == 0) break;
            }
            return false;
        }
        // we get system nodes
        public static bool FindRegisterItems(OpenFSDBContext? ctx, String file,
            Int64 parentId, NodeType type, bool useType, SpecialNodeType specialType, bool useSpecialType,
            DomainType domainType, bool useDomainType, ref List<RegisterItem>? listOut, bool loadNode)
        {

            List<RegisterItem> list = new List<RegisterItem>();

            // first get parent
            listOut = list;
            RegisterItem? parent = LoadSetupRegisterItem(ctx, file, parentId, false, false, false, false, false, false);
            if (parent == null) return false;
            if (parent.childrenCount == 0 && parent.firstChildId == 0) return true;
            // now get the required register item

            Int64 nextSiblingId = -1;

            RegisterItem? current = null;

            // get the head
            if (Register.FindNode(ctx, file, parent.firstChildId, ref current) < 0)
                return false; // end of chain or error or no children present in parent, abort

            if (current == null)
                return false; // there is no child node present so abort

            nextSiblingId = current.Id;
            if (current.Id == 0) return false; // error root has no siblings or next sibling item
            if (nextSiblingId == 0) return true; // end of stream or end of register so break

            while (true)
            {
                // load item from registry
                RegisterItem? nextItem = null;
                Int64 nextOffset = Register.FindNode(ctx, file, nextSiblingId, ref nextItem);
                if (nextOffset <= 0) break; // error or end of stream or end of register so break
                if (nextItem == null) break; // no more items so break

                // configure
                nextSiblingId = nextItem.nextSiblingId;

                if (useSpecialType)
                {
                    if (nextItem.specialNodeType != specialType)
                        continue;
                }

                if (useType)
                {
                    if (nextItem.nodeType != type)
                        continue;
                }
                if (useDomainType)
                {
                    if (nextItem.domainType != domainType)
                        continue;
                }
                String rtf = "";

                // load node config file if required
                byte[]? xamlbytesOut = null;
                if (loadNode) nextItem.loadNode(ctx, ref rtf, ref xamlbytesOut, false);

                // this is valid item so add it to list
                list.Add(nextItem);

                // check if end of children register reached
                if (nextItem.nextSiblingId == 0) break;
            }
            // done
            return true;
        }

        // this method loads and setups up a register item
        public static RegisterItem? LoadSetupRegisterItem(OpenFSDBContext? ctx, String file, Int64 id, bool loadNode, bool loadRtf,
            bool loadChildren, bool loadChildrenConfigs, bool loadChildrenRegister, bool loadLineageRegister)
        {
            // initialize register
            RegisterItem? item = null;
            if (Register.FindNode(ctx, file, id, ref item) < 0) return null; // critical error
            if (item == null) return null; // critical error

            item.node = null;
            if (loadNode) item.loadNode(ctx, ref item.rtf, ref item.xamlbytes, loadRtf);

            if (loadChildrenRegister)
            {
                item.children = new ChildrenRegister();
                if (!ChildrenRegister.Initialize(ctx, file, item.children, item))
                    return null; // critical error

                if (loadChildren)
                    item.children.GetChildren(ref item.childrenList, loadChildrenConfigs);

            }

            if (loadLineageRegister)
            {
                if (item.Id != 0)
                {
                    // item is not root, we cannot setup lineage tree chain in absolute root itself
                    item.tree = new LineageRegister();
                    if (!LineageRegister.Initialize(ctx, file, item.tree, item))
                        return null; // critical error
                }
            }

            // done
            return item;
        }
        // this method loads and setups up a register item
        public static RegisterItem? LoadSetupRegisterItem(OpenFSDBContext? ctx, String file, RegisterItem item, bool loadNode, bool loadRtf,
            bool loadChildren, bool loadChildrenConfigs, bool loadChildrenRegister, bool loadLineageRegister)
        {
            byte[]? xamlbytesOut = null;
            if (loadNode) item.loadNode(ctx, ref item.rtf, ref xamlbytesOut, loadRtf);

            if (loadChildrenRegister)
            {
                item.children = new ChildrenRegister();
                if (!ChildrenRegister.Initialize(ctx, file, item.children, item))
                    return null; // critical error
            }

            if (loadLineageRegister)
            {
                if (item.Id != 0)
                {
                    // item is not root, we cannot setup lineage tree chain in absolute root itself
                    item.tree = new LineageRegister();
                    if (!LineageRegister.Initialize(ctx, file, item.tree, item))
                        return null; // critical error
                }
            }

            if (loadChildren)
                item.children.GetChildren(ref item.childrenList, loadChildrenConfigs);

            // done
            return item;
        }
        // this method chains up all the empty slots into the parent empty slots system node's register
        public static bool BuildEmptySlotsRegister(OpenFSDBContext? ctx, String file, RegisterItem emptySlots)
        {
            // phase 1 - load the entire register into memory
            byte[]? buffer = File.ReadAllBytes(file);
            if (buffer == null) return false;
            if (buffer.Length == 0) return false;
            MemoryStream ms = new MemoryStream(buffer);
            BinaryWriter bw = new BinaryWriter(ms);
            ms.Position = 0;

            RegisterItem? firstChild = null;
            RegisterItem? lastChild = null;
            Int64 totalSlots = buffer.LongLength / RegisterItem.blockSize;
            Int64 id = 1; // we cannot change root node which is 0 so we start from 1
            Int64 emptySlotsFound = 0;

            while (id < totalSlots)
            {
                // set position to current slot by id
                ms.Position = RegisterItem.blockSize * id;
                // read slot into register item
                RegisterItem? current = RegisterItem.convertFromBytesStream(ms);
                if (current.domainType != DomainType.EmptySlot)
                {
                    // this is an allocated slot used by some item, so skip this slot
                    id++;
                    continue;
                }
                
                // this is an empty zeroed slot, so take it
                if (firstChild == null) firstChild = current; // if this is the first empty slots child set it as first child

                current.firstChildId = current.lastChildId = 0;
                if (lastChild != null)
                {
                    // there is a last child item either 1st or 2nd or any other item in chain
                    current.previousSiblingId = lastChild.Id;
                    lastChild.nextSiblingId = id;
                    lastChild.parentId = emptySlots.Id;
                    lastChild.nodeType = NodeType.EmptySlot;
                    lastChild.specialNodeType = SpecialNodeType.EmptySlot;
                    lastChild.domainType = DomainType.EmptySlot;
                    // update last node in buffer
                    ms.Position = lastChild.position;
                    bw.Write(RegisterItem.convertToBytes(lastChild));
                }
                else
                {
                }
                // update current in buffer
                current.nextSiblingId = 0;
                current.Id = id;
                current.DirectorySectionId = 0;
                current.parentId = emptySlots.Id;
                current.nodeType = NodeType.EmptySlot;
                current.specialNodeType = SpecialNodeType.EmptySlot;
                current.domainType = DomainType.EmptySlot;
                // update current node in buffer
                ms.Position = current.position;
                bw.Write(RegisterItem.convertToBytes(current));
                // set current as the last child item in chain so we can process it in next id
                lastChild = current;
                emptySlotsFound++;
                id++;
            }
            // finally update parent node
            if (firstChild != null)
                emptySlots.firstChildId = firstChild.Id;
            else
                emptySlots.firstChildId = emptySlots.lastChildId = 0;

            if (lastChild != null)
                emptySlots.lastChildId = lastChild.Id;
            else
                emptySlots.lastChildId = emptySlots.firstChildId; // we apply first child id from parent in both first and last child ids

            // update parent node in buffer
            emptySlots.childrenCount = emptySlotsFound;
            ms.Position = emptySlots.position;
            bw.Write(RegisterItem.convertToBytes(emptySlots));
            bw.Flush();
            ms.Flush();
            // now finally change the file
            File.WriteAllBytes(file, buffer);
            GC.Collect();
            return true;
        }
        
        public static void test(OpenFSDBContext? ctx, String file)
        {
            /*
            RegisterItem? root = LoadSetupRegisterItem(ctx, file, 0, true, false, true, true, true, false);
            RegisterItem? lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
            RegisterItem? jrn = LoadSetupRegisterItem(ctx, file, 1, true, false, true, true, true, true);
            RegisterItem? emptySlots = entryMethodsNewDesign.findRegistryEmptySlotSystemNodeItem(root.childrenList);
            emptySlots.InitializeSystem(ctx, file, true, true);

            myNode? node = new myNode(true);

            RegisterItem item = Insert(ctx, file, lib.Id, ref emptySlots, ref node, "");
            RegisterItem item2 = Insert(ctx, file, lib.Id, ref emptySlots, ref node, "");
            RegisterItem item3 = Insert(ctx, file, lib.Id, ref emptySlots, ref node, "");
            for (int i = 0; i < 30; i++)
            {
                node = new myNode(true);
                RegisterItem item4 = Insert(ctx, file, item.Id, ref emptySlots, ref node, "");
                RegisterItem item5 = Insert(ctx, file, item4.Id, ref emptySlots, ref node, "");
                RegisterItem item6 = Insert(ctx, file, item5.Id, ref emptySlots, ref node, "");
                RegisterItem item7 = Insert(ctx, file, item6.Id, ref emptySlots, ref node, "");

            }
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            root = LoadSetupRegisterItem(ctx, file, root.Id, false, false, false, false, true, true);
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);

            Move(ctx, file, item.Id, 0);
            Move(ctx, file, item.Id, 1);

            String rtf = "";
            myNode? nodetest = entryMethods.DBFindLoadNodeOFSDB(ctx.dbCtx, item.Id, ref rtf, false, item.DirectorySectionId);

            jrn = LoadSetupRegisterItem(ctx, file, 1, true, false, true, true, true, true);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            root = LoadSetupRegisterItem(ctx, file, root.Id, false, false, false, false, true, true);
            List<RegisterItem> item20tree = null;
            RegisterItem? item20 = LoadSetupRegisterItem(ctx, file, 20, false, false, false, false, true, true);
            item20.tree.GetDescendants(item20, ref item20tree);
            //BuildCustomLineageStructure(ctx, file, item);
            //Register.Delete(ctx, file, 20, ref emptySlots, false);
            Move(ctx, file, 32, 1);
            Move(ctx, file, item.Id, 35);
            Register.Delete(ctx, file, item.Id, ref emptySlots, false);
            Move(ctx, file, 32, 0);
            Move(ctx, file, 32, 2);
            Register.Delete(ctx, file, 2, ref emptySlots, false);
            emptySlots = LoadSetupRegisterItem(ctx, file, emptySlots.Id, true, false, true, true, true, true);
            jrn = LoadSetupRegisterItem(ctx, file, 1, true, false, true, true, true, true);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            root = LoadSetupRegisterItem(ctx, file, root.Id, false, false, true, false, true, true);

            List<RegisterItem> item35tree = null;
            RegisterItem? item35 = LoadSetupRegisterItem(ctx, file, 35, false, false, false, false, true, true);
            item35.tree.GetDescendants(item35, ref item35tree);
            List<RegisterItem> item35children = null;
            item35.children.GetChildren(ref item35children, false);

            List<RegisterItem> item32tree = null;
            RegisterItem? item32 = LoadSetupRegisterItem(ctx, file, 32, false, false, false, false, true, true);
            item32.tree.GetDescendants(item32, ref item32tree);
            List<RegisterItem> item32children = null;
            item32.children.GetChildren(ref item32children, false);

            List<RegisterItem> itemtree = null;
            List<RegisterItem> rootchildren = null;
            root.children.GetChildren(ref rootchildren, false);
            List<RegisterItem> itemchildren = null;
            item.children.GetChildren(ref itemchildren, false);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            item.tree.GetWholeTree(ref itemtree, false);

            List<RegisterItem> jrnchildren = null;
            List<RegisterItem> jrntree = null;
            List<RegisterItem> libtree = null;
            jrn.children.GetChildren(ref jrnchildren, false);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            item.tree.GetWholeTree(ref itemtree, false);
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
            List<RegisterItem> libchildren = null;
            lib.children.GetChildren(ref libchildren, false);
            jrn.tree.GetWholeTree(ref jrntree, false);
            lib.tree.GetWholeTree(ref libtree, false);

            return;
            */

            /*
            //List<RegisterItem> rootchildren = null;
            //root.children.GetChildren(ref rootchildren, false);
            //item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            //item.tree.GetWholeTree(ref itemtree, false);

            root = LoadSetupRegisterItem(ctx, file, root.Id, false, false, false, false, true, true);
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            Register.Move(ctx, file, item.Id, root.Id);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            root = LoadSetupRegisterItem(ctx, file, root.Id, false, false, false, false, true, true);
            //List<RegisterItem> rootchildren = null;
            root.children.GetChildren(ref rootchildren, false);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            item.tree.GetWholeTree(ref itemtree, false);
            */

            //List<RegisterItem> libchildren = null;

            /*
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false);
            lib.children.Remove(item);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false);
            root.children.Add(item, root);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false);
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true);
            lib.children.GetChildren(ref libchildren, false);

            List<RegisterItem> itemtree = null;
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false);
            item.tree.GetWholeTree(ref itemtree, false);
            List<RegisterItem> itemchildren = null;
            item.children.GetChildren(ref itemchildren, false);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false);
            item2 = LoadSetupRegisterItem(ctx, file, item2.Id, false, false, false, false);
            item.tree.MoveDescendants(item, false, item2.tree.rootAncestor);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false);
            item2 = LoadSetupRegisterItem(ctx, file, item2.Id, false, false, false, false);
            List<RegisterItem> item2tree = null;
            item2.tree.GetWholeTree(ref item2tree, false);
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true);
            List<RegisterItem> libtree = null;
            lib.tree.GetWholeTree(ref libtree, false);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false);
            item.tree.GetWholeTree(ref itemtree, false);
            item.children.GetChildren(ref itemchildren, false);
            libchildren = null;
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true);
            lib.children.GetChildren(ref libchildren, false);
            */

            /* 05:49 PM, 03 January 2025: i discovered that = 
             * integral synchronization is mandatory in every step. we change lib and we change item, in both changes in every step
             * intregral synchronization by reloading the latest state register item from registers are mandatory. if even a single 
             * integral synchronization is missing then integral corruption will happen and the register and db will be integrally corrupted.
             * when we add item to lib, when we remove item from lib, both item and lib are integrally changed, 
             * so we are required to reload both of them their latest states from register.
            // step 1 =
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            lib.children.Remove(item);
            // step 2 =
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
            item.tree.MoveDescendants(item, false, lib.tree.rootAncestor);
            // step 3 =
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
            lib.children.Add(item, lib);
            // step 4 =
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            lib.tree.Add(item);
            // step 5 =
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
            List<RegisterItem> libtree = null;
            lib.tree.GetWholeTree(ref libtree, false);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            libchildren = null;
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
            lib.children.GetChildren(ref libchildren, false);
            List<RegisterItem> itemtree = null;
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            item.tree.GetWholeTree(ref itemtree, false);
            List<RegisterItem> itemchildren = null;
            item.children.GetChildren(ref itemchildren, false);

            // step 1 =
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            lib.children.Remove(item);
            // step 2 =
            //lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true);
            //item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false);
            //lib.tree.Remove(item);
            // step 3 =
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            BuildCustomLineageStructure(ctx, file, item);
            // we cannot move descendants to root because we cannot put tree in there.
            //item.tree.MoveDescendants(item, false, root.tree.rootAncestor);
            // step 4 =
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            root = LoadSetupRegisterItem(ctx, file, root.Id, false, false, false, false, true, true);
            root.children.Add(item, root);
            // step 5 =
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            root = LoadSetupRegisterItem(ctx, file, root.Id, false, false, false, false, true, true);
            List<RegisterItem> rootchildren = null;
            root.children.GetChildren(ref rootchildren, false);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            item.tree.GetWholeTree(ref itemtree, false);
            // step 5 =
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            root = LoadSetupRegisterItem(ctx, file, root.Id, false, false, false, false, true, true);
            root.children.Remove(item);
            // step 5 =
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            root = LoadSetupRegisterItem(ctx, file, root.Id, false, false, false, false, true, true);
            item.tree.MoveDescendants(item, true, lib.tree.rootAncestor);
            // step 5 =
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            lib.children.Add(item, lib);
            // step 5 =
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
            lib.tree.GetWholeTree(ref libtree, false);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false, true, true);
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true, true, true);
            lib.children.GetChildren(ref libchildren, false);
            return;
             */

            /*
            lib = LoadSetupRegisterItem(ctx, file, 2, true, false, true, true);
            //lib.tree.MoveDescendants(item, true, lib);
            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false);

            RegisterItem? parent = LoadSetupRegisterItem(ctx, file, item.parentId, false, false, false, false);
            parent.children.Move(item, lib);

            item = LoadSetupRegisterItem(ctx, file, item.Id, false, false, false, false);
            lib = LoadSetupRegisterItem(ctx, file, 2, false, false, false, false);
            List<RegisterItem> treelib = null;
            lib.tree.GetWholeTree(ref treelib, false);
            List<RegisterItem> children2 = null;
            item.children.GetChildren(ref children2, false);
            List<RegisterItem> itemtree = null;
            item.tree.GetWholeTree(ref itemtree, false);
            return;
            */

            /*
            RegisterItem? jrn = LoadSetupRegisterItem(ctx, file, 1, false, false, false, false);
            RegisterItem? lib = LoadSetupRegisterItem(ctx, file, 2, false, false, false, false);

            myNode? node = new myNode(true);
            RegisterItem? emptySlots = null;
            RegisterItem item = Insert(ctx, file, jrn.Id, ref emptySlots, ref node, "");
            RegisterItem item0 = item;
            RegisterItem rootAncestor = item.tree.rootAncestor;

            //item0 = LoadSetupRegisterItem(ctx, file, item0.Id, false, false, false, false);

            for (int i = 0; i < 10; i++)
            {
                //item0 = LoadSetupRegisterItem(ctx, file, item0.Id, false, false, false, false);
                //item0.tree.rootAncestor = rootAncestor;
                node = new myNode(true);
                RegisterItem item1 = Insert(ctx, file, item0.Id, ref emptySlots, ref node, "");

                //RegisterItem parent = LoadSetupRegisterItem(ctx, file, item0.Id, false, false, false, false);
                //continue;
                //item1.tree.rootAncestor = rootAncestor;
                //node = new myNode(true);
                //item1 = Insert(ctx, file, item1, ref emptySlots, ref node, "");
            }
            Register.FindNode(ctx, file, item0.Id, ref item0);

            item0 = LoadSetupRegisterItem(ctx, file, item0.Id, false, false, false, false);
            List<RegisterItem> tree = null;
            item0.tree.GetWholeTree(ref tree, false);
            List<RegisterItem> children = null;
            item0.children.GetChildren(ref children, false);

            jrn = LoadSetupRegisterItem(ctx, file, 1, false, false, false, false);
            item0 = LoadSetupRegisterItem(ctx, file, item0.Id, false, false, false, false);
            item0.tree.MoveDescendants(item0, ref lib);
            item0 = LoadSetupRegisterItem(ctx, file, item0.Id, false, false, false, false);
            jrn = LoadSetupRegisterItem(ctx, file, 1, false, false, false, false);
            lib = LoadSetupRegisterItem(ctx, file, 2, false, false, false, false);
            List<RegisterItem> treelib = null;
            lib.tree.GetWholeTree(ref treelib, false);
            List<RegisterItem> treejrn = null;
            jrn.tree.GetWholeTree(ref treejrn, false);
            List<RegisterItem> children2 = null;
            item0.children.GetChildren(ref children2, false);
            */
            return;
        }

        public static bool BuildCustomLineageStructure(OpenFSDBContext? ctx, String file, RegisterItem ancestor)
        {
            if (ctx.readOnly) return false;

            if (ancestor.Id == 0) return false; // root cannot be used for tree

            if (ancestor.treeRootId == 0)
            {
                // this ancestor is already a root child ancestor Adam, so we just initialize it with original state and do not change anything.
                ancestor.InitializeSystem(ctx, file, true, true);
                return true;
            }
            else
            {
                // this is not root child ancestor Adam but a local child in some tree of another root child ancestor Adam
                // so we remove it from it's root ancestor and create this node as custom root ancestor and import all it's true descendants from original
                // root ancestor into this custom root ancestor's lineage register
                RegisterItem? srcRootAncestor = LoadSetupRegisterItem(ctx, file, ancestor.treeRootId, false, false, false, false, true, true);
                // firstly remove the ancestor from it's original root ancestor's lineage register
                //if (!srcRootAncestor.tree.Remove(ancestor)) return false; // if error abort
                // reload states
                //srcRootAncestor = LoadSetupRegisterItem(ctx, file, srcRootAncestor.Id, false, false, false, false, true, true);
                //if (FindNode(ctx, file, ancestor.Id, ref ancestor) < 0) return false; // if error abort
                //if (ancestor == null) return false; // if error abort
                // create custom root ancesotor tree
                ancestor.tree = new LineageRegister();
                LineageRegister.InitializeCustomLineageTree(ctx, file, ancestor.tree, ancestor);
                // now migrate the true descendants from original source into this new custom lineage register
                if (!srcRootAncestor.tree.MoveDescendants(ancestor, false, ancestor)) return false; // if error abort
                if (!srcRootAncestor.tree.Remove(ancestor)) return false; // if error abort
            }
            return true;
        }
        #endregion 
    }

    public class ChildrenRegister
    {
        public RegisterItem? parent;
        public RegisterItem? current;

        // vhd handle and configuration which is to be used whenever operating this double linked list engine
        public OpenFSDBContext? ctx = null;
        public String registerFile = "";

        public ChildrenRegister()
        {
        }
        public long Count
        {
            get
            {
                return parent.childrenCount;
            }
        }

        public RegisterItem? this[int index]
        {
            get
            {
                return GetAt(index);
            }
            set
            {
                // todo
                //var node = GetAt(index);
                //if (node == null)
                //    throw new ArgumentOutOfRangeException();
                //node = value;
            }
        }

        private RegisterItem? GetAt(int index)
        {
            if (index >= parent.childrenCount) return null;
            if (parent.firstChildId == 0) return null; // there are no children so abort 
            if (current == null) current = First();

            // reset
            RegisterItem? original = current;
            current = null;

            RegisterItem? item = null;
            for (int i = 0; i < index; i++)
            {
                // calculate position in stream and directly get the node by offset/position
                item = Next();
                if (item == null) break; // end of stream abort from loop
            }
            current = original;
            return item;
        }
        public bool DeleteAt(int index, ref RegisterItem? emptySlotsItem, bool withoutEmptySlotsRegister)
        {
            if (ctx.readOnly) return false;
            if (index >= parent.childrenCount) return false;
            if (parent.firstChildId == 0) return false; // there are no children so abort 
            if (current == null) current = First();

            // reset
            RegisterItem? original = current;
            current = null;

            RegisterItem? item = null;
            for (int i = 0; i < index; i++)
            {
                // calculate position in stream and directly get the node by offset/position
                item = Next();
                if (item == null) break; // end of stream abort from loop
            }
            current = original;
            if (item != null)
                return Delete(item, ref emptySlotsItem, withoutEmptySlotsRegister);

            return false;
        }

        // this method deletes the node from parent's register and adds it to unused slots register if required
        public bool Delete(RegisterItem item, ref RegisterItem? emptySlotsItem, bool withoutEmptySlotsRegister)
        {
            /* first reload parent current config
             * if item's offset is head then this is first node
             * if item's offset is not head but tail then this is 2nd or 3rd or 2+ indexed child node
             * if item's offset is tail then this is last node
             * if item is both head and tail then this is first and one and only node
             * if item's offset is none of head and tail then this is middle node which has previous and next nodes in chain and it 
             * exists between them.
             * if item is niether head nor tail then this is middle node which is in between first and last head and tail nodes.
             * if item is middle node then delete the item and delete it's links from both next and previous nodes and join them into each other.
             * if item is tail then delete the item and remove it's link from previous node set it to 0 and update parent set it's tail previous item
             * if item is head remove the item and remove it's link from parent head set it to 0 and update parent
             * if item is head but there is a different tail, then there are 2+ items, delete the node and set next node as head and remove link from next node
             * tail can be any other node 2nd 3rd thousandth. head is first node and next to head node is the 2nd node in chain. so when we delete head node
             * then the next node becomes head. tail can be any other node it is last node even a millionth node can be tail.
             * there are only 2 nodes head and tail when tail's previous node is head by offset. when we delete the tail the previous node's next is set to 0
             * because there is no more nodes and the tail becomes the previous node.
             */

            // phase 1 - get parent's current configuration and item from register
            if (ctx.readOnly) return false;
            if (parent.firstChildId == 0) return false; // error there is no child in this parent, abort with error
            if (item.parentId != parent.Id) return false; // error this item is not in this parent

            // phase 2 - get both head and tail items
            RegisterItem? head = null;
            RegisterItem? tail = null;
            if (parent.firstChildId != 0) Register.FindNode(ctx, registerFile, parent.firstChildId, ref head);
            if (parent.lastChildId != 0) Register.FindNode(ctx, registerFile, parent.lastChildId, ref tail);

            // phase 3 - get this node's previous and next nodes in chain
            RegisterItem? prev = null;
            RegisterItem? next = null;
            if (item.previousSiblingId != 0) Register.FindNode(ctx, registerFile, item.previousSiblingId, ref prev);
            if (item.nextSiblingId != 0) Register.FindNode(ctx, registerFile, item.nextSiblingId, ref next);

            // phase 4 - decide and do
            if (item.Id == parent.firstChildId && item.Id == parent.lastChildId)
            {
                // this item is head node and one and only first node there is no other node

                // delete node and register in empty slots system node register or simply complete delete()
                RegisterItem? newEmptySlotItem = null;
                if (withoutEmptySlotsRegister)
                {
                    if (!Register.DeleteNode(ctx, registerFile, item))
                        return false; // if error abort with error
                }
                else
                {
                    if (!Register.WriteEmptySlot(ctx, registerFile, ref emptySlotsItem, item.Id, ref newEmptySlotItem))
                    {
                        return false; // error abort
                    }
                }

                // finally update parent node
                parent.firstChildId = 0;
                parent.lastChildId = 0;
                parent.childrenCount--;
                Register.UpdateNode(ctx, registerFile, parent, 0, false, 0, false, 0, false);
                // no more child node left so both head and tail are 0

            }
            else if (item.Id == parent.firstChildId && item.Id != parent.lastChildId)
            {
                // this item is head node or the first node but not the tail node, so there are 2+ nodes

                // delete node and register in empty slots system node register or simply complete delete()
                RegisterItem? newEmptySlotItem = null;
                if (withoutEmptySlotsRegister)
                {
                    if (!Register.DeleteNode(ctx, registerFile, item))
                        return false; // if error abort with error
                }
                else
                {
                    if (!Register.WriteEmptySlot(ctx, registerFile, ref emptySlotsItem, item.Id, ref newEmptySlotItem))
                    {
                        return false; // error abort
                    }
                }

                // configure this node's next linked node and update it
                next.previousSiblingId = 0; // because there is no previous node
                Register.UpdateNode(ctx, registerFile, next, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.firstChildId = next.Id; // next node becomes the first node or the head
                parent.childrenCount--;
                Register.UpdateNode(ctx, registerFile, parent, 0, false, 0, false, 0, false);

            }
            else if (item.Id == parent.lastChildId && item.Id != parent.firstChildId)
            {
                // this item is tail or the last node but there is a different head, head and tail 2 or more nodes means there are
                // 2+ nodes, tail can be 2nd node or 3rd or 3+ node

                // delete node and register in empty slots system node register or simply complete delete()
                RegisterItem? newEmptySlotItem = null;
                if (withoutEmptySlotsRegister)
                {
                    if (!Register.DeleteNode(ctx, registerFile, item))
                        return false; // if error abort with error
                }
                else
                {
                    if (!Register.WriteEmptySlot(ctx, registerFile, ref emptySlotsItem, item.Id, ref newEmptySlotItem))
                    {
                        return false; // error abort
                    }
                }

                // we update the previous node in chain remove this node's link and set the previous node as tail
                prev.nextSiblingId = 0; // because previous node becomes tail or the last node and there is no next node
                Register.UpdateNode(ctx, registerFile, prev, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.lastChildId = prev.Id; // prev node becomes the last node or the tail. it is also already head if it is first node
                parent.childrenCount--;
                Register.UpdateNode(ctx, registerFile, parent, 0, false, 0, false, 0, false);
            }
            else
            {
                // this item is neither head nor tail node means this node is a middle node which exists between the node chain

                // delete node and register in empty slots system node register or simply complete delete()
                RegisterItem? newEmptySlotItem = null;
                if (withoutEmptySlotsRegister)
                {
                    if (!Register.DeleteNode(ctx, registerFile, item))
                        return false; // if error abort with error
                }
                else
                {
                    if (!Register.WriteEmptySlot(ctx, registerFile, ref emptySlotsItem, item.Id, ref newEmptySlotItem))
                    {
                        return false; // error abort
                    }
                }

                // now join both prev and next nodes into each other and remove the deleted node links
                prev.nextSiblingId = next.Id; // previous's next is deleted node's next, we join both
                next.previousSiblingId = prev.Id; // next's previous is deleted node's previous, we join both
                // deleted node's links removed now update both nodes
                Register.UpdateNode(ctx, registerFile, next, 0, false, 0, false, 0, false);
                Register.UpdateNode(ctx, registerFile, prev, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.childrenCount--;
                Register.UpdateNode(ctx, registerFile, parent, 0, false, 0, false, 0, false);
            }
            return true;
        }
        public bool Remove(RegisterItem item)
        {
            /* first reload parent current config
             * if item's offset is head then this is first node
             * if item's offset is not head but tail then this is 2nd or 3rd or 2+ indexed child node
             * if item's offset is tail then this is last node
             * if item is both head and tail then this is first and one and only node
             * if item's offset is none of head and tail then this is middle node which has previous and next nodes in chain and it 
             * exists between them.
             * if item is niether head nor tail then this is middle node which is in between first and last head and tail nodes.
             * if item is middle node then delete the item and delete it's links from both next and previous nodes and join them into each other.
             * if item is tail then delete the item and remove it's link from previous node set it to 0 and update parent set it's tail previous item
             * if item is head remove the item and remove it's link from parent head set it to 0 and update parent
             * if item is head but there is a different tail, then there are 2+ items, delete the node and set next node as head and remove link from next node
             * tail can be any other node 2nd 3rd thousandth. head is first node and next to head node is the 2nd node in chain. so when we delete head node
             * then the next node becomes head. tail can be any other node it is last node even a millionth node can be tail.
             * there are only 2 nodes head and tail when tail's previous node is head by offset. when we delete the tail the previous node's next is set to 0
             * because there is no more nodes and the tail becomes the previous node.
             */

            // phase 1 - get parent's current configuration and item from register
            if (ctx.readOnly) return false;
            if (parent.firstChildId == 0) return false; // error there is no child in this parent, abort with error
            if (item.parentId != parent.Id) return false; // error this item is not in this parent

            // phase 2 - get both head and tail items
            RegisterItem? head = null;
            RegisterItem? tail = null;
            if (parent.firstChildId != 0) Register.FindNode(ctx, registerFile, parent.firstChildId, ref head);
            if (parent.lastChildId != 0) Register.FindNode(ctx, registerFile, parent.lastChildId, ref tail);

            // phase 3 - get this node's previous and next nodes in chain
            RegisterItem? prev = null;
            RegisterItem? next = null;
            if (item.previousSiblingId != 0) Register.FindNode(ctx, registerFile, item.previousSiblingId, ref prev);
            if (item.nextSiblingId != 0) Register.FindNode(ctx, registerFile, item.nextSiblingId, ref next);

            // phase 4 - decide and do
            if (item.Id == parent.firstChildId && item.Id == parent.lastChildId)
            {
                // this item is head node and one and only first node there is no other node

                // first edit the item
                item.parentId = -1;
                item.previousSiblingId = item.nextSiblingId = 0;
                Register.UpdateNode(ctx, registerFile, item, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.firstChildId = 0;
                parent.lastChildId = 0;
                parent.childrenCount--;
                Register.UpdateNode(ctx, registerFile, parent, 0, false, 0, false, 0, false);
                // no more child node left so both head and tail are 0

            }
            else if (item.Id == parent.firstChildId && item.Id != parent.lastChildId)
            {
                // this item is head node or the first node but not the tail node, so there are 2+ nodes

                // first edit the item
                item.parentId = -1;
                item.previousSiblingId = item.nextSiblingId = 0;
                Register.UpdateNode(ctx, registerFile, item, 0, false, 0, false, 0, false);

                // configure this node's next linked node and update it
                next.previousSiblingId = 0; // because there is no previous node
                Register.UpdateNode(ctx, registerFile, next, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.firstChildId = next.Id; // next node becomes the first node or the head
                parent.childrenCount--;
                Register.UpdateNode(ctx, registerFile, parent, 0, false, 0, false, 0, false);

            }
            else if (item.Id == parent.lastChildId && item.Id != parent.firstChildId)
            {
                // this item is tail or the last node but there is a different head, head and tail 2 or more nodes means there are
                // 2+ nodes, tail can be 2nd node or 3rd or 3+ node

                // first edit the item
                item.parentId = -1;
                item.previousSiblingId = item.nextSiblingId = 0;
                Register.UpdateNode(ctx, registerFile, item, 0, false, 0, false, 0, false);

                // we update the previous node in chain remove this node's link and set the previous node as tail
                prev.nextSiblingId = 0; // because previous node becomes tail or the last node and there is no next node
                Register.UpdateNode(ctx, registerFile, prev, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.lastChildId = prev.Id; // prev node becomes the last node or the tail. it is also already head if it is first node
                parent.childrenCount--;
                Register.UpdateNode(ctx, registerFile, parent, 0, false, 0, false, 0, false);
            }
            else
            {
                // this item is neither head nor tail node means this node is a middle node which exists between the node chain

                // first edit the item
                item.parentId = -1;
                item.previousSiblingId = item.nextSiblingId = 0;
                Register.UpdateNode(ctx, registerFile, item, 0, false, 0, false, 0, false);

                // now join both prev and next nodes into each other and remove the deleted node links
                prev.nextSiblingId = next.Id; // previous's next is deleted node's next, we join both
                next.previousSiblingId = prev.Id; // next's previous is deleted node's previous, we join both
                // deleted node's links removed now update both nodes
                Register.UpdateNode(ctx, registerFile, next, 0, false, 0, false, 0, false);
                Register.UpdateNode(ctx, registerFile, prev, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.childrenCount--;
                Register.UpdateNode(ctx, registerFile, parent, 0, false, 0, false, 0, false);
            }
            return true;
        }
        // this method removes the item from this parent and inserts/moves it to the another parent
        public bool Move(RegisterItem item, RegisterItem targetParent)
        {
            // validation
            if (ctx.readOnly) return false;
            if (parent.firstChildId == 0) return false; // error there is no child in this parent, abort with error
            if (item.parentId != parent.Id) return false; // error this item is not in this parent

            // validations
            if (targetParent.childrenCount >= Register.default_maxChildrenNodes)
                return false;

            // firstly remove the item from this parent
            if (!Remove(item)) return false; // error item not removed from parent

            // nextly insert this item into target parent
            return targetParent.children.Add(item, targetParent);
        }
        // this method inserts a node in the registry by id
        public RegisterItem? Add(myNode node, ref RegisterItem? parentNewStateOut)
        {
            if (ctx.readOnly) return null;
            if (node.chapter == null) return null;

            // finally update the register add this node
            RegisterItem? item = new RegisterItem(0, node.chapter.Id,
                parent.Id, node.DirectorySectionID, 0, 0, 0, 0, 0,
                node.chapter.nodeType, node.chapter.specialNodeType, node.chapter.domainType, RegisterItemFlags1.None, 0, 0, 0, 0, 0, 0);
            if (Add(item, parentNewStateOut))
                return item;
            else
                return null;
        }

        public bool Add(RegisterItem? item, RegisterItem? parent)
        {
            // if 1st child, insert at head and tail, update
            // if 2nd forth child, insert at tail, update tail
            // update parent's configuration - first if 1st node, or last child if last last node
            // update previous node's next node offset to this next node.
            if (ctx.readOnly) return false;

            // validations
            //if (parent.childrenCount >= Register.default_maxChildrenNodes)
            //    return false;

            this.parent = parent;

            if (parent.childrenCount == 0)
            {
                // this is first child being inserted, so insert it and it's offset as head and tail both
                item.parentId = parent.Id;
                item.previousSiblingId = 0;
                item.nextSiblingId = 0;
                Int64 newItemOffset = Register.InsertNode(ctx, registerFile, item);
                if (newItemOffset < 0) return false; // critical error abort with error

                // finally update parent node
                parent.firstChildId = item.Id;
                parent.lastChildId = item.Id;
                parent.childrenCount++;
                Register.UpdateNode(ctx, registerFile, parent, 0, false, 0, false, 0, false);
                // done
            }
            else if (parent.childrenCount == 1)
            {
                // get head
                RegisterItem? head = null;
                Int64 headOffset = Register.FindNode(ctx, registerFile, parent.firstChildId, ref head);
                if (headOffset < 0) return false; // critical error abort with error

                // this is a 2nd node to be inserted, there is 1 first node which is head. so we set tail as this node.
                item.parentId = parent.Id;
                item.previousSiblingId = head.Id;
                item.nextSiblingId = 0;
                Int64 newItemOffset = Register.InsertNode(ctx, registerFile, item);
                if (newItemOffset < 0) return false; // critical error abort with error

                // update previous head node 
                head.nextSiblingId = item.Id;
                Register.UpdateNode(ctx, registerFile, head, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.lastChildId = item.Id;
                parent.childrenCount++;
                Register.UpdateNode(ctx, registerFile, parent, 0, false, 0, false, 0, false);
            }
            else
            {
                // this is 3+ index child so insert it as tail and update the previous tail cum last item with this new item
                // get tail
                RegisterItem? tail = null;
                Int64 tailOffset = Register.FindNode(ctx, registerFile, parent.lastChildId, ref tail);
                if (tailOffset < 0) return false; // critical error abort with error

                // update and insert this node with new configuration
                item.previousSiblingId = tail.Id;
                item.nextSiblingId = 0;
                item.parentId = parent.Id;
                Int64 newItemOffset = Register.InsertNode(ctx, registerFile, item);
                if (newItemOffset < 0) return false; // critcial error abort with error

                // update previous tail node 
                tail.nextSiblingId = item.Id;
                Register.UpdateNode(ctx, registerFile, tail, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.lastChildId = item.Id;
                parent.childrenCount++;
                Register.UpdateNode(ctx, registerFile, parent, 0, false, 0, false, 0, false);
                // done
            }

            // success
            return true;
        }
        public void Reset()
        {
            current = null;
        }

        public RegisterItem? First()
        {
            if (parent.firstChildId == 0) return null;
            Register.FindNode(ctx, registerFile, parent.firstChildId, ref current);
            return current;
        }
        public RegisterItem? Last()
        {
            if (parent.lastChildId == 0) return null;
            Register.FindNode(ctx, registerFile, parent.lastChildId, ref current);
            return current;
        }
        
        // we get all children
        public bool GetChildren(ref List<RegisterItem>? listOut, bool loadNode)
        {
            Int64 nextSiblingId = -1;

            RegisterItem? current = null;

            List<RegisterItem>? list = new List<RegisterItem>();
            listOut = list;

            // we must reload to get latest configuration of the parent
            reloadConfig();

            if (parent.firstChildId == 0) return true; // there are no children so abort and return empty list

            // get the head
            if (Register.FindNode(ctx, registerFile, parent.firstChildId, ref current) < 0)
                return false; // end of chain or error or no children present in parent, abort

            if (current == null)
                return false; // there is no child node present so abort

            nextSiblingId = current.Id;

            while (true)
            {
                // load item from registry
                RegisterItem? nextItem = null;
                Int64 nextOffset = Register.FindNode(ctx, registerFile, nextSiblingId, ref nextItem);
                if (nextOffset < 0) break; // error or end of stream or end of register so break
                if (nextItem == null) break; // no more items so break

                String rtf = "";
                byte[]? xamlbytesOut = null;
                if (loadNode) 
                    nextItem.loadNode(ctx, ref rtf, ref xamlbytesOut, false);

                // add item to list
                list.Add(nextItem);

                // configure
                nextSiblingId = nextItem.nextSiblingId;

                // check if end of children register reached
                if (nextItem.nextSiblingId == 0) break;
            }
            // output
            listOut = list;
            return true;
        }

        // we get next demanded cache of sequence of items from register from the current item passed as parameter
        public RegisterItem? NextCache(int total, RegisterItem? current, ref List<RegisterItem>? listOut)
        {
            Int64 nextSiblingId = -1;
            if (total < 1) total = 1000; // auto set total 1000 if user does not passes param total

            List<RegisterItem>? list = new List<RegisterItem>();
            listOut = list;

            // we must reload to get latest configuration of the parent
            reloadConfig();

            if (parent.firstChildId == 0) return null; // there are no children so abort and return empty list

            if (current == null)
            {
                // current is not loaded, so get first head node as current
                if (Register.FindNode(ctx, registerFile, parent.firstChildId, ref current) < 0)
                    return null; // end of chain or error or no children present in parent, abort

                nextSiblingId = current.Id;
            }
            else
            {
                // current is already loaded so proceed with it's next sibling
                if (current.nextSiblingId == 0) return null; // end of chain, abort

                // get next sibling offset
                nextSiblingId = current.nextSiblingId;
            }

            if (current == null)
                return null; // there is no child node present so abort

            // we take next 1000 or cache number nodes if available 1000 or cache or more, otherwise whatever we can get less then 1000 and return the list
            // with the last node as returned tail
            RegisterItem? tail = null; // last item which was found

            for (int i = 0; i < total; i++)
            {
                // load item from registry
                RegisterItem? nextItem = null;
                Int64 nextOffset = Register.FindNode(ctx, registerFile, nextSiblingId, ref nextItem);
                if (nextOffset < 0) break; // error or end of stream or end of register so break
                if (nextItem == null) break; // no more items so break

                // set tail this next item because it was found
                tail = nextItem;

                // configure
                nextSiblingId = nextItem.nextSiblingId;

                // add item to list
                list.Add(nextItem);
            }
            // output
            listOut = list;
            return tail;
        }

        public RegisterItem? Next()
        {
            Int64 nextSiblingId = -1;

            if (parent.firstChildId == 0) return null; // there are no children so abort and return empty list

            if (current == null)
            {
                // current is not loaded, so get first head node as current
                //current = Register.LoadSetupRegisterItem(ctx, registerFile, parent.firstChildId, false, false, false);
                //if (current == null)
                if (Register.FindNode(ctx, registerFile, parent.firstChildId, ref current) < 0)
                    return null; // end of chain or error or no children present in parent, abort

                nextSiblingId = current.Id;
            }
            else
            {
                // current is already loaded so proceed with it's next sibling
                if (current.nextSiblingId == 0) return null; // end of chain, abort

                // get next sibling offset
                nextSiblingId = current.nextSiblingId;
            }

            if (current == null)
                return null; // there is no child node present so abort

            // load item from registry
            RegisterItem? nextItem = null;
            //nextItem = Register.LoadSetupRegisterItem(ctx, registerFile, nextSiblingId, false, false, false);
            //if (nextItem == null)
            //    return null; // critical error abort with null

            Int64 nextOffset = Register.FindNode(ctx, registerFile, nextSiblingId, ref nextItem);
            if (nextOffset < 0) return null; // critical error abort with null

            // reconfigure current and set next item into it and return it as the next item in register
            current = nextItem;
            return current;
        }

        public RegisterItem? Previous()
        {
            Int64 prevSiblingId = -1;

            if (parent.firstChildId == 0) return null; // there are no children so abort and return empty list

            if (current == null)
            {
                // current is not loaded, so get last tail node as current
                //current = Register.LoadSetupRegisterItem(ctx, registerFile, parent.lastChildId, false, false, false);
                //if (current == null)
                if (Register.FindNode(ctx, registerFile, parent.lastChildId, ref current) < 0)
                    return null; // end of chain or error or no children present in parent, abort

                prevSiblingId = current.Id;
            }
            else
            {
                // current is already loaded so proceed with it's previous sibling
                if (current.previousSiblingId == 0) return null; // end of chain, abort

                // get prev offset
                prevSiblingId = current.previousSiblingId;
            }

            if (current == null)
                return null; // there is no child node present so abort

            // load item from registry
            RegisterItem? prevItem = null;
            //prevItem = Register.LoadSetupRegisterItem(ctx, registerFile, prevSiblingId, false, false, false);
            //if (prevItem == null)
            //    return null; // critical error abort with null

            Int64 prevOffset = Register.FindNode(ctx, registerFile, prevSiblingId, ref prevItem);
            if (prevOffset < 0) return null; // critical error abort with null

            // reconfigure current and set prev item into it and return it as the prev item in register
            current = prevItem;
            return current;
        }

        public bool reloadConfig()
        {
            // original: //parent = Register.LoadSetupRegisterItem(ctx, registerFile, parent.Id, false, false, false);
            //if (parent == null) return false;
            if (Register.FindNode(ctx, registerFile, parent.Id, ref parent) < 0) return false; // critical error
            if (parent == null) return false; // critical error

            return true;
        }
        public void SetLatestState(OpenFSDBContext? ctx, String registerFile, RegisterItem state, bool reset)
        {
            // configure and setup
            this.ctx = ctx;
            this.registerFile = registerFile;
            if (reset) this.current = null;
            this.parent = state;
        }
        public void SetLatestState(RegisterItem state, bool reset)
        {
            // configure and setup
            if (reset) this.current = null;
            this.parent = state;
        }

        public static bool Initialize(OpenFSDBContext? ctx, String registerFile,
            ChildrenRegister list, RegisterItem parentItem)
        {
            // phase 1 - configure and setup

            list.ctx = ctx;
            list.registerFile = registerFile;
            list.current = null;

            // directly setup count from parent item
            list.parent = parentItem;
            return true;
        }

    }

    public class LineageRegister
    {
        /*
         * 30 December 2024, 03-37-am: technical knowledge
         * we can do endless recursion from the root child node throughout the tree which is sequentially
         * chained into the root child node. root child node or the Adam and entire tree exists on Adam 
         * even if it contains millions of nodes. we use absolute ids as binding links. root child node Adam 
         * contains lineageHead and lineageTail and lineageNextNode and lineagePreviousNode and lineageNodesCount, 
         * these properties define the doubly linked list chain same or similar as in ChildrenRegister. but it should be exaustive and 
         * intensive in operations as we need to iterate through every lineage node. sequential id based chaining.
         * the truth is that a child node anywhere in the lineage tree cannot be created unless it's parent and all ancestors truly exist.
         * in this way all tree is binded as a sequential chain. to delete any node anywhere then we remove it from it's parent and from the lineage most properly
         * then finally delete-empty the node and or register it with empty slots register if demanded. to insert the node the node is 
         * added in the very last id whatever available and is inserted last in parent as tail and in lineage as tail.
         * to get all the ancestors the node is traversed from bottom to top by jumping from ancestor (parent) to ancestor (parent) right to the top
         * root child or the Adam. in this way only we can traverse the lineage. we may add root child Adam's id as root property in all the lineage nodes.
         * to delete a child item recursively involves validating all lineage below the item that every child has this item as ancestor, then we delete the item
         * by first removing the children from parent and then from lineage and then finally delete-empty the children.
         * to move from this lineage to another lineage, parent is removed from lineage and added into tail of the other lineage first of all. 
         * then starting from head which is property of root child Adam node, we traverse all the tree down to the tail, and which ever node is 
         * first in sequence and exists under the parent node is delinked-removed from lineage and inserted to the other lineage at tail. this is
         * a proper sequence. we grab the next and previous nodes before removing the node from lineage, then we rewind to the previous node and proceed with latest 
         * config and traverse all lineage upto the tail. whatsoever node which is not child or descendant of target parent and is external to the parent is 
         * not modified and remains unchanged and unaffected.
         * when we insert new node it is inserted at the tail and root child's head and or tail are affected and changed.
         * */
        public RegisterItem? rootAncestor;
        public RegisterItem? parent;
        public RegisterItem? current;

        // vhd handle and configuration which is to be used whenever operating this double linked list engine
        public OpenFSDBContext? ctx = null;
        public String registerFile = "";

        public LineageRegister()
        {
        }
        public long Count
        {
            get
            {
                return rootAncestor.descendantsCount;
            }
        }
        public void Reset()
        {
            current = null;
        }

        public void autoFill()
        {

        }

        public bool Remove(RegisterItem item)
        {
            /* first reload parent current config
             * if item's offset is head then this is first node
             * if item's offset is not head but tail then this is 2nd or 3rd or 2+ indexed child node
             * if item's offset is tail then this is last node
             * if item is both head and tail then this is first and one and only node
             * if item's offset is none of head and tail then this is middle node which has previous and next nodes in chain and it 
             * exists between them.
             * if item is niether head nor tail then this is middle node which is in between first and last head and tail nodes.
             * if item is middle node then delete the item and delete it's links from both next and previous nodes and join them into each other.
             * if item is tail then delete the item and remove it's link from previous node set it to 0 and update parent set it's tail previous item
             * if item is head remove the item and remove it's link from parent head set it to 0 and update parent
             * if item is head but there is a different tail, then there are 2+ items, delete the node and set next node as head and remove link from next node
             * tail can be any other node 2nd 3rd thousandth. head is first node and next to head node is the 2nd node in chain. so when we delete head node
             * then the next node becomes head. tail can be any other node it is last node even a millionth node can be tail.
             * there are only 2 nodes head and tail when tail's previous node is head by offset. when we delete the tail the previous node's next is set to 0
             * because there is no more nodes and the tail becomes the previous node.
             */

            // phase 1 - get parent's current configuration and item from register
            if (ctx.readOnly) return false;
            if (rootAncestor.treeHeadId == 0) return false; // error there is no child in this parent, abort with error
            if (item.treeRootId != rootAncestor.Id) return false; // error this item is not in this parent

            // phase 2 - get both head and tail items
            RegisterItem? head = null;
            RegisterItem? tail = null;
            if (rootAncestor.treeHeadId != 0) Register.FindNode(ctx, registerFile, rootAncestor.treeHeadId, ref head);
            if (rootAncestor.treeTailId != 0) Register.FindNode(ctx, registerFile, rootAncestor.treeTailId, ref tail);

            // phase 3 - get this node's previous and next nodes in chain
            RegisterItem? prev = null;
            RegisterItem? next = null;
            if (item.previousDescendantId != 0) Register.FindNode(ctx, registerFile, item.previousDescendantId, ref prev);
            if (item.nextDescendantId != 0) Register.FindNode(ctx, registerFile, item.nextDescendantId, ref next);

            // phase 4 - decide and do
            if (item.Id == rootAncestor.treeHeadId && item.Id == rootAncestor.treeTailId)
            {
                // this item is head node and one and only first node there is no other node

                // first edit the item
                item.treeRootId = -1;
                item.previousDescendantId = item.nextDescendantId = 0;
                Register.UpdateNode(ctx, registerFile, item, 0, false, 0, false, 0, false);

                // finally update parent node
                rootAncestor.treeHeadId = 0;
                rootAncestor.treeTailId = 0;
                rootAncestor.descendantsCount--;
                Register.UpdateNode(ctx, registerFile, rootAncestor, 0, false, 0, false, 0, false);
                // no more child node left so both head and tail are 0

            }
            else if (item.Id == rootAncestor.treeHeadId && item.Id != rootAncestor.treeTailId)
            {
                // this item is head node or the first node but not the tail node, so there are 2+ nodes

                // first edit the item
                item.treeRootId = -1;
                item.previousDescendantId = item.nextDescendantId = 0;
                Register.UpdateNode(ctx, registerFile, item, 0, false, 0, false, 0, false);

                // configure this node's next linked node and update it
                next.previousDescendantId = 0; // because there is no previous node
                Register.UpdateNode(ctx, registerFile, next, 0, false, 0, false, 0, false);

                // finally update parent node
                rootAncestor.treeHeadId = next.Id; // next node becomes the first node or the head
                rootAncestor.descendantsCount--;
                Register.UpdateNode(ctx, registerFile, rootAncestor, 0, false, 0, false, 0, false);

            }
            else if (item.Id == rootAncestor.treeTailId && item.Id != rootAncestor.treeHeadId)
            {
                // this item is tail or the last node but there is a different head, head and tail 2 or more nodes means there are
                // 2+ nodes, tail can be 2nd node or 3rd or 3+ node

                // first edit the item
                item.treeRootId = -1;
                item.previousDescendantId = item.nextDescendantId = 0;
                Register.UpdateNode(ctx, registerFile, item, 0, false, 0, false, 0, false);

                // we update the previous node in chain remove this node's link and set the previous node as tail
                prev.nextDescendantId = 0; // because previous node becomes tail or the last node and there is no next node
                Register.UpdateNode(ctx, registerFile, prev, 0, false, 0, false, 0, false);

                // finally update parent node
                rootAncestor.treeTailId = prev.Id; // prev node becomes the last node or the tail. it is also already head if it is first node
                rootAncestor.descendantsCount--;
                Register.UpdateNode(ctx, registerFile, rootAncestor, 0, false, 0, false, 0, false);
            }
            else
            {
                // this item is neither head nor tail node means this node is a middle node which exists between the node chain

                // first edit the item
                item.treeRootId = -1;
                item.previousDescendantId = item.nextDescendantId = 0;
                Register.UpdateNode(ctx, registerFile, item, 0, false, 0, false, 0, false);

                // now join both prev and next nodes into each other and remove the deleted node links
                prev.nextDescendantId = next.Id; // previous's next is deleted node's next, we join both
                next.previousDescendantId = prev.Id; // next's previous is deleted node's previous, we join both
                // deleted node's links removed now update both nodes
                Register.UpdateNode(ctx, registerFile, next, 0, false, 0, false, 0, false);
                Register.UpdateNode(ctx, registerFile, prev, 0, false, 0, false, 0, false);

                // finally update parent node
                rootAncestor.descendantsCount--;
                Register.UpdateNode(ctx, registerFile, rootAncestor, 0, false, 0, false, 0, false);
            }
            return true;
        }
        // this method removes the item from this root ancestor and inserts/moves it to the another root ancestor
        public bool Move(RegisterItem item, ref RegisterItem targetRootAncestor)
        {
            // validation
            if (ctx.readOnly) return false;
            if (rootAncestor.treeHeadId == 0) return false; // error there is no child in this parent, abort with error
            if (item.treeRootId != rootAncestor.Id) return false; // error this item is not in this parent

            // firstly remove the item from this parent
            if (!Remove(item)) return false; // error item not removed from parent

            // nextly insert this item into target parent
            return targetRootAncestor.tree.Add(item);
        }
        // this special method removes the root ancestor item and inserts/moves it to the another root ancestor
        public bool MoveRootAncestor(ref RegisterItem targetRootAncestor)
        {
            // firstly remove the item from this parent
            if (ctx.readOnly) return false;
            rootAncestor.treeRootId = -1;
            rootAncestor.previousDescendantId = rootAncestor.nextDescendantId = rootAncestor.treeHeadId = rootAncestor.treeTailId = 
                rootAncestor.descendantsCount = 0;
            Register.UpdateNode(ctx, registerFile, rootAncestor, 0, false, 0, false, 0, false);

            // nextly insert this item into target parent
            return targetRootAncestor.tree.Add(rootAncestor);
        }

        public bool Add(RegisterItem? item)
        {
            // if 1st child, insert at head and tail, update
            // if 2nd forth child, insert at tail, update tail
            // update parent's configuration - first if 1st node, or last child if last last node
            // update previous node's next node offset to this next node.
            //this.rootAncestor = rootAncestor;
            //this.parent = parent;

            if (ctx.readOnly) return false;
            if (rootAncestor.descendantsCount == 0)
            {
                // this is first child being inserted in the lineage chain, so insert it and it's offset as head and tail both
                item.treeRootId = rootAncestor.Id;
                item.previousDescendantId = 0;
                item.nextDescendantId = 0;
                item.treeHeadId = item.treeTailId = item.descendantsCount = 0;
                Register.UpdateNode(ctx, registerFile, item, 0, false, 0, false, 0, false);

                // finally update parent node
                rootAncestor.treeHeadId = item.Id;
                rootAncestor.treeTailId = item.Id;
                rootAncestor.descendantsCount++;
                Register.UpdateNode(ctx, registerFile, rootAncestor, 0, false, 0, false, 0, false);
                // done
            }
            else if (rootAncestor.descendantsCount == 1)
            {
                // get head
                RegisterItem? head = null;
                Int64 headOffset = Register.FindNode(ctx, registerFile, rootAncestor.treeHeadId, ref head);
                if (headOffset < 0) return false; // critical error abort with error

                // this is a 2nd node to be inserted, there is 1 first node which is head. so we set tail as this node.
                item.treeRootId = rootAncestor.Id;
                item.previousDescendantId = head.Id;
                item.nextDescendantId = 0;
                item.treeHeadId = item.treeTailId = item.descendantsCount = 0;
                Register.UpdateNode(ctx, registerFile, item, 0, false, 0, false, 0, false);

                // update previous head node 
                head.nextDescendantId = item.Id;
                Register.UpdateNode(ctx, registerFile, head, 0, false, 0, false, 0, false);

                // finally update parent node
                rootAncestor.treeTailId = item.Id;
                rootAncestor.descendantsCount++;
                Register.UpdateNode(ctx, registerFile, rootAncestor, 0, false, 0, false, 0, false);
            }
            else
            {
                // this is 2nd or any other 2+ index child so insert it as tail and update the previous tail cum last item with this new item
                // get tail
                RegisterItem? tail = null;
                Int64 tailOffset = Register.FindNode(ctx, registerFile, rootAncestor.treeTailId, ref tail);
                if (tailOffset < 0) return false; // critical error abort with error

                // update this node with new configuration
                item.previousDescendantId = tail.Id;
                item.nextDescendantId = 0;
                item.treeRootId = rootAncestor.Id;
                item.treeHeadId = item.treeTailId = item.descendantsCount = 0;
                Register.UpdateNode(ctx, registerFile, item, 0, false, 0, false, 0, false);

                // update previous tail node 
                tail.nextDescendantId = item.Id;
                Register.UpdateNode(ctx, registerFile, tail, 0, false, 0, false, 0, false);

                // finally update parent node
                rootAncestor.treeTailId = item.Id;
                rootAncestor.descendantsCount++;
                Register.UpdateNode(ctx, registerFile, rootAncestor, 0, false, 0, false, 0, false);
                // done
            }
            // success
            return true;
        }
        public bool IsDescendantOfAncestor(RegisterItem? item, RegisterItem? ancestor)
        {
            List<RegisterItem>? lineage = null;
            Register.Lineage(ctx, registerFile, item, ref lineage, true, false, false);
            RegisterItem? found = lineage.Find(x => x.Id == ancestor.Id);
            if (found != null)
            {
                // yes current item is descendant of ancestor
                return true;
            }
            // no current item is not a descendant of ancestor
            return false;
        }

        // traverses the tree beginning from root child node and finds an ancestor's all descendants recursively
        public bool MoveDescendants(RegisterItem? ancestor, bool moveAncestor, RegisterItem? targetRootAncestor)
        {
            // reload latest states
            if (ctx.readOnly) return false;
            rootAncestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, rootAncestor.Id, false, false, false, false, true, true);
            ancestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, ancestor.Id, false, false, false, false, true, true);

            if (rootAncestor.treeHeadId == 0)
            {
                // there are no children so abort and return empty list
                if (rootAncestor.Id == ancestor.Id) return true; // ancestor is root ancestor itself and there are no children in it, so return true
                return false; // ancestor is not root ancestor and root ancestor has no children, so return error
            }
            if (ancestor.treeRootId == targetRootAncestor.Id) return true; // we cannot move from and to same root child ancestor

            Int64 nextDescendantId = -1;

            RegisterItem? current = null;

            // current is not loaded, so get first head node as current
            if (Register.FindNode(ctx, registerFile, rootAncestor.treeHeadId, ref current) < 0)
                return false; // end of chain or error or no children present in parent, abort

            if (current == null)
                return false; // there is no child node present so abort

            nextDescendantId = current.Id;

            /* new unneccesary illogical code ignore 18 Nov 2025
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            BinaryReader br = new BinaryReader(ms);

            // new code - 18 Nov 2025
            while (true)
            {

                RegisterItem? nextItem = null;
                Int64 nextOffset = Register.FindNode(ctx, registerFile, nextDescendantId, ref nextItem);
                if (nextOffset < 0)
                    break; // error or end of stream or end of register so break
                if (nextItem == null)
                    break; // no more items so break

                // configure
                nextDescendantId = nextItem.nextDescendantId;

                if (IsDescendantOfAncestor(nextItem, ancestor))
                {
                    // this descendant is ancestor's true descendant, so queue it
                    bw.Write(nextItem.Id);
                }

                // check if end of register reached
                if (nextDescendantId == 0)
                    break;
            }

            br.BaseStream.Position = 0;
            while (true)
            {
                RegisterItem? nextItem = null;
                if (br.PeekChar() == -1) break;
                Int64 id = br.ReadInt64();
                Int64 nextOffset = Register.FindNode(ctx, registerFile, id, ref nextItem);
                if (nextOffset < 0)
                    break; // error or end of stream or end of register so break
                if (nextItem == null)
                    break; // no more items so break

                // this descendant is ancestor's true descendant, so move it to the target root ancestor
                if (!Move(nextItem, ref targetRootAncestor))
                    return false; // critical error


            }
            */

            while (true)
            {
                // load item from registry
                RegisterItem? nextItem = null;
                Int64 nextOffset = Register.FindNode(ctx, registerFile, nextDescendantId, ref nextItem);
                if (nextOffset < 0)
                    break; // error or end of stream or end of register so break
                if (nextItem == null)
                    break; // no more items so break

                // configure
                nextDescendantId = nextItem.nextDescendantId;

                if (IsDescendantOfAncestor(nextItem, ancestor))
                {
                    // this descendant is ancestor's true descendant, so move it to the target root ancestor
                    if (!Move(nextItem, ref targetRootAncestor))
                        return false; // critical error
                }

                // check if end of register reached
                if (nextDescendantId == 0)
                    break;
            }
            
            // reload latest states
            rootAncestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, rootAncestor.Id, false, false, false, false, true, true);
            ancestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, ancestor.Id, false, false, false, false, true, true);

            // we cannot move ancestor at first we need to move all it's true descendants recursively.
            // if ancestor is itself a root child Adam, then we cannot move ancestor at first because it destroys the lineage chain's 
            // configuration if we move the node to another root child Adam's tree. so we need to first move all descendants, then in the last 
            // the ancestor.
            if (moveAncestor)
            {
                if (ancestor.Id == rootAncestor.Id)
                {
                    // ancestor is root ancestor itself, so we move it with special function
                    return MoveRootAncestor(ref targetRootAncestor);
                }
                else
                {
                    // ancestor is not root ancestor but a child in it's tree so we move it with common function
                    if (!Move(ancestor, ref targetRootAncestor))
                        return false; // critical error
                }
            }
            //current = original;
            return true;
        }
        // this special method removes the root ancestor item from root and deletes it 
        public bool DeleteRootAncestor(ref RegisterItem? emptySlots, bool withoutEmptySlotsRegister)
        {
            // reload latest states
            if (ctx.readOnly) return false;
            rootAncestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, rootAncestor.Id, false, false, false, false, true, true);
            if (rootAncestor == null) return false; // critical error abort all op
            if (rootAncestor.specialNodeType == SpecialNodeType.SystemNode) return false;

            // load node
            String rtf = "";
            byte[]? xamlbytesOut = null;
            if (!rootAncestor.loadNode(ctx, ref rtf, ref xamlbytesOut, false)) return false; // abort all op if error loading the node

            // node loaded successfully, proceed
            // first delete the node files
            if (!entryMethods.DBPurgeNodeOFSDB(ctx, rootAncestor.node)) return false; // critical error then return error before taking any other action

            // parent root 0 has no lineage tree, so we skip it

            // load latest parent state
            // root ancestor's parent is absolute root 0
            RegisterItem? parent = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, rootAncestor.parentId, false, false, false, false, true, true);
            if (parent == null) return false; // critical error abort all op

            // parent root 0 loaded successfully, proceed
            // third delete the item from parent
            bool result = parent.children.Delete(rootAncestor, ref emptySlots, withoutEmptySlotsRegister);
            return result;
        }

        // this method deletes the item from this root ancestor and inserts it to the empty slots register if demanded
        public bool DeleteDescendant(RegisterItem item, ref RegisterItem? emptySlots, bool withoutEmptySlotsRegister, bool removeFromParent)
        {
            // validation
            if (ctx.readOnly) return false;
            if (item.Id == 0) return false; // cannot delete root node
            if (rootAncestor.treeHeadId == 0) return false; // error there is no child in this parent, abort with error
            if (item.treeRootId != rootAncestor.Id) return false; // error this item is not in this parent
            if (item.specialNodeType == SpecialNodeType.SystemNode) return false; // error cannot purge system node

            // load node
            String rtf = "";
            byte[]? xamlbytesOut = null;
            if (!item.loadNode(ctx, ref rtf, ref xamlbytesOut, false)) return false; // abort all op if error loading the node

            // node loaded successfully, proceed
            // now delete the node files
            if (!entryMethods.DBPurgeNodeOFSDB(ctx, item.node)) return false; // critical error then return error before taking any other action

            // reload latest states
            rootAncestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, rootAncestor.Id, false, false, false, false, true, true);

            // remove the item from lineage tree of the root ancestor
            if (!Remove(item)) return false; // error item not removed from parent

            // reload latest states
            item = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, item.Id, false, false, false, false, false, false);
            rootAncestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, rootAncestor.Id, false, false, false, false, true, true);

            if (removeFromParent)
            {
                // load latest parent state
                RegisterItem? parent = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, item.parentId, false, false, false, false, true, true);
                if (parent == null) return false; // critical error abort all op

                // parent loaded successfully, proceed
                // remove the item from parent
                parent.children.Remove(item);
            }
            
            // delete node and register in empty slots system node register or simply complete delete()
            RegisterItem? newEmptySlotItem = null;
            if (withoutEmptySlotsRegister)
            {
                if (!Register.DeleteNode(ctx, registerFile, item))
                    return false; // if error abort with error
            }
            else
            {
                // zero out the deleted node and insert in empty slots register
                if (!Register.WriteEmptySlot(ctx, registerFile, ref emptySlots, item.Id, ref newEmptySlotItem))
                    return false; // error abort
            }

            // reload latest states
            rootAncestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, rootAncestor.Id, false, false, false, false, true, true);
            return true;
        }
        // traverses the tree beginning from root child node and finds an ancestor's all true descendants recursively and marks them
        public bool MarkDescendants(RegisterItem? ancestor, bool markAncestor, NodeType nodeType, bool useNodeType, SpecialNodeType specialNodeType, 
            bool useSpecialNodeType, DomainType domainType, bool useDomainType)
        {
            if (ctx.readOnly) return false;
            RegisterItem? original = current;

            current = null;
            while (true)
            {
                RegisterItem? nextItem = Next();
                if (nextItem == null) break; // end of chain, abort loop
                if (nextItem.specialNodeType == SpecialNodeType.SystemNode) continue;

                if (IsDescendantOfAncestor(nextItem, ancestor))
                {
                    if (useNodeType)
                        nextItem.nodeType = nodeType;

                    if (useSpecialNodeType)
                        nextItem.specialNodeType = specialNodeType;

                    if (useDomainType)
                        nextItem.domainType = domainType;

                    // finally update the item
                    Register.UpdateNode(ctx, registerFile, nextItem, 0, false, 0, false, 0, false);
                }
            }

            // reload latest states
            ancestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, ancestor.Id, false, false, false, false, true, true);
            if (markAncestor)
            {
                if (useNodeType)
                    ancestor.nodeType = nodeType;

                if (useSpecialNodeType)
                    ancestor.specialNodeType = specialNodeType;

                if (useDomainType)
                    ancestor.domainType = domainType;

                // finally update the item
                Register.UpdateNode(ctx, registerFile, ancestor, 0, false, 0, false, 0, false);
            }

            // reload latest states
            rootAncestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, rootAncestor.Id, false, false, false, false, true, true);

            current = original;
            return true;

        }
        // traverses the tree beginning from root child node and finds an ancestor's all true descendants recursively and records them in a temp file list
        public Stream? CreateDescendantList(RegisterItem? ancestor, out String? tmpfileOut)
        {
            // reload latest states
            rootAncestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, rootAncestor.Id, false, false, false, false, true, true);
            ancestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, ancestor.Id, false, false, false, false, true, true);

            RegisterItem? original = current;

            Stream? s = CoreFramework.createTempWorkFile(out tmpfileOut);
            if (s == null) return null;
            BinaryWriter bw = new BinaryWriter(s);

            current = null;
            while (true)
            {
                RegisterItem? nextItem = Next();
                if (nextItem == null) break; // end of chain, abort loop

                if (IsDescendantOfAncestor(nextItem, ancestor))
                    bw.Write(nextItem.Id);
            }
            current = original;
            s.Position = 0;
            return s;
        }

        // traverses the tree beginning from root child node and finds and counts an ancestor's all true descendants recursively
        public Int64 CountDescendants(RegisterItem? ancestor)
        {
            RegisterItem? original = current;

            current = null;
            Int64 found = 0;
            while (true)
            {
                RegisterItem? nextItem = Next();
                if (nextItem == null) break; // end of chain, abort loop

                if (IsDescendantOfAncestor(nextItem, ancestor))
                    found++;
            }

            current = original;
            return found;
        }

        // traverses the tree beginning from root child node and finds an ancestor's all true descendants recursively and deletes them one by one
        public bool DeleteDescendants(RegisterItem? ancestor, bool deleteAncestor, ref RegisterItem? emptySlots, bool withoutEmptySlotsRegister)
        {
            if (ctx.readOnly) return false;

            // reload latest states
            rootAncestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, rootAncestor.Id, false, false, false, false, true, true);
            ancestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, ancestor.Id, false, false, false, false, true, true);

            // using temporary file register for consistent operation so that integrity remains correct.
            String? tmpfile = "";
            Stream? s = CreateDescendantList(ancestor, out tmpfile);
            if (s == null) return false;
            BinaryReader br = new BinaryReader(s);

            // reload latest states
            rootAncestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, rootAncestor.Id, false, false, false, false, true, true);
            ancestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, ancestor.Id, false, false, false, false, true, true);

            while (br.PeekChar() != -1)
            {
                emptySlots = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, emptySlots.Id, false, false, false, false, true, true);
                rootAncestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, rootAncestor.Id, false, false, false, false, true, true);
                Int64 descendantID = br.ReadInt64();
                RegisterItem? nextItem = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, descendantID, false, false, false, false, true, true);
                if (nextItem.specialNodeType == SpecialNodeType.SystemNode) continue;
                if (!DeleteDescendant(nextItem, ref emptySlots, withoutEmptySlotsRegister, true))
                    return false; // critical error

            }
            // close and delete temp file after processing
            s.Close();
            CoreFramework.removeTempWorkFile(tmpfile);

            // reload latest states
            rootAncestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, rootAncestor.Id, false, false, false, false, true, true);
            ancestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, ancestor.Id, false, false, false, false, true, true);
            emptySlots = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, emptySlots.Id, false, false, false, false, true, true);

            // we cannot delete ancestor at first we need to delete all it's true descendants recursively.
            // if ancestor is itself a root child Adam, then we cannot delete ancestor at first because it destroys the lineage chain's 
            // configuration if we delete the root ancesotr node before deleting it's descendants. so we need to first delete all descendants, then in the last 
            // the ancestor.
            if (deleteAncestor)
            {
                if (ancestor.Id == rootAncestor.Id)
                {
                    // ancestor is root ancestor itself, so we delete it with special function
                    return DeleteRootAncestor(ref emptySlots, withoutEmptySlotsRegister);
                }
                else
                {
                    // ancestor is not root ancestor but a child in it's tree so we delete it with common function
                    if (!DeleteDescendant(ancestor, ref emptySlots, withoutEmptySlotsRegister, true))
                        return false; // critical error
                }
            }
            return true;
        }
        // traverses the tree beginning from root child node and finds an ancestor's all descendants recursively
        public bool GetDescendants(RegisterItem? ancestor, ref List<RegisterItem> listOut)
        {
            RegisterItem? original = current;

            List<RegisterItem>? list = new List<RegisterItem>();
            listOut = list;

            // there are descendants, iterate through them and find valid nodes
            current = null;
            while (true)
            {
                RegisterItem? nextItem = Next();
                if (nextItem == null) break;

                if (IsDescendantOfAncestor(nextItem, ancestor))
                    list.Add(nextItem); // yes this is a descendant and exists under ancestor

            }
            current = original;
            return true;
        }
        // we get all lineage tree which exists on root ancestor Adam
        public bool GetWholeTree(ref List<RegisterItem>? listOut, bool loadNode)
        {
            Int64 nextDescendantId = -1;

            RegisterItem? current = null;

            List<RegisterItem>? list = new List<RegisterItem>();
            listOut = list;

            if (rootAncestor.treeHeadId == 0) return false; // there are no children so abort and return empty list

            // current is not loaded, so get first head node as current
            if (Register.FindNode(ctx, registerFile, rootAncestor.treeHeadId, ref current) < 0)
                return false; // end of chain or error or no children present in parent, abort

            if (current == null)
                return false; // there is no child node present so abort

            nextDescendantId = current.Id;

            while (true)
            {
                // load item from registry
                RegisterItem? nextItem = null;
                Int64 nextOffset = Register.FindNode(ctx, registerFile, nextDescendantId, ref nextItem);
                if (nextOffset < 0) break; // error or end of stream or end of register so break
                if (nextItem == null) break; // no more items so break

                String rtf = "";
                byte[]? xamlbytesOut = null;
                if (loadNode)
                    nextItem.loadNode(ctx, ref rtf, ref xamlbytesOut, false);

                // add item to list
                list.Add(nextItem);

                // configure
                nextDescendantId = nextItem.nextDescendantId;

                // check if end of register reached
                if (nextItem.nextDescendantId == 0) break;
            }
            // output
            listOut = list;
            return true;
        }
        // we find a descendant in the tree by physically traversing the tree
        public RegisterItem? FindDescendant(Int64 id, bool loadNode)
        {
            Int64 nextDescendantId = -1;

            RegisterItem? current = null;

            if (rootAncestor.treeHeadId == 0) return null; // there are no children so abort and return empty list

            // current is not loaded, so get first head node as current
            if (Register.FindNode(ctx, registerFile, rootAncestor.treeHeadId, ref current) < 0)
                return null; // end of chain or error or no children present in parent, abort

            if (current == null)
                return null; // end of chain or error or no children present in parent, abort

            nextDescendantId = current.Id;

            while (true)
            {
                // load item from registry
                RegisterItem? nextItem = null;
                Int64 nextOffset = Register.FindNode(ctx, registerFile, nextDescendantId, ref nextItem);
                if (nextOffset < 0) break; // error or end of stream or end of register so break
                if (nextItem == null) break; // no more items so break

                if (nextItem.Id == id)
                {
                    // node found in descendant tree
                    String rtf = "";
                    byte[]? xamlbytesOut = null;
                    if (loadNode)
                    {
                        if (!nextItem.loadNode(ctx, ref rtf, ref xamlbytesOut, false))
                            return null; // critical error node entry files not found so abort with error
                    }
                    // success
                    return nextItem;
                }

                // configure
                nextDescendantId = nextItem.nextDescendantId;

                // check if end of register reached
                if (nextItem.nextDescendantId == 0)
                    break;
            }
            // not found
            return null;
        }

        public RegisterItem? First(RegisterItem? ancestor = null)
        {
            if (rootAncestor.treeHeadId == 0) return null;
            current = null;
            current = Next(ancestor);
            //Register.FindNode(ctx, registerFile, rootAncestor.treeHeadId, ref current);
            return current;
        }
        public RegisterItem? Last(RegisterItem? ancestor = null)
        {
            if (rootAncestor.treeTailId == 0) return null;
            current = null;
            current = Previous(ancestor);
            //Register.FindNode(ctx, registerFile, rootAncestor.treeTailId, ref current);
            return current;
        }
        public RegisterItem? Next(RegisterItem? ancestor = null)
        {
            Int64 nextDescendantId = -1;

            if (rootAncestor.treeHeadId == 0) return null; // there are no children so abort and return empty list

            if (current == null)
            {
                // current is not loaded, so get first head node as current
                if (Register.FindNode(ctx, registerFile, rootAncestor.treeHeadId, ref current) < 0)
                    return null; // end of chain or error or no children present in parent, abort

                nextDescendantId = current.Id;
            }
            else
            {
                // current is already loaded so proceed with the next descendant in chain sequence
                if (current.nextDescendantId == 0) return null; // end of chain, abort

                // get next descendant's offset
                nextDescendantId = current.nextDescendantId;
            }

            RegisterItem? nextItem = null;
            while (true)
            {
                // load item from registry
                Int64 nextOffset = Register.FindNode(ctx, registerFile, nextDescendantId, ref nextItem);
                if (nextOffset < 0) break; // no more nodes so break
                if (ancestor != null)
                {
                    // an ancestor is passed check if this node is descendant
                    // return ancestor's descendant
                    if (IsDescendantOfAncestor(nextItem, ancestor))
                    {
                        // descendant of this ancestor found, so set and return it
                        current = nextItem;
                        return current;
                    }
                }
                else
                {
                    // ancestor param is not passed so set and return this item
                    // a next descendant is found so break and return it
                    current = nextItem;
                    return current;
                }

                // configure
                nextDescendantId = nextItem.nextDescendantId;

                // check if end of register reached
                if (nextItem.nextDescendantId == 0) break;
            }
            return null;
        }
        public RegisterItem? Previous(RegisterItem? ancestor = null)
        {
            Int64 prevDescendantId = -1;

            if (rootAncestor.treeHeadId == 0) return null; // there are no children so abort and return empty list

            if (current == null)
            {
                // current is not loaded, so get last tail node as current
                if (Register.FindNode(ctx, registerFile, rootAncestor.treeTailId, ref current) < 0)
                    return null; // end of chain or error or no children present in parent, abort

                prevDescendantId = current.Id;
            }
            else
            {
                // current is already loaded so proceed with it's previous descendant in chain sequence
                if (current.previousDescendantId == 0) return null; // end of chain, abort

                // get prev offset
                prevDescendantId = current.previousDescendantId;
            }

            RegisterItem? prevItem = null;
            while (true)
            {
                // load item from registry
                Int64 prevOffset = Register.FindNode(ctx, registerFile, prevDescendantId, ref prevItem);
                if (prevOffset < 0) return null; // critical error abort with null
                if (ancestor != null)
                {
                    // an ancestor is passed check if this node is descendant
                    // return ancestor's descendant
                    if (IsDescendantOfAncestor(prevItem, ancestor))
                    {
                        // descendant of this ancestor found, so set and return it
                        current = prevItem;
                        return current;
                    }
                }
                else
                {
                    // ancestor param is not passed so set and return this item
                    // a previous descendant is found so break and return it
                    current = prevItem;
                    return current;
                }

                // configure
                prevDescendantId = prevItem.previousDescendantId;

                // check if end of register reached
                if (prevItem.previousDescendantId == 0) break;
            }
            return null;
        }


        public bool reloadConfig()
        {
            if (Register.FindNode(ctx, registerFile, rootAncestor.Id, ref rootAncestor) < 0) return false; // critical error
            if (rootAncestor == null) return false; // critical error
            return true;
        }
        public void SetLatestState(OpenFSDBContext? ctx, String registerFile, RegisterItem state, bool reset)
        {
            // configure and setup
            this.ctx = ctx;
            this.registerFile = registerFile;
            if (reset) this.current = null;
            this.rootAncestor = state;
        }
        public void SetLatestState(RegisterItem state, bool reset)
        {
            // configure and setup
            if (reset) this.current = null;
            this.rootAncestor = state;
        }

        public static bool Initialize(OpenFSDBContext? ctx, String registerFile,
            LineageRegister list, RegisterItem item)
        {
            // configure and setup

            list.ctx = ctx;
            list.registerFile = registerFile;
            list.current = null;

            // initialize register
            RegisterItem? rootAncestor = null;
            if (item.treeRootId <= 0)
            {
                // item is root child node Adam so set it up as root ancestor and initialize lineage in it
                rootAncestor = item;
            }
            else
            {
                // item is an ordinary child in lineage and has another root child ancestor, so load the ancestor and setup
                //if (Register.FindNode(ctx, registerFile, item.treeRootId, ref rootAncestor) < 0) return false; // critical error
                rootAncestor = Register.LoadSetupRegisterItem(ctx, registerFile, item.treeRootId, false, false, false, false, true, true);
                if (rootAncestor == null) return false; // critical error
            }

            list.rootAncestor = rootAncestor;
            return true;
        }

        public static bool InitializeCustomLineageTree(OpenFSDBContext? ctx, String registerFile,
            LineageRegister list, RegisterItem custom)
        {
            // configure and setup
            list.ctx = ctx;
            list.registerFile = registerFile;
            list.current = null;
            list.rootAncestor = custom;
            return true;
        }

    }

}


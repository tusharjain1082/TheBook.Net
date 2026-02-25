using DiaryJournal.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
 * 
 * original sole developer and inventor: Tushar Jain
 * status: relic
 * Name/Title: Mother of Databases - The Sacred Register
 * I created it for my Projects: Tushar Jain's TheBook.Net and DiaryJournal.Net
 * Copyright(c) Tushar Jain
 * 
 * */

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
    lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
    item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
    lib.children.Remove(item);
    // step 2 =
    item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
    lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
    item.tree.MoveDescendants(item, false, lib.tree.rootAncestor);
    // step 3 =
    item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
    lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
    lib.children.Add(item, lib);
    // step 4 =
    lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
    item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
    lib.tree.Add(item);
    // step 5 =
    item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
    lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
    List<RegisterItem> libtree = null;
    lib.tree.GetWholeTree(ref libtree, false);
    item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
    libchildren = null;
    lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
    lib.children.GetChildren(ref libchildren, false);
    List<RegisterItem> itemtree = null;
    item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
    item.tree.GetWholeTree(ref itemtree, false);
    List<RegisterItem> itemchildren = null;
    item.children.GetChildren(ref itemchildren, false);

    // step 1 =
    lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
    item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
    lib.children.Remove(item);
    // step 2 =
    //lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true);
    //item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false);
    //lib.tree.Remove(item);
    // step 3 =
    lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
    item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
    BuildCustomLineageStructure(ctx, item);
    // we cannot move descendants to root because we cannot put tree in there.
    //item.tree.MoveDescendants(item, false, root.tree.rootAncestor);
    // step 4 =
    item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
    root = LoadSetupRegisterItem(ctx, root.Id, false, false, false, false, true, true);
    root.children.Add(item, root);
    // step 5 =
    item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
    root = LoadSetupRegisterItem(ctx, root.Id, false, false, false, false, true, true);
    List<RegisterItem> rootchildren = null;
    root.children.GetChildren(ref rootchildren, false);
    item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
    item.tree.GetWholeTree(ref itemtree, false);
    // step 5 =
    item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
    root = LoadSetupRegisterItem(ctx, root.Id, false, false, false, false, true, true);
    root.children.Remove(item);
    // step 5 =
    lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
    item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
    root = LoadSetupRegisterItem(ctx, root.Id, false, false, false, false, true, true);
    item.tree.MoveDescendants(item, true, lib.tree.rootAncestor);
    // step 5 =
    lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
    item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
    lib.children.Add(item, lib);
    // step 5 =
    item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
    lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
    lib.tree.GetWholeTree(ref libtree, false);
    item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
    lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
    lib.children.GetChildren(ref libchildren, false);
    return;
     */

    public class RegisterItem
    {
        // offset in register file
        public Int64 position = 0;

        // 10 UInt32, 4 bytes, 1 16 bit uint
        public const int blockSize = ((sizeof(UInt32) * 9) + 3);
        // id is not stored, it is calculated dynamically
        public UInt32 Id = 0;

        // 10 UInt32 * 4 bytes = 40 bytes binary block is made of all these elements =
        public UInt32 parentId = 0;
        public UInt32 sectionId = 0;
        public UInt32 headId = 0;
        public UInt32 tailId = 0;
        public UInt32 nextSiblingId = 0;
        public UInt32 previousSiblingId = 0;
        public UInt32 childrenCount = 0;
        public UInt32 nextId = 0;
        public UInt32 prevId = 0;

        // 4 bytes binary block is made of all these elements =
        public NodeType nodeType = NodeType.Entry;
        public SpecialNodeType specialNodeType = SpecialNodeType.None;
        public DomainType domainType = DomainType.Journal;

        public myNode? node = null;
        public ChildrenRegister? children = null;
        public String? rtf = null;
        public byte[]? xamlbytes = null;
        public List<RegisterItem>? childrenList = null;
        public List<RegisterItem>? treeList = null;

        public TreeSequenceRegister? tree = null;

        public RegisterItem()
        {

        }
        public RegisterItem(Int64 position, UInt32 id, UInt32 parentid, UInt32 sectionid, UInt32 childrenCount,
            UInt32 headId, UInt32 tailId, UInt32 nextSiblingId, UInt32 previousSiblingId, NodeType nodeType,
            SpecialNodeType specialNodeType, DomainType domainType, 
            UInt32 nextId, UInt32 prevId)
        {
            this.position = position;
            this.Id = id;
            this.parentId = parentid;
            this.sectionId = sectionid;
            this.headId = headId;
            this.tailId = tailId;
            this.nextSiblingId = nextSiblingId;
            this.previousSiblingId = previousSiblingId;
            this.childrenCount = childrenCount;
            this.nextId = nextId;
            this.prevId = prevId;
            this.nodeType = nodeType;
            this.specialNodeType = specialNodeType;
            this.domainType = domainType;
        }

        public void CopyFrom(RegisterItem? item)
        {
            this.position = item.position;
            this.Id = item.Id;
            this.parentId = item.parentId;
            this.sectionId = item.sectionId;
            this.headId = item.headId;
            this.tailId = item.tailId;
            this.nextSiblingId = item.nextSiblingId;
            this.previousSiblingId = item.previousSiblingId;
            this.childrenCount = item.childrenCount;
            this.nextId = item.nextId;
            this.prevId = item.prevId;

            this.nodeType = item.nodeType;
            this.specialNodeType = item.specialNodeType;
            this.domainType = item.domainType;

        }
        public void CopyFrom(myNode node, bool copyId, bool copyParentId, bool copySectionId)
        {
            if (copyId) this.Id = node.chapter.Id;
            if (copyParentId) this.parentId = node.chapter.parentId;
            if (copySectionId) this.sectionId = node.DirectorySectionID;
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
            item.sectionId = node.DirectorySectionID;
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
            bw.Write(node.chapter.parentId);
            bw.Write(node.DirectorySectionID);
            bw.Write((UInt32)0);
            bw.Write((UInt32)0);
            bw.Write((UInt32)0);
            bw.Write((UInt32)0);
            bw.Write((UInt32)0);
            bw.Write((UInt32)0);
            bw.Write((UInt32)0);
            bw.Write((byte)node.chapter.nodeType);
            bw.Write((byte)node.chapter.specialNodeType);
            bw.Write((byte)node.chapter.domainType);
            return ms.ToArray();
        }
        public static byte[] convertToBytes(UInt32 id, UInt32 parentId, UInt32 sectionId, UInt32 childrenCount,
            UInt32 headId, UInt32 tailId, UInt32 nextSiblingId, UInt32 previousSiblingId, NodeType nodeType,
            SpecialNodeType specialNodeType, DomainType domainType,
            UInt32 nextId, UInt32 prevId)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(parentId);
            bw.Write(sectionId);
            bw.Write(headId);
            bw.Write(tailId);
            bw.Write(nextSiblingId);
            bw.Write(previousSiblingId);
            bw.Write(childrenCount);
            bw.Write(nextId);
            bw.Write(prevId);
            bw.Write((byte)nodeType);
            bw.Write((byte)specialNodeType);
            bw.Write((byte)domainType);
            return ms.ToArray();
        }
        public static byte[] convertToBytes(RegisterItem item)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(item.parentId);
            bw.Write(item.sectionId);
            bw.Write(item.headId);
            bw.Write(item.tailId);
            bw.Write(item.nextSiblingId);
            bw.Write(item.previousSiblingId);
            bw.Write(item.childrenCount);
            bw.Write(item.nextId);
            bw.Write(item.prevId);
            bw.Write((byte)item.nodeType);
            bw.Write((byte)item.specialNodeType);
            bw.Write((byte)item.domainType);
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
            item.parentId = br.ReadUInt32();
            item.sectionId = br.ReadUInt32();
            item.headId = br.ReadUInt32();
            item.tailId = br.ReadUInt32();
            item.nextSiblingId = br.ReadUInt32();
            item.previousSiblingId = br.ReadUInt32();
            item.childrenCount = br.ReadUInt32();
            item.nextId = br.ReadUInt32();
            item.prevId = br.ReadUInt32();
            item.nodeType = (NodeType)br.ReadByte();
            item.specialNodeType = (SpecialNodeType)br.ReadByte();
            item.domainType = (DomainType)br.ReadByte();
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
            item.parentId = br.ReadUInt32();
            item.sectionId = br.ReadUInt32();
            item.headId = br.ReadUInt32();
            item.tailId = br.ReadUInt32();
            item.nextSiblingId = br.ReadUInt32();
            item.previousSiblingId = br.ReadUInt32();
            item.childrenCount = br.ReadUInt32();
            item.nextId = br.ReadUInt32();
            item.prevId = br.ReadUInt32();
            item.nodeType = (NodeType)br.ReadByte();
            item.specialNodeType = (SpecialNodeType)br.ReadByte();
            item.domainType = (DomainType)br.ReadByte();
            return item;
        }
        public bool InitializeSystem(OpenFSDBContext? ctx, String registerFile, bool loadChildrenRegister, bool loadTreeRegister)
        {
            if (loadChildrenRegister)
            {
                // initialize children register
                this.children = new ChildrenRegister();
                ChildrenRegister.Initialize(ctx, this.children, this);
            }

            if (loadTreeRegister)
            {
                // initialize lineage register
                this.tree = new TreeSequenceRegister();
                TreeSequenceRegister.Initialize(ctx, this.tree, this);
            }

            return true;
        }
        #endregion
    }

    public class Register
    {
        public const UInt32 default_RootNodeId = 0;
        //public const int default_totalPreallocatedNodes = 10000001; // 10 million or 1 crore slots/nodes; //8000001; // 8 million slots/nodes; 3 million slots/nodes //1000001; // 3 million nodes + 1 root node preallocated in register
        public const int default_totalPreallocatedNodes = 10000001; // 10 million or 1 crore slots/nodes 8000001; // 8 million slots/nodes; 3 million slots/nodes //1000001; // 3 million nodes + 1 root node preallocated in register
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

        public static bool toFile(String file)
        {
            using (Stream? s = new FileStream(file, FileMode.Create))
                return toFile(s);
        }

        public static bool toFile(Stream s)
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
                byte[] blockBytes = RegisterItem.convertToBytes(emptySlotItem);
                for (UInt32 id = 0; id < default_totalPreallocatedNodes; id++)
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

        public static Stream? RegisterCopyToMemory(String file)
        {
            if (!File.Exists(file)) return null;
            byte[] bytes = File.ReadAllBytes(file);
            return new MemoryStream(bytes);
        }

        // loads direct children of a parent raw without using register
        public static List<RegisterItem>? LoadChildrenRaw(Stream? s, UInt32 id)
        {
            // first get the item by id
            //RegisterItem? parent = LoadSetupRegisterItem(ctx, id, false, false, false, false, false, false);
            //if (parent == null) return false;

            List<RegisterItem>? list = new List<RegisterItem>();

            s.Position = 0;

            for (UInt32 i = 0; i < Register.default_totalPreallocatedNodes; i++)
            {
                RegisterItem? child = null;
                FindNode(s, i, ref child);
                if (child == null) continue;
                if (child.parentId == id && child.domainType != DomainType.EmptySlot)
                    list.Add(child);
            }
            return list;
        }

        // this method finds a first empty slot
        public static RegisterItem? FindFirstEmptySlot(OpenFSDBContext? ctx, ref RegisterItem? emptySlots, bool deleteEmptySlot)
        {
            // first get the latest state of empty slots register by id
            emptySlots = LoadSetupRegisterItem(ctx, emptySlots.Id, false, false, false, false, true, true);
            if (emptySlots == null) return null;
            // return the first empty slot in chain of the system node
            RegisterItem? found = emptySlots.children.First();
            if (found == null) return null;
            if (deleteEmptySlot) emptySlots.children.Delete(found, ref emptySlots, true, true, false);
            return found;
        }
        // this method deletes the empty slot by id in parent node
        public static bool DeleteEmptySlot(OpenFSDBContext? ctx, RegisterItem emptySlots, UInt32 id)
        {
            if (ctx.readOnly) return false;

            // now get the item by id
            RegisterItem? item = LoadSetupRegisterItem(ctx, id, false, false, false, false, false, false);
            emptySlots = LoadSetupRegisterItem(ctx, emptySlots.Id, false, false, false, false, true, true);
            if (item == null) return false;

            return emptySlots.children.Delete(item, ref emptySlots, true, true, false);
        }

        public static bool IsDescendantOfAncestor(OpenFSDBContext? ctx, RegisterItem? item, RegisterItem? ancestor)
        {
            List<RegisterItem>? lineage = null;
            Register.Lineage(ctx, item, ref lineage, true, false, false);
            RegisterItem? found = lineage.Find(x => x.Id == ancestor.Id);
            if (found != null)
            {
                // yes current item is descendant of ancestor
                return true;
            }
            // no current item is not a descendant of ancestor
            return false;
        }

        // this method finds the node
        public static Int64 FindNode(OpenFSDBContext? ctx, UInt32 id, ref RegisterItem? itemOut)
        {
            // not vhd so process local
            return FindNode(ctx.regFileStream, id, ref itemOut);
        }
        // this method finds the node
        public static Int64 FindNode(Stream s, UInt32 id, ref RegisterItem? itemOut)
        {
            Int64 offset = RegisterItem.blockSize * id;
            s.Position = offset;
            RegisterItem? item = RegisterItem.convertFromBytesStream(s);
            if (item == null) return -1; // end of stream so break
            
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
        // this method inserts a node in the registry and writes the node files in db path
        public static RegisterItem? Insert(OpenFSDBContext? ctx, UInt32 parentId,
            RegisterItem? emptySlotsItem, myNode? node, String? rtf, byte[]? xamlbytes,
            bool commitUsedSlots = true, bool commitDBConfig = true)
        {
            if (ctx.readOnly) return null;

            // first get latest state of the parent node
            RegisterItem? parent = LoadSetupRegisterItem(ctx, parentId, false, false, false, false, true, true);
            if (parent == null) return null;

            // validations
            //if (parent.childrenCount >= Register.default_maxChildrenNodes)
            //    return null;

            // 2nd setup item
            node.chapter.parentId = parent.Id;
            //DomainType domainType = node.chapter.domainType;
            if (node.chapter.nodeType == NodeType.EmptySlot) node.chapter.domainType = DomainType.HiddenCore;

            if (emptySlotsItem == null) return null; // no empty slots register so abort with error

            //// 3rd get the first empty slot and delete it from empty slots register because it is not empty anymore as it should be allocated
            // empty slots system node is set as param, so go through it
            RegisterItem? firstEmptySlot = FindFirstEmptySlot(ctx, ref emptySlotsItem, false);
            if (firstEmptySlot != null)
            {
                // found an empty slot which is under the current db index, so we use this empty slot and do not increment the db index
                // db index is incremented only when all empty slots have been used upto the current db index so we use current db index and increment it
                // configure
                node.chapter.Id = firstEmptySlot.Id;
                // free the empty slot from it's parent register
                if (emptySlotsItem != null)
                {
                    if (!DeleteEmptySlot(ctx, emptySlotsItem, node.chapter.Id)) // if empty slots is passed then delete the empty slot from it
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
                node.chapter.nodeType, node.chapter.specialNodeType, node.chapter.domainType, 0, 0);

            // first add to parent's tree sequence register
            parent = Register.LoadSetupRegisterItem(ctx, parent.Id, true, false, false, false, true, true);
            parent.tree.Add(item);
            Register.FindNode(ctx, item.Id, ref item);
            parent = Register.LoadSetupRegisterItem(ctx, parent.Id, true, false, false, false, true, true);

            // add to parent's children register
            if (!parent.children.Add(item))
                return null;

            // update used slots config
            ctx.usedSlots += 1;
            if (commitUsedSlots) ctx.writeUsedSlotsFile();

            item = Register.LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            item.node = node;
            if (item != null)
            {
                ctx.dbConfig.latestCreatedEntry = item.Id;
                if (commitDBConfig) DatabaseConfig.toYamlFile(ctx.dbConfig, ctx.dbConfigFile);
            }
            return item;
        }

        // this method inserts a node in the registry
        public static Int64 InsertNode(OpenFSDBContext? ctx, RegisterItem item)
        {
            if (ctx.readOnly) return -1;
            return InsertNode(ctx.regFileStream, item);

        }
        // this method overwrites with empty slot and registers it with empty slots system node
        public static bool WriteEmptySlot(OpenFSDBContext? ctx,
            ref RegisterItem emptySlotsParentItem, UInt32 id, ref RegisterItem? itemOut)
        {
            if (ctx.readOnly) return false;
            
            RegisterItem? emptySlotItem = new RegisterItem();
            emptySlotItem.Id = id;
            emptySlotItem.parentId = emptySlotsParentItem.Id;
            emptySlotItem.nodeType = NodeType.EmptySlot;
            emptySlotItem.specialNodeType = SpecialNodeType.EmptySlot;
            emptySlotItem.domainType = DomainType.EmptySlot;

            // convert the node item to bytes and write it
            // add this empty slot into empty slots parent's register. this also overwrites the old with new item
            if (!emptySlotsParentItem.children.Add(emptySlotItem)) return false;
            itemOut = emptySlotItem;
            return true;
        }

        // this method deletes a node in the registry
        public static bool DeleteNode(OpenFSDBContext? ctx, RegisterItem item)
        {
            if (ctx.readOnly) return false;
            return DeleteNode(ctx.regFileStream, item);
        }
        // this method deletes a node in the registry
        public static bool DeleteNode(OpenFSDBContext? ctx, UInt32 id)
        {
            if (ctx.readOnly) return false;
            return DeleteNode(ctx.regFileStream, id);
        }
        // this method deletes a node in the registry
        public static bool DeleteNode(Stream s, RegisterItem? item)
        {
            if (item == null) return false;
            return DeleteNode(s, item.Id);
        }

        // this method deletes a node in the registry
        public static bool DeleteNode(Stream s, UInt32 id)
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
        public static bool UpdateNode(OpenFSDBContext? ctx, RegisterItem? item, UInt32 parentId, bool useParentId, UInt32 DirectorySectionId, bool useDirectorySectionId,
            UInt32 childrenCount, bool useChildrenCount)
        {
            if (ctx.readOnly) return false;
            return UpdateNode(ctx.regFileStream, item, parentId, useParentId, DirectorySectionId, useDirectorySectionId,
                childrenCount, useChildrenCount);
        }

        // this method updates a node in the registry
        public static bool UpdateNode(Stream s, RegisterItem? item, UInt32 parentId, bool useParentId, UInt32 DirectorySectionId, bool useDirectorySectionId,
            UInt32 childrenCount, bool useChildrenCount)
        {
            if (item == null) return false;

            if (useParentId)
                item.parentId = parentId;

            if (useDirectorySectionId)
                item.sectionId = DirectorySectionId;

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
        public static bool Lineage(OpenFSDBContext? ctx,
            RegisterItem item, ref List<RegisterItem>? listOut, bool topFirst, bool addCurrentItem = true, bool addRoot = true)
        {
            return Lineage(ctx.regFileStream, item, ref listOut, topFirst, addCurrentItem, addRoot);
        }
        // gets all parents right to the root
        public static bool Lineage(OpenFSDBContext? ctx,
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
                item = LoadSetupRegisterItem(ctx, item.parentId, loadNode, loadRtf, loadChildren, loadChildrenConfigs, false, false);
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
        public static bool ExistsSomewhereInAncestoralChain(OpenFSDBContext? ctx, RegisterItem? item, RegisterItem? target)
        {
            List<RegisterItem>? targetLineage = null;
            Register.Lineage(ctx, target, ref targetLineage, true, true, false);
            List<RegisterItem>? itemLineage = null;
            Register.Lineage(ctx, item, ref itemLineage, true, true, false);

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
        public static bool LineageRootAncestor(OpenFSDBContext? ctx,
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
                FindNode(ctx, item.parentId, ref item);
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
        public static bool FindLineageAncestor(List<RegisterItem> list, UInt32 id)
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
        public static bool LineageRemoveAncestor(List<RegisterItem> list, UInt32 id)
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
        public static bool LineageIntegrity(OpenFSDBContext? ctx, RegisterItem item)
        {
            if (item.domainType == DomainType.EmptySlot)
                return false; // critical error, integrity of ancestoral chain invalid/corrupted, error abort

            // this item is root, it does not has any parent so abort
            if (item.Id == 0) return true;

            // this item is not root but a child item, so get all lineage right to the root
            // ascending order list from bottom to top parents
            while (true)
            {
                item = LoadSetupRegisterItem(ctx, item.parentId, true, false, false, false, false, false);
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
        public static bool LineageFromFullPath(String path, ref List<UInt32> listOut)
        {
            if (path == "") return false;
            List<UInt32> list = new List<UInt32>();
            String[] ancestors = path.Split(@"\");
            if (ancestors.Count() == 0) return false; // now a valid path

            foreach (String ancestor in ancestors)
            {
                if (ancestor == "") continue;

                UInt32 value = 0;
                if (!UInt32.TryParse(ancestor, out value)) return false; // invalid path or garbage
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

                UInt32 id = 0;
                if (!UInt32.TryParse(ancestor, out id)) return false; // invalid path or garbage

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
        public static bool LoadFullPath(OpenFSDBContext? ctx, String path, ref List<RegisterItem>? listOut, bool topFirst)
        {
            return LoadFullPath(ctx.regFileStream, path, ref listOut, topFirst);
        }
        // loads all registry node items from path
        public static bool LoadFullPath(OpenFSDBContext? ctx, RegisterItem item, ref List<RegisterItem>? listOut, bool topFirst,
            bool loadNode, bool loadRtf, bool loadChildren, bool loadChildrenConfigs)
        {
            return Lineage(ctx, item, ref listOut, topFirst, loadNode, loadRtf, loadChildren, loadChildrenConfigs);
        }

        // loads all registry node items from path
        public static bool LoadFullPath(Stream s, String path, ref List<RegisterItem>? listOut, bool topFirst)
        {
            List<RegisterItem> list = new List<RegisterItem>();

            // phase 1 - get all lineage ids

            // get all ancestors ids
            List<UInt32> lineageItems = null;
            if (!LineageFromFullPath(path, ref lineageItems)) return false;
            if (lineageItems.Count == 0) return false;

            // phase 2 - get the lowest item by id from the register

            // now get the lowest child's offset and item
            UInt32 lowestChildId = lineageItems.LastOrDefault();
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
        public static bool LoadFullPath(OpenFSDBContext? ctx, UInt32 id, ref List<RegisterItem>? listOut, bool topFirst)
        {
            // first get the item by id
            RegisterItem? item = LoadSetupRegisterItem(ctx, id, false, false, false, false, true, true);
            if (item == null) return false;

            List<RegisterItem>? list = null;

            // now directly get lineage
            if (!Lineage(ctx, item, ref list, topFirst)) return false; // if error return error

            // success
            listOut = list;
            return true;
        }
        // gets register file's size
        public static Int64 RegisterFileSize(OpenFSDBContext? ctx)
        {
            return ctx.regFileSize;
        }

        // returns the total number of entries
        public static Int64 Count(OpenFSDBContext? ctx, RegisterItem? emptySlotsItem, ref Int64 emptySlotsCountOut, ref Int64 regfileSizeOut)
        {
            // first get the latest state of empty slots register by id
            emptySlotsItem = LoadSetupRegisterItem(ctx, emptySlotsItem.Id, false, false, false, false, false, false);
            if (emptySlotsItem == null) return -1;

            Int64 emptySlotsCount = emptySlotsItem.childrenCount;
            Int64 registerFileSize = RegisterFileSize(ctx);
            emptySlotsCountOut = emptySlotsCount;
            regfileSizeOut = registerFileSize;
            Int64 count = registerFileSize / RegisterItem.blockSize;
            count -= emptySlotsItem.childrenCount;
            return count;
        }
        // returns the total number of entries which validly exist in all root ancestors
        public static UInt32 Total(OpenFSDBContext? ctx)
        {
            return ctx.usedSlots;
        }
        // we get system nodes
        public static bool FindSystemNodeRegisterItem(OpenFSDBContext? ctx, RegisterItem? parent, NodeType type, ref RegisterItem? itemOut)
        {
            // now get the required register item

            UInt32 nextSiblingId = 0;

            RegisterItem? current = null;

            if (parent.headId == 0) return true; // there are no children so abort and return empty list

            // get the head
            if (Register.FindNode(ctx, parent.headId, ref current) < 0)
                return false; // end of chain or error or no children present in parent, abort

            if (current == null)
                return false; // there is no child node present so abort

            nextSiblingId = current.Id;

            while (true)
            {
                // load item from registry
                RegisterItem? nextItem = null;
                Int64 nextOffset = Register.FindNode(ctx, nextSiblingId, ref nextItem);
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
        public static bool FindRegisterItems(OpenFSDBContext? ctx,
            UInt32 parentId, NodeType type, bool useType, SpecialNodeType specialType, bool useSpecialType,
            DomainType domainType, bool useDomainType, ref List<RegisterItem>? listOut, bool loadNode)
        {

            List<RegisterItem> list = new List<RegisterItem>();

            // first get parent
            listOut = list;
            RegisterItem? parent = LoadSetupRegisterItem(ctx, parentId, false, false, false, false, false, false);
            if (parent == null) return false;
            if (parent.childrenCount == 0 && parent.headId == 0) return true;
            // now get the required register item

            UInt32 nextSiblingId = 0;

            RegisterItem? current = null;

            // get the head
            if (Register.FindNode(ctx, parent.headId, ref current) < 0)
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
                Int64 nextOffset = Register.FindNode(ctx, nextSiblingId, ref nextItem);
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
        public static RegisterItem? LoadSetupRegisterItem(OpenFSDBContext? ctx, RegisterItem? item, bool loadNode, bool loadRtf,
            bool loadChildren, bool loadChildrenConfigs, bool loadChildrenRegister, bool loadTreeSeqRegister)
        {
            return Register.LoadSetupRegisterItem(ctx, item.Id, loadNode, loadRtf, loadChildren, loadChildrenConfigs, loadChildrenRegister, loadTreeSeqRegister);
        }

        // this method loads and setups up a register item
        public static RegisterItem? LoadSetupRegisterItem(OpenFSDBContext? ctx, UInt32 id, bool loadNode, bool loadRtf,
            bool loadChildren, bool loadChildrenConfigs, bool loadChildrenRegister, bool loadTreeSeqRegister)
        {
            // initialize register
            RegisterItem? item = null;
            if (Register.FindNode(ctx, id, ref item) < 0) return null; // critical error
            if (item == null) return null; // critical error

            item.node = null;
            if (loadNode) item.loadNode(ctx, ref item.rtf, ref item.xamlbytes, loadRtf);

            if (loadChildrenRegister)
            {
                item.children = new ChildrenRegister();
                if (!ChildrenRegister.Initialize(ctx, item.children, item))
                    return null; // critical error

                if (loadChildren)
                    item.children.GetChildren(ref item.childrenList, loadChildrenConfigs);

            }

            if (loadTreeSeqRegister)
            {
                // tree sequence register begins from root and is present in every node
                item.tree = new TreeSequenceRegister();
                if (!TreeSequenceRegister.Initialize(ctx, item.tree, item))
                    return null; // critical error
            }

            // done
            return item;
        }
        // this method joins item's last tail's next to another item and returns original next's id
        public static bool JoinNext(OpenFSDBContext? ctx, UInt32 id, UInt32 nextId)
        {
            // 1: fetch latest states
            RegisterItem? item = Register.LoadSetupRegisterItem(ctx, id, false, false, false, false, true, true);
            RegisterItem? next = Register.LoadSetupRegisterItem(ctx, nextId, false, false, false, false, true, true);

            // check if next is root, if root then do not update root but only item or tail's next to 0

            // 2: get this item's absolute and last tail
            RegisterItem? tail = item.tree.GetAbsoluteLastTreeSequenceTail();
            if (tail == null) tail = item; // if there is no tree use item directly

            if (nextId == 0)
            {
                // next is 0 means dead end so there is nothing next
                // next is 0 or root so do not update root's prev
                tail.nextId = 0;
                Register.UpdateNode(ctx, tail, 0, false, 0, false, 0, false);
                return true;
            }

            // 3: if tail is null means no tree in item so join to item itself. if tail then join to tail. backup original next id and configure item and tail and the next node
            //if (tail != null)
            //{

            // means there is absolute last tail in item, so configure it and the next
            // means next isn't 0 or root but a real node so update it
            tail.nextId = next.Id;
            next.prevId = tail.Id;
            Register.UpdateNode(ctx, tail, 0, false, 0, false, 0, false);
            Register.UpdateNode(ctx, next, 0, false, 0, false, 0, false);
            //}
            //else
            //{
            // means there is no tree in item so configure item itself and the next
            //item.nextId = next.Id;
            //next.prevId = item.Id;
            //Register.UpdateNode(ctx, item, 0, false, 0, false, 0, false);
            //Register.UpdateNode(ctx, next, 0, false, 0, false, 0, false);
            //}
            return true;
        }
        // this method joins item's prev to another item and returns original prev's id
        public static bool JoinPrev(OpenFSDBContext? ctx, UInt32 id, UInt32 prevId)
        {
            // 1: fetch latest states
            RegisterItem? item = Register.LoadSetupRegisterItem(ctx, id, false, false, false, false, true, true);
            RegisterItem? prev = Register.LoadSetupRegisterItem(ctx, prevId, false, false, false, false, true, true);

            // means there is no tree in item so configure item itself and the next
            if (item.Id == 0)
            {
                // means item is root, we cannot set root's previous because it creates integral corruption.
                // means dead end we cannot change root's previous
                if (prev.Id == 0) return false; // both cannot be dead end or root so return error

                prev.nextId = 0;
                Register.UpdateNode(ctx, prev, 0, false, 0, false, 0, false);
                return true;
            }

            // means item is not root but real ordinary node so we change both.
            item.prevId = prev.Id;
            prev.nextId = item.Id;
            Register.UpdateNode(ctx, item, 0, false, 0, false, 0, false);
            Register.UpdateNode(ctx, prev, 0, false, 0, false, 0, false);
            return true;
        }
        // this method properly moves a parent item with a tree to another location and preserves the tree sequence.
        public static bool MoveParentProper(OpenFSDBContext? ctx, RegisterItem? item, UInt32 dstParentId)
        {
            /*
             * these things are to be modified:
             * source parent, destination parent, item's prev, item's last tail's next, destination last tail's next.
             * so =
             * 1. load source and destination parents.
             * 2. load item's prev and last tail's next.
             * 3. remove the item from tree sequence by joining both original endpoints to each other. this removes the item.
             * 
             * 
             * if source parent is empty then that parent itself is affected it's next is changed not previous. it is joined to item's prev.
             * if source parent has a tree then it's absolute last tail is affected it's next is changed not previous. it is joined to item's prev.
             * if destination parent is empty then that parent itself is affected it's next is changed not previous. it is joined to item's prev.
             * if destination parent has a tree then it's absolute last tail is affected it's next is changed not previous. it is joined to item's prev.
             * original item's prev and item's next if item has tree otherwise item's last tail's next are changed and original next is joined to original item's prev. both are affected.
             * original destination parent's next if empty or last tail's next if a tree is joined to item's next if empty or item's last tail's next if there is a tree.
             * if item is empty then item itself is affected it's previous and next are changed.
             * if item has a tree then item's prev is changed and item's absolute last tail is changed.
             * so destination parent's original next if destination parent empty or destination parent's original last tail and original next are also affected therefore changed.
             * 
             * join destination last tail's next to item and item's prev to destination last tail.
             * join destination last tail's original next to item's last tail next.
             * join item's last tail's original next to item's original prev. so that entire item and it's tree is removed then moved at once.
             * if tail is missing means there is no tree so we use the item or the parent itself. we configure prev and next.
             * */

            if (item.Id == 0) return true; // root cannot be moved so skip

            // 1: fetch latest states
            item = Register.LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            RegisterItem? srcParent = Register.LoadSetupRegisterItem(ctx, item.parentId, false, false, false, false, true, true);
            RegisterItem? dstParent = Register.LoadSetupRegisterItem(ctx, dstParentId, false, false, false, false, true, true);

            // phase 1: join item's previous and item's last tail's next to each other so that item and it's tree is removed from between.
            // item's prev is considered previous from entire item and it's tree in tree sequence. and last tail's next is considered an alien node outside item's tree scope.

            UInt32 id = item.Id;
            UInt32 sectionId = item.sectionId;

            // get next
            RegisterItem? tail = item.tree.Last();
            UInt32 nextId = item.nextId;
            if (tail != null) // if item has a tree then it's last tail is affected.
                nextId = tail.nextId;

            // now join next to item's prev
            JoinPrev(ctx, nextId, item.prevId);

            // phase 2: remove item from parent's children register
            item = Register.LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            srcParent = Register.LoadSetupRegisterItem(ctx, item.parentId, false, false, false, false, true, true);
            srcParent.children.Remove(item);
            item = Register.LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            srcParent = Register.LoadSetupRegisterItem(ctx, item.parentId, false, false, false, false, true, true);
            dstParent = Register.LoadSetupRegisterItem(ctx, dstParentId, false, false, false, false, true, true);

            // phase 3: join item to destination's last tail and then destination's last tail's next to item's last tail's next
            // get destination parent's last tail
            RegisterItem? dstTail = dstParent.tree.Last();
            if (dstTail == null) dstTail = dstParent; // if there is no tree in destination parent then destination parent is itself affected and changed.
            UInt32 dstNextId = dstTail.nextId;
            // first join destination last tail to back of item
            JoinPrev(ctx, item.Id, dstTail.Id);
            // now join destination parent if empty otherwise last tail if tree to the item and item's next if empty otherwise item's last tail to destination last next
            JoinNext(ctx, item.Id, dstNextId);

            // phase 4: add the item to the destination parent's children register
            item = Register.LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            dstParent = Register.LoadSetupRegisterItem(ctx, dstParentId, false, false, false, false, true, true);
            dstParent.children.Add(item);
            item = Register.LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            dstParent = Register.LoadSetupRegisterItem(ctx, dstParentId, false, false, false, false, true, true);

            // final phase 5: move node to destination location physically in db config files
            return entryMethods.DBChangeNodeParentOFSDB(ctx, sectionId, id, dstParentId);
        }


        // this method chains up all the empty slots into the parent empty slots system node's register
        public static bool BuildEmptySlotsRegister(OpenFSDBContext? ctx, RegisterItem emptySlots)
        {
            // phase 1 - load the entire register into memory
            /*
            byte[]? buffer = File.ReadAllBytes(file);
            if (buffer == null) return false;
            if (buffer.Length == 0) return false;
            MemoryStream ms = new MemoryStream(buffer);
            ms.Position = 0;
            */
            ctx.regFileStream.Position = 0;
            BinaryWriter bw = new BinaryWriter(ctx.regFileStream);

            RegisterItem? firstChild = null;
            RegisterItem? lastChild = null;
            Int64 totalSlots = Register.default_totalPreallocatedNodes;
            UInt32 id = 1; // we cannot change root node which is 0 so we start from 1
            UInt32 emptySlotsFound = 0;

            while (id < totalSlots)
            {
                // set position to current slot by id
                ctx.regFileStream.Position = RegisterItem.blockSize * id;
                // read slot into register item
                RegisterItem? current = RegisterItem.convertFromBytesStream(ctx.regFileStream);
                if (current.domainType != DomainType.EmptySlot)
                {
                    // this is an allocated slot used by some item, so skip this slot
                    id++;
                    continue;
                }
                
                // this is an empty zeroed slot, so take it
                if (firstChild == null) firstChild = current; // if this is the first empty slots child set it as first child

                current.headId = current.tailId = 0;
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
                    ctx.regFileStream.Position = lastChild.position;
                    bw.Write(RegisterItem.convertToBytes(lastChild));
                }
                else
                {
                }
                // update current in buffer
                current.nextSiblingId = 0;
                current.Id = id;
                current.sectionId = 0;
                current.parentId = emptySlots.Id;
                current.nodeType = NodeType.EmptySlot;
                current.specialNodeType = SpecialNodeType.EmptySlot;
                current.domainType = DomainType.EmptySlot;
                // update current node in buffer
                ctx.regFileStream.Position = current.position;
                bw.Write(RegisterItem.convertToBytes(current));
                // set current as the last child item in chain so we can process it in next id
                lastChild = current;
                emptySlotsFound++;
                id++;
            }
            // finally update parent node
            if (firstChild != null)
                emptySlots.headId = firstChild.Id;
            else
                emptySlots.headId = emptySlots.tailId = 0;

            if (lastChild != null)
                emptySlots.tailId = lastChild.Id;
            else
                emptySlots.tailId = emptySlots.headId; // we apply first child id from parent in both first and last child ids

            // update parent node in buffer
            emptySlots.childrenCount = emptySlotsFound;
            ctx.regFileStream.Position = emptySlots.position;
            bw.Write(RegisterItem.convertToBytes(emptySlots));
            bw.Flush();
            ctx.regFileStream.Flush();
            // now finally change the file
            //File.WriteAllBytes(file, buffer);
            //GC.Collect();
            return true;
        }
        
        public static void test(OpenFSDBContext? ctx, String file)
        {
            /*
            RegisterItem? root = LoadSetupRegisterItem(ctx, 0, true, false, true, true, true, false);
            RegisterItem? lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
            RegisterItem? jrn = LoadSetupRegisterItem(ctx, 1, true, false, true, true, true, true);
            RegisterItem? emptySlots = entryMethodsNewDesign.findRegistryEmptySlotSystemNodeItem(root.childrenList);
            emptySlots.InitializeSystem(ctx, true, true);

            myNode? node = new myNode(true);

            RegisterItem item = Insert(ctx, lib.Id, ref emptySlots, ref node, "");
            RegisterItem item2 = Insert(ctx, lib.Id, ref emptySlots, ref node, "");
            RegisterItem item3 = Insert(ctx, lib.Id, ref emptySlots, ref node, "");
            for (int i = 0; i < 30; i++)
            {
                node = new myNode(true);
                RegisterItem item4 = Insert(ctx, item.Id, ref emptySlots, ref node, "");
                RegisterItem item5 = Insert(ctx, item4.Id, ref emptySlots, ref node, "");
                RegisterItem item6 = Insert(ctx, item5.Id, ref emptySlots, ref node, "");
                RegisterItem item7 = Insert(ctx, item6.Id, ref emptySlots, ref node, "");

            }
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            root = LoadSetupRegisterItem(ctx, root.Id, false, false, false, false, true, true);
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);

            Move(ctx, item.Id, 0);
            Move(ctx, item.Id, 1);

            String rtf = "";
            myNode? nodetest = entryMethods.DBFindLoadNodeOFSDB(ctx.dbCtx, item.Id, ref rtf, false, item.DirectorySectionId);

            jrn = LoadSetupRegisterItem(ctx, 1, true, false, true, true, true, true);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            root = LoadSetupRegisterItem(ctx, root.Id, false, false, false, false, true, true);
            List<RegisterItem> item20tree = null;
            RegisterItem? item20 = LoadSetupRegisterItem(ctx, 20, false, false, false, false, true, true);
            item20.tree.GetDescendants(item20, ref item20tree);
            //BuildCustomLineageStructure(ctx, item);
            //Register.Delete(ctx, 20, ref emptySlots, false);
            Move(ctx, 32, 1);
            Move(ctx, item.Id, 35);
            Register.Delete(ctx, item.Id, ref emptySlots, false);
            Move(ctx, 32, 0);
            Move(ctx, 32, 2);
            Register.Delete(ctx, 2, ref emptySlots, false);
            emptySlots = LoadSetupRegisterItem(ctx, emptySlots.Id, true, false, true, true, true, true);
            jrn = LoadSetupRegisterItem(ctx, 1, true, false, true, true, true, true);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            root = LoadSetupRegisterItem(ctx, root.Id, false, false, true, false, true, true);

            List<RegisterItem> item35tree = null;
            RegisterItem? item35 = LoadSetupRegisterItem(ctx, 35, false, false, false, false, true, true);
            item35.tree.GetDescendants(item35, ref item35tree);
            List<RegisterItem> item35children = null;
            item35.children.GetChildren(ref item35children, false);

            List<RegisterItem> item32tree = null;
            RegisterItem? item32 = LoadSetupRegisterItem(ctx, 32, false, false, false, false, true, true);
            item32.tree.GetDescendants(item32, ref item32tree);
            List<RegisterItem> item32children = null;
            item32.children.GetChildren(ref item32children, false);

            List<RegisterItem> itemtree = null;
            List<RegisterItem> rootchildren = null;
            root.children.GetChildren(ref rootchildren, false);
            List<RegisterItem> itemchildren = null;
            item.children.GetChildren(ref itemchildren, false);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            item.tree.GetWholeTree(ref itemtree, false);

            List<RegisterItem> jrnchildren = null;
            List<RegisterItem> jrntree = null;
            List<RegisterItem> libtree = null;
            jrn.children.GetChildren(ref jrnchildren, false);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            item.tree.GetWholeTree(ref itemtree, false);
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
            List<RegisterItem> libchildren = null;
            lib.children.GetChildren(ref libchildren, false);
            jrn.tree.GetWholeTree(ref jrntree, false);
            lib.tree.GetWholeTree(ref libtree, false);

            return;
            */

            /*
            //List<RegisterItem> rootchildren = null;
            //root.children.GetChildren(ref rootchildren, false);
            //item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            //item.tree.GetWholeTree(ref itemtree, false);

            root = LoadSetupRegisterItem(ctx, root.Id, false, false, false, false, true, true);
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            Register.Move(ctx, item.Id, root.Id);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            root = LoadSetupRegisterItem(ctx, root.Id, false, false, false, false, true, true);
            //List<RegisterItem> rootchildren = null;
            root.children.GetChildren(ref rootchildren, false);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            item.tree.GetWholeTree(ref itemtree, false);
            */

            //List<RegisterItem> libchildren = null;

            /*
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false);
            lib.children.Remove(item);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false);
            root.children.Add(item, root);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false);
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true);
            lib.children.GetChildren(ref libchildren, false);

            List<RegisterItem> itemtree = null;
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false);
            item.tree.GetWholeTree(ref itemtree, false);
            List<RegisterItem> itemchildren = null;
            item.children.GetChildren(ref itemchildren, false);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false);
            item2 = LoadSetupRegisterItem(ctx, item2.Id, false, false, false, false);
            item.tree.MoveDescendants(item, false, item2.tree.rootAncestor);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false);
            item2 = LoadSetupRegisterItem(ctx, item2.Id, false, false, false, false);
            List<RegisterItem> item2tree = null;
            item2.tree.GetWholeTree(ref item2tree, false);
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true);
            List<RegisterItem> libtree = null;
            lib.tree.GetWholeTree(ref libtree, false);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false);
            item.tree.GetWholeTree(ref itemtree, false);
            item.children.GetChildren(ref itemchildren, false);
            libchildren = null;
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true);
            lib.children.GetChildren(ref libchildren, false);
            */

            /* 05:49 PM, 03 January 2025: i discovered that = 
             * integral synchronization is mandatory in every step. we change lib and we change item, in both changes in every step
             * intregral synchronization by reloading the latest state register item from registers are mandatory. if even a single 
             * integral synchronization is missing then integral corruption will happen and the register and db will be integrally corrupted.
             * when we add item to lib, when we remove item from lib, both item and lib are integrally changed, 
             * so we are required to reload both of them their latest states from register.
            // step 1 =
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            lib.children.Remove(item);
            // step 2 =
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
            item.tree.MoveDescendants(item, false, lib.tree.rootAncestor);
            // step 3 =
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
            lib.children.Add(item, lib);
            // step 4 =
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            lib.tree.Add(item);
            // step 5 =
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
            List<RegisterItem> libtree = null;
            lib.tree.GetWholeTree(ref libtree, false);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            libchildren = null;
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
            lib.children.GetChildren(ref libchildren, false);
            List<RegisterItem> itemtree = null;
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            item.tree.GetWholeTree(ref itemtree, false);
            List<RegisterItem> itemchildren = null;
            item.children.GetChildren(ref itemchildren, false);

            // step 1 =
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            lib.children.Remove(item);
            // step 2 =
            //lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true);
            //item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false);
            //lib.tree.Remove(item);
            // step 3 =
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            BuildCustomLineageStructure(ctx, item);
            // we cannot move descendants to root because we cannot put tree in there.
            //item.tree.MoveDescendants(item, false, root.tree.rootAncestor);
            // step 4 =
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            root = LoadSetupRegisterItem(ctx, root.Id, false, false, false, false, true, true);
            root.children.Add(item, root);
            // step 5 =
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            root = LoadSetupRegisterItem(ctx, root.Id, false, false, false, false, true, true);
            List<RegisterItem> rootchildren = null;
            root.children.GetChildren(ref rootchildren, false);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            item.tree.GetWholeTree(ref itemtree, false);
            // step 5 =
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            root = LoadSetupRegisterItem(ctx, root.Id, false, false, false, false, true, true);
            root.children.Remove(item);
            // step 5 =
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            root = LoadSetupRegisterItem(ctx, root.Id, false, false, false, false, true, true);
            item.tree.MoveDescendants(item, true, lib.tree.rootAncestor);
            // step 5 =
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            lib.children.Add(item, lib);
            // step 5 =
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
            lib.tree.GetWholeTree(ref libtree, false);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false, true, true);
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true, true, true);
            lib.children.GetChildren(ref libchildren, false);
            return;
             */

            /*
            lib = LoadSetupRegisterItem(ctx, 2, true, false, true, true);
            //lib.tree.MoveDescendants(item, true, lib);
            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false);

            RegisterItem? parent = LoadSetupRegisterItem(ctx, item.parentId, false, false, false, false);
            parent.children.Move(item, lib);

            item = LoadSetupRegisterItem(ctx, item.Id, false, false, false, false);
            lib = LoadSetupRegisterItem(ctx, 2, false, false, false, false);
            List<RegisterItem> treelib = null;
            lib.tree.GetWholeTree(ref treelib, false);
            List<RegisterItem> children2 = null;
            item.children.GetChildren(ref children2, false);
            List<RegisterItem> itemtree = null;
            item.tree.GetWholeTree(ref itemtree, false);
            return;
            */

            /*
            RegisterItem? jrn = LoadSetupRegisterItem(ctx, 1, false, false, false, false);
            RegisterItem? lib = LoadSetupRegisterItem(ctx, 2, false, false, false, false);

            myNode? node = new myNode(true);
            RegisterItem? emptySlots = null;
            RegisterItem item = Insert(ctx, jrn.Id, ref emptySlots, ref node, "");
            RegisterItem item0 = item;
            RegisterItem rootAncestor = item.tree.rootAncestor;

            //item0 = LoadSetupRegisterItem(ctx, item0.Id, false, false, false, false);

            for (int i = 0; i < 10; i++)
            {
                //item0 = LoadSetupRegisterItem(ctx, item0.Id, false, false, false, false);
                //item0.tree.rootAncestor = rootAncestor;
                node = new myNode(true);
                RegisterItem item1 = Insert(ctx, item0.Id, ref emptySlots, ref node, "");

                //RegisterItem parent = LoadSetupRegisterItem(ctx, item0.Id, false, false, false, false);
                //continue;
                //item1.tree.rootAncestor = rootAncestor;
                //node = new myNode(true);
                //item1 = Insert(ctx, item1, ref emptySlots, ref node, "");
            }
            Register.FindNode(ctx, item0.Id, ref item0);

            item0 = LoadSetupRegisterItem(ctx, item0.Id, false, false, false, false);
            List<RegisterItem> tree = null;
            item0.tree.GetWholeTree(ref tree, false);
            List<RegisterItem> children = null;
            item0.children.GetChildren(ref children, false);

            jrn = LoadSetupRegisterItem(ctx, 1, false, false, false, false);
            item0 = LoadSetupRegisterItem(ctx, item0.Id, false, false, false, false);
            item0.tree.MoveDescendants(item0, ref lib);
            item0 = LoadSetupRegisterItem(ctx, item0.Id, false, false, false, false);
            jrn = LoadSetupRegisterItem(ctx, 1, false, false, false, false);
            lib = LoadSetupRegisterItem(ctx, 2, false, false, false, false);
            List<RegisterItem> treelib = null;
            lib.tree.GetWholeTree(ref treelib, false);
            List<RegisterItem> treejrn = null;
            jrn.tree.GetWholeTree(ref treejrn, false);
            List<RegisterItem> children2 = null;
            item0.children.GetChildren(ref children2, false);
            */
            return;
        }
        #endregion 
    }

    public class ChildrenRegister
    {
        public RegisterItem? parent;
        
        public RegisterItem? first;
        public RegisterItem? current;
        public RegisterItem? last;


        // vhd handle and configuration which is to be used whenever operating this double linked list engine
        public OpenFSDBContext? ctx = null;
        public ChildrenRegister()
        {
        }
        public UInt32 Count
        {
            get
            {
                parent = Register.LoadSetupRegisterItem(ctx, parent.Id, false, false, false, false, true, true);
                return parent.childrenCount;
            }
        }

        /*
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
        */

        // this method deletes the node from parent's register and adds it to unused slots register if required
        public bool Delete(RegisterItem item, ref RegisterItem? emptySlotsItem, bool withoutEmptySlotsRegister, bool isEmptySlot, bool commitUsedSlots)
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

            UInt32 id = item.Id;
            UInt32 sectionId = item.sectionId;

            Register.FindNode(ctx, item.Id, ref item);
            parent = Register.LoadSetupRegisterItem(ctx, parent.Id, false, false, false, false, true, true);

            if (parent.headId == 0) return false; // error there is no child in this parent, abort with error
            if (item.parentId != parent.Id) return false; // error this item is not in this parent

            // phase 2 - remove from parent tree sequence and parent's register
            if (!isEmptySlot)
            {
                if (!parent.tree.Remove(item)) return false;
                Register.FindNode(ctx, item.Id, ref item);
                parent = Register.LoadSetupRegisterItem(ctx, parent.Id, false, false, false, false, true, true);
            }

            if (!Remove(item)) return false; // remove from children

            // phase 3 - delete node and register in empty slots system node register or simply complete delete()
            RegisterItem? newEmptySlotItem = null;
            if (withoutEmptySlotsRegister)
            {
                if (!Register.DeleteNode(ctx, item.Id))
                    return false; // if error abort with error
            }
            else
            {
                if (!Register.WriteEmptySlot(ctx, ref emptySlotsItem, item.Id, ref newEmptySlotItem))
                    return false; // error abort
            }

            // phase 4 - change in root
            if (!isEmptySlot)
            {
                // update used slots config
                ctx.usedSlots -= 1;
                if (commitUsedSlots) ctx.writeUsedSlotsFile();

                // final phase 5 - delete node's files
                return entryMethods.DBPurgeNodeOFSDB(ctx, id, sectionId, true, true);
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
            if (item.parentId != parent.Id) return false; // error this item is not in this parent

            Register.FindNode(ctx, parent.Id, ref parent);
            if (parent.headId == 0) return false; // error there is no child in this parent, abort with error

            // phase 2 - get both head and tail items
            RegisterItem? head = null;
            RegisterItem? tail = null;
            if (parent.headId != 0) Register.FindNode(ctx, parent.headId, ref head);
            if (parent.tailId != 0) Register.FindNode(ctx, parent.tailId, ref tail);

            // phase 3 - get this node's previous and next nodes in chain
            RegisterItem? prev = null;
            RegisterItem? next = null;
            if (item.previousSiblingId != 0) Register.FindNode(ctx, item.previousSiblingId, ref prev);
            if (item.nextSiblingId != 0) Register.FindNode(ctx, item.nextSiblingId, ref next);

            // phase 4 - decide and do
            if (item.Id == parent.headId && item.Id == parent.tailId)
            {
                // this item is head node and one and only first node there is no other node

                // first edit the item
                item.parentId = 0;
                item.previousSiblingId = item.nextSiblingId = 0;
                Register.UpdateNode(ctx, item, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.headId = 0;
                parent.tailId = 0;
                parent.childrenCount--;
                Register.UpdateNode(ctx, parent, 0, false, 0, false, 0, false);
                // no more child node left so both head and tail are 0

            }
            else if (item.Id == parent.headId && item.Id != parent.tailId)
            {
                // this item is head node or the first node but not the tail node, so there are 2+ nodes

                // first edit the item
                item.parentId = 0;
                item.previousSiblingId = item.nextSiblingId = 0;
                Register.UpdateNode(ctx, item, 0, false, 0, false, 0, false);

                // configure this node's next linked node and update it
                next.previousSiblingId = 0; // because there is no previous node
                Register.UpdateNode(ctx, next, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.headId = next.Id; // next node becomes the first node or the head
                parent.childrenCount--;
                Register.UpdateNode(ctx, parent, 0, false, 0, false, 0, false);

            }
            else if (item.Id == parent.tailId && item.Id != parent.headId)
            {
                // this item is tail or the last node but there is a different head, head and tail 2 or more nodes means there are
                // 2+ nodes, tail can be 2nd node or 3rd or 3+ node

                // first edit the item
                item.parentId = 0;
                item.previousSiblingId = item.nextSiblingId = 0;
                Register.UpdateNode(ctx, item, 0, false, 0, false, 0, false);

                // we update the previous node in chain remove this node's link and set the previous node as tail
                prev.nextSiblingId = 0; // because previous node becomes tail or the last node and there is no next node
                Register.UpdateNode(ctx, prev, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.tailId = prev.Id; // prev node becomes the last node or the tail. it is also already head if it is first node
                parent.childrenCount--;
                Register.UpdateNode(ctx, parent, 0, false, 0, false, 0, false);
            }
            else
            {
                // this item is neither head nor tail node means this node is a middle node which exists between the node chain

                // first edit the item
                item.parentId = 0;
                item.previousSiblingId = item.nextSiblingId = 0;
                Register.UpdateNode(ctx, item, 0, false, 0, false, 0, false);

                // now join both prev and next nodes into each other and remove the deleted node links
                prev.nextSiblingId = next.Id; // previous's next is deleted node's next, we join both
                next.previousSiblingId = prev.Id; // next's previous is deleted node's previous, we join both
                // deleted node's links removed now update both nodes
                Register.UpdateNode(ctx, next, 0, false, 0, false, 0, false);
                Register.UpdateNode(ctx, prev, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.childrenCount--;
                Register.UpdateNode(ctx, parent, 0, false, 0, false, 0, false);
            }
            return true;
        }
        // this method removes the item from this parent and inserts/moves it to the another parent
        public bool Move(RegisterItem item, RegisterItem targetParent)
        {
            // validation
            if (ctx.readOnly) return false;
            if (parent.headId == 0) return false; // error there is no child in this parent, abort with error
            if (item.parentId != parent.Id) return false; // error this item is not in this parent

            // validations
            if (targetParent.childrenCount >= Register.default_maxChildrenNodes)
                return false;

            return Register.MoveParentProper(ctx, item, targetParent.Id);
        }

        public bool Add(RegisterItem? item)
        {
            // if 1st child, insert at head and tail, update
            // if 2nd forth child, insert at tail, update tail
            // update parent's configuration - first if 1st node, or last child if last last node
            // update previous node's next node offset to this next node.
            if (ctx.readOnly) return false;

            parent = Register.LoadSetupRegisterItem(ctx, parent.Id, true, false, false, false, true, true);

            if (parent.childrenCount == 0)
            {
                // this is first child being inserted, so insert it and it's offset as head and tail both
                item.parentId = parent.Id;
                item.previousSiblingId = 0;
                item.nextSiblingId = 0;
                Int64 newItemOffset = Register.InsertNode(ctx, item);
                if (newItemOffset < 0) return false; // critical error abort with error

                // finally update parent node
                parent.headId = item.Id;
                parent.tailId = item.Id;
                parent.childrenCount++;
                Register.UpdateNode(ctx, parent, 0, false, 0, false, 0, false);
                // done
            }
            else if (parent.childrenCount == 1)
            {
                // get head
                RegisterItem? head = null;
                Int64 headOffset = Register.FindNode(ctx, parent.headId, ref head);
                if (headOffset < 0) return false; // critical error abort with error

                // this is a 2nd node to be inserted, there is 1 first node which is head. so we set tail as this node.
                item.parentId = parent.Id;
                item.previousSiblingId = head.Id;
                item.nextSiblingId = 0;
                Int64 newItemOffset = Register.InsertNode(ctx, item);
                if (newItemOffset < 0) return false; // critical error abort with error

                // update previous head node 
                head.nextSiblingId = item.Id;
                Register.UpdateNode(ctx, head, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.tailId = item.Id;
                parent.childrenCount++;
                Register.UpdateNode(ctx, parent, 0, false, 0, false, 0, false);
            }
            else
            {
                // this is 3+ index child so insert it as tail and update the previous tail cum last item with this new item
                // get tail
                RegisterItem? tail = null;
                Int64 tailOffset = Register.FindNode(ctx, parent.tailId, ref tail);
                if (tailOffset < 0) return false; // critical error abort with error

                // update and insert this node with new configuration
                item.previousSiblingId = tail.Id;
                item.nextSiblingId = 0;
                item.parentId = parent.Id;
                Int64 newItemOffset = Register.InsertNode(ctx, item);
                if (newItemOffset < 0) return false; // critcial error abort with error

                // update previous tail node 
                tail.nextSiblingId = item.Id;
                Register.UpdateNode(ctx, tail, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.tailId = item.Id;
                parent.childrenCount++;
                Register.UpdateNode(ctx, parent, 0, false, 0, false, 0, false);
                // done
            }

            // success
            return true;
        }

        // get first child of parent
        public RegisterItem? First()
        {
            // get first head child in parent which should be first head in tree sequence of parent
            // note that if there is a single child then both head and tail and current are same item.
            if (parent.headId == 0) return null;
            Register.FindNode(ctx, parent.headId, ref first);
            Register.FindNode(ctx, parent.tailId, ref last);
            current = first;
            return current;
        }
        // get last child of parent
        public RegisterItem? Last()
        {
            // get tail child in parent which should be last child in parent
            // note that if there is a single child then both head and tail and current are same item.
            if (parent.headId == 0) return null;
            Register.FindNode(ctx, parent.headId, ref first);
            Register.FindNode(ctx, parent.tailId, ref last);
            current = last;
            return current;
        }
        
        // we get all children
        public bool GetChildren(ref List<RegisterItem>? listOut, bool loadNode)
        {
            RegisterItem? original = current;

            List<RegisterItem>? list = new List<RegisterItem>();
            listOut = list;

            // there are children, iterate through them and find valid nodes
            current = null;
            while (true)
            {
                RegisterItem? nextItem = Next();
                if (nextItem == null) break;

                String? rtf = "";
                byte[]? xaml = null;
                if (loadNode)
                    nextItem.loadNode(ctx, ref rtf, ref xaml, loadNode);

                list.Add(nextItem); // yes this is a child and exists under parent
            }
            current = original;
            listOut = list;
            return true;
        }
        public RegisterItem? Next()
        {
            if (current == null)
            {
                // current is not loaded, so get first head node as current
                return First();
            }

            // get and set next of current
            if (current.nextSiblingId == 0) return null; // eof return null

            // get next 
            RegisterItem? item = null;
            // load item from registry
            Int64 offset = Register.FindNode(ctx, current.nextSiblingId, ref item);
            if (offset < 0) return null; // no more nodes or error so break
            current = item;
            return current;
        }

        public RegisterItem? Previous()
        {
            if (current == null)
            {
                // current is not loaded, so get first head node as current
                return Last();
            }

            // get and set prev of current
            if (current.previousSiblingId == 0) return null; // eof return null

            // get next 
            RegisterItem? item = null;
            // load item from registry
            Int64 offset = Register.FindNode(ctx, current.previousSiblingId, ref item);
            if (offset < 0) return null; // no more nodes or error so break
            current = item;
            return current;
        }
        public void Reset()
        {
            current = first = last = null;
        }

        public static bool Initialize(OpenFSDBContext? ctx,
            ChildrenRegister list, RegisterItem parentItem)
        {
            // phase 1 - configure and setup

            list.ctx = ctx;
            list.current = list.first = list.last = null;

            // directly setup count from parent item
            list.parent = parentItem;
            return true;
        }

    }

    public class TreeSequenceRegister
    {
        public RegisterItem? parent = null;
        public RegisterItem? current = null;
        public RegisterItem? first = null;
        public RegisterItem? last = null;

        // vhd handle and configuration which is to be used whenever operating this double linked list engine
        public OpenFSDBContext? ctx = null;

        public TreeSequenceRegister()
        {
        }
        public UInt32 Count
        {
            // get latest tree descendants count located under this parent ancestor
            get
            {
                // fetch latest parent state
                // correction on 16 Feb 2026
                //Register.FindNode(ctx, parent.Id, ref parent);
                return CountDescendants();
                //return parent.childrenCount;
            }
        }
        // traverses and finds and loads the absolute last tree sequence node under an ancestor
        public RegisterItem? GetAbsoluteLastTreeSequenceTail()
        {
            if (parent == null) return null;

            // fetch latest parent state
            Register.FindNode(ctx, parent.Id, ref parent);

            // check if there is any tree in this parent
             if (parent.childrenCount == 0)
                return null; // no tree in this parent so return null

            // there is a tree in this parent

            // note that a tree sequence tail is actually the children and in them the last registered child in tree sequence of parent.

            // if this parent is emptySlot system core node, then take it.
            if (parent.nodeType == NodeType.EmptySlot && parent.specialNodeType == SpecialNodeType.SystemNode)
                return parent; // empty slot core node

            // first get the initial tail and it's parent
            RegisterItem? tail = null;
            Register.FindNode(ctx, parent.tailId, ref tail);

            // if this tail is emptySlot system, then deny chaining it.
            if (tail.nodeType == NodeType.EmptySlot && tail.specialNodeType == SpecialNodeType.SystemNode)
                return tail; // empty slot core node

            if (tail.childrenCount == 0)
            {
                // tail has no further children means tree sequence
                return tail;
            }

            // tail has a tree so loop through it 
            while (true)
            {
                // load next tail child from this current parent tail child
                if (tail.tailId == 0) break; // no further tree this is the last final tree sequence tail so break;
                if (tail.childrenCount == 0) break; // no further tree this is the last final tree sequence tail so break;
                // there is still a tree this bottom so iterate to the next tree sequence tail
                Register.FindNode(ctx, tail.tailId, ref tail);
                if (tail == null) break;

                // if this absolute last tail is emptySlot system, then deny chaining it.
                if (tail.nodeType == NodeType.EmptySlot)
                    return null; // empty slot core node or empty slot node, deny it.
            }

            // here we have the absolute last tree sequence tail node. return tail;
            return tail;
        }

        // add an item in the last of parent's tree sequence
        public RegisterItem? Add(myNode? node)
        {
            if (ctx.readOnly) return null;
            if (node == null) return null;
            if (node.chapter == null) return null;

            // finally update the register add this node
            RegisterItem? item = new RegisterItem(0, node.chapter.Id,
                parent.Id, node.DirectorySectionID, 0, 0, 0, 0, 0, node.chapter.nodeType, node.chapter.specialNodeType, node.chapter.domainType,
                0, 0);
            if (Add(item))
                return item;
            else
                return null;
        }

        // add an item in the last of parent's tree sequence
        public bool Add(RegisterItem? item)
        {
            if (ctx.readOnly) return false;
            if (parent == null) return false;

            /* 1. load latest parent state
             * 2. find if parent has tree. if parent is empty directly use parent.
             * 3. if parent is not empty then find absolute last tail by jumping from every child to a dead end.
             * 4. join the absolute last tail to new node and join new node's next to original previous next pointer of the final tail.
             * 5. update parent's tail to this new inserted item 
             * */

            // 1: fetch latest parent state
            Register.FindNode(ctx, parent.Id, ref parent);

            // 2: find absolute last tail in parent. if no tail means no tree in parent then take the parent itself.
            RegisterItem? tail = GetAbsoluteLastTreeSequenceTail();
            if (tail == null)
                tail = parent; // if tail is null means parent has no tree, so we setup parent and it's next as this item.

            // fetch original previous next node from tail
            RegisterItem? nextInSequence = null;
            if (tail.nextId != 0) Register.FindNode(ctx, tail.nextId, ref nextInSequence);

            // 3: join everything
            tail.nextId = item.Id;
            item.prevId = tail.Id;
            item.nextId = 0;
            if (nextInSequence != null)
            {
                // there was a next node in tree sequence, so join it to item
                item.nextId = nextInSequence.Id;
                nextInSequence.prevId = item.Id;
            }
            // update all
            Register.UpdateNode(ctx, tail, 0, false, 0, false, 0, false);
            Register.UpdateNode(ctx, item, 0, false, 0, false, 0, false);
            Register.UpdateNode(ctx, nextInSequence, 0, false, 0, false, 0, false);
            // success
            return true;
        }
        // remove the item from parent's tree sequence
        public bool Remove(RegisterItem? item)
        {
            if (ctx.readOnly) return false;
            if (parent == null) return false;

            /* 1. load latest item and parent state
             * 2. find if parent has tree. if parent is empty abort with error
             * 3. load both adjucent nodes of item.
             * 4. configure both adjucent nodes and the item.
             * 5. reload item and parent.
             * 6. finally configure parent. head and tail are to be configured.
             * 
             * if children count is 1 then this item is both head and tail so set both to 0.
             * if children count is 2 then if this item is head then next item is tail, but if this item is tail then previous item is head. so when we remove
             * from head otherwise from tail then remaining single item is set as both head and tail.
             * if children count is 3 then if this item is head then next is 2nd and it's next is 3rd which is tail, and if this item is tail then it's previous is 2nd
             * and it's previous is 1st which is head. so when head we remove it from head and set next as head. so when tail we remove it from tail and set previous as tail.
             * if neither head nor tail then we just separate and remove the item from adjecent nodes and join them to each other. one of them should be head other should be tail. we
             * need not configure head and tail they remain unaffected.
             * if children count is 4 or more then if this item is head then it is ensured that next is not tail and last is tail so there is no need to configure tail we configure
             * head as the next in line. if this item is tail it is ensured that previous is not head and 1st in line is head so there is no need to configure head so we
             * configure tail as the previous in line.
             *  
             * */

            if (item.Id == 0) return false; // root cannot be removed so return error

            // 1: fetch latest states
            Register.FindNode(ctx, parent.Id, ref parent);
            Register.FindNode(ctx, item.Id, ref item);

            if (item.prevId == item.nextId) return false; // both 0 is error both same is error.

            // 2. find if parent has tree. if parent is empty abort with error
            if (parent.childrenCount == 0) return false;

            // 3. load both adjucent nodes of item.
            RegisterItem? prev = null;
            RegisterItem? next = null;
            Register.FindNode(ctx, item.nextId, ref next);
            Register.FindNode(ctx, item.prevId, ref prev);

            /* rules and logic
             * if item's next is root 0 then root 0's prev always remains 0 cannot be changed because there is nothing previous in root 0.
             * 
             * 
             * */

            // 4.configure both adjucent nodes and the item.
            item.prevId = item.nextId = 0;
            if (prev.Id == 0)
            {
                // prev is root 0 so it's prev cannot be changed and is set to 0 but it's next is changed on nonzero next
                prev.prevId = 0;
                if (next.Id > 0)
                {
                    // assuming root 0 should be assigned a valid next which is nonzero node then we assign it to root's next because it is mandatory if
                    // truly root 0 is being validly affected and if we don't assign it the nonzero next node then it causes integral corruption.
                    prev.nextId = next.Id;
                }
            }
            else
            {
                // prev is not root 0 but a nonzero node so change it
                prev.nextId = next.Id;
            }
            // if next is 0 means not root but a dead end. root 0 is most previous which is first ground node. so root's prev is always set to 0.
            if (next.Id == 0)
            {
                // next is root 0 then it's prev is set to 0 always as a rule.
                next.prevId = 0;
            }
            else
            {
                // next is not root 0 so change the next node
                next.prevId = prev.Id;
            }

            Register.UpdateNode(ctx, next, 0, false, 0, false, 0, false);
            Register.UpdateNode(ctx, item, 0, false, 0, false, 0, false);
            Register.UpdateNode(ctx, prev, 0, false, 0, false, 0, false);
            // reload parent
            Register.FindNode(ctx, parent.Id, ref parent);
            return true;
        }
        // delete the item and it's tree from parent's tree sequence
        public bool Delete(RegisterItem? item, RegisterItem? emptySlots)
        {
            if (ctx.readOnly) return false;
            if (parent == null) return false;
            if (item.Id == 0) return false; // root cannot be removed so return error
            if (item.nodeType == NodeType.EmptySlot) return false;
            if (item.specialNodeType == SpecialNodeType.SystemNode) return false;

            // 1: fetch latest states
            Register.FindNode(ctx, parent.Id, ref parent);
            Register.FindNode(ctx, item.Id, ref item);

            // 2. find if parent has tree. if parent is empty abort with error
            //if (parent.childrenCount == 0) return false;

            // 3. delete iterating from last to first in tree sequence
            RegisterItem? original = current;

            // there are descendants, iterate through them and find valid nodes
            current = null;
            while (true)
            {
                RegisterItem? prev = Previous();
                if (prev == null) break;
                if (prev.Id == 0) break; // dead end eof abort
                if (prev.Id == item.prevId) break; // eof we reached outside item to item's previous means end of sequence reached so abort.

                if (prev.nodeType == NodeType.EmptySlot) continue;
                if (prev.specialNodeType == SpecialNodeType.SystemNode) continue;

                // load item's parent
                RegisterItem? parentInLine = Register.LoadSetupRegisterItem(ctx, prev.parentId, false, false, false, false, true, true);

                // delete item from parent proper
                parentInLine.children.Delete(prev, ref emptySlots, false, false, false);
            }

            // finally delete the top item
            // load item's parent
            RegisterItem? itemparent = Register.LoadSetupRegisterItem(ctx, item.parentId, false, false, false, false, true, true);

            // delete item from parent proper
            itemparent.children.Delete(item, ref emptySlots, false, false, false);

            // finally commit updated config
            ctx.writeUsedSlotsFile();

            current = original;
            return true;
        }

        // get first item which is next to parent and is head in tree sequence of parent
        public RegisterItem? First()
        {
            // get first head child in parent which should be first head in tree sequence of parent
            // note that if there is a single child then both head and tail and current are same item.
            if (parent.headId == 0) return null;
            Register.FindNode(ctx, parent.headId, ref first);
            RegisterItem? tail = GetAbsoluteLastTreeSequenceTail();
            last = tail;
            current = first;
            return current;
        }
        // get absolute last tail in tree sequence of parent
        public RegisterItem? Last()
        {
            // get absolute last tail in parent
            // note that if there is a single child then both head and tail and current are same item.
            if (parent.headId == 0) return null;
            Register.FindNode(ctx, parent.headId, ref first);
            RegisterItem? tail = GetAbsoluteLastTreeSequenceTail();
            last = tail;
            current = tail;
            return current;
        }

        public UInt32 CountDescendants()
        {
            if (parent == null) return 0;
            Register.FindNode(ctx, parent.Id, ref parent);
            RegisterItem? original = current;
            UInt32 count = 0;
            current = null;
            while (true)
            {
                RegisterItem? nextItem = Next();
                if (nextItem == null) break;
                count++;
            }
            current = original;
            return count;
        }

        // we get all decendants in tree sequence of parent
        public bool GetDescendantTreeSequence(ref List<RegisterItem> listOut)
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
                
                list.Add(nextItem); // yes this is a descendant and exists under ancestor
            }
            current = original;
            return true;
        }
        public RegisterItem? Next()
        {
            if (current == null)
            {
                // current is not loaded, so get first head node as current
                return First();
            }

            // get and set next of current
            if (current.nextId == 0) return null; // eof return null
            if (current.Id == last.nextId) return null; // eof return null, because prev found item is outside top ancestor's tree scope.

            // get next 
            RegisterItem? item = null;
            // load item from registry
            Int64 offset = Register.FindNode(ctx, current.nextId, ref item);
            if (offset < 0) return null; // no more nodes or error so break
            if (item.Id == last.nextId) return null; // eof return null, because next found item is outside top ancestor's tree scope.
            // next found item is inside top ancestor's tree scope so assign it.
            current = item;
            return current;
        }

        public RegisterItem? Previous()
        {
            if (current == null)
            {
                // current is not loaded, so get last tail as current
                return Last();
            }

            // get and set previous of current
            if (current.prevId == 0) return null; // eof return null
            if (current.Id == first.prevId) return null; // eof return null, because prev found item is outside top ancestor's tree scope.

            // get prev 
            RegisterItem? item = null;
            // load item from registry
            Int64 offset = Register.FindNode(ctx, current.prevId, ref item);
            if (offset < 0) return null; // no more nodes or error so break
            // this previous node in tree sequence is descendant, so set and return it
            if (item.Id == first.prevId) return null; // eof return null, because prev found item is outside top ancestor's tree scope.
            // prev found item is inside top ancestor's tree scope so assign it.
            current = item;
            return current;
        }
        public void Reset()
        {
            current = first = last = null;
        }

        public static bool Initialize(OpenFSDBContext? ctx,
            TreeSequenceRegister? list, RegisterItem? parentItem)
        {
            // configure and setup

            list.ctx = ctx;
            list.current = list.first = list.last = null;

            // directly setup count from parent item
            list.parent = parentItem;
            return true;
        }

    }

}


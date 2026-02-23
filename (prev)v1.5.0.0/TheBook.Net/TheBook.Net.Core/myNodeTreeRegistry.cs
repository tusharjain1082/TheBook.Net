using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DiaryJournal.Net;
using System.Windows.Documents;

namespace TheBook.Net.Core
{
    public class myNodeTreeRegistry
    {
        public const Int64 default_RootNodeId = 0;

        public class myNodeTreeRegistryItem
        {
            public Int64 position = 0;

            // 36 bytes binary block is made of all these elements =
            public const int blockSize = ((sizeof(Int64) * 8) + (sizeof(Int32)));
            public Int64 Id = 0;
            public Int64 parentId = 0;
            public Int64 DirectorySectionId = 0;
            public Int64 parentOffset = 0;
            public Int64 firstChildOffset = 0;
            public Int64 lastChildOffset = 0;
            public Int64 previousSiblingOffset = 0;
            public Int64 nextSiblingOffset = 0;
            public Int32 childrenCount = 0;
            
            public myNode? node = null;
            public ChildrenRegister? children = null;

            public myNodeTreeRegistryItem()
            {

            }
            public myNodeTreeRegistryItem(Int64 position, Int64 id, Int64 parentid, Int64 sectionid, Int64 parentOffset, Int32 childrenCount, 
                Int64 firstChildOffset, Int64 lastChildOffset, Int64 nextSiblingOffset, Int64 previousSiblingOffset)
            {
                this.position = position;
                this.Id = id;
                this.parentId = parentid;
                this.DirectorySectionId = sectionid;
                this.parentOffset = parentOffset;
                this.childrenCount = childrenCount;
                this.firstChildOffset = firstChildOffset;
                this.lastChildOffset = lastChildOffset;
                this.nextSiblingOffset = nextSiblingOffset;
                this.previousSiblingOffset = previousSiblingOffset;
            }

            #region "framework"

            // load the node file and it's configuration into this collection's myNode object
            public bool loadNode(VirtualDiskFramework.VirtualDiskContext ctx, ref String rtfOut, bool loadData, bool setNode)
            {
                myNode? node = null;
                String? rtf = "";
                if (!entryMethodsNewDesign.DBFindLoadNode(ctx, this, ref rtf, loadData, ref node))
                    return false; // critical error abort with error

                if (node == null) return false;

                rtfOut = rtf;
                if (setNode) this.node = node;

                return true;
            }

            public static myNodeTreeRegistryItem? convertFromMyNode(myNode? node)
            {
                if (node == null) return null;
                if (node.chapter == null) return null;
                myNodeTreeRegistryItem item = new myNodeTreeRegistryItem();
                item.Id = node.chapter.Id;
                item.parentId = node.chapter.parentId;
                item.DirectorySectionId = node.DirectorySectionID;
                return item;
            }
            public static byte[]? convertToBytesFromMyNode(myNode? node)
            {
                if (node == null) return null;
                if (node.chapter == null) return null;
                MemoryStream ms = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(node.chapter.Id);
                bw.Write(node.chapter.parentId);
                bw.Write(node.DirectorySectionID);
                bw.Write((Int64)0);
                bw.Write((Int64)0);
                bw.Write((Int64)0);
                bw.Write((Int64)0);
                bw.Write((Int64)0);
                bw.Write((Int32)0);
                return ms.ToArray();
            }
            public static byte[] convertToBytes(Int64 id, Int64 parentId, Int64 DirectorySectionId, Int64 parentOffset, Int32 childrenCount,
                Int64 firstChildOffset, Int64 lastChildOffset, Int64 nextSiblingOffset, Int64 previousSiblingOffset)
            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(id);
                bw.Write(parentId);
                bw.Write(DirectorySectionId);
                bw.Write(parentOffset);
                bw.Write(firstChildOffset);
                bw.Write(lastChildOffset);
                bw.Write(nextSiblingOffset);
                bw.Write(previousSiblingOffset);
                bw.Write(childrenCount);
                return ms.ToArray();
            }
            public static byte[] convertToBytes(myNodeTreeRegistryItem item)
            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(item.Id);
                bw.Write(item.parentId);
                bw.Write(item.DirectorySectionId);
                bw.Write(item.parentOffset);
                bw.Write(item.firstChildOffset);
                bw.Write(item.lastChildOffset);
                bw.Write(item.nextSiblingOffset);
                bw.Write(item.previousSiblingOffset);
                bw.Write(item.childrenCount);
                return ms.ToArray();
            }
            public static myNodeTreeRegistryItem? convertFromBytesStream(Stream s)
            {
                BinaryReader br = new BinaryReader(s);
                try
                {
                    //if (br.PeekChar() == -1) return null; // end of stream
                    br.ReadByte();
                    s.Position--;
                }
                catch
                {
                    return null;
                }
                myNodeTreeRegistryItem item = new myNodeTreeRegistryItem();
                item.position = s.Position;
                item.Id = br.ReadInt64();
                item.parentId = br.ReadInt64();
                item.DirectorySectionId = br.ReadInt64();
                item.parentOffset = br.ReadInt64();
                item.firstChildOffset = br.ReadInt64();
                item.lastChildOffset = br.ReadInt64();
                item.nextSiblingOffset = br.ReadInt64();
                item.previousSiblingOffset = br.ReadInt64();
                item.childrenCount = br.ReadInt32();
                return item; // valid node is found
            }
            public static myNodeTreeRegistryItem? Root(Stream s)
            {
                Int64 pos = s.Position;
                s.Position = 0;
                myNodeTreeRegistryItem? item = convertFromBytesStream(s);
                if (item == null) return null;
                s.Position = pos;
                return item; // valid node is found
            }


            public static myNodeTreeRegistryItem? convertFromBytes(byte[] bytes)
            {
                MemoryStream ms = new MemoryStream(bytes);
                BinaryReader br = new BinaryReader(ms);
                myNodeTreeRegistryItem item = new myNodeTreeRegistryItem();
                item.Id = br.ReadInt64();
                item.parentId = br.ReadInt64();
                item.DirectorySectionId = br.ReadInt64();
                item.parentOffset = br.ReadInt64();
                item.firstChildOffset = br.ReadInt64();
                item.lastChildOffset = br.ReadInt64();
                item.nextSiblingOffset = br.ReadInt64();
                item.previousSiblingOffset = br.ReadInt64();
                item.childrenCount = br.ReadInt32();
                return item;
            }
            // this method updates root in the registry
            public static myNodeTreeRegistryItem? updateRoot(VirtualDiskFramework.VirtualDiskContext ctx, String file,
                Int32 childrenCount, bool useChildrenCount, bool incrementChildren, bool decrementChildren)
            {
                using (VirtualDiskFramework.VirtualDiskContext.VhdSystem? sys = VirtualDiskFramework.VirtualDiskContext.VhdSystem.CreateSystem(ctx.disk))
                {
                    if (sys == null) return null;
                    if (!sys.fs.FileExists(file)) return null;

                    try
                    {
                        using (Stream? s = sys.fs.OpenFile(file, FileMode.Open))
                        {
                            return updateRoot(s, childrenCount, useChildrenCount, incrementChildren, decrementChildren);
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            // this method updates root in the registry
            public static myNodeTreeRegistryItem? updateRoot(Stream s, Int32 childrenCount, bool useChildrenCount, bool incrementChildren, bool decrementChildren)
            {
                Int64 pos = s.Position;
                s.Position = 0;
                BinaryReader br = new BinaryReader(s);
                try
                {
                    //if (br.PeekChar() == -1) return null; // end of stream
                    br.ReadByte();
                    s.Position--;
                }
                catch 
                {
                    return null;
                }

                myNodeTreeRegistryItem item = new myNodeTreeRegistryItem();
                item.position = s.Position;
                item.Id = br.ReadInt64();
                item.parentId = br.ReadInt64();
                item.DirectorySectionId = br.ReadInt64();
                item.parentOffset = br.ReadInt64();
                item.firstChildOffset = br.ReadInt64();
                item.lastChildOffset = br.ReadInt64();
                item.nextSiblingOffset = br.ReadInt64();
                item.previousSiblingOffset = br.ReadInt64();
                item.childrenCount = br.ReadInt32();

                // now update
                if (useChildrenCount) item.childrenCount = childrenCount;
                if (incrementChildren) item.childrenCount += 1;
                if (decrementChildren) item.childrenCount -= 1;

                s.Position = 0;
                BinaryWriter bw = new BinaryWriter(s);
                bw.Write(convertToBytes(item));

                // done
                s.Position = pos;
                return item;
            }


            #endregion
        }

        // this method dumps all the my nodes tree to registry file
        public static bool dumpToFile(VirtualDiskFramework.VirtualDiskContext ctx, List<myNode> list, String file)
        {
            using (VirtualDiskFramework.VirtualDiskContext.VhdSystem? sys = VirtualDiskFramework.VirtualDiskContext.VhdSystem.CreateSystem(ctx.disk))
            {
                if (sys == null) return false;
                if (sys.fs.FileExists(file)) sys.fs.DeleteFile(file);

                try
                {
                    // phase 1 - dump nodes
                    using (Stream? s = sys.fs.OpenFile(file, FileMode.Create))
                    {
                        // first write root 0 item
                        myNodeTreeRegistryItem default0Item = new myNodeTreeRegistryItem();
                        BinaryWriter bw = new BinaryWriter(s);
                        bw.Write(myNodeTreeRegistryItem.convertToBytes(default0Item));

                        // now write all the nodes from index 1
                        foreach (myNode node in list)
                        {
                            byte[]? bytes = myNodeTreeRegistryItem.convertToBytesFromMyNode(node);
                            if (bytes == null) continue;
                            bw.Write(bytes);
                        }
                        s.Flush();
                    }

                    // phase 2 reconfigure all registry
                    using (Stream? s = sys.fs.OpenFile(file, FileMode.Open))
                    {
                        // loop each registry item and reconfigure
                        while (true)
                        {
                            // phase 1 - find children count
                            List<myNodeTreeRegistryItem>? children = null;
                            Int64 pos = s.Position;
                            myNodeTreeRegistryItem? item = myNodeTreeRegistryItem.convertFromBytesStream(s);
                            if (item == null) break; // end of stream so break
                            if (item.Id == 0 && item.parentId == 0 && item.DirectorySectionId == 0) continue; // empty slot ignore
                            s.Position = 0;
                            GetNodes(s, item.Id, 0, ref children);
                            item.childrenCount = children.Count;

                            // phase 2 - find parent offset
                            s.Position = 0;
                            myNodeTreeRegistryItem? parentItem = null;
                            Int64 parentOffset = FindNode(s, item.parentId, ref parentItem);
                            item.parentOffset = parentOffset;

                            // phase 3 - reconfigure and store
                            s.Position = pos;
                            BinaryWriter bw = new BinaryWriter(s);
                            bw.Write(myNodeTreeRegistryItem.convertToBytes(item));
                        }
                        s.Flush();
                    }

                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        public static bool toFile(VirtualDiskFramework.VirtualDiskContext ctx, ref myNodeTreeRegistry registry, String file)
        {
            using (VirtualDiskFramework.VirtualDiskContext.VhdSystem? sys = VirtualDiskFramework.VirtualDiskContext.VhdSystem.CreateSystem(ctx.disk))
            {
                if (sys == null) return false;
                if (sys.fs.FileExists(file)) sys.fs.DeleteFile(file);

                using (Stream? s = sys.fs.OpenFile(file, FileMode.Create))
                {
                    return toFile(ref registry, s);
                }
            }
        }

        public static bool toFile(ref myNodeTreeRegistry registry, Stream s)
        {
            try
            {
                myNodeTreeRegistryItem item = new myNodeTreeRegistryItem();
                using (BinaryWriter bw = new BinaryWriter(s))
                {
                    bw.Write(myNodeTreeRegistryItem.convertToBytes(item));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region "framework"

        // this method finds a first empty slot
        public static Int64 FindFirstEmptySlot(Stream s)
        {
            while (true)
            {
                // check if stream position is not in root node registry block, if so move it to the 2nd node
                if (s.Position == 0) s.Position = myNodeTreeRegistryItem.blockSize;

                Int64 pos = s.Position;
                if (pos == s.Length) return -1;
                myNodeTreeRegistryItem? item = myNodeTreeRegistryItem.convertFromBytesStream(s);
                if (item == null) break; // end of stream so break
                if (item.Id == 0 && item.parentId == 0 && item.DirectorySectionId == 0)
                {
                    // this is empty slot so it's not any valid node
                    return pos;
                }
            }
            // no empty space found, so abort with error
            return -1;
        }
        // this method finds the node offset
        public static Int64 FindNode(Stream s, Int64 id, ref myNodeTreeRegistryItem? itemOut)
        {
            while (true)
            {
                Int64 pos = s.Position;
                myNodeTreeRegistryItem? item = myNodeTreeRegistryItem.convertFromBytesStream(s);
                if (item == null) break; // end of stream so break
                //if (item.Id == 0 && item.parentId == 0 && item.DirectorySectionId == 0) continue; // this is empty slot, so skip
                if (item.Id == id)
                {
                    // item found return it
                    itemOut = item;
                    return pos;
                }
            }
            // no empty space found, so abort with error
            return -1;
        }
        // this method inserts a node in the registry
        public static Int64 InsertNode(Stream s, myNodeTreeRegistryItem item, bool useEmptySlot)
        {
            // configure stream position
            Int64 pos = -1;
            if (useEmptySlot)
                pos = FindFirstEmptySlot(s);
            else
                pos = s.Length;

            if (pos < 0) pos = s.Length;

            // convert the node item to bytes and write it
            try
            {
                s.Position = pos;
                BinaryWriter bw = new BinaryWriter(s);
                byte[] bytes = myNodeTreeRegistryItem.convertToBytes(item);
                bw.Write(bytes);
                item.position = pos;

                return pos;
            }
            catch
            {
                return -1;
            }
        }
        // this method inserts a node in the registry
        public static Int64 InsertNode(VirtualDiskFramework.VirtualDiskContext ctx, String file, myNodeTreeRegistryItem item, bool useEmptySlot)
        {
            using (VirtualDiskFramework.VirtualDiskContext.VhdSystem? sys = VirtualDiskFramework.VirtualDiskContext.VhdSystem.CreateSystem(ctx.disk))
            {
                if (sys == null) return -1;
                if (!sys.fs.FileExists(file)) return -1;

                try
                {
                    using (Stream? s = sys.fs.OpenFile(file, FileMode.Open))
                    {
                        return InsertNode(s, item, useEmptySlot);
                    }
                }
                catch
                {
                    return -1;
                }
            }
        }
        // this method deletes a node in the registry
        public static bool DeleteNode(VirtualDiskFramework.VirtualDiskContext ctx, String file, myNodeTreeRegistryItem item)
        {
            using (VirtualDiskFramework.VirtualDiskContext.VhdSystem? sys = VirtualDiskFramework.VirtualDiskContext.VhdSystem.CreateSystem(ctx.disk))
            {
                if (sys == null) return false;
                if (!sys.fs.FileExists(file)) return false;

                try
                {
                    using (Stream? s = sys.fs.OpenFile(file, FileMode.Open))
                    {
                        return DeleteNode(s, item);
                    }
                }
                catch
                {
                    return false;
                }
            }
        }
        // this method deletes a node in the registry
        public static bool DeleteNode(Stream s, myNodeTreeRegistryItem? item)
        {
            if (item == null) return false;

            // convert the node item to bytes and write it
            try
            {
                s.Position = item.position;
                myNodeTreeRegistryItem default0Item = new myNodeTreeRegistryItem();
                BinaryWriter bw = new BinaryWriter(s);
                bw.Write(myNodeTreeRegistryItem.convertToBytes(default0Item));

                return true;
            }
            catch
            {
                return false;
            }
        }

        // this method deletes a node in the registry
        public static bool DeleteNode(Stream s, Int64 id)
        {
            myNodeTreeRegistryItem? item = null;
            Int64 pos = FindNode(s, id, ref item);
            if (pos < 0) return false; // item not found

            // convert the node item to bytes and write it
            try
            {
                s.Position = pos;
                myNodeTreeRegistryItem default0Item = new myNodeTreeRegistryItem();
                BinaryWriter bw = new BinaryWriter(s);
                bw.Write(myNodeTreeRegistryItem.convertToBytes(default0Item));

                return true;
            }
            catch
            {
                return false;
            }
        }

        // this method recursively deletes a node tree in registry file
        public static bool DeleteTreeRecursive(VirtualDiskFramework.VirtualDiskContext ctx, String file, Int64 id)
        {
            using (VirtualDiskFramework.VirtualDiskContext.VhdSystem? sys = VirtualDiskFramework.VirtualDiskContext.VhdSystem.CreateSystem(ctx.disk))
            {
                if (sys == null) return false;
                if (!sys.fs.FileExists(file)) return false;

                try
                {
                    using (Stream? s = sys.fs.OpenFile(file, FileMode.Open))
                    {
                        return DeleteTreeRecursive(s, id);
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool DeleteTreeRecursive(Stream s, Int64 id)
        {

            //LifoBuffer<DiscDirectoryInfo> lifoStack = new LifoBuffer<DiscDirectoryInfo>();

            // lifo stack: last pushed entry is later poped first.
            CyclicStack<myNodeTreeRegistryItem> lifoStack = new CyclicStack<myNodeTreeRegistryItem>(1);// 0000);

            // find the top parent node
            s.Position = 0;
            myNodeTreeRegistryItem? parentItem = null;
            Int64 pos = FindNode(s, id, ref parentItem);
            if (pos < 0) return false; // item not found
            lifoStack.Push(parentItem);

            while (parentItem != null)
            {
                if (lifoStack.Count == 0) break;

                s.Position = 0;
                myNodeTreeRegistryItem? currentItem = lifoStack.Pop();
                List<myNodeTreeRegistryItem>? children = null;
                GetNodes(s, currentItem.Id, 1, ref children);
                myNodeTreeRegistryItem? firstChild = children.FirstOrDefault();
                if (firstChild != null)
                {
                    // there are still children so process them add them to stack
                    lifoStack.Push(firstChild);
                }
                else
                {
                    // no more children, so delete this node
                    DeleteNode(s, currentItem);

                    // now check with parent item if it has more children
                    s.Position = 0;
                    GetNodes(s, parentItem.Id, 1, ref children);
                    if (children.Count > 0)
                        lifoStack.Push(parentItem); // top parent has more children so reprocess the top parent item again
                }
            }

            // finalize
            s.Position = 0;
            pos = FindNode(s, id, ref parentItem);
            {
                if (parentItem != null)
                    DeleteNode(s, parentItem);
            }

            // completed
            return true;

        }
        // this method updates a node in the registry
        public static bool UpdateNode(VirtualDiskFramework.VirtualDiskContext ctx, String file,
            myNodeTreeRegistryItem? item, Int64 parentId, bool useParentId, Int64 DirectorySectionId, bool useDirectorySectionId,
            Int64 parentOffset, bool useParentOffset, Int32 childrenCount, bool useChildrenCount)
        {
            using (VirtualDiskFramework.VirtualDiskContext.VhdSystem? sys = VirtualDiskFramework.VirtualDiskContext.VhdSystem.CreateSystem(ctx.disk))
            {
                if (sys == null) return false;
                if (!sys.fs.FileExists(file)) return false;

                try
                {
                    using (Stream? s = sys.fs.OpenFile(file, FileMode.Open))
                    {
                        return UpdateNode(s, item, parentId, useParentId, DirectorySectionId, useDirectorySectionId, parentOffset, useParentOffset,
                            childrenCount, useChildrenCount);
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        // this method updates a node in the registry
        public static bool UpdateNode(Stream s, myNodeTreeRegistryItem? item, Int64 parentId, bool useParentId, Int64 DirectorySectionId, bool useDirectorySectionId,
            Int64 parentOffset, bool useParentOffset, Int32 childrenCount, bool useChildrenCount)
        {
            if (item == null) return false;

            if (useParentId)
                item.parentId = parentId;

            if (useDirectorySectionId)
                item.DirectorySectionId = DirectorySectionId;

            if (useParentOffset)
                item.parentOffset = parentOffset;

            if (useChildrenCount)
                item.childrenCount = childrenCount;

            // convert the node item to bytes and write it
            try
            {
                s.Position = item.position;
                BinaryWriter bw = new BinaryWriter(s);
                byte[] bytes = myNodeTreeRegistryItem.convertToBytes(item);
                bw.Write(bytes);
                return true;
            }
            catch
            {
                return false;
            }
        }
        // this method finds the node by offset
        public static Int64 GetNodeByOffset(Stream s, Int64 offset, ref myNodeTreeRegistryItem? itemOut)
        {
            Int64 pos = offset;
            if (offset >= s.Length) return -1;

            s.Position = offset;
            myNodeTreeRegistryItem? item = myNodeTreeRegistryItem.convertFromBytesStream(s);
            if (item == null) return -1; // end of stream so break
            itemOut = item;
            return pos;
        }
        // this method finds the node by offset
        public static Int64 GetNodeByOffset(VirtualDiskFramework.VirtualDiskContext ctx, String file,
            Int64 offset, ref myNodeTreeRegistryItem? itemOut)
        {
            using (VirtualDiskFramework.VirtualDiskContext.VhdSystem? sys = VirtualDiskFramework.VirtualDiskContext.VhdSystem.CreateSystem(ctx.disk))
            {
                if (sys == null) return -1;
                if (!sys.fs.FileExists(file)) return -1;

                try
                {
                    using (Stream? s = sys.fs.OpenFile(file, FileMode.Open))
                    {
                        return GetNodeByOffset(s, offset, ref itemOut);
                    }
                }
                catch
                {
                    return -1;
                }
            }
        }

        // gets all direct children by parent id
        public static bool GetNodes(VirtualDiskFramework.VirtualDiskContext ctx,
            String registryFile, Int64 id, Int64 count, ref List<myNodeTreeRegistryItem> listOut, bool loadNodeConfigs)
        {
            using (VirtualDiskFramework.VirtualDiskContext.VhdSystem? sys = VirtualDiskFramework.VirtualDiskContext.VhdSystem.CreateSystem(ctx.disk))
            {
                if (sys == null) return false;
                if (!sys.fs.FileExists(registryFile)) return false;

                List<myNodeTreeRegistryItem> list = new List<myNodeTreeRegistryItem>();
                try
                {
                    using (Stream? s = sys.fs.OpenFile(registryFile, FileMode.Open))
                    {
                        Int64 ctr = 0;
                        while (true)
                        {
                            myNodeTreeRegistryItem? item = myNodeTreeRegistryItem.convertFromBytesStream(s);
                            if (item == null) break; // end of stream so break
                            if (item.Id == 0 && item.parentId == 0 && item.DirectorySectionId == 0) continue; // this is empty slot, so skip
                            if (item.parentId == id)
                            {
                                // direct child item, so add it and reloop with next item
                                list.Add(item);
                                ctr++;
                                if (count >= 1)
                                {
                                    // user only demands some children so abort when counter reached
                                    if (ctr >= count) break;
                                }
                            }
                        }
                    }
                    // done, return the list
                    listOut = list;
                }
                catch
                {
                    // critical error, abort with error
                    return false;
                }
            }

            if (loadNodeConfigs)
            {
                // load all nodes configurations from files
                foreach (myNodeTreeRegistryItem? item in listOut)
                {
                    String rtf = "";
                    item.loadNode(ctx, ref rtf, false, true);
                }

            }
            return true;
        }

        // gets all direct children by parent id
        public static bool GetNodes(Stream s, Int64 id, Int64 count, ref List<myNodeTreeRegistryItem>? listOut)
        {
            List<myNodeTreeRegistryItem> list = new List<myNodeTreeRegistryItem>();
            Int64 ctr = 0;
            while (true)
            {
                myNodeTreeRegistryItem? item = myNodeTreeRegistryItem.convertFromBytesStream(s);
                if (item == null) break; // end of stream so break
                if (item.Id == 0 && item.parentId == 0 && item.DirectorySectionId == 0) continue; // this is empty slot, so skip
                if (item.parentId == id)
                {
                    // direct child item, so add it and reloop with next item
                    list.Add(item);
                    ctr++;
                    if (count >= 1)
                    {
                        // user only demands some children so abort when counter reached
                        if (ctr >= count) break;
                    }
                }
            }
            // done, return the list
            listOut = list;
            return true;
        }
        // gets all parents right to the root
        public static bool Lineage(VirtualDiskFramework.VirtualDiskContext ctx, String file,
            myNodeTreeRegistryItem item, ref List<myNodeTreeRegistryItem>? listOut, bool topFirst)//, bool addTargetItem)
        {
            using (VirtualDiskFramework.VirtualDiskContext.VhdSystem? sys = VirtualDiskFramework.VirtualDiskContext.VhdSystem.CreateSystem(ctx.disk))
            {
                if (sys == null) return false;
                if (!sys.fs.FileExists(file)) return false;

                try
                {
                    using (Stream? s = sys.fs.OpenFile(file, FileMode.Open))
                    {
                        return Lineage(s, item, ref listOut, topFirst);//, addTargetItem);
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        // gets all parents right to the root
        public static bool Lineage(Stream s, myNodeTreeRegistryItem item, ref List<myNodeTreeRegistryItem>? listOut, bool topFirst)
        {
            List<myNodeTreeRegistryItem> list = new List<myNodeTreeRegistryItem>();

            // first add the target item
            list.Add(item);

            listOut = list;

            // this item is root, it does not has any parent so abort
            if (item.Id == 0) return true;

            // this item is not root but a child item, so get all lineage right to the root
            // ascending order list from bottom to top parents
            while (true)
            {
                s.Position = 0;
                Int64 offset = GetNodeByOffset(s, item.parentOffset, ref item);
                if (offset < 0) break;

                // finally add this parent to the list
                list.Add(item);

                if (item.Id == 0) break; // this is root so there is nothing more so we abort
            }

            // make descending order list: parent then child highest is first lowest is last, if required
            if (topFirst) list.Reverse();

            return true;
        }
        // gets full path string
        public static String LineageFullPath(List<myNodeTreeRegistryItem> list)//, myNodeTreeRegistryItem? targetItem)//, bool addTargetItem)
        {
            //List<myNodeTreeRegistryItem> worklist = list.ToList();
            //worklist.Reverse();

            //if (targetItem.Id == 0)
            //    return "0"; // this item is root node so there is not any ancestors to the root node so return path

            String path = @"\";
            foreach (myNodeTreeRegistryItem item in list)//worklist)
                path = Path.Combine(path, item.Id.ToString());

            // add the target item if required, which is the lowest child of all the ancestors
            //if (addTargetItem)
            //    path = Path.Combine(path, targetItem.Id.ToString());

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
        // loads all registry node items from path
        public static bool LineageFromFullPath(VirtualDiskFramework.VirtualDiskContext ctx, String file, String path, ref List<myNodeTreeRegistryItem>? listOut)
        {
            using (VirtualDiskFramework.VirtualDiskContext.VhdSystem? sys = VirtualDiskFramework.VirtualDiskContext.VhdSystem.CreateSystem(ctx.disk))
            {
                if (sys == null) return false;
                if (!sys.fs.FileExists(file)) return false;

                try
                {
                    using (Stream? s = sys.fs.OpenFile(file, FileMode.Open))
                    {
                        return LineageFromFullPath(s, path, ref listOut);
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        // converts full path into ancestor node items
        public static bool LineageFromFullPath(Stream s, String path, ref List<myNodeTreeRegistryItem>? listOut)
        {
            if (path == "") return false;
            List<Int64> list = new List<Int64>();
            String[] ancestors = path.Split(@"\");
            if (ancestors.Count() == 0) return false; // now a valid path

            // initialize root register
            //myNodeTreeRegistry.myNodeTreeRegistryItem? root = null;
            //if (myNodeTreeRegistry.GetNodeByOffset(s, 0, ref root) < 0)
            //    return false; // critical error

            foreach (String ancestor in ancestors)
            {
                if (ancestor == "") continue;

                Int64 value = 0;
                if (!Int64.TryParse(ancestor, out value)) return false; // invalid path or garbage

                // initialize root register
                myNodeTreeRegistry.myNodeTreeRegistryItem? root = null;
                if (myNodeTreeRegistry.GetNodeByOffset(s, 0, ref root) < 0)
                    return false; // critical error

                root.children = new ChildrenRegister();
                if (!ChildrenRegister.Initialize(cfg.ctx1.VhdCtx, cfg.ctx1.dbNodeTreeRegistryFile, root.children, root))
                    return false; // critical error

                // phase 1: get root nodes collection
                List<myNodeTreeRegistry.myNodeTreeRegistryItem> registry = null;
                if (!root.children.GetChildren(ref registry))
                    return false; // critical error abort with error



                list.Add(value);
            }
            listOut = list;
            return true;
        }

        // loads all registry node items from path
        public static bool LoadFullPath(VirtualDiskFramework.VirtualDiskContext ctx, String file, String path, ref List<myNodeTreeRegistryItem>? listOut, bool topFirst)
        {
            using (VirtualDiskFramework.VirtualDiskContext.VhdSystem? sys = VirtualDiskFramework.VirtualDiskContext.VhdSystem.CreateSystem(ctx.disk))
            {
                if (sys == null) return false;
                if (!sys.fs.FileExists(file)) return false;

                try
                {
                    using (Stream? s = sys.fs.OpenFile(file, FileMode.Open))
                    {
                        return LoadFullPath(s, path, ref listOut, topFirst);
                    }
                }
                catch
                {
                    return false;
                }
            }
        }
        // loads all registry node items from path
        public static bool LoadFullPath(VirtualDiskFramework.VirtualDiskContext ctx, String file, myNodeTreeRegistryItem item, ref List<myNodeTreeRegistryItem>? listOut, bool topFirst)
        {
            using (VirtualDiskFramework.VirtualDiskContext.VhdSystem? sys = VirtualDiskFramework.VirtualDiskContext.VhdSystem.CreateSystem(ctx.disk))
            {
                if (sys == null) return false;
                if (!sys.fs.FileExists(file)) return false;

                try
                {
                    using (Stream? s = sys.fs.OpenFile(file, FileMode.Open))
                    {
                        return LoadFullPath(s, item, ref listOut, topFirst);
                    }
                }
                catch
                {
                    return false;
                }
            }
        }
        // loads all registry node items from path
        public static bool LoadFullPath(Stream s, String path, ref List<myNodeTreeRegistryItem>? listOut, bool topFirst)
        {
            List<myNodeTreeRegistryItem> list = new List<myNodeTreeRegistryItem>();

            // phase 1 - first load root path and add it as topmost ancestor


            // check if only root is required then return root node only
            /*
            if (path == @"\")
            {
                myNodeTreeRegistryItem? root = null;
                s.Position = 0;
                Int64 rootOffset = FindNode(s, 0, ref root);
                if (root == null) return false; // critical error root not found abort with error
                list.Add(root);
                listOut = list;
                return true;
            }
            */

            // phase 1 - get all lineage ids

            // get all ancestors ids
            List<Int64> lineageItems = null;
            if (!LineageFromFullPath(path, ref lineageItems)) return false;
            if (lineageItems.Count == 0) return false;

            // phase 2 - get the lowest item by id from the register

            // now get the lowest child's offset and item
            Int64 lowestChildId = lineageItems.LastOrDefault();
            myNodeTreeRegistryItem? item = null;
            s.Position = 0;
            Int64 offset = FindNode(s, lowestChildId, ref item);
            if (item == null) return false; // error node not found or something wrong abort with error

            // phase 3 - get lineage

            // now directly get lineage
            if (!Lineage(s, item, ref list, topFirst)) return false; // if error return error

            // success
            listOut = list;
            return true;
        }

        // loads full path items from a target item id
        public static bool LoadFullPath(Stream s, myNodeTreeRegistryItem item, ref List<myNodeTreeRegistryItem>? listOut, bool topFirst)
        {
            List<myNodeTreeRegistryItem>? list = null;

            // now directly get lineage
            if (!Lineage(s, item, ref list, topFirst)) return false; // if error return error

            // success
            listOut = list;
            return true;
        }
        // returns total entries in existence in the journal database
        public static Int64 Count(VirtualDiskFramework.VirtualDiskContext ctx, String file)
        {
            using (VirtualDiskFramework.VirtualDiskContext.VhdSystem? sys = VirtualDiskFramework.VirtualDiskContext.VhdSystem.CreateSystem(ctx.disk))
            {
                if (sys == null) return -1;
                if (!sys.fs.FileExists(file)) return -1;

                try
                {
                    using (Stream? s = sys.fs.OpenFile(file, FileMode.Open))
                    {
                        myNodeTreeRegistryItem? root = myNodeTreeRegistryItem.Root(s);
                        if (root == null) return -1;
                        return root.childrenCount;
                    }
                }
                catch
                {
                    return -1;
                }
            }
        }

        #endregion 
    }

    
    public class ChildrenRegister
    {
        public myNodeTreeRegistry.myNodeTreeRegistryItem? parent;
        public myNodeTreeRegistry.myNodeTreeRegistryItem? current;
        private long __count = 0;

        // vhd handle and configuration which is to be used whenever operating this double linked list engine
        public VirtualDiskFramework.VirtualDiskContext? ctx = null;
        public String registerFile = "";


        public ChildrenRegister()
        {
        }
        public long Count
        {
            get
            {
                return __count;
            }
        }

        public myNodeTreeRegistry.myNodeTreeRegistryItem? this[int index]
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

        private myNodeTreeRegistry.myNodeTreeRegistryItem? GetAt(int index)
        {
            if (index < 0) return null;
            if (index > __count) return null;

            // reset
            myNodeTreeRegistry.myNodeTreeRegistryItem? original = current;
            current = null;

            myNodeTreeRegistry.myNodeTreeRegistryItem? item = null;
            for (int i = 0; i < index; i++)
            {
                // calculate position in stream and directly get the node by offset/position
                item = Next();
                if (item == null) break; // end of stream abort from loop

                // if found item return it
                if (i == index) break;
            }
            current = original;
            return item;
        }

        public bool Delete(myNodeTreeRegistry.myNodeTreeRegistryItem? item)
        {
            /* first reload parent current config
             * if item's offset is head then this is first node
             * if item's offset is not head but tail then this is 2nd or 3rd or 2+ indexed child node
             * if item's offset is tail then this is last node
             * if item is both head and tail then this is first node
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
            if (!reloadConfig()) return false; // error abort
            if (parent.firstChildOffset == 0) return false; // error there is no child in this parent, abort with error

            // verify itegrity if item is surely present is same and is owned by this parent
            Int64 id = item.Id;
            if (myNodeTreeRegistry.GetNodeByOffset(ctx, registerFile, item.position, ref item) < 0) return false;
            if (item.parentId != parent.Id) return false; // this item was not registered with parent or parent changed so abort
            if (item.Id != id) return false; // this item is not the item which was given as it has changed to some other item so abort

            // phase 2 - get both head and tail items
            myNodeTreeRegistry.myNodeTreeRegistryItem? head = null;
            myNodeTreeRegistry.myNodeTreeRegistryItem? tail = null;
            if (parent.firstChildOffset != 0) myNodeTreeRegistry.GetNodeByOffset(ctx, registerFile, parent.firstChildOffset, ref head);
            if (parent.lastChildOffset != 0) myNodeTreeRegistry.GetNodeByOffset(ctx, registerFile, parent.lastChildOffset, ref tail);

            // phase 3 - get this node's previous and next nodes in chain
            myNodeTreeRegistry.myNodeTreeRegistryItem? prev = null;
            myNodeTreeRegistry.myNodeTreeRegistryItem? next = null;
            if (item.previousSiblingOffset != 0) myNodeTreeRegistry.GetNodeByOffset(ctx, registerFile, item.previousSiblingOffset, ref prev);
            if (item.nextSiblingOffset != 0) myNodeTreeRegistry.GetNodeByOffset(ctx, registerFile, item.nextSiblingOffset, ref next);

            // phase 4 - decide and do
            if (item.position == parent.firstChildOffset && item.position == parent.lastChildOffset)
            {
                // this item is head node and one and only first node there is no other node

                // delete node
                if (!myNodeTreeRegistry.DeleteNode(ctx, registerFile, item)) return false; // if error abort with error

                // finally update parent node
                parent.firstChildOffset = 0;
                parent.lastChildOffset = 0;
                parent.childrenCount--;
                myNodeTreeRegistry.UpdateNode(ctx, registerFile, parent, 0, false, 0, false, 0, false, 0, false);
                // no more child node left so both head and tail are 0

            }
            else if (item.position == parent.firstChildOffset)
            {
                // this item is head node or the first node but not the tail node, so there are 2+ nodes

                // delete node
                if (!myNodeTreeRegistry.DeleteNode(ctx, registerFile, item)) return false; // if error abort with error

                // configure this node's next linked node and update it
                next.previousSiblingOffset = 0; // because there is no previous node
                myNodeTreeRegistry.UpdateNode(ctx, registerFile, next, 0, false, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.firstChildOffset = next.position; // next node becomes the first node or the head
                parent.childrenCount--;
                myNodeTreeRegistry.UpdateNode(ctx, registerFile, parent, 0, false, 0, false, 0, false, 0, false);

            }
            else if (item.position == parent.lastChildOffset)
            {
                // this item is tail or the last node but there is a different head, head and tail 2 or more nodes

                // delete node
                if (!myNodeTreeRegistry.DeleteNode(ctx, registerFile, item)) return false; // if error abort with error

                // we update the previous node in chain remove this node's link and set the previous node as tail
                prev.nextSiblingOffset = 0; // because previous node becomes tail or the last node and there is no next node
                myNodeTreeRegistry.UpdateNode(ctx, registerFile, prev, 0, false, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.lastChildOffset = prev.position; // prev node becomes the last node or the tail. it is also already head if it is first node
                parent.childrenCount--;
                myNodeTreeRegistry.UpdateNode(ctx, registerFile, parent, 0, false, 0, false, 0, false, 0, false);
            }
            else
            {
                // this item is neither head nor tail node means this node is a middle node which exists between the node chain

                // delete node
                if (!myNodeTreeRegistry.DeleteNode(ctx, registerFile, item)) return false; // if error abort with error

                // now join both prev and next nodes into each other and remove the deleted node links
                prev.nextSiblingOffset = next.position; // previous's next is deleted node's next, we join both
                next.previousSiblingOffset = prev.position; // next's previous is deleted node's previous, we join both
                // deleted node's links removed now update both nodes
                myNodeTreeRegistry.UpdateNode(ctx, registerFile, next, 0, false, 0, false, 0, false, 0, false);
                myNodeTreeRegistry.UpdateNode(ctx, registerFile, prev, 0, false, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.childrenCount--;
                myNodeTreeRegistry.UpdateNode(ctx, registerFile, parent, 0, false, 0, false, 0, false, 0, false);
            }
            return true;
        }

        public bool Add(myNodeTreeRegistry.myNodeTreeRegistryItem? item)
        {
            // if 1st child, insert at head and tail, update
            // if 2nd forth child, insert at tail, update tail
            // update parent's configuration - first if 1st node, or last child if last last node
            // update previous node's next node offset to this next node.

            // phase 1 - get parent's current configuration and item from register
            if (!reloadConfig()) return false; // error abort

            if (parent.childrenCount == 0)
            {
                // this is first child being inserted, so insert it and it's offset as head and tail both
                item.parentOffset = parent.position;
                item.parentId = parent.Id;
                item.previousSiblingOffset = 0;
                item.nextSiblingOffset = 0;
                Int64 newItemOffset = myNodeTreeRegistry.InsertNode(ctx, registerFile, item, true);
                if (newItemOffset < 0) return false; // critical error abort with error
                
                // finally update parent node
                parent.firstChildOffset = newItemOffset;
                parent.lastChildOffset = newItemOffset;
                parent.childrenCount++;
                myNodeTreeRegistry.UpdateNode(ctx, registerFile, parent, 0, false, 0, false, 0, false, 0, false);
                // done
            }
            else
            {
                // this is 2nd or any other 2+ index child so insert it as tail and update the previous tail cum last item with this new item
                // get tail
                myNodeTreeRegistry.myNodeTreeRegistryItem? tail = null;
                Int64 tailOffset = myNodeTreeRegistry.GetNodeByOffset(ctx, registerFile, parent.lastChildOffset, ref tail);
                if (tailOffset < 0) return false; // critical error abort with error

                // update and insert this node with new configuration
                item.previousSiblingOffset = tailOffset;
                item.nextSiblingOffset = 0;
                item.parentOffset = parent.position;
                item.parentId = parent.Id;
                Int64 newItemOffset = myNodeTreeRegistry.InsertNode(ctx, registerFile, item, true);
                if (newItemOffset < 0) return false; // critcial error abort with error

                // update previous tail node 
                tail.nextSiblingOffset = newItemOffset;
                myNodeTreeRegistry.UpdateNode(ctx, registerFile, tail, 0, false, 0, false, 0, false, 0, false);

                // finally update parent node
                parent.lastChildOffset = newItemOffset;
                parent.childrenCount++;
                myNodeTreeRegistry.UpdateNode(ctx, registerFile, parent, 0, false, 0, false, 0, false, 0, false);
                // done
            }

            // finally - update root and its configuration if this parent is not root
            /*
            if (parent.Id != 0)
            {
                // not root node but ordinary node, so we need to update root as well
                myNodeTreeRegistry.myNodeTreeRegistryItem.updateRoot(ctx, registerFile, 0, false, true, false);
            }
            */

            // finally reconfigure the register
            __count = parent.childrenCount;

            // success
            return true;
        }
        public void Reset()
        {
            current = null;
        }

        public myNodeTreeRegistry.myNodeTreeRegistryItem? First()
        {
            myNodeTreeRegistry.GetNodeByOffset(ctx, registerFile, parent.firstChildOffset, ref current);
            return current;
        }
        public myNodeTreeRegistry.myNodeTreeRegistryItem? Last()
        {
            myNodeTreeRegistry.GetNodeByOffset(ctx, registerFile, parent.lastChildOffset, ref current);
            return current;
        }

        // we get all children
        public bool GetChildren(ref List<myNodeTreeRegistry.myNodeTreeRegistryItem>? listOut)
        {
            Int64 nextSiblingOffset = -1;

            myNodeTreeRegistry.myNodeTreeRegistryItem? current = null;

            List<myNodeTreeRegistry.myNodeTreeRegistryItem>? list = new List<myNodeTreeRegistry.myNodeTreeRegistryItem>();
            listOut = list;
            if (parent.firstChildOffset == 0) return true; // there are no children so abort and return empty list

            // get the head
            if (myNodeTreeRegistry.GetNodeByOffset(ctx, registerFile, parent.firstChildOffset, ref current) < 0)
                return false; // end of chain or error or no children present in parent, abort

            if (current == null)
                return false; // there is no child node present so abort

            nextSiblingOffset = current.position;

            while (true) 
            {
                // load item from registry
                myNodeTreeRegistry.myNodeTreeRegistryItem? nextItem = null;
                Int64 nextOffset = myNodeTreeRegistry.GetNodeByOffset(ctx, registerFile, nextSiblingOffset, ref nextItem);
                if (nextOffset < 0) break; // error or end of stream or end of register so break
                if (nextItem == null) break; // no more items so break
                
                // add item to list
                list.Add(nextItem);

                // configure
                nextSiblingOffset = nextItem.nextSiblingOffset;

                // check if end of children register reached
                if (nextItem.nextSiblingOffset == 0) break;
            }
            // output
            listOut = list;
            return true;
        }

        // we get next demanded cache of sequence of items from register from the current item passed as parameter
        public myNodeTreeRegistry.myNodeTreeRegistryItem? NextCache(int total, myNodeTreeRegistry.myNodeTreeRegistryItem? current,
            ref List<myNodeTreeRegistry.myNodeTreeRegistryItem>? listOut)
        {
            Int64 nextSiblingOffset = -1;
            if (total < 1) total = 1000; // auto set total 1000 if user does not passes param total

            List<myNodeTreeRegistry.myNodeTreeRegistryItem>? list = new List<myNodeTreeRegistry.myNodeTreeRegistryItem>();
            listOut = list;
            if (parent.firstChildOffset == 0) return null; // there are no children so abort and return empty list

            if (current == null)
            {
                // current is not loaded, so get first head node as current
                if (myNodeTreeRegistry.GetNodeByOffset(ctx, registerFile, parent.firstChildOffset, ref current) < 0)
                    return null; // end of chain or error or no children present in parent, abort

                nextSiblingOffset = current.position;
            }
            else
            {
                // current is already loaded so proceed with it's next sibling
                if (current.nextSiblingOffset == 0) return null; // end of chain, abort

                // get next sibling offset
                nextSiblingOffset = current.nextSiblingOffset;
            }

            if (current == null)
                return null; // there is no child node present so abort

            // we take next 1000 or cache number nodes if available 1000 or cache or more, otherwise whatever we can get less then 1000 and return the list
            // with the last node as returned tail
            myNodeTreeRegistry.myNodeTreeRegistryItem? tail = null; // last item which was found

            for (int i = 0; i < total; i++) 
            {
                // load item from registry
                myNodeTreeRegistry.myNodeTreeRegistryItem? nextItem = null;
                Int64 nextOffset = myNodeTreeRegistry.GetNodeByOffset(ctx, registerFile, nextSiblingOffset, ref nextItem);
                if (nextOffset < 0) break; // error or end of stream or end of register so break
                if (nextItem == null) break; // no more items so break

                // set tail this next item because it was found
                tail = nextItem;

                // configure
                nextSiblingOffset = nextItem.nextSiblingOffset;

                // add item to list
                list.Add(nextItem);
            }
            // output
            listOut = list;
            return tail;
        }

        public myNodeTreeRegistry.myNodeTreeRegistryItem? Next()
        {
            Int64 nextSiblingOffset = -1;
            if (current == null)
            {
                // current is not loaded, so get first head node as current
                if (myNodeTreeRegistry.GetNodeByOffset(ctx, registerFile, parent.firstChildOffset, ref current) < 0)
                    return null; // end of chain or error or no children present in parent, abort

                nextSiblingOffset = current.position;
            }
            else
            {
                // current is already loaded so proceed with it's next sibling
                if (current.nextSiblingOffset == 0) return null; // end of chain, abort

                // get next sibling offset
                nextSiblingOffset = current.nextSiblingOffset;
            }

            if (current == null)
                return null; // there is no child node present so abort

            // load item from registry
            myNodeTreeRegistry.myNodeTreeRegistryItem? nextItem = null;
            Int64 nextOffset = myNodeTreeRegistry.GetNodeByOffset(ctx, registerFile, nextSiblingOffset, ref nextItem);
            if (nextOffset < 0) return null; // critical error abort with null

            // reconfigure current and set next item into it and return it as the next item in register
            current = nextItem;
            return current;
        }
        
        public myNodeTreeRegistry.myNodeTreeRegistryItem? Previous()
        {
            Int64 prevSiblingOffset = -1;
            if (current == null)
            {
                // current is not loaded, so get tail node as current
                if (myNodeTreeRegistry.GetNodeByOffset(ctx, registerFile, parent.lastChildOffset, ref current) < 0)
                    return null; // end of chain or error or no children present in parent, abort

                prevSiblingOffset = current.position;
            }
            else
            {
                // current is already loaded so proceed with it's previous sibling
                if (current.previousSiblingOffset == 0) return null; // end of chain, abort

                // get prev offset
                prevSiblingOffset = current.previousSiblingOffset;
            }

            if (current == null)
                return null; // there is no child node present so abort

            // load item from registry
            myNodeTreeRegistry.myNodeTreeRegistryItem? prevItem = null;
            Int64 prevOffset = myNodeTreeRegistry.GetNodeByOffset(ctx, registerFile, prevSiblingOffset, ref prevItem);
            if (prevOffset < 0) return null; // critical error abort with null

            // reconfigure current and set prev item into it and return it as the prev item in register
            current = prevItem;
            return current;
        }

        public bool reloadConfig()
        {
            Int64 parentOffset = myNodeTreeRegistry.GetNodeByOffset(ctx, registerFile, parent.position, ref parent);
            if (parentOffset < 0) return false; // critical error abort with error

            // reconfigure
            __count = parent.childrenCount;

            return true;
        }

        public static bool Initialize(VirtualDiskFramework.VirtualDiskContext ctx, String registerFile,
            ChildrenRegister list, myNodeTreeRegistry.myNodeTreeRegistryItem parentItem)
        {
            // phase 1 - configure and setup

            list.ctx = ctx;
            list.registerFile = registerFile;
            list.current = null;

            // directly setup count from parent item
            list.__count = parentItem.childrenCount;
            list.parent = parentItem;
            return true;
        }

    }
    
}


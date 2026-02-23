using RtfPipe.Model;
using RtfPipe.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Linq;
using TheBook.Net.Core;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace DiaryJournal.Net
{
    public static class entryMethods
    {
        public const String xmlExt = "xml";
        public const String rtfExt = "rtf";
        public const String htmlExt = "html";
        public const String txtExt = "txt";
        public const String cfgExt = "cfg";
        public const String pdfExt = "pdf";
        public const String xamlExt = "xaml";
        public const String xmlExtComplete = ".xml";
        public const String rtfExtComplete = ".rtf";
        public const String htmlExtComplete = ".html";
        public const String txtExtComplete = ".txt";
        public const String cfgExtComplete = ".cfg";
        public const String pdfExtComplete = ".pdf";
        public const String xamlExtComplete = ".xaml";
        public const String xmlExtSearchPattern = "*.xml";
        public const String rtfExtSearchPattern = "*.rtf";
        public const String htmlExtSearchPattern = "*.html";
        public const String txtExtSearchPattern = "*.txt";
        public const String cfgExtSearchPattern = "*.cfg";
        public const String pdfExtSearchPattern = "*.pdf";
        public const String xamlExtSearchPattern = "*.xaml";

        public static bool getEntryTypeFormatsByFileName(String file, ref EntryType entryTypeOut, 
            ref String extOut, ref String extCompleteOut, ref String extSearchPatternOut)
        {
            if (file.Length <= 0)
                return false;

            FileInfo fileInfo = new FileInfo(file);
            String extension = fileInfo.Extension;
            if (extension.Length <= 0)
                return false;

            switch (extension)
            {
                case ".xml":
                    entryTypeOut = EntryType.Xml;
                    getEntryTypeFormats(EntryType.Xml, ref extOut, ref extCompleteOut, ref extSearchPatternOut);
                    break;

                case ".rtf":
                    entryTypeOut = EntryType.Rtf;
                    getEntryTypeFormats(EntryType.Rtf, ref extOut, ref extCompleteOut, ref extSearchPatternOut);
                    break;

                case ".txt":
                    entryTypeOut = EntryType.Txt;
                    getEntryTypeFormats(EntryType.Txt, ref extOut, ref extCompleteOut, ref extSearchPatternOut);
                    break;

                case ".html":
                    entryTypeOut = EntryType.Html;
                    getEntryTypeFormats(EntryType.Html, ref extOut, ref extCompleteOut, ref extSearchPatternOut);
                    break;

                case ".pdf":
                    entryTypeOut = EntryType.Pdf;
                    getEntryTypeFormats(EntryType.Pdf, ref extOut, ref extCompleteOut, ref extSearchPatternOut);
                    break;

                case ".xaml":
                    entryTypeOut = EntryType.Xaml;
                    getEntryTypeFormats(EntryType.Xaml, ref extOut, ref extCompleteOut, ref extSearchPatternOut);
                    break;

                case ".cfg":
                    entryTypeOut = EntryType.Cfg;
                    getEntryTypeFormats(EntryType.Cfg, ref extOut, ref extCompleteOut, ref extSearchPatternOut);
                    break;

                default:
                    return false;
            }
            return true;
        }
        public static void getEntryTypeFormats(EntryType entryType, ref String extOut, ref String extCompleteOut, ref String extSearchPatternOut)
        {
            switch (entryType)
            {
                case EntryType.Xml:
                    extOut = xmlExt;
                    extCompleteOut = xmlExtComplete;
                    extSearchPatternOut = xmlExtSearchPattern;
                    break;
                case EntryType.Rtf:
                    extOut = rtfExt;
                    extCompleteOut = rtfExtComplete;
                    extSearchPatternOut = rtfExtSearchPattern;
                    break;
                case EntryType.Html:
                    extOut = htmlExt;
                    extCompleteOut = htmlExtComplete;
                    extSearchPatternOut = htmlExtSearchPattern;
                    break;
                case EntryType.Txt:
                    extOut = txtExt;
                    extCompleteOut = txtExtComplete;
                    extSearchPatternOut = txtExtSearchPattern;
                    break;
                case EntryType.Pdf:
                    extOut = pdfExt;
                    extCompleteOut = pdfExtComplete;
                    extSearchPatternOut = pdfExtSearchPattern;
                    break;
                case EntryType.Xaml:
                    extOut = xamlExt;
                    extCompleteOut = xamlExtComplete;
                    extSearchPatternOut = xamlExtSearchPattern;
                    break;
                case EntryType.Cfg:
                    extOut = cfgExt;
                    extCompleteOut = cfgExtComplete;
                    extSearchPatternOut = cfgExtSearchPattern;
                    break;
                default:
                    extOut = "";
                    extCompleteOut = "";
                    extSearchPatternOut = "";
                    break;
            }
        }

        public static String getFormattedPathFileName(String path, Int64 id, Int64 parentId, String title,
            DateTime dateTime, long exportIndex, EntryType entryType, out String entryNameOut)
        {
            String entryName = getFormattedFileName(id, parentId, title, dateTime, exportIndex, entryType, out entryNameOut);
            String file = Path.Combine(path, Path.GetFileName(entryName));
            //file = @"\\?\" + file;
            return file;
        }
        public static String getFormattedFileName(Int64 id, Int64 parentId, String title,
            DateTime dateTime, long exportIndex, EntryType entryType, out String entryNameOut)
        {
            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            entryMethods.getEntryTypeFormats(entryType, ref ext, ref extComplete, ref extSearchPattern);

            // common chapter entry. we use proper journal format
            String modifiedTitle = ((title != "") ? title.Replace("--", "-") : "");
            modifiedTitle = title.Replace(":", "-");
            String entryName = String.Format("{0}--{1}--{2}--{3}--{4}--.{5}", exportIndex, dateTime.ToString("yyyy-MM-dd-HH-mm-ss-fff"),
                id, parentId, modifiedTitle, ext);
            entryNameOut = String.Format("{0}--{1}--{2}--{3}--{4}--", exportIndex, dateTime.ToString("yyyy-MM-dd-HH-mm-ss-fff"),
                id, parentId, modifiedTitle);
            return entryName;
        }
        public static String removeAllInvalidPathCharacters(String value)
        {
            string illegal = value;//"//\"M\"\\a/ry/ h**ad:>> a\\/:*?\"| li*tt|le|| la\"mb.?";
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            value = r.Replace(illegal, "");
            return value;
        }
        public static String getEntryLabel(myNode node, bool removeInvalidPathChars, bool insertRevDateTime = false)
        {
            String entryName = "";
            switch (node.chapter.nodeType)
            {
                /*
                case NodeType.Journal:
                    entryName = String.Format(@"{0}:(ID {1})", mySystemNodes.JournalSystemNodeName, node.chapter.Id.ToString());
                    break;

                case NodeType.Library:
                    entryName = String.Format(@"{0}:(ID {1})", mySystemNodes.LibrarySystemNodeName, node.chapter.Id.ToString());
                    break;
                */
                case NodeType.Template:
                    entryName = String.Format(@"(T):({0}):({1}:{2}:{3}:{4}):({5}):(ID {6})", node.chapter.chapterDateTime.ToString("dd-MM-yyyy"),
                        node.chapter.chapterDateTime.Hour, node.chapter.chapterDateTime.Minute,
                        node.chapter.chapterDateTime.Second, node.chapter.chapterDateTime.Millisecond, node.chapter.Title, node.chapter.Id.ToString());
                    break;

                case NodeType.Label:
                    entryName = String.Format(@"{0}:(ID {1})", node.chapter.Title, node.chapter.Id.ToString());
                    break;

                case NodeType.NonCalendarEntry:
                    entryName = String.Format(@"{0}:(ID {1})", node.chapter.Title, node.chapter.Id.ToString());
                    break;

                case NodeType.Set:
                    entryName = String.Format(@"CloneSet:({0}):({1}):({2}:{3}:{4}:{5}):(ID {6})", node.chapter.Title,
                        node.chapter.chapterDateTime.ToString("dd-MM-yyyy"),
                        node.chapter.chapterDateTime.Hour, node.chapter.chapterDateTime.Minute,
                        node.chapter.chapterDateTime.Second,
                        node.chapter.chapterDateTime.Millisecond,
                        node.chapter.Id.ToString());
                    break;

                case NodeType.Year:
                    entryName = String.Format("{0}:(ID {1})", node.chapter.chapterDateTime.ToString("yyyy"), node.chapter.Id.ToString());
                    break;

                case NodeType.Month:
                    entryName = String.Format("{0}:(ID {1})", node.chapter.chapterDateTime.ToString("MMMM"), node.chapter.Id.ToString());
                    break;

                case NodeType.Entry:
                    entryName = String.Format(@"({0}):({1}:{2}:{3}:{4}):({5}):(ID {6})", node.chapter.chapterDateTime.ToString("dd-MM-yyyy"),
                        node.chapter.chapterDateTime.Hour, node.chapter.chapterDateTime.Minute,
                        node.chapter.chapterDateTime.Second, node.chapter.chapterDateTime.Millisecond, node.chapter.Title, node.chapter.Id.ToString());
                    break;

                default:
                    if (node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                        entryName = String.Format(@"{0}:(ID {1})", mySystemNodes.getSystemNodeName(node.chapter.nodeType), node.chapter.Id.ToString());
                    else
                        entryName = String.Format(@"{0}:(ID {1})", node.chapter.Title, node.chapter.Id.ToString());

                    break;

            }
            if (removeInvalidPathChars)
                entryName = removeAllInvalidPathCharacters(entryName);

            if (insertRevDateTime)
                entryName = CoreFramework.getFormattedEntryFileName(entryName, DateTime.Now);

            return entryName;
        }

        // find all system nodes
        public static List<myNode> findSystemNodes(ref List<myNode> srcNodes, bool coreSystemNodes = true, bool calendarNodes = false, bool sort = true, bool descending = false)
        {
            List<myNode> nodes = new List<myNode>();
            foreach (myNode srcNode in srcNodes)
            {
                myNode node = srcNode;

                if (node.chapter.specialNodeType != SpecialNodeType.SystemNode)
                    continue; // not system node so skip

                if (!calendarNodes)
                {
                    if (node.chapter.nodeType == NodeType.Year || node.chapter.nodeType == NodeType.Month)
                        continue; // calendar system node not wanted so skip it
                }

                if (!coreSystemNodes)
                {
                    // core system nodes not wanted so skip
                    if (mySystemNodes.isCoreSystemNode(node.chapter.nodeType))
                        continue;
                }
                // finally add this system node
                nodes.Add(node);
            }

            // sort by date and time
            if (sort)
                entryMethods.sortNodesByDateTime(ref nodes, descending);

            return nodes;
        }
        // finds a system node by it's name from the source list
        public static myNode? findSystemNodeByName(List<myNode> nodes, String systemNodeName)
        {
            foreach (myNode node in nodes)
            {
                if (node.chapter.specialNodeType != SpecialNodeType.SystemNode)
                    continue; // not system node so skip

                //NodeType nodeType = CoreFramework.convertToEnum<NodeType>(node.chapter.nodeType);
                String name = CoreFramework.convertToString(node.chapter.nodeType);

                if (systemNodeName != name)
                    continue; // not the wanted node name so skip

                // this is the system node which is being wanted, so return it
                return node;
            }
            return null;
        }


        // find all system nodes
        public static List<myNode> findNodesByTypes(ref List<myNode> srcNodes, SpecialNodeType specialNodeType,
            NodeType nodeType, bool sort = true, bool descending = false)
        {
            List<myNode> nodes = new List<myNode>();
            foreach (myNode srcNode in srcNodes)
            {
                myNode node = srcNode;

                // a valid format entry file found, process
                if (node.chapter.specialNodeType != SpecialNodeType.AnyOrAll)
                {
                    // find only by special type
                    if (node.chapter.specialNodeType != specialNodeType)
                        continue;

                    // found matching node, process
                    if (nodeType != NodeType.AnyOrAll)
                    {
                        // 2nd condition required by user
                        if (node.chapter.nodeType == nodeType)
                            nodes.Add(node);
                    }
                    else
                    {
                        // no node type requirement by user, so add all matching nodes of node type
                        nodes.Add(node);
                    }
                }
                else
                {
                    // find all special types

                    if (nodeType != NodeType.AnyOrAll)
                    {
                        // find only by node type
                        // 2nd condition required by user
                        if (node.chapter.nodeType == nodeType)
                            nodes.Add(node);
                    }
                    else
                    {
                        // find all node types
                        // no node type requirement by user, so add all matching nodes of node type
                        nodes.Add(node);
                    }

                }
            }

            // sort by date and time
            if (sort)
                entryMethods.sortNodesByDateTime(ref nodes, descending);

            return nodes;
        }
        // this method finds and loads all system nodes into proper places in the system nodes collection
        public static void loadOtherSystemNodesCollection(ref List<myNode> allNodes, ref mySystemNodes systemNodes)
        {
            // reset
            systemNodes.YearNodes.Clear();
            systemNodes.MonthNodes.Clear();

            List<myNode> otherNodes = findSystemNodes(ref allNodes, false, true, true, false);

            // now find items and fill in appropirate places
            foreach (myNode node in otherNodes)
            {
                if (node.chapter.nodeType == NodeType.Year)
                    systemNodes.YearNodes.Add(node);
                else if (node.chapter.nodeType == NodeType.Month)
                    systemNodes.MonthNodes.Add(node);
            }
        }
        // find all deleted marked nodes in db
        public static List<myNode> DBFindDeletedNodes(List<myNode> allNodes)
        {
            List<myNode> nodes = new List<myNode>();
            foreach (myNode node in allNodes)
            {
                if (node.chapter.IsDeleted)
                    nodes.Add(node);
            }
            return nodes;
        }
        // this promotes the node to one level up in tree structure.
        public static bool DBPromoteNodeOFSDB(OpenFSDBContext ctx, myNode? node)
        {
            if (node == null)
                return false; // error node not found

            if (node.chapter.Id == 0) // cannot move root
                return false;

            // validate
            if (node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return false;

            if (node.chapter.parentId == 0) // this item is already located in root so we cannot proceed further
                return false;

            // get current register item latest state
            RegisterItem? currentItem = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, node.chapter.Id, true, false, false, false, false, false);
            if (currentItem == null) return false;
            RegisterItem? parent = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, currentItem.parentId, true, false, false, false, true, true);
            if (parent == null) return false;
            RegisterItem? dst = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, parent.parentId, true, false, false, false, true, true);
            if (dst == null) return false;

            // move from parent to another location
            return parent.children.Move(currentItem, dst);
        }
        // this explores the node's entry file
        public static void DBExploreEntryFileOFSDB(OpenFSDBContext? ctx, Int64 sectionId, Int64 id)
        {
            OpenFileSystemDB.exploreNodeEntryFile(ctx, sectionId, id);
        }

        // this promotes the node to one level up in tree structure.
        public static bool DBChangeNodeParentOFSDB(OpenFSDBContext ctx, Int64 sectionId, Int64 id, Int64 newParentId)
        {
            // change parent of node
            return OpenFileSystemDB.changeNodeParent(ctx, sectionId, id, newParentId);
        }

        // this method sets or unsets a parent for a target node by guid
        public static bool DBSetNodeParent(OpenFSDBContext? ctx, myNode? node, Int64 parentId)
        {
            if (node == null)
                return false; // error node not found

            // validate
            if (node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return false;

            // set parent id: 0 for no parent, 1+ for a valid parent
            node.chapter.parentId = parentId;

            // now update the node
            return DBUpdateNodeOFSDB(ctx, node, "", null, false, false, false);
        }

        // load the entry data through entry's node
        public static Object? DBLoadNodeData(OpenFSDBContext? ctx, Int64 id, Int64 sectionId = 0)
        {
            return OpenFileSystemDB.loadNodeData(ctx, sectionId, id);
        }

        // this method creates a new node with rtf into it
        public static bool DBCreateNodeOFSDB(OpenFSDBContext ctx, myNode node, String? rtf, byte[]? xamlbytes,
            bool resetCD, bool resetMD, bool resetDD, bool root)
        {
            return OpenFileSystemDB.createNode(ctx, node, rtf, xamlbytes, resetCD, resetMD, resetDD, root);
        }

        // this method auto selects the db core writes it's config file
        public static String DBWriteConfig(OpenFSDBContext? ctx)
        {
            String xml = "";
            xml = OpenFileSystemDB.writeDBConfig(ctx);
            return xml;
        }
        // auto select db and retrieve entire tree structure sequential list most recursively
        public static List<myNode> DBFindAllNodesTreeSequence(ref List<myNode> allNodes,
            bool sort = true, bool descending = false)
        {
            return findAllNodesTreeSequence(ref allNodes, sort, descending);
        }

        // auto select db and find a node by id
        public static myNode? DBFindLoadNode(OpenFSDBContext? ctx, Int64 id, ref String? rtf, ref byte[] xaml, bool loadData = false, Int64 sectionId = 0)
        {
            return OpenFileSystemDB.findLoadNode(ctx, sectionId, id, ref rtf, ref xaml, loadData);
        }
        // find a node by id
        public static myNode? DBFindLoadNodeOFSDB(OpenFSDBContext ctx, Int64 id, ref String rtf, bool loadData = false, Int64 sectionId = 0)
        {
            byte[]? xamlbytesOut = null;

            if (sectionId <= 0)
                return DBSearchNodeOFSDB(ctx, id, ref rtf, ref xamlbytesOut, loadData);
            else
                return OpenFileSystemDB.findLoadNode(ctx, sectionId, id, ref rtf, ref xamlbytesOut, loadData);
        }
        // search a node by id
        public static myNode? DBSearchNodeOFSDB(OpenFSDBContext ctx, Int64 id, ref String? rtf, ref byte[]? xamlbytesOut, bool loadData = false)
        {

            foreach (OpenFSDBSection section in ctx.dbSections.sections)
            {
                myNode? found = OpenFileSystemDB.findLoadNode(ctx, section.sectionId, id, ref rtf, ref xamlbytesOut, loadData);
                if (found != null)
                {
                    // found node by id
                    return found;
                }
            }
            // node not found with this id
            return null;
        }
        // auto select db and find a node by id
        public static myNode? FindNodeInList(List<myNode> allNodes, Int64 id)
        {
            foreach (myNode node in allNodes)
            {
                if (node.chapter.Id == id)
                    return node;
            }
            return null;
        }
        // find node by title
        public static myNode? FindNodeInListByTitle(ref List<myNode> allNodes, String title)
        {
            foreach (myNode node in allNodes)
            {
                if (node.chapter.Title == title)
                    return node;
            }
            return null;
        }
        // auto select db and find a node by title
        public static myNode? FindNodeByTitleInList(ref List<myNode> allNodes, String title)
        {
            foreach (myNode node in allNodes)
            {
                if (node.chapter.Title == title)
                    return node;
            }
            return null;
        }
        // auto select db and find a node by title
        public static myNode? BuildPathNodes(ref Int64 startingNodeIndex, String path, String delimiter, ref List<myNode> listOut)
        {
            List<myNode> list = new List<myNode>();

            if (path.Length == 0)
                return null;

            if (delimiter.Length == 0)
                return null;

            String[] values = path.Split(delimiter);
            if (values.Length == 0)
                return null;

            myNode? parentNode = null;
            foreach (String value in values)
            {
                myNode node = new myNode(true);
                node.chapter.Id = startingNodeIndex++;
                if (parentNode != null)
                    node.chapter.parentId = parentNode.chapter.Id;

                node.chapter.Title = value;
                node.chapter.chapterDateTime = DateTime.Now;
                listOut.Add(node);
                parentNode = node;
            }
            return null;
        }

        // this method finds all lineage chain of a parent node most recursively
        public static List<myNode> FindAllChildrenRecursiveInList(ref List<myNode> allNodes,
            ref myNode node, bool sort = true, bool descending = false, bool addParentNode = false)
        {
            List<myNode> list = new List<myNode>();

            if (node == null)
                return list; // error node not found

            Queue<myNode> queue = new Queue<myNode>();
            queue.Enqueue(node);

            // add parent node at index 0 if demanded
            if (addParentNode)
                list.Add(node);

            while (queue.Count > 0)
            {
                myNode currentNode = queue.Dequeue();

                List<myNode> children = findFirstLevelChildren(currentNode.chapter.Id, ref allNodes, sort, descending);

                foreach (myNode childNode in children)
                    queue.Enqueue(childNode);

                if (currentNode.chapter.Id != node.chapter.Id)
                    list.Add(currentNode);
            }
            return list;
        }
        // this method finds all lineage chain of all parent nodes most recursively
        public static List<myNode> FindSelectedNodesAllChildrenRecursiveInList(ref List<myNode> allNodes,
            ref List<myNode> selNodes, bool sort = true, bool descending = false, bool addParentNode = false)
        {
            List<myNode> list = new List<myNode>();

            if (selNodes.Count == 0)
                return list; // error node not found

            foreach (myNode selNode in selNodes)
            {
                Queue<myNode> queue = new Queue<myNode>();
                queue.Enqueue(selNode);

                // add parent node at index 0 if demanded
                if (addParentNode)
                    list.Add(selNode);

                while (queue.Count > 0)
                {
                    myNode currentNode = queue.Dequeue();

                    List<myNode> children = findFirstLevelChildren(currentNode.chapter.Id, ref allNodes, sort, descending);

                    foreach (myNode childNode in children)
                        queue.Enqueue(childNode);

                    if (currentNode.chapter.Id != selNode.chapter.Id)
                        list.Add(currentNode);
                }
            }
            return list;
        }

        public static bool GenerateLineagePath(List<myNode> allNodes, ref myNode srcNode, out String outFormatted, out List<myNode> outLineage)
        {
            List<myNode> lineage = new List<myNode>();
            if ((srcNode == null) || (allNodes.Count() <= 0))
            {
                outFormatted = "";
                outLineage = lineage;
                return false;
            }

            // get lineage
            lineage = findBottomToRootNodesRecursive(allNodes, ref srcNode, true, true, true);

            // get lineage formatted full path
            outFormatted = "";
            foreach (myNode? node in lineage)
                outFormatted += @"\\" + getEntryLabel(node, false);

            // set and return
            outLineage = lineage;
            return true;
        }
        // this method first finds & loads the current node by it's guid, then recursively finds & loads all it's parents and ancestors
        // right to the root ancestor which has no parent of it's own.
        public static List<myNode> findBottomToRootNodesRecursive(List<myNode> allNodes, ref myNode srcNode, bool includeDeleted = false,
            bool topToBottom = false, bool includeNonDeleted = false)
        {
            List<myNode> nodes = new List<myNode>();
            if (srcNode == null)
                return nodes; // error node not found

            // add this node at index 0 of the list
            nodes.Add(srcNode);

            myNode? node = srcNode;
            while (true)
            {
                if (node == null)
                    break;

                if (includeDeleted && !includeNonDeleted)
                {
                    if (!node.chapter.IsDeleted)
                        continue;
                }
                else if (!includeDeleted && includeNonDeleted)
                {
                    if (node.chapter.IsDeleted)
                        continue;
                }
                else if (includeDeleted && includeNonDeleted)
                {
                    // take both deleted and nondelete
                }
                else
                {
                    // parameters error, correct the parameters
                    break;
                }

                if (node.chapter.parentId == 0)
                    break;

                // find and load all parent nodes recursively from bottom to root
                node = FindNodeInList(allNodes, node.chapter.parentId);
                if (node == null)
                    break; // no more parents found, this is end of loop

                // a parent found. add this parent node in the list
                nodes.Add(node);
            }

            // from top ancestor to bottom most child if demanded
            if (topToBottom)
                nodes.Reverse();

            return nodes;
        }
        // this method automatically checks if a node is an ancestor of source node
        public static int IsAncestorNode(List<myNode> allNodes, Int64 nodeToCheckId, myNode srcNode, bool deleted = false)
        {
            if (srcNode == null) return -1;

            myNode? node = srcNode;
            List<myNode> ancestors = findBottomToRootNodesRecursive(allNodes, ref srcNode, deleted, true, true);
            myNode? found = FindNodeInList(ancestors, nodeToCheckId);
            if (found == null)
                return 0;
            else
                return 1;
        }

        // this method purges old config and updates the node accordingly. 
        public static bool DBUpdateNodeOFSDB(OpenFSDBContext? ctx, myNode node, String? rtf = "", byte[]? xamlbytes = null, 
            bool storeData = false, bool updateModificationDate = true, bool backupOldEntryFirst = false, EntryType entryType = EntryType.Default) 
        {
            // finally update the node's files
            return OpenFileSystemDB.updateNode(ctx, node, rtf, xamlbytes, storeData, updateModificationDate, backupOldEntryFirst, entryType);
        }

        // erases and purges the node's files
        public static bool DBPurgeNodeOFSDB(OpenFSDBContext ctx, Int64 id, Int64 sectionId, bool purgeConfig = true, bool purgeData = true)
        {
            return OpenFileSystemDB.purgeNode(ctx, id, sectionId, purgeConfig, purgeData);
        }

        // everything is kept in sets. set node is the root node. all import and export of sets exist in set node which is their root node.
        // set node is root node and has no parent.
        public static myNode createSetNode(ref Int64 currentIndex, String setName, DateTime setDateTime)
        {
            myNode node = new myNode(true);
            node.chapter.chapterDateTime = setDateTime;
            node.chapter.creationDateTime = setDateTime;
            node.chapter.modificationDateTime = setDateTime;
            // todo node.chapter.Id = CreateNodeID(ref currentIndex);
            node.chapter.nodeType = NodeType.Set;
            node.chapter.parentId = 0;
            node.chapter.Title = setName;
            return node;
        }
        // everything is kept in sets. set node is the root node. all import and export of sets exist in set node which is their root node.
        // set node is root node and has no parent.
        public static myNode createSetNode(OpenFSDBContext? ctx, String setName, DateTime setDateTime)
        {
            myNode node = new myNode(true);
            node.chapter.chapterDateTime = setDateTime;
            node.chapter.creationDateTime = setDateTime;
            node.chapter.modificationDateTime = setDateTime;
            // todo node.chapter.Id = CreateNodeID(ctx);
            node.chapter.nodeType = NodeType.Set;
            node.chapter.parentId = 0;
            node.chapter.Title = setName;
            return node;
        }

        // apply the set node to root nodes. this applies the set node to the entire 100% tree.
        public static void applySetNode(Int64 setNodeID, ref List<myNode> tree)
        {
            if (setNodeID == 0)
                return;

            List<myNode> rootNodes = entryMethods.findRootNodes(ref tree, SpecialNodeType.AnyOrAll, true, false);
            foreach (myNode rootNode in rootNodes)
                rootNode.chapter.parentId = setNodeID;
        }

        // find all root nodes
        public static List<myNode> findRootNodes(ref List<myNode> srcNodes, SpecialNodeType specialNodeType,
            bool sort = true, bool descending = false, bool notAbsoluteRoot = true)
        {
            List<myNode> nodes = new List<myNode>();
            foreach (myNode srcNode in srcNodes)
            {
                myNode node = srcNode;

                if (specialNodeType == SpecialNodeType.AnyOrAll)
                {
                    // process all nodes
                }
                else if (specialNodeType == SpecialNodeType.NonSystemNode)
                {
                    // user demands non system node type
                    if (node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                        continue; 
                }
                else if (specialNodeType == SpecialNodeType.SystemNode)
                {
                    // user demands system node type
                    if (node.chapter.specialNodeType != SpecialNodeType.SystemNode)
                        continue;
                }
                else if (specialNodeType == SpecialNodeType.None)
                {
                    // user demands non system node type
                    if (node.chapter.specialNodeType != SpecialNodeType.None)
                        continue;
                }

                if (notAbsoluteRoot)
                {
                    if (node.chapter.Id == 0)
                        continue; // absolute true root node not wanted, so skip it
                }

                // a valid format entry file found, process
                if (node.chapter.parentId == 0)
                {
                    // found matching node, process

                    // add this found node to the list
                    nodes.Add(node);
                }
            }

            // sort by date and time
            if (sort)
                entryMethods.sortNodes(ref nodes, false, true, true, false, false, descending);
                //entryMethods.sortNodesByIdThenDateTime(ref nodes, descending);

            return nodes;
        }
        // dom method: this method sets the new id to a node. if demanded resets the children's parent node id to new parent id
        public static void setNodeID(ref List<myNode> allNodes, myNode node, Int64 newID, bool setChildrenParentID = true)
        {
            node.previousID = node.chapter.Id;
            node.chapter.Id = newID;
            if (setChildrenParentID)
            {
                List<myNode> children = findFirstLevelChildren(node.previousID, ref allNodes, false, false);
                foreach (myNode child in children)
                    child.chapter.parentId = newID;
            }
        }

        // sets node common date and time
        public static RegisterItem? DBSetNodeCommonDateTimeOFSDB(OpenFSDBContext ctx, Int64 id, DateTime newDateTime)
        {
            RegisterItem? item = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, id, true, false, false, false, false, false);
            if (item == null) return null; // not found or critical error

            // skip if this is system node, we cannot change it
            if (item.node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return null; // not found or critical error

            // configure node
            item.node.chapter.chapterDateTime = newDateTime;

            // update
            if (OpenFileSystemDB.updateNode(ctx, item.node, "", null, false, false))
                return item;
            else
                return null;
        }

        // this method finds all first level or direct children of the target parent node, non-recursive.
        public static List<myNode> findFirstLevelChildren(Int64 parentId, ref List<myNode> srcNodes,
            bool sort = true, bool descending = false)
        {
            List<myNode> nodes = new List<myNode>();
            //if (parentId == 0)
            //    return nodes;

            foreach (myNode srcNode in srcNodes)
            {
                myNode node = srcNode;

                // a valid format entry file found, process
                if (node.chapter.parentId == parentId)
                {
                    // found matching node, process

                    // add this found node to the list
                    nodes.Add(node);
                }
            }

            // sort if required
            if (sort)
                entryMethods.sortNodesByDateTime(ref nodes, descending);

            return nodes;
        }

        // export a node and all it's tree as documents like rtf, pdf, html, etc.
        public static bool DBExportNodeTree(OpenFSDBContext? ctx, Int64 id, String path, EntryType OutputEntryType, FormOperation? formop = null)
        {
            // first export ancestor
            RegisterItem? ancestor = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, id, true, false, false, false, true, true);
            if (ancestor == null) return false;
            entryMethods.exportEntry(ctx, ref ancestor.node, path, false, id, OutputEntryType);

            while (true)
            {
                // load item from registry
                RegisterItem? nextDescendant = ancestor.tree.Next();
                if (nextDescendant == null) break;
                nextDescendant = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, nextDescendant.Id, true, false, false, false, true, true);
                if (nextDescendant == null) return false;

                // export this descendant node as document
                entryMethods.exportEntry(ctx, ref nextDescendant.node, path, false, id, OutputEntryType);
            }
            return true;
        }


        // this method exports a node to common exported human readable format document.
        // this method does not export importable set or file. non-importable document.
        public static bool exportEntry(OpenFSDBContext? ctx, ref myNode? node, String path, bool useCustomPathFileName,
            Int64 exportIndex, EntryType OutputEntryType)
        {
            if (node == null)
                return false;

            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            entryMethods.getEntryTypeFormats(OutputEntryType, ref ext, ref extComplete, ref extSearchPattern);

            // 1st load chapter's data blob
            byte[]? xaml = null;
            String? rtf = "";
            entryMethods.DBFindLoadNode(ctx, node.chapter.Id, ref rtf, ref xaml, true, node.DirectorySectionID);

            if (ctx.dbEntryType == EntryType.Rtf)
                xamlEntry.dummy.Rtf = rtf;
            else
                xamlEntry.dummy.XamlBytes = xaml;

            String fileData = "";
            byte[]? fileDataBytes = null;
            switch (OutputEntryType)
            {
                case EntryType.Xml:
                    fileData = xmlEntry.toXml(ref node.chapter, xamlEntry.dummy.Rtf, true);
                    break;

                case EntryType.Rtf:
                    fileData = rtfEntry.toRtf(xamlEntry.dummy.Rtf);
                    break;

                case EntryType.Html:
                    fileData = htmlEntry.toHtml(xamlEntry.dummy.Rtf);
                    break;

                case EntryType.Txt:
                    fileData = txtEntry.toTxt(xamlEntry.dummy.Rtf);
                    break;

                case EntryType.Pdf:
                    fileDataBytes = pdfEntry.toPDF(xamlEntry.dummy.Rtf);
                    break;

                case EntryType.Xaml:
                    fileDataBytes = xamlEntry.toXaml(xamlEntry.dummy.Rtf);
                    break;

                default:
                    break;
            }

            // make sure path is available
            if (path.Length <= 0)
                return false;

            String oldPath = path;
            if (!useCustomPathFileName)
            {
                // no custom file name given, so auto generate filename based on database node/entry attributes.
                String entryName = "";
                path = entryMethods.getFormattedPathFileName(path, node.chapter.Id, node.chapter.parentId, node.chapter.Title,
                    node.chapter.chapterDateTime, exportIndex, OutputEntryType, out entryName);
            }
            else
            {
                // custom path and filename have been given, so use them.
            }

            // make sure to mark the file path as long file name
            if (path.IndexOf(@"\\?\") < 0)
                path = @"\\?\" + path;

            if ((OutputEntryType != EntryType.Pdf) && (OutputEntryType != EntryType.Xaml))
            {
                // export non pdf text file
                try
                {
                    // try to write rtf text
                    File.WriteAllText(path, fileData);
                }
                catch (Exception)
                {
                    // there are some unicode characters in data, so write with unicode writer.
                    using (StreamWriter writer = new StreamWriter(path, false, Encoding.Unicode))
                    {
                        writer.Write(fileData);
                        writer.Flush();
                        writer.Close();
                        writer.Dispose();
                    }
                }
            }
            else
            {
                // export pdf file
                try
                {
                    File.WriteAllBytes(path, fileDataBytes);
                }
                catch (Exception){ }
            }
            return true;
        }


        public static bool validateExtractEntryFile(String file, ref long index, ref DateTime chapterDate,
            ref String title, ref Int64 id, ref Int64 parentId)
        {
            if (file == "")
                return false;

            FileInfo fileInfo = new FileInfo(file);
            String filename = fileInfo.Name;

            String validationPattern = @"[0-9]*--\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d-\d\d\d--";
            Regex regex0 = new Regex(validationPattern, RegexOptions.IgnoreCase);
            MatchCollection matches0 = regex0.Matches(filename);

            String entryDateTimePattern = @"\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d-\d\d\d";
            Regex regex1 = new Regex(entryDateTimePattern, RegexOptions.IgnoreCase);
            MatchCollection matches1 = regex1.Matches(filename);

            String indexPattern = @"([0-9]*)--(\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d-\d\d\d)--";
            Regex regex2 = new Regex(indexPattern, RegexOptions.IgnoreCase);
            MatchCollection matches2 = regex2.Matches(filename);

            //String completePattern = @"([0-9]*)(--)(\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d-\d\d\d)(--)([a-zA-Z].*)(--)([a-zA-Z].*)(--)(.*)(--)(\.[a-zA-Z].*)";
            String completePattern = @"([0-9]*)(--)(\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d-\d\d\d)(--)([0-9]*)(--)([0-9]*)(--)(.*)(--)(\..*)";
            Regex regex3 = new Regex(completePattern, RegexOptions.IgnoreCase);
            MatchCollection matches3 = regex3.Matches(filename);

            if (matches0.Count <= 0)
                return false; // error not this application's generated file.

            if (matches1.Count <= 0)
                return false; // error not valid date and time

            if (matches2.Count <= 0)
                return false; // error not valid date and time

            if (matches3.Count <= 0)
                return false; // error not valid date and time

            if (!long.TryParse(matches2[0].Groups[1].Value, out index))
                return false;

            try
            {
                chapterDate = DateTime.ParseExact(matches3[0].Groups[3].Value, "yyyy-MM-dd-HH-mm-ss-fff",
                                  System.Globalization.CultureInfo.InvariantCulture);
                id = Int64.Parse(matches3[0].Groups[5].Value);
                parentId = Int64.Parse(matches3[0].Groups[7].Value);
                title = matches3[0].Groups[9].Value;
            }
            catch (FormatException)
            {
                return false;
            }


            return true;
        }
        public static bool convertEntryFilenameToChapter(ref Chapter chapter, String file)
        {
            DateTime chapterDate = DateTime.Now;
            String title = "";
            long index = -1;
            Int64 parentId = 0;
            Int64 id = 0;

            if (!validateExtractEntryFile(file, ref index, ref chapterDate, ref title, ref id, ref parentId))
                return false;

            chapter.chapterDateTime = chapterDate;
            chapter.Title = title;
            chapter.Id = id;
            chapter.parentId = parentId;
            return true;
        }

        public static bool convertEntryFilenameToNode(ref myNode node, String file)
        {
            node.chapter = new Chapter();
            return convertEntryFilenameToChapter(ref node.chapter, file);
        }

        public static String findEntryFileByExportIndex(IEnumerable<String> files, long index)
        {
            DateTime chapterDate = DateTime.Now;
            String title = "";
            long foundIndex = -1;
            Int64 parentId = 0;
            Int64 id = 0;

            foreach (String file in files)
            {
                if (!validateExtractEntryFile(file, ref foundIndex, ref chapterDate, ref title, ref id, ref parentId))
                    continue;

                if (index == foundIndex)
                    return file; // found a matching file
            }
            // exception, no file found
            return "";
        }
        // this method configures node's document width
        public static bool DBSetNodeDocumentWidthOFSDB(OpenFSDBContext ctx, myNode? node, int width)
        {
            if (node == null)
                return false; // error node not found

            // configure
            node.chapter.documentWidth = width;

            // now update the node
            return DBUpdateNodeOFSDB(ctx, node, "", null, false, false, false);
        }
        public static bool setEntryHighlightFontCompleteStrings(OpenFSDBContext? ctx, myNode? node,
            String HLFontColor,
            String HLBackColor,
            String HLFont)
        {
            return updateHLFont(ctx, node, HLFont, HLFontColor, HLBackColor);
        }
        public static bool setEntryHighlightFontComplete(OpenFSDBContext? ctx, myNode? node,
            Color highlightFontColor,
            Color highlightBackColor,
            Font highlightFont)
        {
            String HLFont = myCommonMethods1.FontToString(highlightFont);
            String HLFontColor = myCommonMethods1.ColorToString(highlightFontColor);
            String HLBackColor = myCommonMethods1.ColorToString(highlightBackColor);
            return updateHLFont(ctx, node, HLFont, HLFontColor, HLBackColor);
        }
        public static bool setEntryHighlightFont(OpenFSDBContext? ctx, myNode? node, Color highlightFontColor, Font highlightFont)
        {
            String HLFont = myCommonMethods1.FontToString(highlightFont);
            String HLFontColor = myCommonMethods1.ColorToString(highlightFontColor);
            return updateHLFont(ctx, node, HLFont, HLFontColor, null);
        }
        public static bool setEntryClearHighlightFont(OpenFSDBContext? ctx, myNode? node)
        {
            String HLFont = "";
            String HLFontColor = "";
            return updateHLFont(ctx, node, HLFont, HLFontColor, null);
        }

        public static bool setEntryHighlightBackColor(OpenFSDBContext? ctx, myNode? node, Color highlightBackColor)
        {
            String HLBackColor = myCommonMethods1.ColorToString(highlightBackColor);
            return updateHLBackColor(ctx, node, HLBackColor);
        }
        public static bool setEntryClearBackColor(OpenFSDBContext? ctx, myNode? node)
        {
            String HLBackColor = "";
            return updateHLBackColor(ctx, node, HLBackColor);
        }
        public static bool setEntryClearHighlight(OpenFSDBContext? ctx, myNode? node)
        {
            return setEntryHighlightFontCompleteStrings(ctx, node, "", "", "");
        }
        public static bool updateHLBackColor(OpenFSDBContext? ctx, myNode? node, String? HLBackColor = null)
        {
            if (node == null)
                return false; // error node not found

            // set properties/config
            if (HLBackColor != null)
                node.chapter.HLBackColor = HLBackColor;

            // update
            return OpenFileSystemDB.updateNode(ctx, node, "", null, false, false);
        }
        public static bool updateHLFont(OpenFSDBContext? ctx, myNode? node,
            String? HLFont = null, String? HLFontColor = null, String? HLBackColor = null)
        {
            if (node == null)
                return false; // error node not found

            // set properties/config
            if (HLFont != null)
                node.chapter.HLFont = HLFont;

            if (HLFontColor != null)
                node.chapter.HLFontColor = HLFontColor;

            if (HLBackColor != null)
                node.chapter.HLBackColor = HLBackColor;

            // update
            return OpenFileSystemDB.updateNode(ctx, node, "", null, false, false);
        }

        // clone the entry/node
        public static RegisterItem? DBCloneNodeOFSDB(OpenFSDBContext ctx, Int64 id, Int64 locationId, ref RegisterItem? emptySlots)
        {
            if (id == 0) return null; // error root cannot be cloned

            // get latest state register item
            RegisterItem? item = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, id, true, true, false, false, false, false);
            if (item == null) return null;

            // validate
            if (item.node.chapter.specialNodeType == SpecialNodeType.SystemNode) return null; // error system node cannot be cloned
            if (item.node.chapter.domainType == DomainType.EmptySlot) return null; // error empty slot cannot be cloned

            // configure
            Int64 parentId = item.node.chapter.parentId;
            if (locationId != -1) parentId = locationId;

            // reconfigure node and insert to create a clone
            item.node.chapter.Id = -1;
            item.node.chapter.parentId = parentId;
			RegisterItem? cloneItem = Register.Insert(ctx, ctx.dbNodeTreeRegistryFile, parentId, emptySlots, item.node, item.rtf, item.xamlbytes);
            return cloneItem;
        }

        // load new tree node with configuration

        public static TreeNode? InitializeNewTreeNode(ref myNode? node, Font defaultFont)
        {
            if (node == null) return null;
            if (node.chapter == null) return null;
            System.Drawing.Font? nodeFont = null;
            String path = String.Format(@"{0}", node.chapter.Id);
            String entryName = getEntryLabel(node, false);
            TreeNode newTreeNode = new TreeNode(entryName);
            newTreeNode.Name = path;
            loadNodeHighlight(newTreeNode, node, defaultFont, Color.White, Color.Black);
            return newTreeNode;                
        }

        // sets node state cursor position
        public static bool DBUpdateCaretConfig(OpenFSDBContext? ctx, myNode? node, Int32 caretIndex, Int32 careSelLength)
        {
            if (node == null)
                return false; // error node not found

            if (caretIndex < 0) return false;
            if (careSelLength < 0) return false;

            // configure
            node.chapter.caretIndex = caretIndex;
            node.chapter.caretSelectionLength = careSelLength;

            // update
            return OpenFileSystemDB.updateNode(ctx, node, "", null, false, false);
        }

        // this method selects the db core and updates the node's title
        public static bool DBUpdateNodeTitle(OpenFSDBContext? ctx, myNode? node, String title)
        {
            if (node == null)
                return false; // error node not found

            // set properties/config
            node.chapter.Title = title;

            // update
            return DBUpdateNodeOFSDB(ctx, node, "", null, false, false, false);
        }
        // find all nodes by type and date
        public static List<myNode> findNodesByNodeTypeDate(List<myNode> allNodes,
            SpecialNodeType specialNodeType, NodeType nodeType,
            int year, int month, int day)
        {
            List<myNode> nodes = new List<myNode>();

            foreach (myNode listNode in allNodes)
            {
                myNode node = listNode;

                if (specialNodeType == SpecialNodeType.AnyOrAll)
                {
                    // process all nodes
                }
                else if (specialNodeType == SpecialNodeType.NonSystemNode)
                {
                    // user demands non system node type
                    if (node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                        continue;
                }
                else if (specialNodeType == SpecialNodeType.SystemNode)
                {
                    // user demands system node type
                    if (node.chapter.specialNodeType != SpecialNodeType.SystemNode)
                        continue;
                }
                else if (specialNodeType == SpecialNodeType.None)
                {
                    // user demands non system node type
                    if (node.chapter.specialNodeType != SpecialNodeType.None)
                        continue;
                }

                switch (nodeType)
                {
                    case NodeType.Year:
                        if (node.chapter.nodeType == NodeType.Year && node.chapter.chapterDateTime.Year == year)
                            nodes.Add(node);

                        break;
                    case NodeType.Month:
                        if (node.chapter.nodeType == NodeType.Month && node.chapter.chapterDateTime.Year == year && node.chapter.chapterDateTime.Month == month)
                            nodes.Add(node);

                        break;
                    case NodeType.Entry:
                        if (node.chapter.nodeType == NodeType.Entry && node.chapter.chapterDateTime.Year == year && node.chapter.chapterDateTime.Month == month &&
                            node.chapter.chapterDateTime.Day == day)
                            nodes.Add(node);

                        break;
                    case NodeType.AnyOrAll:
                        if (node.chapter.chapterDateTime.Year == year && node.chapter.chapterDateTime.Month == month &&
                            node.chapter.chapterDateTime.Day == day)
                            nodes.Add(node);

                        break;
                    default:
                        break;
                }
            }
            return nodes;
        }
        // this method sets the highlights and font for a given tree node
        public static void loadNodeHighlight(TreeNode treeNode, myNode node, Font defaultFont, Color defaultBackColor, Color defaultForeColor)
        {
            String defaultFontString = myCommonMethods1.FontToString(defaultFont);
            String defaultBCString = myCommonMethods1.ColorToString(defaultBackColor);
            String defaultFCString = myCommonMethods1.ColorToString(defaultForeColor);

            if ((node.chapter.HLFont.Length > 0 ) && (node.chapter.HLFont != defaultFontString))
                treeNode.NodeFont = myCommonMethods1.StringToFont(node.chapter.HLFont);

            if ((node.chapter.HLFontColor.Length > 0) && (node.chapter.HLFontColor != defaultFCString))
                treeNode.ForeColor = myCommonMethods1.StringToColor(node.chapter.HLFontColor);

            if ((node.chapter.HLBackColor.Length > 0) && (node.chapter.HLBackColor != defaultBCString))
                treeNode.BackColor = myCommonMethods1.StringToColor(node.chapter.HLBackColor);
            
        }
        // loads and sets node's icon in tree view node
        public static void loadNodeTreeViewItemIcon(TreeNode treeNode, myNode node)
        {
            if ((treeNode == null) || (node == null))
                return;

            // setup tree node icons
            if (node.chapter.specialNodeType == SpecialNodeType.SystemNode)
            {
                treeNode.ImageIndex = 1;
                treeNode.SelectedImageIndex = 2;
            }

            // setup treeview icons
            if (node.chapter.nodeType == NodeType.Year || node.chapter.nodeType == NodeType.Month)
            {
                treeNode.ImageIndex = 3;
                treeNode.SelectedImageIndex = 3;
            }
            else if (node.chapter.nodeType == NodeType.Set)
            {
                treeNode.ImageIndex = 4;
                treeNode.SelectedImageIndex = 4;
            }
            else if (node.chapter.nodeType == NodeType.Label)
            {
                treeNode.ImageIndex = 5;
                treeNode.SelectedImageIndex = 5;
            }
            else if (node.chapter.nodeType == NodeType.Template)
            {
                treeNode.ImageIndex = 6;
                treeNode.SelectedImageIndex = 6;
            }
        }

        public static void setCalendarHighlightEntry(MonthCalendar CalendarEntries, DateTime dateTime)
        {
            DateTime day = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
            CalendarEntries.AddBoldedDate(day);
            CalendarEntries.UpdateBoldedDates();
        }

        // this method finds all nodes ordered by first parent and then it's children and so and so
        // and builds a treeview tree struture
        public static List<TreeNode> buildTreeViewTree(ref List<myNode> srcNodes, ref List<myNode> outTree, 
            Font defaultFont, bool addTreeNodes = true, bool nullmyNodeTag = true,
            bool sort = true, bool descending = false, MonthCalendar? CalendarEntries = null,
            bool insertDeletedTreeNode = false)
        {
            List<TreeNode> tree = new List<TreeNode>();
            outTree = new List<myNode>();
            Queue<TreeNode> queue = new Queue<TreeNode>();

            // system nodes are first to be indexed at index 0 before all the rest of nodes.
            List<myNode> rootNodes = new List<myNode>();
            rootNodes.AddRange(entryMethods.findRootNodes(ref srcNodes, SpecialNodeType.SystemNode, sort, descending));
            // non system nodes must exist after the system nodes.
            rootNodes.AddRange(entryMethods.findRootNodes(ref srcNodes, SpecialNodeType.NonSystemNode, sort, descending));

            // first enqueue all root nodes
            foreach (myNode rootNode in rootNodes)
            {
                myNode node = rootNode;
                String path = String.Format(@"{0}", node.chapter.Id);
                String entryName = getEntryLabel(node, false);
                TreeNode newTreeNode = new TreeNode(entryName);
                newTreeNode.Name = path;
                newTreeNode.Tag = node;

                if (CalendarEntries != null)
                    setCalendarHighlightEntry(CalendarEntries, node.chapter.chapterDateTime);

                if (!insertDeletedTreeNode)
                {
                    if (!node.chapter.IsDeleted)
                        tree.Add(newTreeNode);
                }
                else
                {
                    tree.Add(newTreeNode);
                }

                // add both common and deleted node into tree
                outTree.Add(node); // add this node in the output tree list.
                queue.Enqueue(newTreeNode);
            }
            // now build a perfect sequentially ordered queue of all parents first, then 2nd their children most recursively.
            // in the year loop, 1st all years are enqueued. and all months of each year are enqueued into the queue.
            // the 2nd is month loop. when all years and their months added, then all children of each month are enqueued.
            // this is most sequential and recursive layer by layer processing.
            while (queue.Count > 0)
            {
                TreeNode currentTreeNode = queue.Dequeue();
                if (currentTreeNode == null) continue;

                myNode currentNode = (myNode)currentTreeNode.Tag;

                // fetch this node's children
                List<myNode> children = entryMethods.findFirstLevelChildren(currentNode.chapter.Id, ref srcNodes, sort, descending);

                // 2nd in sequence is parent's children, so children are added 2nd to parent in sequence.
                foreach (myNode childNode in children)
                {
                    myNode node = childNode;
                    String path = String.Format(@"{0}", node.chapter.Id);
                    String entryName = getEntryLabel(node, false);
                    TreeNode newTreeNode = new TreeNode(entryName);
                    newTreeNode.Name = path;
                    newTreeNode.Tag = node;

                    if (addTreeNodes)
                    {
                        if (!insertDeletedTreeNode)
                        {
                            if (!node.chapter.IsDeleted)
                                currentTreeNode.Nodes.Add(newTreeNode);
                        }
                        else
                        {
                            currentTreeNode.Nodes.Add(newTreeNode);
                        }
                    }

                    outTree.Add(node); // add this node in the output tree list.
                    queue.Enqueue(newTreeNode);
                }


                // setup tree node icons
                loadNodeTreeViewItemIcon(currentTreeNode, currentNode);

                // setup calendar highlight
                if (CalendarEntries != null)
                    setCalendarHighlightEntry(CalendarEntries, currentNode.chapter.chapterDateTime);

                // highlight
                loadNodeHighlight(currentTreeNode, currentNode, defaultFont, Color.Black, Color.White);

                // null the processed tree node's tag so that the resource is released and memory freed.
                if (nullmyNodeTag) currentTreeNode.Tag = null;
            }

            return tree;
        }

        // this method finds all nodes ordered by first parent and then it's children and so and so
        public static List<myNode> findAllNodesTreeSequence(ref List<myNode> srcNodes, 
            bool sort = true, bool descending = false)
        {
            List<myNode> nodes = new List<myNode>();
            Queue<myNode> queue = new Queue<myNode>();

            // system nodes are first to be indexed at index 0 before all the rest of nodes.
            List<myNode> rootNodes = new List<myNode>();
            rootNodes.AddRange(entryMethods.findRootNodes(ref srcNodes, SpecialNodeType.SystemNode, sort, descending));
            // non system nodes must exist after the system nodes.
            rootNodes.AddRange(entryMethods.findRootNodes(ref srcNodes, SpecialNodeType.NonSystemNode, sort, descending));

            // first enqueue all root nodes
            foreach (myNode rootNode in rootNodes)
                queue.Enqueue(rootNode);

            // now build a perfect sequentially ordered queue of all parents first, then 2nd their children most recursively.
            // in the year loop, 1st all years are enqueued. and all months of each year are enqueued into the queue.
            // the 2nd is month loop. when all years and their months added, then all children of each month are enqueued.
            // this is most sequential and recursive layer by layer processing.
            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                if (currentNode == null)
                    continue;

                // fetch this node's children
                List<myNode> children = entryMethods.findFirstLevelChildren(currentNode.chapter.Id, ref srcNodes, sort, descending);

                // 2nd in sequence is parent's children, so children are added 2nd to parent in sequence.
                foreach (myNode childNode in children)
                    queue.Enqueue(childNode);

                // 1st in sequence is the parent node, so parent node is added 1st in sequence before all children.
                nodes.Add(currentNode);
            }
            return nodes;
        }

        public static void DBReloadLineages(List<myNode> allNodes, bool deleted = false,
            bool topToBottom = false, bool includeNonDeleted = false)
        {
            // now build lineages of all nodes
            foreach (myNode? node in allNodes)
            {
                myNode? listedNode = node;
                listedNode.lineage = entryMethods.findBottomToRootNodesRecursive(allNodes, ref listedNode, deleted, topToBottom, includeNonDeleted);
            }

        }

        public static void sortNodesByDateTime(ref List<myNode> nodes, bool descending = false)
        {
            // descending = latest/top/max/last to earliest/bottom/least/first (-)
            // ascending = earliest/bottom/least/first to latest/top/max/last (+)
            
            if (nodes.Count() <= 0)
                return;

            nodes = nodes.Select(d => new
            {
                d.chapter.chapterDateTime.Year,
                d.chapter.chapterDateTime.Month,
                d.chapter.chapterDateTime.Day,
                d.chapter.chapterDateTime.Hour,
                d.chapter.chapterDateTime.Minute,
                d.chapter.chapterDateTime.Second,
                d.chapter.chapterDateTime.Millisecond,
                x = d
            })
            .Distinct()
            .OrderBy(d => d.Year)
            .ThenBy(d => d.Month)
            .ThenBy(d => d.Day)
            .ThenBy(d => d.Hour)
            .ThenBy(d => d.Minute)
            .ThenBy(d => d.Second)
            .ThenBy(d => d.Millisecond)
            .Select(d => d.x).ToList();

            if (descending)
                nodes.Reverse();

        }

        public static void sortNodes(ref List<myNode> nodes, bool sortById, bool sortByIdExtra,
            bool sortByCommonDateTime, bool sortByCreationDateTime, bool sortByModificationDateTime,
            bool descending = false)
        {
            // descending = latest/top/max/last to earliest/bottom/least/first (-)
            // ascending = earliest/bottom/least/first to latest/top/max/last (+)

            if (nodes.Count() <= 0)
                return;

            if (sortById)
            {
                nodes = nodes.Select(d => new
                {
                    d.chapter.Id,
                    x = d
                })
                .Distinct()
                .OrderBy(d => d.Id)
                .Select(d => d.x).ToList();
            }
            else if (sortByIdExtra)
            {
                // sort by id

                if (sortByCommonDateTime)
                {
                    nodes = nodes.Select(d => new
                    {
                        d.chapter.Id,
                        d.chapter.chapterDateTime.Year,
                        d.chapter.chapterDateTime.Month,
                        d.chapter.chapterDateTime.Day,
                        d.chapter.chapterDateTime.Hour,
                        d.chapter.chapterDateTime.Minute,
                        d.chapter.chapterDateTime.Second,
                        d.chapter.chapterDateTime.Millisecond,
                        x = d
                    })
                    .Distinct()
                    .OrderBy(d => d.Id)
                    .ThenBy(d => d.Year)
                    .ThenBy(d => d.Month)
                    .ThenBy(d => d.Day)
                    .ThenBy(d => d.Hour)
                    .ThenBy(d => d.Minute)
                    .ThenBy(d => d.Second)
                    .ThenBy(d => d.Millisecond)
                    .Select(d => d.x).ToList();
                }
                else if (sortByCreationDateTime)
                {
                    nodes = nodes.Select(d => new
                    {
                        d.chapter.Id,
                        d.chapter.creationDateTime.Year,
                        d.chapter.creationDateTime.Month,
                        d.chapter.creationDateTime.Day,
                        d.chapter.creationDateTime.Hour,
                        d.chapter.creationDateTime.Minute,
                        d.chapter.creationDateTime.Second,
                        d.chapter.creationDateTime.Millisecond,
                        x = d
                    })
                    .Distinct()
                    .OrderBy(d => d.Id)
                    .ThenBy(d => d.Year)
                    .ThenBy(d => d.Month)
                    .ThenBy(d => d.Day)
                    .ThenBy(d => d.Hour)
                    .ThenBy(d => d.Minute)
                    .ThenBy(d => d.Second)
                    .ThenBy(d => d.Millisecond)
                    .Select(d => d.x).ToList();
                }
                else if (sortByModificationDateTime)
                {
                    nodes = nodes.Select(d => new
                    {
                        d.chapter.Id,
                        d.chapter.modificationDateTime.Year,
                        d.chapter.modificationDateTime.Month,
                        d.chapter.modificationDateTime.Day,
                        d.chapter.modificationDateTime.Hour,
                        d.chapter.modificationDateTime.Minute,
                        d.chapter.modificationDateTime.Second,
                        d.chapter.modificationDateTime.Millisecond,
                        x = d
                    })
                    .Distinct()
                    .OrderBy(d => d.Id)
                    .ThenBy(d => d.Year)
                    .ThenBy(d => d.Month)
                    .ThenBy(d => d.Day)
                    .ThenBy(d => d.Hour)
                    .ThenBy(d => d.Minute)
                    .ThenBy(d => d.Second)
                    .ThenBy(d => d.Millisecond)
                    .Select(d => d.x).ToList();
                }

            }
            else
            {
                // do not sort by id

                if (sortByCommonDateTime)
                {
                    nodes = nodes.Select(d => new
                    {
                        d.chapter.chapterDateTime.Year,
                        d.chapter.chapterDateTime.Month,
                        d.chapter.chapterDateTime.Day,
                        d.chapter.chapterDateTime.Hour,
                        d.chapter.chapterDateTime.Minute,
                        d.chapter.chapterDateTime.Second,
                        d.chapter.chapterDateTime.Millisecond,
                        x = d
                    })
                    .Distinct()
                    .OrderBy(d => d.Year)
                    .ThenBy(d => d.Month)
                    .ThenBy(d => d.Day)
                    .ThenBy(d => d.Hour)
                    .ThenBy(d => d.Minute)
                    .ThenBy(d => d.Second)
                    .ThenBy(d => d.Millisecond)
                    .Select(d => d.x).ToList();
                }
                else if (sortByCreationDateTime)
                {
                    nodes = nodes.Select(d => new
                    {
                        d.chapter.creationDateTime.Year,
                        d.chapter.creationDateTime.Month,
                        d.chapter.creationDateTime.Day,
                        d.chapter.creationDateTime.Hour,
                        d.chapter.creationDateTime.Minute,
                        d.chapter.creationDateTime.Second,
                        d.chapter.creationDateTime.Millisecond,
                        x = d
                    })
                    .Distinct()
                    .OrderBy(d => d.Year)
                    .ThenBy(d => d.Month)
                    .ThenBy(d => d.Day)
                    .ThenBy(d => d.Hour)
                    .ThenBy(d => d.Minute)
                    .ThenBy(d => d.Second)
                    .ThenBy(d => d.Millisecond)
                    .Select(d => d.x).ToList();
                }
                else if (sortByModificationDateTime)
                {
                    nodes = nodes.Select(d => new
                    {
                        d.chapter.modificationDateTime.Year,
                        d.chapter.modificationDateTime.Month,
                        d.chapter.modificationDateTime.Day,
                        d.chapter.modificationDateTime.Hour,
                        d.chapter.modificationDateTime.Minute,
                        d.chapter.modificationDateTime.Second,
                        d.chapter.modificationDateTime.Millisecond,
                        x = d
                    })
                    .Distinct()
                    .OrderBy(d => d.Year)
                    .ThenBy(d => d.Month)
                    .ThenBy(d => d.Day)
                    .ThenBy(d => d.Hour)
                    .ThenBy(d => d.Minute)
                    .ThenBy(d => d.Second)
                    .ThenBy(d => d.Millisecond)
                    .Select(d => d.x).ToList();
                }

            }

            if (descending)
                nodes.Reverse();

        }

        // this method is universal, exports a set from source db
        public static bool ExportSet(Form? parentForm, OpenFSDBContext? ctxSrc, ref List<myNode> allNodes, ref List<myNode> parentNodes, String dbName, String dstPath,
            DatabaseType destDBType, bool loadOperationForm = false)
        {
            // todo everything 29 October 2025

            OpenFSDBContext ctxDest = new OpenFSDBContext();
            if (allNodes.Count == 0) return false;
            if (parentNodes.Count == 0) return false;

            // todo if (!OpenFileSystemDB.CreateLoadDB(dstPath, dbName, ctxDest, true, true))
                //return false;


            // operations status form
            FormOperation? formOperation = null;
            if (loadOperationForm)
                formOperation = FormOperation.showForm(parentForm, "please wait. doing operation...", 0, 100, 0, 0);

            long DBIndex = 0;

            // first we need to create a set node. we cannot export without a new set node
            // 1 is preset as set node's id. set node is 1st to be indexed before all nodes most recursively.
            myNode? setNode = entryMethods.createSetNode(ref DBIndex, dbName, DateTime.Now);

			// create set node entry in the destination
			byte[]? xamlbytes = null;
			//DBCreateNode(ref cfgDest, ref setNode, "", xamlbytes, false, false, false, false, true, true);

            // load tree document object model structure
            myTreeDom treeDom = new myTreeDom();
            treeDom.buildCustomTree(allNodes, ref parentNodes, true, false);
            // nullify the entire tree with nulled non-db relative indexing.
            // we cannot use db indexing in an exported set.
            //treeDom.reindexTree(ref DBIndex);
            treeDom.applySetNode(setNode.chapter.Id);
            List<myTreeDomNode> tree = treeDom.ToList();

            // get the total number of nodes to export
            long total = tree.LongCount();
            long current = 0;

            foreach (myTreeDomNode listedNode in tree)
            {
                // load the rtf from current node
                myNode? node = listedNode.self;
                String rtf = "";
                //rtf = (String?)entryMethods.DBLoadNodeData(cfgSrc, listedNode.previousID, listedNode.self.DirectorySectionID);

                // reset special node type, remove system node type attribute because we are just making a clone set
                if (node.chapter.specialNodeType == SpecialNodeType.SystemNode) node.chapter.specialNodeType = SpecialNodeType.NonSystemNode;

				// finally write the node and it's data into destination
				xamlbytes = null;
				//entryMethods.DBCreateNode(ref cfgDest, ref node, rtf, xamlbytes, false, false, false, false, false, false);

                // update ui
                if (loadOperationForm)
                {
                    formOperation.updateProgressBar(current, total);
                    formOperation.updateFilesStatus(current, total);
                }

                // update
                current++;
            }

            // done

            // setup prepared final index in destination db
            //entryMethods.DBSetIndexingIndex(ref cfgDest, destDBType, DBIndex);

            // finally update the db index in file.
            //entryMethods.DBWriteIndexing(ref cfgDest);

            // now close the database
            //cfgDest.close();


            if (loadOperationForm)
                formOperation.close();

            // done
            return true;
        }

        // this method is universal, imports a set from source db
        public static bool ImportSet(Form? parentForm, OpenFSDBContext? ctxSrc, OpenFSDBContext? ctxDest, String dbName, ref List<myNode> allNodes,
            DatabaseType srcDBType, bool loadOperationForm = false)
        {
            // todo everything 29 October 2025

            // operations status form
            FormOperation? formOperation = null;
            if (loadOperationForm)
                formOperation = FormOperation.showForm(parentForm, "please wait. doing operation...", 0, 100, 0, 0);

            // collect all tree list from source db
            //List<myNode>? srcNodes = DBFindAllNodes(cfgSrc, false, false);

            // build tree from source nodes
            myTreeDom treeDom = new myTreeDom();
            //treeDom.buildTree(ref srcNodes, true, false);

            // we import the set with an additional new clone set node so that
            // the original export set node and it's date and time and settings remain preserved upon import.
            //myNode? setNode = entryMethods.createSetNode(ref cfgDest, dbName, DateTime.Now);

			// create set node entry in the destination
			byte[]? xamlbytes = null;
			//entryMethods.DBCreateNode(ref cfgDest, ref setNode, "", xamlbytes, false, false, false, false, true, true);
            
            // we must nullify system node type
            // we import clones, not anything original. so system node type cannot be used.
            treeDom.nullSpecialNodeType();

            // only a clone shall be imported. one and same set cannot be imported repeatedly because it
            // corrupts the db. therefore a set always shall be imported as a new clone.
            // it is user's own effort to manage the imported entries.
            // prepare the tree with proper current db's indexing
            //treeDom.reindexTree(ref cfgDest.ctx1.dbIndexing.currentDBIndex);

            // apply clone set node
            //treeDom.applySetNode(setNode.chapter.Id);

            // create a newly changed tree list from the tree dom structure.
            List<myTreeDomNode> tree = treeDom.ToList();
            List<myNode> setList = new List<myNode>();
            long total = tree.LongCount();
            long index = 0;

            // now direct import
            foreach (myTreeDomNode listedNode in tree)
            {
                // load source node
                String rtf = "";
                //rtf = (String?)DBLoadNodeData(cfgSrc, listedNode.previousID, listedNode.self.DirectorySectionID);

				// write destination node
				// clone set node is created with latest creation date. all other config and data is 1:1 cloned except the id.
				xamlbytes = null;
				//if (!entryMethods.DBCreateNode(ref cfgDest, ref listedNode.self, rtf, xamlbytes, true, false, false, false, false, false))
                //    continue;

                // add newly created node to set's list
                setList.Add(listedNode.self);

                // update ui
                if (loadOperationForm)
                {
                    formOperation.updateProgressBar(index, total);
                    formOperation.updateFilesStatus(index, total);
                }

                // update
                index++;
            }

            // done

            // finally update the db index in file.
            //entryMethods.DBWriteIndexing(ref cfgDest);

            // now first add set node
            //allNodes.Add(setNode);

            // now add all set list into the global session work list
            allNodes.AddRange(setList);

            // now close the database
            //cfgSrc.close();

            if (loadOperationForm)
                formOperation.close();

            // done
            return true;

        }

        // this function customizes the nodes or resets them to default properties
        public static bool DBCustomizeTreeNodesRecursive(OpenFSDBContext? ctx, ref List<myNode> nodes, bool set, bool setFont, bool setFontSize, bool setItalics, bool setBold, bool setStrikeout,
            bool setUnderline, bool setBackColor, bool setForeColor, Color backColor, Color foreColor, String fontName = "", float size = -1)
        {
            // todo everything
            /*
            if (!ctx.isDBOpen())
                return false;

            // phase 2: format all nodes

            foreach (myNode? node in nodes)
            {
                // reformat the node

                myNode? listedNode = node;
                System.Drawing.Font? font = myCommonMethods1.StringToFont(listedNode.chapter.HLFont);
                if (font == null)
                    font = ctx.config.tvEntriesFont;

                if (fontName == "")
                    fontName = font.Name;

                if (size <= 0)
                    size = font.Size;

                if (set)
                {
                    // customize to custom properties

                    if (setFont)
                        font = CustomFontDialog.getNewFontWithStyle(fontName, font.Size, font.Bold, font.Italic, font.Strikeout, font.Underline);

                    if (setFontSize)
                        font = CustomFontDialog.getNewFontWithStyle(font, size, font.Bold, font.Italic, font.Strikeout, font.Underline);

                    if (setBold)
                        font = CustomFontDialog.getNewFontWithStyle(font, font.Size, true, font.Italic, font.Strikeout, font.Underline);

                    if (setItalics)
                        font = CustomFontDialog.getNewFontWithStyle(font, font.Size, font.Bold, true, font.Strikeout, font.Underline);

                    if (setStrikeout)
                        font = CustomFontDialog.getNewFontWithStyle(font, font.Size, font.Bold, font.Italic, true, font.Underline);

                    if (setUnderline)
                        font = CustomFontDialog.getNewFontWithStyle(font, font.Size, font.Bold, font.Italic, font.Strikeout, true);
                }
                else
                {
                    // reset to default values

                    if (setFontSize)
                        size = ctx.config.tvEntriesFont.Size;

                    if (setFont)
                        font = CustomFontDialog.getNewFontWithStyle(ctx.config.tvEntriesFont, font.Size, font.Bold, font.Italic, font.Strikeout, font.Underline);

                    if (setFontSize)
                        font = CustomFontDialog.getNewFontWithStyle(font, size, font.Bold, font.Italic, font.Strikeout, font.Underline);

                    if (setBold)
                        font = CustomFontDialog.getNewFontWithStyle(font, font.Size, false, font.Italic, font.Strikeout, font.Underline);

                    if (setItalics)
                        font = CustomFontDialog.getNewFontWithStyle(font, font.Size, font.Bold, false, font.Strikeout, font.Underline);

                    if (setStrikeout)
                        font = CustomFontDialog.getNewFontWithStyle(font, font.Size, font.Bold, font.Italic, false, font.Underline);

                    if (setUnderline)
                        font = CustomFontDialog.getNewFontWithStyle(font, font.Size, font.Bold, font.Italic, font.Strikeout, false);
                }

                // update node
                listedNode.chapter.HLFont = myCommonMethods1.FontToString(font);

                // now configure colors

                if (backColor == Color.Empty)
                    backColor = ctx.config.tvEntriesBackColor;

                if (foreColor == Color.Empty)
                    foreColor = ctx.config.tvEntriesForeColor;

                if (setBackColor)
                    listedNode.chapter.HLBackColor = myCommonMethods1.ColorToString(backColor);

                if (setForeColor)
                    listedNode.chapter.HLFontColor = myCommonMethods1.ColorToString(foreColor);

                // finally update the node in db

                entryMethods.DBUpdateNodeOFSDB(ctx, ref listedNode, "", null, false, false, false);
            }

            // phase 3: completion

            // commit all
            entryMethods.DBWriteConfig(ctx);
            entryMethods.DBWriteIndexing(ctx);
            */

            return true;
        }

        /*
        public static bool DBNullOrEmptyNodes(OpenFSDBContext? ctx, ref List<myNode> nodes, out long processed, FormOperation? formop = null)
        {
            processed = 0;

            if (!ctx.isDBOpen())
                return false;

            // first get the total number of chapters which exist in db
            long allNodesCount = nodes.LongCount();
            long index = 0;

            foreach (myNode? listedNode in nodes)
            {
                myNode? node = listedNode;

                node.chapter.HLFont = "";//myCommonMethods1.FontToString(tvEntries.Font);
                node.chapter.HLFontColor = "";//myCommonMethods1.ColorToString(Color.Black);
                node.chapter.HLBackColor = "";// myCommonMethods1.ColorToString(Color.White);

                entryMethods.DBUpdateNodeOFSDB(ctx, ref node, "", null, false, false, false);

                if (formop != null)
                {
                    formop.updateProgressBar(index, allNodesCount);
                    formop.updateFilesStatus(index, allNodesCount);
                }
                index++;
            }

            // finally update the db index in file.
            entryMethods.DBWriteIndexing(ctx);

            processed = index;
            return true;
        }
        */

        // this method converts all nodes body to raw text
        public static bool DBNodesConvertToRawTextOFSDB(OpenFSDBContext? ctx, out long processed, FormOperation? formop = null)
        {
            processed = 0;

            // first get the total number of chapters which exist in db
            long total = Register.Total(ctx, ctx.dbNodeTreeRegistryFile);
            long index = 0;

            // first get the latest state of root register by id
            RegisterItem? root = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, 0, false, false, false, false, true, true);
            if (root == null) return false;

            // change root
            String? rtf = "";
            byte[]? xamlbytes = null;
            root.loadNode(ctx, ref rtf, ref xamlbytes, true);
            if (ctx.dbEntryType == EntryType.Xaml)
                xamlEntry.dummy.XamlBytes = xamlbytes;
            else
				xamlEntry.dummy.Rtf = rtf;
			
            xamlEntry.dummy.Text = xamlEntry.dummy.Text;
			entryMethods.DBUpdateNodeOFSDB(ctx, root.node, xamlEntry.dummy.Rtf, xamlEntry.dummy.XamlBytes, true, false, false);

            index++;
            if (formop != null)
            {
                formop.updateProgressBar(index, total);
                formop.updateFilesStatus(index, total);
            }

            while (true)
            {
                // load item from registry
                RegisterItem? nextItem = root.tree.Next();
                if (nextItem == null) break;

                rtf = "";
                xamlbytes = null;
                nextItem.loadNode(ctx, ref rtf, ref xamlbytes, true);
				if (ctx.dbEntryType == EntryType.Xaml)
					xamlEntry.dummy.XamlBytes = xamlbytes;
				else
					xamlEntry.dummy.Rtf = rtf;

				xamlEntry.dummy.Text = xamlEntry.dummy.Text;
				entryMethods.DBUpdateNodeOFSDB(ctx, nextItem.node, xamlEntry.dummy.Rtf, xamlEntry.dummy.XamlBytes, true, false, false);

				index++;
                if (formop != null)
                {
                    formop.updateProgressBar(index, total);
                    formop.updateFilesStatus(index, total);
                }
            }

            processed = index;
            return true;
        }
        // this method converts db from rtf to xaml or xaml to rtf etc.
        public static bool DBConvertOFSDB(OpenFSDBContext? ctx, 
            bool fromRtfToXaml, out long processed, FormOperation? formop = null)
        {
            processed = 0;

            // first get the total number of chapters which exist in db
            long total = Register.Total(ctx, ctx.dbNodeTreeRegistryFile);
            long index = 0;

            // first get the latest state of root register by id
            RegisterItem? root = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, 0, false, false, false, false, true, true);
            if (root == null) return false;

            // change root
            String? rtf = "";
            byte[]? xamlbytes = null;
            root.loadNode(ctx, ref rtf, ref xamlbytes, true);

            if (fromRtfToXaml)
                xamlbytes = xamlEntry.toXaml(rtf);
            else
                rtf = xamlEntry.toRtf(xamlbytes);

            entryMethods.DBPurgeNodeOFSDB(ctx, root.node.chapter.Id, root.node.DirectorySectionID, false, true);
            entryMethods.DBUpdateNodeOFSDB(ctx, root.node, rtf, xamlbytes, true, false, false, ((fromRtfToXaml) ? EntryType.Xaml : EntryType.Rtf));

            index++;
            if (formop != null)
            {
                formop.updateProgressBar(index, total);
                formop.updateFilesStatus(index, total);
            }

            while (true)
            {
                // load item from registry
                RegisterItem? nextItem = root.tree.Next();
                if (nextItem == null) break;

				// change current item

				rtf = "";
				xamlbytes = null;
				nextItem.loadNode(ctx, ref rtf, ref xamlbytes, true);

				if (fromRtfToXaml)
					xamlbytes = xamlEntry.toXaml(rtf);
				else
					rtf = xamlEntry.toRtf(xamlbytes);

				entryMethods.DBPurgeNodeOFSDB(ctx, nextItem.node.chapter.Id, nextItem.node.DirectorySectionID, false, true);
				entryMethods.DBUpdateNodeOFSDB(ctx, nextItem.node, rtf, xamlbytes, true, false, false, ((fromRtfToXaml) ? EntryType.Xaml : EntryType.Rtf));

                index++;
                if (formop != null)
                {
                    formop.updateProgressBar(index, total);
                    formop.updateFilesStatus(index, total);
                }
            }
            
            if (fromRtfToXaml)
            {
                ctx.isXamlDB = true;
                ctx.dbEntryType = EntryType.Xaml;
            }
            else
            {
                ctx.isXamlDB = false;
                ctx.dbEntryType = EntryType.Rtf;
            }
            processed = index;
            return true;
        }

        // this method copies or moves the loaded db
        public static bool DBCopyDatabaseOFSDB(OpenFSDBContext? ctx, String dest, bool convert, bool convertToRawRtf, bool toXaml, FormOperation? formOperation = null)
        {
            return OpenFileSystemDB.CopyDB(ctx, dest, convert, convertToRawRtf, toXaml, formOperation);
        }
    }
}

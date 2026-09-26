using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheBook.Net.Core;

namespace DiaryJournal.Net
{
    public static class theJournalMethods
    {
        public const String defaultEntryDelimiter = @"--+--";

        public static List<String> partitionEntryFileIntoNodes(String filename)
        {
            List<String> nodes = new List<string>();
            if (filename.Length <= 0) return nodes;

            FileInfo info = new FileInfo(filename);
            String name = Path.GetFileNameWithoutExtension(info.Name);
            if (name.Length <= 0) return nodes;

            String[] nodeNames = name.Split(defaultEntryDelimiter);
            if (nodeNames.Length <= 0) return nodes;

            // nodes found, output them as list.
            nodes.AddRange(nodeNames);
            return nodes;
        }

        public static List<myNode> initNodesLineTJNC(OpenFSDBContext? ctx, ref List<myNode> primaryWorkList, ref List<String> lineageNames,
            UInt32 rootParentId, RegisterItem? emptySlotsItem)
        {
            List<myNode> nodes = new List<myNode>();

            UInt32 parentId = rootParentId;
            myNode? prevNode = null;
            for (int i = 0; i < lineageNames.Count; i++)
            {
                // load existing node or create new node
                myNode? node = initializeNodeTJNC(ctx, ref primaryWorkList, parentId, emptySlotsItem, lineageNames[i]);
                if (node == null) return nodes; // error creating/loading node

                // reconfigure
                prevNode = node;
                parentId = node.chapter.Id;
                nodes.Add(node);
            }
            return nodes;
        }

        public static myNode? initializeNodeTJNC(OpenFSDBContext? ctx, ref List<myNode> primaryWorkList, UInt32 parentId, RegisterItem? emptySlotsItem, String title)
        {
            // find if node exists
            List<myNode> children = entryMethods.findFirstLevelChildren(parentId, ref primaryWorkList, true, false);            

            // check if node already exists, else create it
            myNode? node = entryMethods.FindNodeInListByTitle(ref children, title);
            if (node != null) return node; // node already exists, so return it

            // node does not exists, so create new node
            //node = entryMethods.DBNewNode(ref cfg, SpecialNodeType.None, NodeType.NonCalendarEntry, DomainType.AnyOrAll,
            //    ref node, true, true, true, DateTime.Now, parentId, true, title, "", true, false, false);
            //if (node == null) return null; // error creating node

            // configure new node
            node = new myNode(true);
            node.chapter.specialNodeType = SpecialNodeType.None;
            node.chapter.nodeType = NodeType.NonCalendarEntry;
            node.chapter.domainType = DomainType.AnyOrAll;
            node.chapter.chapterDateTime = node.chapter.creationDateTime = node.chapter.modificationDateTime = DateTime.Now;
            node.chapter.Title = title;
            node.chapter.parentId = parentId;

            // node does not exists, so create new node
            byte[]? xamlbytes = null;
            RegisterItem? item = Register.Insert(ctx, parentId, emptySlotsItem, node, "", xamlbytes, false, false);
            if (item == null) return null;
            
            // node created in db, add it in primary work list and return it
            primaryWorkList.Add(node);
            return node;
        }
        public static Chapter? convertFilenameToChapter(String file)
        {
            Chapter chapter = new Chapter();
            FileInfo fileInfo = new FileInfo(file);
            String filename = fileInfo.Name;
            String title = "";
            String dateTime = "";
            String dateTimeFormat = "";
            DateTime chapterDate = DateTime.Now;

            // formal entry
            String pattern0 = @"(\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d)(\..*)";
            // child loose leaf entry
            String pattern1 = @"(\d\d\d\d-\d\d-\d\d-\d\d-\d\d)\+(.*)(\..*)";
            // formal entry with title
            String pattern2 = @"(\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d)(\s+-\s+)(.*)(\..*)";
            // formal entry with duplication index
            String pattern3 = @"(\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d)(.*)(\([0-9]+\))(.*)(\..*)";
            // child loose leaf entry multiple parent-child levels
            //String pattern4 = @"(\d\d\d\d-\d\d-\d\d-\d\d-\d\d)(\+)([0-9]*)(--+--)(.*)(\.[a-zA-Z].*)";
            // child loose leaf entry multiple parent-child levels delimitor --+--
            //String pattern5 = @"(\d\d\d\d--\+--\d\d--\+--\d\d--\+--\d\d--\+--\d\d)(\+)([0-9]*)(.*)(\.[a-zA-Z].*)";
            // new mode child loose leaf entry multiple parent-child levels delimiter: --+--
            String pattern4 = @"(\d\d\d\d--\+--\d\d--\+--\d\d--\+--\d\d--\+--\d\d)(.*)(\.[a-zA-Z].*)";
            // new mode child loose leaf entry multiple parent-child levels delimiter: --+--
            String pattern5 = @"(\d\d\d\d--\+--\d\d--\+--\d\d--\+--\d\d--\+--\d\d)(\+)([0-9]*)(.*)(\.[a-zA-Z].*)";

            Regex regex0 = new Regex(pattern0, RegexOptions.IgnoreCase);
            MatchCollection matches0 = regex0.Matches(filename);

            Regex regex1 = new Regex(pattern1, RegexOptions.IgnoreCase);
            MatchCollection matches1 = regex1.Matches(filename);

            Regex regex2 = new Regex(pattern2, RegexOptions.IgnoreCase);
            MatchCollection matches2 = regex2.Matches(filename);

            Regex regex3 = new Regex(pattern3, RegexOptions.IgnoreCase);
            MatchCollection matches3 = regex3.Matches(filename);

            Regex regex4 = new Regex(pattern4, RegexOptions.IgnoreCase);
            MatchCollection matches4 = regex4.Matches(filename);

            Regex regex5 = new Regex(pattern5, RegexOptions.IgnoreCase);
            MatchCollection matches5 = regex5.Matches(filename);

            //Regex regex6 = new Regex(pattern6, RegexOptions.IgnoreCase);
            //MatchCollection matches6 = regex6.Matches(filename);

            if (matches0.Count > 0)
            {
                if (matches0[0].Groups.Count < 2)
                    return null;
                
                dateTime = matches0[0].Groups[1].Value;
                dateTimeFormat = @"yyyy-MM-dd-HH-mm-ss";
            }
            else if (matches1.Count > 0)
            {
                if (matches1[0].Groups.Count < 2)
                    return null;

                dateTime = matches1[0].Groups[1].Value;
                dateTimeFormat = @"yyyy-MM-dd-HH-mm";
                title = matches1[0].Groups[2].Value;
            }
            else if (matches2.Count > 0)
            {
                if (matches2[0].Groups.Count < 5)
                    return null;

                dateTime = matches2[0].Groups[1].Value;
                dateTimeFormat = @"yyyy-MM-dd-HH-mm-ss";
                title = matches2[0].Groups[3].Value;
            }
            else if (matches3.Count > 0)
            {
                if (matches3[0].Groups.Count < 5)
                    return null;

                dateTime = matches3[0].Groups[1].Value;
                dateTimeFormat = @"yyyy-MM-dd-HH-mm-ss";
                title = matches3[0].Groups[3].Value;
            }
            else if (matches4.Count > 0)
            {
                if (matches4[0].Groups.Count < 4)
                    return null;

                dateTime = matches4[0].Groups[1].Value;
                dateTimeFormat = @"yyyy--+--MM--+--dd--+--HH--+--mm";
                title = matches4[0].Groups[2].Value;
            }
            else if (matches5.Count > 0)
            {
            }
            else
            {
                return null;
            }

            if (matches5.Count > 0)
            {
                if (matches5[0].Groups.Count < 6)
                    return null;

                // note that matches4 should be filled with loose entry title, we should use that title.
                //"20170503203035530"
                //"2017-05-03-20-30-35-530"
                dateTime = matches5[0].Groups[3].Value;
                dateTimeFormat = @"yyyyMMddHHmmssfff";
            }

            try
            {
                chapterDate = DateTime.ParseExact(dateTime, dateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
                chapter.chapterDateTime = chapterDate;
            }
            catch (Exception)
            {

            }
            chapter.Title = title;
            return chapter;
        }
    }
}

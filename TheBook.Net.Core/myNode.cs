using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DiaryJournal.Net
{
    public class Chapter
    {
        public UInt32 Id { get; set; }
        public UInt32 parentId { get; set; } = 0;
        public String? Title { get; set; } = "";
        public bool IsDeleted { get; set; } = false;
        public DateTime chapterDateTime { get; set; }
        public DateTime creationDateTime { get; set; }
        public DateTime modificationDateTime { get; set; }
        public DateTime deletionDateTime { get; set; }
        public String HLFont { get; set; } = "";
        public String HLFontColor { get; set; } = "";
        public String HLBackColor { get; set; } = "";
        public Int32 caretIndex { get; set; }
        public Int32 caretSelectionLength { get; set; }
        public NodeType nodeType { get; set; } = NodeType.Entry;
        public SpecialNodeType specialNodeType { get; set; } = SpecialNodeType.None;
        public DomainType domainType { get; set; } = DomainType.Journal;
        public Int32 documentWidth { get; set; } = myConfig.defaultDocumentWidth;
        public Guid guid { get; set; } = Guid.NewGuid();

        public Chapter? ShallowCopy()
        {
            return (Chapter)this.MemberwiseClone();
        }

        public Chapter? DeepCopy()
        {
            Chapter? other = (Chapter?)this.MemberwiseClone();
            //other.IdInfo = new IdInfo(IdInfo.IdNumber);
            //other.Name = String.Copy(Name);
            return other;
        }
    }

    // note that we cannot store data blob with chapter in db, because when we load the chapter, entire blob is loaded as well, which is a bug.
    // so we keep the data blob in another table to prevent it from automatically loading.
    public class ChapterData
    {
        public UInt32 Id { get; set; }
        public String data { get; set; } = "";
    }

    public enum NodeType : byte
    {
        Journal = 0,
        Library = 1,
        Set = 2,
        Year = 3,
        Month = 4,
        Entry = 5,
        Template = 6,
        Label = 7,
        NonCalendarEntry = 8,
        Notebook = 9,
        Registry = 10,
        Workspace = 11,
        Finance = 12,
        Security = 13,
        Domestic = 14,
        Legal = 15,
        Identity = 16,
        Confidential = 17,
        Roughwork = 18,
        Backup = 19,
        Templates = 20,
        Briefcase = 21,
        EmptySlot = 22,
        Root = 23,
        AnyOrAll = 100,
        Invalid = 255
    }
    public enum SpecialNodeType : byte
    {
        SystemNode = 0,
        NonSystemNode = 1,
        None = 2,
        Root = 3,
        EmptySlot = 4,
        AnyOrAll = 100,
        Invalid = 255
    }

    public enum DomainType : byte
    {
        Journal = 0,
        Library = 1,
        HiddenCore = 2,
        Root = 3,
        EmptySlot = 4,
        AnyOrAll = 100,
        None = 200,
        Invalid = 255
    }

    [Flags]
    public enum RegisterItemFlags1 : byte
    {
        None = 0,
        Marked = 1,
        Processed = 2,
    }

    public enum EntryType : byte
    {
        Xml = 0,
        Rtf = 1,
        Html = 2,
        Txt = 3,
        Cfg = 4,
        Pdf = 5,
        Xaml = 6,
        Default = 7
    }

    public class myNode
    {
        public Chapter? chapter = null;
        public UInt32 previousID = 0;
        public UInt32 DirectorySectionID = 0;
        
        // new 27 March 2023 : 08:43 PM
        public List<myNode> lineage = new List<myNode>();

        public myNode(ref Chapter chapter)
        {
            this.chapter = chapter;
        }
        public myNode(bool newChapter = true)
        {
            if (newChapter)
                this.chapter = new Chapter();
        }

        public myNode? ShallowCopy()
        {
            return (myNode)this.MemberwiseClone();
        }

        public myNode? DeepCopy()
        {
            myNode other = (myNode)this.MemberwiseClone();
            
            if (this.chapter != null)
                other.chapter = this.chapter.DeepCopy();
            
            if (this.lineage != null)   
                other.lineage = new List<myNode>(this.lineage.ToArray());

            return other;
        }

    }

    // system nodes collection
    public class mySystemNodes
    {
        public List<myNode> YearNodes = new List<myNode>();
        public List<myNode> MonthNodes = new List<myNode>();
        public Dictionary<NodeType, myNode> SystemNodes = new Dictionary<NodeType, myNode?>();
        public static List<String> SystemNodesNames = new List<String>() { "Journal", "Library", "Notebook", "Registry", "Workspace", 
            "Finance", "Security", "Domestic", "Legal", "Identity", "Confidential", "Roughwork", "Backup", "Templates", "Briefcase",
            "EmptySlot" };

        public myNode? getSystemNode(NodeType type)
        {
            myNode? value = null;
            if (SystemNodes.TryGetValue(type, out value))
                return value;
            else
                return null;
        }

        public static String? getSystemNodeName(NodeType type)
        {
            return type.convertToString();
        }

        public static NodeType getSystemNodeTypeByName(String name)
        {
            object? type = null;
            if (Enum.TryParse(typeof(NodeType), name, out type))
            {
                return (NodeType)type;
            }
            else
            {
                // suppose old version db, so we need to convert old version to new version wherever required
                switch (name)
                {
                    case "JournalNode":
                        return NodeType.Journal;
                    case "LibraryNode":
                        return NodeType.Library;
                    case "SetNode":
                        return NodeType.Set;
                    case "YearNode":
                        return NodeType.Year;
                    case "MonthNode":
                        return NodeType.Month;
                    case "EntryNode":
                        return NodeType.Entry;
                    case "TemplateNode":
                        return NodeType.Template;
                    case "LabelNode":
                        return NodeType.Label;
                    case "NonCalendarEntryNode":
                        return NodeType.NonCalendarEntry;
                    case "Notebook":
                        return NodeType.Notebook;
                    case "Registry":
                        return NodeType.Registry;
                    case "Workspace":
                        return NodeType.Workspace;
                    case "Finance":
                        return NodeType.Finance;
                    case "Security":
                        return NodeType.Security;
                    case "Domestic":
                        return NodeType.Domestic;
                    case "Legal":
                        return NodeType.Legal;
                    case "Identity":
                        return NodeType.Identity;
                    case "Confidential":
                        return NodeType.Confidential;
                    case "Roughwork":
                        return NodeType.Roughwork;
                    case "Backup":
                        return NodeType.Backup;
                    case "Template":
                        return NodeType.Template;
                    case "Templates":
                        return NodeType.Templates;
                    case "Briefcase":
                        return NodeType.Briefcase;
                    case "EmptySlot":
                        return NodeType.EmptySlot;
                    case "AnyOrAll":
                        return NodeType.AnyOrAll;
                }
            }
            return NodeType.Invalid;
        }

        public bool setSystemNode(NodeType type, ref myNode? node)
        {
            if (node == null) return false;
            if (getSystemNode(type) != null) return false;  
            SystemNodes.Add(type, node);
            return true;
        }

        public static bool isCoreSystemNode(NodeType type)
        {
            String typeName = getSystemNodeName(type);
            String? value = SystemNodesNames.FirstOrDefault(s => s == typeName);
            if ((value != null) && (value != "")) return true;
            return false;
        }

        public List<myNode> findAllSystemNodes()
        {
            List<myNode> list = new List<myNode> ();
            foreach (KeyValuePair<NodeType, myNode> item in SystemNodes)
                list.Add(item.Value);

            list.AddRange(YearNodes);
            list.AddRange(MonthNodes);
            return list;
        }
        public mySystemNodes()
        {

        }
    }
}

/*
 * 
 * 
 * Tushar Jain's Open File System Database (OpenFileSystemDB.cs)
 * location: https://github.com/tusharjain1082/DiaryJournal.Net
 * version: 1.1.0.0
 * description: I myself invented this framework on my own without
 * copying any material from others. this framework is solely my own idea and invention.
 * what we can do with this open file system database is that we can retrieve 
 * and change and even delete database and it's files directly through windows
 * explorer. this database is not single file based database. it is
 * windows file system based database. all database's entries are
 * openly stored as ordinary windows .rtf and .xml files in the 
 * database's own folder. so we can directly in a single step read the files
 * and do whatever we want with the files. so there is no issue of import and export
 * of database because entire database is stored as a common structure of open folders and files
 * in the open like all the files on windows and hard disk. so we can directly pick the files and move/backup them.
 * license: open source and free. no license. i dedicate this work to the public. 
 * initial completion and release date: Saturday, 20 August, 2022.
 * 
 * 
 */

using RtfPipe;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Security.RightsManagement;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml;
using TheBook.Net.Core;

namespace DiaryJournal.Net
{
    public class OpenFSDBContext
    {
        public bool dbLoaded = false;
        public String dbBaseParentPath = "";
        public String dbBasePath = "";
        public String dbEntryPath = "";
        public String dbEntryConfigPath = "";
        public String dbName = "";
        public String dbConfigFile = "";
        public String dbUsedSlotsFile = "";
        public String dbNodeTreeRegistryFile = "";
        public String dbBackupPath = "";
        public String dbTmpPath = "";
        public DatabaseConfig? dbConfig = new DatabaseConfig();
        public EntryType dbEntryType = EntryType.Rtf;
        public EntryType dbEntryConfigType = EntryType.Cfg;
        public Register dbNodesTreeRegistry = new Register();
        public OpenFSDBSections dbSections = new OpenFSDBSections();
        public bool isXamlDB = false; // false means rtf db true means xaml db
        public myConfig? config = null;
        public bool readOnly = false;

        // memory mapped file stream
        private System.IO.MemoryMappedFiles.MemoryMappedFile? __regFilemmf = null;
        public System.IO.MemoryMappedFiles.MemoryMappedViewStream? regFileStream = null;
        public Int64 regFileSize = 0;

        // used slots config
        public UInt32 usedSlots = 0;
        //public FileStream? usedSlotsFS = null;
        //public StreamReader? usedSlotsReader = null;
        //public StreamWriter? usedSlotsWriter = null;

        public bool readUsedSlotsFile()
        {
            if (!File.Exists(this.dbUsedSlotsFile)) return false;
            String v = File.ReadAllText(this.dbUsedSlotsFile);
            this.usedSlots = UInt32.Parse(v);
            return true;
        }

        public bool writeUsedSlotsFile()
        {
            String v = this.usedSlots.ToString();
            try
            {
                File.WriteAllText(this.dbUsedSlotsFile, v);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /*
        public bool readUsedSlotsFile()
        {
            if (usedSlotsFS == null) return false;
            usedSlotsFS.Position = 0;
            String body = usedSlotsReader.ReadToEnd();
            this.usedSlots = UInt32.Parse(body);
            return true;
        }

        public bool writeUsedSlotsFile(bool flush)
        {
            if (usedSlotsFS == null) return false;
            usedSlotsFS.Position = 0;
            usedSlotsFS.SetLength(0);
            usedSlotsWriter.Write(usedSlots.ToString());
            if (flush)
            {
                usedSlotsWriter.Flush();
                usedSlotsFS.Flush();
            }
            return true;
        }
        */
        public bool RecreateRegistryFile()
        {
            // first close
            try
            {
                if (this.regFileStream != null)
                {
                    this.regFileStream.Flush();
                    this.regFileStream.Close();
                    this.regFileStream.Dispose();
                    this.__regFilemmf.Dispose();
                    this.regFileStream = null;
                    this.__regFilemmf = null;
                }
            }
            catch
            {

            }

            // create new register file and preallocate a total number of zeroed slots in it
            if (!Register.toFile(this.dbNodeTreeRegistryFile)) return false;
            
            // finally load file into mmf
            return openRegistryMMF();

        }
        public bool openRegistryMMF()
        {
            FileInfo info = new FileInfo(this.dbNodeTreeRegistryFile);
            if (!info.Exists) return false;

            // set size status
            this.regFileSize = info.Length;

            // 2. Create the MemoryMappedFile and a view accessor for the entire file
            this.__regFilemmf = MemoryMappedFile.CreateFromFile(this.dbNodeTreeRegistryFile, FileMode.Open, null, 0, MemoryMappedFileAccess.ReadWrite);
            if (this.__regFilemmf == null) return false;

            this.regFileStream = this.__regFilemmf.CreateViewStream(0, 0, MemoryMappedFileAccess.ReadWrite);
            if (this.regFileStream == null)
            {
                this.__regFilemmf.Dispose();
                return false;
            }
            return true;
        }
        public bool isDBOpen()
        {
            return dbLoaded;
        }

        public void close()
        {
            try
            {
                if (this.regFileStream != null)
                {
                    this.regFileStream.Flush();
                    this.regFileStream.Close();
                    this.regFileStream.Dispose();
                    this.__regFilemmf.Dispose();
                    this.regFileStream = null;
                    this.__regFilemmf = null;
                }
            }
            catch
            {

            }
            dbLoaded = false;
            dbBasePath = dbEntryPath = dbName = dbBaseParentPath = dbEntryConfigPath = dbConfigFile = "";
            dbConfig = new DatabaseConfig();
                
        }
    }

    public static class OpenFileSystemDB
    {
        public static String defaultDBPath = Application.StartupPath;
        public static String defaultDBPath_factory = Application.StartupPath;
        public static String defaultDBName = "myJournal";
        public static String defaultDBName_factory = "myJournal";
        public static String defaultDBEntryDirName = "Entries";
        public static String defaultDBEntryCfgDirName = "EntryConfig";
        public static String defaultDBTmpDirName = "tmp";
        public static String defaultDBBackupDirName = "backup";
        //public const string XmlconfigFileName = "OpenFSDBConfig.xml";
        public const string configFileName = "OpenFSDBConfig.yaml";
        public const string usedSlotsFileName = "usedSlots.cfg";
        public const string nodeTreeRegistryFileName = "node-tree-registry.bin";

        // this method creates register file and root node in it
        public static bool DBCreateRegister(OpenFSDBContext? ctx, bool force)
        {
            // force create or create new register file and preallocate a total number of zeroed slots in it

            if (force)
            {
                // new file required
                ctx.RecreateRegistryFile();
            }
            else
            {
                // file already exists and force is not used so skip
                if (File.Exists(ctx.dbNodeTreeRegistryFile)) return true; // if register file exists, skip creation
            }

            if (!File.Exists(ctx.dbNodeTreeRegistryFile)) // file does not exists, so create new
                ctx.RecreateRegistryFile();

            // create root node files and register entry
            myNode root = new myNode(true);
            root.chapter.specialNodeType = SpecialNodeType.Root;
            root.chapter.nodeType = NodeType.Root;
            root.chapter.chapterDateTime = DateTime.Now;
            root.chapter.Title = CoreFramework.convertToString(root.chapter.nodeType);
            root.chapter.domainType = DomainType.Root;
            root.chapter.Id = 0;
            root.chapter.parentId = 0;
            String? rtf = "";
            byte[]? xamlbytes = null;
            myNode? found = entryMethods.DBSearchNodeOFSDB(ctx, 0, ref rtf, ref xamlbytes, true);

            if (!OpenFileSystemDB.createNode(ctx, root, rtf, xamlbytes, true, true, true, true))
                return false; // critical error

            // todo tushar: lineage root head tail etc.
            // finally update the register add this node
            RegisterItem? rootItem = new RegisterItem(0, root.chapter.Id,
            root.chapter.parentId, root.DirectorySectionID, 0, 0, 0, 0, 0, root.chapter.nodeType, root.chapter.specialNodeType, root.chapter.domainType,
            0, 0);

            if (Register.InsertNode(ctx, rootItem) < 0)
                return false; // critical error

            // update used slots config
            ctx.usedSlots += 1;
            ctx.writeUsedSlotsFile();

            // done
            return true;
        }

        // prepares db inside vhd file.
        public static bool PrepareDB(OpenFSDBContext ctx, String path, String name, bool isXamlDB)
        {
            if (name == "") name = defaultDBName;

            // prepare formatted paths set
            String dbBasePath = Path.Combine(path, name);
            String dbConfigFile = Path.Combine(dbBasePath, configFileName);
            String dbEntryPath = Path.Combine(dbBasePath, defaultDBEntryDirName);
            String dbEntryCfgPath = Path.Combine(dbBasePath, defaultDBEntryCfgDirName);
            String dbTmpPath = Path.Combine(dbBasePath, defaultDBTmpDirName);
            String dbBackupPath = Path.Combine(dbBasePath, defaultDBBackupDirName);
            String dbUsedSlotsFile = Path.Combine(dbBasePath, usedSlotsFileName);

            // first delete old paths
            if (Directory.Exists(dbBasePath))
               Directory.Delete(dbBasePath, true);

            // secand create new paths
            Directory.CreateDirectory(dbBasePath);
            Directory.CreateDirectory(dbEntryPath);
            Directory.CreateDirectory(dbEntryCfgPath);
            Directory.CreateDirectory(dbTmpPath);
            Directory.CreateDirectory(dbBackupPath);

            // initialize and write brand new config file. if it exists, load it.
            ctx.dbConfig = new DatabaseConfig();
            ctx.dbConfig.setName = name;
            ctx.dbConfig.setDateTime = DateTime.Now;
            ctx.dbConfig.setID = Guid.NewGuid();

            // initialize config
            String dbNodeTreeRegistryFile = Path.Combine(dbBasePath, nodeTreeRegistryFileName);

            // success
            ctx.isXamlDB = isXamlDB;
            if (isXamlDB) ctx.dbEntryType = EntryType.Xaml;
            ctx.dbBasePath = dbBasePath;
            ctx.dbBaseParentPath = path;
            ctx.dbEntryPath = dbEntryPath;
            ctx.dbEntryConfigPath = dbEntryCfgPath;
            ctx.dbBackupPath = dbBackupPath;
            ctx.dbTmpPath = dbTmpPath;
            ctx.dbConfigFile = dbConfigFile;
            ctx.dbUsedSlotsFile = dbUsedSlotsFile;
            ctx.dbNodeTreeRegistryFile = dbNodeTreeRegistryFile;
            ctx.dbLoaded = true; // set marker db is loaded
            ctx.dbName = ctx.dbConfig.setName;

            DatabaseConfig.toYamlFile(ctx.dbConfig, dbConfigFile);

            // initialize and write brand new db nodes tree registry file. if it exists, load it.
            ctx.dbNodesTreeRegistry = new Register();

            // init sections class
            ctx.dbSections = new OpenFSDBSections(ctx.dbBaseParentPath, ctx.dbBasePath, ctx.dbEntryPath, ctx.dbEntryConfigPath);
            // auto load or create sections
            OpenFSDBSections.loadReloadSections(ctx);

            // create new used slots file
            ctx.usedSlots = 0;
            ctx.writeUsedSlotsFile();

            // finally create register and 1st node which is root node
            DBCreateRegister(ctx, true);

            // we create all missing system nodes
            // first create any missing root system node
            List<RegisterItem>? registry = null;
            RegisterItem? emptySlotsItem = null;
            entryMethodsNewDesign.DBCreateDBCore(ctx, ref emptySlotsItem, true);

            // now load all root system nodes
            entryMethodsNewDesign.DBLoadRootSystemNodes(ctx, ref registry, ref emptySlotsItem);

            GC.Collect();

            // test
            //Register.test(ctx.VhdCtx, ctx.VhdCtx.dbCtx.dbNodeTreeRegistryFile);

            return true;
        }

        // prepares db inside vhd file.
        public static OpenFSDBContext? PrepareInitialCloneDB(OpenFSDBContext src, String path)
        {
            OpenFSDBContext ctx = new OpenFSDBContext();

            // prepare formatted paths set
            String dbBasePath = Path.Combine(path, src.dbName);
            String dbConfigFile = Path.Combine(dbBasePath, configFileName);
            String dbEntryPath = Path.Combine(dbBasePath, defaultDBEntryDirName);
            String dbEntryCfgPath = Path.Combine(dbBasePath, defaultDBEntryCfgDirName);
            String dbTmpPath = Path.Combine(dbBasePath, defaultDBTmpDirName);
            String dbBackupPath = Path.Combine(dbBasePath, defaultDBBackupDirName);
            String dbUsedSlotsFile = Path.Combine(dbBasePath, usedSlotsFileName);

            // first delete old paths
            if (Directory.Exists(dbBasePath))
                Directory.Delete(dbBasePath, true);

            // secand create new paths
            Directory.CreateDirectory(dbBasePath);
            Directory.CreateDirectory(dbEntryPath);
            Directory.CreateDirectory(dbEntryCfgPath);
            Directory.CreateDirectory(dbTmpPath);
            Directory.CreateDirectory(dbBackupPath);

            // initialize and write brand new config file. if it exists, load it.
            ctx.dbConfig = new DatabaseConfig();
            ctx.dbConfig.setName = src.dbName;
            ctx.dbConfig.setDateTime = DateTime.Now;
            ctx.dbConfig.setID = Guid.NewGuid();
            ctx.dbConfig.lastModifiedEntry = src.dbConfig.lastModifiedEntry;
            ctx.dbConfig.latestCreatedEntry = src.dbConfig.latestCreatedEntry;
            DatabaseConfig.toYamlFile(ctx.dbConfig, dbConfigFile);

            // directly copy register
            String dbNodeTreeRegistryFile = Path.Combine(dbBasePath, nodeTreeRegistryFileName);
            using (FileStream outs = new FileStream(dbNodeTreeRegistryFile, FileMode.Create, FileAccess.Write))
            {
                src.regFileStream.Flush();
                src.regFileStream.Position = 0;
                src.regFileStream.CopyTo(outs);
                outs.Flush();
                outs.SetLength(src.regFileSize);
            }
            //File.Copy(src.dbNodeTreeRegistryFile, dbNodeTreeRegistryFile);
            ctx.dbNodesTreeRegistry = new Register();

            // success
            ctx.dbBasePath = dbBasePath;
            ctx.dbBaseParentPath = path;
            ctx.dbEntryPath = dbEntryPath;
            ctx.dbEntryConfigPath = dbEntryCfgPath;
            ctx.dbBackupPath = dbBackupPath;
            ctx.dbTmpPath = dbTmpPath;
            ctx.dbConfigFile = dbConfigFile;
            ctx.dbNodeTreeRegistryFile = dbNodeTreeRegistryFile;
            ctx.dbLoaded = true; // set marker db is loaded
            ctx.dbName = ctx.dbConfig.setName;
            ctx.dbEntryType = src.dbEntryType;
            ctx.dbUsedSlotsFile = dbUsedSlotsFile;

            // init sections class
            ctx.dbSections = new OpenFSDBSections(ctx.dbBaseParentPath, ctx.dbBasePath, ctx.dbEntryPath, ctx.dbEntryConfigPath);
            // create all sections
            OpenFSDBSections.createSections(ctx, 1, (UInt32)src.dbSections.sections.Count);

            // create new clone used slots file in clone db
            ctx.usedSlots = src.usedSlots;
            ctx.writeUsedSlotsFile();

            GC.Collect();
            return ctx;
        }
        public static bool LoadVHDFileDB(OpenFSDBContext ctx, String path, bool readOnly)
        {
            // check base path
            String dbBasePath = path;
            if (path == "") dbBasePath = Path.Combine(@"\", defaultDBName);
            if (!Directory.Exists(dbBasePath)) return false;

            // prepare formatted paths set
            String dbConfigFile = Path.Combine(dbBasePath, configFileName);
            //String dbXmlConfigFile = Path.Combine(dbBasePath, XmlconfigFileName);
            String dbEntryPath = Path.Combine(dbBasePath, defaultDBEntryDirName);
            String dbEntryCfgPath = Path.Combine(dbBasePath, defaultDBEntryCfgDirName);
            String dbTmpPath = Path.Combine(dbBasePath, defaultDBTmpDirName);
            String dbBackupPath = Path.Combine(dbBasePath, defaultDBBackupDirName);
            String dbUsedSlotsFile = Path.Combine(dbBasePath, usedSlotsFileName);

            // load db config from db base path
            //DatabaseConfig.fromXml(ctx.dbConfig, dbXmlConfigFile);
            ctx.dbConfig = DatabaseConfig.fromYamlFile(dbConfigFile);

            // load db name
            String dbName = ctx.dbConfig.setName;

            // validate
            if (!Directory.Exists(dbEntryPath)) return false;
            if (!Directory.Exists(dbEntryCfgPath)) return false;
            if (!Directory.Exists(dbTmpPath)) Directory.CreateDirectory(dbTmpPath);
            if (!Directory.Exists(dbBackupPath)) Directory.CreateDirectory(dbBackupPath);

            // nodes tree registry file
            String dbNodeTreeRegistryFile = Path.Combine(dbBasePath, nodeTreeRegistryFileName);
            ctx.dbNodesTreeRegistry = new Register();

            // success
            ctx.dbBasePath = dbBasePath;
            ctx.dbBaseParentPath = path;
            ctx.dbEntryPath = dbEntryPath;
            ctx.dbEntryConfigPath = dbEntryCfgPath;
            ctx.dbBackupPath = dbBackupPath;
            ctx.dbTmpPath = dbTmpPath;
            ctx.dbConfigFile = dbConfigFile;
            ctx.dbNodeTreeRegistryFile = dbNodeTreeRegistryFile;
            ctx.dbLoaded = true; // set marker db is loaded
            ctx.dbName = ctx.dbConfig.setName;
            ctx.readOnly = readOnly;
            ctx.dbUsedSlotsFile = dbUsedSlotsFile;

            // init sections class
            ctx.dbSections = new OpenFSDBSections(ctx.dbBaseParentPath, ctx.dbBasePath, ctx.dbEntryPath, ctx.dbEntryConfigPath);
            // auto load or create sections
            OpenFSDBSections.loadReloadSections(ctx);

            // scan used slots config
            ctx.readUsedSlotsFile();

            // finally create register and root node
            DBCreateRegister(ctx, false);

            // detect db's entry file type
            String? rtf = "";
            byte[]? xamlbytes = null;
            ctx.dbEntryType = EntryType.Rtf;
            ctx.isXamlDB = false;
            myNode? found = entryMethods.DBSearchNodeOFSDB(ctx, 0, ref rtf, ref xamlbytes, true);
            if (found == null) // not rtf db type so try with xaml
            {
                ctx.dbEntryType = EntryType.Xaml;
                ctx.isXamlDB = true;
                found = entryMethods.DBSearchNodeOFSDB(ctx, 0, ref rtf, ref xamlbytes, true);
                if (found == null) // not xaml db type so return with error
                    return false;

                // xaml db
            }

            // finally open mmf of registry file
            if (!ctx.openRegistryMMF())
                return false; // critical error

            return true;
        }

        // this method copies the db 
        public static bool CopyDB(OpenFSDBContext? src, String destBasePath, bool convert, bool rtfToRaw, bool rtfToXaml, FormOperation? formop = null)
        {
            if (destBasePath == "") return false;

            // first get the total number of chapters which exist in db
            UInt32 total = Register.Total(src);
            UInt32 index = 0;

            // first replicate initial db and config
            OpenFSDBContext? ctx = PrepareInitialCloneDB(src, destBasePath);
            if (ctx == null) return false;

            // load register into memory
            Register register = new Register();
            if (!register.loadRegisterCopyToMemory(ctx.dbNodeTreeRegistryFile)) return false;

            // convert and copy all entries
            for (UInt32 id = 0; id < Register.default_totalPreallocatedNodes; id++)
            {
                RegisterItem? thisItem = null;
                Int64 offset = Register.FindNode(register.registerStream, id, ref thisItem);
                if (thisItem == null) continue;
                if (offset < 0) continue;
                if (thisItem.domainType == DomainType.EmptySlot) continue;

                if (formop != null)
                {
                    formop.updateProgressBar(index++, total);
                    formop.updateFilesStatus(id, total);
                }

                // load node
                String? rtf = "";
                byte[] xamlbytes = null;
                if (!thisItem.loadNode(src, ref rtf, ref xamlbytes, true)) continue;

                if (src.isXamlDB)
                    xamlEntry.dummy.XamlBytes = xamlbytes;
                else
                    xamlEntry.dummy.Rtf = rtf;

                // convert
                EntryType entryType = EntryType.Default;
                if (convert)
                {
                    if (rtfToXaml)
                        entryType = EntryType.Xaml;
                    else
                        entryType = EntryType.Rtf;
                }

                if (rtfToRaw)
                    xamlEntry.dummy.Text = xamlEntry.dummy.Text;

                // finally write to destination
                entryMethods.DBUpdateNodeOFSDB(ctx, thisItem.node, xamlEntry.dummy.Rtf, xamlEntry.dummy.XamlBytes,
                    true, false, false, entryType);
            }
            return true;
        }




        //**********************************************************************
        //********************************************************************** 
        //********************************************************************** 
        //**** Shell ***********************************************************
        //********************************************************************** 
        //**********************************************************************



        //**********************************************************************
        //********************************************************************** 
        //********************************************************************** 
        //**** Shell ***********************************************************
        //********************************************************************** 
        //**********************************************************************


        //**********************************************************************
        //********************************************************************** 
        //********************************************************************** 
        //**** Core Framework **************************************************
        //********************************************************************** 
        //**********************************************************************


        //**********************************************************************
        //********************************************************************** 
        //********************************************************************** 
        //**** Core Framework **************************************************
        //********************************************************************** 
        //**********************************************************************


        //**********************************************************************
        //********************************************************************** 
        //********************************************************************** 
        //**** Core ************************************************************
        //********************************************************************** 
        //**********************************************************************
        
        // write appropirate db config file in vhd file or in local windows file system.
        public static String? writeDBConfig(OpenFSDBContext? ctx)
        {
            String? result = "";
            if (ctx.readOnly) return "";
            // local windows file system
            result = DatabaseConfig.toYamlFile(ctx.dbConfig, ctx.dbConfigFile);
            return result;
        }

        // get list of all files in section
        public static bool findSectionFiles(OpenFSDBContext ctx, UInt32 id, bool returnEntryConfigFiles,
            ref String[]? entryFilesOut, ref String[]? entryConfigFilesOut)
        {
            String EntrySectionPath = "";
            String EntryConfigSectionPath = "";
            OpenFSDBSections.getFormattedSectionPaths(ctx, id, ref EntrySectionPath, ref EntryConfigSectionPath);

            // local windows file system
            if (!Directory.Exists(EntryConfigSectionPath))
                return false;

            String EntryExt = "";
            String EntryExtComplete = "";
            String EntryExtSearchPattern = "";
            entryMethods.getEntryTypeFormats(ctx.dbEntryType, ref EntryExt, ref EntryExtComplete, ref EntryExtSearchPattern);

            String EntryCfgExt = "";
            String EntryCfgExtComplete = "";
            String EntryCfgExtSearchPattern = "";
            entryMethods.getEntryTypeFormats(ctx.dbEntryConfigType, ref EntryCfgExt, ref EntryCfgExtComplete, ref EntryCfgExtSearchPattern);

            // local windows file system
            // now find all valid files
            EnumerationOptions options = new EnumerationOptions();
            options.RecurseSubdirectories = false;
            entryFilesOut = Directory.GetFiles(EntrySectionPath, EntryExtSearchPattern, options);

            if (returnEntryConfigFiles)
                entryConfigFilesOut = Directory.GetFiles(EntryConfigSectionPath, EntryCfgExtSearchPattern, options);

            return true;
        }

        public static bool findSectionNodes(OpenFSDBContext ctx, UInt32 id, ref List<myNode> nodes)
        {
            String[]? entryFiles = null;
            String[]? entryConfigFiles = null;
            if (!findSectionFiles(ctx, id, false, ref entryFiles, ref entryConfigFiles))
                return false;

            foreach (String file in entryFiles)
            {
                myNode node = new myNode(true);
                if (!entryMethods.convertEntryFilenameToNode(ref node, file))
                    continue; // some invalid file

                // set section id in the new node
                node.DirectorySectionID = id;

                // load whole entry config into node
                if (!loadNodeConfig(ctx, id, node.chapter.Id, ref node))
                    continue;

                nodes.Add(node);
            }
            return true;
        }

        // loads a node from a path and file string.
        public static myNode? pathToNode(OpenFSDBContext ctx, String file, ref String? rtf, byte[]? xamlbytes, bool loadData = false)
        {
            myNode node = new myNode();
            if (!entryMethods.convertEntryFilenameToNode(ref node, file))
                return null; // some invalid file, skip

            UInt32 sectionId = 0;
            if (!getSectionIdByPathFile(file, ref sectionId))
                return null;

            // set section id in the new node
            node.DirectorySectionID = sectionId;

            // load whole entry config into node
            if (!loadNodeConfig(ctx, sectionId, node.chapter.Id, ref node))
                return null;

            // load entry data if user asks

            if (loadData)
            {
                if (ctx.dbEntryType == EntryType.Xaml)
                    xamlbytes = (byte[]?)loadNodeData(ctx, sectionId, node.chapter.Id);
                else
                    rtf = (String?)loadNodeData(ctx, sectionId, node.chapter.Id);
            }
            return node;
        }
        // this method retrieves section id from path and file
        public static bool getSectionIdByPathFile(String file, ref UInt32 SectionIdOut)
        {
            if (file.Length <= 0)
                return false;

            FileInfo fileInfo = new FileInfo(file);
            DirectoryInfo directoryInfo = fileInfo.Directory;
            String dirName = directoryInfo.Name;
            UInt32 sectionId = 0;
            if (UInt32.TryParse(dirName, out sectionId))
            {
                SectionIdOut = sectionId;
                return true;
            }
            return false;
        }
        // this method searches for the section of the node by node's id
        public static Int64 getSectionByNodeId(OpenFSDBContext ctx, UInt32 nodeId, ref OpenFSDBSection? sectionOut)
        {
            String entryNameOut = "", entryCfgNameOut = "";
            String entryFile = "", entryConfigFile = "";

            foreach (OpenFSDBSection section in ctx.dbSections.sections)
            {
                // get formatted open file system db node file names
                generateNodeFileNames(ctx, section.sectionId, nodeId, out entryNameOut, out entryCfgNameOut, out entryFile, out entryConfigFile);

                // local windows file system
                if (File.Exists(entryFile))
                {
                    sectionOut = section;
                    return section.sectionId;
                }
            }
            // no valid section found
            return -1;
        }

        // directly selects a node and it's files by it's guid without finding the node in the db's path
        public static myNode? selectNode(OpenFSDBContext ctx, UInt32 sectionId, UInt32 id, ref String? rtf, ref byte[]? xamlbytes, bool loadData = false)
        {
            myNode node = new myNode();

            // configure
            node.DirectorySectionID = sectionId;

            // load whole entry config into node
            if (!loadNodeConfig(ctx, sectionId, id, ref node))
                return null;

            // load entry data if user asks
            if (loadData)
            {
                Object? data = null;
                data = loadNodeData(ctx, sectionId, id);
                if (data == null) return null; // error entry file does not exists

                if (ctx.dbEntryType == EntryType.Xaml)
                    xamlbytes = (byte[]?)data;
                else
                    rtf = (String?)data;
            }
            // return node
            return node;
        }
        // finds and loads a node
        public static myNode? findLoadNode(OpenFSDBContext ctx, UInt32 sectionId, UInt32 id, ref String rtf, ref byte[]? xamlbytes, bool loadData = false)
        {
            return selectNode(ctx, sectionId, id, ref rtf, ref xamlbytes, loadData);
        }
        // changes node's parent physically in entry files
        public static bool changeNodeParent(OpenFSDBContext ctx, UInt32 sectionId, UInt32 id, UInt32 newParentId)
        {
            if (newParentId < 0) return false;
            String? rtf = "";
            byte[]? xaml = null;
            myNode? node = findLoadNode(ctx, sectionId, id, ref rtf, ref xaml, false);
            if (node == null) return false;
            if (node.chapter == null) return false;
            node.chapter.parentId = newParentId;
            return updateNode(ctx, node, "", null, false, false, false, EntryType.Default);
        }

        // this explores the entry file of a node
        public static void exploreNodeEntryFile(OpenFSDBContext? ctx, UInt32 sectionId, UInt32 id)
        {

            // get formatted open file system db node file names
            String entryNameOut = "", entryCfgNameOut = "";
            String entryFile = "", entryConfigFile = "";
            generateNodeFileNames(ctx, sectionId, id, out entryNameOut, out entryCfgNameOut, out entryFile, out entryConfigFile);

            myCommonMethods1.OpenFolderSelectFile(entryFile);
        }

        // load the entry data through entry's node
        public static Object? loadNodeData(OpenFSDBContext ctx, UInt32 sectionId, UInt32 id)
        {
            // get formatted open file system db node file names
            String entryNameOut = "", entryCfgNameOut = "";
            String entryFile = "", entryConfigFile = "";
            generateNodeFileNames(ctx, sectionId, id, out entryNameOut, out entryCfgNameOut, out entryFile, out entryConfigFile);

            // load node data
            // local windows file system
            if (!File.Exists(entryFile)) return null;

            if (ctx.dbEntryType == EntryType.Xaml)
                return File.ReadAllBytes(entryFile);
            else
                return File.ReadAllText(entryFile);
        }

        // load the entry config through entry's node 
        public static bool loadNodeConfig(OpenFSDBContext ctx, UInt32 sectionId, UInt32 id, ref myNode node)
        {
            // get formatted open file system db node file names
            String entryNameOut = "", entryCfgNameOut = "";
            String entryFile = "", entryConfigFile = "";
            generateNodeFileNames(ctx, sectionId, id, out entryNameOut, out entryCfgNameOut, out entryFile, out entryConfigFile);

            // check if file exists, if does not then return error
            // local windows file system
            if (!File.Exists(entryConfigFile)) return false;

            // file exists, so proceed

            // check if the node contains no chapter, then create a new chapter for loading the config into it.
            if (node.chapter == null)
                node.chapter = new Chapter();

            // load the config into the node's chapter.
            bool result = false;
            // local windows file system
            result = cfgEntry.fromCfg(ref node.chapter, entryConfigFile);
            return result;
        }

        // this method generates factory default formatted node file names and strings
        public static void generateNodeFileNames(OpenFSDBContext ctx, UInt32 nodeSectionId, UInt32 id,
            out String entryNameOut, out String entryCfgNameOut, out String entryFileOut, out String entryConfigFileOut,
            EntryType entryType = EntryType.Default)
        {
            String entrySectionPath = Path.Combine(ctx.dbEntryPath, nodeSectionId.ToString());
            String entryConfigSectionPath = Path.Combine(ctx.dbEntryConfigPath, nodeSectionId.ToString());

            if (entryType == EntryType.Default) entryType = ctx.dbEntryType;
            entryFileOut = entryMethods.getFormattedPathFileName(entrySectionPath, id, 0,
                "", default(DateTime), 0, entryType, out entryNameOut);
            entryConfigFileOut = entryMethods.getFormattedPathFileName(entryConfigSectionPath, id,
                0, "", default(DateTime), 0, EntryType.Cfg, out entryCfgNameOut);
        }

        // this method generates factory default formatted node file names and strings
        public static void generateNodeFileNames(OpenFSDBContext ctx, UInt32 nodeSectionId, myNode node,
            out String entryNameOut, out String entryCfgNameOut, out String entryFileOut, out String entryConfigFileOut,
            EntryType entryType = EntryType.Default)
        {
            String entrySectionPath = Path.Combine(ctx.dbEntryPath, nodeSectionId.ToString());
            String entryConfigSectionPath = Path.Combine(ctx.dbEntryConfigPath, nodeSectionId.ToString());

            if (entryType == EntryType.Default) entryType = ctx.dbEntryType;
            entryFileOut = entryMethods.getFormattedPathFileName(entrySectionPath, node.chapter.Id, 0,
                "", default(DateTime), 0, entryType, out entryNameOut);
            entryConfigFileOut = entryMethods.getFormattedPathFileName(entryConfigSectionPath, node.chapter.Id,
                0, "", default(DateTime), 0, EntryType.Cfg, out entryCfgNameOut);
        }

        // create a new node. guid used cannot be reused.
        public static bool createNode(OpenFSDBContext? ctx, myNode? node, String? rtf, byte[]? xamlbytes,
            bool resetCD, bool resetMD, bool resetDD, bool root)

        {
            if (ctx.readOnly) return false;
            if (node == null) return false;

            // prepare

            // create root if requird
            if (root) node.chapter.Id = 0;

            DateTime latestDateTime = DateTime.Now;

            // creation date is always set to latest when a new node of any kind is created unless db is cloned.
            if (resetCD)
            {
                node.chapter.creationDateTime = latestDateTime;
            }

            // this is a new node, so setup dates
            if (resetMD)
            {
                node.chapter.modificationDateTime = latestDateTime;
            }

            // reset dd if required
            if (resetDD)
            {
                node.chapter.deletionDateTime = default(DateTime);
            }

            // create entry's config string
            String entryConfigString = cfgEntry.toCfg(ref node.chapter);

            // first auto load or create an available section
            OpenFSDBSection? section = OpenFSDBSections.autoFindCreateAvailableSection(ctx);
            if (section == null)
                return false;

            // setup the section config in node
            node.DirectorySectionID = section.sectionId;

            // increment section's total slots to reserve this slot.
            section.totalNodes++;

            // get formatted open file system db node file names
            String entryNameOut = "", entryCfgNameOut = "";
            String entryFile = "", entryConfigFile = "";
            generateNodeFileNames(ctx, section.sectionId, node, out entryNameOut, out entryCfgNameOut, out entryFile, out entryConfigFile);

            // now write files
            if (xamlbytes == null) xamlbytes = Array.Empty<byte>();
            if (rtf == null) rtf = "";
            // local windows file system
            // write config file
            File.WriteAllText(entryConfigFile, entryConfigString);
            // write entry file. entry file is simply the richtext format .rtf file
            if (ctx.dbEntryType == EntryType.Xaml)
                File.WriteAllBytes(entryFile, xamlbytes);
            else
                File.WriteAllText(entryFile, rtf);

            return true;
        }

        // allocate write new backup entry file in backup dir
        public static bool autoCreateBackupEntryFile(OpenFSDBContext ctx, String entryPath, String entryConfigPath, DateTime dateTime = default(DateTime))
        {
            if (entryPath == "") return false;
            if (entryConfigPath == "") return false;
            if (ctx.readOnly) return false;

            // copy entry
            FileInfo info = new FileInfo(entryPath);
            String filename = info.Name;
            filename = CoreFramework.getFormattedEntryFileName(filename, dateTime);
            String finalPath = Path.Combine(ctx.dbBackupPath, filename);

            // local windows file system
            File.Copy(entryPath, finalPath, true);

            // copy entry config
            info = new FileInfo(entryConfigPath);
            filename = info.Name;
            filename = CoreFramework.getFormattedEntryFileName(filename, dateTime);
            finalPath = Path.Combine(ctx.dbBackupPath, filename);

            // local windows file system
            File.Copy(entryConfigPath, finalPath, true);
            return true;
        }
        // auto clear paths
        public static bool autoClearLocalPaths(OpenFSDBContext? ctx, bool tmppath, bool backuppath)
        {
            if (ctx.readOnly) return false;

            if (backuppath)
            {
                // local windows file system
                Directory.Delete(ctx.dbBackupPath, true);
                Directory.CreateDirectory(ctx.dbBackupPath);
            }
            if (tmppath)
            {
                // local windows file system
                Directory.Delete(ctx.dbTmpPath, true);
                Directory.CreateDirectory(ctx.dbTmpPath);
            }

            // done
            return true;
        }

        // this method auto backs up the entire node into backup location with latest revision timings
        public static bool NodeBackup(OpenFSDBContext? ctx, myNode node)
        {
            if (ctx.readOnly) return false;

            if (node == null)
                return false;

            // get formatted open file system db node file names
            String entryNameOut = "", entryCfgNameOut = "";
            String entryFile = "", entryConfigFile = "";
            generateNodeFileNames(ctx, node.DirectorySectionID, node, out entryNameOut, out entryCfgNameOut, out entryFile, out entryConfigFile);

            // first create new files and write to them
            String entryCfgFileDir = Path.GetDirectoryName(entryConfigFile);
            String entryFileDir = Path.GetDirectoryName(entryFile);

            // first make backup file if required
            autoCreateBackupEntryFile(ctx, entryFile, entryConfigFile, DateTime.Now);
            return true;
        }
        // this method purges the old unusable files and replaces them with new update, and updates the node accordingly. 
        public static bool updateNode(OpenFSDBContext? ctx, myNode node, String? rtf = "", byte[]? xamlbytes = null,
            bool storeData = false, bool updateModificationDate = true, bool backupOldEntryFirst = false,
            EntryType exclusiveDestEntryType = EntryType.Default)
        {
            if (ctx.readOnly) return false;

            if (node == null)
                return false;

            // prepare
            // update dates
            if (updateModificationDate)
                node.chapter.modificationDateTime = DateTime.Now;

            // create entry's config xml string
            String entryConfigString = cfgEntry.toCfg(ref node.chapter);

            // get formatted open file system db node file names
            String entryNameOut = "", entryCfgNameOut = "";
            String entryFile = "", entryConfigFile = "";
            generateNodeFileNames(ctx, node.DirectorySectionID, node, out entryNameOut, out entryCfgNameOut, out entryFile, out entryConfigFile, exclusiveDestEntryType);

            // we implement an ensuring procedure that original file is not directly deleted.
            // we first create a new file and write data to it, then if it is successfully written,
            // we directly replace the old file.
            // this is to prevent original file loss in case of disk full error

            // first create new files and write to them
            //String entryCfgFileDir = Path.GetDirectoryName(entryConfigFile);
            //String entryFileDir = Path.GetDirectoryName(entryFile);
            //String entryCfgFileTmp = Path.Combine(entryCfgFileDir, System.Guid.NewGuid().ToString());
            //String entryFileTmp = Path.Combine(entryFileDir, System.Guid.NewGuid().ToString());

            // first make backup file if required
            if (backupOldEntryFirst)
                NodeBackup(ctx, node);

            // write entry config file
            File.WriteAllText(entryConfigFile, entryConfigString);

            // entry data file
            if (xamlbytes == null) xamlbytes = Array.Empty<byte>();
            if (rtf == null) rtf = "";
            if (storeData)
            {
                // local windows file system
                // write data
                if (exclusiveDestEntryType == EntryType.Default)
                {
                    // current db's original file type save
                    if (ctx.dbEntryType == EntryType.Xaml)
                        File.WriteAllBytes(entryFile, xamlbytes);
                    else
                        File.WriteAllText(entryFile, rtf);
                }
                else
                {
                    // exclusive customized destination rtf file or xaml file write as configured in exclusiveDestEntryType
                    if (exclusiveDestEntryType == EntryType.Xaml)
                        File.WriteAllBytes(entryFile, xamlbytes);
                    else
                        File.WriteAllText(entryFile, rtf);
                }
                //File.Move(entryFileTmp, entryFile, true);
            }

            return true;
        }

        // erases and purges the node's files
        public static bool purgeNode(OpenFSDBContext? ctx, UInt32 id, UInt32 sectionId, bool purgeConfig = true, bool purgeData = true)
        {
            if (ctx.readOnly) return false;

            // get formatted open file system db node file names
            String entryNameOut = "", entryCfgNameOut = "";
            String entryFile = "", entryConfigFile = "";
            generateNodeFileNames(ctx, sectionId, id, out entryNameOut, out entryCfgNameOut, out entryFile, out entryConfigFile);

            // local windows file system
            // purge
            if (purgeConfig)
                File.Delete(entryConfigFile);

            if (purgeData)
                File.Delete(entryFile);

            // vacate one node's slot in this purged node's section
            if (purgeData && purgeConfig)
            {
                // first find and load the section of this purged node
                OpenFSDBSection? section = OpenFSDBSections.findSection(ctx, sectionId);
                if (section != null)
                {
                    if (section.totalNodes > 0)
                        section.totalNodes--;
                }
            }

            return true;
        }
        public static myNode? newNode(OpenFSDBContext? ctx,
            SpecialNodeType specialNodeType, NodeType nodeType, DomainType domainType, ref myNode? initialNode,
            bool resetCD, bool resetMD, bool resetDD,
            DateTime nodeDateTime = default(DateTime), UInt32 parentId = 0, bool DBImport = true,
            String title = "", String? rtf = "", byte[]? xamlbytes = null, bool newId = true)
        {
            if (ctx.readOnly) return null;

            // prepare and configure
            myNode? node = initialNode;
            if (node == null)
                node = new myNode(true);

            node.chapter.parentId = parentId;
            node.chapter.chapterDateTime = nodeDateTime;
            node.chapter.nodeType = nodeType;
            node.chapter.specialNodeType = specialNodeType;
            node.chapter.domainType = domainType;
            node.chapter.Title = title;

            // now when all setup is done, import the entry and it's data object into the db if required
            if (DBImport)
            {
                if (!createNode(ctx, node, rtf, xamlbytes, resetCD, resetMD, resetDD, newId))
                    return null;
            }
            return node;
        }

        //**********************************************************************
        //********************************************************************** 
        //********************************************************************** 
        //**** Core ************************************************************
        //********************************************************************** 
        //**********************************************************************

    }
}

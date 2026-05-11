using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TheBook.Net.Core;
using DiaryJournal.Net;
using System.Windows.Documents;
using System.ComponentModel.DataAnnotations;

namespace DiaryJournal.Net
{
    public class OpenFSDBSection
    {
        public UInt32 sectionId = 0;
        public UInt32 totalNodes = 0;


    }

    public class OpenFSDBSections
    {
        public UInt32 totalSections = 0;
        public List<OpenFSDBSection> sections = new List<OpenFSDBSection>();
        public String dbBaseParentPath = "";
        public String dbBasePath = "";
        public String dbEntryPath = "";
        public String dbEntryConfigPath = "";

        public const UInt32 maxNodesInSection = 50000; // 50,000 * 200 sections = 10000000 = 1 Crore files in 200 sections so this configuration is optimized
                                                     // as no bug stack overflow errors will occur. 
                                                     // entry and entry config path strings and myNodes per section can use 16 or more gb ram
                                                     // max 10,000 nodes per section ///1000; 

        public OpenFSDBSections()
        {

        }

        public OpenFSDBSections(String dbBaseParentPath, String dbBasePath, String dbEntryPath, String dbEntryConfigPath)
        {
            this.dbBaseParentPath = dbBaseParentPath;
            this.dbBasePath = dbBasePath;
            this.dbEntryPath = dbEntryPath;
            this.dbEntryConfigPath = dbEntryConfigPath;
        }

        // find a section
        public static OpenFSDBSection? findSection(OpenFSDBContext ctx, UInt32 id)
        {
            foreach (OpenFSDBSection section in ctx.dbSections.sections)
                if (section.sectionId == id)
                    return section;

            return null;
        }

        // this method auto finds-generates a new available section id which requires no further incrementation
        public static UInt32 findNewSectionId(OpenFSDBContext ctx)
        {
            if (ctx.dbSections.sections.Count > 0)
                return (ctx.dbSections.sections.Max(s => s.sectionId) + 1);
            else
                return 1;
        }
        // get auto formatted paths for section in both entry and entry config directories
        public static void getFormattedSectionPaths(OpenFSDBContext? ctx, UInt32 id, ref String EntrySectionOut, ref String EntryConfigSectionOut)
        {
            String EntrySectionPath = Path.Combine(ctx.dbSections.dbEntryPath, id.ToString());
            String EntryConfigSectionPath = Path.Combine(ctx.dbSections.dbEntryConfigPath, id.ToString());
            EntrySectionOut = EntrySectionPath;
            EntryConfigSectionOut = EntryConfigSectionPath;
        }
        // create sections by indexing
        public static bool createSections(OpenFSDBContext? ctx, UInt32 startingid, UInt32 total)
        {
            UInt32 id = startingid;
            for (UInt32 i = 0; i < total; i++)
                createSection(ctx, id++);
            
            return true;
        }

        // this method creates a new section with one step ahead incremented id
        public static OpenFSDBSection? createSection(OpenFSDBContext? ctx, UInt32 id)
        {
            String EntrySectionPath = "";
            String EntryConfigSectionPath = "";
            getFormattedSectionPaths(ctx, id, ref EntrySectionPath, ref EntryConfigSectionPath);

            // this section already exists, so we must return error and not create a new one
            // so that the user defines a new nonexistent section with new available id
            // normal local windows directory path
            if (Directory.Exists(EntryConfigSectionPath))
                return null;

            // section does not exists, so create it
            Directory.CreateDirectory(EntrySectionPath);
            Directory.CreateDirectory(EntryConfigSectionPath);

            // return new section
            OpenFSDBSection section = new OpenFSDBSection();
            section.sectionId = id;
            ctx.dbSections.sections.Add(section);
            ctx.dbSections.totalSections = (UInt32)ctx.dbSections.sections.Count;
            return section;
        }
        // this method finds the first section in list having an available slot for a node
        public static OpenFSDBSection? findFirstSectionWithFreeSlot(OpenFSDBContext ctx)
        {
            foreach (OpenFSDBSection section in ctx.dbSections.sections)
            {
                if (section.totalNodes < maxNodesInSection)
                    return section;
            }
            return null;
        }
        // this method auto finds an available section, otherwise auto creates a new section in list and returns it
        public static OpenFSDBSection? autoFindCreateAvailableSection(OpenFSDBContext ctx)
        {
            // find and return if we have an available section
            OpenFSDBSection? section = findFirstSectionWithFreeSlot(ctx);
            if (section != null)
                return section;

            // no available section, so create a new one with a new available id
            return createSection(ctx, findNewSectionId(ctx));
        }
        // this method returns latest total number of entries in all sections
        public static UInt32 getTotalEntriesAllSections(OpenFSDBContext ctx)
        {
            // reload latest state
            loadReloadSections(ctx);

            UInt32 entries = 0;
            foreach (OpenFSDBSection section in ctx.dbSections.sections)
                entries += section.totalNodes;
        
            return entries;
        }
        // this method loads all sections into list until no more section is found
        public static bool loadReloadSections(OpenFSDBContext ctx)
        {
            ctx.dbSections.sections.Clear();
            UInt32 id = 1; // 1 is valid section id and 0 is invalid
            foreach (String dir in Directory.EnumerateDirectories(ctx.dbEntryConfigPath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);

                if (!UInt32.TryParse(dirInfo.Name, out id)) continue;

                OpenFSDBSection section = new OpenFSDBSection();
                if (!loadSection(ctx, id, ref section))
                    break;

                // section found, add it
                ctx.dbSections.sections.Add(section);

                //id++;
            }
            if (ctx.dbSections.sections.Count == 0)
            {
                // no sections found, so create the initial first one with index 1
                autoFindCreateAvailableSection(ctx);
            }
            return true;
        }

        // this method loads the section and validates it
        public static bool loadSection(OpenFSDBContext ctx, UInt32 id, ref OpenFSDBSection section)
        {
            String EntrySectionPath = "";
            String EntryConfigSectionPath = "";
            getFormattedSectionPaths(ctx, id, ref EntrySectionPath, ref EntryConfigSectionPath);
            
            // normal local windows directory path
            if (!Directory.Exists(EntryConfigSectionPath))
                return false;

            section.sectionId = id;
            refreshSection(ctx, section);
            return true;
        }

        // this method refreshes the section with new latest config assuming the section already exists
        public static bool refreshSection(OpenFSDBContext ctx, OpenFSDBSection section)
        {
            String EntrySectionPath = "";
            String EntryConfigSectionPath = "";
            getFormattedSectionPaths(ctx, section.sectionId, ref EntrySectionPath, ref EntryConfigSectionPath);

            // normal local windows directory path
            DirectoryInfo directoryInfo = new DirectoryInfo(EntryConfigSectionPath);
            UInt32 totalNodes = (UInt32)directoryInfo.GetFiles().Count();
            section.totalNodes = totalNodes;
            return true;
        }
    }
}

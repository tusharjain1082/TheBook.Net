using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Xml.Xsl;
using System.Xml;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;
using System.Runtime.InteropServices;
using TheBook.Net.Core;

namespace DiaryJournal.Net

{
    public static class journalSearchFramework
    {
        public static void __insertLvSearchItem(OpenFSDBContext ctx, System.Windows.Forms.ListView lv, RegisterItem found, String entryPath, long totalMatches)
        {
            System.Windows.Forms.ListViewItem item = new System.Windows.Forms.ListViewItem();
            item.Name = found.node.chapter.Id.ToString();
            item.Text = totalMatches.ToString();

            item.SubItems.Add(entryPath);

            // dates
            String chapterDateTime = found.node.chapter.chapterDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            item.SubItems.Add(chapterDateTime);
            item.SubItems.Add(found.node.chapter.creationDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
            item.SubItems.Add(found.node.chapter.modificationDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
            item.SubItems.Add(found.node.chapter.deletionDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss"));

            // deleted status
            if (found.node.chapter.IsDeleted)
                item.SubItems.Add("trash can");
            else
                item.SubItems.Add("common");

            // special node type
            item.SubItems.Add(found.node.chapter.specialNodeType.ToString());

            // node type
            item.SubItems.Add(found.node.chapter.nodeType.ToString());

            // node's parent id
            item.SubItems.Add(found.node.chapter.parentId.ToString());

            // node's id
            item.SubItems.Add(found.node.chapter.Id.ToString());

            // other details
            item.SubItems.Add(found.node.chapter.Title);
            lv.Items.Add(item);
        }

        public static bool searchEntries(OpenFSDBContext ctx, 
            FormJournalDesign2 form, System.Windows.Forms.TextBox txtSearchProgressPath, System.Windows.Forms.ListView lv, ToolStripProgressBar tsProgressBar,
            DateTime inputFrom, DateTime inputFromTime, DateTime inputThrough, DateTime inputThroughTime, bool useDateTimeRange,
            DateTime inputCDFrom, DateTime inputCDFromTime, DateTime inputCDThrough, DateTime inputCDThroughTime, bool useCreationDateTimeRange,
            DateTime inputMDFrom, DateTime inputMDFromTime, DateTime inputMDThrough, DateTime inputMDThroughTime, bool useModificationDateTimeRange,
            DateTime inputDDFrom, DateTime inputDDFromTime, DateTime inputDDThrough, DateTime inputDDThroughTime, bool useDeletionDateTimeRange,
            String searchPattern, String replacement, bool searchAll,
            bool searchTrash, bool matchCase, bool matchWholeWord,
            bool replace, bool searchReplaceTitle, bool searchEmptyString, List<Int64> locations)
        {
            // prepare regex
            RegexOptions regexOptions = new RegexOptions();
            if (!matchCase)
                regexOptions |= RegexOptions.IgnoreCase;

            regexOptions |= RegexOptions.Singleline;

            // prepare to match whole word if user requires. but we can use it through manual pattern as well.
            if (matchWholeWord)
            {
                String flattened = Regex.Escape(searchPattern);
                searchPattern = @"\b" + flattened + @"";//[^.*\r*\n*]";
            }

            // prepare entry date and time range
            DateTime from = DateTime.MinValue;
            DateTime through = DateTime.MaxValue;

            // use user's date and time range if required
            if (useDateTimeRange)
            {
                // choose user's given date time range if available
                if (inputFrom != default(DateTime))
                {
                    from = new DateTime(inputFrom.Year, inputFrom.Month, inputFrom.Day, inputFromTime.Hour, inputFromTime.Minute,
                        inputFromTime.Second, 0);
                    //fromTime = new DateTime(inputFrom.Year, inputFrom.Month, inputFrom.Day, inputFromTime.Hour, inputFromTime.Minute,
                    //  inputFromTime.Second, 0);
                }
                // choose user's given date time range if available
                if (inputThrough != default(DateTime))
                {
                    through = new DateTime(inputThrough.Year, inputThrough.Month, inputThrough.Day, inputThroughTime.Hour,
                        inputThroughTime.Minute, inputThroughTime.Second, 0);
                    //throughTime = new DateTime(inputThrough.Year, inputThrough.Month, inputThrough.Day, inputThroughTime.Hour,
                    //    inputThroughTime.Minute, inputThroughTime.Second, 0);
                }
                int result0 = DateTime.Compare(from, through);
                if (result0 > 0)
                    return false; // error, invalid date time input

            }

            // prepare entry creation date and time range
            DateTime CDfrom = DateTime.MinValue;
            DateTime CDthrough = DateTime.MaxValue;

            // use user's date and time range if required
            if (useCreationDateTimeRange)
            {
                // choose user's given creation date time range if available
                if (inputCDFrom != default(DateTime))
                {
                    CDfrom = new DateTime(inputCDFrom.Year, inputCDFrom.Month, inputCDFrom.Day, inputCDFromTime.Hour, inputCDFromTime.Minute,
                        inputCDFromTime.Second, 0);
                }
                // choose user's given creation date time range if available
                if (inputCDThrough != default(DateTime))
                {
                    CDthrough = new DateTime(inputCDThrough.Year, inputCDThrough.Month, inputCDThrough.Day, inputCDThroughTime.Hour,
                        inputCDThroughTime.Minute, inputCDThroughTime.Second, 0);
                }
                int result0 = DateTime.Compare(CDfrom, CDthrough);
                if (result0 > 0)
                    return false; // error, invalid date time input

            }

            // prepare entry modification date and time range
            DateTime MDfrom = DateTime.MinValue;
            DateTime MDthrough = DateTime.MaxValue;

            // use user's date and time range if required
            if (useModificationDateTimeRange)
            {
                // choose user's given modification date time range if available
                if (inputMDFrom != default(DateTime))
                {
                    MDfrom = new DateTime(inputMDFrom.Year, inputMDFrom.Month, inputMDFrom.Day, inputMDFromTime.Hour, inputMDFromTime.Minute,
                        inputMDFromTime.Second, 0);
                }
                // choose user's given modification date time range if available
                if (inputMDThrough != default(DateTime))
                {
                    MDthrough = new DateTime(inputMDThrough.Year, inputMDThrough.Month, inputMDThrough.Day, inputMDThroughTime.Hour,
                        inputMDThroughTime.Minute, inputMDThroughTime.Second, 0);
                }
                int result0 = DateTime.Compare(MDfrom, MDthrough);
                if (result0 > 0)
                    return false; // error, invalid date time input

            }

            // prepare entry deletion date and time range
            DateTime DDfrom = DateTime.MinValue;
            DateTime DDthrough = DateTime.MaxValue;

            // use user's date and time range if required
            if (useDeletionDateTimeRange)
            {
                // choose user's given deletion date time range if available
                if (inputDDFrom != default(DateTime))
                {
                    DDfrom = new DateTime(inputDDFrom.Year, inputDDFrom.Month, inputDDFrom.Day, inputDDFromTime.Hour, inputDDFromTime.Minute,
                        inputDDFromTime.Second, 0);
                }
                // choose user's given deletion date time range if available
                if (inputDDThrough != default(DateTime))
                {
                    DDthrough = new DateTime(inputDDThrough.Year, inputDDThrough.Month, inputDDThrough.Day, inputDDThroughTime.Hour,
                        inputDDThroughTime.Minute, inputDDThroughTime.Second, 0);
                }
                int result0 = DateTime.Compare(DDfrom, DDthrough);
                if (result0 > 0)
                    return false; // error, invalid date time input

            }

            // successfully prepared all config
            // verify if the pattern is valid.
            if (!searchEmptyString)
            {
                if (!myCommonMethods1.IsValidRegex(searchPattern))
                    return false;
            }

            WpfRichTextBoxEx rtb = new WpfRichTextBoxEx();
            //System.Windows.Forms.RichTextBox rtbnative = new System.Windows.Forms.RichTextBox();

            // select search location if user demands it
            List<Int64>? worklist = locations;
            Int64 total = 0;
            Int64 done = 0;

            if (locations.Count == 0) 
            {
                // no location provided, so load root's children as worklist and traverse each tree
                RegisterItem? root = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, 0, false, false, true, false, true, false);
                if (root == null) return false; // error abort
                foreach (RegisterItem? item in root.childrenList)
                    worklist.Add(item.Id);

                // prepare counter and progress
                total = Register.CountValidEntries(ctx, ctx.dbNodeTreeRegistryFile);
                total -= 1; // negate root node
            }
            else
            {
                // prepare counter and progress
                foreach (Int64 id in worklist)
                {
                    RegisterItem? thisItem = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, id, false, false, false, false, false, true);
                    if (thisItem == null) continue;
                    total += thisItem.tree.CountDescendants(thisItem);
                    total += 1; // increment this current item
                }
            }

            // initialize a single regex for all operations
            // now load regex with pattern and options
            Regex regex = new Regex(searchPattern, regexOptions);
            String entryPath = "";

            // phase 1 - first iterate through all locations list

            foreach (Int64 thisItemId in worklist)//RegisterItem item in worklist)
            {
                RegisterItem? thisItem = null;
                String rtf = "";

                if (searchEmptyString)
                    thisItem = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, thisItemId, true, false, false, false, false, false);
                else
                    thisItem = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, thisItemId, true, true, false, false, false, false);

                if (thisItem == null) continue;
                
                myNode? node = thisItem.node;

                done++;
                tsProgressBar.Value = (int)Math.Round((double)(100 * done) / total);

                // if this chapter is deleted but user doesn't wants to get deleted entries, so skip this chapter.
                if (node.chapter.IsDeleted && !searchTrash)
                    continue;

                // if both options are off, so quit
                if (!searchAll && !searchTrash)
                    return false;

                if (searchAll)
                {
                    // if this chapter is deleted but user doesn't wants to get deleted entries, so skip this chapter.
                    if (node.chapter.IsDeleted && !searchTrash)
                        continue;

                }
                else if (searchTrash)
                {
                    if (!node.chapter.IsDeleted)
                        continue; // user unchecked search all entries but deleted, this entry isn't deleted, so skip it.

                }
                else
                {
                    // no option chosen, return empty list
                    GC.Collect();
                    return false;
                }

                // entry date and time range check
                DateTime chapterDate = new DateTime(node.chapter.chapterDateTime.Year, node.chapter.chapterDateTime.Month,
                    node.chapter.chapterDateTime.Day, node.chapter.chapterDateTime.Hour,
                    node.chapter.chapterDateTime.Minute, node.chapter.chapterDateTime.Second, 0);
                int result1 = DateTime.Compare(chapterDate, from);
                int result2 = DateTime.Compare(chapterDate, through);
                if (result1 < 0)
                    continue; // date mismatch

                if (result2 > 0)
                    continue; // date mismatch

                // entry true creation date and time range check
                DateTime chapterCreationDate = new DateTime(node.chapter.creationDateTime.Year, node.chapter.creationDateTime.Month,
                    node.chapter.creationDateTime.Day, node.chapter.creationDateTime.Hour,
                    node.chapter.creationDateTime.Minute, node.chapter.creationDateTime.Second, 0);
                result1 = DateTime.Compare(chapterCreationDate, CDfrom);
                result2 = DateTime.Compare(chapterCreationDate, CDthrough);
                if (result1 < 0)
                    continue; // date mismatch

                if (result2 > 0)
                    continue; // date mismatch

                // entry modification date and time range check
                DateTime chapterModificationDate = new DateTime(node.chapter.modificationDateTime.Year, node.chapter.modificationDateTime.Month,
                    node.chapter.modificationDateTime.Day, node.chapter.modificationDateTime.Hour,
                    node.chapter.modificationDateTime.Minute, node.chapter.modificationDateTime.Second, 0);
                result1 = DateTime.Compare(chapterModificationDate, MDfrom);
                result2 = DateTime.Compare(chapterModificationDate, MDthrough);
                if (result1 < 0)
                    continue; // date mismatch

                if (result2 > 0)
                    continue; // date mismatch

                // entry deletion date and time range check
                DateTime chapterDeletionDate = new DateTime(node.chapter.deletionDateTime.Year, node.chapter.deletionDateTime.Month,
                    node.chapter.deletionDateTime.Day, node.chapter.deletionDateTime.Hour,
                    node.chapter.deletionDateTime.Minute, node.chapter.deletionDateTime.Second, 0);
                result1 = DateTime.Compare(chapterDeletionDate, DDfrom);
                result2 = DateTime.Compare(chapterDeletionDate, DDthrough);
                if (result1 < 0)
                    continue; // date mismatch

                if (result2 > 0)
                    continue; // date mismatch

                bool matchesFound = false;
                long totalMatches = 0;

                List<RegisterItem>? lineage = null;
                Register.Lineage(ctx, ctx.dbNodeTreeRegistryFile, thisItem, ref lineage, true, true, false);
                entryPath = Register.LineageFullPath(lineage);
                form.updateSearchProgressPath(entryPath);

                // user demands empty string meaning he demands to find entries only by their configuration parameters
                // like creation date modification date and so and so.
                if (searchEmptyString)
                {
                    // no text to find and therefore nothing to replace. so directly list this entry/node because other 
                    // search parameters are valid at this place.
                    __insertLvSearchItem(ctx, lv, thisItem, entryPath, totalMatches);
                    continue; // bypass regex text search and replace.
                }

                // configure richtextbox
                try
                {
                    rtb.Rtf = rtf;// rtbnative.Rtf;
                }
                catch (Exception)
                {
                    continue;
                }

                // both node's title and node's body must not be searched and replaced simultaneusly.
                // this is illogical because it confuses the user and also mistakenly
                // replaces the title when it must not be replaced without user's special requirement.
                // therefore node's title and node's body shall be separately searched and replaced.
                if (!searchReplaceTitle)
                {
                    // node's body

                    // initialize search and generate collection
                    FindReplaceFramework.MatchedTextCollection col = FindReplaceFramework.MatchedTextCollection.initializeSearch(ref regexOptions,
                        rtb.Document, searchPattern);

                    // replace if demanded
                    if (col.Count > 0)
                    {
                        // update
                        matchesFound = true;
                        totalMatches += col.Count;

                        // finally commit replace if user requires right here
                        if ((replace) && (node.chapter.specialNodeType != SpecialNodeType.SystemNode))
                        {
                            // begin with first or another node
                            col.Next(true);
                            while (col.current != null)
                            {
                                // set selection and replace
                                TextRange selection = new TextRange(col.current.start, col.current.end);
                                selection.Text = replacement;
                                col.Remove(ref col.current);
                                col.Next(true); // to the next node
                            }
                        }
                    }
                }

                if (searchReplaceTitle)
                {
                    // node's title

                    // check and update node's title
                    MatchCollection matches1 = regex.Matches(node.chapter.Title);
                    if (matches1.Count > 0)
                    {
                        if ((replace) && (node.chapter.specialNodeType != SpecialNodeType.SystemNode))
                        {
                            node.chapter.Title = regex.Replace(node.chapter.Title, replacement);
                        }
                        matchesFound = true;
                        totalMatches += matches1.Count;
                    }
                }

                // finally update node in db
                if (matchesFound)
                {
                    if ((replace) && (node.chapter.specialNodeType != SpecialNodeType.SystemNode))
                    {
                        entryMethods.DBUpdateNodeOFSDB(ctx, node, rtb.Rtf, rtb.XamlBytes, true); // nonsystem node so change it
                    }
                    __insertLvSearchItem(ctx, lv, thisItem, entryPath, totalMatches);
                }

            }

            // phase 2 - now traverse locations true descendants tree

            foreach (Int64 thisItemId in worklist)//RegisterItem item in worklist)
            {
                RegisterItem? thisItem = null;
                String rtf = "";
                thisItem = Register.LoadSetupRegisterItem(ctx, ctx.dbNodeTreeRegistryFile, thisItemId, false, false, false, false, true, true);
                if (thisItem == null) continue;

                // traverse tree process only true descendants of this location item

                while (true)
                {
                    // load item from registry
                    RegisterItem? nextDescendant = thisItem.tree.Next();
                    if (nextDescendant == null) break;

                    if (!thisItem.tree.IsDescendantOfAncestor(nextDescendant, thisItem))
                        continue; // this descendant is not true descendant of current item so skip

                    // this descendant is true descendant of current location item so process it

                    rtf = "";
                    byte[]? xamlbytes = null;
                    if (searchEmptyString)
                        nextDescendant.loadNode(ctx, ref rtf, ref xamlbytes, false);
                    else
                        nextDescendant.loadNode(ctx, ref rtf, ref xamlbytes, true);

                    myNode? node = nextDescendant.node;

                    done++;
                    tsProgressBar.Value = (int)Math.Round((double)(100 * done) / total);

                    // if this chapter is deleted but user doesn't wants to get deleted entries, so skip this chapter.
                    if (node.chapter.IsDeleted && !searchTrash)
                        continue;

                    // if both options are off, so quit
                    if (!searchAll && !searchTrash)
                        return false;

                    if (searchAll)
                    {
                        // if this chapter is deleted but user doesn't wants to get deleted entries, so skip this chapter.
                        if (node.chapter.IsDeleted && !searchTrash)
                            continue;

                    }
                    else if (searchTrash)
                    {
                        if (!node.chapter.IsDeleted)
                            continue; // user unchecked search all entries but deleted, this entry isn't deleted, so skip it.

                    }
                    else
                    {
                        // no option chosen, return empty list
                        GC.Collect();
                        return false;
                    }

                    // entry date and time range check
                    DateTime chapterDate = new DateTime(node.chapter.chapterDateTime.Year, node.chapter.chapterDateTime.Month,
                        node.chapter.chapterDateTime.Day, node.chapter.chapterDateTime.Hour,
                        node.chapter.chapterDateTime.Minute, node.chapter.chapterDateTime.Second, 0);
                    int result1 = DateTime.Compare(chapterDate, from);
                    int result2 = DateTime.Compare(chapterDate, through);
                    if (result1 < 0)
                        continue; // date mismatch

                    if (result2 > 0)
                        continue; // date mismatch

                    // entry true creation date and time range check
                    DateTime chapterCreationDate = new DateTime(node.chapter.creationDateTime.Year, node.chapter.creationDateTime.Month,
                        node.chapter.creationDateTime.Day, node.chapter.creationDateTime.Hour,
                        node.chapter.creationDateTime.Minute, node.chapter.creationDateTime.Second, 0);
                    result1 = DateTime.Compare(chapterCreationDate, CDfrom);
                    result2 = DateTime.Compare(chapterCreationDate, CDthrough);
                    if (result1 < 0)
                        continue; // date mismatch

                    if (result2 > 0)
                        continue; // date mismatch

                    // entry modification date and time range check
                    DateTime chapterModificationDate = new DateTime(node.chapter.modificationDateTime.Year, node.chapter.modificationDateTime.Month,
                        node.chapter.modificationDateTime.Day, node.chapter.modificationDateTime.Hour,
                        node.chapter.modificationDateTime.Minute, node.chapter.modificationDateTime.Second, 0);
                    result1 = DateTime.Compare(chapterModificationDate, MDfrom);
                    result2 = DateTime.Compare(chapterModificationDate, MDthrough);
                    if (result1 < 0)
                        continue; // date mismatch

                    if (result2 > 0)
                        continue; // date mismatch

                    // entry deletion date and time range check
                    DateTime chapterDeletionDate = new DateTime(node.chapter.deletionDateTime.Year, node.chapter.deletionDateTime.Month,
                        node.chapter.deletionDateTime.Day, node.chapter.deletionDateTime.Hour,
                        node.chapter.deletionDateTime.Minute, node.chapter.deletionDateTime.Second, 0);
                    result1 = DateTime.Compare(chapterDeletionDate, DDfrom);
                    result2 = DateTime.Compare(chapterDeletionDate, DDthrough);
                    if (result1 < 0)
                        continue; // date mismatch

                    if (result2 > 0)
                        continue; // date mismatch

                    bool matchesFound = false;
                    long totalMatches = 0;


                    List<RegisterItem>? lineage = null;
                    Register.Lineage(ctx, ctx.dbNodeTreeRegistryFile, nextDescendant, ref lineage, true, true, false);
                    entryPath = Register.LineageFullPath(lineage);
                    form.updateSearchProgressPath(entryPath);

                    // user demands empty string meaning he demands to find entries only by their configuration parameters
                    // like creation date modification date and so and so.
                    if (searchEmptyString)
                    {
                        // no text to find and therefore nothing to replace. so directly list this entry/node because other 
                        // search parameters are valid at this place.
                        __insertLvSearchItem(ctx, lv, nextDescendant, entryPath, totalMatches);
                        continue; // bypass regex text search and replace.
                    }

                    // configure richtextbox
                    try
                    {
                        if (ctx.dbEntryType == EntryType.Xaml)
                            rtb.XamlBytes = xamlbytes;
                        else
                            rtb.Rtf = rtf;// rtbnative.Rtf;
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    // both node's title and node's body must not be searched and replaced simultaneusly.
                    // this is illogical because it confuses the user and also mistakenly
                    // replaces the title when it must not be replaced without user's special requirement.
                    // therefore node's title and node's body shall be separately searched and replaced.
                    if (!searchReplaceTitle)
                    {
                        // node's body

                        // initialize search and generate collection
                        FindReplaceFramework.MatchedTextCollection col = FindReplaceFramework.MatchedTextCollection.initializeSearch(ref regexOptions,
                            rtb.Document, searchPattern);

                        // replace if demanded
                        if (col.Count > 0)
                        {
                            // update
                            matchesFound = true;
                            totalMatches += col.Count;

                            // finally commit replace if user requires right here
                            if ((replace) && (node.chapter.specialNodeType != SpecialNodeType.SystemNode))
                            {
                                // begin with first or another node
                                col.Next(true);
                                while (col.current != null)
                                {
                                    // set selection and replace
                                    TextRange selection = new TextRange(col.current.start, col.current.end);
                                    selection.Text = replacement;
                                    col.Remove(ref col.current);
                                    col.Next(true); // to the next node
                                }
                            }
                        }
                    }

                    if (searchReplaceTitle)
                    {
                        // node's title

                        // check and update node's title
                        MatchCollection matches1 = regex.Matches(node.chapter.Title);
                        if (matches1.Count > 0)
                        {
                            if ((replace) && (node.chapter.specialNodeType != SpecialNodeType.SystemNode))
                            {
                                node.chapter.Title = regex.Replace(node.chapter.Title, replacement);
                            }
                            matchesFound = true;
                            totalMatches += matches1.Count;
                        }
                    }

                    // finally update node in db
                    if (matchesFound)
                    {
                        if ((replace) && (node.chapter.specialNodeType != SpecialNodeType.SystemNode))
                        {
                            entryMethods.DBUpdateNodeOFSDB(ctx, node, rtb.Rtf, rtb.XamlBytes, true); // nonsystem node so change it
                        }
                        __insertLvSearchItem(ctx, lv, nextDescendant, entryPath, totalMatches);
                    }
                }
            }

            // done
            tsProgressBar.Value = 0;
            //txtSearchProgressPath.Text = "";
            GC.Collect();
            return true;
        }
    }
}

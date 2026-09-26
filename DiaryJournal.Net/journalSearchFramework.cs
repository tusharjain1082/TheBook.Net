using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Controls;
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
            FormJournalDesign1? form1, FormJournalDesign2? form2, System.Windows.Forms.TextBox txtSearchProgressPath, System.Windows.Forms.ListView lv, ToolStripProgressBar tsProgressBar,
            DateTime inputFrom, DateTime inputFromTime, DateTime inputThrough, DateTime inputThroughTime, bool useDateTimeRange,
            DateTime inputCDFrom, DateTime inputCDFromTime, DateTime inputCDThrough, DateTime inputCDThroughTime, bool useCreationDateTimeRange,
            DateTime inputMDFrom, DateTime inputMDFromTime, DateTime inputMDThrough, DateTime inputMDThroughTime, bool useModificationDateTimeRange,
            String searchPattern, String replacement, bool searchAll,
            bool matchCase, bool matchWholeWord, bool replace, bool searchReplaceTitle, bool searchEmptyString, List<UInt32> locations)
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

            // successfully prepared all config
            // verify if the pattern is valid.
            if (!searchEmptyString)
            {
                if (!myCommonMethods1.IsValidRegex(searchPattern))
                    return false;
            }

            WpfRichTextBoxEx rtb = xamlEntry.dummy;

            // select search location if user demands it
            List<UInt32>? worklist = locations;
            UInt32 total = 0;
            UInt32 done = 0;

            if (locations.Count == 0) 
            {
                // no location provided, so load root's children as worklist and traverse each tree
                RegisterItem? root = Register.LoadSetupRegisterItem(ctx, 0, false, false, true, false, true, true);
                if (root == null) return false; // error abort
                worklist.Add(root.Id);
                // prepare counter and progress
                total = Register.Total(ctx);
                total += 1; // increment root
            }
            else
            {
                // prepare counter and progress
                foreach (UInt32 id in worklist)
                {
                    RegisterItem? thisItem = Register.LoadSetupRegisterItem(ctx, id, false, false, false, false, true, true);
                    if (thisItem == null) continue;
                    total += thisItem.tree.CountDescendants();
                    total += 1; // increment this current item
                }
            }

            // initialize a single regex for all operations
            // now load regex with pattern and options
            Regex regex = new Regex(searchPattern, regexOptions);
            String entryPath = "";

            // phase 1 - parent to all it's descendants using worklist

            foreach (UInt32 thisItemId in worklist)
            {
                RegisterItem? thisItem = Register.LoadSetupRegisterItem(ctx, thisItemId, false, false, false, false, true, true);
                if (thisItem == null) continue;
                if (!searchEmptyString)
                {
                    thisItem.loadNode(ctx, ref thisItem.rtf, ref thisItem.xamlbytes, true);
                    if (thisItem.xamlbytes != null)
                        rtb.XamlBytes = thisItem.xamlbytes;
                    else if (thisItem.rtf != null)
                        rtb.Rtf = thisItem.rtf;
                    else
                        rtb.Rtf = "";
                }
                else
                {
                    thisItem.loadNode(ctx, ref thisItem.rtf, ref thisItem.xamlbytes, false);
                    rtb.Rtf = "";
                }

                myNode? node = thisItem.node;
                done += 1;
                tsProgressBar.Value = (int)Math.Round((double)(100 * done) / total);

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

                bool matchesFound = false;
                long totalMatches = 0;

                List<RegisterItem>? lineage = null;
                Register.Lineage(ctx, thisItem, ref lineage, true, true, false);
                entryPath = Register.LineageFullPath(lineage);
                if (form1 != null)
                    form1.updateSearchProgressPath(entryPath);
                else
                    form2.updateSearchProgressPath(entryPath);

                // user demands empty string meaning he demands to find entries only by their configuration parameters
                // like creation date modification date and so and so.
                if (searchEmptyString)
                {
                    // no text to find and therefore nothing to replace. so directly list this entry/node because other 
                    // search parameters are valid at this place.
                    __insertLvSearchItem(ctx, lv, thisItem, entryPath, totalMatches);
                }
                else
                {
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
                            if ((replace) && (!ctx.readOnly) && (node.chapter.specialNodeType == SpecialNodeType.None || node.chapter.specialNodeType == SpecialNodeType.AnyOrAll
                                || node.chapter.specialNodeType == SpecialNodeType.NonSystemNode))
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
                            if ((replace) && (!ctx.readOnly) && (node.chapter.specialNodeType == SpecialNodeType.None || node.chapter.specialNodeType == SpecialNodeType.AnyOrAll
                                || node.chapter.specialNodeType == SpecialNodeType.NonSystemNode))
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
                        if ((replace) && (!ctx.readOnly) && (node.chapter.specialNodeType == SpecialNodeType.None || node.chapter.specialNodeType == SpecialNodeType.AnyOrAll
                            || node.chapter.specialNodeType == SpecialNodeType.NonSystemNode))
                        {
                            entryMethods.DBUpdateNodeOFSDB(ctx, node, rtb.Rtf, rtb.XamlBytes, true); // nonsystem node so change it
                        }
                        __insertLvSearchItem(ctx, lv, thisItem, entryPath, totalMatches);
                    }
                }

                // phase 2 - now traverse descendants tree sequence of this parent
                while (true)
                {
                    RegisterItem? descendant = thisItem.tree.Next();
                    if (descendant == null) break; // no more descendant so break
                    if (!searchEmptyString)
                    {
                        descendant.loadNode(ctx, ref descendant.rtf, ref descendant.xamlbytes, true);
                        if (descendant.xamlbytes != null)
                            rtb.XamlBytes = descendant.xamlbytes;
                        else if (thisItem.rtf != null)
                            rtb.Rtf = descendant.rtf;
                        else
                            rtb.Rtf = "";
                    }
                    else
                    {
                        descendant.loadNode(ctx, ref descendant.rtf, ref descendant.xamlbytes, false);
                        rtb.Rtf = "";
                    }

                    node = descendant.node;
                    done += 1;
                    tsProgressBar.Value = (int)Math.Round((double)(100 * done) / total);

                    // entry date and time range check
                    chapterDate = new DateTime(node.chapter.chapterDateTime.Year, node.chapter.chapterDateTime.Month,
                        node.chapter.chapterDateTime.Day, node.chapter.chapterDateTime.Hour,
                        node.chapter.chapterDateTime.Minute, node.chapter.chapterDateTime.Second, 0);
                    result1 = DateTime.Compare(chapterDate, from);
                    result2 = DateTime.Compare(chapterDate, through);
                    if (result1 < 0)
                        continue; // date mismatch

                    if (result2 > 0)
                        continue; // date mismatch

                    // entry true creation date and time range check
                    chapterCreationDate = new DateTime(node.chapter.creationDateTime.Year, node.chapter.creationDateTime.Month,
                        node.chapter.creationDateTime.Day, node.chapter.creationDateTime.Hour,
                        node.chapter.creationDateTime.Minute, node.chapter.creationDateTime.Second, 0);
                    result1 = DateTime.Compare(chapterCreationDate, CDfrom);
                    result2 = DateTime.Compare(chapterCreationDate, CDthrough);
                    if (result1 < 0)
                        continue; // date mismatch

                    if (result2 > 0)
                        continue; // date mismatch

                    // entry modification date and time range check
                    chapterModificationDate = new DateTime(node.chapter.modificationDateTime.Year, node.chapter.modificationDateTime.Month,
                        node.chapter.modificationDateTime.Day, node.chapter.modificationDateTime.Hour,
                        node.chapter.modificationDateTime.Minute, node.chapter.modificationDateTime.Second, 0);
                    result1 = DateTime.Compare(chapterModificationDate, MDfrom);
                    result2 = DateTime.Compare(chapterModificationDate, MDthrough);
                    if (result1 < 0)
                        continue; // date mismatch

                    if (result2 > 0)
                        continue; // date mismatch

                    matchesFound = false;
                    totalMatches = 0;

                    lineage = null;
                    Register.Lineage(ctx, descendant, ref lineage, true, true, false);
                    entryPath = Register.LineageFullPath(lineage);
                    if (form1 != null)
                        form1.updateSearchProgressPath(entryPath);
                    else
                        form2.updateSearchProgressPath(entryPath);

                    // user demands empty string meaning he demands to find entries only by their configuration parameters
                    // like creation date modification date and so and so.
                    if (searchEmptyString)
                    {
                        // no text to find and therefore nothing to replace. so directly list this entry/node because other 
                        // search parameters are valid at this place.
                        __insertLvSearchItem(ctx, lv, descendant, entryPath, totalMatches);
                        continue; // bypass regex text search and replace.
                    }
                    else
                    {
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
                                if ((replace) && (!ctx.readOnly) && (node.chapter.specialNodeType == SpecialNodeType.None || node.chapter.specialNodeType == SpecialNodeType.AnyOrAll
                                    || node.chapter.specialNodeType == SpecialNodeType.NonSystemNode))
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
                                if ((replace) && (!ctx.readOnly) && (node.chapter.specialNodeType == SpecialNodeType.None || node.chapter.specialNodeType == SpecialNodeType.AnyOrAll
                                    || node.chapter.specialNodeType == SpecialNodeType.NonSystemNode))
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
                            if ((replace) && (!ctx.readOnly) && (node.chapter.specialNodeType == SpecialNodeType.None || node.chapter.specialNodeType == SpecialNodeType.AnyOrAll
                                || node.chapter.specialNodeType == SpecialNodeType.NonSystemNode))
                            {
                                entryMethods.DBUpdateNodeOFSDB(ctx, node, rtb.Rtf, rtb.XamlBytes, true); // nonsystem node so change it
                            }
                            __insertLvSearchItem(ctx, lv, descendant, entryPath, totalMatches);
                        }
                    }
                }
            }
            // done
            tsProgressBar.Value = 0;
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Controls;

namespace DiaryJournal.Net
{
    public static class txtEntry
    {
        public static String toTxt(String rtf)
        {
            String text = "";

            if (rtf.Length > 0)
            {
                WpfRichTextBoxEx rtb = new WpfRichTextBoxEx();
                rtb.Rtf = rtf;
                text = rtb.Text;
                GC.Collect();
            }
            return text;
        }
        public static bool fromTxt(ref Chapter chapter, String file, ref String rtf, String extComplete)
        {
            if (!File.Exists(file))
                return false;

            String text = "";
            try
            {
                text = File.ReadAllText(file);
            }
            catch (Exception)
            {
                using (StreamReader reader = new StreamReader(file, Encoding.Unicode))
                {
                    text = reader.ReadToEnd();
                    reader.Close(); 
                    reader.Dispose();   
                }
            }

            WpfRichTextBoxEx rtb = new WpfRichTextBoxEx();
            rtb.Text = text;
            rtf = rtb.Rtf;
            entryMethods.convertEntryFilenameToChapter(ref chapter, file);
            return true;
        }

    }
}

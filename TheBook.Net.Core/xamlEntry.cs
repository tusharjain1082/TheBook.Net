using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Controls;

namespace DiaryJournal.Net
{
    public static class xamlEntry
    {
        public static WpfRichTextBoxEx dummy = new WpfRichTextBoxEx();

        public static byte[]? toXaml(String? rtf)
        {
            // get rtf and update

            if (rtf == null) return Array.Empty<byte>();
            if (rtf.Length <= 0) return Array.Empty<byte>();

            dummy.Rtf = rtf;
            return dummy.SaveXamlPackage();
        }

        public static String? toRtf(byte[]? bytes)
        {
            if (bytes == null) return "";
            if (bytes.Length <= 0) return "";

            dummy.XamlBytes = bytes;
            return dummy.Rtf;
        }

    }
}

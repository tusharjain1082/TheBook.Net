using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DocSharp.Docx; // Ensure you have installed the DocSharp.Docx NuGet package
using DocumentFormat.OpenXml;

namespace DiaryJournal.Net
{
    public static class docxEntry
    {
        public static byte[] toDocX(String rtf)
        {
            // get rtf and update
            if (rtf.Length <= 0)
                return new byte[0];

            // Load and convert RTF to DOCX using DocSharp
            // DocSharp handles conversion without needing Microsoft Office Interop
            MemoryStream ms = new MemoryStream();
            RtfToDocxConverter c = new RtfToDocxConverter();
            c.ConvertString(rtf, ms, WordprocessingDocumentType.Document);
            ms.Flush();

            return ms.ToArray();
        }
    }
}

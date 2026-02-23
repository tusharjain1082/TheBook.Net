using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace TheBook.Net.Core
{
    public class Methods2
    {

        public static String GetMyComputerFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
        }

        // OK:
        public static void SetFileLength(String src, long filesize)
        {
            FileStream fsoutput = new FileStream(src, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            fsoutput.Seek(0, SeekOrigin.Begin);
            fsoutput.SetLength(filesize);
            fsoutput.Flush();
            fsoutput.Close();
        }

        public static List<byte> Combine(List<byte[]> arrays)
        {
            List<byte> list = new List<byte>();
            foreach (byte[] array in arrays)
            {
                list.AddRange(array);
            }

            return list;
        }

        public static byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        public static String GenerateTempFilename()
        {
            return GetFullAppStartupPath() + System.Guid.NewGuid().ToString() + ".tmp";
        }

        public static bool DeleteFile(String strPathFile)
        {

            try
            {
                File.Delete(strPathFile);
            }
            catch (Exception)
            {
                //  ' Access denied or error while deleting the file.
                return false;
            }
            return true;

        }

        public static FileStream CreateTempFile(out String outputFilename)
        {
            outputFilename = GenerateTempFilename();
            DeleteFile(outputFilename);
            FileStream fsoutput = new FileStream(outputFilename, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);//, 52428800, FileOptions.None);
            fsoutput.Seek(0, SeekOrigin.Begin);
            return fsoutput; 
        }

        public static bool MultiplyFileDataSalted(String src, byte[] salt, int totalIterations)
        {
            FileStream fsinput = new FileStream(src, FileMode.Open, FileAccess.ReadWrite, FileShare.None);//, 52428800, FileOptions.RandomAccess);
            fsinput.Seek(0, SeekOrigin.Begin);
            bool result = MultiplyStreamSalted(fsinput, salt, totalIterations);
            fsinput.Close();
            fsinput.Dispose();
            return result;

        }
        public static String removeAllInvalidPathCharacters(String value)
        {
            string illegal = value;//"//\"M\"\\a/ry/ h**ad:>> a\\/:*?\"| li*tt|le|| la\"mb.?";
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            value = r.Replace(illegal, "-");
            return value;
        }


        // Combines 2 or more Streams into one destination stream
        public static bool MultiplyStreamSalted(Stream sOP, byte[] salt, int totalIterations)
        {
            String tmpfile = "";
            FileStream fstmp = CreateTempFile(out tmpfile);
            sOP.Seek(0, SeekOrigin.End);

            for (int i = 0; i < totalIterations; i++)
            {
                // Copy data of src to temp file
                ConcatStreams(sOP, fstmp);
                // Concatenate tmp file data into src file
                ConcatStreams(fstmp, sOP);
                if (salt != null)
                {
                    sOP.Write(salt, 0, salt.Length);
                }
            }
            sOP.Flush();
            fstmp.Close();
            fstmp.Dispose();
            DeleteFile(tmpfile);
            return true;
        }


        // Combines 2 or more Streams into one destination stream
        public static bool CombineMultipleStreams(Stream dest, params Stream[] arrays)
        {
            //byte[] rv = new byte[arrays.Sum(a => a.Length)];
            //int offset = 0;
            //Stream[] src = new Stream[] { };
            bool result = false;
            foreach (Stream src in arrays)
            {
                result = ConcatStreams(src, dest);
            }
            return result;
        }

        public static bool ConcatStreams(Stream src, Stream dest)
        {
            src.Seek(0, SeekOrigin.Begin);
            dest.Seek(0, SeekOrigin.End);
            src.CopyTo(dest);
            return true;
        }

        public static bool CompareByteArrays(byte[] a1, byte[] b1)
        {
            int i;
            if (a1.Length == b1.Length)
            {
                i = 0;
                while (i < a1.Length && (a1[i] == b1[i])) //Earlier it was a1[i]!=b1[i]
                {
                    i++;
                }
                if (i == a1.Length)
                {
                    return true;
                }
            }

            return false;
        }

        public static byte[] MultiplyBytesArray(byte[] data, int totalIterations)
        {
            for (int i = 0; i < totalIterations; i++)
            {
                data = data.Concat(data).ToArray();
            }
            return data;
        }

        // Combines 2 or more byte arrays into one & gives it as output
        public static byte[] CombineMultipleByteArraysLinq(params byte[][] arrays)
        {
            //byte[] rv = new byte[arrays.Sum(a => a.Length)];
            //int offset = 0;
            byte[] data = new byte[] { };

            foreach (byte[] array in arrays)
            {
                data = data.Concat(data).ToArray();
                //System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                //offset += array.Length;
            }
            return data;
        }

        // Combines 2 or more byte arrays into one & gives it as output
        public static byte[] CombineByteArrays(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        public static String? GetFullAppStartupPath()
        {
            String? strPath = "";
            //strPath = System.IO.Path.GetFullPath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName));//System.Environment.CurrentDirectory);////AppDomain.CurrentDomain.BaseDirectory);
            strPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            strPath = AddPathSep(strPath);
            return strPath;
        }

            public static String AddPathSep(String strpathstring)
            {
                String strPath = "";
                strPath = System.IO.Path.GetFullPath(strpathstring);
                String pathsep = strPath.Substring((strPath.Length - 1), 1);
                if (pathsep != System.IO.Path.DirectorySeparatorChar.ToString())
                {
                    strPath = strPath + System.IO.Path.DirectorySeparatorChar;
                }

                return strPath;

            }

        public static byte[]? ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[]?)converter.ConvertTo(img, typeof(byte[]));
        }

        public static byte[] ImageToByte2(Image img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }


        public static Control? FindControlRecursive(Control container, string name)
        {
            if (container.Name == name)
                return container;
 
            foreach (Control ctrl in container.Controls)
            {
                Control? foundCtrl = FindControlRecursive(ctrl, name);
                if (foundCtrl != null)
                    return foundCtrl;
            }
            return null;
        }
    }

    public static class BytesConvertor
    {
        public static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        public static string SizeSuffix_BigInteger(System.Numerics.BigInteger value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix_BigInteger(-value); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)System.Numerics.BigInteger.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Markup.Primitives;
using System.Net.NetworkInformation;
using System.Drawing.Imaging;
using System.Net;
using System.Globalization;
using System.Xml;
using System.Drawing.Drawing2D;
using System.Net.Http;
using System.Windows.Media.Imaging;
using System.Collections;
using System.Xml.Serialization;
using TheBook.Net.Core;
using System.Diagnostics;

namespace DiaryJournal.Net
{
    public sealed class SharedReference<T>
    where T : class
    {
        public T Reference
        {
            get; set;
        }
    }

    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
        public static IEnumerable<T> GetNames<T>()
        {
            return Enum.GetNames(typeof(T)).Cast<T>();
        }
        public static IEnumerable<T> GetEnumValues<T>()
        {
            // Can't use type constraints on value types, so have to do check like this
            if (typeof(T).BaseType != typeof(Enum))
            {
                throw new ArgumentException("T must be of type System.Enum");
            }

            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }

    // last item is pushed and last item is first to pop
    public class LifoBuffer<T> : LinkedList<T>
    {
        private int capacity;

        public LifoBuffer(int capacity)
        {
            this.capacity = capacity;
        }

        public void Add(T item)
        {
            if (Count == capacity) RemoveLast();
            AddFirst(item);
        }
    }
    // last item is pushed and last item is first to pop
    public class DroppingStack<T> : IEnumerable<T>
    {
        T[] array;
        int cap;
        int begin;
        int end;
        public DroppingStack(int capacity)
        {
            cap = capacity + 1;
            array = new T[cap];
            begin = 0;
            end = 0;
        }

        public T pop()
        {
            if (begin == end) throw new Exception("No item");
            begin--;
            if (begin < 0)
                begin += cap;
            return array[begin];
        }

        public void push(T value)
        {
            array[begin] = value;
            begin = (begin + 1) % cap;
            if (begin == end)
                end = (end + 1) % cap;
        }

        public IEnumerator<T> GetEnumerator()
        {
            int i = begin - 1;
            while (i != end - 1)
            {
                yield return array[i];
                i--;
                if (i < 0)
                    i += cap;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
    // last item is pushed and last item is first to pop
    // working
    public class CyclicStack<T>
    {
        private T[] stack;
        private int capacity;
        private int curIndex = 0;

        public int Count { get; private set; }
        public CyclicStack(int capacity)
        {
            this.capacity = capacity;
            stack = new T[capacity];
            this.Count = 0;
        }
        public T this[int index]
        {
            get
            {
                if (index >= capacity)
                    throw new Exception("Index is out of bounds");
                return this.stack[(curIndex + index) % capacity];
            }
        }
        public void Push(T item)
        {
            curIndex = (curIndex + capacity - 1) % capacity;
            stack[curIndex] = item;
            this.Count++;
        }
        public T Pop()
        {
            if (this.Count == 0)
                throw new Exception("Collection is empty");
            int oldIndex = curIndex;
            curIndex = (curIndex + capacity + 1) % capacity;
            this.Count--;
            return stack[oldIndex];
        }
    }
    public static class XmlDocumentExtensions
    {
        public static void IterateThroughAllNodes(
            this XmlDocument doc,
            Action<XmlNode> elementVisitor)
        {
            if (doc != null && elementVisitor != null)
            {
                foreach (XmlNode node in doc.ChildNodes)
                {
                    doIterateNode(node, elementVisitor);
                }
            }
        }

        private static void doIterateNode(
            XmlNode node,
            Action<XmlNode> elementVisitor)
        {
            elementVisitor(node);

            foreach (XmlNode childNode in node.ChildNodes)
            {
                doIterateNode(childNode, elementVisitor);
            }
        }
    }

    public static class DirectoryInfoEx
    {

        public static void CopyToRecursive(this DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
                return;

            if (!target.Exists)
                target.Create();

            foreach (FileInfo f in source.GetFiles())
            {
                FileInfo newFile = new FileInfo(Path.Combine(target.FullName, f.Name));
                f.CopyTo(newFile.FullName, true);
            }

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                diSourceSubDir.CopyToRecursive(nextTargetSubDir);
            }
        }
        public static bool CopyTo(this DirectoryInfo source, DirectoryInfo target, bool overwrite)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
                return false;

            try
            {
                if (!target.Exists)
                    Directory.CreateDirectory(target.FullName);
            }
            catch { return false; }

            foreach (FileInfo f in source.GetFiles())
            {
                try
                {
                    FileInfo newFile = new FileInfo(Path.Combine(target.FullName, f.Name));
                    f.CopyTo(newFile.FullName, overwrite);
                }
                catch { }
            }
            return true;
        }
    }

    public static class ControlHelper
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(this Control Target)
        {
            SendMessage(Target.Handle, WM_SETREDRAW, 0, IntPtr.Zero);
        }

        public static void ResumeDrawing(this Control Target)
        {
            SendMessage(Target.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
            Target.Invalidate(true);
            Target.Update();
        }

        public static IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        public static List<Control> GetAllChildControls(Control Root, Type? FilterType = null)
        {
            List<Control> AllChilds = new List<Control>();
            foreach (Control ctl in Root.Controls)
            {
                Type ctlType = ctl.GetType();

                if (FilterType != null)
                {
                    if (ctlType == FilterType)
                    {
                        AllChilds.Add(ctl);
                    }
                }
                else
                {
                    AllChilds.Add(ctl);
                }
                if (ctl.HasChildren)
                {
                    GetAllChildControls(ctl, FilterType);
                }
            }
            return AllChilds;
        }
    }


    public static class myCommonMethods1
    {
        public static void OpenFolderInExplorer(string folderPath)
        {
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.Console.WriteLine($"Error: Folder '{folderPath}' does not exist.");
                return;
            }

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "explorer.exe";
            psi.Arguments = folderPath;
            Process.Start(psi);
        }

        //public static void OpenFolderSelectFile(String file)
        // {
        //    Process.Start("explorer.exe", $"/select,\"{file}\"");
        //}/

        public static void OpenFolderSelectFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    // Construct the argument string for explorer.exe
                    // The /select parameter tells explorer to open the parent directory and select the specified file.
                    string argument = $"/select,\"{filePath}\"";

                    // Start a new process for explorer.exe
                    Process.Start("explorer.exe", argument);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error opening Explorer: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
            }
        }

        public static bool CheckForInternetConnection(int timeoutMs = 10000, string url = null)
        {
            try
            {
                url ??= CultureInfo.InstalledUICulture switch
                {
                    { Name: var n } when n.StartsWith("fa") => // Iran
                        "http://www.aparat.com",
                    { Name: var n } when n.StartsWith("zh") => // China
                        "http://www.baidu.com",
                    _ =>
                        "http://www.gstatic.com/generate_204",
                };

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = timeoutMs;
                using (var response = (HttpWebResponse)request.GetResponse())
                    return true;
            }
            catch
            {
                return false;
            }
        }
        public static async Task<bool> SendPing(string hostNameOrAddress)
        {
            using (var ping = new Ping())
            {
                try
                {
                    PingReply result = await ping.SendPingAsync(hostNameOrAddress);
                    return result.Status == IPStatus.Success;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }
        /*
        public static String convertToString(this Enum eff)
        {
            return Enum.GetName(eff.GetType(), eff);
        }

        public static EnumType convertToEnum<EnumType>(this String enumValue)
        {
            return (EnumType)Enum.Parse(typeof(EnumType), enumValue);
        }
        */
        public static Type GetListType<T>(this List<T> _)
        {
            return typeof(T);
        }

        public static Type GetEnumeratedType<T>(this IEnumerable<T> _)
        {
            return typeof(T);
        }

        public static List<ArraySegment<byte>> Split(byte[] arr, byte[] delimiter)
        {
            var result = new List<ArraySegment<byte>>();
            var segStart = 0;
            for (int i = 0, j = 0; i < arr.Length; i++)
            {
                if (arr[i] != delimiter[j])
                {
                    if (j == 0) continue;
                    j = 0;
                }

                if (arr[i] == delimiter[j])
                {
                    j++;
                }

                if (j == delimiter.Length)
                {
                    var segLen = (i + 1) - segStart - delimiter.Length;
                    if (segLen > 0) result.Add(new ArraySegment<byte>(arr, segStart, segLen));
                    segStart = i + 1;
                    j = 0;
                }
            }

            if (segStart < arr.Length)
            {
                result.Add(new ArraySegment<byte>(arr, segStart, arr.Length - segStart));
            }

            return result;
        }
        public static IEnumerable<FileInfo> EnumerateFiles(String path, EntryType entryType)
        {
            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            entryMethods.getEntryTypeFormats(entryType, ref ext, ref extComplete, ref extSearchPattern);

            DirectoryInfo dir = new DirectoryInfo(path);
            //Date created latest first
            //var files = dir.EnumerateFiles().OrderByDescending(x => x.CreationTime);

            //Date created latest last
            IEnumerable<FileInfo> files = dir.EnumerateFiles(extSearchPattern);//.OrderBy(x => x.CreationTime);

            ////dir.EnumerateFiles() is the same as the ones below
            //var files3 = dir.EnumerateFiles("*");
            //var files4 = dir.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
            return files;
        }

        public static bool SecureEraseFile(String strPath, int iterations, bool deleteFile = false)
        {
            FileStream fs = null;
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            int sectorSize = 1048576; // 1 mb
            byte[] sector = new byte[sectorSize];

            // this method erases the file beyond any recovery. data is completely destroyed. this is for security reasons.

            try
            {
                for (int i = 0; i < iterations; i++)
                {
                    fs = new FileStream(strPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None, sectorSize, FileOptions.RandomAccess);
                    fs.Seek(0, SeekOrigin.Begin);
                    long length = fs.Length;

                    long sectors = length / sectorSize;

                    // first erase sector by sector
                    for (long ctr = 0; ctr < sectors; ctr++)
                    {
                        // erase 3 times every sector. this overwrites beyond recovery.
                        long pos = fs.Position;
                        rng.GetBytes(sector);
                        fs.Write(sector, 0, sectorSize);
                        fs.Flush();
                        fs.Seek(pos, SeekOrigin.Begin);
                        rng.GetBytes(sector);
                        fs.Write(sector, 0, sectorSize);
                        fs.Flush();
                        fs.Seek(pos, SeekOrigin.Begin);
                        rng.GetBytes(sector);
                        fs.Write(sector, 0, sectorSize);
                        fs.Flush();
                    }

                    // then erase overwrite the remaining bytes 3 times so that the data is completely destroyed.
                    if (fs.Position != fs.Length)
                    {
                        // erase 3 times every sector. this overwrites beyond recovery.
                        long pos = fs.Position;
                        rng.GetBytes(sector);
                        fs.Write(sector, 0, (int)(fs.Length % sectorSize));
                        fs.Flush();
                        fs.Seek(pos, SeekOrigin.Begin);
                        rng.GetBytes(sector);
                        fs.Write(sector, 0, (int)(fs.Length % sectorSize));
                        fs.Flush();
                        fs.Seek(pos, SeekOrigin.Begin);
                        rng.GetBytes(sector);
                        fs.Write(sector, 0, (int)(fs.Length % sectorSize));
                        fs.Flush();
                    }

                    fs.Flush();
                    fs.Close();
                    fs.Dispose();
                    fs = null;
                }

                // iterations erasure completed, now finally delete the file if required.
                if (deleteFile)
                    DeleteFile(strPath);
            }
            catch
            {
                rng.Dispose();
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                    fs = null;
                }
                return false;
            }
            rng.Dispose();
            return true;
        }

        public static bool DeleteFile(String strPathFile)
        {
            if (strPathFile == "")
                return false;

            if (!File.Exists(strPathFile))
                return true;

            try
            {
                File.Delete(strPathFile); ;
            }
            catch
            {
                // Access denied or error while deleting the file.
                return false;

            }
            return true;
        }

        public static bool SecureEraseFilesRecursive(String root)
        {
            Queue<String> queue = new Queue<String>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                // the first node is dequeued and processed first, then it's children are processed level by level.
                String currentNode = queue.Dequeue();

                // get children directories of this node.
                IEnumerable<String> childDirectories = Directory.EnumerateDirectories(currentNode);

                // add all children is queue, they will be processed in this same way in this same place: 1st parent node, 2nd children nodes.
                foreach (String childNode in childDirectories)
                    queue.Enqueue(childNode);

                // get children files of this node.
                IEnumerable<String> childFiles = Directory.EnumerateFiles(currentNode);

                // now secure erase then delete all children files
                foreach (String childNode in childFiles)
                    SecureEraseFile(childNode, 1, true);
            }
            return true;
        }

        public static bool SecureDeleteDirectory(String root)
        {
            if (!SecureEraseFilesRecursive(root))
                return false;

            try
            {
                Directory.Delete(root, true);
            }
            catch
            {
            }
            return true;

        }
        public static bool IsValidRegex(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern)) return false;

            try
            {
                Regex.Match("", pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        public static string Base64Encode(string plainText)
        {
            if (plainText == null)
                return "";

            if (plainText.Length == 0)
                return "";

            //var encoding = new UnicodeEncoding();
            //var plainTextBytes = encoding.GetBytes(plainText);
            //return System.Convert.ToBase64String(plainTextBytes);
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            if (base64EncodedData == null)
                return "";

            if (base64EncodedData.Length == 0)
                return "";

            //var encoding = new UnicodeEncoding();
            //byte[] bytes = Convert.FromBase64String(base64EncodedData);
            //return encoding.GetString(bytes);
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string FontToString(Font font)
        {
            return font.FontFamily.Name + ":" + font.Size + ":" + (int)font.Style;
        }

        public static Font? StringToFont(string font)
        {
            if (font == "") return null;
            if (font.Length <= 0) return null;

            string[] parts = font.Split(':');
            if (parts.Length != 3)
                throw new ArgumentException("Not a valid font string", "font");

            Font loadedFont = new Font(parts[0], float.Parse(parts[1]), (System.Drawing.FontStyle)int.Parse(parts[2]));
            return loadedFont;
        }
        public static string ColorToString(Color color)
        {
            return color.ToArgb().ToString();
        }
        public static Color StringToColor(String color)
        {
            return Color.FromArgb(int.Parse(color));
        }

        public static IEnumerable<Color> getWebColors()
        {
            return Enum.GetValues(typeof(KnownColor))
                .Cast<KnownColor>()
                .Where(k => k >= KnownColor.Transparent && k < KnownColor.ButtonFace) //Exclude system colors
                .Select(k => Color.FromKnownColor(k));
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static bool[] Generate_TRNG_Bits(int total)
        {
            Boolean[] bits = new Boolean[total];
            for (int i = 0; i < total; i++)
            {
                bits[i] = Generate_TRNG_BitBoolean();//Generate_TRNG_BitBoolean();
            }
            return bits;
        }

        // true random number bytes array generator
        public static byte[] GenerateSalt(int sizeBytes)
        {
            bool[] bits = Generate_TRNG_Bits(sizeBytes * 8);
            return BitsToByteArray(bits);
        }

        public static byte[] BitsToByteArray(bool[] bits)
        {
            BitArray a = new BitArray(bits);
            byte[] bytes = new byte[a.Length / 8];
            a.CopyTo(bytes, 0);
            return bytes;
        }

        // true random number generator
        public static bool Generate_TRNG_BitBoolean()
        {
            var gen1 = 0;
            var gen2 = 0;
            Task.Run(() =>
            {
                while (gen1 < 1 || gen2 < 1)
                    Interlocked.Increment(ref gen1);
            });
            while (gen1 < 1 || gen2 < 1)
                Interlocked.Increment(ref gen2);
            return (gen1 + gen2) % 2 == 0;
        }

    }
}

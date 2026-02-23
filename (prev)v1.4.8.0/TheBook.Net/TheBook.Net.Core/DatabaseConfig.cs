using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using TheBook.Net.Core;

namespace DiaryJournal.Net
{
    
    public class DatabaseConfig
    {
        public String setName { get; set; } = "";
        public DateTime setDateTime { get; set; }
        public Guid setID { get; set; } = Guid.Empty;

        //        public const string currentDBIndexTagName = "currentDBIndex";
        public const string configTagName = "config";
        public const string setNameTagName = "setName";
        public const string setDateTimeTagName = "setDateTime";
        public const string setIDTagName = "setID";
        public const string productVersionTagName = "productVersion";

        // database config
        public const string lastModifiedEntryTagName = "lastModifiedEntry";
        public const string latestCreatedEntryTagName = "latestCreatedEntry";
        public Int64 lastModifiedEntry = 0;
        public Int64 latestCreatedEntry = 0;

        public string thisVersionString = Application.ProductVersion;
        public int thisMajorVersion = 0;
        public int thisMinorVersion = 0;
        public int thisRevision = 0;
        public int thisBuild = 0;
        public static string currentVersion = Application.ProductVersion;
        public int currentMajorVersion = 0;
        public int currentMinorVersion = 0;
        public int currentRevision = 0;
        public int currentBuild = 0;

        public DatabaseConfig()
        {
            String[] values = currentVersion.Split(".");
            thisMajorVersion = currentMajorVersion = int.Parse(values[0]);
            thisMinorVersion = currentMinorVersion = int.Parse(values[1]);
            thisRevision = currentRevision = int.Parse(values[2]);
            thisBuild = currentBuild = int.Parse(values[3]);
        }

        public static bool importVersionInfo(ref DatabaseConfig config, string versionString)
        {
            if (config == null) return false;
            if (versionString.Length <= 0) return false;

            String[] values = versionString.Split(".");
            if (values.Count() <= 3) return false;

            config.thisMajorVersion = int.Parse(values[0]);
            config.thisMinorVersion = int.Parse(values[1]);
            config.thisRevision = int.Parse(values[2]);
            config.thisBuild = int.Parse(values[3]);
            return true;

        }
        public static void setDBConfig(ref DatabaseConfig config, String setName, DateTime setDateTime, Guid setID)
        {
            config.setName = setName;
            config.setDateTime = setDateTime;
            config.setID = setID;
        }

        // load config from input db set
        public static bool fromXml(DatabaseConfig config, String file)
        {
            if (!File.Exists(file))
                return false;

            bool result = false;
            try
            {
                using (Stream s = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    result = fromXml(config, s);
                }
            }
            catch (Exception) { }

            return result;
        }
        public static bool fromXml(DatabaseConfig config, Stream s)
        {
            // load stream
            XmlDocument doc = new XmlDocument();
            doc.Load(s);
            XmlElement root = doc.DocumentElement;

            XmlNodeList list = doc.GetElementsByTagName(configTagName);
            if (list.Count <= 0)
                return false;

            // primary element
            XmlElement configElement = (XmlElement)list[0];

            // get all child elements

            // set name
            list = configElement.GetElementsByTagName(setNameTagName);
            if (list.Count <= 0)
                return false;

            XmlElement child0 = (XmlElement)list[0];
            config.setName = child0.InnerText;

            // set date and time
            list = configElement.GetElementsByTagName(setDateTimeTagName);
            if (list.Count <= 0)
                return false;

            XmlElement child1 = (XmlElement)list[0];
            config.setDateTime = DateTime.ParseExact(child1.InnerText, "yyyy-MM-dd-HH-mm-ss-fff",
                  System.Globalization.CultureInfo.InvariantCulture);

            // set ID
            list = configElement.GetElementsByTagName(setIDTagName);
            if (list.Count <= 0)
                return false;

            XmlElement child2 = (XmlElement)list[0];
            config.setID = Guid.Parse(child2.InnerText);//UInt32.Parse(child2.InnerText);

            // set version
            list = configElement.GetElementsByTagName(productVersionTagName);
            if (list.Count >= 1)
            {
                // if version config not found directly apply and use current product version
                XmlElement child3 = (XmlElement)list[0];
                config.thisVersionString = child3.InnerText;//UInt32.Parse(child2.InnerText);
            }
            importVersionInfo(ref config, config.thisVersionString);

            // last modified entry
            list = configElement.GetElementsByTagName(lastModifiedEntryTagName);
            if (list.Count > 0)
            {
                XmlElement child4 = (XmlElement)list[0];
                config.lastModifiedEntry = Int64.Parse(child4.InnerText);
            }
            else
            {
                config.lastModifiedEntry = 0;
            }

            // last created entry
            list = configElement.GetElementsByTagName(latestCreatedEntryTagName);
            if (list.Count > 0)
            {
                XmlElement child5 = (XmlElement)list[0];
                config.latestCreatedEntry = Int64.Parse(child5.InnerText);
            }
            else
            {
                config.latestCreatedEntry = 0;
            }
            return true;
        }

        public static String? toXml(DatabaseConfig config)
        {
            //xml Decalration:
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            XmlElement root = doc.DocumentElement;

            // create entry
            XmlElement configElement = doc.CreateElement(string.Empty, configTagName, string.Empty);
            doc.AppendChild(configElement);

            XmlElement child0 = doc.CreateElement(string.Empty, setNameTagName, string.Empty);
            child0.InnerText = config.setName;
            configElement.AppendChild(child0);
            XmlElement child1 = doc.CreateElement(string.Empty, setDateTimeTagName, string.Empty);
            child1.InnerText = config.setDateTime.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            configElement.AppendChild(child1);
            XmlElement child2 = doc.CreateElement(string.Empty, setIDTagName, string.Empty);
            child2.InnerText = config.setID.ToString();
            configElement.AppendChild(child2);
            XmlElement child3 = doc.CreateElement(string.Empty, productVersionTagName, string.Empty);
            child3.InnerText = DatabaseConfig.currentVersion;
            configElement.AppendChild(child3);

            // db entry config
            XmlElement child4 = doc.CreateElement(string.Empty, lastModifiedEntryTagName, string.Empty);
            child4.InnerText = config.lastModifiedEntry.ToString();
            configElement.AppendChild(child4);
            XmlElement child5 = doc.CreateElement(string.Empty, latestCreatedEntryTagName, string.Empty);
            child5.InnerText = config.latestCreatedEntry.ToString();
            configElement.AppendChild(child5);

            String? output = "";
            using (TextWriter writer = new StringWriterWithEncoding(Encoding.UTF8))
            {
                doc.Save(writer);
                output = writer.ToString();
            }
            return output;
        }

        public static String? toXmlFile(DatabaseConfig config, String file)
        {
            String? result = "";
            try
            {
                using (Stream s = new FileStream(file, FileMode.Create, FileAccess.ReadWrite))
                {
                    result = toXmlFile(config, s);
                }
            }
            catch (Exception) { }

            return result;
        }

        public static String? toXmlFile(OpenFSDBContext? ctx, DatabaseConfig config, String file)
        {
            if (ctx.readOnly) return null;
            return toXmlFile(config, file);
        }

        public static String? toXmlFile(DatabaseConfig config, Stream s)
        {
            String? xml = toXml(config);
            using (StreamWriter writer = new StreamWriter(s))
            {
                writer.Write(xml);
                writer.Flush();
                s.Flush();
            }
            return xml;
        }
        public static String? toXmlFile(DatabaseConfig config)
        {
            String? xml = toXml(config);
            return xml;
        }
        public static String? toXmlFileLocal(DatabaseConfig config, String file)
        {
            using (FileStream s = new FileStream(file, FileMode.CreateNew))
                return toXmlFile(config, s);
        }


        public sealed class StringWriterWithEncoding : StringWriter
        {
            private readonly Encoding encoding;

            public StringWriterWithEncoding() : this(Encoding.UTF8) { }

            public StringWriterWithEncoding(Encoding encoding)
            {
                this.encoding = encoding;
            }

            public override Encoding Encoding
            {
                get { return encoding; }
            }
        }

    }
}

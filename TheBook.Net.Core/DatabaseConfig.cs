using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TheBook.Net.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DiaryJournal.Net
{
    
    public class DatabaseConfig
    {
        public String setName { get; set; } = "";
        public DateTime setDateTime { get; set; }
        public Guid setID { get; set; } = Guid.Empty;

        public UInt32 lastModifiedEntry { get; set; } = 0;
        public UInt32 latestCreatedEntry { get; set; } = 0;

        public string thisVersionString { get; set; } = Application.ProductVersion;
        public int thisMajorVersion { get; set; } = 0;
        public int thisMinorVersion { get; set; } = 0;
        public int thisRevision { get; set; } = 0;
        public int thisBuild { get; set; } = 0;
        public static string currentVersion { get; set; } = Application.ProductVersion;
        public int currentMajorVersion { get; set; } = 0;
        public int currentMinorVersion { get; set; } = 0;
        public int currentRevision { get; set; } = 0;
        public int currentBuild { get; set; } = 0;

        public DatabaseConfig()
        {
            String[] values = currentVersion.Split(".");
            thisMajorVersion = currentMajorVersion = int.Parse(values[0]);
            thisMinorVersion = currentMinorVersion = int.Parse(values[1]);
            thisRevision = currentRevision = int.Parse(values[2]);
            thisBuild = currentBuild = int.Parse(values[3]);
        }

        public static bool importVersionInfo(DatabaseConfig config, string versionString)
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
        public static void setDBConfig(DatabaseConfig config, String setName, DateTime setDateTime, Guid setID)
        {
            config.setName = setName;
            config.setDateTime = setDateTime;
            config.setID = setID;
        }

        public static String? toYaml(DatabaseConfig config)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(config);
            return yaml;
        }
        public static DatabaseConfig? fromYaml(String yaml)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();

            //yml contains a string containing your YAML
            var obj = deserializer.Deserialize<DatabaseConfig>(yaml);
            return obj;
        }
        public static DatabaseConfig? fromYamlFile(String file)
        {
            if (!File.Exists(file)) return null;
            String yaml = File.ReadAllText(file);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();

            //yml contains a string containing your YAML
            var obj = deserializer.Deserialize<DatabaseConfig>(yaml);
            return obj;
        }

        public static String? toYamlFile(DatabaseConfig config, String file)
        {
            String? result = "";
            String? yaml = toYaml(config);
            File.WriteAllText(file, yaml);
            return result;
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

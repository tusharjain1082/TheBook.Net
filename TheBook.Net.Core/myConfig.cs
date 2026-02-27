using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DiaryJournal.Net
{
    public class myConfig
    {
        // other config
        public bool radCfgLMNode { get; set; } = true;
        public bool radCfgLCNode { get; set; } = false;
        public bool radCfgTCNode { get; set; } = false;
        public int cmbCfgRtbViewEntryRMValue { get; set; } = 800;//1500;
        
        public const int default_cmbCfgRtbViewEntryRMValue = 800;//1500;
        public const int defaultDocumentWidth = 800;
    }

    public static class myConfigMethods
    {
        public const string myConfigFileName = "myConfig.yaml";

        public static String getConfigPathFile()
        {
            return Path.Combine(Application.StartupPath, Path.GetFileName(myConfigFileName));
        }

        public static String? toYamlFile(myConfig config, String file)
        {
            String? result = "";
            String? yaml = toYaml(config);
            File.WriteAllText(file, yaml);
            return result;
        }
        public static myConfig? fromYamlFile(String file)
        {
            if (!File.Exists(file)) return null;
            String yaml = File.ReadAllText(file);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();

            //yml contains a string containing your YAML
            var obj = deserializer.Deserialize<myConfig>(yaml);
            return obj;
        }
        public static String? toYaml(myConfig config)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(config);
            return yaml;
        }

        public static myConfig? fromYaml(String yaml)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();

            //yml contains a string containing your YAML
            var obj = deserializer.Deserialize<myConfig>(yaml);
            return obj;
        }
    }
}

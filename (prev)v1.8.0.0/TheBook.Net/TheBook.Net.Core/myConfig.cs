using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using SharpConfig;
using System.IO;

namespace DiaryJournal.Net
{
    public class myConfig
    {
        // other config
        public bool radCfgLMNode = true;
        public bool radCfgLCNode = false;
        public bool radCfgTCNode = false;

        public bool chkCfgAutoLoadCreateDefaultDB = true;
        public int cmbCfgRtbViewEntryRMValue = 800;//1500;
        public static int default_cmbCfgRtbViewEntryRMValue = 800;//1500;
        public String configFilePath = "";

        public const int defaultDocumentWidth = 800;

        public TemplateFormat templateFormat = new TemplateFormat();

        public void close()
        {
            //ctx0.close();
            //ctx1.close();
            templateFormat = new TemplateFormat();
        }
    }
    public enum DatabaseType : byte
    {
        OpenFSDB = 1
    }

    public static class myConfigMethods
    {
        public const string myConfigFileName = "myConfig.cfg";

        public static String getConfigPathFile()
        {
            return Path.Combine(Application.StartupPath, Path.GetFileName(myConfigFileName));
        }
        public static bool saveConfigFile(String file, ref myConfig cfg, bool initNewConfig = false)
        {
            if (initNewConfig)
                cfg = new myConfig();

            try
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
            catch
            {
                return false;
            }

            try
            {
                // Create the configuration.
                Configuration config = new Configuration();
                Section config1V1000 = new Section("Config1Version1.0.0.0");

                // prepare default config wherever required
                config1V1000.Add(new Setting("chkCfgAutoLoadCreateDefaultDB", cfg.chkCfgAutoLoadCreateDefaultDB));
                config1V1000.Add(new Setting("cmbCfgRtbViewEntryRMValue", cfg.cmbCfgRtbViewEntryRMValue));
                config1V1000.Add(new Setting("radCfgLMNode", cfg.radCfgLMNode));
                config1V1000.Add(new Setting("radCfgLCNode", cfg.radCfgLCNode));
                config1V1000.Add(new Setting("radCfgTCNode", cfg.radCfgTCNode));

                config.Add(config1V1000);
                config.SaveToFile(file);
                cfg.configFilePath = file;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool loadConfigFile(String file, ref myConfig cfg)
        {
            cfg = new myConfig();

            try
            {
                // Create the configuration.
                Configuration config = Configuration.LoadFromFile(file);
                if (config == null)
                    return false;

                Section config1V1000 = config["Config1Version1.0.0.0"];
                cfg.chkCfgAutoLoadCreateDefaultDB = config1V1000["chkCfgAutoLoadCreateDefaultDB"].BoolValue;
                cfg.cmbCfgRtbViewEntryRMValue = config1V1000["cmbCfgRtbViewEntryRMValue"].IntValue;
                cfg.radCfgLMNode = config1V1000["radCfgLMNode"].BoolValue;
                cfg.radCfgLCNode = config1V1000["radCfgLCNode"].BoolValue;
                cfg.radCfgTCNode = config1V1000["radCfgTCNode"].BoolValue;
                cfg.configFilePath = file;

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public static bool autoCreateLoadConfigFile(ref myConfig cfg, bool initNewConfig = false)
        {
            String file = getConfigPathFile();

            if (initNewConfig)
            {
                if (!saveConfigFile(file, ref cfg, true))
                    return false;

                if (!loadConfigFile(file, ref cfg))
                    return false;
            }
            else
            {
                if (!File.Exists(file))
                {
                    if (!saveConfigFile(file, ref cfg, false))
                        return false;
                }

                if (!loadConfigFile(file, ref cfg))
                    return false;
            }
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PictureThief.Interfaces
{
    public class ConfigureItem
    {
        public string Name = string.Empty;
        public bool Update = true;
        public string Url = string.Empty;

        public ConfigureItem(XmlElement ele)
        {
            Name = ele.GetAttribute("Name");
            Url = ele.GetAttribute("Url");
            string update = ele.GetAttribute("Update");
            Update = string.IsNullOrEmpty(update) ? true : (update.Equals("False", StringComparison.OrdinalIgnoreCase) ? false : true);
        }
    }

    public class ConfigureGroup
    {
        public string Name;
        public List<ConfigureItem> ConfigureItemList = new List<ConfigureItem>();

        public ConfigureGroup(XmlElement ele)
        {
            Name = ele.GetAttribute("Name");

            XmlNodeList nodeList = ele.SelectNodes("ConfigureItem");

            foreach (XmlNode node in nodeList)
            {
                ConfigureItemList.Add(new ConfigureItem(node as XmlElement));
            }
        }
    }

    public class ConfigureMain
    {
        public string Name;
        public List<ConfigureGroup> ConfigureGroupList = new List<ConfigureGroup>();

        public ConfigureMain(XmlElement ele)
        {
            Name = ele.GetAttribute("Name");

            XmlNodeList nodeList = ele.SelectNodes("ConfigureGroup");

            foreach (XmlNode node in nodeList)
            {
                ConfigureGroupList.Add(new ConfigureGroup(node as XmlElement));
            }
        }
    }

    public class PTConfigure
    {
        public Dictionary<string, ConfigureMain> ConfigureMainList = new Dictionary<string, ConfigureMain>();

        public string SaveRoot = AppDomain.CurrentDomain.BaseDirectory;

        public bool LoadConfigure()
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\PTConfigure.xml");

                XmlElement xmlRoot = xmldoc.SelectSingleNode("PTConfigure") as XmlElement;

                string saveRoot = xmlRoot.GetAttribute("SaveRoot");

                if (!string.IsNullOrEmpty(saveRoot))
                {
                    SaveRoot = saveRoot;
                }

                XmlNodeList nodeList = xmldoc.SelectNodes("PTConfigure/ConfigureMain");

                foreach (XmlNode node in nodeList)
                {
                    ConfigureMain cm = new ConfigureMain(node as XmlElement);
                    ConfigureMainList.Add(cm.Name, cm);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.ToString());
                return false;
            }
        }
    }
}

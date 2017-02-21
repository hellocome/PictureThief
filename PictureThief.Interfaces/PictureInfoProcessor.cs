using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace PictureThief.Interfaces
{
    public class PicInfoProcessor : IJob
    {
        private static readonly int Retry = 3;

        public string Pic1Url = string.Empty;
        public string Pic2Url = string.Empty;
        public string PicDetail
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, string> kvp in PicDetailPair)
                {
                    sb.AppendLine(string.Format("{0}: {1}", kvp.Key, kvp.Value));
                }

                return sb.ToString();
            }
        }
        public List<KeyValuePair<string, string>> PicDetailPair = new List<KeyValuePair<string, string>>();
        public string Pic1LocalName
        {
            get
            {
                string name = Path.GetInvalidFileNameChars().Aggregate(GetFileName(Pic1Url), (current, c) => current.Replace(c.ToString(), string.Empty));
                //name += DateTime.Now.ToString("_yyyyMMddHHmmssfff");

                if (!name.Contains("."))
                {
                    name += ".jpg";
                }

                return name;
            }
        }
        public string Pic2LocalName
        {
            get
            {
                string name = GetFileName(Pic2Url);
                name = Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c.ToString(), string.Empty));
                //name += DateTime.Now.ToString("_yyyyMMddHHmmssfff");

                if (!name.Contains("."))
                {
                    name += ".jpg";
                }

                return name;
            }
        }
        private static string GetFileName(string Url)
        {
            return Url.Substring(Url.LastIndexOf("/") + 1, (Url.Length - Url.LastIndexOf("/") - 1));
        }
        public string Pic1SaveAs;
        public string Pic2SaveAs;
        public int ID = 0;

        public PageInfo ParentPageInfo
        {
            get;
            private set;
        }

        public DirectoryInfo SaveToDirectory
        {
            get;
            private set;
        }

        public PicInfoProcessor(PageInfo pi, DirectoryInfo std)
        {
            ParentPageInfo = pi;
            SaveToDirectory = std;
        }

        public void Process()
        {
            Logger.Instance.Info(string.Format("************************ Processing Job = " + this.ID));
            string dir = SaveToDirectory.FullName;

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            this.Pic1SaveAs = dir + "\\" + this.Pic1LocalName;
            this.Pic2SaveAs = dir + "\\" + this.Pic2LocalName;


            int failCount = 0;
            while (failCount < Retry && !this.Pic1LocalName.Equals("no-244x280-image.jpg", StringComparison.OrdinalIgnoreCase))
            {
                if (!DownloadUrl(this.Pic1Url, this.Pic1SaveAs, ParentPageInfo.UpdateMode))
                {
                    failCount++;
                    Logger.Instance.Error(string.Format("Download Failed! [{0}] = {1}", failCount, this.Pic1Url));
                }
                else
                {
                    break;
                }
            }

            failCount = 0;
            while (failCount < Retry && !this.Pic2LocalName.Equals("no-244x280-image.jpg", StringComparison.OrdinalIgnoreCase))
            {
                if (!DownloadUrl(this.Pic2Url, this.Pic2SaveAs, ParentPageInfo.UpdateMode))
                {
                    failCount++;
                    Logger.Instance.Error(string.Format("Download Failed! [{0}] = {1}", failCount, this.Pic2Url));
                }
                else
                {
                    break;
                }
            }

            ParentPageInfo.UpdateRecords(ToXMLString(ParentPageInfo.xmlDoc));
            ParentPageInfo.SaveRecords();
        }

        private static bool DownloadUrl(string url, string saveTo, bool updateMode)
        {
            try
            {
                string saveToLoc = saveTo;

                if (File.Exists(saveToLoc) && !updateMode)
                {
                    Logger.Instance.InfoImportant("Over write mode, Delete: " + new FileInfo(saveToLoc).Name);
                    File.Delete(saveToLoc);
                }
                else if (File.Exists(saveToLoc) && updateMode)
                {
                    Logger.Instance.InfoImportant("Update Mode, File Exists: " + new FileInfo(saveToLoc).Name);
                }

                if (FileDownloadUtil.DownloadFile(url, saveToLoc))
                {
                    Logger.Instance.Info(string.Format("Dwonload OK: {0}", url));
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }

        public XmlElement ToXMLString(XmlDocument xmlDoc)
        {
            XmlElement picItem = xmlDoc.CreateElement("PictureItem");
            picItem.SetAttribute("ID", ID.ToString());
            picItem.SetAttribute("Pic1Url", Pic1Url);
            picItem.SetAttribute("Pic2Url", Pic2Url);
            picItem.SetAttribute("PicDetail", PicDetail.ToString());
            picItem.SetAttribute("pic1LocalName", Pic1LocalName);
            picItem.SetAttribute("pic2LocalName", Pic2LocalName);

            return picItem;
        }
    }

    public class PageInfo
    {
        public string ConfigureGroupName = string.Empty;
        public string ConfigureItemName = string.Empty;
        public string SaveDirectory = string.Empty;
        public string Url = string.Empty;
        public bool UpdateMode = false;
        public List<PicInfoProcessor> PicInfoList = new List<PicInfoProcessor>();

        public XmlDocument xmlDoc = new XmlDocument();
        XmlNode xmlRoot = null; 
        public PageInfo()
        {
            xmlDoc.LoadXml("<Records></Records>");
            xmlRoot = xmlDoc.SelectSingleNode("Records");
        }

        internal void UpdateRecords(XmlElement xmlEle)
        {
            xmlRoot.AppendChild(xmlEle);
        }

        internal void SaveRecords()
        {
            xmlDoc.Save(SaveDirectory + "\\Records.xml");
        }
    }
}

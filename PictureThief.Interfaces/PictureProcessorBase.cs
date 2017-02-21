using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;

namespace PictureThief.Interfaces
{
    public abstract class PictureProcessorBase : IPictureProcessor, IJob
    {
        private static readonly int Retry = 3;

        private string mName = string.Empty;
        private DirectoryInfo mSaveToDirectory;
        private bool mUpdateMode;
        private List<PageInfo> mPageInfoList = new List<PageInfo>();
        protected PTConfigure configure = new PTConfigure();
        protected string ConfigureMainName = string.Empty;

        public PictureProcessorBase(string configureMainName)
        {
            this.LoadConfigure();
            this.ConfigureMainName = configureMainName;
        }

        public string Name
        {
            get
            {
                return mName;
            }

            set
            {
                mName = value;
            }
        }
        public DirectoryInfo SaveToDirectory
        {
            get
            {
                return mSaveToDirectory;
            }

            set
            {
                mSaveToDirectory = value;
            }
        }
        public bool UpdateMode
        {
            get
            {
                return mUpdateMode;
            }

            set
            {
                mUpdateMode = value;
            }
        }

        public List<PageInfo> PageInfoList
        {
            get
            {
                return mPageInfoList;
            }
        }

        protected bool LoadConfigure()
        {
            return configure.LoadConfigure();
        }

        public bool Load()
        {
            if (this.configure.ConfigureMainList.ContainsKey(ConfigureMainName))
            {
                ConfigureMain cm = this.configure.ConfigureMainList[ConfigureMainName];

                foreach (ConfigureGroup cg in cm.ConfigureGroupList)
                {
                    foreach (ConfigureItem ci in cg.ConfigureItemList)
                    {
                        this.UpdateMode = ci.Update;

                        PageInfo pi = new PageInfo();
                        pi.Url = ci.Url;
                        pi.ConfigureGroupName = cg.Name;
                        pi.ConfigureItemName = ci.Name;

                        mSaveToDirectory = new DirectoryInfo(this.configure.SaveRoot + "\\" + cg.Name+ "\\" + ci.Name);

                        pi.SaveDirectory = mSaveToDirectory.FullName;

                        mPageInfoList.Add(pi);
                    }
                }
                
                return true;
            }

            return false;
        }

        public void PicProcess()
        {
            foreach (PageInfo pi in PageInfoList)
            {
                Encoding encoding = null;
                string content = string.Empty;

                if (ProcessIndex(pi, ref encoding, out content))
                {
                    PareparePicInfo(pi, content);
                }

                if (pi.PicInfoList != null)
                {
                    foreach (PicInfoProcessor pip in pi.PicInfoList)
                    {
                        PictureProcessorThreadPool.Instance.AddNewJob(pip);
                    }
                }
            }
        }

        public void Process()
        {
            PicProcess();
        }

        protected abstract bool PareparePicInfo(PageInfo pi, string content);

        protected bool ProcessIndex(PageInfo pi, ref Encoding encoding, out string content)
        {
            content = string.Empty;

            int count = 0;
            bool res = false;

            while (count < Retry)
            {
                res = FileDownloadUtil.DownloadWebPage(pi.Url, ref content, ref encoding);

                if (res)
                {
                    break;
                }

                Logger.Instance.Error(string.Format("Download Page Failed! [{0}] = Index", count));
                count++;
            }

            if (!res)
            {
                Logger.Instance.Error("Download Index Failed!");
                return false;
            }
            else
            {
                Logger.Instance.Info("Download Index OK!");
                return true;
            }
        }
    }
}

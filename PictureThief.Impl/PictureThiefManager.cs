using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PictureThief.Interfaces;

namespace PictureThief.Impl
{
    public class PictureThiefManager
    {
        private static PictureThiefManager instance = null;
        public static PictureThiefManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PictureThiefManager();
                    instance.Register();
                    instance.Load();
                }

                return instance;
            }
        }

        public static Dictionary<string, IPictureProcessor> IPPList = new Dictionary<string, IPictureProcessor>();
        private void Register()
        {
            IPPList.Add("MyWardrobe", new PTMyWardrobe());
            IPPList.Add("Reiss", new PTReiss());
        }

        public void Load()
        {
            foreach (string ippKey in IPPList.Keys)
            {
                IPPList[ippKey].Load();
            }
        }

        public void Start(string key)
        {
            if (IPPList.ContainsKey(key))
            {
               IJob job = IPPList[key] as IJob;

               if (job != null)
               {
                   PictureProcessorThreadPool.Instance.AddNewJob(job);
               }
            }
        }
    }
}

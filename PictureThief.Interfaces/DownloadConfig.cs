using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace PictureThief.Interfaces
{
    public static class DownloadConfig
    {
        private static string mRegistryRoot = "HKEY_LOCAL_MACHINE\\SOFTWARE\\FileDownloadConfig\\";

        private static string mProtocol = string.Empty;
        public static string Protocol
        {
            get
            {
                return mProtocol;
            }

            set
            {
                Logger.Instance.Info(string.Format("Set Protocol Manually, [{0}]", value));
                mProtocol = value;
            }
        }

        private static int mWebTimeout= 0;
        public static int WebTimeout
        {
            get
            {
                return mWebTimeout;
            }
        }

        static DownloadConfig()
        {
            mProtocol = (string)GetValue(mRegistryRoot, "Protocol", "http");
            mWebTimeout = (int)GetValue(mRegistryRoot, "WebTimeout", 10000);
        }

        public static object GetValue(string root, string key, object defaultValue)
        {
            try
            {
                object newValue = null;

#if !SILVERLIGHT
                newValue = Registry.GetValue(root, key, defaultValue);
#endif

                if (newValue != null)
                {
                    return newValue;
                }
            }
            catch (Exception)
            {
                
            }

            return defaultValue;
        }
    }
}

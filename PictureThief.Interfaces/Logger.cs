using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PictureThief.Interfaces
{
    public delegate void WriteLogDelegate(Object color, string log);

    public class Logger
    {
        public bool inited = false;
        private static Logger instance;
        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Logger();
                }

                return instance;
            }
        }

        Object mColorError, mColorInfo, mColorInfoImportant;

        private WriteLogDelegate mWriteLog = null;
        public void Init(WriteLogDelegate writeLog, Object colorError, Object colorInfo, Object colorInfoImportant)
        {
            mWriteLog = writeLog;
            inited = true;

            mColorError = colorError;
            mColorInfo = colorInfo;
            mColorInfoImportant = colorInfoImportant;

            Info("Log Init OK");
        }

        public void Init(WriteLogDelegate writeLog)
        {
            Init(writeLog, null, null, null);
        }

        protected void WriteFormattedLog(Object color, string level, string logContent)
        {
            try
            {
                if (!inited)
                {
                    return;
                }

                mWriteLog(color, string.Format("{0}  {1,-8}  {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), level, logContent));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public virtual void Error(string logContent)
        {
            WriteFormattedLog(mColorError, "Error", logContent);
        }

        public virtual void Info(string logContent)
        {
            WriteFormattedLog(mColorInfo, "Info", logContent);
        }

        public virtual void InfoImportant(string logContent)
        {
            WriteFormattedLog(mColorInfoImportant, "Info", logContent);
        }
    }
}

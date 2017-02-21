using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace PictureThief.Interfaces
{

    public class PictureProcessorThreadPool
    {
        private int count = 0;
        private static ManualResetEvent mre = new ManualResetEvent(false);

        protected string CurrentUrl = string.Empty;
        protected string HttpMainHost = string.Empty;

        protected virtual bool LoadConfigure()
        {
            return true;
        }

        private bool mStop = false;
        protected bool StopJob
        {
            get { return mStop; }
            private set
            {
                mStop = value;
            }
        }

        private static PictureProcessorThreadPool instance = null;
        public static PictureProcessorThreadPool Instance
        {
            get{
                if (instance == null)
                {
                    instance = new PictureProcessorThreadPool();
                    instance.StartThreadPool();
                }

                return instance;
            }
        }

        private ConcurrentQueue<IJob> indexQueue = new ConcurrentQueue<IJob>();
        public void AddNewJob(IJob job)
        {
            lock (indexQueue)
            {
                indexQueue.Enqueue(job);
                mre.Set();
            }
        }


        public virtual void Stop()
        {
            Logger.Instance.Info("Stop is trigged, but it might take a while to stop");
            StopJob = true;
            mre.Set();
        }

        Dictionary<int, bool> threadPools = new Dictionary<int, bool>();
        public void StartThreadPool()
        {
            if (threadPools.Count <= 0)
            {
                for (int i = 0; i < 1; i++)
                {
                    System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(Run));
                    thread.Start();
                }
            }
        }

        protected virtual void Run()
        {
            while (!StopJob)
            {
                mre.WaitOne();

                IJob job = null;

                lock (indexQueue)
                {
                    if (!indexQueue.TryDequeue(out job))
                    {
                        if (indexQueue.Count == 0)
                        {
                            mre.Reset();
                        }

                        continue;
                    }
                }

                count++;

                Logger.Instance.Info("Processing Record: " + count);
                Logger.Instance.Info("Records In Queue: " + indexQueue.Count);
                job.Process();
            }
        }
    }
}

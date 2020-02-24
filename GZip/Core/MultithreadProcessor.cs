using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GZip.Core
{
    public abstract class MultithreadProcessor
    {
        protected CancellationTokenSource CancellationTokenSource;
        protected object lockObject = new object();

        public abstract int GetThreadCount();

        public virtual void Processing(object threadParam)
        {
            CancellationTokenSource = new CancellationTokenSource();

            var threadCount = GetThreadCount();

            for (int i = 0; i < threadCount; i++)
            {
                if (CancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }

                var thread = new Thread(() => ThreadProcessing(threadParam));
                thread.Start();
            }
        }

        public abstract void ThreadProcessing(object threadParam);
    }
}

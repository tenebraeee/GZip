using System;
using System.Threading;

namespace GZip.Core
{
    public abstract class BaseGZip : MultithreadProcessor
    {
        public virtual event Action<string> OnError;

        public ManualResetEvent ManualResetEvent;
        protected bool IsCompleted { get; set; }
        protected uint defaultBufferSize = 1024 * 1024 * 5;

        public override int GetThreadCount()
        {
            return Environment.ProcessorCount;
        }
    }
}

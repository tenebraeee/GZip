using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace GZip.Core
{
    public abstract class BaseGZip : MultithreadProcessor
    {
        public virtual event Action<string> OnError;

        public ManualResetEvent ManualResetEvent;

        protected FileStream source;
        protected FileStream target;
        protected BufferedStream bufferedStream;
        protected GZipStream gzipStream;

        protected bool IsCompleted { get; set; }
        protected uint defaultBufferSize = 1024 * 1024 * 5;

        public override int GetThreadCount()
        {
            return Environment.ProcessorCount;
        }
    }
}

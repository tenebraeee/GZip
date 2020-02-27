using GZip.Core;
using GZip.Models.Common;
using System;
using System.Threading;

namespace GZip.Operations
{
    public class CompressOperation : IOperation
    {
        public event Action<string> OnError;
        public ManualResetEvent ManualResetEvent;

        public CompressOperation() { }
        public CompressOperation(Action<string> onError, ManualResetEvent manualResetEvent) 
        {
            OnError = onError;
            ManualResetEvent = manualResetEvent;
        }

        public void Execute(Options options)
        {            
            var compressor = new GZipFileCompressor();
            compressor.OnError += OnError;
            compressor.ManualResetEvent = ManualResetEvent;
            compressor.Compress(options);
        }
    }
}

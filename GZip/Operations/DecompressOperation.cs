using GZip.Core;
using GZip.Models.Common;
using System;
using System.Threading;

namespace GZip.Operations
{
    public class DecompressOperation : IOperation
    {
        public event Action<string> OnError;

        public ManualResetEvent ManualResetEvent;
        public DecompressOperation(Action<string> onError, ManualResetEvent manualResetEvent)
        {
            OnError = onError;
            ManualResetEvent = manualResetEvent;
        }

        public DecompressOperation() { }

        public void Execute(Options options)
        {
            var decompressor = new GZipFileDecompressor();
            decompressor.OnError += OnError;
            decompressor.ManualResetEvent = ManualResetEvent;
            decompressor.Decompress(options);
        }
    }
}

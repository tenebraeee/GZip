using GZip.Core;
using GZip.Models.Common;
using System;

namespace GZip.Operations
{
    public class CompressOperation : IOperation
    {
        public void Execute(Options options)
        {
            var compressor = new GZipFileCompressor();
            compressor.Compress(options);
        }
    }
}

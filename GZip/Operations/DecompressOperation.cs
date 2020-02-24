using GZip.Core;
using GZip.Models.Common;

namespace GZip.Operations
{
    public class DecompressOperation : IOperation
    {
        public void Execute(Options options)
        {
            var compressor = new GZipFileDecompressor();
            compressor.Decompress(options);
        }
    }
}

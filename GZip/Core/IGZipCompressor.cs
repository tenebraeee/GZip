using GZip.Models.Common;

namespace GZip.Core
{
    public interface IGZipCompressor
    {
        void Compress(Options compressOptions);
    }
}

using GZip.Models.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace GZip.Core
{
    public interface IGZipDecompressor
    {
        void Decompress(Options decompressOptions);
    }
}

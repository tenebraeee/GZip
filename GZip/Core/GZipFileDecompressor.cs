using GZip.Models.Common;
using GZip.Utils;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace GZip.Core
{
    /// <summary>
    /// Деархиватор
    /// </summary>
    public class GZipFileDecompressor : BaseGZip, IGZipDecompressor
    {
        public void Decompress(Options decompressOptions)
        {
            if (decompressOptions == null)
            {
                throw new ArgumentNullException(nameof(decompressOptions));
            }

            if (decompressOptions.BufferSize == default)
            {
                decompressOptions.BufferSize = defaultBufferSize;
            }

            Processing(decompressOptions);
        }

        public override void ThreadProcessing(object threadParam)
        {
            if (CancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            var decompressOptions = threadParam as Options;

            using FileStream stream = new FileStream(decompressOptions.SourceFilePath, FileMode.Open, FileAccess.Read);
            using FileStream target = new FileStream(decompressOptions.TargetFilePath, FileMode.Create, FileAccess.Write, FileShare.Write);
            using GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress);
            using BufferedStream bufferedStream = new BufferedStream(gzipStream);

            bufferedStream.CopyWithChunkTo(target, decompressOptions.BufferSize, lockObject);


            CancellationTokenSource.Cancel();
        }
    }
}

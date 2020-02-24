using GZip.Models.Common;
using GZip.Utils;
using System;
using System.IO;
using System.IO.Compression;

namespace GZip.Core
{
    /// <summary>
    /// Архиватор
    /// </summary>
    public class GZipFileCompressor : BaseGZip, IGZipCompressor
    {
        public void Compress(Options compressOptions)
        {
            if (compressOptions == null)
            {
                throw new ArgumentNullException(nameof(compressOptions));
            }

            if(compressOptions.BufferSize == default)
            {
                compressOptions.BufferSize = defaultBufferSize;
            }

            Processing(compressOptions);
        }

        public override void ThreadProcessing(object threadParam)
        {
            if (CancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            var compressOptions = threadParam as Options;

            using FileStream stream = new FileStream(compressOptions.SourceFilePath, FileMode.Open, FileAccess.Read);
            using FileStream target = new FileStream(compressOptions.TargetFilePath, FileMode.Create, FileAccess.Write, FileShare.Write);
            using BufferedStream bufferedStream = new BufferedStream(stream);
            using GZipStream gzipStream = new GZipStream(target, CompressionLevel.Fastest);

            bufferedStream.CopyWithChunkTo(gzipStream, compressOptions.BufferSize, lockObject);


            CancellationTokenSource.Cancel();
        }
    }
}

using GZip.Models.Common;
using GZip.Utils;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace GZip.Core
{
    /// <summary>
    /// Архиватор
    /// </summary>
    public class GZipFileCompressor : BaseGZip, IGZipCompressor
    {
        public override event Action<string> OnError;
       
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

            IsCompleted = false;

            ManualResetEvent.Reset();

            Processing(compressOptions);
        }

        public override void ThreadProcessing(object threadParam)
        {
            lock (lockObject)
            {
                if (CancellationTokenSource.IsCancellationRequested)
                {
                    return;
                } 
            }

            var compressOptions = threadParam as Options;
            string errorMessage = string.Empty;

            try
            {
                var targetFileDirectory = Path.GetDirectoryName(compressOptions.TargetFilePath);
                var targetFileName = Path.GetFileNameWithoutExtension(compressOptions.TargetFilePath);
                var sourceFileExtension = Path.GetExtension(compressOptions.SourceFilePath);

                var newTargetFilePath = $"{Path.Combine(targetFileDirectory, targetFileName)}{sourceFileExtension}.gz";

                lock (lockObject)
                {
                    using (source = new FileStream(compressOptions.SourceFilePath, FileMode.Open, FileAccess.Read))
                    using (target = new FileStream(newTargetFilePath, FileMode.Create, FileAccess.Write, FileShare.Write))
                    using (bufferedStream = new BufferedStream(source))
                    using (gzipStream = new GZipStream(target, CompressionLevel.Fastest))
                    {
                        bufferedStream.CopyWithChunkTo(gzipStream, compressOptions.BufferSize);
                    } 
                }
            }
            catch (FileNotFoundException ex)
            {
                errorMessage = $"Source file ({ex.FileName}) not found";
            }
            catch (UnauthorizedAccessException ex)
            {
                errorMessage = $"Can`t access to file: {ex.Message}";
            }
            catch (IOException ex)
            {
                errorMessage = $"Error occered while compressing file: {ex.Message}";
            }
            catch (Exception ex)
            {
                errorMessage = $"Not recognized error: {ex.Message}";
            }
            finally
            {
                lock (lockObject)
                {
                    if (!string.IsNullOrWhiteSpace(errorMessage) && !IsCompleted)
                    {
                        OnError?.Invoke(errorMessage);
                        IsCompleted = true;
                    }
                    ManualResetEvent.Set();
                    CancellationTokenSource.Cancel();
                }

            }
        }
    }
}

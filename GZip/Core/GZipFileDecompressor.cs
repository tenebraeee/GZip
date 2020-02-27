using GZip.Models.Common;
using GZip.Utils;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace GZip.Core
{
    /// <summary>
    /// Деархиватор
    /// </summary>
    public class GZipFileDecompressor : BaseGZip, IGZipDecompressor
    {
        public override event Action<string> OnError;

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

            ManualResetEvent.Reset();

            Processing(decompressOptions);
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

            var decompressOptions = threadParam as Options;
            string errorMessage = string.Empty;

            try
            {
                var targetFileDirectory = Path.GetDirectoryName(decompressOptions.TargetFilePath);
                var targetFileName = Path.GetFileNameWithoutExtension(decompressOptions.TargetFilePath);
                var sourceFileExtension = decompressOptions.SourceFilePath.Split('.').Reverse().ElementAt(1);

                var newTargetFilePath = $"{targetFileDirectory}{targetFileName}.{sourceFileExtension}";

                using FileStream stream = new FileStream(decompressOptions.SourceFilePath, FileMode.Open, FileAccess.Read);
                using FileStream target = new FileStream(newTargetFilePath, FileMode.Create, FileAccess.Write, FileShare.Write);
                using GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress);
                using BufferedStream bufferedStream = new BufferedStream(gzipStream);

                bufferedStream.CopyWithChunkTo(target, decompressOptions.BufferSize, lockObject);
            }
            catch (FileNotFoundException ex)
            {
                errorMessage = $"Source file ({ex.FileName}) not found";
            }
            catch (UnauthorizedAccessException ex)
            {
                errorMessage = $"Can`t access to file. Error message: {ex.Message}";
            }
            catch (IOException ex)
            {
                errorMessage = $"Error occured while decompressing file. Error message: {ex.Message}";
            }
            catch (Exception ex)
            {
                errorMessage = $"Not recognized error. Error message: {ex.Message}";
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

using System.IO;
using System.Linq;

namespace GZip.Utils
{
    public static class StreamExtension
    {
        public static void CopyWithChunkTo(this Stream source, Stream target, uint bufferSize, object lockObject = null)
        {
            byte[] buffer = new byte[bufferSize];

            while (true)
            {
                var bytesCount = source.Read(buffer, 0, buffer.Length);

                if (bytesCount == 0)
                {
                    break;
                }

                if(lockObject == null)
                {
                    target.Write(buffer.Take(bytesCount).ToArray(), 0, bytesCount);

                }
                else
                {
                    lock (lockObject)
                    {
                        target.Write(buffer.Take(bytesCount).ToArray(), 0, bytesCount);
                    }
                }
            }
        }

    }
}

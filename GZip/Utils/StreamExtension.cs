using System.IO;
using System.Linq;

namespace GZip.Utils
{
    public static class StreamExtension
    {
        public static void CopyWithChunkTo(this Stream source, Stream target, uint bufferSize)
        {
            byte[] buffer = new byte[bufferSize];

            while (true)
            {
                var bytesCount = source.Read(buffer, 0, buffer.Length);

                if (bytesCount == 0)
                {
                    break;
                }

                var bytes = buffer;

                if (bytesCount != bufferSize)
                {
                    bytes = bytes.Take(bytesCount).ToArray();
                }
                
                target.Write(bytes, 0, bytesCount);
            }
        }

    }
}

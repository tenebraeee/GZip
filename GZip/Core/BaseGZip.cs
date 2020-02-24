using GZip.Models.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace GZip.Core
{
    public abstract class BaseGZip : MultithreadProcessor
    {

        protected uint defaultBufferSize = 1024 * 1024 * 5;

        public override int GetThreadCount()
        {
            return Environment.ProcessorCount;
        }
    }
}

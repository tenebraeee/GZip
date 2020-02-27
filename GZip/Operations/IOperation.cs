using GZip.Models.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace GZip.Operations
{
    public interface IOperation
    {
        event Action<string> OnError;
        void Execute(Options options);
    }
}

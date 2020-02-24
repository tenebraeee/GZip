using GZip.Models.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace GZip.Operations
{
    public interface IOperation
    {
        void Execute(Options options);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Data
{
    public interface IAutoCloseable : IDisposable
    {
        new void Dispose();
    }
}

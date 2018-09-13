using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.Common.Threading
{
    public interface IWorker
    {
        string Name { get; }
        void Start();
        void Stop();
    }
}

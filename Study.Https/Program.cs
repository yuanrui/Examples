using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Study.Https
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerHost host = new ServerHost(22221);
            host.Start();
        }
    }
}

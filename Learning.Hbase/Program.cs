using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Hadoop.Hbase.Thrift2;
using Thrift.Transport;
using Thrift.Protocol;

namespace Learning.Hbase
{
    class Program
    {
        static void Main(string[] args)
        {
            var thriftsocket = new TSocket("192.168.1.40", 8090, 10000);
            var client = new THBaseService.Client(new TBinaryProtocol(thriftsocket));
            thriftsocket.Open();
            var rowKey = new TPut();

            rowKey.Row = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            var col1 = new TColumnValue(null, null, null);
        }
    }
}

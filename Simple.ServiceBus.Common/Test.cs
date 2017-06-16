using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Simple.ServiceBus.Common.Impl
{
    public class Test : IBusData
    {
        public string Input { get; set; }

        public override string ToString()
        {
            return Input;
        }
    }

    //[DataContract, Serializable]
    public class Test1 : IBusData
    {
        //[DataMember]
        public string Id { get; set; }

        //[DataMember]
        public string Url { get; set; }

        //[DataMember]
        public string Img { get; set; }

        //[DataMember]
        public string Code { get; set; }

        public Test1()
        {
            Id = Guid.NewGuid().ToString();
            Url = Guid.NewGuid().ToString();
            Img = Guid.NewGuid().ToString();
            Code = DateTime.Now.ToShortTimeString();
        }

        public override string ToString()
        {
            return Id + "@" + Url + "@" + Img + "@" + Code;
        }
    }

    //[DataContract, Serializable]
    public class Test2 : IBusData
    {
        //[DataMember]
        public Int64 Id { get; set; }

        //[DataMember]
        public DateTime Now { get; set; }

        public Test2()
        {
            Now = DateTime.Now;
            Id = Now.Ticks;
        }

        public override string ToString()
        {
            return Id + "@" + Now;
        }
    }
}

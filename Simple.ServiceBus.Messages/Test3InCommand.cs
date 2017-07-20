using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.ServiceBus.Messages
{
    public class Test3InCommand : ICommand
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public DateTime CreatedAt { get; set; }

        public Int64 Token { get; set; }

        public double Value { get; set; }

        public string Remark { get; set; }

        public byte[] Blob { get; set; }

        public Int32 Index { get; set; }

        public Test3InCommand()
        {
            Id = Guid.NewGuid();
            Name = Guid.NewGuid().ToString();
            Phone = "12345678911";
            Token = DateTime.Now.Ticks;
            Value = new Random().NextDouble() * Token;

            Remark = new string('啊', (Math.Abs(Guid.NewGuid().GetHashCode()) % 4000) + 4000);
            //Remark = new string('啊', 5);
            Blob = new byte[65536];
            CreatedAt = DateTime.Now;
        }

        public override string ToString()
        {
            return string.Format("Id={0} Time={1}", this.Id, this.CreatedAt.ToString("HH:mm:ss"));
        }
    }

    public class Test3OutCommand : ICommand
    {
        public class Test3OutWapper : ICommand
        {
            public Guid Id { get; set; }

            public bool Success { get; set; }

            public string Message { get; set; }
        }

        public List<Test3OutWapper> List { get; set; }

        public Test3OutCommand()
        {
            List = new List<Test3OutWapper>();
        }

        public override string ToString()
        {
            return "List Count = " + List.Count;
        }
    }
}

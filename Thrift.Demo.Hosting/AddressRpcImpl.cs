using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Thrift.Demo.Shared;

namespace Thrift.Demo.Hosting
{
    public class AddressRpcImpl : AddressRpc.Iface
    {
        protected static Int64 id;

        public long GetNewId()
        {
            Interlocked.Increment(ref id);

            return id;
        }

        public bool Exists(long ADDR_ID)
        {
            return true;
        }

        public AddressDTO Get(long ADDR_ID)
        {
            return new AddressDTO { ADDR_ID = id, ADDRESS = ADDR_ID + ":" + Guid.NewGuid().ToString(), 
                REMARK = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                Time = DateTime.Now.AddHours(-1),
                Time2 = DateTime.Now.AddDays(-1)
            };
        }

        public bool Add(AddressDTO dto)
        {
            return true;
        }

        public bool AddMulti(List<AddressDTO> dtoList)
        {
            return true;
        }

        public bool Update(AddressDTO dto)
        {
            return true; 
        }

        public bool UpdateMulti(List<AddressDTO> dtoList)
        {
            return true;
        }

        public bool Save(AddressDTO dto)
        {
            return true;
        }

        public bool SaveMulti(List<AddressDTO> dtoList)
        {
            return true;
        }

        public bool Delete(long ADDR_ID)
        {
            return true;
        }

        public bool DeleteMulti(List<AddressDTO> dtoList)
        {
            return true;
        }


        public bool Add2(List<AddressDTO> list2)
        {
            throw new NotImplementedException();
        }
    }
}

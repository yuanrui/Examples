using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Banana.RPC;

namespace Thrift.Demo.Hosting
{
    public class SmsSendShortMessageRpcImpl : SmsSendShortMessageRpc.Iface
    {
        protected static Int64 id;

        public long GetNewId()
        {
            Interlocked.Increment(ref id);
            id = id + 1000;
            return id;
        }

        public bool Exists(string ID)
        {
            return true;
        }

        public SmsSendShortMessageDTO Get(string ID)
        {
            return new SmsSendShortMessageDTO { 
                ID = ID, 
                CONTENT = Guid.NewGuid().ToString(),
                CATEGORY = 1,
                PLATE_NUMBER = "PLATE_NUMBER",
                EVENT_CODE = "EVENT_CODE",
                CONTACT = "CONTACT",
                RESULT = "RESULT",
                PHONE = "PHONE",
                PECCANCY_TYPE = "PECCANCY_TYPE",
                MODEM_ID = "MODEM_ID",
                IMG_INFO = "IMG_INFO",
                FILE_PATH = "FILE_PATH",
                REMARK = "REMARK",
                PLATE_COLOR = "pc",
                ENTITY_ID = "EntID",
                STATUS = 1,
                SEND_STATE = true,
                SENT_AT = DateTime.Now,
                REQUEST_AT = DateTime.Now,
                SENT_COUNT =2,
                SMS_COUNT = 1,
                SURPLUS_COUNT = 2222,
                CREATED_AT = DateTime.Now,
                CREATED_BY = "by",
                TERMINAL_CODE = "code"
            };
        }

        public bool Add(SmsSendShortMessageDTO dto)
        {
            throw new NotImplementedException();
        }

        public bool AddMulti(List<SmsSendShortMessageDTO> dtoList)
        {
            throw new NotImplementedException();
        }

        public bool Update(SmsSendShortMessageDTO dto)
        {
            throw new NotImplementedException();
        }

        public bool UpdateMulti(List<SmsSendShortMessageDTO> dtoList)
        {
            throw new NotImplementedException();
        }

        public bool Save(SmsSendShortMessageDTO dto)
        {
            throw new NotImplementedException();
        }

        public bool SaveMulti(List<SmsSendShortMessageDTO> dtoList)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string ID)
        {
            throw new NotImplementedException();
        }

        public bool DeleteMulti(List<SmsSendShortMessageDTO> dtoList)
        {
            throw new NotImplementedException();
        }

    }
}

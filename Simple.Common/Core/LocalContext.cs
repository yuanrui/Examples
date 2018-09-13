using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Web;

namespace Simple.Common
{
    public class LocalContext
    {
        public static readonly LocalContext Current = new LocalContext();

        private LocalContext()
        {

        }

        public Object this[Object key]
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Items[key];
                }
                else
                {
                    LocalDataStoreSlot slot = Thread.GetNamedDataSlot(key.ToString());
                    return Thread.GetData(slot);
                }
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[key] = value;
                }
                else
                {
                    LocalDataStoreSlot slot = Thread.GetNamedDataSlot(key.ToString());
                    Thread.SetData(slot, value);
                }
            }
        }
    }
}

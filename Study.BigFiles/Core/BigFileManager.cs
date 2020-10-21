using System;
using System.Collections.Generic;
using System.Text;

namespace Study.BigFiles
{
    public class BigFileManager
    {
        protected Dictionary<String, HostElement> FileGroup;

        public BigFileManager()
        {
            FileGroup = new Dictionary<String, HostElement>();
        }

        public void Register(String key, HostElement elm)
        {
            if (FileGroup.ContainsKey(key))
            {
                return;
            }

            FileGroup.Add(key, elm);
        }

        public String Write(Byte[] buffer)
        {
            //TODO:
            return null;
        }

        public Byte[] Read(String url)
        {
            //TODO
            return null;
        }
    }
}

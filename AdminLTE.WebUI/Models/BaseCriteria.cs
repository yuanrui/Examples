using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminLTE.WebUI.Models
{
    public class BaseCriteria
    {
        public virtual Int32? offset { get; set; }

        public virtual Int32? limit { get; set; }

        public virtual string sort { get; set; }

        public virtual string order { get; set; }

        public virtual string search { get; set; }

        public virtual Int32 PageIndex
        {
            get
            {
                if (!offset.HasValue)
                {
                    offset = 0;
                }

                return (Int32)Math.Ceiling(offset.Value / (Single)PageSize) + 1;
            }
        }

        public virtual Int32 PageSize
        {
            get
            {
                if (!limit.HasValue)
                {
                    limit = 15;
                }

                return limit.Value;
            }
        }
    }
}
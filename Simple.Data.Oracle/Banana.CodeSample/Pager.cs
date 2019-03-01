using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Banana.Entity
{
    public class Pager<T>
    {
        public Int32 PageSize { get; private set; }

        public Int32 PageIndex { get; private set; }

        public Int64 Total { get; private set; }

        public Int32 PageCount { get; private set; }

        public IEnumerable<T> Data { get; private set; }

        public Pager(Int32 pageSize, Int32 pageIndex, Int64 total, IList<T> queryResult)
        {
            if (pageSize == 0)
            {
                throw new ArgumentException("pageSize must > 0", "pageSize");
            }

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.Total = total;
            this.Data = queryResult ?? new List<T>();
            this.PageCount = (Int32)Math.Ceiling(this.Total / (Single)this.PageSize);
        }
    }
}

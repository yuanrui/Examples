using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminLTE.WebUI.Common
{
    public class Pager<T>
    {
        /// <summary>
        /// 页大小
        /// </summary>
        public Int32 PageSize { get; private set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public Int32 PageIndex { get; private set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public Int64 Total { get; private set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public Int32 PageCount { get; private set; }

        /// <summary>
        /// 内容
        /// </summary>
        public IEnumerable<T> Data { get; private set; }

        public Pager(Int32 pageSize, Int32 pageIndex, Int64 total, IList<T> queryResult)
        {
            if (pageSize == 0)
            {
                throw new ArgumentException("pageSize值不能为空", "pageSize");
            }

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.Total = total;
            this.Data = queryResult ?? new List<T>();
            this.PageCount = (Int32)Math.Ceiling(this.Total / (Single)this.PageSize);
        }
    }
}
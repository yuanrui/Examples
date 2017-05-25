using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminLTE.WebUI.Common
{
    public class DataGrid<TRow>
    {
        public long total { get; set; }

        public int pageSize { get; set; }
        
        public int pageNumber { get; set; }
        
        public IEnumerable<TRow> rows { get; set; }
    }

    public static class PagerExtention
    {
        public static DataGrid<TRow> AsDataGrid<TRow>(this Pager<TRow> pager)
        {
            var result = new DataGrid<TRow>();
            result.total = pager.Total;
            result.pageSize = pager.PageSize;
            result.pageNumber = pager.PageIndex;
            result.rows = pager.Data;

            return result;
        }

        public static DataGrid<TRow> AsDataGrid<TSource, TRow>(this Pager<TSource> pager, Func<TSource, TRow> selector)
        {
            var result = new DataGrid<TRow>();
            result.total = pager.Total;
            result.pageSize = pager.PageSize;
            result.pageNumber = pager.PageIndex;
            result.rows = pager.Data != null ? pager.Data.Select(selector) : default(IEnumerable<TRow>);

            return result;
        }
    }
}
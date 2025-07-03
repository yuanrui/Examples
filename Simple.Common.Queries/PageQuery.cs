// Copyright (c) 2023 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Simple.Common.Queries
{
    public class PageQuery : Query
    {
        public virtual int Index { get; set; }

        public virtual int Size { get; set; }

        public virtual List<Criteria> Where { get; set; } = new List<Criteria>();
        public virtual Dictionary<string, string> OrderBy { get; set; } = new Dictionary<string, string>();

        protected override List<Criteria> _where { get => Where; set => Where = value; }

        protected override Dictionary<string, string> _orderBy { get => OrderBy; set => OrderBy = value; }

        public PageQuery() : base("dual", "*", null)
        {

        }

        public PageQuery(string tableName) : base(tableName, "*", null)
        {

        }

        public PageQuery(string tableName, Func<string, string> convertToRealField) : base(tableName, "*", convertToRealField)
        {

        }

        public PageQuery(string tableName, string columnNames, Func<string, string> convertToRealField) : base(tableName, columnNames, convertToRealField)
        {
            
        }

        public override string ToString()
        {
            return base.ToString() + "limit @__Index, @__Size ";
        }

        public override string ToSql(string paramTag = "@")
        {
            return base.ToSql(paramTag) + $"limit {(Index - 1) * Size}, {Size} ";
        }

        public override Dictionary<string, object> GetParamObject()
        {
            var result = base.GetParamObject();
            var offset = (Index - 1) * Size;

            result["__Index"] = offset;
            result["__Size"] = Size;

            return result;
        }
    }
}

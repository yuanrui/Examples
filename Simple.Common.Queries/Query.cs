// Copyright (c) 2023 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace Simple.Common.Queries
{
    public class Query
    {
        private string _columnNames;
        private string _tableName;
        protected virtual List<Criteria> _where { get; set; } = new List<Criteria>();
        protected virtual Dictionary<string, string> _orderBy { get; set; } = new Dictionary<string, string>();

        public Func<string, string> ConvertToRealField;

        public Query() : this("dual", "*", null)
        {

        }

        public Query(string tableName) : this(tableName, "*", null)
        {

        }

        public Query(string tableName, Func<string, string> convertToRealField) : this(tableName, "*", convertToRealField)
        {

        }

        public Query(string tableName, string columnNames, Func<string, string> convertToRealField)
        {
            _tableName = tableName;
            _columnNames = columnNames;
            ConvertToRealField = convertToRealField;
        }

        public virtual Query SetTableName(string tableName)
        {
            _tableName = tableName;

            return this;
        }

        public virtual Query And(Criteria criteria)
        {
            return AndIf(criteria, c => true);
        }

        public virtual Query AndIf(Criteria criteria)
        {
            return AndIf(criteria, c =>
            {
                return !ValueIsNullOrDefaultValue(c);
            });
        }

        public virtual Query AndIf(Criteria criteria, Func<Criteria, bool> checkFunc)
        {
            if (criteria == null)
            {
                return this;
            }

            if (checkFunc != null)
            {
                if (!checkFunc(criteria))
                {
                    return this;
                }
            }

            criteria.AndOr = "and";
            _where.Add(criteria);

            return this;
        }

        public virtual Query Or(Criteria criteria)
        {
            return OrIf(criteria, c => true);
        }

        public virtual Query OrIf(Criteria criteria)
        {
            return OrIf(criteria, c =>
            {
                return !ValueIsNullOrDefaultValue(c);
            });
        }

        public virtual Query OrIf(Criteria criteria, Func<Criteria, bool> checkFunc)
        {
            if (criteria == null)
            {
                return this;
            }

            if (checkFunc != null)
            {
                if (!checkFunc(criteria))
                {
                    return this;
                }
            }

            criteria.AndOr = "or";
            _where.Add(criteria);

            return this;
        }

        public virtual Query Asc(string field)
        {
            var input = SafeSqlLiteral(ConvertToRealField != null ? ConvertToRealField(field) : field);
            _orderBy[input] = "asc";

            return this;
        }

        public virtual Query Desc(string field)
        {
            var input = SafeSqlLiteral(ConvertToRealField != null ? ConvertToRealField(field) : field);
            _orderBy[input] = "desc";

            return this;
        }

        protected virtual bool ValueIsNullOrDefaultValue(Criteria criteria)
        {
            if (criteria == null)
            {
                return true;
            }

            if (criteria.Value == null)
            {
                return true;
            }

            if (criteria.Value is string || criteria.Value is char)
            {
                return string.IsNullOrEmpty(criteria.Value.ToString());
            }

            if (criteria.Value is DateTime)
            {
                return (DateTime)criteria.Value == default(DateTime);
            }

            if (criteria.Value is DateTimeOffset)
            {
                return (DateTimeOffset)criteria.Value == default(DateTimeOffset);
            }

            if (criteria.Value is TimeSpan)
            {
                return (TimeSpan)criteria.Value == default(TimeSpan);
            }

            if (criteria.Value is bool)
            {
                return !(bool)criteria.Value;
            }

            if (criteria.Value is Guid)
            {
                return (Guid)criteria.Value == default(Guid);
            }

            if (criteria.Value is int || criteria.Value is uint
                || criteria.Value is short || criteria.Value is ushort
                || criteria.Value is long || criteria.Value is ulong
                || criteria.Value is byte)
            {
                return (long)criteria.Value == 0L;
            }

            if (criteria.Value is double || criteria.Value is float
                || criteria.Value is decimal)
            {
                return (decimal)criteria.Value == 0;
            }

            return false;
        }

        protected virtual string SafeSqlLiteral(string inputSql)
        {
            if (inputSql == null)
            {
                return string.Empty;
            }

            var result = inputSql.Replace("'", "''");

            return result;
        }

        protected virtual string GenerateColumnNames()
        {
            if (string.IsNullOrWhiteSpace(_columnNames) || _columnNames == "*")
            {
                return "*";
            }

            var colBuilder = new StringBuilder();
            var colNames = _columnNames.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

            for (int i = 0; i < colNames.Length; i++)
            {
                var col = SafeSqlLiteral(colNames[i]);

                if (ConvertToRealField != null)
                {
                    var field = ConvertToRealField(col);
                    if (string.IsNullOrEmpty(field))
                    {
                        continue;
                    }

                    colBuilder.Append(field);
                }
                else
                {
                    colBuilder.Append(col);
                }

                if (i < colNames.Length - 1)
                {
                    colBuilder.Append(',');
                }
            }

            var result = colBuilder.ToString();
            if (string.IsNullOrEmpty(result))
            {
                return _columnNames;
            }

            return result;
        }

        protected virtual string GenerateOrderByClause(string tableAlias = "")
        {
            if (_orderBy == null || _orderBy.Count == 0)
            {
                return string.Empty;
            }

            var orderByBuilder = new StringBuilder();

            var index = 0;
            foreach (var item in _orderBy)
            {
                var field = ConvertToRealField != null ? ConvertToRealField(item.Key) : item.Key;
                if (string.IsNullOrEmpty(field))
                {
                    continue;
                }

                field = SafeSqlLiteral(field);
                var ascDesc = item.Value == null || string.Equals(item.Value, "asc", StringComparison.OrdinalIgnoreCase) || (item.Value != null && item.Value.StartsWith("asc", StringComparison.OrdinalIgnoreCase)) ? "asc" : "desc";
                orderByBuilder.Append($"{tableAlias}{field} {ascDesc}");

                if (index < _orderBy.Count - 1)
                {
                    orderByBuilder.Append(',');
                }

                index++;
            }

            if (index == 0)
            {
                return string.Empty;
            }

            return orderByBuilder.ToString();
        }

        protected virtual string GenerateWhereClause(string tableAlias = "", string paramTag = "@")
        {
            if (_where == null || _where.Count == 0)
            {
                return string.Empty;
            }

            var whereBuilder = new StringBuilder();
            var index = 0;
            foreach (var item in _where)
            {
                item.Field = SafeSqlLiteral(item.Field);

                if (ConvertToRealField != null)
                {
                    item.Field = ConvertToRealField(item.Field);
                }

                if (string.IsNullOrEmpty(item.Field))
                {
                    continue;
                }

                if (index > 0)
                {
                    whereBuilder.Append($"{item.GetAndOr()} ");
                }
                index++;

                whereBuilder.Append(item.GenerateSql(tableAlias, paramTag));
            }

            if (index == 0)
            {
                return string.Empty;
            }

            return whereBuilder.ToString();
        }

        public virtual Dictionary<string, object> GetParamObject()
        {
            if (_where == null || _where.Count == 0)
            {
                return new Dictionary<string, object>(0);
            }

            var result = new Dictionary<string, object>(_where.Count);

            foreach (var item in _where)
            {
                var field = SafeSqlLiteral(ConvertToRealField != null ? ConvertToRealField(item.Field) : item.Field);
                if (string.IsNullOrEmpty(field))
                {
                    continue;
                }

                if (result.ContainsKey(field))
                {
                    continue;
                }

                result.Add(field, item.Value);
            }

            return result;
        }

        public override string ToString()
        {
            return ToSql();
        }

        public virtual string ToSql(string paramTag = "@")
        {
            var colNames = GenerateColumnNames();
            var whereClause = GenerateWhereClause(paramTag: paramTag);
            var orderByClause = GenerateOrderByClause();
            whereClause = string.IsNullOrEmpty(whereClause) ? string.Empty : "where " + whereClause;
            orderByClause = string.IsNullOrEmpty(orderByClause) ? string.Empty : "order by " + orderByClause;

            return $"select {colNames} from {_tableName} {whereClause} {orderByClause} ";
        }

        public virtual string ToCountSql(string paramTag = "@")
        {
            var whereClause = GenerateWhereClause(paramTag: paramTag);
            whereClause = string.IsNullOrEmpty(whereClause) ? string.Empty : "where " + whereClause;

            return $"select count(*) from {_tableName} {whereClause}";
        }
    }
}

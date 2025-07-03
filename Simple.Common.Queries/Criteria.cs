// Copyright (c) 2023 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Simple.Common.Queries
{
    public class Criteria
    {
        readonly static Dictionary<string, string> SqlOperator = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Equal", "=" },
            { "eq", "=" },
            { "=", "=" },
            
            { "like", "like" },
            { "Contains", "like" },
            { "StartWith", "like" },
            { "EndWith", "like" },
            
            { "GreaterThan", ">" },
            { "gt", ">" },
            { ">", ">" },
            
            { "GreaterThanOrEqual", ">=" },
            { "ge", ">=" },
            { ">=", ">=" },
            
            { "LessThan", "<" },
            { "lt", "<" },
            { "<", "<" },
            
            { "LessThanOrEqual", "<=" },
            { "le", "<=" },
            { "<=", "<=" },

            { "In", "in" }
        };

        public string AndOr { get; set; }

        public string Field { get; set; }

        public string Operator { get; set; }

        public object Value { get; set; }

        protected virtual string BindValue(string sqlOperator)
        {
            if (Value is int || Value is uint
                || Value is short || Value is ushort
                || Value is long || Value is ulong
                || Value is byte || Value is double 
                || Value is float || Value is decimal)
            {
                return Value.ToString();
            }

            if (Value is bool)
            {
                return (bool)Value ? "1" : "0";
            }

            //if (Value is IEnumerable<int> || Value is IEnumerable<uint>
            //    || Value is IEnumerable<short> || Value is IEnumerable<ushort>
            //    || Value is IEnumerable<long> || Value is IEnumerable<ulong>
            //    || Value is IEnumerable<byte> || Value is IEnumerable<double>
            //    || Value is IEnumerable<float> || Value is IEnumerable<decimal>)
            //{
            //    var value = string.Join(",", (dynamic)Value);
            //    return $"({value})";
            //}

            //if (Value is IEnumerable)
            //{
            //    var value = string.Join("','", (dynamic)Value);
            //    return $"('{value}')";
            //}

            var paramValue = "'" + Value.ToString() + "'";
            paramValue = TryFixLikeClause(sqlOperator, paramValue);

            return paramValue;
        }

        protected virtual string BindParamName(string sqlOperator, string paramTag = "@")
        {
            if (string.IsNullOrEmpty(paramTag))
            {
                return BindValue(sqlOperator);
            }

            var paramName = paramTag + this.Field;
            paramName = TryFixLikeClause(sqlOperator, paramName);

            return paramName;
        }

        protected virtual string TryFixLikeClause(string sqlOperator, string paramName)
        {
            if (string.Equals(this.Operator, "StartWith", StringComparison.OrdinalIgnoreCase))
            {
                return $"concat({paramName}, '%')";
            }

            if (string.Equals(this.Operator, "EndWith", StringComparison.OrdinalIgnoreCase))
            {
                return $"concat('%', {paramName})";
            }

            if (sqlOperator == "like")
            {
                return $"concat('%', {paramName}, '%')";
            }

            return paramName;
        }

        protected virtual string GenerateSqlOperator(string input)
        {
            if (SqlOperator.TryGetValue(input, out var sqlOperator))
            {
                return sqlOperator;
            }

            return "=";
        }

        public virtual string GenerateSql(string tableAlias = "", string paramTag = "@")
        {
            var sqlOperator = GenerateSqlOperator(this.Operator);

            return $"{tableAlias}{Field} {sqlOperator} {BindParamName(sqlOperator, paramTag)} ";
        }

        internal virtual string GetAndOr()
        {
            if (string.IsNullOrWhiteSpace(this.AndOr) || this.AndOr.Contains("and", StringComparison.OrdinalIgnoreCase))
            {
                return "and";
            }

            return "or";
        }

        public override string ToString()
        {
            return $"{GetAndOr()} {GenerateSql()}";
        }

        public static Criteria Equal(string field, object value)
        { 
            return new Criteria() { Field = field, Value = value, Operator = "=" };
        }

        public static Criteria Contains(string field, object value)
        {
            return new Criteria() { Field = field, Value = value, Operator = "like" };
        }

        public static Criteria GreaterThan(string field, object value)
        {
            return new Criteria() { Field = field, Value = value, Operator = ">" };
        }

        public static Criteria GreaterThanOrEqual(string field, object value)
        {
            return new Criteria() { Field = field, Value = value, Operator = ">=" };
        }

        public static Criteria LessThan(string field, object value)
        {
            return new Criteria() { Field = field, Value = value, Operator = "<" };
        }

        public static Criteria LessThanOrEqual(string field, object value)
        {
            return new Criteria() { Field = field, Value = value, Operator = "<=" };
        }

        public static Criteria In(string field, object value)
        {
            return new Criteria() { Field = field, Value = value, Operator = "in" };
        }

        public static Criteria StartWith(string field, object value)
        {
            return new Criteria() { Field = field, Value = value, Operator = "StartWith" };
        }

        public static Criteria EndWith(string field, object value)
        {
            return new Criteria() { Field = field, Value = value, Operator = "EndWith" };
        }
    }
}

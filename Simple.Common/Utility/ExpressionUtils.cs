using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Simple.Common.Utility
{
    public class ExpressionUtils
    {
        public static MemberInfo GetMemberInfo<T, U>(Expression<Func<T, U>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member != null)
                return member.Member;

            throw new ArgumentException("无法通过表达式访问成员", "expression");
        }

        public static string GetMemberName<T, U>(Expression<Func<T, U>> expression)
        {
            return GetMemberInfo(expression).Name;
        }
    }
}

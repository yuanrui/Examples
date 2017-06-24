using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Simple.Common.Utility
{
    public delegate T ObjectActivator<T>(params object[] args);

    public static class ActivatorHelper
    {
        private static ConcurrentDictionary<string, ObjectActivator<object>> CacheMap = new ConcurrentDictionary<string, ObjectActivator<object>>();

        public static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor)
        {
            Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            ParameterExpression param = Expression.Parameter(typeof(object[]), "args");

            Expression[] argsExp = new Expression[paramsInfo.Length];

            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp = Expression.ArrayIndex(param, index);

                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            NewExpression newExp = Expression.New(ctor, argsExp);

            LambdaExpression lambda = Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

            ObjectActivator<T> compiled = (ObjectActivator<T>)lambda.Compile();

            return compiled;
        }

        public static Object CreateInstance(Type type, params object[] args)
        {
            var ctorCount = 0;

            if (args != null)
            {
                ctorCount = args.Length;
            }
            var key = type.FullName + "." + ctorCount.ToString();

            if (CacheMap.ContainsKey(key))
            {
                return CacheMap[key].Invoke(args);
            }
            else
            {
                var ctor = type.GetConstructors()[ctorCount];

                ObjectActivator<Object> createdActivator = GetActivator<Object>(ctor);
                
                CacheMap.AddOrUpdate(key, createdActivator, (k, oldValue) => createdActivator);

                return createdActivator.Invoke(args);
            }            
        }
    }
}

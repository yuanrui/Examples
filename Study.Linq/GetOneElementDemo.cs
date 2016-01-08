//------------------------------------------------------------------------------
// <copyright file="GetOneElementDemo.cs" company="CQ Ebos Co., Ltd.">
//    Copyright (c) 2016, CQ Ebos Co., Ltd. All rights reserved.
// </copyright>
// <author>Yuan Rui</author>
// <email>yuanrui@live.cn</email>
// <date>2016-01-08 14:17:22</date>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Study.Linq
{
    internal class GetOneElementDemo
    {
        protected class KeyValue
        {
            public string Key { get; set; }

            public int Value { get; set; }

            public override string ToString()
            {
                return string.Format("Key:{0} Value:{1}", Key, Value);
            }
        }

        private static readonly Stopwatch Watch = new Stopwatch();

        private static IEnumerable<KeyValue> BuildNonUniqueSources()
        {
            var result = new List<KeyValue>();

            for (int i = 0; i < 10000; i++)
            {
                for (int j = 65; j < 91; j++)
                {
                    var obj = new KeyValue() { Key = string.Format("{0}{0}{0}", (char)j), Value = i };
                    result.Add(obj);
                }
            }

            return result;
        }

        private static IEnumerable<KeyValue> BuildUniqueSources()
        {
            var result = new List<KeyValue>();

            for (int i = 0; i < 10000; i++)
            {
                for (int j = 65; j < 91; j++)
                {
                    var obj = new KeyValue() { Key = string.Format("{0}{0}{0}-{1}", (char)j, i), Value = i };
                    result.Add(obj);
                }
            }

            return result;
        }

        private static void ShowTest(IEnumerable<KeyValue> sources, Func<KeyValue, bool> predicate,
            Func<IEnumerable<KeyValue>, Func<KeyValue, bool>, KeyValue> getKeyValueFunc)
        {
            var methodName = getKeyValueFunc.Method.Name;
            Console.Write("Method:{0} ", methodName);
            Watch.Restart();
            try
            {
                Console.Write("Result:{0}", getKeyValueFunc(sources, predicate));
                Watch.Stop();
            }
            catch (InvalidOperationException invalidOptEx)
            {
                Console.Write("Exception:{0}", invalidOptEx.Message);
            }

            Console.WriteLine(" Total:{1}ms\n", methodName, Watch.Elapsed.TotalMilliseconds);
        }

        public static void Run()
        {
            IEnumerable<KeyValue> _sources;
            _sources = BuildNonUniqueSources();
            //_sources = BuildUniqueSources();

            const string key = "ZZZ";

            //消除初始化等影响
            ShowTest(_sources, m => m.Key == key, Enumerable.Last);
            Console.Clear();

            ShowTest(_sources, m => m.Key == key, Enumerable.First);
            ShowTest(_sources, m => m.Key == key, Enumerable.FirstOrDefault);
            ShowTest(_sources, m => m.Key == key, Enumerable.Single);
            ShowTest(_sources, m => m.Key == key, Enumerable.SingleOrDefault);
            ShowTest(_sources, m => m.Key == key, Enumerable.Last);
            ShowTest(_sources, m => m.Key == key, Enumerable.LastOrDefault);
        }
    }
}

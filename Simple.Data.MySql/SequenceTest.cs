// Copyright (c) 2021 YuanRui
// GitHub: https://github.com/yuanrui
// Contact: cqdeveloper@hotmail.com
// License: Apache-2.0

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Simple.Data.MySql
{
    public class SequenceTest
    {
        public static void Run()
        {
            var sql = @"select nextval('t_sys_role');";
            //sql = "CALL GET_CURRENT_SEQ('b', 1, @p_out);SELECT @p_out;";
            var idValue = DbUtils.ExecuteScalar<Int64>(sql);
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder("Database=sso_db;Data Source=127.0.0.1;port=3306;User Id=root;Password=abc123456;CharSet=utf8;Allow User Variables=True;Default Command Timeout=600");
            var size = builder.MaximumPoolSize;
            var min = builder.MinimumPoolSize;
            builder.MinimumPoolSize = 50;
            builder.MaximumPoolSize = 500;
            builder.Pooling = true;
            var str = builder.ToString();
            const string ConnectionName = "DefaultConnectionString";
            Dictionary<Int64, DateTime> dict = new Dictionary<long, DateTime>();

            for (int i = 0; i < 100; i++)
            {
                var thread = new Thread(() => {
                    try
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        for (int j = 0; j < 100000; j++)
                        {
                            stopwatch.Restart();
                            var id = DbUtils.ExecuteScalar<Int64>(sql);
                            stopwatch.Stop();
                            //for (int k = 0; k < 20; k++)
                            {
                                dict.Add(id, DateTime.Now);
                                //Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}>>{dict.Count}- {Thread.CurrentThread.Name}  id= {id} time:{stopwatch.Elapsed.Milliseconds}ms");
                                //id++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    //using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin())
                    //{
                    //    for (int j = 0; j < 1000; j++)
                    //    {
                    //        var id = scope.DataContext.ExecuteScalar<Int64>(sql);
                    //        dict.Add(id, DateTime.Now);
                    //        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}>>{dict.Count}- {Thread.CurrentThread.Name}  id= {id}");
                    //    }
                    //}

                });
                thread.Name = "thread_" + i;
                thread.IsBackground = true;
                thread.Start();
            }

            var input = string.Empty;
            do
            {
                input = Console.ReadLine();

                try
                {
                    var secTotal = dict.Values.ToList().GroupBy(m => m.ToString("HH:mm:ss")).Count();
                    var secCount = (int)Math.Round((decimal)dict.Count / secTotal);
                    for (int i = 0; i < 3; i++)
                    {
                        Console.WriteLine($"平均每秒构造：" + secCount + "个Key");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                //var grp = dict.Values.GroupBy(m => m.ToString("HH:mm:ss")).ToList();
                //var cnt = grp.Max(m => m.Count());
                //var obj = grp.FirstOrDefault(m => m.Count() == cnt);
                //if (obj != null)
                //{
                //    Console.WriteLine($"{obj.Key} = {cnt}");
                //}
            } while (input != "q");
        }
    }
}

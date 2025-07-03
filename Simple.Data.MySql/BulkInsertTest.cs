// Copyright (c) 2021 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Simple.Data.MySql
{
    public class BulkInsertTest
    {
        public static void Run()
        {
            var maxCount = 10000;
            Console.Write("请输入待导入的总数:");
            var input = Console.ReadLine();
            if (! int.TryParse(input, out maxCount))
            {
                maxCount = 10000;
            }

            var list = CreateList(maxCount);
            Console.WriteLine($"导入数据总条数:{list.Count}");
            DoTruncateTable();
            var table = ToDataTable(list);
            Stopwatch stopwatch = Stopwatch.StartNew();
            //var csv = ToCsvFile(list);

            stopwatch.Restart();
            DoInsertList4(list);
            stopwatch.Stop();
            Console.WriteLine($"使用MySqlBulkLoader耗时:{stopwatch.Elapsed.TotalSeconds}s {stopwatch.Elapsed.TotalMilliseconds}ms");

            stopwatch.Restart();
            DoInsertList4(list);
            stopwatch.Stop();
            Console.WriteLine($"使用MySqlBulkLoader耗时:{stopwatch.Elapsed.TotalSeconds}s {stopwatch.Elapsed.TotalMilliseconds}ms");

            stopwatch.Restart();
            DoInsertList2(table);
            stopwatch.Stop();
            Console.WriteLine($"适配器批量更新耗时:{stopwatch.Elapsed.TotalSeconds}s {stopwatch.Elapsed.TotalMilliseconds}ms");

            stopwatch.Restart();
            DoInsertList1(list);
            stopwatch.Stop();
            Console.WriteLine($"循环执行命令耗时:{stopwatch.Elapsed.TotalSeconds}s {stopwatch.Elapsed.TotalMilliseconds}ms");

            stopwatch.Restart();
            DoInsertList3(list);
            stopwatch.Stop();
            Console.WriteLine($"执行拼接SQL语句耗时:{stopwatch.Elapsed.TotalSeconds}s {stopwatch.Elapsed.TotalMilliseconds}ms");
        }

        public static List<VehiclePassModel> CreateList(int maxCount = 10000)
        {
            var rd = new Random(Guid.NewGuid().GetHashCode());
            var list = new List<VehiclePassModel>(maxCount);
            for (int i = 0; i < maxCount; i++)
            {
                var model = new VehiclePassModel();
                model.PlateNO = $"京A{Guid.NewGuid().ToString().Substring(0, 2)}{(i % 1000).ToString().PadLeft(3, '0')}".ToUpper();
                model.PlateColor = i % 7 == 0 ? "黄" : "蓝";
                model.PassTime = DateTime.Now.AddSeconds(0 - rd.Next(0, maxCount));
                model.EquipId = $"{DateTime.Now.ToString("yyyyMMddHHmm")}";
                model.CreatedAt = DateTime.Now;

                list.Add(model);
            }

            return list;
        }

        public static DataTable ToDataTable(IEnumerable<VehiclePassModel> list)
        {
            var table = new DataTable("t_vehicle_pass_record");
            table.Columns.Add("id", typeof(Int64));
            table.Columns.Add("plate_no", typeof(string));
            table.Columns.Add("plate_color", typeof(string));
            table.Columns.Add("pass_time", typeof(DateTime));
            table.Columns.Add("equip_id", typeof(string));

            foreach (var model in list)
            {
                var row = table.NewRow();
                row["id"] = model.Id;
                row["plate_no"] = model.PlateNO;
                row["plate_color"] = model.PlateColor;
                row["pass_time"] = model.PassTime;
                row["equip_id"] = model.EquipId;

                table.Rows.Add(row);
            }

            return table;
        }

        public static string ToCsvFile(IEnumerable<VehiclePassModel> list)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString() + ".csv");
            var head = "id,plate_color,plate_no,pass_time,equip_id";

            using (var writer = File.CreateText(path))
            {
                writer.WriteLine(head);
                foreach (var item in list)
                {
                    var content = $"{item.Id},{item.PlateColor},{item.PlateNO},{item.PassTime.ToString("yyyy-MM-dd HH:mm:ss")},{item.EquipId}";
                    writer.WriteLine(content);
                }
                writer.Flush();
            }

            return path;
        }


        public static void DoTruncateTable()
        {
            const string sql = "truncate table t_vehicle_pass_record;";
            var connStr = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }

                    tran.Commit();
                }
            }
        }

        public static int DoInsertList1(IEnumerable<VehiclePassModel> list)
        {
            const string sql = "insert into t_vehicle_pass_record (id, plate_no, plate_color, pass_time, equip_id, created_at) values (?id, ?plate_no, ?plate_color, ?pass_time, ?equip_id, now());";
            var result = 0;
            var connStr = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.Transaction = tran;

                        foreach (var model in list)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("id", model.Id);
                            cmd.Parameters.AddWithValue("plate_no", model.PlateNO);
                            cmd.Parameters.AddWithValue("plate_color", model.PlateColor);
                            cmd.Parameters.AddWithValue("pass_time", model.PassTime);
                            cmd.Parameters.AddWithValue("equip_id", model.EquipId);

                            result += cmd.ExecuteNonQuery(); //执行sql语句
                        }

                        tran.Commit();
                    }
                }
            }

            return result;
        }

        public static int DoInsertList2(IEnumerable<VehiclePassModel> list)
        {
            const string sql = "insert into t_vehicle_pass_record (id, plate_no, plate_color, pass_time, equip_id, created_at) values (?id, ?plate_no, ?plate_color, ?pass_time, ?equip_id, now());";
            var result = 0;
            var connStr = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;

            var table = new DataTable("t_vehicle_pass_record");
            table.Columns.Add("id", typeof(Int64));
            table.Columns.Add("plate_no", typeof(string));
            table.Columns.Add("plate_color", typeof(string));
            table.Columns.Add("pass_time", typeof(DateTime));
            table.Columns.Add("equip_id", typeof(string));

            foreach (var model in list)
            {
                var row = table.NewRow();
                row["id"] = model.Id;
                row["plate_no"] = model.PlateNO;
                row["plate_color"] = model.PlateColor;
                row["pass_time"] = model.PassTime;
                row["equip_id"] = model.EquipId;

                table.Rows.Add(row);
            }

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.Transaction = tran;

                        using (var adapter = new MySqlDataAdapter())
                        {
                            adapter.InsertCommand = cmd;

                            adapter.InsertCommand.Parameters.Add("?id", MySqlDbType.Int64, 21, "id");
                            adapter.InsertCommand.Parameters.Add("?plate_no", MySqlDbType.VarChar, 10, "plate_no");
                            adapter.InsertCommand.Parameters.Add("?plate_color", MySqlDbType.VarChar, 6, "plate_color");
                            adapter.InsertCommand.Parameters.Add("?pass_time", MySqlDbType.DateTime, 8, "pass_time");
                            adapter.InsertCommand.Parameters.Add("?equip_id", MySqlDbType.VarChar, 20, "equip_id");

                            adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;


                            //adapter.UpdateBatchSize = 1000;
                            result = adapter.Update(table); //执行sql语句
                            tran.Commit();
                        }
                    }
                }
            }

            return result;
        }

        public static int DoInsertList2(DataTable table)
        {
            const string sql = "insert into t_vehicle_pass_record (id, plate_no, plate_color, pass_time, equip_id, created_at) values (?id, ?plate_no, ?plate_color, ?pass_time, ?equip_id, now());";
            var result = 0;
            var connStr = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.Transaction = tran;

                        using (var adapter = new MySqlDataAdapter())
                        {
                            adapter.InsertCommand = cmd;

                            adapter.InsertCommand.Parameters.Add("?id", MySqlDbType.Int64, 21, "id");
                            adapter.InsertCommand.Parameters.Add("?plate_no", MySqlDbType.VarChar, 10, "plate_no");
                            adapter.InsertCommand.Parameters.Add("?plate_color", MySqlDbType.VarChar, 6, "plate_color");
                            adapter.InsertCommand.Parameters.Add("?pass_time", MySqlDbType.DateTime, 8, "pass_time");
                            adapter.InsertCommand.Parameters.Add("?equip_id", MySqlDbType.VarChar, 20, "equip_id");

                            adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;

                            adapter.UpdateBatchSize = 1000;
                            result = adapter.Update(table); //执行sql语句
                            tran.Commit();
                        }
                    }
                }
            }

            return result;
        }

        public static int DoInsertList3(IEnumerable<VehiclePassModel> list)
        {
            const string insertIntoClause = "insert into t_vehicle_pass_record (id, plate_no, plate_color, pass_time, equip_id, created_at) values ";
            var result = 0;
            var connStr = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;

            var valueClauses = new List<string>(list.Count());
            var index = 0;
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tran;

                        //foreach (var model in list)
                        //{
                        //    var suffix = Convert.ToString(index, 16);
                        //    var clause = $"(?a{suffix}, ?b{suffix}, ?c{suffix}, ?d{suffix}, ?e{suffix}, now())";
                        //    valueClauses.Add(clause);
                        //    cmd.Parameters.AddWithValue($"a{suffix}", model.Id);
                        //    cmd.Parameters.AddWithValue($"b{suffix}", model.PlateNO);
                        //    cmd.Parameters.AddWithValue($"c{suffix}", model.PlateColor);
                        //    cmd.Parameters.AddWithValue($"d{suffix}", model.PassTime);
                        //    cmd.Parameters.AddWithValue($"e{suffix}", model.EquipId);
                        //    index++;
                        //}

                        foreach (var model in list)
                        {
                            var clause = $"(?id_{index}, ?plate_no_{index}, ?plate_color_{index}, ?pass_time_{index}, ?equip_id_{index}, now())";
                            valueClauses.Add(clause);
                            cmd.Parameters.AddWithValue($"id_{index}", model.Id);
                            cmd.Parameters.AddWithValue($"plate_no_{index}", model.PlateNO);
                            cmd.Parameters.AddWithValue($"plate_color_{index}", model.PlateColor);
                            cmd.Parameters.AddWithValue($"pass_time_{index}", model.PassTime);
                            cmd.Parameters.AddWithValue($"equip_id_{index}", model.EquipId);
                            index++;
                        }

                        var sql = insertIntoClause + string.Join(",", valueClauses) + ";";
                        cmd.CommandText = sql;

                        Console.WriteLine(sql.Length);
                        Console.WriteLine(Encoding.UTF8.GetBytes(sql).Length);

                        result = cmd.ExecuteNonQuery(); //执行sql语句
                    }

                    tran.Commit();
                }
            }

            return result;
        }

        public static int DoInsertList4(IEnumerable<VehiclePassModel> list)
        {
            var result = 0;
            var connStr = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString() + ".csv");
            var head = "id,plate_color,plate_no,pass_time,equip_id";
            using (var writer = File.CreateText(path))
            {
                writer.WriteLine(head);//注释掉此行后, loader.NumberOfLinesToSkip可以设置为0
                foreach (var item in list)
                {
                    var content = $"{item.Id},{item.PlateColor},{item.PlateNO},{item.PassTime.ToString("yyyy-MM-dd HH:mm:ss")},{item.EquipId}";
                    writer.WriteLine(content);
                }
                writer.Flush();
            }

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    MySqlBulkLoader loader = new MySqlBulkLoader(conn);
                    loader.Local = true;
                    loader.FieldTerminator = ",";
                    loader.LineTerminator = Environment.NewLine;
                    loader.FileName = path;
                    loader.NumberOfLinesToSkip = 1;
                    loader.CharacterSet = "utf8mb4";

                    loader.TableName = "t_vehicle_pass_record";
                    loader.Columns.AddRange(head.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

                    result = loader.Load();
                }
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            return result;
        }

        public static int DoInsertList4(string path)
        {
            var result = 0;
            var connStr = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
            //var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString() + ".csv");

            var head = "id,plate_color,plate_no,pass_time,equip_id" + Environment.NewLine;
            //File.AppendAllText(path, head, Encoding.UTF8);
            //foreach (var item in list)
            //{
            //    var content = $"{item.Id},{item.PlateColor},{item.PlateNO},{item.PassTime.ToString("yyyy-MM-dd HH:mm:ss")},{item.EquipId}{Environment.NewLine}";
            //    File.AppendAllText(path, content, Encoding.UTF8);
            //}
            //Console.WriteLine(path);

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlBulkLoader loader = new MySqlBulkLoader(conn);
                loader.Local = true;
                loader.FieldTerminator = ",";
                loader.LineTerminator = Environment.NewLine;
                loader.FileName = path;
                loader.NumberOfLinesToSkip = 1;
                loader.CharacterSet = "utf8mb4";

                loader.TableName = "t_vehicle_pass_record";
                loader.Columns.AddRange(head.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

                result = loader.Load();
            }
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return result;
        }
    }
}

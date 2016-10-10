using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Oracle.ManagedDataAccess.Client;
using OracleCore = Oracle.ManagedDataAccess;

namespace Simple.Data.Oracle
{
    class Program
    {
        static void Main(string[] args)
        {
            const int totalCount = 100000;
            DropTable();
            CreateTable();

            var list = new List<Employee>();
            var sqlList = new List<string>();

            for (int i = 0; i < totalCount; i++)
            {
                var ent = new Employee();
                ent.Id = Guid.NewGuid().ToString();
                ent.Name = "Test-" + i.ToString().PadLeft(4, '0');
                ent.DeptId = i;
                sqlList.Add(string.Format("INSERT INTO emp (id, name, job, deptid) values ('{0}', '{1}', '{2}', {3})", Guid.NewGuid().ToString(), "A-" + ent.Name, ent.Job, ent.DeptId));
                sqlList.Add(string.Format("INSERT INTO emp (id, name, job, deptid) values ('{0}', '{1}', '{2}', {3})", Guid.NewGuid().ToString(), "B-" + ent.Name, ent.Job, ent.DeptId));
                list.Add(ent);
            }

            WatchExecute(() =>
            {
                Console.WriteLine("Insert one by one with connection open and close");
                Insert(sqlList.Take(totalCount));
            });

            WatchExecute(() =>
            {
                Console.WriteLine("Insert one by one with connection openning in long time");
                Insert2(sqlList.Skip(totalCount));
            });

            WatchExecute(() =>
            {
                Console.WriteLine("Insert use bind array");
                Insert(list);
            });
            
            Console.WriteLine("\nPress Any Key To Exit...");
            Console.ReadLine();
        }

        static DbConnection CreateConnection()
        {
            var buider = new OracleConnectionStringBuilder();
            buider.UserID = "scott";
            buider.Password = "tiger";
            buider.DataSource = "(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.1.173)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ebos0)))";
            buider.Pooling = true;
            buider.ConnectionTimeout = 5;
            //Console.WriteLine(buider.ToString());

            return new OracleConnection(buider.ToString());
        }

        static OracleCommand CreateCommand()
        {
            return new OracleCommand();
        }

        static DbConnection GetOpenConnection()
        {
            var conn = CreateConnection();
            conn.Open();

            return conn;
        }

        static int ExecuteNonQuery(string cmdText, CommandType cmdType = CommandType.Text)
        {
            using (var conn = GetOpenConnection())
            {
                using (var cmd = CreateCommand())
                {
                    cmd.Connection = (OracleConnection)conn;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = cmdType;

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        static void DropTable()
        {
            try
            {
                ExecuteNonQuery("DROP TABLE EMP");
                Console.WriteLine("DROP TABLE EMP");
            }
            catch (Exception ex)
            {
                Console.WriteLine("DROP TABLE EMP; Exception:{0}", ex.Message);
            }
        }

        static void CreateTable()
        {
            ExecuteNonQuery(@"
CREATE TABLE EMP
(
       id varchar2(36) not null primary key,
       name nvarchar2(20),
       job nvarchar2(20),
       deptid number(9)
)"
                );

            Console.WriteLine("CREATE TABLE EMP");
        }

        public class Employee 
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string Job { get; set; }

            public Int32 DeptId { get; set; }
        }

        static void Insert(IEnumerable<Employee> list)
        {
            const string cmdText = "INSERT INTO emp (id, name, job, deptid) values (:id, :name, :job, :deptid)";

            using (var conn = GetOpenConnection())
            {
                using (var cmd = CreateCommand())
                {
                    cmd.Connection = (OracleConnection)conn;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;

                    cmd.ArrayBindCount = list.Count();
                    cmd.Parameters.Add("id", OracleDbType.Varchar2, list.Select(c => c.Id).ToArray(), ParameterDirection.Input);
                    cmd.Parameters.Add("name", OracleDbType.Varchar2, list.Select(c => c.Name).ToArray(), ParameterDirection.Input);
                    cmd.Parameters.Add("job", OracleDbType.Varchar2, list.Select(c => c.Job).ToArray(), ParameterDirection.Input);
                    cmd.Parameters.Add("deptid", OracleDbType.Decimal, list.Select(c => c.DeptId).ToArray(), ParameterDirection.Input);
                    //cmd.Parameters.Add("id", list.Select(c => c.Id).ToArray());
                    //cmd.Parameters.Add("name", list.Select(c => c.Name).ToArray());
                    //cmd.Parameters.Add("job", list.Select(c => c.Job).ToArray());
                    //cmd.Parameters.Add("deptid", list.Select(c => c.DeptId).ToArray());

                    cmd.ExecuteNonQuery();
                }
            }
        }

        static void Insert(IEnumerable<string> sqlList)
        {
            foreach (var sql in sqlList)
            {
                ExecuteNonQuery(sql);
            }
        }

        static void Insert2(IEnumerable<string> sqlList)
        {
            using (var conn = GetOpenConnection())
            {
                using (var cmd = CreateCommand())
                {
                    cmd.Connection = (OracleConnection)conn;
                    cmd.CommandType = CommandType.Text;
                    
                    foreach (var sql in sqlList)
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }                    
                }
            }
        }

        static void WatchExecute(Action doAction)
        {
            var watch = Stopwatch.StartNew();
            doAction();
            Console.WriteLine("Execute Time:{0}s", Math.Round(watch.Elapsed.TotalSeconds, 2));
        }
    }
}

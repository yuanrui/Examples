using System;
using System.Collections.Generic;
using System.Configuration;
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
            TestBindByName();
            Console.WriteLine("\nPress Any Key To Exit...");
            Console.ReadLine();
        }

        private static void TestBindByName()
        {
            const string cmdText = @"
insert into emp (EMPNO, ENAME, JOB, MGR, HIREDATE, SAL, COMM, DEPTNO) 
values (:EMPNO, :ENAME, :JOB, :MGR, :HIREDATE, :SAL, :COMM, :DEPTNO)";

            var connStr = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.1.170)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ebos)));User ID=scott;Password=tiger;";
            using (var conn = new OracleConnection(connStr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    //cmd.BindByName = false;
                    cmd.Parameters.Add("EMPNO", OracleDbType.Int16, 4, ParameterDirection.Input);
                    cmd.Parameters.Add("ENAME", OracleDbType.Varchar2, "ENAME".ToArray(), ParameterDirection.Input);
                    cmd.Parameters.Add("JOB", OracleDbType.Varchar2, "JOB", ParameterDirection.Input);
                    cmd.Parameters.Add("MGR", OracleDbType.Int32, 1, ParameterDirection.Input);
                    cmd.Parameters.Add("HIREDATE", OracleDbType.Date, DateTime.Now, ParameterDirection.Input);
                    cmd.Parameters.Add("DEPTNO", OracleDbType.Int32, 30, ParameterDirection.Input);
                    cmd.Parameters.Add("COMM", OracleDbType.Decimal, 20, ParameterDirection.Input);
                    cmd.Parameters.Add("SAL", OracleDbType.Decimal, 10, ParameterDirection.Input);

                    conn.Open();
                    var result = cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        private static void TestInsert()
        {
            const int totalCount = 5000;
            SaveConfig();
            DropTable();
            CreateTable();

            var list = new List<Employee>();
            var sqlList = new List<string>();

            for (int i = 0; i < totalCount; i++)
            {
                var ent = new Employee();
                ent.EmpNo = Guid.NewGuid().ToString();
                ent.EName = "Test-" + i.ToString().PadLeft(4, '0');
                ent.DeptNo = i;
                ent.Hiredate = DateTime.Now.AddDays((0 - i) * 1.5);
                ent.Sal = ent.EName.GetHashCode() / 100000;
                ent.Comm = ent.EName.GetHashCode() / 100000;

                sqlList.Add(string.Format("insert into emp (EMPNO, ENAME, JOB, MGR, HIREDATE, SAL, COMM, DEPTNO) values ('{0}', '{1}', '{2}', {3}, to_date('{4}', 'yyyy-mm-dd'), {5}, {6}, {7})", Guid.NewGuid().ToString(), "A-" + ent.EName, ent.Job, ent.Mgr, ent.Hiredate.ToString("yyyy-MM-dd"), ent.Sal, ent.Comm, ent.DeptNo));
                sqlList.Add(string.Format("insert into emp (EMPNO, ENAME, JOB, MGR, HIREDATE, SAL, COMM, DEPTNO) values ('{0}', '{1}', '{2}', {3}, to_date('{4}', 'yyyy-mm-dd'), {5}, {6}, {7})", Guid.NewGuid().ToString(), "A-" + ent.EName, ent.Job, ent.Mgr, ent.Hiredate.ToString("yyyy-MM-dd"), ent.Sal, ent.Comm, ent.DeptNo));
                list.Add(ent);
            }

            WatchExecute(() =>
            {
                Console.WriteLine("Insert use bind array with transaction");
                Insert(list);
            });

            WatchExecute(() =>
            {
                Console.WriteLine("Insert one by one with connection(transaction) openning in long time");
                Insert2(sqlList.Skip(totalCount));
            });

            WatchExecute(() =>
            {
                Console.WriteLine("Insert one by one with connection open and close");
                Insert(sqlList.Take(totalCount));
            });

            var empCount = DbUtils.Query<Int32>("SELECT COUNT(*) FROM EMP").FirstOrDefault();
            Console.WriteLine("EMP table count:{0}", empCount);
        }

        static void SaveConfig()
        {
            //TODO: Lock
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings
                .ConnectionStrings.Add(new ConnectionStringSettings("MyConnectionString", GetBuilder().ToString()));
            config.ConnectionStrings
                .ConnectionStrings["DefaultConnectionString"].ConnectionString = GetBuilder().ToString();
            config.Save(ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        static OracleConnectionStringBuilder GetBuilder()
        {
            var builder = new OracleConnectionStringBuilder();
            builder.UserID = "scott";
            builder.Password = "tiger";
            builder.DataSource = "(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.1.170)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ebos)))";
            builder.Pooling = true;
            builder.ConnectionTimeout = 5;
            
            //Console.WriteLine(builder.ToString());
            return builder;
        }

        static DbConnection CreateConnection()
        {
            var builder = GetBuilder();

            return new OracleConnection(builder.ToString());
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
  empno    varchar2(36) not null primary key,
  ename    nvarchar2(20),
  job      nvarchar2(20),
  mgr      NUMBER(9),
  hiredate DATE,
  sal      NUMBER(7,2),
  comm     NUMBER(7,2),
  deptno   NUMBER(9)
)"
                );

            Console.WriteLine("CREATE TABLE EMP");
        }

        public class Employee 
        {
            public string EmpNo { get; set; }

            public string EName { get; set; }

            public string Job { get; set; }

            public Int32 Mgr { get; set; }

            public DateTime Hiredate { get; set; }

            public decimal Sal { get; set; }

            public decimal Comm { get; set; }

            public Int32 DeptNo { get; set; }
        }

        static void Insert(IEnumerable<Employee> list)
        {
            const string cmdText = "insert into emp (EMPNO, ENAME, JOB, MGR, HIREDATE, SAL, COMM, DEPTNO) values (:EMPNO, :ENAME, :JOB, :MGR, :HIREDATE, :SAL, :COMM, :DEPTNO)";

            using (var conn = CreateConnection())
            {
                using (var cmd = CreateCommand())
                {
                    cmd.Connection = (OracleConnection)conn;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;

                    cmd.ArrayBindCount = list.Count();
                    cmd.Parameters.Add("EMPNO", OracleDbType.Varchar2, list.Select(c => c.EmpNo).ToArray(), ParameterDirection.Input);
                    cmd.Parameters.Add("ENAME", OracleDbType.Varchar2, list.Select(c => c.EName).ToArray(), ParameterDirection.Input);
                    cmd.Parameters.Add("JOB", OracleDbType.Varchar2, list.Select(c => c.Job).ToArray(), ParameterDirection.Input);
                    cmd.Parameters.Add("MGR", OracleDbType.Int32, list.Select(c => c.Mgr).ToArray(), ParameterDirection.Input);
                    cmd.Parameters.Add("HIREDATE", OracleDbType.Date, list.Select(c => c.Hiredate).ToArray(), ParameterDirection.Input);
                    cmd.Parameters.Add("SAL", OracleDbType.Decimal, list.Select(c => c.Sal).ToArray(), ParameterDirection.Input);
                    cmd.Parameters.Add("COMM", OracleDbType.Decimal, list.Select(c => c.Comm).ToArray(), ParameterDirection.Input);
                    cmd.Parameters.Add("DEPTNO", OracleDbType.Int32, list.Select(c => c.DeptNo).ToArray(), ParameterDirection.Input);
                    
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        cmd.Transaction = (OracleTransaction)tran;
                        cmd.ExecuteNonQuery();
                        tran.Commit();
                    }
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
            using (var conn = CreateConnection())
            {
                using (var cmd = CreateCommand())
                {
                    cmd.Connection = (OracleConnection)conn;
                    cmd.CommandType = CommandType.Text;
                    conn.Open();

                    using (var tran = conn.BeginTransaction())
                    {
                        foreach (var sql in sqlList)
                        {
                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                        }

                        tran.Commit();
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

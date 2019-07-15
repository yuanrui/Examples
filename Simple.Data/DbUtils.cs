using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Simple.Data
{
    public class DbUtils
    {
        public const string ConnectionName = "DefaultConnectionString";

        public static List<TEntity> Query<TEntity>(string sql, object param = null, CommandType? cmdType = CommandType.Text)
        {
            return new DbUtils<TEntity>(ConnectionName).Query(sql, param, cmdType);
        }

        public static List<dynamic> QueryDynamic(string sql, object param = null, CommandType? cmdType = CommandType.Text)
        {
            return new DbUtils<IEntityWrapper>(ConnectionName).QueryDynamic(sql, param, cmdType);
        }

        public static DataTable QueryDataTable(string sql)
        {
            return new DbUtils<IEntityWrapper>(ConnectionName).QueryDataTable(sql);
        }

        public static DataTable QueryDataTable(string sql, string tableName, CommandType cmdType = CommandType.Text)
        {
            return new DbUtils<IEntityWrapper>(ConnectionName).QueryDataTable(sql, tableName, cmdType);
        }

        public static int Execute(string sql, object param = null, CommandType? cmdType = CommandType.Text)
        {
            return new DbUtils<IEntityWrapper>(ConnectionName).Execute(sql, param, cmdType);
        }

        public static Object ExecuteScalar(string sql, object param = null, CommandType? cmdType = CommandType.Text)
        {
            return new DbUtils<IEntityWrapper>(ConnectionName).ExecuteScalar(sql, param, cmdType);
        }

        public static TEntity ExecuteScalar<TEntity>(string sql, object param = null, CommandType? cmdType = CommandType.Text)
        {
            return new DbUtils<TEntity>(ConnectionName).ExecuteScalar<TEntity>(sql, param, cmdType);
        }

        private interface IEntityWrapper { }
    }

    public class DbUtils<TEntity>
    {
        protected readonly string ConnectionName;

        public DbUtils(string connectionName)
        {
            ConnectionName = connectionName;
        }

        public List<TEntity> Query(string sql, object param = null, CommandType? cmdType = CommandType.Text)
        {
            using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin())
            {
                return scope.DataContext.Query<TEntity>(sql, param, cmdType);
            }
        }

        public List<dynamic> QueryDynamic(string sql, object param = null, CommandType? cmdType = CommandType.Text)
        {
            using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin())
            {
                return scope.DataContext.QueryDynamic(sql, param, cmdType);
            }
        }

        public int Execute(string sql, object param = null, CommandType? cmdType = CommandType.Text, bool useTran = true)
        {
            using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin(useTran))
            {
                var result = scope.DataContext.Execute(sql, param, cmdType);
                scope.Commit();

                return result;
            }
        }

        public Object ExecuteScalar(string sql, object param = null, CommandType? cmdType = CommandType.Text, bool useTran = false)
        {
            using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin(useTran))
            {
                var result = scope.DataContext.ExecuteScalar(sql, param, cmdType);
                scope.Commit();

                return result;
            }
        }

        public TObject ExecuteScalar<TObject>(string sql, object param = null, CommandType? cmdType = CommandType.Text, bool useTran = false) where TObject : TEntity
        {
            using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin(useTran))
            {
                var result = scope.DataContext.ExecuteScalar<TObject>(sql, param, cmdType);
                scope.Commit();

                return result;
            }
        }

        public DataTable QueryDataTable(string sql)
        {
            using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin())
            {
                return scope.DataContext.QueryDataTable(sql);
            }
        }

        public DataTable QueryDataTable(string sql, string tableName, CommandType cmdType = CommandType.Text)
        {
            using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin())
            {
                return scope.DataContext.QueryDataTable(sql, tableName, cmdType);
            }
        }
    }
}

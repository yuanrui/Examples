using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using Simple.Data;
using Banana.Entity;

namespace Banana.DataAccess
{
    public abstract class DataProviderBase<T>
    {
#if DEBUG
        protected readonly Stopwatch _stopwatch = new Stopwatch();
#endif
        protected const String DefaultDatabase = "DefaultConnectionString";
        private readonly String _connectionName;
        private readonly Database _db;
        private readonly DataContext _dataContext;

        protected const String PagerDataFormatSql = @"
SELECT * FROM
(
    SELECT a.*, rownum rn__
    FROM
    (
    	{0}
    ) a
    WHERE rownum < ((:PageIndex * :PageSize) + 1 )
)
WHERE rn__ >= (((:PageIndex-1) * :PageSize) + 1)";

        /// <summary>
        /// 采用row_number分页
        /// http://www.oracle.com/technetwork/issue-archive/2007/07-jan/o17asktom-093877.html
        /// <example>
        /// SELECT * FROM (
        ///        SELECT /*+ first_rows(15) */ A.*, row_number() over (order by 字段 DESC) rn__
        ///        FROM 表名称 A
        ///        WHERE  1=1 AND 其他条件
        /// )
        /// WHERE rn__ BETWEEN (((19-1) * 15) + 1) AND ((19 * 15) + 1 ) 
        /// ORDER BY rn__;
        /// </example>
        /// </summary>
        protected const String PagerDataFormatSql2 = @"
SELECT * FROM (
    {0}
)
WHERE rn__ BETWEEN ((({1}-1) * {2}) + 1) AND ({1} * {2})
ORDER BY rn__ ";

        protected const String PagerTotalFormatSql = @"SELECT COUNT(1) FROM ( {0} )";

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected String ConnectionName
        {
            get { return _connectionName; }
        }

        /// <summary>
        /// 数据库对象
        /// </summary>
        protected Database DatabaseObject
        {
            get
            {
                return this._db;
            }
        }

        /// <summary>
        /// 数据上下文
        /// </summary>
        protected DataContext DataContextObject
        {
            get
            {
                return _dataContext;
            }
        }

        public DataProviderBase()
            : this(DefaultDatabase)
        {
        }

        public DataProviderBase(string connectionName)
        {
            _connectionName = connectionName;
            _dataContext = DataContextScope.GetCurrent(connectionName).DataContext;
            _db = _dataContext.DatabaseObject;
        }

        #region Sql注入防御

        /// <summary>
        /// 筛选输入可以删除转义符,这也可能有助于防止 SQL 注入.但由于可引起问题的字符数量很大,因此这并不是一种可靠的防护方法.
        /// 或者使用 引用运算符q 格式为:q'[quote char]string[quote char]'
        /// [quote char] 任何单字节,多字节字符或者四种括号--圆括号(),花括号{},方括号[]或者尖括号&lt;&gt;
        /// eg1.SELECT * FROM Table WHERE Column = Q'[Value]';
        /// eg2.SELECT * FROM Table WHERE Column LIKE Q'[%Value%]';
        /// </summary>
        /// <param name="inputSql"></param>
        /// <returns></returns>
        public virtual string SafeSqlLiteral(string inputSql)
        {
            return SafeSqlLiteral(inputSql, false);
        }

        /// <summary>
        /// 筛选输入可以删除转义符,这也可能有助于防止 SQL 注入.但由于可引起问题的字符数量很大,因此这并不是一种可靠的防护方法.
        /// 使用 LIKE 子句,需对通配符字符进行转义
        /// 或者使用 引用运算符q 格式为:q'[quote char]string[quote char]'
        /// [quote char] 任何单字节,多字节字符或者四种括号--圆括号(),花括号{},方括号[]或者尖括号&lt;&gt;
        /// eg1.SELECT * FROM Table WHERE Column = Q'[Value]';
        /// eg2.SELECT * FROM Table WHERE Column LIKE Q'[%Value%]';
        /// </summary>
        /// <param name="inputSql"></param>
        /// <param name="useLike">输入的字段是否用于LIKE子局</param>
        /// <returns></returns>
        public virtual string SafeSqlLiteral(string inputSql, bool useLike)
        {
            if (inputSql == null)
            {
                return string.Empty;
            }

            var s = inputSql.Replace("'", "''");

            if (useLike)
            {
                s = s.Replace("%", "\\%");
                s = s.Replace("_", "\\_");
            }

            return s;
        }

        #endregion

        #region 分页

        public virtual Pager<T> GetPager(Func<IDataReader, T> fillFunc, Int32 pageIndex, Int32 pageSize, String tableName, String primaryKeys, String sortBy, String fields, String whereClause)
        {
            var fillResult = new List<T>();
            var cmd = DatabaseObject.GetStoredProcCommand("SP_PAGING");
            DatabaseObject.AddInParameter(cmd, "TableName", DbType.String, tableName);
            DatabaseObject.AddInParameter(cmd, "PrimaryKeys", DbType.String, primaryKeys);
            DatabaseObject.AddInParameter(cmd, "PageIndex", DbType.Int32, pageIndex);
            DatabaseObject.AddInParameter(cmd, "PageSize", DbType.Int32, pageSize);
            DatabaseObject.AddInParameter(cmd, "Fields", DbType.String, fields);
            DatabaseObject.AddInParameter(cmd, "SortBy", DbType.String, sortBy);
            DatabaseObject.AddInParameter(cmd, "WhereClause", DbType.String, whereClause);

            var resultParam = new OracleParameter("Result", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.Parameters.Add(resultParam);
            var totalParam = cmd.CreateParameter();
            totalParam.DbType = DbType.Int64; totalParam.ParameterName = "Total"; totalParam.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(totalParam);

            using (var reader = DataContextObject.ExecuteReader(cmd))
            {
                fillResult = new List<T>();
                while (reader.Read())
                {
                    var ent = fillFunc(reader);
                    if (ent != null)
                    {
                        fillResult.Add(ent);
                    }
                }
            }
            var total = Convert.ToInt64(totalParam.Value);
            var pager = new Pager<T>(pageSize, pageIndex, total, fillResult);

            return pager;
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="TEntity">类型TEntity</typeparam>
        /// <param name="subQuerySql">子查询</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual Pager<TEntity> GetPager<TEntity>(String subQuerySql, Int32 pageIndex, Int32 pageSize)
        {
            var sql = GetPageDataSql(subQuerySql);
            var totalSql = String.Format(PagerTotalFormatSql, subQuerySql);

            var pager = GetPager<TEntity>(sql, totalSql, pageIndex, pageSize);
            return pager;
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="TEntity">类型TEntity</typeparam>
        /// <param name="pageDataSql">查询分页数据Sql Sql格式如下:
        /// SELECT * FROM
        /// (
        ///    SELECT a.*, rownum r__
        ///    FROM
        ///    (
        ///        SELECT * FROM YourTable WHERE Id=1 AND Name='ABC' ORDER BY Id DESC
        ///    ) a
        ///    WHERE rownum &lt; ((:PageIndex * :PageSize) + 1 )
        /// )
        /// WHERE r__ >= (((:PageIndex-1) * :PageSize) + 1)
        /// </param>
        /// <param name="pageTotalSql">查询分页总数Sql Sql格式如下:
        /// SELECT COUNT(1) FROM YourTable WHERE Id=1 AND Name='ABC'
        /// </param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual Pager<TEntity> GetPager<TEntity>(String pageDataSql, String pageTotalSql, Int32 pageIndex, Int32 pageSize)
        {
#if DEBUG
            Debug.WriteLine(string.Format("{0}{1}{0}", Environment.NewLine, pageDataSql.Replace(":PageIndex", pageIndex.ToString()).Replace(":PageSize", pageSize.ToString())));
            _stopwatch.Start();
#endif

            var cmd = DatabaseObject.GetSqlStringCommand(pageTotalSql);
            var total = Convert.ToInt64(DataContextObject.ExecuteScalar(cmd));

            if (total == 0)
            {
                return new Pager<TEntity>(pageSize, pageIndex, total, new List<TEntity>());
            }

#if DEBUG
            _stopwatch.Stop();
            Debug.WriteLine(string.Format("page data sql:{0}{1}{0}total seconds:{2}s", Environment.NewLine, pageDataSql.Replace(":PageIndex", pageIndex.ToString()).Replace(":PageSize", pageSize.ToString()), _stopwatch.Elapsed.TotalSeconds));
            _stopwatch.Restart();
#endif

            var list = DataContextObject.Query<TEntity>(pageDataSql, new { PageIndex = pageIndex, PageSize = pageSize });

#if DEBUG
            _stopwatch.Stop();
            Debug.WriteLine(string.Format("page total sql:{0}{1}{0}total seconds:{2}s", Environment.NewLine, pageTotalSql, _stopwatch.Elapsed.TotalSeconds));
#endif

            var pager = new Pager<TEntity>(pageSize, pageIndex, total, list);
            return pager;
        }

        /// <summary>
        /// 获取分页sql
        /// </summary>
        /// <example>
        /// SELECT * FROM
        /// (
        ///     SELECT a.*, rownum rn__
        ///     FROM
        ///     (
        ///     	{0}
        ///     ) a
        ///     WHERE rownum < ((:PageIndex * :PageSize) + 1 )
        /// )
        /// WHERE rn__ >= (((:PageIndex-1) * :PageSize) + 1)
        /// </example>
        /// <param name="subQuerySql">子查询</param>
        /// <returns></returns>
        public virtual string GetPageDataSql(String subQuerySql)
        {
            var sql = String.Format(PagerDataFormatSql, subQuerySql);
            return sql;
        }

        /// <summary>
        /// 采用row_number分页
        /// http://www.oracle.com/technetwork/issue-archive/2007/07-jan/o17asktom-093877.html
        /// <example>
        ///SELECT * FROM (
        ///    {0}
        ///)
        ///WHERE rn__ BETWEEN ((({1}-1) * {2}) + 1) AND ({1} * {2})
        ///ORDER BY rn__ 
        /// </example>
        /// <param name="subQuerySql">
        /// SELECT /*+ first_rows(15) */ A.*, row_number() over (order by 字段 DESC) rn__
        /// FROM 表名称 A
        /// WHERE  1=1 AND 其他条件
        /// </param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// </summary>
        public virtual string GetPageDataSql2(String subQuerySql, Int32 pageIndex, Int32 pageSize)
        {
            if (!subQuerySql.Contains("rn__"))
            {
                throw new ArgumentException("select语句必须存在 row_number() over (order by 排序字段 DESC) rn__ ");
            }

            var sql = String.Format(PagerDataFormatSql2, subQuerySql, pageIndex, pageSize);
            return sql;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Dapper;

namespace Simple.Data
{
    public class DataContext : IDisposable
    {
        protected const String ConnectionName = "DefaultConnectionString";
        private DbConnection _dbConnection;

        private DbTransaction _dbTransaction;

        private readonly DbProviderFactory _dbProviderFactory;

        private Database _database;

        public virtual Database DatabaseObject
        {
            get { return _database; }
        }
        
        internal DbProviderFactory DbProviderFactory
        {
            get { return _dbProviderFactory; }
        }

        public Boolean IsDisposed { get; private set; }
        public Boolean IsTransactionOpened { get; private set; }

        public DataContext() : this(ConnectionName)
        {
            //_database = DatabaseFactory.CreateDatabase();
            //_dbConnection = _database.CreateConnection();
        }

        public DataContext(String connectionName)
        {
            var connSetting = ConfigurationManager.ConnectionStrings[connectionName];
            var connectionString = Environment.GetEnvironmentVariable(connectionName);
            if (string.IsNullOrWhiteSpace(connectionString) && connSetting != null)
            {
                connectionString = connSetting.ConnectionString;
            }

            var providerName = Environment.GetEnvironmentVariable(connectionName + ".ProviderName");
            if (string.IsNullOrWhiteSpace(providerName) && connSetting != null)
            {
                providerName = connSetting.ProviderName;
            }

            _dbProviderFactory = DbProviderFactories.GetFactory(providerName);
            _database = new Database(_dbProviderFactory);
            _dbConnection = _dbProviderFactory.CreateConnection();
            _dbConnection.ConnectionString = connectionString;
        }

        public virtual void OpenConnection()
        {
            if (_dbConnection.State == ConnectionState.Open)
            {
                return;
            }

            //if (IsDisposed)
            //{
            //    _dbConnection = _database.CreateConnection();
            //}

            _dbConnection.Open();
            _dbTransaction = null;
            IsDisposed = false;
        }

        public virtual void BeginTransaction()
        {
            _dbTransaction = _dbConnection.BeginTransaction();
            IsTransactionOpened = true;
        }

        public virtual void BeginTransaction(IsolationLevel isolationLevel)
        {
            _dbTransaction = _dbConnection.BeginTransaction(isolationLevel);
            IsTransactionOpened = true;
        }

        public virtual void CommitTransaction()
        {
            if (!IsTransactionOpened)
            {
                throw new InvalidOperationException("事务未开启, 无法提交");
            }

            try
            {
                _dbTransaction.Commit();
            }
            finally
            {
                _dbTransaction.Dispose();
                _dbConnection.Close();
                //_dbConnection.Dispose();
                IsTransactionOpened = false;
            }
        }

        public virtual void RollbackTransaction()
        {
            if (!IsTransactionOpened)
            {
                throw new InvalidOperationException("事务未开启, 无法回滚");
            }

            try
            {
                _dbTransaction.Rollback();
            }
            finally
            {
                _dbTransaction.Dispose();
                _dbConnection.Close();
                //_dbConnection.Dispose();
                IsTransactionOpened = false;
            }
        }

        public virtual IDataReader ExecuteReader(DbCommand dbCommand)
        {
            dbCommand.Connection = _dbConnection;
            dbCommand.Transaction = _dbTransaction;
            
            return dbCommand.ExecuteReader();
        }

        public virtual Object ExecuteScalar(DbCommand dbCommand)
        {
            dbCommand.Connection = _dbConnection;
            dbCommand.Transaction = _dbTransaction;
            
            return dbCommand.ExecuteScalar();
        }

        public virtual Int32 ExecuteNonQuery(DbCommand dbCommand)
        {
            dbCommand.Connection = _dbConnection;
            dbCommand.Transaction = _dbTransaction;

            return dbCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Sample query
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="cmdType"></param>
        /// <returns></returns>
        public virtual List<TEntity> Query<TEntity>(String sql, object param = null, CommandType? cmdType = CommandType.Text)
        {
            return _dbConnection.Query<TEntity>(sql, param, transaction: _dbTransaction, commandType: cmdType).ToList();
        }

        /// <summary>
        /// Query one to one 
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <param name="sql"></param>
        /// <param name="map"></param>
        /// <param name="splitOn"></param>
        /// <param name="param"></param>
        /// <param name="cmdType"></param>
        /// <returns></returns>
        public virtual List<TFirst> Query<TFirst, TSecond>(String sql, Func<TFirst, TSecond, TFirst> map, String splitOn, object param = null, CommandType? cmdType = CommandType.Text)
        {
            return _dbConnection.Query<TFirst, TSecond, TFirst>(sql, map, param, transaction: _dbTransaction, splitOn: splitOn, commandType: cmdType).ToList();
        }

        /// <summary>
        /// Query one to many
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <typeparam name="TChild"></typeparam>
        /// <typeparam name="TParentKey"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parentKeySelector"></param>
        /// <param name="childSelector"></param>
        /// <param name="splitOn"></param>
        /// <param name="param"></param>
        /// <param name="cmdType"></param>
        /// <returns></returns>
        public virtual List<TParent> Query<TParent, TChild, TParentKey>(String sql, Func<TParent, TParentKey> parentKeySelector, Func<TParent, ICollection<TChild>> childSelector, String splitOn = "ID", object param = null, CommandType? cmdType = CommandType.Text)
        {
            var cache = new Dictionary<TParentKey, TParent>();

            _dbConnection.Query<TParent, TChild, TParent>(sql, (parent, child) =>
            {
                if (!cache.ContainsKey(parentKeySelector(parent)))
                {
                    cache.Add(parentKeySelector(parent), parent);
                }

                TParent cachedParent = cache[parentKeySelector(parent)];
                var children = childSelector(cachedParent);
                children.Add(child);
                return cachedParent;
            }, param, _dbTransaction, splitOn: splitOn, commandType: cmdType);

            return cache.Values.ToList();
        }

        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>> QueryMultiple<TFirst, TSecond>(String sql, object param = null, CommandType? cmdType = CommandType.Text)
        {
            SqlMapper.GridReader gridReader = _dbConnection.QueryMultiple(sql, param, transaction: _dbTransaction, commandType: cmdType);

            using (gridReader)
            {
                return Tuple.Create(gridReader.Read<TFirst>(), gridReader.Read<TSecond>());
            }
        }

        public virtual List<dynamic> QueryDynamic(String sql, object param = null, CommandType? cmdType = CommandType.Text)
        {
            return _dbConnection.Query(sql, param, transaction: _dbTransaction, commandType: cmdType).ToList();
        }

        public virtual DataTable QueryDataTable(String sql)
        {
            return QueryDataTable(sql, String.Empty);
        }

        public virtual DataTable QueryDataTable(String sql, String tableName, CommandType cmdType = CommandType.Text)
        {
            using (var cmd = _dbConnection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = cmdType;
                
                using (var adapter = _dbProviderFactory.CreateDataAdapter())
                {
                    var table = String.IsNullOrEmpty(tableName) ? new DataTable() : new DataTable(tableName);

                    adapter.SelectCommand = cmd;
                    adapter.Fill(table);
                    
                    return table;
                }
            }
        }

        public virtual Int32 Execute(String sql, object param = null, CommandType? cmdType = CommandType.Text)
        {
            return _dbConnection.Execute(sql, param, transaction: _dbTransaction, commandType: cmdType);
        }

        public virtual Object ExecuteScalar(String sql, object param = null, CommandType? cmdType = CommandType.Text)
        {
            return _dbConnection.ExecuteScalar(sql, param, transaction: _dbTransaction, commandType: cmdType);
        }

        public virtual TEntity ExecuteScalar<TEntity>(String sql, object param = null, CommandType? cmdType = CommandType.Text)
        {
            return _dbConnection.ExecuteScalar<TEntity>(sql, param, transaction: _dbTransaction, commandType: cmdType);
        }

        public void Dispose()
        {
            DoDispose(false);
        }

        internal void DoDispose(bool disposeConnection)
        {
            if (!IsDisposed)
            {
                if (_dbTransaction != null)
                {
                    _dbTransaction.Dispose();
                    _dbTransaction = null;
                }

                _dbConnection.Close();

                IsDisposed = true;
            }

            if (disposeConnection)
            {
                _dbConnection.Dispose();
                IsDisposed = true;
            }
        }
    }
}

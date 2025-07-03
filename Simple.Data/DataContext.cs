// Copyright (c) 2017 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

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
        private String _connectionString;

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

        public virtual Boolean IsDisposed { get; private set; }
        public virtual Boolean IsTransactionOpened { get; private set; }

        public DataContext() : this(ConnectionName)
        {

        }

        public DataContext(String connectionName)
        {
            var connSetting = ConfigurationManager.ConnectionStrings[connectionName];
            _connectionString = Environment.GetEnvironmentVariable(connectionName);
            if (String.IsNullOrWhiteSpace(_connectionString) && connSetting != null)
            {
                _connectionString = connSetting.ConnectionString;
            }

            var providerName = Environment.GetEnvironmentVariable(connectionName + ".ProviderName");
            if (String.IsNullOrWhiteSpace(providerName) && connSetting != null)
            {
                providerName = connSetting.ProviderName;
            }

            _dbProviderFactory = DbProviderFactories.GetFactory(providerName);
            _database = new Database(_connectionString, _dbProviderFactory);
            _dbConnection = _dbProviderFactory.CreateConnection();
            _dbConnection.ConnectionString = _connectionString;
        }

        public virtual void OpenConnection()
        {
            if (_dbConnection == null || IsDisposed)
            {
                _dbConnection = _database.CreateConnection();
                _dbConnection.ConnectionString = _connectionString;
            }

            if (_dbConnection.State == ConnectionState.Open)
            {
                IsDisposed = false;
                return;
            }

            //if (IsDisposed)
            //{
            //    _dbConnection = _database.CreateConnection();
            //}

            _dbConnection.Open();
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
        public virtual List<TEntity> Query<TEntity>(String sql, Object param = null, CommandType? cmdType = CommandType.Text)
        {
            return _dbConnection.Query<TEntity>(sql, (Object)param, transaction: _dbTransaction, commandType: cmdType).ToList();
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
        public virtual List<TFirst> Query<TFirst, TSecond>(String sql, Func<TFirst, TSecond, TFirst> map, String splitOn, Object param = null, CommandType? cmdType = CommandType.Text)
        {
            return _dbConnection.Query<TFirst, TSecond, TFirst>(sql, map, (Object)param, transaction: _dbTransaction, splitOn: splitOn, commandType: cmdType).ToList();
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
        public virtual List<TParent> Query<TParent, TChild, TParentKey>(String sql, Func<TParent, TParentKey> parentKeySelector, Func<TParent, ICollection<TChild>> childSelector, String splitOn = "ID", Object param = null, CommandType? cmdType = CommandType.Text)
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
            }, param as Object, _dbTransaction, splitOn: splitOn, commandType: cmdType);

            return cache.Values.ToList();
        }

        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>> QueryMultiple<TFirst, TSecond>(String sql, Object param = null, CommandType? cmdType = CommandType.Text)
        {
            SqlMapper.GridReader gridReader = _dbConnection.QueryMultiple(sql, (Object)param, transaction: _dbTransaction, commandType: cmdType);

            using (gridReader)
            {
                return Tuple.Create(gridReader.Read<TFirst>(), gridReader.Read<TSecond>());
            }
        }

        public virtual List<dynamic> QueryDynamic(String sql, Object param = null, CommandType? cmdType = CommandType.Text)
        {
            return _dbConnection.Query(sql, (Object)param, transaction: _dbTransaction, commandType: cmdType).ToList();
        }

        public virtual DataTable QueryDataTable(String sql)
        {
            return QueryDataTable(sql, String.Empty);
        }

        public virtual DataTable QueryDataTable(String sql, DbParameter[] parameters)
        {
            return QueryDataTable(sql, String.Empty, parameters);
        }

        public virtual DataTable QueryDataTable(String sql, String tableName, CommandType cmdType = CommandType.Text)
        {
            return QueryDataTable(sql, tableName, null, cmdType);
        }

        public virtual DataTable QueryDataTable(String sql, String tableName, DbParameter[] parameters, CommandType cmdType = CommandType.Text)
        {
            using (var cmd = _dbConnection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = cmdType;

                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using (var adapter = _database.GetDataAdapter())
                {
                    var table = String.IsNullOrEmpty(tableName) ? new DataTable() : new DataTable(tableName);

                    adapter.SelectCommand = cmd;
                    adapter.Fill(table);

                    return table;
                }
            }
        }

        public virtual Object ExecuteScalar(String sql, Object param = null, CommandType? cmdType = CommandType.Text)
        {
            return _dbConnection.ExecuteScalar(sql, (Object)param, transaction: _dbTransaction, commandType: cmdType);
        }

        public virtual TEntity ExecuteScalar<TEntity>(String sql, Object param = null, CommandType? cmdType = CommandType.Text)
        {
            return _dbConnection.ExecuteScalar<TEntity>(sql, (Object)param, transaction: _dbTransaction, commandType: cmdType);
        }

        public virtual int Execute(String sql, Object param = null, CommandType? cmdType = CommandType.Text)
        {
            return _dbConnection.Execute(sql, (Object)param, transaction: _dbTransaction, commandType: cmdType);
        }

        public virtual void Dispose()
        {
            DoDispose(true);
        }

        internal void DoDispose(bool disposeConnection)
        {
            if (!IsDisposed)
            {
                if (_dbConnection != null)
                {
                    _dbConnection.Close();
                }

                //IsDisposed = false;
            }

            if (_dbTransaction != null)
            {
                _dbTransaction.Dispose();
                _dbTransaction = null;
            }

            if (disposeConnection)
            {
                if (_dbConnection != null)
                {
                    _dbConnection.Dispose();
                    _dbConnection = null;
                }

                IsDisposed = true;
            }
        }
    }
}

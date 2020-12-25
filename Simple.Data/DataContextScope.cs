using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
#if NET
using System.Web;
#endif

namespace Simple.Data
{
    public class DataContextScope : IAutoCloseable, IDisposable
    {
        private const string KeyPrefix = "Simple.DataContextScope.";
        private const string DefaultConnectionName = "DefaultConnectionString";
#if NETCOREAPP2_1 || NETSTANDARD
        static AsyncLocal<DataContextScope> Local = new AsyncLocal<DataContextScope>();
#else
        static ThreadLocal<DataContextScope> Local = new ThreadLocal<DataContextScope>();
#endif

        public DataContext DataContext { get; private set; }

        public static DataContextScope Current
        {
            get
            {
                DataContextScope ctx = GetCurrent();
                return ctx;
            }
        }

        public static DataContextScope GetCurrent(string connectionName = null)
        {
            if (string.IsNullOrEmpty(connectionName))
            {
                connectionName = DefaultConnectionName;
            }
            var key = KeyPrefix + connectionName;
            
            DataContextScope ctx = null;
#if NETCOREAPP2_1 || NETSTANDARD
            ctx = Local.Value;

            if (ctx == null)
            {
                ctx = new UnitOfWork(connectionName);
                Local.Value = ctx;
            }

            return ctx;
#elif NET
            if (HttpContext.Current != null)
            {
                ctx = HttpContext.Current.Items[key] as UnitOfWork;

                if (ctx == null)
                {
                    ctx = new UnitOfWork(connectionName);
                    HttpContext.Current.Items[key] = ctx;
                }

                return ctx;
            }
            else
            {
                ctx = Local.Value;

                if (ctx == null)
                {
                    ctx = new UnitOfWork(connectionName);
                    Local.Value = ctx;
                }

                return ctx;
            }

#endif
            return ctx;
        }

        private DataContextScope() : this(DefaultConnectionName)
        {
            
        }

        private DataContextScope(string connectionName)
        {
            DataContext = new DataContext(connectionName);            
        }

        public DataContextScope Begin()
        {
            return Begin(false);
        }

        public DataContextScope Begin(bool useTran)
        {
            DataContext.OpenConnection();

            if (useTran)
            {
                DataContext.BeginTransaction();
            }
            
            return this;
        }

        public DataContextScope Begin(IsolationLevel isolationLevel)
        {
            DataContext.OpenConnection();
            DataContext.BeginTransaction(isolationLevel);

            return this;
        }
        
        public void Commit()
        {
            if (DataContext.IsTransactionOpened)
            {
                DataContext.CommitTransaction();
            }
        }

        public void Rollback()
        {
            if (DataContext.IsTransactionOpened)
            {
                DataContext.RollbackTransaction();
            }
        }

        public void Dispose()
        {
            DataContext.Dispose();
        }

        void IAutoCloseable.Dispose()
        {
            DataContext.DoDispose(true);
        }
    }
}

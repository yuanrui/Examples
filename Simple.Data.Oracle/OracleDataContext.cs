using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace Simple.Data.Oracle
{
    public class OracleDataContext : DataContext
    {
        protected virtual void SetBindByName(DbCommand dbCommand)
        {
            var cmd = dbCommand as OracleCommand;

            if (cmd == null)
            {
                return;
            }

            cmd.BindByName = true;
        }

        public override IDataReader ExecuteReader(DbCommand dbCommand)
        {
            SetBindByName(dbCommand);

            return base.ExecuteReader(dbCommand);
        }

        public override int ExecuteNonQuery(DbCommand dbCommand)
        {
            SetBindByName(dbCommand);

            return base.ExecuteNonQuery(dbCommand);
        }

        public override object ExecuteScalar(DbCommand dbCommand)
        {
            SetBindByName(dbCommand);

            return base.ExecuteScalar(dbCommand);
        }
    }
}

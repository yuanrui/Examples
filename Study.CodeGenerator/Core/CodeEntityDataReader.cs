using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Study.CodeGenerator.Core
{
    public class CodeEntityDataReader
    {
        private readonly string connStr;
        private CodeEntityDataReader() { }
        public CodeEntityDataReader(string _connStr)
        {
            this.connStr = _connStr;
        }

        public IList<Column> GetColumns(Table table)
        {
            string sql = string.Format(@"
WITH colConsCTE(TABLE_NAME, COLUMN_NAME, CONSTRAINT_TYPE) AS (
     SELECT A.TABLE_NAME, A.COLUMN_NAME, B.CONSTRAINT_TYPE FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE A
     LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS B ON A.CONSTRAINT_NAME = B.CONSTRAINT_NAME AND A.TABLE_NAME = B.TABLE_NAME 
)
SELECT a.object_id AS TableID, a.name AS TableName, b.column_id AS ID, b.name as Name, d.value AS Comment , c.name AS DBType, b.max_length AS 'Length',
	CASE WHEN (SELECT COUNT(1) FROM colConsCTE cte WHERE b.name = cte.COLUMN_NAME AND a.name = cte.TABLE_NAME AND cte.CONSTRAINT_TYPE ='PRIMARY KEY') > 0 THEN 1 ELSE 0 END AS IsPrimaryKey,
	CASE WHEN (SELECT COUNT(1) FROM colConsCTE cte WHERE b.name = cte.COLUMN_NAME AND a.name = cte.TABLE_NAME AND cte.CONSTRAINT_TYPE ='FOREIGN KEY') > 0 THEN 1 ELSE 0 END AS IsForeignKey,
	b.IS_NULLABLE as 'IsNullAble'
FROM sys.tables a 
LEFT JOIN sys.columns b on a.object_id = b.object_id
LEFT JOIN sys.types c on b.user_type_id = c.user_type_id
LEFT JOIN sys.extended_properties d on a.object_id = d.major_id and d.minor_id = b.column_id
WHERE a.name='{0}'", table.Name);

            DataTable dt = DBUtil.GetDataTable(connStr, sql);
            List<Column> list = new List<Column>();
            foreach (DataRow row in dt.Rows)
            {
                Column column = new Column();
                column.ID = row["ID"].ToString();
                column.Name = row["Name"].ToString();
                column.Comment = row["Comment"].ToString();
                column.DBType = row["DBType"].ToString();
                column.IsPrimaryKey = row["IsPrimaryKey"].ToString() == "1";
                column.IsForeignKey = row["IsForeignKey"].ToString() == "1";
                column.IsNullAble = bool.Parse(row["IsNullAble"].ToString());
                column.Table = table;
                list.Add(column);
            }

            table.Columns = list;

            return list;    
        }

        public IList<Table> GetTables()
        {
            string sql = @"select a.object_id as ID, a.name as Name, b.value as Comment from sys.tables a 
left join sys.extended_properties b on a.object_id = b.major_id and minor_id =0
inner join sys.tables c on a.object_id = c.object_id";

            DataTable dt = DBUtil.GetDataTable(connStr, sql);
            List<Table> list = new List<Table>();
            foreach (DataRow row in dt.Rows)
            {
                Table table = new Table();
                table.ID = row["ID"].ToString();
                table.Name = row["Name"].ToString();
                table.Comment = row["Comment"].ToString();

                list.Add(table);
            }

            return list;
        }
    }
}

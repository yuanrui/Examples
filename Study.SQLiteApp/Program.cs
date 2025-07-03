using SQLite;
using System;
using System.Data.SQLite;
using System.IO;
using SQLiteConnection = SQLite.SQLiteConnection;

namespace Study.SQLiteApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //SQLiteConnection.CreateFile("test.db");
            //var conn = new SQLiteConnection("Data Source=test.db");
            //conn.ChangePassword("");
            var databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cipher.db");
            var options = new SQLiteConnectionString(databasePath, true, key: "123456");
            //var encryptedDb = new SQLiteAsyncConnection(options);
            SQLiteConnection connection = new SQLiteConnection(options);
            var sql = @"
CREATE TABLE IF NOT EXISTS `T_SQL_QUERY_TASK_LIST` (
    `ID`	nvarchar NOT NULL,
    `TOTAL_TASKS`	nvarchar,
    `TABLE_PREFIX`	nvarchar,
    `DAYS`	int,
    `CONDITION_JSON`	nvarchar,
    `CONDITION_TYPE`	nvarchar,
    `USE_ASC`	bit,
    `CREATED_AT`	datetime NOT NULL,
    PRIMARY KEY(`Id`)
);";
            connection.Execute(sql);
            Console.WriteLine("Hello World!");
        }
    }
}

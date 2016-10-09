using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Simple.Data.Oracle
{
    class Program
    {
        static void Main(string[] args)
        {
            var table = DbUtils.QueryDataTable("SELECT * FROM T_SYS_ACCOUNT");

            foreach (DataRow row in table.Rows)
            {
                Console.WriteLine(row["NAME"]);
            }

            Console.WriteLine("\nPress Any Key To Exit...");
            Console.ReadLine();
        }
    }
}

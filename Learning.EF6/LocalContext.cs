//------------------------------------------------------------------------------
// <copyright file="SqliteContext.cs" company="CQ Ebos Co., Ltd.">
//    Copyright (c) 2016, CQ Ebos Co., Ltd. All rights reserved.
// </copyright>
// <author>Yuan Rui</author>
// <email>yuanrui@live.cn</email>
// <date>2016-01-08 14:54:51</date>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Learning.EF6
{
    public class LocalContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }

        public LocalContext()
            : base("DefaultConnectionString")
        {
            this.Database.Log = Console.Write;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new SqliteContextInitializer<LocalContext>(this.Database, modelBuilder));
            
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}

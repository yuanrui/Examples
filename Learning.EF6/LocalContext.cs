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
        
        public DbSet<Joke> Jokes { get; set; }

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

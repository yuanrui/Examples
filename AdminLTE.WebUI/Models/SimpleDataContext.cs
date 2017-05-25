using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Web;
using AdminLTE.WebUI.Common;

namespace AdminLTE.WebUI.Models
{
    public class SimpleDataContext : DbContext
    {
        const string CURRENT_KEY = "__SimpleDataContext";

        public static SimpleDataContext Current 
        {
            get 
            {
                if (HttpContext.Current == null)
	            {
                    return new SimpleDataContext();
	            }

                var result = HttpContext.Current.Items[CURRENT_KEY] as SimpleDataContext;
                if (result == null)
                {
                    result = new SimpleDataContext();
                    HttpContext.Current.Items[CURRENT_KEY] = result;
                }

                return result;
            }
        }

        public SimpleDataContext()
            : base("DefaultConnectionString")
        {
            this.Database.Log = m => {
                Debug.WriteLine(m);
            };
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PermissionEntity>().ToTable("T_Permission").HasKey(m => m.Id);

            modelBuilder.Entity<RoleEntity>().ToTable("T_Role").HasKey(m => m.Id)
                .HasMany(m => m.Permissions)
                .WithMany(m => m.Roles)
                .Map(m =>
                {
                    m.MapLeftKey("RoleId");
                    m.MapRightKey("PermissionId");
                    m.ToTable("T_Role_Permission");
                });

            modelBuilder.Entity<AccountEntity>().ToTable("T_Account").HasKey(m => m.Id)
                .HasMany(m => m.Roles)
                .WithMany(m => m.Accounts)
                .Map(m => 
                {
                    m.MapLeftKey("AccountId");
                    m.MapRightKey("RoleId");
                    m.ToTable("T_Account_Role");
                });

            Database.SetInitializer(new SqliteContextInitializer<SimpleDataContext>(this.Database, modelBuilder));
            
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
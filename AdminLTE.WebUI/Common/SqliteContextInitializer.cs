using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Mapping;

namespace AdminLTE.WebUI.Common
{
    /// <summary>
    /// from https://gist.github.com/flaub/1968486e1b3f2b9fddaf
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqliteContextInitializer<T> : IDatabaseInitializer<T>
        where T : DbContext
    {
        Database _db;
        DbModelBuilder _modelBuilder;

        public SqliteContextInitializer(Database db, DbModelBuilder modelBuilder)
        {
            _db = db;
            _modelBuilder = modelBuilder;
        }

        public void InitializeDatabase(T context)
        {
            _db.CreateIfNotExists();

            var model = _modelBuilder.Build(context.Database.Connection);

            using (var xact = context.Database.BeginTransaction())
            {
                try
                {
                    CreateDatabase(context, model);
                    xact.Commit();
                }
                catch (Exception)
                {
                    xact.Rollback();
                    //throw;
                }
            }
        }

        class Index
        {
            public string Name { get; set; }
            public string Table { get; set; }
            public List<string> Columns { get; set; }
        }

        private string FixTableName(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return string.Empty;
            }
            return tableName;
            if (! tableName.StartsWith("T_"))
            {
                tableName = "T_" + tableName;
            }

            tableName = tableName.Replace("Entity", "_");

            return tableName.Trim('_');
        }

        private string GetTableName(string typeName, DbContext context)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(metadata);
            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

            // Get the entity type from the model that maps to the CLR type
            var entityType = metadata
                    .GetItems<EntityType>(DataSpace.OSpace)
                    .FirstOrDefault(e => objectItemCollection.GetClrType(e).Name == typeName);

            if (entityType == null)
            {
                return typeName;
            }

            // Get the entity set that uses this entity type
            var entitySet = metadata
                .GetItems<EntityContainer>(DataSpace.CSpace)
                .Single()
                .EntitySets
                .FirstOrDefault(s => s.ElementType.Name == entityType.Name);

            if (entityType == null)
            {
                return typeName;
            }

            // Find the mapping between conceptual and storage model for this entity set
            var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                    .Single()
                    .EntitySetMappings
                    .FirstOrDefault(s => s.EntitySet == entitySet);

            if (mapping == null)
            {
                return typeName;
            }

            // Find the storage entity set (table) that the entity is mapped
            var table = mapping
                .EntityTypeMappings.Single()
                .Fragments.Single()
                .StoreEntitySet;

            // Return the table name from the storage entity set
            return (string)table.MetadataProperties["Table"].Value ?? table.Name;
        }


        private void CreateDatabase(DbContext context, DbModel model)
        {
            const string tableTmpl = "CREATE TABLE IF NOT EXISTS [{0}] (\n{1}\n);";
            const string columnTmpl = "    [{0}] {1} {2}"; // name, type, decl
            const string primaryKeyTmpl = "    PRIMARY KEY ({0})";
            const string foreignKeyTmpl = "    FOREIGN KEY ({0}) REFERENCES {1} ({2})";
            const string indexTmpl = "CREATE INDEX IF NOT EXISTS {0} ON {1} ({2});";
            
            var db = context.Database;
            var indicies = new Dictionary<string, Index>();
            
            foreach (var type in model.StoreModel.EntityTypes)
            {
                var defs = new List<string>();
                
                // columns
                foreach (var p in type.Properties)
                {
                    var decls = new HashSet<string>();

                    if (!p.Nullable)
                        decls.Add("NOT NULL");

                    var annotations = p.MetadataProperties
                        .Select(x => x.Value)
                        .OfType<IndexAnnotation>();

                    foreach (var annotation in annotations)
                    {
                        foreach (var attr in annotation.Indexes)
                        {
                            if (attr.IsUnique)
                                decls.Add("UNIQUE");

                            if (string.IsNullOrEmpty(attr.Name))
                                continue;

                            Index index;
                            if (!indicies.TryGetValue(attr.Name, out index))
                            {
                                index = new Index
                                {
                                    Name = attr.Name,
                                    Table = type.Name,
                                    Columns = new List<string>(),
                                };
                                indicies.Add(index.Name, index);
                            }
                            index.Columns.Add(p.Name);
                        }
                    }

                    defs.Add(string.Format(columnTmpl, p.Name, p.TypeName, string.Join(" ", decls)));
                }

                // primary keys
                if (type.KeyProperties.Any())
                {
                    var keys = type.KeyProperties.Select(x => x.Name);
                    defs.Add(string.Format(primaryKeyTmpl, string.Join(", ", keys)));
                }

                // foreign keys
                foreach (var assoc in model.StoreModel.AssociationTypes)
                {
                    if (assoc.Constraint.ToRole.Name == type.Name)
                    {
                        var thisKeys = assoc.Constraint.ToProperties.Select(x => x.Name);
                        var thatKeys = assoc.Constraint.FromProperties.Select(x => x.Name);
                        defs.Add(string.Format(foreignKeyTmpl,
                            string.Join(", ", thisKeys),
                            assoc.Constraint.FromRole.Name,
                            string.Join(", ", thatKeys)));
                    }
                }

                // create table
                var sql = string.Format(tableTmpl, FixTableName(GetTableName(type.Name, context)), string.Join(",\n", defs));
                db.ExecuteSqlCommand(sql);
            }

            // create index
            foreach (var index in indicies.Values)
            {
                var columns = string.Join(", ", index.Columns);
                var sql = string.Format(indexTmpl, index.Name, index.Table, columns);
                db.ExecuteSqlCommand(sql);
            }
        }
    }
}
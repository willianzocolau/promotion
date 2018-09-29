using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;
using Npgsql.NameTranslation;
using PromotionApi.Models;
using System;
using System.Text.RegularExpressions;

namespace PromotionApi.Data
{
    public class DatabaseContext : DbContext
    {
        private static readonly Regex _keysRegex = new Regex("^(PK|FK|IX)_", RegexOptions.Compiled);

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ForgotPasswordRequest> ForgotPasswordRequests { get; set; }
        public DbSet<WishItem> WishList { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            FixSnakeCaseNames(modelBuilder);

            modelBuilder.Entity<State>().HasData(
                    new State { Id = 0, Name = "Acre" },
                    new State { Id = 1, Name = "Alagoas" },
                    new State { Id = 2, Name = "Amapá" },
                    new State { Id = 3, Name = "Amazonas" },
                    new State { Id = 4, Name = "Bahia" },
                    new State { Id = 5, Name = "Ceará" },
                    new State { Id = 6, Name = "Distrito Federal" },
                    new State { Id = 7, Name = "Espírito Santo" },
                    new State { Id = 8, Name = "Goiás" },
                    new State { Id = 9, Name = "Maranhão" },
                    new State { Id = 10, Name = "Mato Grosso" },
                    new State { Id = 11, Name = "Mato Grosso do Sul" },
                    new State { Id = 12, Name = "Minas Gerais" },
                    new State { Id = 13, Name = "Paraná" },
                    new State { Id = 14, Name = "Paraíba" },
                    new State { Id = 15, Name = "Pará" },
                    new State { Id = 16, Name = "Pernambuco" },
                    new State { Id = 17, Name = "Piauí" },
                    new State { Id = 18, Name = "Rio Grande do Norte" },
                    new State { Id = 19, Name = "Rio Grande do Sul" },
                    new State { Id = 20, Name = "Rio de Janeiro" },
                    new State { Id = 21, Name = "Rondônia" },
                    new State { Id = 22, Name = "Roraima" },
                    new State { Id = 23, Name = "Santa Catarina" },
                    new State { Id = 24, Name = "Sergipe" },
                    new State { Id = 25, Name = "São Paulo" },
                    new State { Id = 26, Name = "Tocantins" }
                );
        }

        private void FixSnakeCaseNames(ModelBuilder modelBuilder)
        {
            var mapper = new NpgsqlSnakeCaseNameTranslator();
            foreach (var table in modelBuilder.Model.GetEntityTypes())
            {
                ConvertToSnake(mapper, table);
                foreach (var property in table.GetProperties())
                {
                    ConvertToSnake(mapper, property);
                }

                foreach (var primaryKey in table.GetKeys())
                {
                    ConvertToSnake(mapper, primaryKey);
                }

                foreach (var foreignKey in table.GetForeignKeys())
                {
                    ConvertToSnake(mapper, foreignKey);
                }

                foreach (var indexKey in table.GetIndexes())
                {
                    ConvertToSnake(mapper, indexKey);
                }
            }
        }

        private void ConvertToSnake(INpgsqlNameTranslator mapper, object entity)
        {
            switch (entity)
            {
                case IMutableEntityType table:
                    var relationalTable = table.Relational();
                    relationalTable.TableName = ConvertGeneralToSnake(mapper, relationalTable.TableName);
                    if (relationalTable.TableName.StartsWith("asp_net_"))
                    {
                        relationalTable.TableName = relationalTable.TableName.Replace("asp_net_", string.Empty);
                        relationalTable.Schema = "identity";
                    }

                    break;
                case IMutableProperty property:
                    property.Relational().ColumnName = ConvertGeneralToSnake(mapper, property.Relational().ColumnName);
                    break;
                case IMutableKey primaryKey:
                    primaryKey.Relational().Name = ConvertKeyToSnake(mapper, primaryKey.Relational().Name);
                    break;
                case IMutableForeignKey foreignKey:
                    foreignKey.Relational().Name = ConvertKeyToSnake(mapper, foreignKey.Relational().Name);
                    break;
                case IMutableIndex indexKey:
                    indexKey.Relational().Name = ConvertKeyToSnake(mapper, indexKey.Relational().Name);
                    break;
                default:
                    throw new NotImplementedException("Unexpected type was provided to snake case converter");
            }
        }

        private string ConvertKeyToSnake(INpgsqlNameTranslator mapper, string keyName) =>
            ConvertGeneralToSnake(mapper, _keysRegex.Replace(keyName, match => match.Value.ToLower()));

        private string ConvertGeneralToSnake(INpgsqlNameTranslator mapper, string entityName) =>
            mapper.TranslateMemberName(ModifyNameBeforeConvertion(mapper, entityName));

        protected virtual string ModifyNameBeforeConvertion(INpgsqlNameTranslator mapper, string entityName) => entityName;
    }
}

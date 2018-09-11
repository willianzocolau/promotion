using Microsoft.EntityFrameworkCore;
using PromotionApi.Models;

namespace PromotionApi.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Store> Stores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
    }
}

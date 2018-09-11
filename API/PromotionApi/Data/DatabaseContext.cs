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
        public DbSet<Promotion> Promotions { get; set; }
    }
}

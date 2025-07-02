using Microsoft.EntityFrameworkCore;

namespace AppCaching.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Models.User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.User>().HasKey(u => u.Id);
            modelBuilder.Entity<Models.User>().Property(u => u.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Models.User>().Property(u => u.Email).IsRequired().HasMaxLength(200);
        }
    }
}

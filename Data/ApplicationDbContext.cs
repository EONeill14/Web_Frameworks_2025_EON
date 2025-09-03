using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web_Frameworks_2025_EON.Models;

namespace Web_Frameworks_2025_EON.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "GAA" },
                new Category { Id = 2, Name = "Tennis" },
                new Category { Id = 3, Name = "Golf" },
                new Category { Id = 4, Name = "Cycling" },
                new Category { Id = 5, Name = "Soccer" }
            );
        }
    }
}
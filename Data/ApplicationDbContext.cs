using GundamStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GundamStore.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product>? Products { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Scale>? Scales { get; set; }
        public DbSet<ProductImage>? ProductImages { get; set; }
        public DbSet<OrderItem>? OrderItems { get; set; }
        public DbSet<Order>? Orders { get; set; }
        public DbSet<Banner>? Banners { get; set; }
        public DbSet<Status>? Statuses { get; set; }
    }
}

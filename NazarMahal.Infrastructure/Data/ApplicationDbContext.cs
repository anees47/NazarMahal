using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NazarMahal.Core.Entities;
using NazarMahal.Infrastructure.EntityConfiguration;

namespace NazarMahal.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<GlassesCategory> GlassesCategories { get; set; }
        public DbSet<GlassesSubCategory> GlassesSubCategories { get; set; }
        public DbSet<Glasses> Glasses { get; set; }
        public DbSet<GlassesAttachment> GlassesAttachments { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            _ = modelBuilder.ApplyConfiguration(new GlassesEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new GlassesAttachmentEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new GlassesCategoryEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new GlassesSubCategoryEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new OrderItemEntityConfiguration());

            _ = modelBuilder.Entity<Glasses>().HasQueryFilter(g => g.IsActive);
            _ = modelBuilder.Entity<GlassesCategory>().HasQueryFilter(gc => gc.IsActive);
            _ = modelBuilder.Entity<GlassesSubCategory>().HasQueryFilter(gsc => gsc.IsActive);

            _ = modelBuilder.Entity<Appointment>().HasIndex(a => new { a.AppointmentDate, a.AppointmentTime }).IsUnique();
        }
    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NazarMahal.Core.Entities;
using NazarMahal.Infrastructure.Data;
using NazarMahal.Infrastructure.Data.Configurations;
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

            modelBuilder.ApplyConfiguration(new GlassesEntityConfiguration());
            modelBuilder.ApplyConfiguration(new GlassesAttachmentEntityConfiguration());
            modelBuilder.ApplyConfiguration(new GlassesCategoryEntityConfiguration());
            modelBuilder.ApplyConfiguration(new GlassesSubCategoryEntityConfiguration());
            modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityConfiguration());

            modelBuilder.Entity<Glasses>().HasQueryFilter(g => g.IsActive);
            modelBuilder.Entity<GlassesCategory>().HasQueryFilter(gc => gc.IsActive);
            modelBuilder.Entity<GlassesSubCategory>().HasQueryFilter(gsc => gsc.IsActive);
            
            modelBuilder.Entity<Appointment>().HasIndex(a => new { a.AppointmentDate, a.AppointmentTime }).IsUnique();
        }
    }
}

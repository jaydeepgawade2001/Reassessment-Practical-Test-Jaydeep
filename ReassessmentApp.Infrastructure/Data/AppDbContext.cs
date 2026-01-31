using Microsoft.EntityFrameworkCore;
using ReassessmentApp.Domain.Entities;

namespace ReassessmentApp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity properties if needed
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
                entity.Property(r => r.Location).IsRequired();
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
                entity.Property(b => b.CreatedBy).IsRequired();
                
                // Relationship
                entity.HasOne(b => b.Room)
                      .WithMany(r => r.Bookings) // Explicitly map to Bookings collection
                      .HasForeignKey(b => b.RoomId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent deleting room if bookings exist (Business Rule 4 support)
            });
        }
    }
}

using Microsoft.EntityFrameworkCore;
using TourPlanner.Models;

namespace TourPlanner.Data
{
    public class TourPlannerContext : DbContext
    {
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourLog> Logs { get; set; }
        public DbSet<List> Lists { get; set; }

        public TourPlannerContext(DbContextOptions<TourPlannerContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure tables
            modelBuilder.Entity<Tour>().ToTable("Tours");
            modelBuilder.Entity<TourLog>().ToTable("TourLogs");
            modelBuilder.Entity<List>().ToTable("Lists");

            // Configure relationships
            modelBuilder.Entity<Tour>()
                .HasOne(t => t.List)
                .WithMany(l => l.Tours)
                .HasForeignKey(t => t.ListId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TourLog>()
                .HasOne(l => l.Tour)
                .WithMany()
                .HasForeignKey(l => l.TourId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure non-nullable string properties
            modelBuilder.Entity<Tour>().Property(t => t.Name).IsRequired();
            modelBuilder.Entity<Tour>().Property(t => t.FromLocation).IsRequired();
            modelBuilder.Entity<Tour>().Property(t => t.ToLocation).IsRequired();
            modelBuilder.Entity<Tour>().Property(t => t.TransportType).IsRequired();

            modelBuilder.Entity<TourLog>().Property(l => l.Comment).IsRequired();
            
            modelBuilder.Entity<List>().Property(l => l.Name).IsRequired();
        }
    }
} 
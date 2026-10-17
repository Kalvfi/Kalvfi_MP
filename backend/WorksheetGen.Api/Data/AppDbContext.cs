using Microsoft.EntityFrameworkCore;
using WorksheetGen.Api.Models;

namespace WorksheetGen.Api.Data
{
    public class AppDbContext : DbContext
    {
        internal DbSet<User> Users { get; set; }
        internal DbSet<Worksheet> Worksheets { get; set; }
        internal DbSet<SourceDocument> SourceDocuments { get; set; }

        public AppDbContext(DbContextOptions options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Worksheet>(e =>
            {
                e.Property(w => w.Title).HasMaxLength(200).IsRequired();
                e.HasOne(w => w.Owner).WithMany().HasForeignKey(w => w.OwnerId);
                e.OwnsMany(
                    w => w.Questions,
                    q =>
                    {
                        q.ToJson();
                        q.Property(x => x.Type).HasConversion<string>();
                    }
                );
                e.HasIndex(w => new { w.IsPublic, w.CreatedAt });
            });

            modelBuilder.Entity<SourceDocument>(e =>
            {
                e.Property(d => d.FileName).HasMaxLength(200).IsRequired();
                e.Property(d => d.ContentType).HasMaxLength(100).IsRequired();
                e.HasOne(d => d.Owner).WithMany().HasForeignKey(d => d.OwnerId);
            });

            modelBuilder.Entity<User>(e =>
            {
                e.Property(u => u.Email).HasMaxLength(200).IsRequired();
                e.Property(u => u.DisplayName).HasMaxLength(100);
                e.HasIndex(u => u.Email).IsUnique();

                e.HasData(
                    new User
                    {
                        Id = DevUser.Id,
                        Email = "dev@example.com",
                        DisplayName = "Dev Teacher",
                        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    }
                );
            });
        }
    }
}

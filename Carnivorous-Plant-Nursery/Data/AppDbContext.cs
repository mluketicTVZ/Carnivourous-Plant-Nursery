using Carnivorous_Plant_Nursery.Models;
using Microsoft.EntityFrameworkCore;

namespace Carnivorous_Plant_Nursery.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Plant> Plant { get; set; }
        public DbSet<SeedBatch> SeedBatch { get; set; }
        public DbSet<Taxonomy> Taxonomy { get; set; }
        public DbSet<CareProfile> CareProfile { get; set; }
        public DbSet<Lineage> Lineage { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Lineage>()
                .HasOne(l => l.Mother)
                .WithMany()
                .HasForeignKey(l => l.MotherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lineage>()
                .HasOne(l => l.Father)
                .WithMany()
                .HasForeignKey(l => l.FatherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Article>()
                .ToTable(t => t.HasCheckConstraint("CK_Article_Price_NonNegative", "[Price] >= 0"));

            modelBuilder.Entity<Plant>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Plant_PotDiameterCm_NonNegative", "[PotDiameterCm] >= 0");
                    t.HasCheckConstraint("CK_Plant_PotHeightCm_NonNegative", "[PotHeightCm] >= 0");
                    t.HasCheckConstraint("CK_Plant_EstimatedAgeAtAcquiryYears_NonNegative", "[EstimatedAgeAtAcquiryYears] >= 0");
                });

            modelBuilder.Entity<SeedBatch>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint("CK_SeedBatch_SeedCount_NonNegative", "[SeedCount] >= 0");
                    t.HasCheckConstraint("CK_SeedBatch_EstimatedGerminationRate_Range", "[EstimatedGerminationRate] >= 0 AND [EstimatedGerminationRate] <= 1");
                });
        }
    }
}

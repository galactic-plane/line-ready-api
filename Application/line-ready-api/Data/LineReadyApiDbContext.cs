using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LineReadyApi.Data
{
    public class LineReadyApiDbContext : DbContext
    {
        readonly ILogger<LineReadyApiDbContext> _logger;

        public LineReadyApiDbContext(DbContextOptions<LineReadyApiDbContext> options, ILogger<LineReadyApiDbContext> logger)
          : base(options)
        {
            _logger = logger;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<RootEntity>(eb =>
                {
                    eb.HasNoKey();
                    eb.ToView("View_RootEntity");
                });
        }

        public DbSet<RootEntity> Roots { get; set; }

        public DbSet<CityEntity> Cities { get; set; }
        public DbSet<CoordEntity> Coords { get; set; }

        public DbSet<FishEntity> Fish { get; set; }
        public DbSet<SpeciesIllustrationPhotoEntity> Species { get; set; }
    }
}
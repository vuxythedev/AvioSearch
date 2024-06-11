using Entities;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class FlightDbContext : DbContext
    {
        public DbSet<FlightSearch> FlightSearches { get; set; }
        public DbSet<FlightOffer> FlightOffers { get; set; }

        public FlightDbContext(DbContextOptions<FlightDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FlightSearch>()
                .HasKey(fs => fs.SearchKey); 

            modelBuilder.Entity<FlightOffer>()
                .HasKey(fo => fo.Id);

            modelBuilder.Entity<FlightSearch>()
                .HasMany(fs => fs.FlightOffers)
                .WithOne(fo => fo.FlightSearch)
                .HasForeignKey(fo => fo.FlightSearchKey);


            base.OnModelCreating(modelBuilder);
        }
    }
}

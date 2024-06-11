using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;


namespace Repository
{
    public class FlightSearchRepository : IFlightSearchRepository
    {
        private readonly FlightDbContext _context;

        public FlightSearchRepository(FlightDbContext context)
        {
            _context = context;
        }

        public async Task<FlightSearch?> GetAsync(string searchKey)
        {
            return await _context.FlightSearches
                .Include(fs => fs.FlightOffers)
                .FirstOrDefaultAsync(fs => fs.SearchKey == searchKey);
        }

        public async Task AddAsync(FlightSearch flightSearch)
        {
            await _context.FlightSearches.AddAsync(flightSearch);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSearchDataAsync(int period) 
        {
            var cutoffTime = DateTime.UtcNow.AddMinutes(-period);
            var oldSearches = _context.FlightSearches
                .Where(fs => fs.LastUpdate < cutoffTime); 

            _context.FlightSearches.RemoveRange(oldSearches);
            await _context.SaveChangesAsync();
        }
    }
}

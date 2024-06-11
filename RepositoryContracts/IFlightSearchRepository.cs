using Entities;

namespace RepositoryContracts
{
    public interface IFlightSearchRepository
    {
        Task<FlightSearch?> GetAsync(string searchKey);
        Task AddAsync(FlightSearch flightSearch);
        Task DeleteSearchDataAsync(int period);
    }
}

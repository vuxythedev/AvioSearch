using Entities;
using RepositoryContracts;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FlightSearchService : IFlightSearchService
    {
        private readonly IFlightSearchRepository _flightSearchRepository;

        public FlightSearchService(IFlightSearchRepository flightSearchRepository)
        {
            _flightSearchRepository = flightSearchRepository;
        }
      
        public async Task DeleteFlightSearchDataAsync(int period)
        {
           await _flightSearchRepository.DeleteSearchDataAsync(period);
        }
    }
}

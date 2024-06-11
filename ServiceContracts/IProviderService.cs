using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface IProviderService
    {
        public Task<List<FlightResponse>?> GetFlightOfferList(FlightRequest flightRequest);
    }
}

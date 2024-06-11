using AvioSearch.Models;
using Entities;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;

namespace AvioSearch.Controllers.Components
{
    public class RepresentFlighDataControler : Controller
    {
        private readonly IProviderService _poviderService;
        

        public RepresentFlighDataControler(IProviderService providerService)
        {
            _poviderService = providerService;          
        }

        [Route("representFlightData")]
        public async Task<IActionResult> LoadFlightData(FlightRequest flightRequest)
        {
            var flightDataList = await _poviderService.GetFlightOfferList(flightRequest);

            FlightDataModel flightDataModel = new FlightDataModel();
            
            if(flightDataList != null) 
                flightDataModel.FlightOfferList = flightDataList;

            return ViewComponent("FlightResults", flightDataModel);
        }
    }
}

using AvioSearch.Models;
using Microsoft.AspNetCore.Mvc;

namespace AvioSearch.ViewComponents
{
    public class FlightResultsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(FlightDataModel flightDataModel)
        {
            return View("TableComponent", flightDataModel);
        }
    }
}

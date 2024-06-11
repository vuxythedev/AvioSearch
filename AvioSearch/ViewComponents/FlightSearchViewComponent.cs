using Microsoft.AspNetCore.Mvc;

namespace AvioSearch.ViewComponents
{
    public class FlightSearchViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("SearchComponent");
        }
    }
}

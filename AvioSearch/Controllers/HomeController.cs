using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace AvioSearch.Controllers
{
    public class HomeController : Controller
    {
        //private readonly IProvider _provider;

        //public HomeController(IProvider provider)
        //{
        //    _provider = provider;
        //}

        [Route("/")]
        public IActionResult GetFlightView()
        {
            //var testList = await _provider.GetFlightOfferList();
            // return Content("");
            return View("FlightView");
        }
    }
}

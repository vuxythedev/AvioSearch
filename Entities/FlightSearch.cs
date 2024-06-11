using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class FlightSearch
    {
        public string SearchKey { get; set; } = null!;
        public ICollection<FlightOffer> FlightOffers { get; set; } = new List<FlightOffer>();
        public DateTime LastUpdate { get; set; }
    }
}

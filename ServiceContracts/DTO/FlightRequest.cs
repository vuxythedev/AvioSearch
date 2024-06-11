using Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public record FlightRequest(string OriginLocationCode, string DestinationLocationCode, string DepartureDate, string Adults, string Children, string Currency);

}

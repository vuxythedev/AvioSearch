using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class FlightResponse
    {
        public string? OriginCode { get; set; }
        public string? DestinationCode { get; set; }
        public DateTime DepartureDate { get; set; }
        public int? Itineraries { get; set; }
        public int? NumberOfBookableSeats { get; set; }
        public string? CurrencyCode { get; set; }
        public decimal Price { get; set; }
    }


    public static class FlightExtensions
    {
        /// <summary>
        /// An extension method to convert an object of FlightOffer class into FlightResponse class
        /// </summary>
        /// <param name="flightOffer">The FlightOffer object to convert</param>
        /// <returns>Returns the converted FlightResponse object</returns>
        public static FlightResponse ToFlightResponse(this FlightOffer flightOffer)
        {
            return new FlightResponse
            {
                OriginCode = flightOffer.OriginCode,
                DestinationCode = flightOffer.DestinationCode,
                DepartureDate = flightOffer.DepartureDate,
                Itineraries = flightOffer.Itineraries,
                NumberOfBookableSeats = flightOffer.NumberOfBookableSeats,
                CurrencyCode = flightOffer.CurrencyCode,
                Price = flightOffer.Price
            };
        }
     
    }
}



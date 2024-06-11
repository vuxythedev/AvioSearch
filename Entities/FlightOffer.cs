using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class FlightOffer
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [StringLength(3)]
        public string? OriginCode { get; set; }
        [Required]
        [StringLength(3)]
        public string? DestinationCode { get; set; }
        public DateTime DepartureDate { get; set; }
        public int? Itineraries { get; set; }
        public int? NumberOfBookableSeats { get; set; }
        [Required]
        [StringLength(3)]
        public string? CurrencyCode { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Required]
        public string FlightSearchKey { get; set; } = null!;
        public FlightSearch FlightSearch { get; set; } = null!;

    }
}

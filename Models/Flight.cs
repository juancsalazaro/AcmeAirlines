using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcmeAirlines.Models
{
    public class Flight
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string FlightNumber { get; set; }

        [Required]
        public int OriginCityId { get; set; }

        [Required]
        public int DestinationCityId { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        public int TotalSeats { get; set; }

        public int AvailableSeats { get; set; }

        [Column(TypeName = "numeric(18,2)")]
        public decimal BasePrice { get; set; }

        public string Status { get; set; } = "Scheduled";

        [ForeignKey("OriginCityId")]
        public City OriginCity { get; set; }

        [ForeignKey("DestinationCityId")]
        public City DestinationCity { get; set; }

        public ICollection<Fare> Fares { get; set; }
    }
}
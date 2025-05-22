using System.ComponentModel.DataAnnotations;

namespace AcmeAirlines.Models
{
    public class City
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(3)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

        public ICollection<Flight> OriginFlights { get; set; }
        public ICollection<Flight> DestinationFlights { get; set; }
    }
}
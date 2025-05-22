using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcmeAirlines.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string ReservationCode { get; set; }

        [Required]
        public int FlightId { get; set; }

        [Required]
        public int PassengerId { get; set; }

        [Required]
        public int FareId { get; set; }

        public DateTime ReservationDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; }

        public string SeatNumber { get; set; }

        [Column(TypeName = "numeric(18,2)")]
        public decimal TotalPrice { get; set; }

        [ForeignKey("FlightId")]
        public Flight Flight { get; set; }

        [ForeignKey("PassengerId")]
        public Passenger Passenger { get; set; }

        [ForeignKey("FareId")]
        public Fare Fare { get; set; }
    }
}
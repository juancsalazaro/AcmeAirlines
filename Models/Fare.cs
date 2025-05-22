using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcmeAirlines.Models
{
    public class Fare
    {
        public int Id { get; set; }

        [Required]
        public int FlightId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [Column(TypeName = "numeric(18,2)")]
        public decimal Price { get; set; }

        public int AvailableSeats { get; set; }

        public bool IsRefundable { get; set; }

        public bool IncludesCheckedBaggage { get; set; }

        [Column(TypeName = "numeric(18,2)")]
        public decimal ChangeFee { get; set; }

        [ForeignKey("FlightId")]
        public Flight Flight { get; set; }
    }
}
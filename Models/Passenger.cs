using System.ComponentModel.DataAnnotations;

namespace AcmeAirlines.Models
{
    public class Passenger
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [StringLength(20)]
        public string DocumentNumber { get; set; }

        [StringLength(50)]
        public string DocumentType { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(20)]
        [Phone]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        public string Nationality { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
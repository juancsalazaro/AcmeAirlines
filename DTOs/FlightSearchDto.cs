using System.ComponentModel.DataAnnotations;

namespace AcmeAirlines.DTOs
{
    public class FlightSearchDto
    {
        [Required(ErrorMessage = "La ciudad de origen es obligatoria")]
        public int OriginCityId { get; set; }

        [Required(ErrorMessage = "La ciudad de destino es obligatoria")]
        public int DestinationCityId { get; set; }

        [Required(ErrorMessage = "La fecha de salida es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime DepartureDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }

        public int Passengers { get; set; } = 1;
    }
}
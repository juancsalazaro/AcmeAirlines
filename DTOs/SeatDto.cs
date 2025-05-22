namespace AcmeAirlines.DTOs
{
    public class SeatMapDto
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public int TotalRows { get; set; }
        public int SeatsPerRow { get; set; }
        public List<SeatDto> Seats { get; set; } = new List<SeatDto>();
        public string AircraftType { get; set; }
    }

    public class SeatDto
    {
        public string SeatNumber { get; set; } // Ejemplo: 12A, 23F
        public bool IsAvailable { get; set; }
        public bool IsEmergencyExit { get; set; }
        public bool IsAisle { get; set; }
        public bool IsWindow { get; set; }
        public string SeatClass { get; set; } // Economy, Business, First
        public decimal? ExtraCharge { get; set; } // Costo adicional si aplica
        public bool IsSelected { get; set; } // Si el usuario lo ha seleccionado

        // Para el mapa visual
        public int Row { get; set; }
        public int Column { get; set; }
    }

    public class SeatSelectionDto
    {
        public int FlightId { get; set; }
        public List<string> SelectedSeats { get; set; } = new List<string>();
        public decimal TotalExtraCharge { get; set; }
    }
}
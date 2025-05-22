namespace AcmeAirlines.DTOs
{
    public class FlightResultDto
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public string OriginCity { get; set; }
        public string OriginCityCode { get; set; }
        public string DestinationCity { get; set; }
        public string DestinationCityCode { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public TimeSpan Duration => ArrivalTime - DepartureTime;
        public decimal MinPrice { get; set; }
        public int AvailableSeats { get; set; }
        public List<FareDto> Fares { get; set; }
        public List<SeatDto> Seats { get; set; } = new List<SeatDto>(); // Propiedad añadida para almacenar información de asientos
    }

    public class FareDto
    {
        public int FareId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsRefundable { get; set; }
        public bool IncludesCheckedBaggage { get; set; }
        public decimal ChangeFee { get; set; }
    }
}
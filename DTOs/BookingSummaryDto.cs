namespace AcmeAirlines.DTOs
{
    public class BookingSummaryDto
    {
        // Informaci�n del vuelo
        public FlightResultDto FlightInfo { get; set; }

        // Informaci�n de la tarifa seleccionada
        public FareDto SelectedFare { get; set; }

        // Informaci�n de pasajeros
        public PassengerListDto Passengers { get; set; }

        // Asientos seleccionados
        public SeatSelectionDto SeatSelection { get; set; }

        // Servicios adicionales
        public AdditionalServicesDto AdditionalServices { get; set; }

        // C�lculos de precios
        public PricingSummaryDto PricingSummary { get; set; }
    }

    public class PricingSummaryDto
    {
        public decimal BaseFarePrice { get; set; }
        public int PassengerCount { get; set; }
        public decimal TotalBaseFare => BaseFarePrice * PassengerCount;

        public decimal SeatUpgradeCharges { get; set; }
        public decimal BaggageCharges { get; set; }
        public decimal MealCharges { get; set; }
        public decimal OtherServiceCharges { get; set; }

        public decimal TotalAdditionalCharges =>
            SeatUpgradeCharges + BaggageCharges + MealCharges + OtherServiceCharges;

        public decimal TaxesAndFees { get; set; }

        public decimal TotalAmount => TotalBaseFare + TotalAdditionalCharges + TaxesAndFees;
    }
}
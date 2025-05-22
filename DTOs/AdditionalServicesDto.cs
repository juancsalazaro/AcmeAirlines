namespace AcmeAirlines.DTOs
{
    public class AdditionalServicesDto
    {
        public List<BaggageOptionDto> BaggageOptions { get; set; } = new List<BaggageOptionDto>();
        public List<MealOptionDto> MealOptions { get; set; } = new List<MealOptionDto>();
        public List<ExtraServiceDto> ExtraServices { get; set; } = new List<ExtraServiceDto>();

        public decimal TotalAdditionalCost =>
            BaggageOptions.Sum(b => b.IsSelected ? b.Price : 0) +
            MealOptions.Sum(m => m.IsSelected ? m.Price : 0) +
            ExtraServices.Sum(e => e.IsSelected ? e.Price : 0);
    }

    public class BaggageOptionDto
    {
        public int Id { get; set; }
        public string Description { get; set; } // Ej: "Maleta adicional 23kg"
        public int WeightInKg { get; set; }
        public decimal Price { get; set; }
        public bool IsSelected { get; set; }
        public int Quantity { get; set; } = 0; // Para seleccionar más de una unidad
    }

    public class MealOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DietaryType { get; set; } // Regular, Vegetariano, Vegano, Sin gluten, etc.
        public decimal Price { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ExtraServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsSelected { get; set; }
    }

    public class SelectedServicesDto
    {
        public int FlightId { get; set; }
        public List<int> SelectedBaggageIds { get; set; } = new List<int>();
        public List<int> SelectedMealIds { get; set; } = new List<int>();
        public List<int> SelectedExtraServiceIds { get; set; } = new List<int>();
        public decimal TotalAdditionalCost { get; set; }
    }
}
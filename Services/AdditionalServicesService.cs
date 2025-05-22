using Microsoft.EntityFrameworkCore;
using AcmeAirlines.Data;
using AcmeAirlines.DTOs;
using AcmeAirlines.Models;

namespace AcmeAirlines.Services
{
    public class AdditionalServicesService : IAdditionalServicesService
    {
        private readonly AcmeAirlinesContext _context;

        public AdditionalServicesService(AcmeAirlinesContext context)
        {
            _context = context;
        }

        public async Task<AdditionalServicesDto> GetAvailableServicesAsync(int flightId, int fareId)
        {
            // Obtener información del vuelo y la tarifa
            var flight = await _context.Flights
                .Include(f => f.OriginCity)
                .Include(f => f.DestinationCity)
                .FirstOrDefaultAsync(f => f.Id == flightId);

            var fare = await _context.Fares.FindAsync(fareId);

            if (flight == null || fare == null)
            {
                return new AdditionalServicesDto();
            }

            // Determinar si es un vuelo internacional
            bool isInternational = flight.OriginCity.Country != flight.DestinationCity.Country;

            // Generar opciones de servicios según el tipo de vuelo y tarifa
            var services = new AdditionalServicesDto
            {
                BaggageOptions = GetBaggageOptions(fare, isInternational),
                MealOptions = GetMealOptions(isInternational),
                ExtraServices = GetExtraServices(isInternational)
            };

            return services;
        }

        private List<BaggageOptionDto> GetBaggageOptions(Fare fare, bool isInternational)
        {
            var options = new List<BaggageOptionDto>();

            // Las opciones de equipaje dependen de si la tarifa incluye equipaje y del tipo de vuelo
            bool includesCheckedBag = fare.IncludesCheckedBaggage;

            if (!includesCheckedBag)
            {
                // Si la tarifa no incluye equipaje, ofrecer primera maleta
                options.Add(new BaggageOptionDto
                {
                    Id = 1,
                    Description = "Primera maleta facturada (hasta 23kg)",
                    WeightInKg = 23,
                    Price = isInternational ? 80000m : 50000m,
                    IsSelected = false
                });
            }

            // Opciones adicionales independientemente de la tarifa
            options.Add(new BaggageOptionDto
            {
                Id = 2,
                Description = "Maleta adicional (hasta 23kg)",
                WeightInKg = 23,
                Price = isInternational ? 120000m : 70000m,
                IsSelected = false
            });

            options.Add(new BaggageOptionDto
            {
                Id = 3,
                Description = "Equipaje deportivo/especial (hasta 32kg)",
                WeightInKg = 32,
                Price = isInternational ? 150000m : 100000m,
                IsSelected = false
            });

            options.Add(new BaggageOptionDto
            {
                Id = 4,
                Description = "Exceso de peso (por kg adicional)",
                WeightInKg = 1,
                Price = isInternational ? 15000m : 10000m,
                IsSelected = false
            });

            return options;
        }

        private List<MealOptionDto> GetMealOptions(bool isInternational)
        {
            var options = new List<MealOptionDto>();

            // En vuelos internacionales hay más opciones de comida
            options.Add(new MealOptionDto
            {
                Id = 1,
                Name = "Menú regular",
                Description = "Comida estándar incluida en el vuelo",
                DietaryType = "Regular",
                Price = 0m, // Incluido en el precio del boleto
                IsSelected = true
            });

            options.Add(new MealOptionDto
            {
                Id = 2,
                Name = "Menú vegetariano",
                Description = "Opción vegetariana (sin carne ni pescado)",
                DietaryType = "Vegetariano",
                Price = 0m, // Sin costo adicional
                IsSelected = false
            });

            options.Add(new MealOptionDto
            {
                Id = 3,
                Name = "Menú vegano",
                Description = "Sin productos de origen animal",
                DietaryType = "Vegano",
                Price = 0m, // Sin costo adicional
                IsSelected = false
            });

            options.Add(new MealOptionDto
            {
                Id = 4,
                Name = "Menú sin gluten",
                Description = "Adecuado para celíacos",
                DietaryType = "Sin gluten",
                Price = 0m, // Sin costo adicional
                IsSelected = false
            });

            if (isInternational)
            {
                options.Add(new MealOptionDto
                {
                    Id = 5,
                    Name = "Menú premium",
                    Description = "Comida gourmet con selección de vinos",
                    DietaryType = "Premium",
                    Price = 45000m, // Costo adicional
                    IsSelected = false
                });

                options.Add(new MealOptionDto
                {
                    Id = 6,
                    Name = "Menú kosher",
                    Description = "Preparado según leyes dietéticas judías",
                    DietaryType = "Kosher",
                    Price = 0m, // Sin costo adicional
                    IsSelected = false
                });

                options.Add(new MealOptionDto
                {
                    Id = 7,
                    Name = "Menú halal",
                    Description = "Preparado según preceptos islámicos",
                    DietaryType = "Halal",
                    Price = 0m, // Sin costo adicional
                    IsSelected = false
                });
            }

            return options;
        }

        private List<ExtraServiceDto> GetExtraServices(bool isInternational)
        {
            var services = new List<ExtraServiceDto>();

            // Servicios básicos disponibles en todos los vuelos
            services.Add(new ExtraServiceDto
            {
                Id = 1,
                Name = "Embarque prioritario",
                Description = "Acceso prioritario al embarque",
                Price = 25000m,
                IsSelected = false
            });

            services.Add(new ExtraServiceDto
            {
                Id = 2,
                Name = "Seguro de viaje básico",
                Description = "Cobertura médica durante el vuelo",
                Price = 35000m,
                IsSelected = false
            });

            // Servicios adicionales para vuelos internacionales
            if (isInternational)
            {
                services.Add(new ExtraServiceDto
                {
                    Id = 3,
                    Name = "Acceso a sala VIP",
                    Description = "Acceso a sala VIP en el aeropuerto de origen",
                    Price = 90000m,
                    IsSelected = false
                });

                services.Add(new ExtraServiceDto
                {
                    Id = 4,
                    Name = "Seguro de viaje premium",
                    Description = "Cobertura médica y de equipaje extendida",
                    Price = 65000m,
                    IsSelected = false
                });

                services.Add(new ExtraServiceDto
                {
                    Id = 5,
                    Name = "Fast Track",
                    Description = "Acceso rápido en controles de seguridad y migración",
                    Price = 50000m,
                    IsSelected = false
                });

                services.Add(new ExtraServiceDto
                {
                    Id = 6,
                    Name = "Transporte de mascota",
                    Description = "Transporte de mascota en bodega (requiere documentación)",
                    Price = 180000m,
                    IsSelected = false
                });
            }
            else
            {
                // Servicios específicos para vuelos nacionales
                services.Add(new ExtraServiceDto
                {
                    Id = 7,
                    Name = "Transporte de mascota",
                    Description = "Transporte de mascota en bodega (requiere documentación)",
                    Price = 120000m,
                    IsSelected = false
                });
            }

            return services;
        }

        public async Task<decimal> CalculateServicesCostAsync(SelectedServicesDto selectedServices)
        {
            // Obtener los servicios disponibles para el vuelo
            var availableServices = await GetAvailableServicesAsync(
                selectedServices.FlightId,
                // En un caso real obtendríamos el fareId de la sesión o base de datos
                1 // ID de tarifa por defecto para esta demo
            );

            decimal totalCost = 0;

            // Calcular costo de equipaje
            foreach (var baggageId in selectedServices.SelectedBaggageIds)
            {
                var baggage = availableServices.BaggageOptions.FirstOrDefault(b => b.Id == baggageId);
                if (baggage != null)
                {
                    totalCost += baggage.Price;
                }
            }

            // Calcular costo de comidas
            foreach (var mealId in selectedServices.SelectedMealIds)
            {
                var meal = availableServices.MealOptions.FirstOrDefault(m => m.Id == mealId);
                if (meal != null)
                {
                    totalCost += meal.Price;
                }
            }

            // Calcular costo de servicios extra
            foreach (var serviceId in selectedServices.SelectedExtraServiceIds)
            {
                var service = availableServices.ExtraServices.FirstOrDefault(s => s.Id == serviceId);
                if (service != null)
                {
                    totalCost += service.Price;
                }
            }

            return totalCost;
        }

        public async Task<bool> ValidateServicesAvailabilityAsync(SelectedServicesDto selectedServices)
        {
            // Verificar que todos los servicios seleccionados estén disponibles
            var availableServices = await GetAvailableServicesAsync(
                selectedServices.FlightId,
                // En un caso real obtendríamos el fareId de la sesión o base de datos
                1 // ID de tarifa por defecto para esta demo
            );

            // Validar opciones de equipaje
            foreach (var baggageId in selectedServices.SelectedBaggageIds)
            {
                if (!availableServices.BaggageOptions.Any(b => b.Id == baggageId))
                {
                    return false;
                }
            }

            // Validar opciones de comida
            foreach (var mealId in selectedServices.SelectedMealIds)
            {
                if (!availableServices.MealOptions.Any(m => m.Id == mealId))
                {
                    return false;
                }
            }

            // Validar servicios extra
            foreach (var serviceId in selectedServices.SelectedExtraServiceIds)
            {
                if (!availableServices.ExtraServices.Any(s => s.Id == serviceId))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
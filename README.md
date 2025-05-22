# Sistema de Reservas para Aerolínea Acme

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-green.svg)](https://docs.microsoft.com/en-us/aspnet/core/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Database-blue.svg)](https://www.postgresql.org/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-4.6.0-purple.svg)](https://getbootstrap.com/)

## 📋 Descripción

Sistema web completo de reservas de vuelos para la aerolínea Acme, desarrollado con ASP.NET Core MVC. Permite a los usuarios buscar vuelos, seleccionar tarifas, ingresar información de pasajeros, elegir asientos, agregar servicios adicionales y completar el proceso de reserva.

## ✨ Características Implementadas

### 🔍 Búsqueda de Vuelos
- Búsqueda por ciudad de origen, destino y fecha
- Soporte para múltiples pasajeros
- Validaciones de fechas y disponibilidad
- Filtrado inteligente de resultados

### 🎯 Selección de Vuelos y Tarifas
- Visualización detallada de horarios y duración
- Múltiples tipos de tarifas (Económica, Ejecutiva, Premium)
- Comparación de beneficios por tarifa
- Restricciones basadas en tipo de tarifa

### 👥 Gestión de Pasajeros
- Formulario completo para pasajero principal
- Soporte para pasajeros adicionales
- Validaciones específicas por edad
- Gestión de menores no acompañados
- Sistema de contactos de emergencia

### 💺 Selección de Asientos
- Mapa interactivo del avión
- Visualización por clases (Primera, Ejecutiva, Económica)
- Asientos con cargos adicionales
- Restricciones según tipo de tarifa
- Indicadores visuales de disponibilidad

### 🛄 Servicios Adicionales
- Opciones de equipaje adicional
- Menús de comida personalizados
- Servicios premium (sala VIP, embarque prioritario)
- Cálculo automático de costos
- Validación de disponibilidad

### 📊 Resumen y Confirmación
- Resumen detallado de toda la reserva
- Desglose completo de costos
- Simulación de proceso de pago
- Generación de código de reserva
- Confirmación visual del proceso

## 🛠️ Tecnologías Utilizadas

- **Backend**: ASP.NET Core 8.0 MVC
- **Base de Datos**: PostgreSQL
- **ORM**: Entity Framework Core
- **Frontend**: HTML5, CSS3, JavaScript (jQuery)
- **Framework CSS**: Bootstrap 4.6
- **Iconos**: Font Awesome
- **Arquitectura**: Patrón MVC con servicios y DTOs

## 📁 Arquitectura del Proyecto

```
AcmeAirlines/
├── Controllers/           # Controladores MVC
│   ├── FlightController.cs
│   ├── PassengerController.cs
│   ├── SeatController.cs
│   ├── AdditionalServicesController.cs
│   └── SummaryController.cs
├── Models/               # Modelos de entidades
│   ├── Flight.cs
│   ├── City.cs
│   ├── Fare.cs
│   ├── Passenger.cs
│   └── Reservation.cs
├── DTOs/                 # Objetos de transferencia de datos
│   ├── FlightSearchDto.cs
│   ├── PassengerDto.cs
│   ├── SeatDto.cs
│   └── BookingSummaryDto.cs
├── Services/             # Lógica de negocio
│   ├── FlightService.cs
│   ├── PassengerService.cs
│   ├── SeatService.cs
│   └── BookingService.cs
├── Data/                 # Contexto de base de datos
│   └── AcmeAirlinesContext.cs
├── Views/                # Vistas Razor
│   ├── Flight/
│   ├── Passenger/
│   ├── Seat/
│   ├── AdditionalServices/
│   └── Summary/
└── wwwroot/              # Archivos estáticos
    ├── css/
    └── js/
```

## 🚀 Instalación y Configuración

### Prerrequisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 12+](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/)

### Pasos de Instalación

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/juancsalazaro/acme-airlines.git
   cd acme-airlines
   ```

2. **Configurar la base de datos**
   ```bash
   # Crear base de datos en PostgreSQL
   createdb AcmeAirlines
   ```

3. **Configurar la cadena de conexión**
   
   Editar `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=AcmeAirlines;Username=tu_usuario;Password=tu_contraseña;Port=5432"
     }
   }
   ```

4. **Instalar dependencias y ejecutar migraciones**
   ```bash
   dotnet restore
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Ejecutar la aplicación**
   ```bash
   dotnet run
   ```

6. **Acceder a la aplicación**
   
   Abrir navegador en: `https://localhost:5668` o `http://localhost:5000`

## 🎮 Uso del Sistema

### Flujo de Reserva Completo

1. **Búsqueda**: Seleccionar origen, destino, fecha y número de pasajeros
2. **Selección**: Elegir vuelo y tipo de tarifa deseada
3. **Pasajeros**: Ingresar información completa de todos los pasajeros
4. **Asientos**: Seleccionar asientos según preferencias y restricciones
5. **Servicios**: Agregar equipaje, comidas especiales y servicios premium
6. **Resumen**: Revisar todos los detalles y costos
7. **Confirmación**: Completar proceso y obtener código de reserva

### Datos de Prueba

El sistema incluye datos semilla con:
- 5 ciudades (Bogotá, Medellín, Cali, Miami, Madrid)
- 3 vuelos de ejemplo con horarios y precios
- 3 tipos de tarifas por vuelo
- Configuración completa de asientos y servicios

## 🧪 Funcionalidades de Validación

- ✅ Fechas de viaje no pueden ser anteriores a hoy
- ✅ Ciudades de origen y destino deben ser diferentes  
- ✅ Validación de información de pasajeros por edad
- ✅ Restricciones de asientos según tipo de tarifa
- ✅ Disponibilidad de servicios adicionales
- ✅ Cálculo automático de impuestos y tasas
- ✅ Validación de formularios en cliente y servidor

## 📈 Estado del Proyecto

### Completado (Entregas 1 y 2) - 50%
- [x] Búsqueda y selección de vuelos
- [x] Gestión completa de pasajeros
- [x] Sistema de selección de asientos
- [x] Servicios adicionales
- [x] Resumen y proceso de reserva
- [x] Interfaz responsive y moderna
- [x] Validaciones completas

### Próximas Funcionalidades (Entrega 3) - 50%
- [ ] Persistencia real en base de datos
- [ ] Sistema de autenticación de usuarios
- [ ] Gestión de reservas existentes
- [ ] Check-in online
- [ ] Panel de administración
- [ ] Reportes y estadísticas
- [ ] API REST para integración
- [ ] Notificaciones por email

## 🤝 Contribución

Las contribuciones son bienvenidas. Para cambios importantes:

1. Fork el proyecto
2. Crear una rama para la feature (`git checkout -b feature/AmazingFeature`)
3. Commit los cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abrir un Pull Request

## 📝 Notas de Desarrollo

- **Patrón MVC**: Separación clara de responsabilidades
- **Servicios**: Lógica de negocio centralizada y reutilizable
- **DTOs**: Transferencia de datos optimizada entre capas
- **Entity Framework**: ORM para gestión de base de datos
- **Responsive Design**: Compatible con dispositivos móviles
- **Validaciones**: Doble validación (cliente/servidor)

## 🎓 Contexto Académico

Este proyecto fue desarrollado como parte del curso de **Programación Web 2**, implementando conceptos avanzados de desarrollo web con ASP.NET Core MVC, incluyendo:

- Arquitectura en capas
- Patrón Modelo-Vista-Controlador
- Entity Framework Core y migraciones
- Servicios y inyección de dependencias
- Validaciones y manejo de errores
- Diseño responsive y UX/UI

## 👨‍💻 Autor

**Camilo Salazar**
- GitHub: [@juancsalazaro](https://github.com/juancsalazaro)
- Email: camilosala55zar@gmail.com
---
⭐ Si este proyecto te fue útil, ¡dale una estrella en GitHub!

using Microsoft.EntityFrameworkCore;
using AcmeAirlines.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar la conexión a la base de datos PostgreSQL
builder.Services.AddDbContext<AcmeAirlinesContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar la autenticación JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Agregar servicios de autorización
builder.Services.AddAuthorization();

// Registrar servicios de la aplicación
builder.Services.AddScoped<AcmeAirlines.Services.IFlightService, AcmeAirlines.Services.FlightService>();
builder.Services.AddScoped<AcmeAirlines.Services.IPassengerService, AcmeAirlines.Services.PassengerService>();
builder.Services.AddScoped<AcmeAirlines.Services.ISeatService, AcmeAirlines.Services.SeatService>();
builder.Services.AddScoped<AcmeAirlines.Services.IAdditionalServicesService, AcmeAirlines.Services.AdditionalServicesService>();
builder.Services.AddScoped<AcmeAirlines.Services.IBookingService, AcmeAirlines.Services.BookingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Usa la página detallada de errores en desarrollo
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Usa la página de error genérica en producción
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Crear y aplicar las migraciones si no existen en desarrollo
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AcmeAirlinesContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}

app.Run();
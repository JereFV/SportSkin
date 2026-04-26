using Libreria.Web.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using SportSkin.Application.DTOs;
using SportSkin.Application.Profiles;
using SportSkin.Application.Services.Implementations;
using SportSkin.Application.Services.Interfaces;
using SportSkin.Infrastructure.Data;
using SportSkin.Infrastructure.FilesStorage.Implementations;
using SportSkin.Infrastructure.FilesStorage.Interfaces;
using SportSkin.Infrastructure.Repository.Implementations;
using SportSkin.Infrastructure.Repository.Interfaces;
using SportSkin.Infrastructure.Transactions.Implementations;
using SportSkin.Infrastructure.Transactions.Interfaces;
using SportSkin.Web.BackgroundServices;
using SportSkin.Web.Hubs;
using SportSkin.Web.Models;
using System.Text;

//***********
// =======================
// Configurar Serilog
// =======================
// Crear carpeta Logs automáticamente (evita errores si no existe)
Directory.CreateDirectory("Logs");

// Configuración Serilog
var logger = new LoggerConfiguration()
    // Nivel mínimo global (recomendado: Information)
    .MinimumLevel.Information()

    // Reducir ruido de logs internos de Microsoft
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    //Mostrar SQL ejecutado por EF Core
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)

    // Enriquecer logs con contexto (RequestId, etc.)
    .Enrich.FromLogContext()

    // Consola: útil para depurar en Visual Studio
    .WriteTo.Console()

    // Archivos separados por nivel (rolling diario)
    .WriteTo.Logger(l => l
        .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information)
        .WriteTo.File(@"Logs\Info-.log",
            shared: true,
            encoding: Encoding.UTF8,
            rollingInterval: RollingInterval.Day))

    .WriteTo.Logger(l => l
        .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning)
        .WriteTo.File(@"Logs\Warning-.log",
            shared: true,
            encoding: Encoding.UTF8,
            rollingInterval: RollingInterval.Day))

    .WriteTo.Logger(l => l
        .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
        .WriteTo.File(@"Logs\Error-.log",
            shared: true,
            encoding: Encoding.UTF8,
            rollingInterval: RollingInterval.Day))

    .WriteTo.Logger(l => l
        .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Fatal)
        .WriteTo.File(@"Logs\Fatal-.log",
            shared: true,
            encoding: Encoding.UTF8,
            rollingInterval: RollingInterval.Day))

    .CreateLogger();

// Paso obligatorio ANTES de crear builder
Log.Logger = logger;

var builder = WebApplication.CreateBuilder(args);

// Integrar Serilog al host
builder.Host.UseSerilog(Log.Logger);

//  Uso de MVC.
builder.Services.AddControllersWithViews();

// Uso de SignalR
builder.Services.AddSignalR();

//***********
// =======================
// Configurar Dependency Injection
// =======================
//*** Repositories
builder.Services.AddScoped<IRepositoryCamiseta, RepositoryCamiseta>();
builder.Services.AddTransient<IRepositoryUsuario, RepositoryUsuario>();
builder.Services.AddTransient<IRepositorySubasta, RepositorySubasta>();
builder.Services.AddTransient<IRepositoryCategoriaCamiseta, RepositoryCategoriaCamiseta>();
builder.Services.AddTransient<IRepositoryCondicionCamiseta, RepositoryCondicionCamiseta>();
builder.Services.AddScoped<IRepositoryEquipo, RepositoryEquipo>();
builder.Services.AddScoped<IRepositoryJugador, RepositoryJugador>();
builder.Services.AddTransient<IRepositoryPuja, RepositoryPuja>();
builder.Services.AddTransient<IRepositoryPago, RepositoryPago>();
builder.Services.AddTransient<IRepositoryMetodoPago, RepositoryMetodoPago>();
builder.Services.AddTransient<IRepositoryPreguntaRecuperacionUsuario, RepositoryPreguntaRecuperacionUsuario>();
//Controlador de transacciones en una unidad de trabajo.
builder.Services.AddScoped<IUnitOfWork, UnitofWork>();

//*** Services
builder.Services.AddScoped<IServiceCamiseta, ServiceCamiseta>();
builder.Services.AddTransient<IServiceUsuario, ServiceUsuario>();
builder.Services.AddTransient<IServiceSubasta, ServiceSubasta>();
builder.Services.AddTransient<IServiceCategoriaCamiseta, ServiceCategoriaCamiseta>();
builder.Services.AddTransient<IServiceCondicionCamiseta, ServiceCondicionCamiseta>();
builder.Services.AddTransient<IServiceEquipo, ServiceEquipo>();
builder.Services.AddTransient<IServiceJugador, ServiceJugador>();
builder.Services.AddTransient<IServiceTrayectoriaJugador, ServiceTrayectoriaJugador>();
builder.Services.AddTransient<IImageStorage, ImageStorage>();
builder.Services.AddTransient<IServicePuja, ServicePuja>();
builder.Services.AddTransient<IServicePago, ServicePago>();
builder.Services.AddTransient<IServiceMetodoPago, ServiceMetodoPago>();
builder.Services.AddTransient<IServicePreguntaRecuperacionUsuario, ServicePreguntaRecuperacionUsuario>();

//Background Service
builder.Services.AddSingleton<SubastaBackgroundService>();
builder.Services.AddHostedService<SubastaBackgroundService>(provider =>
    provider.GetRequiredService<SubastaBackgroundService>());

//Conf images route
builder.Services.Configure<ImageSettings>(
    builder.Configuration.GetSection("ImageSettings")
);

//Comision de subasta(fija)
builder.Services.Configure<SubastaSettings>(
    builder.Configuration.GetSection("SubastaSettings")
);

// =======================
// Configurar AutoMapper
// =======================
builder.Services.AddAutoMapper(config =>
{
    //*** Profiles
    config.AddProfile <CamisetaProfile>();
    config.AddProfile<CondicionCamisetaProfile>();
    config.AddProfile<CategoriaCamisetaProfile>();
    config.AddProfile<EquipoProfile>();
    config.AddProfile<JugadorProfile>();
    config.AddProfile<UsuarioProfile>();
    config.AddProfile<ImagenCamisetaProfile>();
    config.AddProfile<EstadoCamisetaProfile>();
    config.AddProfile<RolUsuarioProfile>();
    config.AddProfile<SubastaProfile>();
    config.AddProfile<EstadoSubastaProfile> ();
    config.AddProfile<PujaProfile>();
    config.AddProfile<FacturaProfile>();
    config.AddProfile<DatosEnvioProfile>();
    config.AddProfile<MetodoPagoProfile>();
    config.AddProfile<EstadoFacturaProfile>();
    config.AddProfile<MetodoPagoProfile>();
    config.AddProfile<ZonaEnvioProfile>();
    config.AddProfile<PreguntaRecuperacionUsuarioProfile>();
});

// =======================
// Configurar SQL Server DbContext
// =======================
var connectionString = builder.Configuration.GetConnectionString("AzureSqlDatabase");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "No se encontró la cadena de conexión 'SqlServerDataBase' en appsettings.json / appsettings.Development.json.");
}

builder.Services.AddDbContext<SportSkinContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        // Reintentos ante fallos transitorios (recomendado)
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount:5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd:null);
        sqlOptions.CommandTimeout(60);
    });

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

//Añadir caché. (requerido por la sesión)
builder.Services.AddDistributedMemoryCache();

//Configuración de sesión.
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "SportSkin.Session";
});

//Configuración de cookie como método de autenticación.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        //options.LoginPath = "/Login/Index";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        options.AccessDeniedPath = "/Login/Forbidden";
    });

var app = builder.Build();

// Warm-up: despierta la conexión antes del primer request
//try
//{
//    using var scope = app.Services.CreateScope();
//    var db = scope.ServiceProvider.GetRequiredService<SportSkinContext>();
//    await db.Database.ExecuteSqlRawAsync("SELECT 1");
//    Log.Information("Conexión a la base de datos establecida correctamente.");
//}
//catch (Exception ex)
//{
//    Log.Warning(ex, "Warm-up de base de datos falló, se reintentará en el primer request.");
//}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}else
{
    // Middleware personalizado
    app.UseMiddleware<ErrorHandlingMiddleware>();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseSerilogRequestLogging();
app.UseAuthorization();

app.MapStaticAssets();

// Mapeo de Hubs de SignalR.
app.MapHub<SubastaHub>("hubs/subasta");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=HomeIndex}/{id?}")
    .WithStaticAssets();

app.Run();

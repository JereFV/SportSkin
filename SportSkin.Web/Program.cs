using Libreria.Web.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using SportSkin.Application.Services.Interfaces;
using SportSkin.Application.Services.Implementations;
using SportSkin.Infrastructure.Data;
using SportSkin.Infrastructure.Repository.Implementations;
using SportSkin.Infrastructure.Repository.Interfaces;
using System.Text;
using SportSkin.Application.Profiles;
using SportSkin.Web.Models;
using SportSkin.Application.DTOs;

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

// Add services to the container.
builder.Services.AddControllersWithViews();
//***********
// =======================
// Configurar Dependency Injection
// =======================
//*** Repositories
builder.Services.AddTransient<IRepositoryCamiseta, RepositoryCamiseta>();
builder.Services.AddTransient<IRepositoryUsuario, RepositoryUsuario>();
builder.Services.AddTransient<IRepositorySubasta, RepositorySubasta>();
builder.Services.AddTransient<IRepositoryCategoriaCamiseta, RepositoryCategoriaCamiseta>();
builder.Services.AddTransient<IRepositoryCondicionCamiseta, RepositoryCondicionCamiseta>();

//*** Services
builder.Services.AddTransient<IServiceCamiseta, ServiceCamiseta>();
builder.Services.AddTransient<IServiceUsuario, ServiceUsuario>();
builder.Services.AddTransient<IServiceSubasta, ServiceSubasta>();
builder.Services.AddTransient<IServiceCategoriaCamiseta, ServiceCategoriaCamiseta>();
builder.Services.AddTransient<IServiceCondicionCamiseta, ServiceCondicionCamiseta>();
builder.Services.AddTransient<IServiceEquipo, ServiceEquipo>();
builder.Services.AddTransient<IServiceJugador, ServiceJugador>();
builder.Services.AddTransient<IServiceTrayectoriaJugador, ServiceTrayectoriaJugador>();

//Conf images route
builder.Services.Configure<ImageSettings>(
    builder.Configuration.GetSection("ImageSettings")
);

// =======================
// Configurar AutoMapper
// =======================
builder.Services.AddAutoMapper(config =>
{
    //*** Profiles
    //config.AddProfile<AutorProfile>();   
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

//Configuración de sesión.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    //options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Warm-up: despierta la conexión antes del primer request
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<SportSkinContext>();
    await db.Database.ExecuteSqlRawAsync("SELECT 1");
    Log.Information("Conexión a la base de datos establecida correctamente.");
}
catch (Exception ex)
{
    Log.Warning(ex, "Warm-up de base de datos falló, se reintentará en el primer request.");
}


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


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=HomeIndex}/{id?}")
    .WithStaticAssets();

app.Run();

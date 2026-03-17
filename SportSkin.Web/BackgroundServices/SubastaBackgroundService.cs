using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SportSkin.Infrastructure.Repository.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SportSkin.Web.BackgroundServices
{


    /*
         Servicio en segundo plano con Smart Scheduling.
    
         Pregunta cuándo es el próximo evento (FechaInicio o FechaCierre) y
         duerme exactamente hasta ese momento.
    
         Flujo de estados que gestiona:
           Borrador(5)   → En proceso(1)  cuando FechaInicio llega
           En proceso(1) → Finalizada(4)  cuando FechaCierre llega y hay pujas
           En proceso(1) → Cerrada(2)     cuando FechaCierre llega y no hay pujas
    
         IMPORTANTE — lifetimes de DI:
           BackgroundService = Singleton. DbContext = Scoped.
           Usamos IServiceScopeFactory para crear un scope nuevo por ciclo,
           evitando el error "Cannot consume scoped service from singleton".
    */
    /*
    public class SubastaBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SubastaBackgroundService> _logger;

        // Espera máxima cuando no hay eventos futuros registrados.
        // Revisa cada hora por si se crearon nuevas subastas.
        private static readonly TimeSpan EsperaMaxima = TimeSpan.FromHours(1);

        // Margen de seguridad para compensar imprecisiones del timer de .NET.
        private static readonly TimeSpan MargenAnticipacion = TimeSpan.FromSeconds(5);

        public SubastaBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<SubastaBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[SubastaService] Iniciado con Smart Scheduling.");

            // Espera inicial para que la app termine de arrancar completamente
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                // 1. Procesar transiciones vencidas en este momento
                await ProcesarTransicionesAsync(stoppingToken);

                if (stoppingToken.IsCancellationRequested) break;

                // 2. Preguntar cuándo es el próximo evento futuro
                var proximoEvento = await ObtenerProximoEventoAsync();

                TimeSpan espera;

                if (proximoEvento == null)
                {
                    // Sin eventos futuros: dormir la espera máxima
                    espera = EsperaMaxima;
                    _logger.LogInformation(
                        "[SubastaService] Sin eventos futuros. " +
                        "Reintentando en {Horas}h.", EsperaMaxima.TotalHours);
                }
                else
                {
                    // Dormir exactamente hasta el próximo evento
                    espera = proximoEvento.Value - DateTime.Now - MargenAnticipacion;

                    if (espera < TimeSpan.Zero)
                        espera = TimeSpan.Zero; // Ya pasó, procesar de inmediato

                    _logger.LogInformation(
                        "[SubastaService] Próximo evento: {Evento:dd/MM/yyyy HH:mm:ss}. " +
                        "Durmiendo {Minutos:F1} min.",
                        proximoEvento.Value,
                        espera.TotalMinutes);
                }

                // Dormimos. El CancellationToken permite detener limpiamente si la app cierra.
                try
                {
                    await Task.Delay(espera, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // La app está cerrando — salir del loop limpiamente
                    break;
                }
            }

            _logger.LogInformation("[SubastaService] Detenido correctamente.");
        }

        private async Task ProcesarTransicionesAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested) return;

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRepositorySubasta>();

                // Borrador(5) → En proceso(1)
                int activadas = await repo.ActivarSubastasPendientesAsync();
                if (activadas > 0)
                    _logger.LogInformation(
                        "[SubastaService] {Count} subasta(s) activada(s) → En proceso.",
                        activadas);

                // En proceso(1) → Finalizada(4) o Cerrada(2)
                int cerradas = await repo.CerrarSubastasVencidasAsync();
                if (cerradas > 0)
                    _logger.LogInformation(
                        "[SubastaService] {Count} subasta(s) cerrada(s) / finalizadas.",
                        cerradas);
            }
            catch (Exception ex)
            {
                // Logueamos pero NO relanzamos — el servicio debe seguir corriendo
                _logger.LogError(ex,
                    "[SubastaService] Error al procesar transiciones automáticas.");
            }
        }

        private async Task<DateTime?> ObtenerProximoEventoAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRepositorySubasta>();
                return await repo.GetProximoEventoAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[SubastaService] Error al obtener próximo evento. " +
                    "Usando espera máxima como fallback.");
                return null;
            }
        }
    }*/
}

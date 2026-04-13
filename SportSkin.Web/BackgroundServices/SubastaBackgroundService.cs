using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SportSkin.Infrastructure.Repository.Interfaces;
using SportSkin.Web.Hubs;
//using SportSkin.Web.Hubs;

namespace SportSkin.Web.BackgroundServices
{
    /*
        SubastaBackgroundService — Smart Scheduling + Despertar Anticipado.

        Dos mecanismos de cancelación:
          1. stoppingToken  → lo controla el host. Si la app cierra, este token
                              se cancela y el servicio termina limpiamente.
          2. _wakeUpCts     → token INTERNO. Cancelarlo interrumpe solo el
                              Task.Delay del sueño actual sin matar el servicio.
                              Cualquier clase puede llamar NotificarCambio()
                              para despertar el ciclo anticipadamente.
        
         SemaphoreSlim para el despertar.SemaphoreSlim.Release() es  thread-safe para este patrón.

        Flujo de estados que gestiona:
          Publicada(6) → En proceso(1)  cuando FechaInicio llega
          En proceso(1)→ Vendida(2)     cuando FechaCierre llega y hay pujas
          En proceso(1)→ Finalizada(3)  cuando FechaCierre llega y no hay pujas
    */
    public class SubastaBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SubastaBackgroundService> _logger;

        // SemaphoreSlim(0,1): empieza sin señal.
        // Release() = señal de despertar (Para Jeremy =>equivale al Cancel() anterior).
        // WaitAsync(timeout) = esperar con timeout (equivale al Task.Delay anterior).
        // Si Release() llega antes de WaitAsync, la espera retorna inmediatamente.
        // Si llega durante WaitAsync, la interrumpe. Sin condición de carrera.
        private readonly SemaphoreSlim _wakeUpSignal = new SemaphoreSlim(0, 1);

        private static readonly TimeSpan EsperaMaxima = TimeSpan.FromHours(1);
        private static readonly TimeSpan MargenPostCierre = TimeSpan.FromSeconds(2);

        public SubastaBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<SubastaBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        // ── API pública ──────────────────────────────────────────────
        // Thread-safe. Puede llamarse desde cualquier hilo.
        // Si ya hay una señal pendiente (Release previo no consumido),
        // este Release no hace nada — el semáforo tiene max=1.
        public void NotificarCambio()
        {
            // TryRelease: si currentCount ya es 1 (señal pendiente), no hace nada.
            // Evita la excepción SemaphoreFullException.
            if (_wakeUpSignal.CurrentCount == 0)
            {
                _wakeUpSignal.Release();
                _logger.LogInformation("[SubastaService] Despertar anticipado solicitado.");
            }
        }

        // ── Loop principal ───────────────────────────────────────────
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[SubastaService] Iniciado con Smart Scheduling.");

            // Espera inicial — da tiempo a que la app arranque completamente
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                // 1. Procesar todo lo que ya venció en este momento
                await ProcesarTransicionesAsync(stoppingToken);

                if (stoppingToken.IsCancellationRequested) break;

                // 2. Calcular cuánto tiempo dormir hasta el próximo evento
                var proximoEvento = await ObtenerProximoEventoAsync();
                TimeSpan espera = CalcularEspera(proximoEvento);

                _logger.LogInformation(
                   "[SubastaService] Próximo evento: {Evento}. Durmiendo {Min:F1} min.",
                   proximoEvento?.ToString("dd/MM/yyyy HH:mm:ss") ?? "ninguno",
                   espera.TotalMinutes);

                // 3. Dormir usando el semáforo.
                //    WaitAsync retorna true si fue despertado por Release() (NotificarCambio)
                //    WaitAsync retorna false si venció el timeout (espera normal)
                //    Si stoppingToken se cancela, lanza OperationCanceledException → salimos
                try
                {
                    await _wakeUpSignal.WaitAsync((int)espera.TotalMilliseconds, stoppingToken);
                    _logger.LogInformation("[SubastaService] Ciclo completado, reprocesando.");
                }
                catch (OperationCanceledException)
                {
                    // Puede ser la app cerrando o un despertar anticipado.
                    // El while lo distingue: si stoppingToken está cancelado, sale.
                    _logger.LogInformation("[SubastaService] Iniciando nuevo ciclo a las {Hora:HH:mm:ss}", DateTime.Now);
                }
            }

            _logger.LogInformation("[SubastaService] Detenido correctamente.");
        }

        // ── Helpers privados ─────────────────────────────────────────

        private TimeSpan CalcularEspera(DateTime? proximoEvento)
        {
            if (proximoEvento == null)
                return EsperaMaxima;


            var espera = proximoEvento.Value - DateTime.Now + MargenPostCierre;

            // Si ya pasó el evento (espera negativa), procesar inmediatamente
            return espera < TimeSpan.Zero ? TimeSpan.Zero : espera;
        }
       /*
          Viejo
        private async Task ProcesarTransicionesAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested) return;

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider
                                .GetRequiredService<IRepositorySubasta>();
                /*var hub = scope.ServiceProvider
                                .GetRequiredService<IHubContext<SubastaHub>>();
                /*
                // Publicada → En proceso
                int activadas = await repo.ActivarSubastasPendientesAsync();
                if (activadas > 0)
                    _logger.LogInformation(
                        "[SubastaService] {N} subasta(s) activada(s) → En proceso.", activadas);

                // En proceso → Vendida / Finalizada
                // La lista nos permite notificar a cada grupo en SignalR
                var cerradas = await repo.CerrarSubastasVencidasAsync();

                foreach (var subasta in cerradas)
                {
                    bool tieneGanador = subasta.IdUsuarioComprador.HasValue;

                    string nombreGanador = tieneGanador
                        ? $"{subasta.IdUsuarioCompradorNavigation?.Nombre} " +
                          $"{subasta.IdUsuarioCompradorNavigation?.Apellido1}".Trim()
                        : string.Empty;

                    // Notificar a todos los navegadores viendo esta subasta
                   /* await hub.Clients
                        .Group($"subasta-{subasta.IdSubasta}")
                        .SendAsync("SubastaCerrada", new
                        {
                            tieneGanador,
                            idGanador = subasta.IdUsuarioComprador,
                            nombreGanador,
                            montoFinal = subasta.MontoCompra,
                            fechaCierre = subasta.FechaCompra?.ToString("dd/MM/yyyy HH:mm")
                                           ?? DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                        });
                   /*
                    _logger.LogInformation(
                        "[SubastaService] Subasta {Id} cerrada. Ganador: {G}",
                        subasta.IdSubasta,
                        tieneGanador ? nombreGanador : "ninguno (sin pujas)");
                }
            }
            catch (Exception ex)
            {
                // No relanzamos — el servicio debe seguir corriendo aunque falle un ciclo
                _logger.LogError(ex, "[SubastaService] Error procesando transiciones.");
            }
        }*/

        private async Task ProcesarTransicionesAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested) return;

            try
            {
             using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider
                                .GetRequiredService<IRepositorySubasta>();
                var hub = scope.ServiceProvider
                                .GetRequiredService<IHubContext<SubastaHub>>();

                // Publicada(6) → En proceso(1)
                int activadas = await repo.ActivarSubastasPendientesAsync();
                _logger.LogInformation(
                    "[SubastaService] ActivarPendientes ejecutado: {N} activada(s). Hora: {H:HH:mm:ss}",
                    activadas, DateTime.Now);
                if (activadas > 0)
                    _logger.LogInformation(
                        "[SubastaService] {N} subasta(s) activada(s) → En proceso.", activadas);

                // En proceso(1) → Vendida(2) / Finalizada(3)
                // Devuelve la lista para poder notificar por SignalR a cada grupo
                var cerradas = await repo.CerrarSubastasVencidasAsync();

                foreach (var subasta in cerradas)
                {
                    bool tieneGanador = subasta.IdUsuarioComprador.HasValue;

                    // Necesitamos el navigation del comprador para el nombre.
                    // CerrarSubastasVencidasAsync no lo incluye por defecto,
                    // así que lo cargamos solo si hay ganador.
                    string nombreGanador = string.Empty;
                    if (tieneGanador)
                    {
                        // Cargar el usuario ganador para mostrar su nombre
                        var ganadorNav = subasta.IdUsuarioCompradorNavigation;
                        nombreGanador = ganadorNav != null
                            ? $"{ganadorNav.Nombre} {ganadorNav.Apellido1}".Trim()
                            : $"Usuario #{subasta.IdUsuarioComprador}";
                    }

                    // Notificar a TODOS los navegadores viendo esta subasta
                    await hub.Clients
                        .Group($"subasta-{subasta.IdSubasta}")
                        .SendAsync("SubastaCerrada", new
                        {
                            tieneGanador,
                            idGanador = subasta.IdUsuarioComprador,
                            nombreGanador,
                            montoFinal = subasta.MontoCompra,
                            fechaCierre = subasta.FechaCompra?.ToString("dd/MM/yyyy HH:mm")
                                           ?? DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                        });

                    _logger.LogInformation(
                        "[SubastaService] Subasta {Id} cerrada. Ganador: {G}",
                        subasta.IdSubasta,
                        tieneGanador ? nombreGanador : "ninguno (sin pujas)");
                }

                if (cerradas.Any())
                    _logger.LogInformation(
                        "[SubastaService] {N} subasta(s) cerrada(s).", cerradas.Count);
            }
            catch (Exception ex)
            {
                // No relanzamos — el servicio debe seguir corriendo aunque falle un ciclo
                _logger.LogError(ex, "[SubastaService] Error procesando transiciones.");
            }
        }

        private async Task<DateTime?> ObtenerProximoEventoAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRepositorySubasta>();
                var resultado = await repo.GetProximoEventoAsync();

                // Log temporal para debug
                _logger.LogInformation(
                    "[SubastaService] GetProximoEvento retornó: {R}",
                    resultado?.ToString("dd/MM HH:mm:ss") ?? "NULL");

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SubastaService] Error obteniendo próximo evento.");
                return null;
            }
        }
    }
}
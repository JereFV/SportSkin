using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SportSkin.Infrastructure.Repository.Interfaces;
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

        Flujo de estados que gestiona:
          Publicada(6) → En proceso(1)  cuando FechaInicio llega
          En proceso(1)→ Vendida(2)     cuando FechaCierre llega y hay pujas
          En proceso(1)→ Finalizada(3)  cuando FechaCierre llega y no hay pujas
    */
    public class SubastaBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SubastaBackgroundService> _logger;

        // Token interno del sueño — se renueva en cada ciclo
        private CancellationTokenSource _wakeUpCts = new();

        private static readonly TimeSpan EsperaMaxima = TimeSpan.FromHours(1);
        private static readonly TimeSpan MargenAnticipacion = TimeSpan.FromSeconds(5);

        public SubastaBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<SubastaBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        // ── API pública ──────────────────────────────────────────────
        // El SubastaController llama esto cada vez que se publica
        // una subasta nueva, para que el servicio recalcule su sueño.
        // Thread-safe: CancellationTokenSource.Cancel() lo es.
        public void NotificarCambio()
        {
            _logger.LogInformation(
                "[SubastaService] Despertar anticipado solicitado por cambio externo.");
            _wakeUpCts.Cancel();
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

                // 3. Renovar el token interno
                //    El anterior ya fue cancelado (por NotificarCambio o por vencimiento).
                //    Creamos uno nuevo para el próximo sueño.
                var viejoCts = _wakeUpCts;
                _wakeUpCts = new CancellationTokenSource();
                viejoCts.Dispose();

                // 4. Dormir enlazando AMBOS tokens:
                //    stoppingToken = la app cierra → salir del loop
                //    _wakeUpCts    = alguien llamó NotificarCambio() → despertar
                using var linked = CancellationTokenSource
                    .CreateLinkedTokenSource(stoppingToken, _wakeUpCts.Token);

                _logger.LogInformation(
                    "[SubastaService] Durmiendo {Min:F1} min. Próximo evento: {Evento}",
                    espera.TotalMinutes,
                    proximoEvento?.ToString("dd/MM/yyyy HH:mm:ss") ?? "ninguno");

                try
                {
                    await Task.Delay(espera, linked.Token);
                }
                catch (OperationCanceledException)
                {
                    // Puede ser la app cerrando o un despertar anticipado.
                    // El while lo distingue: si stoppingToken está cancelado, sale.
                    _logger.LogInformation("[SubastaService] Sueño interrumpido, reprocessando.");
                }
            }

            _logger.LogInformation("[SubastaService] Detenido correctamente.");
        }

        // ── Helpers privados ─────────────────────────────────────────

        private TimeSpan CalcularEspera(DateTime? proximoEvento)
        {
            if (proximoEvento == null)
                return EsperaMaxima;

            var espera = proximoEvento.Value - DateTime.Now - MargenAnticipacion;
            return espera < TimeSpan.Zero ? TimeSpan.Zero : espera;
        }

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
                */
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
                   */
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
        }

        private async Task<DateTime?> ObtenerProximoEventoAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider
                                .GetRequiredService<IRepositorySubasta>();
                return await repo.GetProximoEventoAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[SubastaService] Error obteniendo próximo evento. Usando espera máxima.");
                return null;
            }
        }
    }
}
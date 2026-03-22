using Microsoft.EntityFrameworkCore;
using SportSkin.Infrastructure.Data;
using SportSkin.Infrastructure.Models;
using SportSkin.Infrastructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Infrastructure.Repository.Implementations
{
    public class RepositorySubasta : IRepositorySubasta
    {
        private readonly SportSkinContext _context;
        // IDs de EstadoSubasta según BD 
        // 1=En proceso | 2=Cerrada | 3=Vendida | 4=Finalizada | 5=Borrador
        private const byte ESTADO_EN_PROCESO = 1;
        private const byte ESTADO_VENDIDA = 2;
        private const byte ESTADO_FINALIZADA = 3;        
        private const byte ESTADO_BORRADOR = 4;
        private const byte ESTADO_CANCELADA = 5;
        private const byte ESTADO_PUBLICADA = 6;
        private const byte ESTADO_CAMISETA_DISPONIBLE = 1;
        private const byte ESTADO_CAMISETA_EN_SUBASTA = 2;
        private const byte ESTADO_CAMISETA_VENDIDA = 3;
        public RepositorySubasta(SportSkinContext context)
        {
            _context = context;
        }

        // Includes base 
        private IQueryable<Subasta> QueryBase()
        {
            return _context.Subasta
                .Include(s => s.IdCamisetaNavigation)
                    .ThenInclude(c => c.ImagenCamiseta)
                .Include(s => s.IdCamisetaNavigation)
                    .ThenInclude(c => c.IdCondicionCamisetaNavigation)
                .Include(s => s.IdCamisetaNavigation)
                    .ThenInclude(c => c.IdEquipoNavigation)
                .Include(s => s.IdCamisetaNavigation)
                    .ThenInclude(c => c.IdEstadoCamisetaNavigation)
                .Include(s => s.IdCamisetaNavigation)
                    .ThenInclude(c => c.IdJugadorNavigation)
                .Include(s => s.IdCamisetaNavigation)
                    .ThenInclude(c => c.IdCategoriaCamiseta)
                .Include(s => s.IdCamisetaNavigation)
                    .ThenInclude(c => c.IdUsuarioVendedorNavigation) // agregar esto
                .Include(s => s.IdEstadoSubastaNavigation)
                .Include(s => s.Puja)
                    .ThenInclude(p => p.IdUsuarioPujaNavigation);
        }

        public async Task<ICollection<Subasta>> ListAsync()
        {
            return await QueryBase()
                .OrderByDescending(s => s.FechaInicio)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Subasta?> FindByIdAsync(int id)
        {
            return await QueryBase()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.IdSubasta == id);
        }

        /* 
            Crea la subasta con estado de borrador (ID 5)
            La fecha de inicio viene del formulario (puede ser a futuro días, semanas etc)
         */
        public async Task<int> AddAsync(Subasta entity)
        {
            _context.Subasta.Add(entity);
            await _context.Camiseta
                .Where(c => c.IdCamiseta == entity.IdCamiseta)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.IdEstadoCamiseta, ESTADO_CAMISETA_EN_SUBASTA));

            await _context.SaveChangesAsync();
            return entity.IdSubasta;
        }

        //Actualiza solo campos editables (fechas y precios)
        public async Task UpdateAsync(Subasta entity)
        {
            var existing = await _context.Subasta.FindAsync(entity.IdSubasta)
                ?? throw new KeyNotFoundException($"No existe la subasta con id={entity.IdSubasta}");

            existing.FechaInicio = entity.FechaInicio;
            existing.FechaCierre = entity.FechaCierre;
            existing.PrecioBase = entity.PrecioBase;
            existing.IncrementoMinimo = entity.IncrementoMinimo;
            existing.PrecioCompraInmediata = entity.PrecioCompraInmediata;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Subasta.FindAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"No existe la subasta con id={id}");

            _context.Subasta.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task CambiarEstadoAsync(int id, byte nuevoEstado)
        {
            var entity = await _context.Subasta.FindAsync(id)
                         ?? throw new KeyNotFoundException($"Subasta con id={id} no encontrada.");

            entity.IdEstadoSubasta = nuevoEstado;
            await _context.SaveChangesAsync();
        }

        // --- Transiciones manuales ---

        /* 
            Publica manualmente: Borrador(4) → En proceso(1).
            Solo si la fecha de inicio aún no ha pasado.
        */

        
        public async Task PublicarAsync(int id)
        {
            var entity = await _context.Subasta.FindAsync(id)
                ?? throw new KeyNotFoundException($"No existe la subasta con id={id}");

            if (entity.IdEstadoSubasta != ESTADO_BORRADOR)
                throw new InvalidOperationException(
                    "Solo se pueden publicar subastas en estado Borrador.");

            if (entity.FechaInicio < DateTime.Now.AddMinutes(-5))
                throw new InvalidOperationException(
                    "La fecha de inicio ya pasó. Actualice las fechas antes de publicar.");

            entity.IdEstadoSubasta = ESTADO_EN_PROCESO;
            await _context.SaveChangesAsync();
        }
        

        /*
            Cancela manualmente: estado activo → cancelada(5).
            Permitido si no ha iniciado O si no tiene pujas.
        */
        
        public async Task CancelarAsync(int id)
        {
            var entity = await _context.Subasta
                .Include(s => s.Puja)
                .FirstOrDefaultAsync(s => s.IdSubasta == id)
                ?? throw new KeyNotFoundException($"No existe la subasta con id={id}");

            bool yaInicio = entity.FechaInicio <= DateTime.Now;
            bool tienePujas = entity.Puja.Any();

            if (yaInicio && tienePujas)
                throw new InvalidOperationException(
                    "No se puede cancelar: la subasta ya inició y tiene pujas registradas.");

            entity.IdEstadoSubasta = ESTADO_CANCELADA;
            await _context.SaveChangesAsync();
            await _context.Camiseta
                .Where(c => c.IdCamiseta == entity.IdCamiseta)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.IdEstadoCamiseta, ESTADO_CAMISETA_DISPONIBLE));
        }



        // --- Transiciones automáticas (Background Service) ---

        /* 
            Activa subastas Borrador(5) cuya FechaInicio ya llegó → En proceso(1).
            Retorna la cantidad de subastas activadas.
        */
        /*
        public async Task<int> ActivarSubastasPendientesAsync()
        {
            var pendientes = await _context.Subasta
                .Where(s => s.IdEstadoSubasta == ESTADO_BORRADOR
                         && s.FechaInicio <= DateTime.Now)
                .ToListAsync();

            if (!pendientes.Any()) return 0;

            foreach (var subasta in pendientes)
                subasta.IdEstadoSubasta = ESTADO_EN_PROCESO;

            await _context.SaveChangesAsync();
            return pendientes.Count;
        }
        */

        /* 
            Cierra subastas En proceso(1) cuya FechaCierre ya pasó.
            Con pujas → Finalizada(4). Sin pujas → Cerrada(2).
            Retorna la cantidad de subastas cerradas.
        */
        /*
        public async Task<int> CerrarSubastasVencidasAsync()
        {
            var vencidas = await _context.Subasta
                .Include(s => s.Puja)
                .Where(s => s.IdEstadoSubasta == ESTADO_EN_PROCESO
                         && s.FechaCierre <= DateTime.Now
                         && s.FechaCompra == null)
                .ToListAsync();

            if (!vencidas.Any()) return 0;

            foreach (var subasta in vencidas)
            {
                subasta.IdEstadoSubasta = subasta.Puja.Any()
                    ? ESTADO_FINALIZADA   // Tuvo pujas → espera determinación de ganador
                    : ESTADO_FINALIZADA;     // Sin pujas → cerrada sin actividad
            }

            await _context.SaveChangesAsync();
            return vencidas.Count;
        }
        */
        // --- Smart Scheduling ---

        /* 
            Retorna la fecha del próximo evento esperado:
            - FechaInicio más próxima de borradores pendientes de activar
            - FechaCierre más próxima de subastas en proceso pendientes de cerrar
            El Background Service duerme exactamente hasta esa fecha.
            Retorna null si no hay eventos futuros registrados.
        */
        /*
        public async Task<DateTime?> GetProximoEventoAsync()
        {
            var ahora = DateTime.Now;

            // Próxima activación: FechaInicio de borradores futuros
            var proximaActivacion = await _context.Subasta
                .Where(s => s.IdEstadoSubasta == ESTADO_BORRADOR
                         && s.FechaInicio > ahora)
                .Select(s => (DateTime?)s.FechaInicio)
                .MinAsync();

            // Próximo cierre: FechaCierre de subastas en proceso
            var proximoCierre = await _context.Subasta
                .Where(s => s.IdEstadoSubasta == ESTADO_EN_PROCESO
                         && s.FechaCierre > ahora
                         && s.FechaCompra == null)
                .Select(s => (DateTime?)s.FechaCierre)
                .MinAsync();

            // Devuelve la más próxima de las dos
            if (proximaActivacion == null) return proximoCierre;
            if (proximoCierre == null) return proximaActivacion;
            return proximaActivacion < proximoCierre ? proximaActivacion : proximoCierre;
        }

        */

        // ---- Validación de negocio ---
        //Se verifica si una camiseta tiene una subasta activa
        public async Task<bool> CamisetaTieneSubastaActivaAsync(int idCamiseta, int? excluirIdSubasta = null)
        {
            var ahora = DateTime.Now;
            return await _context.Subasta
                .Where(s => s.IdCamiseta == idCamiseta
                         && (!excluirIdSubasta.HasValue || s.IdSubasta != excluirIdSubasta.Value)
                         && (
                             s.IdEstadoSubasta == ESTADO_VENDIDA  // vendida = bloqueada para siempre
                             ||
                             (s.IdEstadoSubasta == ESTADO_EN_PROCESO && s.FechaCierre > ahora) // activa y no vencida
                             ||
                             s.IdEstadoSubasta == ESTADO_BORRADOR //no permite duplicar borradores
                         ))
                .AnyAsync();
        }


        // Activas — sin FechaCompra y FechaCierre futura
        public async Task<ICollection<Subasta>> GetSubastasActivasAsync(
        DateTime? desde, DateTime? hasta)
        {
            //Solución vuelta loca
            //var fechaActual = DateTime.ParseExact(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture), ("yyyy/MM/dd HH:mm:ss.fff"), CultureInfo.InvariantCulture);
            var fechaPrueba = DateTime.Now;
            var query = QueryBase()
                .Where(s => s.IdEstadoSubasta == 1 || s.IdEstadoSubasta == 6);

            // Aplica filtro de fecha solo si se proporcionó
            if (desde.HasValue)
                query = query.Where(s => s.FechaInicio >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(s => s.FechaInicio <= hasta.Value);

            return await query
                .OrderByDescending(s => s.FechaCierre)
                .AsNoTracking()
                .ToListAsync();
        }

        // Finalizadas — FechaCierre pasada pero sin compra
        public async Task<ICollection<Subasta>> GetSubastasFinalizadasAsync(
        DateTime? desde, DateTime? hasta)
        {
            var fechaActual = DateTime.Now;
            var query = QueryBase()
                .Where(s => s.IdEstadoSubasta == 2   // Vendida
                 || s.IdEstadoSubasta == 3   // Finalizada
                 || s.IdEstadoSubasta == 5); // Cancelada

            if (desde.HasValue)
                query = query.Where(s => s.FechaCierre >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(s => s.FechaCierre <= hasta.Value);

            return await query
                .OrderByDescending(s => s.FechaCierre)
                .AsNoTracking()
                .ToListAsync();
        }

        // Vendidas — tienen FechaCompra registrada
        public async Task<ICollection<Subasta>> GetSubastasVendidasAsync()
        {
            return await QueryBase()
                .Where(s => s.FechaCompra != null)
                .OrderByDescending(s => s.FechaCompra)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ICollection<Subasta>> GetSubastasByVendedorAsync(int idUsuarioVendedor)
        {
            return await QueryBase()
                .Where(s => s.IdCamisetaNavigation.IdUsuarioVendedor == idUsuarioVendedor)
                .OrderByDescending(s => s.FechaInicio)
                .AsNoTracking()
                .ToListAsync();
        }

        // Subastas activas ordenadas por cantidad de pujas
        public async Task<ICollection<Subasta>> GetSubastasMasPopularesAsync(int top)
        {
            var fechaActual = DateTime.Now;
            return await QueryBase()
                .Where(s => s.FechaCompra == null && s.FechaCierre > fechaActual)
                .OrderByDescending(s => s.Puja.Count)
                .Take(top)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}

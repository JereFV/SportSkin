//using Microsoft.EntityFrameworkCore;
//using SportSkin.Data;
//using SportSkin.Models.ViewModels;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace SportSkin.Repositories
//{
//    public interface IHistorialRepository
//    {
//        Task<List<PujaHistorialVM>>    GetPujasByUsuarioAsync(int idUsuario);
//        Task<List<SubastaHistorialVM>> GetSubastasByUsuarioAsync(int idUsuario);
//        Task<List<CompraHistorialVM>>  GetComprasByUsuarioAsync(int idUsuario);
//        Task<List<VentaHistorialVM>>   GetVentasByUsuarioAsync(int idUsuario);
//        Task<List<PagoHistorialVM>>    GetPagosByUsuarioAsync(int idUsuario);
//    }

//    public class RepositoryHistorial : IHistorialRepository
//    {
//        private readonly ApplicationDbContext _context;

//        public RepositoryHistorial(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<List<PujaHistorialVM>> GetPujasByUsuarioAsync(int idUsuario)
//        {
//            // Máxima puja por subasta finalizada (para determinar ganador)
//            var maxPujas = await _context.Pujas
//                .Where(p => p.Subasta.Estado == "Finalizada")
//                .GroupBy(p => p.IdSubasta)
//                .Select(g => new { IdSubasta = g.Key, MaxMonto = g.Max(x => x.Monto) })
//                .ToListAsync();

//            return await _context.Pujas
//                .Include(p => p.Subasta).ThenInclude(s => s.Camiseta)
//                .Where(p => p.IdUsuario == idUsuario)
//                .OrderByDescending(p => p.FechaPuja)
//                .Select(p => new PujaHistorialVM
//                {
//                    IdPuja         = p.IdPuja,
//                    NombreCamiseta = p.Subasta.Camiseta.Nombre,
//                    ImagenCamiseta = p.Subasta.Camiseta.Imagen,
//                    Monto          = p.Monto,
//                    FechaPuja      = p.FechaPuja,
//                    EstadoSubasta  = p.Subasta.Estado,
//                    EsGanador      = p.Subasta.Estado == "Finalizada" &&
//                                     maxPujas
//                                         .Where(m => m.IdSubasta == p.IdSubasta)
//                                         .Select(m => m.MaxMonto)
//                                         .FirstOrDefault() == p.Monto
//                })
//                .ToListAsync();
//        }

//        public async Task<List<SubastaHistorialVM>> GetSubastasByUsuarioAsync(int idUsuario)
//        {
//            return await _context.Subastas
//                .Include(s => s.Camiseta)
//                .Include(s => s.Pujas)
//                .Where(s => s.Camiseta.IdUsuario == idUsuario)
//                .OrderByDescending(s => s.FechaInicio)
//                .Select(s => new SubastaHistorialVM
//                {
//                    IdSubasta      = s.IdSubasta,
//                    NombreCamiseta = s.Camiseta.Nombre,
//                    ImagenCamiseta = s.Camiseta.Imagen,
//                    PrecioInicial  = s.PrecioInicial,
//                    PrecioFinal    = s.Pujas.Any() ? s.Pujas.Max(p => p.Monto) : (decimal?)null,
//                    FechaInicio    = s.FechaInicio,
//                    FechaFin       = s.FechaFin,
//                    Estado         = s.Estado,
//                    TotalPujas     = s.Pujas.Count
//                })
//                .ToListAsync();
//        }

//        public async Task<List<CompraHistorialVM>> GetComprasByUsuarioAsync(int idUsuario)
//        {
//            return await _context.Pagos
//                .Include(p => p.Subasta).ThenInclude(s => s.Camiseta).ThenInclude(c => c.Usuario)
//                .Where(p => p.IdUsuarioComprador == idUsuario)
//                .OrderByDescending(p => p.FechaPago)
//                .Select(p => new CompraHistorialVM
//                {
//                    IdPago         = p.IdPago,
//                    NombreCamiseta = p.Subasta.Camiseta.Nombre,
//                    ImagenCamiseta = p.Subasta.Camiseta.Imagen,
//                    MontoTotal     = p.Monto,
//                    FechaCompra    = p.FechaPago,
//                    EstadoPago     = p.Estado,
//                    NombreVendedor = p.Subasta.Camiseta.Usuario.Nombre + " " +
//                                     p.Subasta.Camiseta.Usuario.Apellido1
//                })
//                .ToListAsync();
//        }

//        public async Task<List<VentaHistorialVM>> GetVentasByUsuarioAsync(int idUsuario)
//        {
//            return await _context.Pagos
//                .Include(p => p.Subasta).ThenInclude(s => s.Camiseta)
//                .Include(p => p.UsuarioComprador)
//                .Where(p => p.Subasta.Camiseta.IdUsuario == idUsuario)
//                .OrderByDescending(p => p.FechaPago)
//                .Select(p => new VentaHistorialVM
//                {
//                    IdPago          = p.IdPago,
//                    NombreCamiseta  = p.Subasta.Camiseta.Nombre,
//                    ImagenCamiseta  = p.Subasta.Camiseta.Imagen,
//                    MontoTotal      = p.Monto,
//                    FechaVenta      = p.FechaPago,
//                    EstadoPago      = p.Estado,
//                    NombreComprador = p.UsuarioComprador.Nombre + " " + p.UsuarioComprador.Apellido1
//                })
//                .ToListAsync();
//        }

//        public async Task<List<PagoHistorialVM>> GetPagosByUsuarioAsync(int idUsuario)
//        {
//            return await _context.Pagos
//                .Include(p => p.Subasta).ThenInclude(s => s.Camiseta)
//                .Where(p => p.IdUsuarioComprador == idUsuario)
//                .OrderByDescending(p => p.FechaPago)
//                .Select(p => new PagoHistorialVM
//                {
//                    IdPago           = p.IdPago,
//                    NombreCamiseta   = p.Subasta.Camiseta.Nombre,
//                    Monto            = p.Monto,
//                    FechaPago        = p.FechaPago,
//                    MetodoPago       = p.MetodoPago,
//                    Estado           = p.Estado,
//                    NumeroReferencia = p.NumeroReferencia
//                })
//                .ToListAsync();
//        }
//    }
//}

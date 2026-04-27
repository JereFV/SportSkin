using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportSkin.Core.Interfaces;
using SportSkin.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SportSkin.Web.Controllers
{
    [Authorize]
    public class HistorialController : Controller
    {
        private readonly IServiceHistorial _service;

        public HistorialController(IServiceHistorial service)
        {
            _service = service;
        }

        // GET /Historial/GetHistorial
        // Llamado via fetch() desde _ModalHistorial. Devuelve el partial ya mapeado.
        [HttpGet]
        public async Task<IActionResult> GetHistorial()
        {
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

            if (rol == "Comprador")
            {
                var vm = await MapearCompradorAsync(idUsuario);
                return PartialView("_HistorialContenido", vm);
            }

            if (rol == "Vendedor")
            {
                var vm = await MapearVendedorAsync(idUsuario);
                return PartialView("_HistorialContenido", vm);
            }

            return Unauthorized();
        }

        // ── Mapeo entidades → ViewModel (responsabilidad del Controller) ──────

        private async Task<MiHistorialVM> MapearCompradorAsync(int idUsuario)
        {
            var tPujas = await _service.GetPujasCompradorAsync(idUsuario);
            var tPagos = await _service.GetFacturasCompradorAsync(idUsuario);
            var tMaxMontos = await _service.GetMaxMontosFinalizadasAsync(idUsuario);       

            var maxMontos = tMaxMontos;

            var pujas = tPujas.Select(p => new PujaHistorialVM
            {
                IdPuja = p.IdPuja,
                NombreCamiseta = p.IdSubastaNavigation.IdCamisetaNavigation.Nombre,
                ImagenCamiseta = p.IdSubastaNavigation?.IdCamisetaNavigation?.ImagenesCamiseta?.FirstOrDefault()?.RutaImagen ?? "",
                Monto = p.Monto,
                FechaPuja = p.Fecha,
                EstadoSubasta = p.IdSubastaNavigation.IdEstadoSubastaNavigation.Nombre,
                EsGanador = p.IdSubastaNavigation.IdEstadoSubastaNavigation.Nombre == "Vendida"
                                 && maxMontos.TryGetValue(p.IdSubasta, out var max)
                                 && max == p.Monto
            }).ToList();

            // Los pagos del comprador representan sus compras también
            var pagos = tPagos.Select(p => new PagoHistorialVM
            {
                IdPago = p.IdFactura,
                NombreCamiseta = p.IdSubastaNavigation.IdCamisetaNavigation.Nombre,
                Monto = (decimal)p.Total,
                FechaPago = (DateTime)p.FechaCreacion,
                MetodoPago = p.IdMetodoPagoNavigation?.Nombre,
                Estado = p.IdEstadoFacturaNavigation?.Nombre,
                NumeroReferencia = p.IdFactura
            }).ToList();

            var compras = tPagos.Select(p => new CompraHistorialVM
            {
                IdPago = p.IdFactura,
                NombreCamiseta = p.IdSubastaNavigation.IdCamisetaNavigation.Nombre,
                ImagenCamiseta = p.IdSubastaNavigation?.IdCamisetaNavigation?.ImagenesCamiseta?.FirstOrDefault()?.RutaImagen ?? "",
                MontoTotal = (decimal)p.Total,
                FechaCompra = (DateTime)p.FechaCreacion,
                EstadoPago = p.IdEstadoFacturaNavigation?.Nombre,
                NombreVendedor = p.IdSubastaNavigation.IdCamisetaNavigation.UsuarioVendedorNavigation.Nombre + " "
                                 + p.IdSubastaNavigation.IdCamisetaNavigation.UsuarioVendedorNavigation.Apellido1
            }).ToList();

            return new MiHistorialVM
            {
                Rol = "Comprador",
                Pujas = pujas,
                Pagos = pagos,
                Compras = compras
            };
        }

        private async Task<MiHistorialVM> MapearVendedorAsync(int idUsuario)
        {
            var tSubastas = await _service.GetSubastasVendedorAsync(idUsuario);
            var tVentas = await _service.GetVentasVendedorAsync(idUsuario);           

            var subastas = tSubastas.Select(s => new SubastaHistorialVM
            {
                IdSubasta = s.IdSubasta,
                NombreCamiseta = s.IdCamisetaNavigation.Nombre,
                ImagenCamiseta = s.IdCamisetaNavigation?.ImagenesCamiseta?.FirstOrDefault()?.RutaImagen ?? "",
                PrecioInicial = s.PrecioBase,
                PrecioFinal = s.Puja.Count != 0 ? s.Puja.Max(p => p.Monto) : (decimal?)null,
                FechaInicio = s.FechaInicio,
                FechaFin = s.FechaCierre,
                Estado = s.IdEstadoSubastaNavigation?.Nombre,
                TotalPujas = s.Puja.Count
            }).ToList();

            var ventas = tVentas.Select(p => new VentaHistorialVM
            {
                IdPago = p.IdFactura,
                NombreCamiseta = p.IdSubastaNavigation.IdCamisetaNavigation.Nombre,
                ImagenCamiseta = p.IdSubastaNavigation.IdCamisetaNavigation.ImagenesCamiseta?.FirstOrDefault()?.RutaImagen ?? "",
                MontoTotal = (decimal)p.Total,
                FechaVenta = p.FechaCreacion,
                EstadoPago = p.IdEstadoFacturaNavigation?.Nombre,
                NombreComprador = p.IdSubastaNavigation.IdUsuarioCompradorNavigation.Nombre + " " + p.IdSubastaNavigation.IdUsuarioCompradorNavigation.Apellido1
            }).ToList();

            return new MiHistorialVM
            {
                Rol = "Vendedor",
                Subastas = subastas,
                Ventas = ventas
            };
        }
    }
}

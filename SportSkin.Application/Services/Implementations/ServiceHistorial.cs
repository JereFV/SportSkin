using SportSkin.Application.Services.Interfaces;
using SportSkin.Infrastructure.Repository.Interfaces;
using System.Threading.Tasks;

namespace SportSkin.Application.Services.Implementations{

    public class ServiceHistorial : IServiceHistorial
    {
        private readonly IRepositoryHistorial _repo;

        public ServiceHistorial(IRepositoryHistorial repo)
        {
            _repo = repo;
        }

        public async Task<MiHistorialVM> GetHistorialCompradorAsync(int idUsuario)
        {
            var vm = new MiHistorialVM { Rol = "Comprador" };

            var tPujas = _repo.GetPujasByUsuarioAsync(idUsuario);
            var tCompras = _repo.GetComprasByUsuarioAsync(idUsuario);
            var tPagos = _repo.GetPagosByUsuarioAsync(idUsuario);

            await Task.WhenAll(tPujas, tCompras, tPagos);

            vm.Pujas = tPujas.Result;
            vm.Compras = tCompras.Result;
            vm.Pagos = tPagos.Result;

            return vm;
        }

        public async Task<MiHistorialVM> GetHistorialVendedorAsync(int idUsuario)
        {
            var vm = new MiHistorialVM { Rol = "Vendedor" };

            var tSubastas = _repo.GetSubastasByUsuarioAsync(idUsuario);
            var tVentas = _repo.GetVentasByUsuarioAsync(idUsuario);

            await Task.WhenAll(tSubastas, tVentas);

            vm.Subastas = tSubastas.Result;
            vm.Ventas = tVentas.Result;

            return vm;
        }
    }
}

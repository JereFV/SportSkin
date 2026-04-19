using AutoMapper;
using SportSkin.Application.DTOs;
using SportSkin.Application.Services.Interfaces;
using SportSkin.Infrastructure.Models;
using SportSkin.Infrastructure.Repository.Interfaces;

namespace SportSkin.Application.Services.Implementations
{
    public class ServicePago : IServicePago
    {
        private readonly IRepositoryPago _repository;
        private readonly IRepositorySubasta _repositorySubasta;
        private readonly IMapper _mapper;

        public ServicePago(IRepositoryPago repository, IRepositorySubasta repositorySubasta, IMapper mapper)
        {
            _repository = repository;
            _repositorySubasta = repositorySubasta;
            _mapper = mapper;
        }

        public async Task<ICollection<FacturaDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<FacturaDTO>>(list);
        }

        public async Task<string> RegistrarAsync(int idSubasta, byte idMetodoPago)
        {
            var existente = await _repository.FindBySubastaAsync(idSubasta);
            if (existente != null)
                throw new InvalidOperationException("Ya existe un pago registrado para esta subasta.");

            var subasta = await _repositorySubasta.FindByIdAsync(idSubasta);
            if (subasta == null)
                throw new KeyNotFoundException($"No existe la subasta con id={idSubasta}.");

            if (subasta.IdUsuarioComprador == null || subasta.MontoCompra == null)
                throw new InvalidOperationException("La subasta no tiene ganador o monto definido.");

            var factura = new Factura
            {
                IdFactura = Guid.NewGuid().ToString("N")[..12].ToUpper(),
                IdSubasta = idSubasta,
                Total = subasta.MontoCompra.Value,
                FechaCreacion = DateTime.Now,
                FechaPago = null,
                IdEstadoFactura = 1,        // 1 = Pendiente
                IdMetodoPago = idMetodoPago
            };

            return await _repository.AddAsync(factura);
        }

        public async Task ConfirmarAsync(string idFactura)
        {
            var factura = await _repository.FindByIdAsync(idFactura);
            if (factura == null)
                throw new KeyNotFoundException($"No existe la factura {idFactura}.");

            if (factura.IdEstadoFactura == 2)
                throw new InvalidOperationException("El pago ya fue confirmado anteriormente.");

            await _repository.ConfirmarAsync(idFactura);
        }

        public async Task<ICollection<SubastaDTO>> GetSubastasPendientesPagoAsync()
        {
            var list = await _repository.GetSubastasPendientesPagoAsync();
            return _mapper.Map<ICollection<SubastaDTO>>(list);
        }
    }
}
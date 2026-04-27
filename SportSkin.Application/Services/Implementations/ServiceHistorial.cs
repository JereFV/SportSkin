using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using SportSkin.Application.DTOs;
using SportSkin.Core.Interfaces;
using SportSkin.Infrastructure.Data;
using SportSkin.Infrastructure.Models;
using SportSkin.Infrastructure.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportSkin.Infrastructure.Services
{
    public class ServiceHistorial : IServiceHistorial
    {
        private readonly IRepositoryHistorial _repo;
        private readonly SportSkinContext _context;
        private readonly IMapper _mapper;

        public ServiceHistorial(IRepositoryHistorial repo, SportSkinContext context, IMapper mapper)
        {
            _repo = repo;
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PujaDTO>> GetPujasCompradorAsync(int idUsuario)
        {
            var pujas = await _repo.GetPujasByUsuarioAsync(idUsuario);

            return _mapper.Map<List<PujaDTO>>(pujas);
        }          

        public async Task<List<FacturaDTO>> GetFacturasCompradorAsync(int idUsuario)
        {
            var facturas = await _repo.GetPagosByUsuarioAsync(idUsuario);

            return _mapper.Map<List<FacturaDTO>>(facturas);
        }        

        public async Task<List<SubastaDTO>> GetSubastasVendedorAsync(int idUsuario)
        {
            var subastas = await _repo.GetSubastasByUsuarioAsync(idUsuario);

            return _mapper.Map<List<SubastaDTO>>(subastas);
        }           

        public async Task<List<FacturaDTO>> GetVentasVendedorAsync(int idUsuario)
        {
            var ventas = await _repo.GetVentasVendedorAsync(idUsuario);

            return _mapper.Map<List<FacturaDTO>>(ventas);
        }

        public async Task<Dictionary<int, int>> GetMaxMontosFinalizadasAsync(int idUsuario)
        {
           return await _repo.GetMaxMontoSubastasFinalizadasAsync(idUsuario);           
        }            
    }
}

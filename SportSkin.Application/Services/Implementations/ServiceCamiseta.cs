using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SportSkin.Application.DTOs;
using SportSkin.Application.Services.Interfaces;
using SportSkin.Infrastructure.FilesStorage.Interfaces;
using SportSkin.Infrastructure.Models;
using SportSkin.Infrastructure.Repository.Interfaces;
using SportSkin.Infrastructure.Transactions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.Services.Implementations
{
    public class ServiceCamiseta : IServiceCamiseta
    {
        private readonly IRepositoryCamiseta _repositoryCamiseta;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageStorage _imageStorage;
        private readonly IRepositoryEquipo _repositoryEquipo;
        private readonly IRepositoryJugador _repositoryJugador;

        public ServiceCamiseta(IRepositoryCamiseta repositoryCamiseta, IMapper mapper, IUnitOfWork unitOfWork, IImageStorage imageStorage, IRepositoryEquipo repositoryEquipo, IRepositoryJugador repositoryJugador)
        {
            _repositoryCamiseta = repositoryCamiseta;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _imageStorage = imageStorage;
            _repositoryEquipo = repositoryEquipo;
            _repositoryJugador = repositoryJugador;
        }

        public async Task AddAsync(CamisetaDTO dto, ICollection<IFormFile> imagenes)
        {
            Camiseta? camiseta = null;
            var strategy = _unitOfWork.CreateExecutionStrategy();

            //Envuelve la transacción completa en una estrategia de ejecucción. (necesario para ejecutar transacciones manuales con configuración actual de EF Core)
            await strategy.ExecuteAsync(async () =>
            {
                try
                {
                    //Mapeo de DTO a entidad.
                    camiseta = _mapper.Map<Camiseta>(dto);
                    camiseta.FechaRegistro = DateTime.Now;
                    camiseta.IdEstadoCamiseta = 1; //Disponible
                    camiseta.IdUsuarioVendedor = 2; //Disponible

                    //Inicia transacción.
                    await _unitOfWork.BeginTransactionAsync();

                    /*Inicialmente, añade los registros de equipo y jugador en caso de que no existan, de lo contrario obtiene los registros respectivos
                    para asociarlos a la nueva camiseta.*/
                    camiseta.IdEquipoNavigation = await _repositoryEquipo.AddAsync(camiseta.IdEquipoNavigation);
                    camiseta.IdJugadorNavigation = await _repositoryJugador.AddAsync(camiseta.IdJugadorNavigation);

                    //Añade la camiseta y ejecuta SaveChanges para obtener el id generado.
                    await _repositoryCamiseta.AddAsync(camiseta);
                    await _unitOfWork.SaveChangesAsync();

                    //Recorre cada una de las imágenes guardandolas en la ruta física y posteriormente en la entidad de base de datos.
                    foreach (IFormFile imagen in imagenes)
                    {
                        string rutaImagen = await _imageStorage.SaveImageAsync(camiseta.IdCamiseta, imagen);

                        camiseta.ImagenCamiseta.Add(new ImagenCamiseta
                        {
                            IdImagen = (byte)(camiseta.ImagenCamiseta.Count + 1),
                            IdCamiseta = camiseta.IdCamiseta,
                            RutaImagen = rutaImagen
                        });
                    }

                    //Guarda los registros de imagenes y persiste lo realizado durante la transacción.
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch (Exception)
                {
                    //Al produciarse una excepción, ejecuta Rollback sobre las entidades y borra las imágenes que hayan sido guardadas.
                    await _unitOfWork.RollBackTransactionAsync();

                    if (camiseta != null && camiseta.IdCamiseta != 0)
                        _imageStorage.DeleteImages(camiseta.IdCamiseta);

                    throw;
                }
            });      
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<CamisetaDTO> FindByIdAsync(int id)
        {
            var camiseta = await _repositoryCamiseta.FindByIdAsync(id);
            var camisetaDTO = _mapper.Map<CamisetaDTO>(camiseta);

            //foreach (subas)

            return camisetaDTO;
        }

        public Task<ICollection<CamisetaDTO>> GetCamisetaByCategoria(int idCategoria)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CamisetaDTO>> GetCamisetaByEquipo(short idEquipo)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CamisetaDTO>> GetCamisetaByJugador(int idJugador)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CamisetaDTO>> GetCamisetaByNombre(string nombre)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CamisetaDTO>> GetCamisetasAutografiadas()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CamisetaDTO>> GetCamisetasByTemporada(short temporada)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<CamisetaDTO>> GetCamisetasByVendedor(int idUsuarioVendedor)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<CamisetaDTO>> ListAsync()
        {
            var list = await _repositoryCamiseta.ListAsync();
            var collection = _mapper.Map<ICollection<CamisetaDTO>>(list);
            return collection;
        }
        public async Task<ICollection<CamisetaDTO>> GetCamisetasVendidas()
        {
            var list = await _repositoryCamiseta.GetCamisetasVendidas();
            return _mapper.Map<ICollection<CamisetaDTO>>(list);
        }

        public async Task<ICollection<CamisetaDTO>> GetCamisetasEnSubasta()
        {
            var list = await _repositoryCamiseta.GetCamisetasEnSubasta();
            return _mapper.Map<ICollection<CamisetaDTO>>(list);
        }

        public async Task<ICollection<CamisetaDTO>> GetCamisetasSinSubasta()
        {
            var list = await _repositoryCamiseta.GetCamisetasSinSubasta();
            return _mapper.Map<ICollection<CamisetaDTO>>(list);
        }

        public async Task UpdateAsync(CamisetaDTO dto, ICollection<IFormFile> imagenes, List<int> idsImagenesEliminadas)
        {
            Camiseta? camiseta = null;
            var strategy = _unitOfWork.CreateExecutionStrategy();

            //Envuelve la transacción completa en una estrategia de ejecucción. (necesario para ejecutar transacciones manuales con configuración actual de EF Core)
            await strategy.ExecuteAsync(async () =>
            {
                try
                {
                    camiseta = await _repositoryCamiseta.FindByIdAsync(dto.IdCamiseta);

                    //Mapeo de DTO a la entidad obtenida para mantener el tracking.
                    _mapper.Map(dto, camiseta);

                    //Inicia transacción.
                    await _unitOfWork.BeginTransactionAsync();

                    /*Agrega los registros de equipo y jugador, en caso de que haya sido seleccionado un valor diferente.*/
                    camiseta.IdEquipoNavigation = await _repositoryEquipo.AddAsync(camiseta.IdEquipoNavigation);
                    camiseta.IdJugadorNavigation = await _repositoryJugador.AddAsync(camiseta.IdJugadorNavigation);

                    //Actualización de camiseta.
                    _repositoryCamiseta.Update(camiseta);

                    //Recorre las imagenes eliminadas, para borrarlas en su ruta física y lógica.
                    if (idsImagenesEliminadas != null && idsImagenesEliminadas.Count != 0)
                    {
                        var imagenesParaBorrar = camiseta.ImagenCamiseta
                            .Where(img => idsImagenesEliminadas.Contains(img.IdImagen))
                            .ToList();

                        foreach (var img in imagenesParaBorrar)
                        {
                            // Borrado físico del archivo
                            _imageStorage.DeleteSpecificImage(img.RutaImagen);

                            // Borrado en la base de datos
                            camiseta.ImagenCamiseta.Remove(img);
                        }
                    }

                    //Guardado de imágenes nuevas.
                    if (imagenes != null && imagenes.Any())
                    {
                        foreach (var archivo in imagenes)
                        {
                            string ruta = await _imageStorage.SaveImageAsync(camiseta.IdCamiseta, archivo);
                            
                            byte proximoId = (byte)((camiseta.ImagenCamiseta.Count != 0
                                             ? camiseta.ImagenCamiseta.Max(i => i.IdImagen)
                                             : 0) + 1);

                            camiseta.ImagenCamiseta.Add(new ImagenCamiseta
                            {
                                IdImagen = proximoId,
                                IdCamiseta = camiseta.IdCamiseta,
                                RutaImagen = ruta
                            });
                        }
                    }

                    //Guarda los registros de imagenes y persiste lo realizado durante la transacción.
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch (Exception)
                {
                    //Al produciarse una excepción, ejecuta Rollback sobre las entidades.
                    await _unitOfWork.RollBackTransactionAsync();
                    
                    throw;
                }
            });
        }

        public async Task ChangeStateAsync(int id)
        {
            await _repositoryCamiseta.ChangeStateAsync(id);
        }
    }
}

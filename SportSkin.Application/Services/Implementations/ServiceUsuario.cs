using AutoMapper;
using SportSkin.Application.DTOs;
using SportSkin.Application.Services.Interfaces;
using SportSkin.Infrastructure.Data;
using SportSkin.Infrastructure.Models;
using SportSkin.Infrastructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.Services.Implementations
{
    public class ServiceUsuario : IServiceUsuario
    {
        private readonly IRepositoryUsuario _repository;
        private readonly IMapper _mapper;

        public ServiceUsuario(IRepositoryUsuario repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<UsuarioDTO>> ListAsync()
        {
            var usuarios = await _repository.ListAsync();
            var usuariosDTO = _mapper.Map<ICollection<UsuarioDTO>>(usuarios);

            return usuariosDTO;
        }

        public async Task<UsuarioDTO> FindByIdAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);
            return _mapper.Map<UsuarioDTO>(entity);
        }

        /*
          Crea un nuevo usuario aplicando validaciones de negocio:
            - Correo único
            - Nombre de usuario único
            - Contraseña mínimo 6 caracteres (ya validado en ViewModel con [StringLength])
          La contraseña se almacena tal cual por ahora (sin hash, sin autenticación real en este avance).
        */
        public async Task<int> AddAsync(UsuarioDTO dto)
        {
            //  Regla: correo único 
            bool correoUsado = await _repository.ExisteCorreoAsync(dto.Correo);
            if (correoUsado)
                throw new InvalidOperationException(
                    $"El correo '{dto.Correo}' ya está registrado en el sistema.");

            //  Regla: nombre de usuario único 
            bool usuarioUsado = await _repository.ExisteUsuarioAsync(dto.Usuario1);
            if (usuarioUsado)
                throw new InvalidOperationException(
                    $"El nombre de usuario '{dto.Usuario1}' ya está en uso.");

            var entity = _mapper.Map<Usuario>(dto);
            return await _repository.AddAsync(entity);
        }


        /* 
           Actualiza solo los campos de perfil permitidos: Nombre, Apellidos, Correo, Teléfono
           El Rol, contraseña y FechaCreacion nunca se modifican desde aquí
        */
        public async Task UpdateAsync(int id, UsuarioDTO dto)
        {
            var existing = await _repository.FindByIdAsync(id)
                ?? throw new KeyNotFoundException($"No existe el usuario con id={id}");

            /* Mapeamos manualmente solo los campos editables para evitar
            que AutoMapper sobreescriba datos sensibles (rol, contraseña, fecha)*/
            existing.Nombre = dto.Nombre;
            existing.Apellido1 = dto.Apellido1;
            existing.Apellido2 = dto.Apellido2;
            existing.Correo = dto.Correo;
            existing.Telefono = dto.Telefono;

            await _repository.UpdateAsync(existing);
        }

        
        // Cambia el estado lógico del usuario (activo o inactivo).
        public async Task ChangeStateAsync(int id)
        {
            await _repository.ChangeStateAsync(id);
        }

        public async Task ChangePasswordAsync(int id, string nuevaContrasenna)
        {
            _ = await _repository.FindByIdAsync(id)
                ?? throw new KeyNotFoundException($"No existe el usuario con id={id}");

            await _repository.ChangePasswordAsync(id, nuevaContrasenna);
        }

        //Se obtiene el catálogo de roles
        public async Task<ICollection<RolUsuarioDTO>> GetRolesAsync()
        {
            var roles = await _repository.GetRolesAsync();
            return _mapper.Map<ICollection<RolUsuarioDTO>>(roles);
        }

        public async Task<(int total, int activas, int vendidas, int finalizadas)> GetEstadisticasVendedorAsync(int idUsuario)
        {
            var total = await _repository.CountSubastasByVendedorAsync(idUsuario);
            var activas = await _repository.CountSubastasActivasByVendedorAsync(idUsuario);
            var vendidas = await _repository.CountSubastasVendidasByVendedorAsync(idUsuario);
            var finalizadas = total - activas - vendidas;
            return (total, activas, vendidas, finalizadas);
        }
    }
}

using AutoMapper;
using Libreria.Application.Utils;
using Microsoft.Extensions.Configuration;
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
        private readonly IRepositoryUsuario _repositoryUsuario;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ServiceUsuario(IRepositoryUsuario repository, IMapper mapper, IConfiguration configuration)
        {
            _repositoryUsuario = repository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ICollection<UsuarioDTO>> ListAsync()
        {
            var usuarios = await _repositoryUsuario.ListAsync();
            var usuariosDTO = _mapper.Map<ICollection<UsuarioDTO>>(usuarios);

            return usuariosDTO;
        }

        public async Task<UsuarioDTO> FindByIdAsync(int id)
        {
            var entity = await _repositoryUsuario.FindByIdAsync(id);
            return _mapper.Map<UsuarioDTO>(entity);
        }

        /*
          Crea un nuevo usuario aplicando validaciones de negocio:
            - Correo único
            - Nombre de usuario único
            - Contraseña mínimo 6 caracteres (ya validado en ViewModel con [StringLength])         
        */
        public async Task<int> AddAsync(UsuarioDTO dto)
        {
            //  Regla: correo único 
            bool correoUsado = await _repositoryUsuario.ExisteCorreoAsync(dto.Correo);
            if (correoUsado)
                throw new InvalidOperationException(
                    $"El correo '{dto.Correo}' ya está registrado en el sistema. Por favor digite otro valor.");

            //  Regla: nombre de usuario único 
            bool usuarioUsado = await _repositoryUsuario.ExisteUsuarioAsync(dto.Usuario1);
            if (usuarioUsado)
                throw new InvalidOperationException(
                    $"El nombre de usuario '{dto.Usuario1}' ya está en uso. Por favor digite otro valor");

            //Encriptación de contraseña.
            var secretKey = _configuration["Crypto:Secret"];
            dto.Contrasenna = Cryptography.Encrypt(dto.Contrasenna, secretKey ?? string.Empty);

            var entity = _mapper.Map<Usuario>(dto);

            return await _repositoryUsuario.AddAsync(entity);
        }

        /* 
           Actualiza solo los campos de perfil permitidos: Nombre, Apellidos, Correo, Teléfono
           El Rol, contraseña y FechaCreacion nunca se modifican desde aquí
        */
        public async Task UpdateAsync(int id, UsuarioDTO dto)
        {
            var existing = await _repositoryUsuario.FindByIdAsync(id)
                ?? throw new KeyNotFoundException($"No existe el usuario con id={id}");
            bool correEnUso = await _repositoryUsuario.ExisteCorreoAsync(dto.Correo, id);
            if (correEnUso) throw new InvalidOperationException("Correo ya registrado");
            /* Se mapea manualmente solo los campos editables para evitar
            que AutoMapper sobreescriba datos sensibles (rol, contraseña, fecha)*/
            existing.Nombre = dto.Nombre;
            existing.Apellido1 = dto.Apellido1;
            existing.Apellido2 = dto.Apellido2;
            existing.Correo = dto.Correo;
            existing.Telefono = dto.Telefono;

            await _repositoryUsuario.UpdateAsync(existing);
        }
        
        // Cambia el estado lógico del usuario (activo o inactivo).
        public async Task ChangeStateAsync(int id)
        {
            await _repositoryUsuario.ChangeStateAsync(id);
        }

        public async Task ChangePasswordAsync(int id, string nuevaContrasenna)
        {
            _ = await _repositoryUsuario.FindByIdAsync(id)
                ?? throw new KeyNotFoundException($"No existe el usuario con id={id}");

            //Encriptación de contraseña.
            var secretKey = _configuration["Crypto:Secret"];
            nuevaContrasenna = Cryptography.Encrypt(nuevaContrasenna, secretKey ?? string.Empty);

            await _repositoryUsuario.ChangePasswordAsync(id, nuevaContrasenna);
        }

        //Se obtiene el catálogo de roles
        public async Task<ICollection<RolUsuarioDTO>> GetRolesAsync()
        {
            var roles = await _repositoryUsuario.GetRolesAsync();
            return _mapper.Map<ICollection<RolUsuarioDTO>>(roles);
        }

        public async Task<(int total, int activas, int vendidas, int finalizadas)> GetEstadisticasVendedorAsync(int idUsuario)
        {
            var total = await _repositoryUsuario.CountSubastasByVendedorAsync(idUsuario);
            var activas = await _repositoryUsuario.CountSubastasActivasByVendedorAsync(idUsuario);
            var vendidas = await _repositoryUsuario.CountSubastasVendidasByVendedorAsync(idUsuario);
            var finalizadas = total - activas - vendidas;
            return (total, activas, vendidas, finalizadas);
        }

        //Inicio de sesión a partir de la encriptación de la contraseña digitada.
        public async Task<UsuarioDTO?> LoginAsync(string user, string password)
        {
            UsuarioDTO? usuarioDTO = null;

            var secretKey = _configuration["Crypto:Secret"];
            var claveEncriptada = Cryptography.Encrypt(password, secretKey ?? string.Empty);

            var usuario = await _repositoryUsuario.LoginAsync(user, claveEncriptada);

            if (usuario != null)
                usuarioDTO = _mapper.Map<UsuarioDTO>(usuario);

            return usuarioDTO;
        }

        public async Task<UsuarioDTO> FindByUserAsync(string usuario)
        {
            var entity = await _repositoryUsuario.FindByUserAsync(usuario);

            return _mapper.Map<UsuarioDTO>(entity);
        }
    }
}

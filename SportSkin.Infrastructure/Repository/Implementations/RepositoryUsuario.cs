using Microsoft.EntityFrameworkCore;
using SportSkin.Infrastructure.Data;
using SportSkin.Infrastructure.Models;
using SportSkin.Infrastructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Infrastructure.Repository.Implementations
{
    public class RepositoryUsuario : IRepositoryUsuario
    {
        private readonly SportSkinContext _context;

        public RepositoryUsuario(SportSkinContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Usuario>> ListAsync()
        {
            var usuarios = await _context.Set<Usuario>()
                                         .Include(x => x.IdRolUsuarioNavigation)
                                         .AsNoTracking()
                                         .ToListAsync();

            return usuarios;
        }
        public async Task<Usuario?> FindByIdAsync(int id)
        {
            return await _context.Usuario
                .Include(u => u.IdRolUsuarioNavigation)
                .Include(u => u.Camiseta)
                    .ThenInclude(c => c.Subasta)
                .Include(u => u.Puja)
                .Include(u => u.Subasta)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }

        // Inserta un nuevo usuario en la BD.Estado inicial = true (activo), FechaCreacion = ahora.
        // La contraseña se recibe ya procesada desde el Service.
        public async Task<int> AddAsync(Usuario entity)
        {
            entity.Estado = true;
            entity.FechaCreacion = DateTime.Now;
            entity.FechaModificacion = DateTime.Now;

            _context.Usuario.Add(entity);
            await _context.SaveChangesAsync();
            return entity.IdUsuario;
        }


        /* Actualiza únicamente los campos de perfil editables del usuario:
            Nombre, Apellido1, Apellido2, Correo y Telefono.
            El Rol, la contraseña y la FechaCreacion nunca se modifican aquí.
        */
        public async Task UpdateAsync(Usuario entity)
        {
            var existing = await _context.Usuario.FindAsync(entity.IdUsuario)
                ?? throw new KeyNotFoundException($"No existe el usuario con id={entity.IdUsuario}");

            existing.Nombre = entity.Nombre;
            existing.Apellido1 = entity.Apellido1;
            existing.Apellido2 = entity.Apellido2;
            existing.Correo = entity.Correo;
            existing.Telefono = entity.Telefono;
            existing.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(int id, string nuevaContrasenna)
        {
            var entity = await _context.Usuario.FindAsync(id)
                ?? throw new KeyNotFoundException($"No existe el usuario con id={id}");

            entity.Contrasenna = nuevaContrasenna;
            entity.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        /*
            Cambia el estado lógico del usuario (activo o inactivo).
            No elimina físicamente el registro.
        */
        public async Task ChangeStateAsync(int id)
        {
            var entity = await _context.Usuario.FindAsync(id)
                ?? throw new KeyNotFoundException($"No existe el usuario con id={id}");

            entity.Estado = !entity.Estado;
            entity.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
        }


        // Catálogo de Roles
        public async Task<ICollection<RolUsuario>> GetRolesAsync()
        {
            return await _context.RolUsuario
                                 .AsNoTracking()
                                 .OrderBy(r => r.Nombre)
                                 .ToListAsync();
        }

        // ---- Validaciones de unicidad ---
        // Verifica si el correo ya está en uso, excluirId permite ignorar el usuario actual al editar.
        public async Task<bool> ExisteCorreoAsync(string correo, int? excluirId = null)
        {
            return await _context.Usuario
                .AnyAsync(u => u.Correo == correo
                            && (excluirId == null || u.IdUsuario != excluirId));
        }

        // Verifica si el nombre de usuario ya está en uso.
        public async Task<bool> ExisteUsuarioAsync(string usuario1, int? excluirId = null)
        {
            return await _context.Usuario
                .AnyAsync(u => u.Usuario1 == usuario1
                            && (excluirId == null || u.IdUsuario != excluirId));
        }

        // ---- Fin de Validaciones de unicidad ---

        // Estadísticas de usuarios
        public async Task<int> CountSubastasByVendedorAsync(int idUsuario)
        {
            return await _context.Subasta
                .Include(s => s.IdCamisetaNavigation)
                .CountAsync(s => s.IdCamisetaNavigation.IdUsuarioVendedor == idUsuario);
        }

        public async Task<int> CountSubastasActivasByVendedorAsync(int idUsuario)
        {
            var fechaActual = DateTime.Now;
            return await _context.Subasta
                .Include(s => s.IdCamisetaNavigation)
                .CountAsync(s => s.IdCamisetaNavigation.IdUsuarioVendedor == idUsuario
                              && s.FechaCompra == null
                              && s.FechaCierre > fechaActual);
        }

        public async Task<int> CountSubastasVendidasByVendedorAsync(int idUsuario)
        {
            return await _context.Subasta
                .Include(s => s.IdCamisetaNavigation)
                .CountAsync(s => s.IdCamisetaNavigation.IdUsuarioVendedor == idUsuario
                              && s.FechaCompra != null);
        }
    }
}

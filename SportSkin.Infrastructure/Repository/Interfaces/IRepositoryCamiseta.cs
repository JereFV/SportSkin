using SportSkin.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Infrastructure.Repository.Interfaces
{
    public  interface IRepositoryCamiseta
    {
        Task<ICollection<Camiseta>> ListAsync();

        Task<ICollection<Camiseta>> ListAsyncByUser(int idUsuario);

        Task<Camiseta> FindByIdAsync(int id);
        Task AddAsync(Camiseta entity);
        void Update(Camiseta entity);
        Task DeleteAsync(int id);
        Task ChangeStateAsync(int id);
        Task<Camiseta> FindByIdAsyncWithTracking(int id);

        Task<ICollection<Camiseta>> GetCamisetaByCategoria(int idCategoria);
        Task<ICollection<Camiseta>> GetCamisetaByEquipo(int idEquipo);
        Task<ICollection<Camiseta>> GetCamisetaByJugador(int idJugador);
        Task<ICollection<Camiseta>> GetCamisetaByNombre(string  nombre);
        Task<ICollection<Camiseta>> GetCamisetasAutografiadas();
        Task<ICollection<Camiseta>> GetCamisetasByVendedor(int idUsuarioVendedor);
        Task<ICollection<Camiseta>> GetCamisetasByTemporada(short temporada);
        Task<ICollection<Camiseta>> GetCamisetasVendidas(int idUsuario);
        Task<ICollection<Camiseta>> GetCamisetasEnSubasta(int idUsuario);
        Task<ICollection<Camiseta>> GetCamisetasSinSubasta(int idUsuario);

    }
}

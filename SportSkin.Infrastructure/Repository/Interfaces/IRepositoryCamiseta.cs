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

        Task<Camiseta?> FindByIdAsync(int id);
        Task<int>AddAsync(Camiseta entity);
        Task UpdateAsync(Camiseta entity);
        Task DeleteAsync(int id);

        Task<ICollection<Camiseta>> GetCamisetaByCategoria(int idCategoria);
        Task<ICollection<Camiseta>> GetCamisetaByEquipo(int idEquipo);
        Task<ICollection<Camiseta>> GetCamisetaByJugador(int idJugador);
        Task<ICollection<Camiseta>> GetCamisetaByNombre(string  nombre);
        Task<ICollection<Camiseta>> GetCamisetasAutografiadas();
        Task<ICollection<Camiseta>> GetCamisetasByVendedor(int idUsuarioVendedor);
        Task<ICollection<Camiseta>> GetCamisetasByTemporada(short temporada);
        Task<ICollection<Camiseta>> GetCamisetasVendidas();
        Task<ICollection<Camiseta>> GetCamisetasEnSubasta();
        Task<ICollection<Camiseta>> GetCamisetasSinSubasta();

    }
}

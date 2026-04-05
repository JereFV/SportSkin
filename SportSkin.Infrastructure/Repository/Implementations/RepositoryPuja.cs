using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
    public class RepositoryPuja : IRepositoryPuja
    {
        private readonly SportSkinContext _context;

        public RepositoryPuja(SportSkinContext context)
        {
            _context = context;
        }

        //Guarda una nueva puja mediante una transacción para control de concurrencia.
        public async Task AddAsync(Puja entity)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () => 
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {                   
                    await _context.Puja.AddAsync(entity);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }               
            });                 
        }
    }
}

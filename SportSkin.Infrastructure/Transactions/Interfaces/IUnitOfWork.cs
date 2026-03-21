using SportSkin.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Infrastructure.Transactions.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task BeginTransactionAsync();
        Task<int> SaveChangesAsync();
        Task CommitTransactionAsync();
        Task RollBackTransactionAsync();
    }
}

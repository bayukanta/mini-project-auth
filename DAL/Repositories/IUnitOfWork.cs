using Microsoft.EntityFrameworkCore.Storage;
using DAL.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<User> UserRepository { get; }
        IBaseRepository<UserRole> UserRoleRepository { get; }
        IBaseRepository<Role> RoleRepository { get; }

        void Save();

        Task SaveAsync(CancellationToken cancellationToken = default(CancellationToken));

        IDbContextTransaction StartNewTransaction();

        Task<IDbContextTransaction> StartNewTransactionAsync();

        Task<int> ExecuteSqlCommandAsync(string sql, object[] parameters, CancellationToken cancellationToken = default(CancellationToken));

    }
}

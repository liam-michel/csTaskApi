using TaskApi.Repositories;
using TaskApi.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace TaskApi.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ITaskRepository TaskItems { get; }
    Task<int> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    AppDbContext GetContext();
}
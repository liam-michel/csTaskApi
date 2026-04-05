using Microsoft.EntityFrameworkCore.Storage;
using TaskApi.Data;
using TaskApi.Repositories;

namespace TaskApi.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly ITaskRepository _taskRepository;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(
        AppDbContext context,
        IUserRepository userRepository,
        ITaskRepository taskRepository)
    {
        _context = context;
        _userRepository = userRepository;
        _taskRepository = taskRepository;
    }

    public IUserRepository Users => _userRepository;
    public ITaskRepository TaskItems => _taskRepository;

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
        return _transaction;
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            await _transaction?.CommitAsync()!;
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            await _transaction?.RollbackAsync()!;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public AppDbContext GetContext()
    {
        return _context;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
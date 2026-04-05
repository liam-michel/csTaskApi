using TaskApi.Models;
using TaskApi.UnitOfWork;
using TaskApi.Models.Dtos;

namespace TaskApi.Services;

public class UserService : IUserService, IDisposable
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ILogger<UserService> _logger;
  private readonly IDisposable? _logScope;

  public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
  {
    _unitOfWork = unitOfWork;
    _logger = logger;
    _logScope = logger.BeginScope("{Layer}", "UserService");
  }

  public void Dispose()
  {
    _logScope?.Dispose();
    GC.SuppressFinalize(this);
  }

  public async Task<List<User>> GetAllAsync()
  {
    return await _unitOfWork.Users.GetAllAsync();
  }

  public async Task<User?> GetByIdAsync(int id)
  {
    return await _unitOfWork.Users.GetByIdAsync(id);
  }

  public async Task<User> AddAsync(CreateUserRequest request)
  {
    _logger.LogInformation("Adding user: {Username}", request.Username);
    return await _unitOfWork.Users.AddAsync(request);
  }

  public async Task<User> UpdateDetailsAsync(UpdateUserDetailsRequest request, int userId)
  {
    if (request.Id != userId) throw new UnauthorizedAccessException("You can only update your own account.");

    _logger.LogInformation("Updating details for user {UserId}", request.Id);
    return await _unitOfWork.Users.UpdateDetailsAsync(request);
  }

  public async Task<User> UpdateUserPasswordAsync(UpdateUserPasswordRequest request, int userId)
  {
    if (request.Id != userId) throw new UnauthorizedAccessException("You can only update your own account.");

    _logger.LogInformation("Updating password for user {UserId}", request.Id);
    return await _unitOfWork.Users.UpdateUserPasswordAsync(request);
  }

  public async Task<bool> DeleteAsync(int id, int userId)
  {
    if (id != userId) throw new UnauthorizedAccessException("You can only delete your own account.");

    _logger.LogInformation("Deleting user {UserId}", id);
    return await _unitOfWork.Users.DeleteAsync(id);
  }
}

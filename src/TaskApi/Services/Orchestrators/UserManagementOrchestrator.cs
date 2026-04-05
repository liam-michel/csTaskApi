using TaskApi.Models;
using TaskApi.Models.Dtos;

namespace TaskApi.Services.Orchestrators;

public class UserManagementOrchestrator : IUserManagementOrchestrator, IDisposable
{
    private readonly IUserService _userService;
    private readonly ILogger<UserManagementOrchestrator> _logger;
    private readonly IDisposable? _logScope;

    public UserManagementOrchestrator(
        IUserService userService,
        ILogger<UserManagementOrchestrator> logger)
    {
        _userService = userService;
        _logger = logger;
        _logScope = logger.BeginScope("{Layer}", "UserOrchestrator");
    }

    public void Dispose()
    {
        _logScope?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _userService.GetAllAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userService.GetByIdAsync(id);
    }

    public async Task<User> AddUserAsync(CreateUserRequest request)
    {
        _logger.LogInformation("Orchestrating user creation: {Username}", request.Username);
        return await _userService.AddAsync(request);
    }

    public async Task<User> UpdateUserDetailsAsync(UpdateUserDetailsRequest request, int userId)
    {
        _logger.LogInformation("Orchestrating user details update: {UserId}", request.Id);
        return await _userService.UpdateDetailsAsync(request, userId);
    }

    public async Task<User> UpdateUserPasswordAsync(UpdateUserPasswordRequest request, int userId)
    {
        _logger.LogInformation("Orchestrating password update: {UserId}", request.Id);
        return await _userService.UpdateUserPasswordAsync(request, userId);
    }

    public async Task<bool> DeleteUserAsync(int id, int userId)
    {
        _logger.LogInformation("Orchestrating user deletion: {UserId}", id);
        return await _userService.DeleteAsync(id, userId);
    }
}

using TaskApi.Models;
using TaskApi.Models.Dtos;
namespace TaskApi.Services.Orchestrators;

public interface IUserManagementOrchestrator
{
  Task<List<User>> GetAllUsersAsync();
  Task<User?> GetUserByIdAsync(int id);
  Task<User> AddUserAsync(CreateUserRequest request);
  Task<User> UpdateUserDetailsAsync(UpdateUserDetailsRequest request, int userId);
  Task<User> UpdateUserPasswordAsync(UpdateUserPasswordRequest request, int userId);
  Task<bool> DeleteUserAsync(int id, int userId);
}
  
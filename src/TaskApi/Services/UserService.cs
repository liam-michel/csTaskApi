using TaskApi.Models;
using TaskApi.Models.Dtos;

namespace TaskApi.Services;

public interface IUserService
{
  Task<List<User>> GetAllAsync();
  Task<User?> GetByIdAsync(int id);
  Task<User> AddAsync(CreateUserRequest request);
  Task<User> UpdateDetailsAsync(UpdateUserDetailsRequest request, int userId);
  Task<User> UpdateUserPasswordAsync(UpdateUserPasswordRequest request, int userId);
  Task<bool> DeleteAsync(int id, int userId);
}
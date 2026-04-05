using TaskApi.Models;
using TaskApi.Models.Dtos;
namespace TaskApi.Repositories;

public interface IUserRepository
{
  Task<List<User>> GetAllUsersAsync();
  Task<User?> GetUserByIdAsync(int id);
  Task<User> AddUserAsync(AddUser newUser);
  Task<User> UpdateUserDetailsAsync(UpdateUserDetails updatedUser);
  Task<User> UpdateUserPasswordAsync(UpdateUserPassword updatedUser);
  Task<bool> DeleteUserAsync(int id);
}
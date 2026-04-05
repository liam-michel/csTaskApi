using TaskApi.Models;
using TaskApi.Models.Dtos;
namespace TaskApi.Repositories;

public interface IUserRepository
{
  Task<List<User>> GetAllAsync();
  Task<User?> GetByIdAsync(int id);
  Task<User?> GetByEmailAsync(string email);
  Task<User> AddAsync(CreateUserRequest request);
  Task<User> UpdateDetailsAsync(UpdateUserDetailsRequest request);
  Task<User> UpdateUserPasswordAsync(UpdateUserPasswordRequest request);
  Task<bool> DeleteAsync(int id);
}
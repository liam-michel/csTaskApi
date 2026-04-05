using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Models;
using TaskApi.Models.Dtos;

namespace TaskApi.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
  private readonly AppDbContext _context = context;

  public async Task<User?> GetUserByIdAsync(int id)
  {
    return await _context.Users.Include(u => u.Tasks).FirstOrDefaultAsync(u => u.Id == id);
  }
  public async Task<List<User>> GetAllUsersAsync()
  {
    return await _context.Users.Include(u => u.Tasks).ToListAsync();
  }
  public async Task<User> AddUserAsync(AddUser newuser)
  {
    var user = new User
    {
      Username = newuser.Username,
      Email = newuser.Email,
      PasswordHash = newuser.PasswordHash
    };
    var createdUser = _context.Users.Add(user).Entity;
    await _context.SaveChangesAsync();
    return createdUser;
  }
  public async Task<User> UpdateUserDetailsAsync(UpdateUserDetails updatedUser)
  {
    var user = await _context.Users.FindAsync(updatedUser.Id);
    if (user == null) throw new InvalidOperationException("User not found");
    user.Username = updatedUser.Username;
    user.Email = updatedUser.Email;
    await _context.SaveChangesAsync();
    return user;
  }
  public async Task<User> UpdateUserPasswordAsync(UpdateUserPassword updatedUser)
  {
    var user = await _context.Users.FindAsync(updatedUser.Id);
    if (user == null) throw new InvalidOperationException("User not found");
    user.PasswordHash = updatedUser.PasswordHash;
    await _context.SaveChangesAsync();
    return user;
  }

  public async Task<bool> DeleteUserAsync(int id)
  {
    var user = await _context.Users.FindAsync(id);
    if (user == null) return false;
    _context.Users.Remove(user);
    await _context.SaveChangesAsync();
    return true;
  }

}
using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Models;
using TaskApi.Models.Dtos;

namespace TaskApi.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
  private readonly AppDbContext _context = context;

  public async Task<User?> GetByIdAsync(int id)
  {
    return await _context.Users.Include(u => u.Tasks).FirstOrDefaultAsync(u => u.Id == id);
  }
  public async Task<User?> GetByEmailAsync(string email)
  {
    return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
  }
  public async Task<List<User>> GetAllAsync()
  {
    return await _context.Users.Include(u => u.Tasks).ToListAsync();
  }
  public async Task<User> AddAsync(CreateUserRequest request)
  {
    var user = new User
    {
      Username = request.Username,
      Email = request.Email,
      PasswordHash = request.PasswordHash
    };
    var createdUser = _context.Users.Add(user).Entity;
    await _context.SaveChangesAsync();
    return createdUser;
  }
  public async Task<User> UpdateDetailsAsync(UpdateUserDetailsRequest request)
  {
    var user = await _context.Users.FindAsync(request.Id) ?? throw new InvalidOperationException("User not found");
    user.Username = request.Username;
    user.Email = request.Email;
    await _context.SaveChangesAsync();
    return user;
  }
  public async Task<User> UpdateUserPasswordAsync(UpdateUserPasswordRequest request)
  {
    var user = await _context.Users.FindAsync(request.Id);
    if (user == null) throw new InvalidOperationException("User not found");
    user.PasswordHash = request.PasswordHash;
    await _context.SaveChangesAsync();
    return user;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var user = await _context.Users.FindAsync(id);
    if (user == null) return false;
    _context.Users.Remove(user);
    await _context.SaveChangesAsync();
    return true;
  }

}
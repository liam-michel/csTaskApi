using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaskApi.Models;
using TaskApi.Models.Dtos;
using TaskApi.Repositories;

namespace TaskApi.Services;

public class AuthService : IAuthService
{
  private readonly IUserRepository _userRepository;
  private readonly IConfiguration _config;

  public AuthService(IUserRepository userRepository, IConfiguration config)
  {
    _userRepository = userRepository;
    _config = config;
  }

  public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
  {
    var existingUser = await _userRepository.GetByEmailAsync(request.Email);
    if (existingUser != null)
      throw new InvalidOperationException("A user with this email already exists.");

    var createRequest = new CreateUserRequest
    {
      Username = request.Username,
      Email = request.Email,
      PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
    };

    var user = await _userRepository.AddAsync(createRequest);

    return new AuthResponse
    {
      Token = GenerateToken(user),
      UserId = user.Id,
      Username = user.Username,
      Email = user.Email
    };
  }

  public async Task<AuthResponse> LoginAsync(LoginRequest request)
  {
    var user = await _userRepository.GetByEmailAsync(request.Email);
    if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
      throw new UnauthorizedAccessException("Invalid email or password.");

    return new AuthResponse
    {
      Token = GenerateToken(user),
      UserId = user.Id,
      Username = user.Username,
      Email = user.Email
    };
  }

  private string GenerateToken(User user)
  {
    var claims = new[]
    {
      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new Claim(ClaimTypes.Name, user.Username),
      new Claim(ClaimTypes.Email, user.Email)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      issuer: _config["Jwt:Issuer"],
      audience: _config["Jwt:Audience"],
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiresInMinutes"]!)),
      signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}

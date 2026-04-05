namespace TaskApi.Models.Dtos;

public class CreateUserRequest
{
  public string Username { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string PasswordHash { get; set; } = string.Empty;
}

public class UpdateUserDetailsRequest
{
  public int Id { get; set; }
  public string Username { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
}

public class UpdateUserPasswordRequest
{
  public int Id { get; set; }
  public string PasswordHash { get; set; } = string.Empty;
} 





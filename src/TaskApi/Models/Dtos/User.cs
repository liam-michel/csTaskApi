namespace TaskApi.Models.Dtos;

public class AddUser
{
  public string Username { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string PasswordHash { get; set; } = string.Empty;
}

public class UpdateUserDetails
{
  public int Id { get; set; }
  public string Username { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
}

public class UpdateUserPassword
{
  public int Id { get; set; }
  public string PasswordHash { get; set; } = string.Empty;
} 





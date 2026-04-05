namespace TaskApi.Models.Dtos;

public class CreateTaskRequest
{
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public int UserId { get; set; }  // Foreign key to User
}


public class UpdateTaskRequest
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public bool IsCompleted { get; set; }
} 



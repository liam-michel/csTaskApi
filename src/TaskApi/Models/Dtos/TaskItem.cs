namespace TaskApi.Models.Dtos;

public class CreateTaskItem
{
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public int UserId { get; set; }  // Foreign key to User
}


public class UpdateTaskItem
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public bool IsCompleted { get; set; }
} 



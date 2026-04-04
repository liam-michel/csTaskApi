using TaskApi.Models;
namespace TaskApi.Services;

public class TaskService : ITaskService
{
  private static List<TaskItem> _tasks = new();
  private static int _nextId = 1;

  public Task<List<TaskItem>> GetAllTasksAsync()
  {
    return Task.FromResult(_tasks);
  }

  public Task<TaskItem?> GetTaskByIdAsync(int id)
  {
    var task = _tasks.FirstOrDefault(t => t.Id == id);
    return Task.FromResult(task);
  }
  public Task<TaskItem> CreateTaskAsync(TaskItem task)
  {
    task.Id = _nextId++;
    _tasks.Add(task);
    return Task.FromResult(task);
  }
  public Task<TaskItem?> UpdateTaskAsync(int id, TaskItem task)
  {
    var existingTask = _tasks.FirstOrDefault(t => t.Id == id);
    if (existingTask == null)
    {
      return Task.FromResult<TaskItem?>(null);
    }
    existingTask.Title = task.Title;
    existingTask.Description = task.Description;
    existingTask.IsCompleted = task.IsCompleted;
    return Task.FromResult<TaskItem?>(existingTask);
  }
  public Task<bool> DeleteTaskAsync(int id)
  {
    var task = _tasks.FirstOrDefault(t => t.Id == id);
    if (task == null)
    {
      return Task.FromResult(false);
    }
    _tasks.Remove(task);
    return Task.FromResult(true);
  }
  
}
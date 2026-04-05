using TaskApi.Models;
using TaskApi.Models.Dtos;

namespace TaskApi.Repositories;

public interface ITaskRepository
{
  Task<List<TaskItem>> GetAllAsync();
  Task<List<TaskItem>> GetByUserIdAsync(int userId);
  Task<TaskItem?> GetByIdAsync(int id);
  Task<TaskItem> CreateAsync(CreateTaskRequest request);
  Task<TaskItem?> UpdateAsync(UpdateTaskRequest request);
  Task<bool> DeleteAsync(int id);
}

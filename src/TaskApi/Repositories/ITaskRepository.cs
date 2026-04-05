using TaskApi.Models;
using TaskApi.Models.Dtos;

namespace TaskApi.Repositories;

public interface ITaskRepository
{
  Task<List<TaskItem>> GetAllAsync();
  Task<TaskItem?> GetByIdAsync(int id);
  Task<TaskItem> CreateAsync(CreateTaskItem newTask);
  Task<TaskItem?> UpdateAsync(UpdateTaskItem updatedTask);
  Task<bool> DeleteAsync(int id);
}

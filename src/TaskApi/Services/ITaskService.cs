using TaskApi.Models;
using TaskApi.Models.Dtos;

namespace TaskApi.Services;

public interface ITaskService
{
    Task<List<TaskItem>> GetAllAsync();
    Task<List<TaskItem>> GetAllByUserAsync(int userId);
    Task<TaskItem?> GetByIdAsync(int id, int userId);
    Task<TaskItem> CreateAsync(CreateTaskRequest request);
    Task<TaskItem?> UpdateAsync(UpdateTaskRequest request, int userId);
    Task<bool> DeleteAsync(int id, int userId);
}
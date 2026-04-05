using TaskApi.Models;
using TaskApi.UnitOfWork;
using TaskApi.Models.Dtos;

namespace TaskApi.Services;

public class TaskItemService : ITaskService, IDisposable
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TaskItemService> _logger;
    private readonly IDisposable? _logScope;

    public TaskItemService(IUnitOfWork unitOfWork, ILogger<TaskItemService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _logScope = logger.BeginScope("{Layer}", "TaskService");
    }

    public void Dispose()
    {
        _logScope?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task<List<TaskItem>> GetAllAsync()
    {
        return await _unitOfWork.TaskItems.GetAllAsync();
    }

    public async Task<List<TaskItem>> GetAllByUserAsync(int userId)
    {
        return await _unitOfWork.TaskItems.GetByUserIdAsync(userId);
    }

    public async Task<TaskItem?> GetByIdAsync(int id, int userId)
    {
        var task = await _unitOfWork.TaskItems.GetByIdAsync(id);
        if (task == null || task.UserId != userId) return null;
        return task;
    }

    public async Task<TaskItem> CreateAsync(CreateTaskRequest request)
    {
        _logger.LogInformation("Creating task: {Title}", request.Title);
        return await _unitOfWork.TaskItems.CreateAsync(request);
    }

    public async Task<TaskItem?> UpdateAsync(UpdateTaskRequest request, int userId)
    {
        var task = await _unitOfWork.TaskItems.GetByIdAsync(request.Id);
        if (task == null || task.UserId != userId) return null;

        _logger.LogInformation("Updating task {TaskId}", request.Id);
        return await _unitOfWork.TaskItems.UpdateAsync(request);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var task = await _unitOfWork.TaskItems.GetByIdAsync(id);
        if (task == null || task.UserId != userId) return false;

        _logger.LogInformation("Deleting task {TaskId}", id);
        return await _unitOfWork.TaskItems.DeleteAsync(id);
    }
}

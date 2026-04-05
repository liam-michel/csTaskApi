using TaskApi.Models;
using TaskApi.Models.Dtos;

namespace TaskApi.Services.Orchestrators;

public class TaskManagementOrchestrator : ITaskManagementOrchestrator, IDisposable
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TaskManagementOrchestrator> _logger;
    private readonly IDisposable? _logScope;

    public TaskManagementOrchestrator(
        ITaskService taskService,
        ILogger<TaskManagementOrchestrator> logger)
    {
        _taskService = taskService;
        _logger = logger;
        _logScope = logger.BeginScope("{Layer}", "TaskOrchestrator");
    }

    public void Dispose()
    {
        _logScope?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task<List<TaskItem>> GetAllAsync(int userId)
    {
        return await _taskService.GetAllByUserAsync(userId);
    }

    public async Task<TaskItem?> GetByIdAsync(int id, int userId)
    {
        return await _taskService.GetByIdAsync(id, userId);
    }

    public async Task<TaskItem> CreateAsync(CreateTaskRequest request)
    {
        _logger.LogInformation("Orchestrating task creation: {Title}", request.Title);
        return await _taskService.CreateAsync(request);
    }

    public async Task<TaskItem?> UpdateAsync(UpdateTaskRequest request, int userId)
    {
        _logger.LogInformation("Orchestrating task update: {TaskId}", request.Id);
        return await _taskService.UpdateAsync(request, userId);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        _logger.LogInformation("Orchestrating task deletion: {TaskId}", id);
        return await _taskService.DeleteAsync(id, userId);
    }
}

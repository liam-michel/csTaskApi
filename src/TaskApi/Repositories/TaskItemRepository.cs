using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using TaskApi.Data;
using TaskApi.Models;
using TaskApi.Models.Dtos;

namespace TaskApi.Repositories;

public class TaskItemRepository(AppDbContext context) : ITaskRepository
{
    private readonly AppDbContext _context = context;

  public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _context.Tasks
            .Include(t => t.User)  // Load related user
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<TaskItem>> GetAllAsync()
    {
        return await _context.Tasks
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<TaskItem> CreateAsync(CreateTaskItem newTask)
    {
        var task = new TaskItem
        {
            Title = newTask.Title,
            Description = newTask.Description,
            UserId = newTask.UserId

        };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem?> UpdateAsync(UpdateTaskItem updatedTask)
    {
        var task = await _context.Tasks.FindAsync(updatedTask.Id);
        if (task == null) return null;

        task.Title = updatedTask.Title;
        task.Description = updatedTask.Description;
        task.IsCompleted = updatedTask.IsCompleted;

        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<TaskItem>> GetByUserIdAsync(int userId)
    {
        return await _context.Tasks
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}
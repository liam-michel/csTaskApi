using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Models;
using TaskApi.Models.Dtos;
using TaskApi.Repositories;

namespace TaskApi.Tests.Repositories;

public class TaskItemRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly TaskItemRepository _sut;

    public TaskItemRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _sut = new TaskItemRepository(_context);
    }

    public void Dispose() => _context.Dispose();

    private async Task<User> SeedUserAsync()
    {
        var user = new User { Username = "liam", Email = "liam@example.com", PasswordHash = "hashed" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    private async Task<TaskItem> SeedTaskAsync(int userId, string title = "Test task")
    {
        var task = new TaskItem { Title = title, Description = "desc", UserId = userId };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    // --- GetAllAsync ---

    [Fact]
    public async Task GetAllAsync_ReturnsTasks()
    {
        var user = await SeedUserAsync();
        await SeedTaskAsync(user.Id, "Task 1");
        await SeedTaskAsync(user.Id, "Task 2");

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_EmptyDb_ReturnsEmptyList()
    {
        var result = await _sut.GetAllAsync();
        result.Should().BeEmpty();
    }

    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_ExistingTask_ReturnsTask()
    {
        var user = await SeedUserAsync();
        var seeded = await SeedTaskAsync(user.Id, "Fix bug");

        var result = await _sut.GetByIdAsync(seeded.Id);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Fix bug");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingTask_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(999);
        result.Should().BeNull();
    }

    // --- CreateAsync ---

    [Fact]
    public async Task CreateAsync_ValidRequest_SavesAndReturnsTask()
    {
        var user = await SeedUserAsync();
        var request = new CreateTaskRequest { Title = "New task", Description = "Details", UserId = user.Id };

        var result = await _sut.CreateAsync(request);

        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("New task");
        result.Description.Should().Be("Details");
        result.UserId.Should().Be(user.Id);

        var inDb = await _context.Tasks.FindAsync(result.Id);
        inDb.Should().NotBeNull();
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_ExistingTask_UpdatesAndReturnsTask()
    {
        var user = await SeedUserAsync();
        var seeded = await SeedTaskAsync(user.Id);
        var request = new UpdateTaskRequest { Id = seeded.Id, Title = "Updated", Description = "Updated desc", IsCompleted = true };

        var result = await _sut.UpdateAsync(request);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated");
        result.IsCompleted.Should().BeTrue();

        var inDb = await _context.Tasks.FindAsync(seeded.Id);
        inDb!.Title.Should().Be("Updated");
        inDb.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_NonExistingTask_ReturnsNull()
    {
        var request = new UpdateTaskRequest { Id = 999, Title = "Updated" };

        var result = await _sut.UpdateAsync(request);

        result.Should().BeNull();
    }

    // --- DeleteAsync ---

    [Fact]
    public async Task DeleteAsync_ExistingTask_DeletesAndReturnsTrue()
    {
        var user = await SeedUserAsync();
        var seeded = await SeedTaskAsync(user.Id);

        var result = await _sut.DeleteAsync(seeded.Id);

        result.Should().BeTrue();
        var inDb = await _context.Tasks.FindAsync(seeded.Id);
        inDb.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistingTask_ReturnsFalse()
    {
        var result = await _sut.DeleteAsync(999);
        result.Should().BeFalse();
    }
}

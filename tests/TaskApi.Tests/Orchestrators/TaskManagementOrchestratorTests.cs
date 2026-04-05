using FluentAssertions;
using Moq;
using TaskApi.Models;
using TaskApi.Models.Dtos;
using TaskApi.Services;
using TaskApi.Services.Orchestrators;
using Microsoft.Extensions.Logging;

namespace TaskApi.Tests.Orchestrators;

public class TaskManagementOrchestratorTests
{
    private const int MockUserId = 1;
    private readonly Mock<ITaskService> _taskServiceMock = new();
    private readonly Mock<ILogger<TaskManagementOrchestrator>> _loggerMock = new();
    private readonly TaskManagementOrchestrator _sut;

    public TaskManagementOrchestratorTests()
    {
        _sut = new TaskManagementOrchestrator(_taskServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsTasks()
    {
        var tasks = new List<TaskItem>
        {
            new() { Id = 1, Title = "Task 1", UserId = 1 },
            new() { Id = 2, Title = "Task 2", UserId = 1 }
        };
        _taskServiceMock.Setup(s => s.GetAllByUserAsync(1)).ReturnsAsync(tasks);

        var result = await _sut.GetAllAsync(1);

        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(tasks);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsTask()
    {
        var task = new TaskItem { Id = 1, Title = "Task 1" };
        _taskServiceMock.Setup(s => s.GetByIdAsync(1, MockUserId)).ReturnsAsync(task);

        var result = await _sut.GetByIdAsync(1, MockUserId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Should().Be("Task 1");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        _taskServiceMock.Setup(s => s.GetByIdAsync(99, MockUserId)).ReturnsAsync((TaskItem?)null);

        var result = await _sut.GetByIdAsync(99, MockUserId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsCreatedTask()
    {
        var request = new CreateTaskRequest { Title = "New task", Description = "Details", UserId = 1 };
        var created = new TaskItem { Id = 1, Title = "New task", Description = "Details", UserId = 1 };
        _taskServiceMock.Setup(s => s.CreateAsync(request)).ReturnsAsync(created);

        var result = await _sut.CreateAsync(request);

        result.Id.Should().Be(1);
        result.Title.Should().Be("New task");
    }

    [Fact]
    public async Task UpdateAsync_ExistingTask_ReturnsUpdatedTask()
    {
        var request = new UpdateTaskRequest { Id = 1, Title = "Updated", Description = "Updated desc", IsCompleted = true };
        var updated = new TaskItem { Id = 1, Title = "Updated", IsCompleted = true };
        _taskServiceMock.Setup(s => s.UpdateAsync(request, MockUserId)).ReturnsAsync(updated);

        var result = await _sut.UpdateAsync(request, MockUserId);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated");
        result.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_NonExistingTask_ReturnsNull()
    {
        var request = new UpdateTaskRequest { Id = 99, Title = "Updated" };
        _taskServiceMock.Setup(s => s.UpdateAsync(request, MockUserId)).ReturnsAsync((TaskItem?)null);

        var result = await _sut.UpdateAsync(request, MockUserId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ExistingTask_ReturnsTrue()
    {
        _taskServiceMock.Setup(s => s.DeleteAsync(1, MockUserId)).ReturnsAsync(true);

        var result = await _sut.DeleteAsync(1, MockUserId);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_NonExistingTask_ReturnsFalse()
    {
        _taskServiceMock.Setup(s => s.DeleteAsync(99, MockUserId)).ReturnsAsync(false);

        var result = await _sut.DeleteAsync(99, MockUserId);

        result.Should().BeFalse();
    }
}

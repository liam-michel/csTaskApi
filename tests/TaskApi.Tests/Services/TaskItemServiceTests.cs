using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskApi.Models;
using TaskApi.Models.Dtos;
using TaskApi.Repositories;
using TaskApi.Services;
using TaskApi.UnitOfWork;

namespace TaskApi.Tests.Services;

public class TaskItemServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ITaskRepository> _taskRepoMock = new();
    private readonly Mock<ILogger<TaskItemService>> _loggerMock = new();
    private readonly TaskItemService _sut;

    public TaskItemServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.TaskItems).Returns(_taskRepoMock.Object);
        _sut = new TaskItemService(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_DelegatesToRepository()
    {
        var tasks = new List<TaskItem> { new() { Id = 1, Title = "Task 1" } };
        _taskRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

        var result = await _sut.GetAllAsync();

        result.Should().BeEquivalentTo(tasks);
        _taskRepoMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllByUserAsync_DelegatesToRepository()
    {
        var tasks = new List<TaskItem> { new() { Id = 1, Title = "Task 1", UserId = 1 } };
        _taskRepoMock.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(tasks);

        var result = await _sut.GetAllByUserAsync(1);

        result.Should().BeEquivalentTo(tasks);
        _taskRepoMock.Verify(r => r.GetByUserIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_OwnedTask_ReturnsTask()
    {
        var task = new TaskItem { Id = 1, Title = "Task 1", UserId = 1 };
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);

        var result = await _sut.GetByIdAsync(1, userId: 1);

        result.Should().BeEquivalentTo(task);
    }

    [Fact]
    public async Task GetByIdAsync_NotOwnedTask_ReturnsNull()
    {
        var task = new TaskItem { Id = 1, Title = "Task 1", UserId = 2 };
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);

        var result = await _sut.GetByIdAsync(1, userId: 1);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        _taskRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((TaskItem?)null);

        var result = await _sut.GetByIdAsync(99, userId: 1);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_DelegatesToRepository()
    {
        var request = new CreateTaskRequest { Title = "New task", UserId = 1 };
        var created = new TaskItem { Id = 1, Title = "New task", UserId = 1 };
        _taskRepoMock.Setup(r => r.CreateAsync(request)).ReturnsAsync(created);

        var result = await _sut.CreateAsync(request);

        result.Should().BeEquivalentTo(created);
        _taskRepoMock.Verify(r => r.CreateAsync(request), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_OwnedTask_UpdatesAndReturnsTask()
    {
        var request = new UpdateTaskRequest { Id = 1, Title = "Updated" };
        var existing = new TaskItem { Id = 1, Title = "Original", UserId = 1 };
        var updated = new TaskItem { Id = 1, Title = "Updated", UserId = 1 };
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _taskRepoMock.Setup(r => r.UpdateAsync(request)).ReturnsAsync(updated);

        var result = await _sut.UpdateAsync(request, userId: 1);

        result.Should().BeEquivalentTo(updated);
        _taskRepoMock.Verify(r => r.UpdateAsync(request), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NotOwnedTask_ReturnsNull()
    {
        var request = new UpdateTaskRequest { Id = 1, Title = "Updated" };
        var existing = new TaskItem { Id = 1, UserId = 2 };
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        var result = await _sut.UpdateAsync(request, userId: 1);

        result.Should().BeNull();
        _taskRepoMock.Verify(r => r.UpdateAsync(It.IsAny<UpdateTaskRequest>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingTask_ReturnsNull()
    {
        var request = new UpdateTaskRequest { Id = 99, Title = "Updated" };
        _taskRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((TaskItem?)null);

        var result = await _sut.UpdateAsync(request, userId: 1);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_OwnedTask_DeletesAndReturnsTrue()
    {
        var existing = new TaskItem { Id = 1, UserId = 1 };
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _taskRepoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _sut.DeleteAsync(1, userId: 1);

        result.Should().BeTrue();
        _taskRepoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NotOwnedTask_ReturnsFalse()
    {
        var existing = new TaskItem { Id = 1, UserId = 2 };
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        var result = await _sut.DeleteAsync(1, userId: 1);

        result.Should().BeFalse();
        _taskRepoMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingTask_ReturnsFalse()
    {
        _taskRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((TaskItem?)null);

        var result = await _sut.DeleteAsync(99, userId: 1);

        result.Should().BeFalse();
    }
}

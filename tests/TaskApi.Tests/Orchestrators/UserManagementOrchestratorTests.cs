using FluentAssertions;
using Moq;
using TaskApi.Models;
using TaskApi.Models.Dtos;
using TaskApi.Services;
using TaskApi.Services.Orchestrators;
using Microsoft.Extensions.Logging;

namespace TaskApi.Tests.Orchestrators;

public class UserManagementOrchestratorTests
{
    private const int mockUserId = 1; 
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<ILogger<UserManagementOrchestrator>> _loggerMock = new();
    private readonly UserManagementOrchestrator _sut;

    public UserManagementOrchestratorTests()
    {
        _sut = new UserManagementOrchestrator(_userServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsUsers()
    {
        var users = new List<User>
        {
            new() { Id = 1, Username = "liam" },
            new() { Id = 2, Username = "jane" }
        };
        _userServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(users);

        var result = await _sut.GetAllUsersAsync();

        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task GetUserByIdAsync_ExistingId_ReturnsUser()
    {
        var user = new User { Id = 1, Username = "liam", Email = "liam@example.com" };
        _userServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(user);

        var result = await _sut.GetUserByIdAsync(1);

        result.Should().NotBeNull();
        result!.Username.Should().Be("liam");
    }

    [Fact]
    public async Task GetUserByIdAsync_NonExistingId_ReturnsNull()
    {
        _userServiceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((User?)null);

        var result = await _sut.GetUserByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddUserAsync_ValidRequest_ReturnsCreatedUser()
    {
        var request = new CreateUserRequest { Username = "liam", Email = "liam@example.com", PasswordHash = "hashed" };
        var created = new User { Id = 1, Username = "liam", Email = "liam@example.com" };
        _userServiceMock.Setup(s => s.AddAsync(request)).ReturnsAsync(created);

        var result = await _sut.AddUserAsync(request);

        result.Id.Should().Be(1);
        result.Username.Should().Be("liam");
    }

    [Fact]
    public async Task UpdateUserDetailsAsync_ValidRequest_ReturnsUpdatedUser()
    {
        var request = new UpdateUserDetailsRequest { Id = 1, Username = "updated", Email = "updated@example.com" };
        var updated = new User { Id = 1, Username = "updated", Email = "updated@example.com" };
        _userServiceMock.Setup(s => s.UpdateDetailsAsync(request, mockUserId)).ReturnsAsync(updated);

        var result = await _sut.UpdateUserDetailsAsync(request, mockUserId);

        result.Username.Should().Be("updated");
        result.Email.Should().Be("updated@example.com");
    }

    [Fact]
    public async Task DeleteUserAsync_ExistingUser_ReturnsTrue()
    {
        _userServiceMock.Setup(s => s.DeleteAsync(1, mockUserId)).ReturnsAsync(true);

        var result = await _sut.DeleteUserAsync(1, mockUserId);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteUserAsync_NonExistingUser_ReturnsFalse()
    {
        _userServiceMock.Setup(s => s.DeleteAsync(99, mockUserId)).ReturnsAsync(false);

        var result = await _sut.DeleteUserAsync(99, mockUserId);

        result.Should().BeFalse();
    }
}

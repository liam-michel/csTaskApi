using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskApi.Models;
using TaskApi.Models.Dtos;
using TaskApi.Repositories;
using TaskApi.Services;
using TaskApi.UnitOfWork;

namespace TaskApi.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<ILogger<UserService>> _loggerMock = new();
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepoMock.Object);
        _sut = new UserService(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_DelegatesToRepository()
    {
        var users = new List<User> { new() { Id = 1, Username = "liam" } };
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var result = await _sut.GetAllAsync();

        result.Should().BeEquivalentTo(users);
        _userRepoMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_DelegatesToRepository()
    {
        var user = new User { Id = 1, Username = "liam" };
        _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        var result = await _sut.GetByIdAsync(1);

        result.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        var result = await _sut.GetByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_DelegatesToRepository()
    {
        var request = new CreateUserRequest { Username = "liam", Email = "liam@example.com", PasswordHash = "hashed" };
        var created = new User { Id = 1, Username = "liam", Email = "liam@example.com" };
        _userRepoMock.Setup(r => r.AddAsync(request)).ReturnsAsync(created);

        var result = await _sut.AddAsync(request);

        result.Should().BeEquivalentTo(created);
        _userRepoMock.Verify(r => r.AddAsync(request), Times.Once);
    }

    [Fact]
    public async Task UpdateDetailsAsync_SameUser_DelegatesToRepository()
    {
        var request = new UpdateUserDetailsRequest { Id = 1, Username = "updated", Email = "updated@example.com" };
        var updated = new User { Id = 1, Username = "updated", Email = "updated@example.com" };
        _userRepoMock.Setup(r => r.UpdateDetailsAsync(request)).ReturnsAsync(updated);

        var result = await _sut.UpdateDetailsAsync(request, userId: 1);

        result.Should().BeEquivalentTo(updated);
        _userRepoMock.Verify(r => r.UpdateDetailsAsync(request), Times.Once);
    }

    [Fact]
    public async Task UpdateDetailsAsync_DifferentUser_ThrowsUnauthorizedAccessException()
    {
        var request = new UpdateUserDetailsRequest { Id = 2, Username = "updated", Email = "updated@example.com" };

        var act = () => _sut.UpdateDetailsAsync(request, userId: 1);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("You can only update your own account.");
        _userRepoMock.Verify(r => r.UpdateDetailsAsync(It.IsAny<UpdateUserDetailsRequest>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_SameUser_DelegatesToRepository()
    {
        var request = new UpdateUserPasswordRequest { Id = 1, PasswordHash = "newhash" };
        var updated = new User { Id = 1, PasswordHash = "newhash" };
        _userRepoMock.Setup(r => r.UpdateUserPasswordAsync(request)).ReturnsAsync(updated);

        var result = await _sut.UpdateUserPasswordAsync(request, userId: 1);

        result.Should().BeEquivalentTo(updated);
        _userRepoMock.Verify(r => r.UpdateUserPasswordAsync(request), Times.Once);
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_DifferentUser_ThrowsUnauthorizedAccessException()
    {
        var request = new UpdateUserPasswordRequest { Id = 2, PasswordHash = "newhash" };

        var act = () => _sut.UpdateUserPasswordAsync(request, userId: 1);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("You can only update your own account.");
        _userRepoMock.Verify(r => r.UpdateUserPasswordAsync(It.IsAny<UpdateUserPasswordRequest>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_SameUser_DeletesAndReturnsTrue()
    {
        _userRepoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _sut.DeleteAsync(1, userId: 1);

        result.Should().BeTrue();
        _userRepoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DifferentUser_ThrowsUnauthorizedAccessException()
    {
        var act = () => _sut.DeleteAsync(2, userId: 1);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("You can only delete your own account.");
        _userRepoMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingUser_ReturnsFalse()
    {
        _userRepoMock.Setup(r => r.DeleteAsync(99)).ReturnsAsync(false);

        var result = await _sut.DeleteAsync(99, userId: 99);

        result.Should().BeFalse();
    }
}

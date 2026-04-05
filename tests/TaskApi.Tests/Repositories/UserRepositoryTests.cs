using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Models;
using TaskApi.Models.Dtos;
using TaskApi.Repositories;

namespace TaskApi.Tests.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UserRepository _sut;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh DB per test class
            .Options;

        _context = new AppDbContext(options);
        _sut = new UserRepository(_context);
    }

    public void Dispose() => _context.Dispose();

    // Helper to seed a user directly into the DB
    private async Task<User> SeedUserAsync(string username = "liam", string email = "liam@example.com")
    {
        var user = new User { Username = username, Email = email, PasswordHash = "hashed" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // --- GetAllAsync ---

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        await SeedUserAsync("liam", "liam@example.com");
        await SeedUserAsync("jane", "jane@example.com");

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
    public async Task GetByIdAsync_ExistingUser_ReturnsUser()
    {
        var seeded = await SeedUserAsync();

        var result = await _sut.GetByIdAsync(seeded.Id);

        result.Should().NotBeNull();
        result!.Username.Should().Be("liam");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingUser_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(999);
        result.Should().BeNull();
    }

    // --- GetByEmailAsync ---

    [Fact]
    public async Task GetByEmailAsync_ExistingEmail_ReturnsUser()
    {
        await SeedUserAsync();

        var result = await _sut.GetByEmailAsync("liam@example.com");

        result.Should().NotBeNull();
        result!.Email.Should().Be("liam@example.com");
    }

    [Fact]
    public async Task GetByEmailAsync_NonExistingEmail_ReturnsNull()
    {
        var result = await _sut.GetByEmailAsync("ghost@example.com");
        result.Should().BeNull();
    }

    // --- AddAsync ---

    [Fact]
    public async Task AddAsync_ValidRequest_SavesAndReturnsUser()
    {
        var request = new CreateUserRequest { Username = "liam", Email = "liam@example.com", PasswordHash = "hashed" };

        var result = await _sut.AddAsync(request);

        result.Id.Should().BeGreaterThan(0);
        result.Username.Should().Be("liam");
        result.Email.Should().Be("liam@example.com");

        var inDb = await _context.Users.FindAsync(result.Id);
        inDb.Should().NotBeNull();
    }

    // --- UpdateDetailsAsync ---

    [Fact]
    public async Task UpdateDetailsAsync_ExistingUser_UpdatesAndReturnsUser()
    {
        var seeded = await SeedUserAsync();
        var request = new UpdateUserDetailsRequest { Id = seeded.Id, Username = "updated", Email = "updated@example.com" };

        var result = await _sut.UpdateDetailsAsync(request);

        result.Username.Should().Be("updated");
        result.Email.Should().Be("updated@example.com");

        var inDb = await _context.Users.FindAsync(seeded.Id);
        inDb!.Username.Should().Be("updated");
    }

    [Fact]
    public async Task UpdateDetailsAsync_NonExistingUser_ThrowsInvalidOperationException()
    {
        var request = new UpdateUserDetailsRequest { Id = 999, Username = "x", Email = "x@x.com" };

        var act = () => _sut.UpdateDetailsAsync(request);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User not found");
    }

    // --- UpdateUserPasswordAsync ---

    [Fact]
    public async Task UpdateUserPasswordAsync_ExistingUser_UpdatesPassword()
    {
        var seeded = await SeedUserAsync();
        var request = new UpdateUserPasswordRequest { Id = seeded.Id, PasswordHash = "newhash" };

        var result = await _sut.UpdateUserPasswordAsync(request);

        result.PasswordHash.Should().Be("newhash");

        var inDb = await _context.Users.FindAsync(seeded.Id);
        inDb!.PasswordHash.Should().Be("newhash");
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_NonExistingUser_ThrowsInvalidOperationException()
    {
        var request = new UpdateUserPasswordRequest { Id = 999, PasswordHash = "newhash" };

        var act = () => _sut.UpdateUserPasswordAsync(request);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User not found");
    }

    // --- DeleteAsync ---

    [Fact]
    public async Task DeleteAsync_ExistingUser_DeletesAndReturnsTrue()
    {
        var seeded = await SeedUserAsync();

        var result = await _sut.DeleteAsync(seeded.Id);

        result.Should().BeTrue();
        var inDb = await _context.Users.FindAsync(seeded.Id);
        inDb.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistingUser_ReturnsFalse()
    {
        var result = await _sut.DeleteAsync(999);
        result.Should().BeFalse();
    }
}

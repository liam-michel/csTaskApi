using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using TaskApi.Models;
using TaskApi.Models.Dtos;
using TaskApi.Repositories;
using TaskApi.Services;

namespace TaskApi.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly IConfiguration _config;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        // Build a minimal IConfiguration with JWT settings
        var configValues = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "super-secret-test-key-that-is-long-enough-for-hmac!!",
            ["Jwt:Issuer"] = "TaskApi",
            ["Jwt:Audience"] = "TaskApi",
            ["Jwt:ExpiresInMinutes"] = "60"
        };
        _config = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();
        _sut = new AuthService(_userRepoMock.Object, _config);
    }

    // --- Register ---

    [Fact]
    public async Task Register_NewUser_ReturnsAuthResponseWithToken()
    {
        var request = new RegisterRequest { Username = "liam", Email = "liam@example.com", Password = "securepass1" };

        _userRepoMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((User?)null);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<CreateUserRequest>()))
            .ReturnsAsync(new User { Id = 1, Username = request.Username, Email = request.Email, PasswordHash = "hashed" });

        var response = await _sut.RegisterAsync(request);

        response.UserId.Should().Be(1);
        response.Username.Should().Be("liam");
        response.Email.Should().Be("liam@example.com");
        response.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_ExistingEmail_ThrowsInvalidOperationException()
    {
        var request = new RegisterRequest { Username = "liam", Email = "liam@example.com", Password = "securepass1" };
        var existingUser = new User { Id = 1, Email = request.Email };

        _userRepoMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(existingUser);

        var act = () => _sut.RegisterAsync(request);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("A user with this email already exists.");
    }

    [Fact]
    public async Task Register_HashesPasswordBeforeSaving()
    {
        var request = new RegisterRequest { Username = "liam", Email = "liam@example.com", Password = "securepass1" };
        CreateUserRequest? capturedRequest = null;

        _userRepoMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((User?)null);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<CreateUserRequest>()))
            .Callback<CreateUserRequest>(r => capturedRequest = r)
            .ReturnsAsync(new User { Id = 1, Username = request.Username, Email = request.Email, PasswordHash = "hashed" });

        await _sut.RegisterAsync(request);

        capturedRequest!.PasswordHash.Should().NotBe(request.Password);
        BCrypt.Net.BCrypt.Verify(request.Password, capturedRequest.PasswordHash).Should().BeTrue();
    }

    // --- Login ---

    [Fact]
    public async Task Login_ValidCredentials_ReturnsAuthResponseWithToken()
    {
        var password = "securepass1";
        var user = new User
        {
            Id = 1, Username = "liam", Email = "liam@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var response = await _sut.LoginAsync(new LoginRequest { Email = user.Email, Password = password });

        response.UserId.Should().Be(1);
        response.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_UserNotFound_ThrowsUnauthorizedAccessException()
    {
        _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var act = () => _sut.LoginAsync(new LoginRequest { Email = "ghost@example.com", Password = "password123" });

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password.");
    }

    [Fact]
    public async Task Login_WrongPassword_ThrowsUnauthorizedAccessException()
    {
        var user = new User
        {
            Id = 1, Email = "liam@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var act = () => _sut.LoginAsync(new LoginRequest { Email = user.Email, Password = "wrongpassword" });

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password.");
    }
}

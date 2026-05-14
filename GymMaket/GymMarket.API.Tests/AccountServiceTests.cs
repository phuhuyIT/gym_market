using GymMarket.API.DTOs.Account;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Identity;

namespace GymMarket.API.Tests;

public class AccountServiceTests
{
    [Fact]
    public async Task SignUp_rejects_duplicate_email_without_creating_user()
    {
        var existingUser = new AppUser { Id = "existing-user-id", Email = "test@example.com" };
        var repository = new RecordingAccountRepository { ExistingUser = existingUser };
        var service = CreateService(repository);

        var response = await service.SignUp(new SignUpDto
        {
            FullName = "Test User",
            Email = "test@example.com",
            Password = "password123",
            ConfirmPassword = "password123",
            Role = "Student"
        });

        Assert.False(response.Success);
        Assert.Equal(400, response.StatusCode);
        Assert.Contains("EMAIL_ALREADY_EXISTS", response.Errors);
        Assert.Equal(0, repository.CreateUserCallCount);
    }

    [Fact]
    public async Task SignUp_rejects_unknown_role_without_creating_user()
    {
        var repository = new RecordingAccountRepository
        {
            CreateResult = IdentityResult.Success,
            AddToRoleResult = IdentityResult.Success
        };
        var service = CreateService(repository);

        var response = await service.SignUp(new SignUpDto
        {
            FullName = "Test User",
            Email = "test@example.com",
            Password = "password123",
            ConfirmPassword = "password123",
            Role = "Admin"
        });

        Assert.False(response.Success);
        Assert.Equal(400, response.StatusCode);
        Assert.Contains("INVALID_ROLE", response.Errors);
        Assert.Equal(0, repository.CreateUserCallCount);
    }

    [Fact]
    public async Task SignUp_normalizes_supported_role_before_assignment()
    {
        var repository = new RecordingAccountRepository
        {
            CreateResult = IdentityResult.Success,
            AddToRoleResult = IdentityResult.Success
        };
        var service = CreateService(repository);

        var response = await service.SignUp(new SignUpDto
        {
            FullName = "Trainer User",
            Email = "trainer@example.com",
            Password = "password123",
            ConfirmPassword = "password123",
            Role = "trainer"
        });

        Assert.True(response.Success);
        Assert.Equal("Trainer", repository.AssignedRole);
    }

    [Fact]
    public async Task SignUp_returns_error_when_role_assignment_fails()
    {
        var repository = new RecordingAccountRepository
        {
            CreateResult = IdentityResult.Success,
            AddToRoleResult = IdentityResult.Failed(new IdentityError { Description = "Role failed" })
        };
        var service = CreateService(repository);

        var response = await service.SignUp(new SignUpDto
        {
            FullName = "Trainer User",
            Email = "trainer@example.com",
            Password = "password123",
            ConfirmPassword = "password123",
            Role = "Trainer"
        });

        Assert.False(response.Success);
        Assert.Equal(400, response.StatusCode);
        Assert.Contains("ROLE_ASSIGNMENT_FAILED", response.Errors);
    }

    [Fact]
    public async Task Login_rejects_unknown_email()
    {
        var repository = new RecordingAccountRepository();
        var signInService = new RecordingPasswordSignInService();
        var service = CreateService(repository, signInService);

        var response = await service.Login(new LoginDto
        {
            Email = "missing@example.com",
            Password = "password123"
        });

        Assert.False(response.Success);
        Assert.Equal(400, response.StatusCode);
        Assert.Contains("INVALID_CREDENTIALS", response.Errors);
        Assert.Equal(0, signInService.CallCount);
    }

    [Fact]
    public async Task Login_rejects_invalid_password()
    {
        var user = new AppUser { Id = "user-id", Email = "test@example.com" };
        var repository = new RecordingAccountRepository { ExistingUser = user };
        var signInService = new RecordingPasswordSignInService { Result = SignInResult.Failed };
        var service = CreateService(repository, signInService);

        var response = await service.Login(new LoginDto
        {
            Email = "test@example.com",
            Password = "wrong-password"
        });

        Assert.False(response.Success);
        Assert.Equal(400, response.StatusCode);
        Assert.Contains("INVALID_CREDENTIALS", response.Errors);
    }

    [Fact]
    public async Task Login_returns_account_locked_when_identity_locks_user()
    {
        var user = new AppUser { Id = "user-id", Email = "test@example.com" };
        var repository = new RecordingAccountRepository { ExistingUser = user };
        var signInService = new RecordingPasswordSignInService { Result = SignInResult.LockedOut };
        var service = CreateService(repository, signInService);

        var response = await service.Login(new LoginDto
        {
            Email = "test@example.com",
            Password = "password123"
        });

        Assert.False(response.Success);
        Assert.Equal(400, response.StatusCode);
        Assert.Contains("ACCOUNT_LOCKED", response.Errors);
    }

    [Fact]
    public async Task Login_returns_token_when_credentials_are_valid()
    {
        var user = new AppUser { Id = "user-id", Email = "test@example.com" };
        var repository = new RecordingAccountRepository { ExistingUser = user };
        var signInService = new RecordingPasswordSignInService { Result = SignInResult.Success };
        var jwtService = new RecordingJwtService { Token = "jwt-token" };
        var service = CreateService(repository, signInService, jwtService);

        var response = await service.Login(new LoginDto
        {
            Email = "test@example.com",
            Password = "password123"
        });

        Assert.True(response.Success);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal("jwt-token", response.Token);
        Assert.Same(user, jwtService.User);
    }

    private static AccountService CreateService(IAccountRepository repository)
    {
        return CreateService(repository, new RecordingPasswordSignInService(), new RecordingJwtService());
    }

    private static AccountService CreateService(
        IAccountRepository repository,
        IPasswordSignInService? signInService = null,
        IJwtService? jwtService = null)
    {
        return new AccountService(
            repository,
            jwtService ?? new RecordingJwtService(),
            signInService ?? new RecordingPasswordSignInService());
    }

    private sealed class RecordingAccountRepository : IAccountRepository
    {
        public AppUser? ExistingUser { get; init; }
        public IdentityResult CreateResult { get; init; } = IdentityResult.Success;
        public IdentityResult AddToRoleResult { get; init; } = IdentityResult.Success;
        public string? AssignedRole { get; private set; }
        public int CreateUserCallCount { get; private set; }

        public Task<AppUser?> FindByEmail(string email)
        {
            return Task.FromResult(ExistingUser);
        }

        public Task<IdentityResult> CreateUser(AppUser user, string password)
        {
            CreateUserCallCount++;
            return Task.FromResult(CreateResult);
        }

        public Task<IdentityResult> AddToRole(AppUser user, string role)
        {
            AssignedRole = role;
            return Task.FromResult(AddToRoleResult);
        }
    }

    private sealed class RecordingPasswordSignInService : IPasswordSignInService
    {
        public SignInResult Result { get; init; } = SignInResult.Success;
        public int CallCount { get; private set; }

        public Task<SignInResult> PasswordSignInAsync(
            AppUser user,
            string password,
            bool isPersistent,
            bool lockoutOnFailure)
        {
            CallCount++;
            return Task.FromResult(Result);
        }
    }

    private sealed class RecordingJwtService : IJwtService
    {
        public string Token { get; init; } = "token";
        public AppUser? User { get; private set; }

        public Task<string> CreateJWT(AppUser user)
        {
            User = user;
            return Task.FromResult(Token);
        }
    }
}

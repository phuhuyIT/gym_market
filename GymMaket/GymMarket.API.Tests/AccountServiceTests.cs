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
    public async Task SignUp_creates_student_profile_for_student_role()
    {
        var repository = new RecordingAccountRepository
        {
            CreateResult = IdentityResult.Success,
            AddToRoleResult = IdentityResult.Success
        };
        var service = CreateService(repository);

        var response = await service.SignUp(new SignUpDto
        {
            FullName = "Student User",
            Email = "student@example.com",
            Password = "password123",
            ConfirmPassword = "password123",
            Role = "Student",
            HealthStatus = "Active"
        });

        Assert.True(response.Success);
        Assert.Equal(1, repository.CreateStudentCallCount);
        Assert.Equal(0, repository.CreateTrainerCallCount);
        Assert.Equal("Student User", repository.CreatedStudent!.Name);
        Assert.Equal("student@example.com", repository.CreatedStudent.Email);
        Assert.Equal("Active", repository.CreatedStudent.HealthStatus);
    }

    [Fact]
    public async Task SignUp_creates_trainer_profile_for_trainer_role()
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
            Role = "Trainer",
            Certification = "Strength Coach",
            Category = "Strength",
            Bio = "Builds strength programs.",
            Experience = 7
        });

        Assert.True(response.Success);
        Assert.Equal(0, repository.CreateStudentCallCount);
        Assert.Equal(1, repository.CreateTrainerCallCount);
        Assert.Equal("Trainer User", repository.CreatedTrainer!.Name);
        Assert.Equal("trainer@example.com", repository.CreatedTrainer.Email);
        Assert.Equal("Strength Coach", repository.CreatedTrainer.Certification);
        Assert.Equal("Strength", repository.CreatedTrainer.Category);
        Assert.Equal("Builds strength programs.", repository.CreatedTrainer.Bio);
        Assert.Equal(7, repository.CreatedTrainer.Experience);
    }

    [Fact]
    public async Task SignUp_normalizes_trainer_category()
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
            Role = "Trainer",
            Category = "strength"
        });

        Assert.True(response.Success);
        Assert.Equal("Strength", repository.CreatedTrainer!.Category);
    }

    [Fact]
    public async Task SignUp_rejects_invalid_trainer_fields_without_creating_user()
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
            Role = "Trainer",
            Category = "Pilates",
            Experience = 51,
            Certification = new string('c', 201),
            Bio = new string('b', 501)
        });

        Assert.False(response.Success);
        Assert.Equal(400, response.StatusCode);
        Assert.Contains("INVALID_TRAINER_CATEGORY", response.Errors);
        Assert.Contains("INVALID_TRAINER_EXPERIENCE", response.Errors);
        Assert.Contains("TRAINER_CERTIFICATION_TOO_LONG", response.Errors);
        Assert.Contains("TRAINER_BIO_TOO_LONG", response.Errors);
        Assert.Equal(0, repository.CreateUserCallCount);
        Assert.Equal(0, repository.CreateTrainerCallCount);
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
    public async Task SignUp_sends_email_confirmation()
    {
        var repository = new RecordingAccountRepository
        {
            CreateResult = IdentityResult.Success,
            AddToRoleResult = IdentityResult.Success
        };
        var emailSender = new RecordingEmailSender();
        var service = CreateService(repository, emailSender: emailSender);

        var response = await service.SignUp(new SignUpDto
        {
            FullName = "Student User",
            Email = "student@example.com",
            Password = "password123",
            ConfirmPassword = "password123",
            Role = "Student"
        });

        Assert.True(response.Success);
        Assert.Equal(1, emailSender.SendCallCount);
        Assert.Equal("student@example.com", emailSender.ToEmail);
        Assert.Contains("confirm-email", emailSender.HtmlBody);
    }

    [Fact]
    public async Task SignUp_returns_error_when_confirmation_email_fails()
    {
        var repository = new RecordingAccountRepository
        {
            CreateResult = IdentityResult.Success,
            AddToRoleResult = IdentityResult.Success
        };
        var service = CreateService(repository, emailSender: new FailingEmailSender());

        var response = await service.SignUp(new SignUpDto
        {
            FullName = "Student User",
            Email = "student@example.com",
            Password = "password123",
            ConfirmPassword = "password123",
            Role = "Student"
        });

        Assert.False(response.Success);
        Assert.Equal(500, response.StatusCode);
        Assert.Contains("EMAIL_SEND_FAILED", response.Errors);
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
        var repository = new RecordingAccountRepository { ExistingUser = user, EmailConfirmed = true };
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

    [Fact]
    public async Task Login_rejects_unconfirmed_email_after_valid_password()
    {
        var user = new AppUser { Id = "user-id", Email = "test@example.com" };
        var repository = new RecordingAccountRepository { ExistingUser = user, EmailConfirmed = false };
        var signInService = new RecordingPasswordSignInService { Result = SignInResult.Success };
        var jwtService = new RecordingJwtService { Token = "jwt-token" };
        var service = CreateService(repository, signInService, jwtService);

        var response = await service.Login(new LoginDto
        {
            Email = "test@example.com",
            Password = "password123"
        });

        Assert.False(response.Success);
        Assert.Equal(400, response.StatusCode);
        Assert.Contains("EMAIL_NOT_CONFIRMED", response.Errors);
        Assert.Null(jwtService.User);
    }

    [Fact]
    public async Task Login_rejects_suspended_account_after_valid_password()
    {
        var user = new AppUser { Id = "user-id", Email = "test@example.com", Status = "Suspended" };
        var repository = new RecordingAccountRepository { ExistingUser = user, EmailConfirmed = true };
        var signInService = new RecordingPasswordSignInService { Result = SignInResult.Success };
        var jwtService = new RecordingJwtService { Token = "jwt-token" };
        var service = CreateService(repository, signInService, jwtService);

        var response = await service.Login(new LoginDto
        {
            Email = "test@example.com",
            Password = "password123"
        });

        Assert.False(response.Success);
        Assert.Equal(400, response.StatusCode);
        Assert.Contains("ACCOUNT_SUSPENDED", response.Errors);
        Assert.Null(jwtService.User);
    }


    [Fact]
    public async Task GoogleLogin_returns_error_for_invalid_token()
    {
        var repository = new RecordingAccountRepository();
        var service = CreateService(repository);

        var response = await service.GoogleLogin(new GoogleLoginDto { IdToken = "invalid-token" });

        Assert.False(response.Success);
        Assert.Equal(400, response.StatusCode);
        // The error message comes from Google.Apis.Auth, it will likely complain about the token format
        Assert.NotEmpty(response.Errors);
    }

    private static AccountService CreateService(IAccountRepository repository)
    {
        return CreateService(repository, new RecordingPasswordSignInService(), new RecordingJwtService());
    }

    private static AccountService CreateService(
        IAccountRepository repository,
        IPasswordSignInService? signInService = null,
        IJwtService? jwtService = null,
        IEmailSender? emailSender = null)
    {
        return new AccountService(
            repository,
            jwtService ?? new RecordingJwtService(),
            signInService ?? new RecordingPasswordSignInService(),
            new Microsoft.Extensions.Configuration.ConfigurationBuilder().Build(),
            Microsoft.Extensions.Logging.Abstractions.NullLogger<AccountService>.Instance,
            null!,
            emailSender ?? new RecordingEmailSender());
    }

    private sealed class RecordingEmailSender : IEmailSender
    {
        public int SendCallCount { get; private set; }
        public string? ToEmail { get; private set; }
        public string? HtmlBody { get; private set; }

        public Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            SendCallCount++;
            ToEmail = toEmail;
            HtmlBody = htmlBody;
            return Task.CompletedTask;
        }
    }

    private sealed class FailingEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            throw new InvalidOperationException("SMTP unavailable");
        }
    }

    private sealed class RecordingAccountRepository : IAccountRepository
    {
        public AppUser? ExistingUser { get; init; }
        public IdentityResult CreateResult { get; init; } = IdentityResult.Success;
        public IdentityResult AddToRoleResult { get; init; } = IdentityResult.Success;
        public bool EmailConfirmed { get; init; }
        public string? AssignedRole { get; private set; }
        public int CreateUserCallCount { get; private set; }
        public int CreateStudentCallCount { get; private set; }
        public int CreateTrainerCallCount { get; private set; }
        public Student? CreatedStudent { get; private set; }
        public Trainer? CreatedTrainer { get; private set; }
 
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
 
        public Task<AppUser?> FindByLoginAsync(string provider, string providerKey)
        {
            return Task.FromResult<AppUser?>(null);
        }
 
        public Task<IdentityResult> AddLoginAsync(AppUser user, UserLoginInfo login)
        {
            return Task.FromResult(IdentityResult.Success);
        }
 
        public Task<IdentityResult> CreateUserWithoutPasswordAsync(AppUser user)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task CreateStudentAsync(Student student)
        {
            CreateStudentCallCount++;
            CreatedStudent = student;
            return Task.CompletedTask;
        }

        public Task CreateTrainerAsync(Trainer trainer)
        {
            CreateTrainerCallCount++;
            CreatedTrainer = trainer;
            return Task.CompletedTask;
        }

        public Task<AppUser?> FindByIdAsync(string userId) => Task.FromResult(ExistingUser);
        public Task<IdentityResult> UpdateUserAsync(AppUser user) => Task.FromResult(IdentityResult.Success);
        public Task<IdentityResult> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword) => Task.FromResult(IdentityResult.Success);
        public Task<bool> HasPasswordAsync(AppUser user) => Task.FromResult(true);
        public Task<string> GenerateEmailConfirmationTokenAsync(AppUser user) => Task.FromResult("token");
        public Task<IdentityResult> ConfirmEmailAsync(AppUser user, string token) => Task.FromResult(IdentityResult.Success);
        public Task<bool> IsEmailConfirmedAsync(AppUser user) => Task.FromResult(EmailConfirmed);
        public Task<string?> GetAuthenticatorKeyAsync(AppUser user) => Task.FromResult<string?>("key");
        public Task ResetAuthenticatorKeyAsync(AppUser user) => Task.CompletedTask;
        public Task<bool> SetTwoFactorEnabledAsync(AppUser user, bool enabled) => Task.FromResult(true);
        public Task<bool> VerifyTwoFactorTokenAsync(AppUser user, string code) => Task.FromResult(true);
        public Task<bool> GetTwoFactorEnabledAsync(AppUser user) => Task.FromResult(false);
        public Task<bool> IsLockedOutAsync(AppUser user) => Task.FromResult(false);
        public Task<DateTimeOffset?> GetLockoutEndDateAsync(AppUser user) => Task.FromResult<DateTimeOffset?>(null);
        public Task<int> GetAccessFailedCountAsync(AppUser user) => Task.FromResult(0);
        public Task ResetAccessFailedCountAsync(AppUser user) => Task.CompletedTask;
        public Task SetLockoutEndDateAsync(AppUser user, DateTimeOffset? lockoutEnd) => Task.CompletedTask;
        public Task SaveRefreshTokenAsync(RefreshToken refreshToken) => Task.CompletedTask;
        public Task<RefreshToken?> GetRefreshTokenAsync(string token) => Task.FromResult<RefreshToken?>(null);
        public Task RevokeRefreshTokenAsync(RefreshToken refreshToken) => Task.CompletedTask;
        public Task RevokeAllUserRefreshTokensAsync(string userId) => Task.CompletedTask;
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

        public string GenerateRefreshToken() => "refresh-token";
    }
}

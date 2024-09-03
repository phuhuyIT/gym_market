using GymMarket.API.Data;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// server
builder.Services.AddDbContext<GymMarketContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
});


// identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<GymMarketContext>() // provide our context
    .AddDefaultTokenProviders() // create email for email confirmation
    .AddRoles<IdentityRole>() // be able to add roles
    .AddRoleManager<RoleManager<IdentityRole>>() // be able to make use of RoleManager
    .AddSignInManager<SignInManager<AppUser>>() // make use of sign in manager
    .AddUserManager<UserManager<AppUser>>(); // make use of user manager to create user

builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập về Password 
    //options.Password.RequireDigit = false; // Không bắt phải có số 
    //options.Password.RequireLowercase = false; // Không bắt phải có chữ thường 
    //options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt 
    //options.Password.RequireUppercase = false; // Không bắt buộc chữ in 
    //options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password 
    //options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt 
    //                                          // Cấu hình Lockout - khóa user 
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    //options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa 
    //options.Lockout.AllowedForNewUsers = true;
    //// Cấu hình về User. 
    //options.User.AllowedUserNameCharacters = // các ký tự đặt tên user 
    //"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    //options.User.RequireUniqueEmail = true; // Email là duy nhất 
    //                                        // Cấu hình đăng nhập. 
    //options.SignIn.RequireConfirmedEmail = true; // Cấu hình xác thực địa  chỉ email(email phải tồn tại)
    //options.SignIn.RequireConfirmedPhoneNumber = false; // Xác thực số điện thoại
    //                                                    // mặc định false
    //                                                    // nếu true => không cho đăng nhập mà chuyển hướng đến trang  RegisterConfirmation.cshtml
    //options.SignIn.RequireConfirmedAccount = false;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

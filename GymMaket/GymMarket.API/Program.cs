using GymMarket.API.Data;
using GymMarket.API.Hubs;
using GymMarket.API.Models;
using GymMarket.API.Repositories;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Minio;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(typeof(Program));
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
    options.Password.RequireDigit = false; // Không bắt phải có số 
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường 
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt 
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in 
    options.Password.RequiredLength = 8; // Số ký tự tối thiểu của password 
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt 

    // Cấu hình Lockout - khóa user 
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa 
    options.Lockout.AllowedForNewUsers = true;

    //// Cấu hình về User. 
    //options.User.AllowedUserNameCharacters = // các ký tự đặt tên user 
    //"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true; // Email là duy nhất 

    // Cấu hình đăng nhập. 
    //options.SignIn.RequireConfirmedEmail = true; // Cấu hình xác thực địa  chỉ email(email phải tồn tại)
    //options.SignIn.RequireConfirmedPhoneNumber = false; // Xác thực số điện thoại
    //                                                    // mặc định false
    //                                                    // nếu true => không cho đăng nhập mà chuyển hướng đến trang  RegisterConfirmation.cshtml
    //options.SignIn.RequireConfirmedAccount = false;
});

// authenticate user using jwt
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        // validate the issuer (who ever is issuing the JWT)
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true, // validate token based on the key we have provided in appsetting.json

        // don't validate audience (angular side)
        ValidateAudience = false,

        //ValidAudience = builder.Configuration.GetSection("JWT:ValidAudience").Value,
        // the issuer which in here is the api project url
        ValidIssuer = builder.Configuration.GetSection("JWT:Issuer").Value,

        // the issuer signin key based on JWT:Key
        IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:Key").Value!))
    };
});


// repositories
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICourseOptionRepository, CourseOptionRepository>();
builder.Services.AddScoped<CourseRatingRepository>();
builder.Services.AddScoped<CourseRegistrationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ConversationRepository>();
builder.Services.AddScoped<FoodNutritionRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

// service
builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<MinIOService>();


// enable cors
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", option => option.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddSignalR();

builder.Services.AddSingleton<IMinioClient>(sp =>
{
    var config = builder.Configuration.GetSection("MinIO");
    var client = new MinioClient()
                    .WithEndpoint(config["Endpoint"])
                    .WithCredentials(config["AccessKey"], config["SecretKey"])
                    .Build();
    return client;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// enable cors
//app.UseCors(option => option.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200"));

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseWebSockets();
app.MapHub<ChatHub>("hubs/chat");

app.MapControllers();

app.Run();

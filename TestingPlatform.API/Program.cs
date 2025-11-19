using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Infrastructure.Data;
using TestingPlatform.Infrastructure.Repositories;
using TestingPlatform.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];

if (string.IsNullOrEmpty(secretKey))
{
    // Генерируем ключ по умолчанию для development
    secretKey = "your-super-secret-key-that-is-at-least-32-characters-long";
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"] ?? "TestingPlatform",
            ValidAudience = jwtSettings["Audience"] ?? "TestingPlatformUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        // Для .NET 10 - обработка событий аутентификации
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// Repositories
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
builder.Services.AddScoped<ITestRepository, TestRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Важно: UseAuthentication ДО UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        Console.WriteLine("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying migrations: {ex.Message}");
    }
}

app.Run();
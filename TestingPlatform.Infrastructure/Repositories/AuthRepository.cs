
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Domain.Enums;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Data;
using TestingPlatform.Infrastructure.Exceptions;

namespace TestingPlatform.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    public AuthRepository(AppDbContext context, IMapper mapper, IPasswordHasher passwordHasher)
    {
        _context = context;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto> GetUserByLoginAsync(string login)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Login == login);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateUserAsync(RegisterDto registerDto)
    {
        // Проверяем, существует ли пользователь
        if (await UserExistsAsync(registerDto.Login, registerDto.Email))
        {
            throw new InvalidOperationException("Пользователь с таким логином или email уже существует");
        }

        // Создаем пользователя
        var user = new User
        {
            Login = registerDto.Login,
            Email = registerDto.Email,
            PasswordHash = _passwordHasher.HashPassword(registerDto.Password),
            FirstName = registerDto.FirstName,
            MiddleName = registerDto.MiddleName,
            LastName = registerDto.LastName,
            Role = registerDto.Role,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Если это студент, создаем запись в Student
        if (registerDto.Role == UserRole.Student)
        {
            user.Student = new Student
            {
                Phone = string.Empty, // Заполнится позже
                VkProfileLink = string.Empty // Заполнится позже
            };
        }

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> UserExistsAsync(string login, string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Login == login || u.Email == email);
    }

    public async Task<bool> ValidateCredentialsAsync(string login, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Login == login);

        if (user == null)
            return false;

        return _passwordHasher.VerifyPassword(password, user.PasswordHash);
    }

    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new EntityNotFoundException($"User with id {id} not found");

        return _mapper.Map<UserDto>(user);
    }
}
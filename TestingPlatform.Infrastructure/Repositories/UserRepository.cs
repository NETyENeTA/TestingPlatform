
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Data;
using TestingPlatform.Infrastructure.Exceptions;

namespace TestingPlatform.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UserRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _context.Users
            .Include(u => u.Student)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await _context.Users
            .Include(u => u.Student)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new EntityNotFoundException($"User with id {id} not found");

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> GetByLoginAsync(string login)
    {
        var user = await _context.Users
            .Include(u => u.Student)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Login == login);

        if (user == null)
            throw new EntityNotFoundException($"User with login {login} not found");

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateAsync(int id, UpdateUserDto userDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new EntityNotFoundException($"User with id {id} not found");

        // Обновляем только разрешенные поля
        user.Email = userDto.Email;
        user.FirstName = userDto.FirstName;
        user.MiddleName = userDto.MiddleName;
        user.LastName = userDto.LastName;

        await _context.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }
}
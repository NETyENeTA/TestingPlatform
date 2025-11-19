
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Data;
using TestingPlatform.Infrastructure.Exceptions;

namespace TestingPlatform.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public StudentRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<StudentDto>> GetAllAsync()
    {
        var students = await _context.Students
            .Include(s => s.User)
            .Include(s => s.Groups)
            .Include(s => s.Tests)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<StudentDto>>(students);
    }

    public async Task<StudentDto> GetByIdAsync(int id)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .Include(s => s.Groups)
            .Include(s => s.Tests)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
            throw new EntityNotFoundException($"Student with id {id} not found");

        return _mapper.Map<StudentDto>(student);
    }

    public async Task<StudentDto> GetByUserIdAsync(int userId)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .Include(s => s.Groups)
            .Include(s => s.Tests)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (student == null)
            throw new EntityNotFoundException($"Student with user id {userId} not found");

        return _mapper.Map<StudentDto>(student);
    }

    public async Task<StudentDto> CreateAsync(CreateStudentDto studentDto)
    {
        // Проверяем существование пользователя
        var user = await _context.Users.FindAsync(studentDto.UserId);
        if (user == null)
            throw new EntityNotFoundException($"User with id {studentDto.UserId} not found");

        // Проверяем, не является ли пользователь уже студентом
        var existingStudent = await _context.Students
            .FirstOrDefaultAsync(s => s.UserId == studentDto.UserId);

        if (existingStudent != null)
            throw new InvalidOperationException("User is already a student");

        var student = _mapper.Map<Student>(studentDto);

        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(student.Id);
    }

    public async Task<StudentDto> UpdateAsync(int id, UpdateStudentDto studentDto)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
            throw new EntityNotFoundException($"Student with id {id} not found");

        student.Phone = studentDto.Phone;
        student.VkProfileLink = studentDto.VkProfileLink;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
            return false;

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Students.AnyAsync(s => s.Id == id);
    }
}
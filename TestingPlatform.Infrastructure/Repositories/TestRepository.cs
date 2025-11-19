
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Data;
using TestingPlatform.Infrastructure.Exceptions;

namespace TestingPlatform.Infrastructure.Repositories;

public class TestRepository : ITestRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public TestRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TestDto>> GetAllAsync(bool? isPublic = null, List<int>? groupIds = null, List<int>? studentIds = null)
    {
        var query = _context.Tests
            .Include(t => t.Students)
            .Include(t => t.Groups)
            .Include(t => t.Projects)
            .Include(t => t.Courses)
            .Include(t => t.Directions)
            .AsQueryable();

        if (isPublic.HasValue)
            query = query.Where(t => t.IsPublic == isPublic.Value);

        if (groupIds != null && groupIds.Any())
            query = query.Where(t => t.Groups.Any(g => groupIds.Contains(g.Id)));

        if (studentIds != null && studentIds.Any())
            query = query.Where(t => t.Students.Any(s => studentIds.Contains(s.Id)));

        var tests = await query.AsNoTracking().ToListAsync();
        return _mapper.Map<IEnumerable<TestDto>>(tests);
    }

    public async Task<TestDto> GetByIdAsync(int id)
    {
        var test = await _context.Tests
            .Include(t => t.Students)
            .Include(t => t.Groups)
            .Include(t => t.Projects)
            .Include(t => t.Courses)
            .Include(t => t.Directions)
            .Include(t => t.Questions)
                .ThenInclude(q => q.Answers)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (test == null)
            throw new EntityNotFoundException($"Test with id {id} not found");

        return _mapper.Map<TestDto>(test);
    }

    public async Task<IEnumerable<TestDto>> GetAllForStudentAsync(int studentId)
    {
        var tests = await _context.Tests
            .Include(t => t.Students)
            .Include(t => t.Groups)
            .Where(t => t.IsPublic &&
                       (t.Students.Any(s => s.Id == studentId) ||
                        t.Groups.Any(g => g.Students.Any(s => s.Id == studentId))))
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<TestDto>>(tests);
    }

    public async Task<int> CreateAsync(TestDto testDto)
    {
        var test = _mapper.Map<Test>(testDto);

        // Handle many-to-many relationships
        if (testDto.Students?.Any() == true)
        {
            var students = await _context.Students
                .Where(s => testDto.Students.Select(st => st.Id).Contains(s.Id))
                .ToListAsync();
            test.Students = students;
        }

        // Similar logic for Groups, Projects, Courses, Directions

        await _context.Tests.AddAsync(test);
        await _context.SaveChangesAsync();

        return test.Id;
    }

    public async Task UpdateAsync(TestDto testDto)
    {
        var existingTest = await _context.Tests
            .Include(t => t.Students)
            .Include(t => t.Groups)
            .Include(t => t.Projects)
            .Include(t => t.Courses)
            .Include(t => t.Directions)
            .FirstOrDefaultAsync(t => t.Id == testDto.Id);

        if (existingTest == null)
            throw new EntityNotFoundException($"Test with id {testDto.Id} not found");

        _mapper.Map(testDto, existingTest);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var test = await _context.Tests.FindAsync(id);
        if (test == null)
            throw new EntityNotFoundException($"Test with id {id} not found");

        _context.Tests.Remove(test);
        await _context.SaveChangesAsync();
    }
}
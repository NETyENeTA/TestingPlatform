
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Infrastructure.Exceptions;

namespace TestingPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class StudentsController : ControllerBase
{
    private readonly IStudentRepository _studentRepository;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IStudentRepository studentRepository, ILogger<StudentsController> logger)
    {
        _studentRepository = studentRepository;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(typeof(IEnumerable<StudentDto>), 200)]
    public async Task<IActionResult> GetAllStudents()
    {
        try
        {
            var students = await _studentRepository.GetAllAsync();
            return Ok(students);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all students");
            return StatusCode(500, "Ошибка при получении списка студентов");
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(StudentDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetStudentById(int id)
    {
        try
        {
            var student = await _studentRepository.GetByIdAsync(id);
            return Ok(student);
        }
        catch (EntityNotFoundException)
        {
            return NotFound($"Student with id {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting student by id {Id}", id);
            return StatusCode(500, "Ошибка при получении студента");
        }
    }

    [HttpGet("user/{userId:int}")]
    [ProducesResponseType(typeof(StudentDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetStudentByUserId(int userId)
    {
        try
        {
            var student = await _studentRepository.GetByUserIdAsync(userId);
            return Ok(student);
        }
        catch (EntityNotFoundException)
        {
            return NotFound($"Student with user id {userId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting student by user id {UserId}", userId);
            return StatusCode(500, "Ошибка при получении студента");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(typeof(StudentDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto studentDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var student = await _studentRepository.CreateAsync(studentDto);
            return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating student");
            return StatusCode(500, "Ошибка при создании студента");
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(StudentDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentDto studentDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var student = await _studentRepository.UpdateAsync(id, studentDto);
            return Ok(student);
        }
        catch (EntityNotFoundException)
        {
            return NotFound($"Student with id {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating student {Id}", id);
            return StatusCode(500, "Ошибка при обновлении студента");
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        try
        {
            var deleted = await _studentRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound($"Student with id {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting student {Id}", id);
            return StatusCode(500, "Ошибка при удалении студента");
        }
    }
}
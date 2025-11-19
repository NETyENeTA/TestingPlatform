
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Infrastructure.Exceptions;

namespace TestingPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class TestsController : ControllerBase
{
    private readonly ITestRepository _testRepository;
    private readonly ILogger<TestsController> _logger;

    public TestsController(ITestRepository testRepository, ILogger<TestsController> logger)
    {
        _testRepository = testRepository;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TestDto>), 200)]
    public async Task<IActionResult> GetAllTests([FromQuery] bool? isPublic = null)
    {
        try
        {
            var tests = await _testRepository.GetAllAsync(isPublic);
            return Ok(tests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tests");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TestDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTestById(int id)
    {
        try
        {
            var test = await _testRepository.GetByIdAsync(id);
            return Ok(test);
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting test by id {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("student/{studentId:int}")]
    [ProducesResponseType(typeof(IEnumerable<TestDto>), 200)]
    public async Task<IActionResult> GetTestsForStudent(int studentId)
    {
        try
        {
            var tests = await _testRepository.GetAllForStudentAsync(studentId);
            return Ok(tests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tests for student {StudentId}", studentId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreatedResult), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateTest([FromBody] TestDto testDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var testId = await _testRepository.CreateAsync(testDto);
            return CreatedAtAction(nameof(GetTestById), new { id = testId }, new { id = testId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating test");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTest(int id, [FromBody] TestDto testDto)
    {
        if (id != testDto.Id)
            return BadRequest("ID mismatch");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _testRepository.UpdateAsync(testDto);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating test {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteTest(int id)
    {
        try
        {
            await _testRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting test {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
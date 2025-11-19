
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
public class QuestionsController : ControllerBase
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ILogger<QuestionsController> _logger;

    public QuestionsController(IQuestionRepository questionRepository, ILogger<QuestionsController> logger)
    {
        _questionRepository = questionRepository;
        _logger = logger;
    }

    [HttpGet("test/{testId:int}")]
    [ProducesResponseType(typeof(IEnumerable<QuestionDto>), 200)]
    public async Task<IActionResult> GetQuestionsByTestId(int testId)
    {
        try
        {
            var questions = await _questionRepository.GetByTestIdAsync(testId);
            return Ok(questions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting questions for test {TestId}", testId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(QuestionDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetQuestionById(int id)
    {
        try
        {
            var question = await _questionRepository.GetByIdAsync(id);
            return Ok(question);
        }
        catch (EntityNotFoundException)
        {
            return NotFound($"Question with id {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting question by id {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id:int}/with-answers")]
    [ProducesResponseType(typeof(QuestionDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetQuestionByIdWithAnswers(int id)
    {
        try
        {
            var question = await _questionRepository.GetByIdWithAnswersAsync(id);
            return Ok(question);
        }
        catch (EntityNotFoundException)
        {
            return NotFound($"Question with id {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting question with answers by id {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreatedResult), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> CreateQuestion([FromBody] QuestionDto questionDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var questionId = await _questionRepository.CreateAsync(questionDto);
            return CreatedAtAction(nameof(GetQuestionById), new { id = questionId }, new { id = questionId });
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
            _logger.LogError(ex, "Error creating question");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> UpdateQuestion(int id, [FromBody] QuestionDto questionDto)
    {
        if (id != questionDto.Id)
            return BadRequest("ID mismatch");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _questionRepository.UpdateAsync(questionDto);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            return NotFound($"Question with id {id} not found");
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating question {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        try
        {
            await _questionRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            return NotFound($"Question with id {id} not found");
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("test/{testId:int}/count")]
    [ProducesResponseType(typeof(int), 200)]
    public async Task<IActionResult> GetQuestionCount(int testId)
    {
        try
        {
            var count = await _questionRepository.GetQuestionCountAsync(testId);
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting question count for test {TestId}", testId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("test/{testId:int}/next-number")]
    [ProducesResponseType(typeof(int), 200)]
    public async Task<IActionResult> GetNextQuestionNumber(int testId)
    {
        try
        {
            var nextNumber = await _questionRepository.GetNextQuestionNumberAsync(testId);
            return Ok(nextNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting next question number for test {TestId}", testId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("test/{testId:int}/paged")]
    [ProducesResponseType(typeof(IEnumerable<QuestionDto>), 200)]
    public async Task<IActionResult> GetQuestionsPaged(int testId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var questions = await _questionRepository.GetByTestIdWithPaginationAsync(testId, pageNumber, pageSize);
            return Ok(questions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged questions for test {TestId}", testId);
            return StatusCode(500, "Internal server error");
        }
    }
}
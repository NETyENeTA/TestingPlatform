
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
public class AnswersController : ControllerBase
{
    private readonly IAnswerRepository _answerRepository;
    private readonly ILogger<AnswersController> _logger;

    public AnswersController(IAnswerRepository answerRepository, ILogger<AnswersController> logger)
    {
        _answerRepository = answerRepository;
        _logger = logger;
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AnswerDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetAnswerById(int id)
    {
        try
        {
            var answer = await _answerRepository.GetByIdAsync(id);
            return Ok(answer);
        }
        catch (EntityNotFoundException)
        {
            return NotFound($"Answer with id {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting answer by id {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("question/{questionId:int}")]
    [ProducesResponseType(typeof(IEnumerable<AnswerDto>), 200)]
    public async Task<IActionResult> GetAnswersByQuestionId(int questionId)
    {
        try
        {
            var answers = await _answerRepository.GetByQuestionIdAsync(questionId);
            return Ok(answers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting answers for question {QuestionId}", questionId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("question/{questionId:int}/correct")]
    [ProducesResponseType(typeof(IEnumerable<AnswerDto>), 200)]
    public async Task<IActionResult> GetCorrectAnswersByQuestionId(int questionId)
    {
        try
        {
            var answers = await _answerRepository.GetCorrectAnswersByQuestionIdAsync(questionId);
            return Ok(answers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting correct answers for question {QuestionId}", questionId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreatedResult), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> CreateAnswer([FromBody] AnswerDto answerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var answerId = await _answerRepository.CreateAsync(answerDto);
            return CreatedAtAction(nameof(GetAnswerById), new { id = answerId }, new { id = answerId });
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
            _logger.LogError(ex, "Error creating answer");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> UpdateAnswer(int id, [FromBody] AnswerDto answerDto)
    {
        if (id != answerDto.Id)
            return BadRequest("ID mismatch");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _answerRepository.UpdateAsync(answerDto);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            return NotFound($"Answer with id {id} not found");
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating answer {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> DeleteAnswer(int id)
    {
        try
        {
            await _answerRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (EntityNotFoundException)
        {
            return NotFound($"Answer with id {id} not found");
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting answer {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
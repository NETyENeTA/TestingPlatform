
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
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserRepository userRepository, ILogger<UsersController> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            return StatusCode(500, "Ошибка при получении списка пользователей");
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            return Ok(user);
        }
        catch (EntityNotFoundException)
        {
            return NotFound($"User with id {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by id {Id}", id);
            return StatusCode(500, "Ошибка при получении пользователя");
        }
    }

    [HttpGet("login/{login}")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUserByLogin(string login)
    {
        try
        {
            var user = await _userRepository.GetByLoginAsync(login);
            return Ok(user);
        }
        catch (EntityNotFoundException)
        {
            return NotFound($"User with login {login} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by login {Login}", login);
            return StatusCode(500, "Ошибка при получении пользователя");
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = await _userRepository.UpdateAsync(id, userDto);
            return Ok(user);
        }
        catch (EntityNotFoundException)
        {
            return NotFound($"User with id {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {Id}", id);
            return StatusCode(500, "Ошибка при обновлении пользователя");
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var deleted = await _userRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound($"User with id {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {Id}", id);
            return StatusCode(500, "Ошибка при удалении пользователя");
        }
    }
}
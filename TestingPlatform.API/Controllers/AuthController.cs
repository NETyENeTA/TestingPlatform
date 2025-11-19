
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;

namespace TestingPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthRepository authRepository,
        IJwtService jwtService,
        IPasswordHasher passwordHasher,
        ILogger<AuthController> logger)
    {
        _authRepository = authRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // Проверяем существование пользователя
            var user = await _authRepository.GetUserByLoginAsync(loginDto.Login);
            if (user == null)
            {
                _logger.LogWarning("Login attempt for non-existent user: {Login}", loginDto.Login);
                return Unauthorized("Неверный логин или пароль");
            }

            // Проверяем пароль
            var isValid = await _authRepository.ValidateCredentialsAsync(loginDto.Login, loginDto.Password);
            if (!isValid)
            {
                _logger.LogWarning("Invalid password for user: {Login}", loginDto.Login);
                return Unauthorized("Неверный логин или пароль");
            }

            // Генерируем токен
            var token = _jwtService.GenerateToken(user);
            var expires = DateTime.UtcNow.AddMinutes(60);

            _logger.LogInformation("User {Login} successfully logged in", loginDto.Login);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Expires = expires,
                User = user
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {Login}", loginDto.Login);
            return StatusCode(500, "Ошибка при входе в систему");
        }
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // Создаем пользователя
            var user = await _authRepository.CreateUserAsync(registerDto);

            // Генерируем токен
            var token = _jwtService.GenerateToken(user);
            var expires = DateTime.UtcNow.AddMinutes(60);

            _logger.LogInformation("User {Login} successfully registered", registerDto.Login);

            return CreatedAtAction(nameof(Login), new AuthResponseDto
            {
                Token = token,
                Expires = expires,
                User = user
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("уже существует"))
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user: {Login}", registerDto.Login);
            return StatusCode(500, "Ошибка при регистрации");
        }
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var user = await _authRepository.GetUserByIdAsync(userId.Value);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return StatusCode(500, "Ошибка при получении информации о пользователе");
        }
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.NameIdentifier ||
            c.Type == "nameid" ||
            c.Type == "sub");

        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }
}
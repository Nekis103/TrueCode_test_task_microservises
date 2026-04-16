using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService.API.DTOs;
using UserService.Application.Commands;
using UserService.Application.Queries;

namespace UserService.API.Controllers
{
    /// <summary>
    /// Авторизация.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Медиатор.
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Констурктор авторизации.
        /// </summary>
        /// <param name="mediator"></param>
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Регистрация пользователя.
        /// </summary>
        /// <param name="request"> Имя пользователя и пароль.</param>
        /// <returns> Пользователь.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        { 
            var userId = await _mediator
                .Send(new RegisterUserCommand(request.Username, request.Password));

            return Ok(new UserResponse { UserId = userId, Username = request.Username });
        }

        /// <summary>
        /// Авторизация.
        /// </summary>
        /// <param name="request"> Запрос имени пользователя и пароля.</param>
        /// <returns> Токен авторизации.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _mediator
                .Send(new LoginUserCommand(request.Username, request.Password));

            return Ok(new LoginResponse { Token = token });
        }

        /// <summary>
        /// Вывод информации о авторизованном пользователе.
        /// </summary>
        /// <returns> Пользователь.</returns>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _mediator.Send(new GetUserByIdQuery(Guid.Parse(userId)));

            if (user == null)
            {
                return NotFound();
            } 

            return Ok(new UserResponse { UserId = user.Id, Username = user.Name });
        }
    }
}

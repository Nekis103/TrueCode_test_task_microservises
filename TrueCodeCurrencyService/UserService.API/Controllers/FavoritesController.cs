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
    /// Контроллер для работы с избранными валютами пользователя.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FavoritesController> _logger;

        /// <summary>
        /// Конструктор контроллера избранных валют.
        /// </summary>
        /// <param name="mediator"> Медиатор для CQRS.</param>
        /// <param name="logger"> Логгер.</param>
        public FavoritesController(IMediator mediator, ILogger<FavoritesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Получить идентификатор текущего пользователя из JWT токена.
        /// </summary>
        /// <returns> Идентификатор пользователя.</returns>
        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            return Guid.Parse(userIdClaim);
        }

        /// <summary>
        /// Получить список избранных валют текущего пользователя.
        /// </summary>
        /// <returns> Список валют.</returns>
        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = GetUserId();
            var favorites = await _mediator.Send(new GetUserFavoritesQuery(userId));

            var response = favorites.Select(c => new FavoriteCurrencyResponse
            {
                Code = c.Code,
                Name = c.Name
            });

            return Ok(response);
        }

        /// <summary>
        /// Добавить валюту в избранное текущего пользователя.
        /// </summary>
        /// <param name="currencyCode"> Код валюты (USD, EUR).</param>
        /// <returns> Результат операции.</returns>
        [HttpPost("{currencyCode}")]
        public async Task<IActionResult> AddFavorite(string currencyCode)
        {
            var userId = GetUserId();
            var result = await _mediator.Send(new AddFavoriteCurrencyCommand(userId, currencyCode));
            
            if (result)
            {
                return Ok(new { message = "Валюта добавлена в избранное." });
            }
            else
            {
                return Ok(new { message = "Валюта уже в избранном." });
            }
        }

        /// <summary>
        /// Удалить валюту из избранного текущего пользователя.
        /// </summary>
        /// <param name="currencyCode"> Код валюты (USD, EUR).</param>
        /// <returns> Результат операции.</returns>
        [HttpDelete("{currencyCode}")]
        public async Task<IActionResult> RemoveFavorite(string currencyCode)
        {
            var userId = GetUserId();
            var result = await _mediator.Send(new RemoveFavoriteCurrencyCommand(userId, currencyCode));

            if (result)
            {
                return Ok(new { message = "Валюта была удалена из избранного." });
            }
            else
            {
                return Ok(new { message = "Такой валюты не было в избранном пользователя." });
            }
            
        }

        /// <summary>
        /// Получить список избранных валют пользователя (внутренний эндпоинт для FinanceService).
        /// </summary>
        /// <param name="userId"> Идентификатор пользователя.</param>
        /// <returns> Список кодов валют.</returns>
        [HttpGet("internal/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserFavoritesInternal(Guid userId)
        {
            _logger.LogInformation("Внутренний запрос: получение избранных валют для пользователя {UserId}", userId);

            var favorites = await _mediator.Send(new GetUserFavoritesQuery(userId));

            if (favorites == null)
            {
                _logger.LogWarning("Пользователь {UserId} не найден", userId);
                return NotFound(new { message = $"Пользователь с идентификатором {userId} не найден" });
            }

            var currencyCodes = favorites.Select(c => c.Code).ToList();
            _logger.LogInformation("Пользователь {UserId} имеет {Count} избранных валют", userId, currencyCodes.Count);

            return Ok(currencyCodes);
        }
    }
}
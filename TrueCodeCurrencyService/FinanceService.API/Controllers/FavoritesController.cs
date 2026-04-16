using FinanceService.API.DTOs;
using FinanceService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceService.API.Controllers
{
    /// <summary>
    /// Контроллер для работы с валютами избранными пользователем.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        /// <summary>
        /// Медиатор.
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Конструктор для работы с валютами избранными пользователем.
        /// </summary>
        /// <param name="mediator"> Медиатор.</param>
        public FavoritesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить избранные пользователем валюты.
        /// </summary>
        /// <param name="userId"> Идентификатор пользователя.</param>
        /// <returns> Список валют.</returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserFavoriteRates(Guid userId)
        {
            var currencies = await _mediator.Send(new GetUserFavoriteRatesQuery(userId));

            if (currencies == null || !currencies.Any())
            {
                throw new KeyNotFoundException($"У пользователя {userId} нет избранных валют");
            }

            var response = currencies.Select(c => new CurrencyResponse
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                Rate = c.Rate,
                LastUpdated = c.LastUpdated
            });

            return Ok(response);
        }
    }
}

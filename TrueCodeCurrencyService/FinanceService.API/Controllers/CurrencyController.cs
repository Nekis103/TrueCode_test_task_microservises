using FinanceService.API.DTOs;
using FinanceService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceService.API.Controllers
{
    /// <summary>
    /// Контроллер для работы с валютами.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CurrencyController> _logger;

        /// <summary>
        /// Конструктор создания контроллера валют.
        /// </summary>
        /// <param name="mediator"> Медиатор для CQRS.</param>
        /// <param name="logger"> Логгер.</param>
        public CurrencyController(IMediator mediator, ILogger<CurrencyController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Получить все валюты.
        /// </summary>
        /// <returns> Список валют.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Запрос на получение всех валют");

            var currencies = await _mediator.Send(new GetAllCurrenciesQuery());

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

        /// <summary>
        /// Получить валюту по коду.
        /// </summary>
        /// <param name="code"> Код валюты.</param>
        /// <returns> Валюта.</returns>
        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            _logger.LogInformation("Запрос на получение валюты по коду: {Code}", code);

            var currency = await _mediator.Send(new GetCurrencyByCodeQuery(code));

            if (currency == null)
            {
                _logger.LogWarning("Валюта с кодом {Code} не найдена", code);
                return NotFound(new { message = $"Валюта с кодом {code} не найдена" });
            }

            var response = new CurrencyResponse
            {
                Id = currency.Id,
                Code = currency.Code,
                Name = currency.Name,
                Rate = currency.Rate,
                LastUpdated = currency.LastUpdated
            };

            return Ok(response);
        }
    }
}
using FinanceService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Entities;

namespace FinanceService.Application.Queries
{
    /// <summary>
    /// Запрос на получение валюты по коду.
    /// </summary>
    /// <param name="Code"> Код валюты (USD, EUR, CNY).</param>
    public record GetCurrencyByCodeQuery(string Code) : IRequest<Currency?>;

    /// <summary>
    /// Обработчик запроса для получения валюты по коду.
    /// </summary>
    public class GetCurrencyByCodeQueryHandler : IRequestHandler<GetCurrencyByCodeQuery, Currency?>
    {
        /// <summary>
        /// Репозиторий для работы с валютами.
        /// </summary>
        private readonly ICurrencyRepository _currencyRepository;

        /// <summary>
        /// Логгер.
        /// </summary>
        private readonly ILogger<GetCurrencyByCodeQueryHandler> _logger;

        /// <summary>
        /// Конструктор обработчика получения валюты по коду.
        /// </summary>
        /// <param name="currencyRepository"> Репозиторий для работы с валютами.</param>
        /// <param name="logger"> Логгер.</param>
        public GetCurrencyByCodeQueryHandler(
            ICurrencyRepository currencyRepository,
            ILogger<GetCurrencyByCodeQueryHandler> logger)
        {
            _currencyRepository = currencyRepository;
            _logger = logger;
        }

        /// <summary>
        /// Обработчик.
        /// </summary>
        /// <param name="request"> Запрос.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Валюта с указанным кодом, или null если не найдена.</returns>
        public async Task<Currency?> Handle(GetCurrencyByCodeQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Получение валюты по коду: {Code}", request.Code);
            return await _currencyRepository.GetByCodeAsync(request.Code, cancellationToken);
        }
    }
}
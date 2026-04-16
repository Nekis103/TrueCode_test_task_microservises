using FinanceService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Entities;

namespace FinanceService.Application.Queries
{
    /// <summary>
    /// Запрос на получение всех валют.
    /// </summary>
    public record GetAllCurrenciesQuery : IRequest<IEnumerable<Currency>>;

    /// <summary>
    /// Обработчик запроса для получения всех валют.
    /// </summary>
    public class GetAllCurrenciesQueryHandler : IRequestHandler<GetAllCurrenciesQuery, IEnumerable<Currency>>
    {
        /// <summary>
        ///  Репозиторий для работы с валютами.
        /// </summary>
        private readonly ICurrencyRepository _currencyRepository;

        /// <summary>
        /// Логгер.
        /// </summary>
        private readonly ILogger<GetAllCurrenciesQueryHandler> _logger;

        /// <summary>
        /// Конструктор обработчика получения всех валют.
        /// </summary>
        /// <param name="currencyRepository"> Репозиторий для работы с валютами.</param>
        /// <param name="logger"> Логгер.</param>
        public GetAllCurrenciesQueryHandler(
            ICurrencyRepository currencyRepository,
            ILogger<GetAllCurrenciesQueryHandler> logger)
        {
            _currencyRepository = currencyRepository;
            _logger = logger;
        }

        /// <summary>
        /// Обработчик.
        /// </summary>
        /// <param name="request"> Запрос.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Список всех валют.</returns>
        public async Task<IEnumerable<Currency>> Handle(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Получение списка всех валют");
            return await _currencyRepository.GetAllAsync(cancellationToken);
        }
    }
}
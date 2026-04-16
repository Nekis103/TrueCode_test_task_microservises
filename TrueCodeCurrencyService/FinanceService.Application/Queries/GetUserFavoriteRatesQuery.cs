using System.Net.Http.Json;
using FinanceService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Entities;


namespace FinanceService.Application.Queries
{
    /// <summary>
    /// Запрос на получение курсов избранных пользователем валют.
    /// </summary>
    /// <param name="UserId"> Идентификатор пользователя.</param>
    public record GetUserFavoriteRatesQuery(Guid UserId) : IRequest<IEnumerable<Currency>>;

    /// <summary>
    /// Обработчик запроса для получения курсов валют, добавленных пользователем в избранное.
    /// </summary>
    public class GetUserFavoriteRatesQueryHandler : IRequestHandler<GetUserFavoriteRatesQuery, IEnumerable<Currency>>
    {
        /// <summary>
        /// Фабрика Http клиентов.
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Репозиторий для работы с валютами.
        /// </summary>
        private readonly ICurrencyRepository _currencyRepository;

        /// <summary>
        /// Логер.
        /// </summary>
        private readonly ILogger<GetUserFavoriteRatesQueryHandler> _logger;

        /// <summary>
        /// Конструктор обработчика получения курсов избранных валют пользователя.
        /// </summary>
        /// <param name="httpClientFactory"> Фабрика HTTP клиентов.</param>
        /// <param name="currencyRepository"> Репозиторий для работы с валютами.</param>
        /// <param name="logger"> Логгер.</param>
        public GetUserFavoriteRatesQueryHandler(
            IHttpClientFactory httpClientFactory,
            ICurrencyRepository currencyRepository,
            ILogger<GetUserFavoriteRatesQueryHandler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _currencyRepository = currencyRepository;
            _logger = logger;
        }

        /// <summary>
        /// Обработчик.
        /// </summary>
        /// <param name="request"> Запрос.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Список валют с актуальными курсами.</returns>
        /// <exception cref="InvalidOperationException"> Пользователь с таким идентификатором не найден.</exception>
        /// <exception cref="HttpRequestException"> Ошибка при вызове UserService.</exception>
        public async Task<IEnumerable<Currency>> Handle(GetUserFavoriteRatesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Получение курсов избранных валют для пользователя {UserId}", request.UserId);

            var httpClient = _httpClientFactory.CreateClient("UserService");

            var response = await httpClient.GetAsync($"/api/favorites/internal/{request.UserId}", cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Пользователь {UserId} не найден", request.UserId);
                throw new InvalidOperationException($"Пользователь с идентификатором {request.UserId} не найден.");
            }

            response.EnsureSuccessStatusCode();

            var favoriteCurrencyCodes = await response.Content.ReadFromJsonAsync<List<string>>(cancellationToken);

            if (favoriteCurrencyCodes == null || !favoriteCurrencyCodes.Any())
            {
                _logger.LogInformation("У пользователя {UserId} нет избранных валют", request.UserId);
                return new List<Currency>();
            }

            _logger.LogInformation("Пользователь {UserId} добавил в избранное {Count} валют: {Codes}",
                request.UserId, favoriteCurrencyCodes.Count, string.Join(", ", favoriteCurrencyCodes));

            var currencies = await _currencyRepository.GetByCodesAsync(favoriteCurrencyCodes, cancellationToken);

            _logger.LogInformation("Найдено {Count} валют с курсами для пользователя {UserId}",
                currencies.Count(), request.UserId);

            return currencies;
        }
    }
}
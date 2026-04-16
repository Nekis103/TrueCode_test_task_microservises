using Microsoft.Extensions.Logging;
using UserService.Application.Interfaces;

namespace UserService.Infrastructure.Services
{
    /// <summary>
    /// Реализация клиента для взаимодействия с FinanceService.
    /// </summary>
    public class FinanceServiceClient : IFinanceServiceClient
    {
        /// <summary>
        /// Http клиент.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Логгер.
        /// </summary>
        private readonly ILogger<FinanceServiceClient> _logger;

        /// <summary>
        /// Конструктор клиента FinanceService.
        /// </summary>
        /// <param name="httpClient"> HTTP клиент.</param>
        /// <param name="logger"> Логгер.</param>
        public FinanceServiceClient(HttpClient httpClient, ILogger<FinanceServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Проверить, существует ли валюта с указанным кодом.
        /// </summary>
        /// <param name="code"> Код валюты (USD, EUR).</param>
        /// <param name="cancellationToken"> Токен отмены операции.</param>
        /// <returns> True - валюта существует, False - не найдена или ошибка.</returns>
        public async Task<bool> CurrencyExistsAsync(string code, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/currency/{code}", cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Валюта {Code} найдена в FinanceService", code);
                    return true;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Валюта {Code} не найдена в FinanceService", code);
                    return false;
                }

                _logger.LogWarning("Ошибка при проверке валюты {Code}. Статус: {StatusCode}", code, response.StatusCode);
                return false;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Не удалось подключиться к FinanceService при проверке валюты {Code}", code);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке валюты {Code}", code);
                return false;
            }
        }
    }
}
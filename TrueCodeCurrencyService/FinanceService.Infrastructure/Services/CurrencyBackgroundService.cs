using FinanceService.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;

namespace FinanceService.Infrastructure.Services
{
    /// <summary>
    /// Фоновый сервис для загрузки валют из ЦБ.
    /// </summary>
    public class CurrencyBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CurrencyBackgroundService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Конструктор для регистрации кодировки.
        /// </summary>
        static CurrencyBackgroundService()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// Конструктор для создания фонового сервиса загрузки валют.
        /// </summary>
        /// <param name="serviceProvider"> Провайдер сервисов.</param>
        /// <param name="logger"> Логгер.</param>
        /// <param name="configuration"> Конфигурация.</param>
        /// <param name="httpClient"> Клиент.</param>
        public CurrencyBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<CurrencyBackgroundService> logger,
            IConfiguration configuration,
            HttpClient httpClient)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Запуск фонового сервиса.
        /// </summary>
        /// <param name="stoppingToken"> Токен остановки.</param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var intervalMinutes = _configuration.GetValue<int>("CurrencyApi:UpdateIntervalMinutes", 60);

            _logger.LogInformation("Запущен сервис обновления валют. Интервал обновления: {Interval} минут", intervalMinutes);

            await UpdateCurrenciesAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
                await UpdateCurrenciesAsync(stoppingToken);
            }
        }

        /// <summary>
        /// Обновление курсов валют.
        /// </summary>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns></returns>
        private async Task UpdateCurrenciesAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Обновление валют ЦБ...");

            var url = _configuration["CurrencyApi:Url"] ?? "http://www.cbr.ru/scripts/XML_daily.asp";

            try
            {
                var response = await _httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Ошибка получания валют. Status code: {StatusCode}", response.StatusCode);
                    return;
                }

                var byteArray = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                var xmlContent = Encoding.GetEncoding("windows-1251").GetString(byteArray);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var parser = scope.ServiceProvider.GetRequiredService<ICurrencyParser>();
                    var currencyRepository = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();

                    var currencies = await parser.ParseAsync(xmlContent, cancellationToken);
                    _logger.LogInformation("Распасили {Count} валют Из ЦБ", currencies.Count);

                    await currencyRepository.AddOrUpdateRangeAsync(currencies, cancellationToken);
                }

                _logger.LogInformation("Валюты обновлены успешно.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновления валют.");
            }
        }
    }
}
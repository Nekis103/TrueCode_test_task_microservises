// UserService.Tests/TestData/CurrencyTestData.cs
using SharedKernel.Entities;

namespace UserService.Tests.TestData
{
    /// <summary>
    /// Тестовые данные для валют.
    /// </summary>
    public static class CurrencyTestData
    {
        /// <summary>
        /// Получить идентификатор тестовой валюты USD.
        /// </summary>
        public static Guid GetUsdCurrencyId()
        {
            return Guid.Parse("11111111-1111-1111-1111-111111111111");
        }

        /// <summary>
        /// Получить идентификатор тестовой валюты EUR.
        /// </summary>
        public static Guid GetEurCurrencyId()
        {
            return Guid.Parse("22222222-2222-2222-2222-222222222222");
        }

        /// <summary>
        /// Получить тестовую валюту USD.
        /// </summary>
        public static Currency GetUsdCurrency()
        {
            return new Currency
            {
                Id = GetUsdCurrencyId(),
                Code = "USD",
                CbrId = "R01235",
                Name = "Доллар США",
                Rate = 75.5m,
                LastUpdated = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Получить тестовую валюту EUR.
        /// </summary>
        public static Currency GetEurCurrency()
        {
            return new Currency
            {
                Id = GetEurCurrencyId(),
                Code = "EUR",
                CbrId = "R01239",
                Name = "Евро",
                Rate = 85.3m,
                LastUpdated = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Получить список тестовых валют.
        /// </summary>
        public static List<Currency> GetTestCurrencies()
        {
            return new List<Currency> { GetUsdCurrency(), GetEurCurrency() };
        }
    }
}
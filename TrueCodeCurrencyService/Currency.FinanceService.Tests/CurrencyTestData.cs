using System.Net;
using System.Text;
using System.Text.Json;
using SharedKernel.Entities;

namespace FinanceService.Tests.TestData
{
    /// <summary>
    /// Тестовые данные для валют.
    /// </summary>
    public static class CurrencyTestData
    {
        /// <summary>
        /// Получить список тестовых валют.
        /// </summary>
        public static List<Currency> GetTestCurrencies()
        {
            return new List<Currency>
            {
                new Currency
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Code = "USD",
                    CbrId = "R01235",
                    Name = "Доллар США",
                    Rate = 75.5m,
                    LastUpdated = new DateTime(2024, 12, 20, 10, 0, 0, DateTimeKind.Utc)
                },
                new Currency
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Code = "EUR",
                    CbrId = "R01239",
                    Name = "Евро",
                    Rate = 85.3m,
                    LastUpdated = new DateTime(2024, 12, 20, 10, 0, 0, DateTimeKind.Utc)
                },
                new Currency
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Code = "CNY",
                    CbrId = "R01010",
                    Name = "Китайский юань",
                    Rate = 10.45m,
                    LastUpdated = new DateTime(2024, 12, 20, 10, 0, 0, DateTimeKind.Utc)
                }
            };
        }

        /// <summary>
        /// Получить список кодов валют для избранного.
        /// </summary>
        public static List<string> GetFavoriteCurrencyCodes()
        {
            return new List<string> { "USD", "EUR" };
        }

        /// <summary>
        /// Получить валюту USD.
        /// </summary>
        public static Currency GetUsdCurrency()
        {
            return GetTestCurrencies().First(x => x.Code == "USD");
        }

        /// <summary>
        /// Получить валюту EUR.
        /// </summary>
        public static Currency GetEurCurrency()
        {
            return GetTestCurrencies().First(x => x.Code == "EUR");
        }

        /// <summary>
        /// Получить XML с курсами валют.
        /// </summary>
        public static string GetTestXml()
        {
            return @"<?xml version='1.0' encoding='windows-1251'?>
                        <ValCurs Date='20.12.2024'>
                            <Valute ID='R01235'>
                                <CharCode>USD</CharCode>
                                <Name>Доллар США</Name>
                                <Value>75,50</Value>
                            </Valute>
                            <Valute ID='R01239'>
                                <CharCode>EUR</CharCode>
                                <Name>Евро</Name>
                                <Value>85,30</Value>
                            </Valute>
                            <Valute ID='R01010'>
                                <CharCode>CNY</CharCode>
                                <Name>Китайский юань</Name>
                                <Value>10,45</Value>
                            </Valute>
                        </ValCurs>";
        }

        /// <summary>
        /// Получить XML с одной валютой.
        /// </summary>
        public static string GetSingleCurrencyXml()
        {
            return @"<?xml version='1.0' encoding='windows-1251'?>
                        <ValCurs Date='20.12.2024'>
                            <Valute ID='R01235'>
                                <CharCode>USD</CharCode>
                                <Name>Доллар США</Name>
                                <Value>75,50</Value>
                            </Valute>
                        </ValCurs>";
        }

        /// <summary>
        /// Получить пустой XML.
        /// </summary>
        public static string GetEmptyXml()
        {
            return @"<?xml version='1.0' encoding='windows-1251'?>
                        <ValCurs Date='20.12.2024'>
                        </ValCurs>";
        }

        /// <summary>
        /// Получить XML с некорректным курсом.
        /// </summary>
        public static string GetInvalidRateXml()
        {
            return @"<?xml version='1.0' encoding='windows-1251'?>
                        <ValCurs Date='20.12.2024'>
                            <Valute ID='R01235'>
                                <CharCode>USD</CharCode>
                                <Name>Доллар США</Name>
                                <Value>не число</Value>
                            </Valute>
                        </ValCurs>";
        }

        /// <summary>
        /// Получить успешный HTTP ответ с курсами валют.
        /// </summary>
        public static HttpResponseMessage GetSuccessHttpResponse()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(
                JsonSerializer.Serialize(GetFavoriteCurrencyCodes()),
                Encoding.UTF8,
                "application/json");
            return response;
        }

        /// <summary>
        /// Получить HTTP ответ с ошибкой 404.
        /// </summary>
        public static HttpResponseMessage GetNotFoundHttpResponse()
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Получить HTTP ответ с ошибкой 500.
        /// </summary>
        public static HttpResponseMessage GetInternalServerErrorHttpResponse()
        {
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// Получить HTTP ответ с пустым списком избранного.
        /// </summary>
        public static HttpResponseMessage GetEmptyFavoritesHttpResponse()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(
                JsonSerializer.Serialize(new List<string>()),
                Encoding.UTF8,
                "application/json");
            return response;
        }
    }
}
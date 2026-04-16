using System.Globalization;
using System.Xml;
using SharedKernel.Entities;

namespace FinanceService.Infrastructure.Services
{
    /// <summary>
    /// Парсер валют.
    /// </summary>
    public interface ICurrencyParser
    {
        /// <summary>
        /// Парсинг валют из XML в список валют.
        /// </summary>
        /// <param name="xmlContent"> XML контент.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Список валют.</returns>
        Task<List<Currency>> ParseAsync(string xmlContent, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Парсер валют из XML ЦБ РФ.
    /// </summary>
    public class CurrencyParser : ICurrencyParser
    {
        /// <summary>
        /// Преобразование валют из XML в список валют.
        /// </summary>
        /// <param name="xmlContent"> XML контент.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Список валют.</returns>
        public Task<List<Currency>> ParseAsync(string xmlContent, CancellationToken cancellationToken = default)
        {
            var currencies = new List<Currency>();

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            var valuteNodes = xmlDoc.SelectNodes("//Valute");

            if (valuteNodes != null)
            {
                foreach (XmlNode node in valuteNodes)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var cbrId = node.Attributes?["ID"]?.Value ?? string.Empty;
                    var code = node.SelectSingleNode("CharCode")?.InnerText ?? string.Empty;
                    var name = node.SelectSingleNode("Name")?.InnerText ?? string.Empty;
                    var valueStr = node.SelectSingleNode("Value")?.InnerText ?? "0";

                    var rate = decimal.Parse(valueStr, new CultureInfo("ru-RU"));

                    var currency = new Currency
                    {
                        Id = Guid.NewGuid(),
                        CbrId = cbrId,
                        Code = code,
                        Name = name,
                        Rate = rate,
                        LastUpdated = DateTime.UtcNow
                    };

                    currencies.Add(currency);
                }
            }

            return Task.FromResult(currencies);
        }
    }
}
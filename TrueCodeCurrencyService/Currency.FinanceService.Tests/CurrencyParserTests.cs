using FinanceService.Infrastructure.Services;
using FinanceService.Tests.TestData;
using FluentAssertions;
using Xunit;

namespace FinanceService.Tests.Infrastructure.Services
{
    /// <summary>
    /// Тесты для парсера валют из XML ЦБ РФ.
    /// </summary>
    public class CurrencyParserTests
    {
        private readonly CurrencyParser _parser;

        /// <summary>
        /// Конструктор тестов парсера валют.
        /// </summary>
        public CurrencyParserTests()
        {
            _parser = new CurrencyParser();
        }

        /// <summary>
        /// Тест: успешный парсинг валидного XML с курсами валют.
        /// </summary>
        [Fact]
        public async Task ParseAsync_ValidXml_ShouldReturnCurrencies()
        {
            // Arrange
            var xml = CurrencyTestData.GetTestXml();

            // Act
            var result = await _parser.ParseAsync(xml);

            // Assert
            result.Should().HaveCount(3);

            var usd = result.First(x => x.Code == "USD");
            usd.Should().NotBeNull();
            usd.CbrId.Should().Be("R01235");
            usd.Name.Should().Be("Доллар США");
            usd.Rate.Should().Be(75.50m);
        }

        /// <summary>
        /// Тест: парсинг XML без валют возвращает пустой список.
        /// </summary>
        [Fact]
        public async Task ParseAsync_EmptyXml_ShouldReturnEmptyList()
        {
            // Arrange
            var xml = CurrencyTestData.GetEmptyXml();

            // Act
            var result = await _parser.ParseAsync(xml);

            // Assert
            result.Should().BeEmpty();
        }

        /// <summary>
        /// Тест: парсинг XML с одной валютой.
        /// </summary>
        [Fact]
        public async Task ParseAsync_SingleCurrency_ShouldReturnOneCurrency()
        {
            // Arrange
            var xml = CurrencyTestData.GetSingleCurrencyXml();

            // Act
            var result = await _parser.ParseAsync(xml);

            // Assert
            result.Should().HaveCount(1);
            result.First().Code.Should().Be("USD");
        }

        /// <summary>
        /// Тест: парсинг XML с некорректным значением курса.
        /// </summary>
        [Fact]
        public async Task ParseAsync_InvalidRateValue_ShouldThrowException()
        {
            // Arrange
            var xml = CurrencyTestData.GetInvalidRateXml();

            // Act & Assert
            await Assert.ThrowsAsync<FormatException>(async () =>
                await _parser.ParseAsync(xml));
        }

        /// <summary>
        /// Тест: отмена операции через CancellationToken.
        /// </summary>
        [Fact]
        public async Task ParseAsync_CancellationRequested_ShouldThrowOperationCanceledException()
        {
            // Arrange
            var xml = CurrencyTestData.GetTestXml();
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                await _parser.ParseAsync(xml, cts.Token));
        }
    }
}
using FinanceService.Application.Queries;
using FinanceService.Domain.Interfaces;
using FinanceService.Tests.TestData;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SharedKernel.Entities;
using Xunit;

namespace FinanceService.Tests.Application.Queries
{
    /// <summary>
    /// Тесты для запроса получения валюты по коду.
    /// </summary>
    public class GetCurrencyByCodeQueryTests
    {
        private readonly Mock<ICurrencyRepository> _currencyRepositoryMock;
        private readonly Mock<ILogger<GetCurrencyByCodeQueryHandler>> _loggerMock;
        private readonly GetCurrencyByCodeQueryHandler _handler;

        /// <summary>
        /// Конструктор тестов получения валюты по коду.
        /// </summary>
        public GetCurrencyByCodeQueryTests()
        {
            _currencyRepositoryMock = new Mock<ICurrencyRepository>();
            _loggerMock = new Mock<ILogger<GetCurrencyByCodeQueryHandler>>();
            _handler = new GetCurrencyByCodeQueryHandler(_currencyRepositoryMock.Object, _loggerMock.Object);
        }

        /// <summary>
        /// Тест: успешное получение существующей валюты по коду.
        /// </summary>
        [Fact]
        public async Task Handle_CurrencyExists_ShouldReturnCurrency()
        {
            // Arrange
            var expectedCurrency = CurrencyTestData.GetUsdCurrency();

            _currencyRepositoryMock.Setup(x => x.GetByCodeAsync("USD", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCurrency);

            var query = new GetCurrencyByCodeQuery("USD");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Code.Should().Be("USD");
            result.Name.Should().Be("Доллар США");
            result.Rate.Should().Be(75.5m);
        }

        /// <summary>
        /// Тест: попытка получения несуществующей валюты по коду.
        /// </summary>
        [Fact]
        public async Task Handle_CurrencyNotFound_ShouldReturnNull()
        {
            // Arrange
            _currencyRepositoryMock.Setup(x => x.GetByCodeAsync("XXX", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Currency?)null);

            var query = new GetCurrencyByCodeQuery("XXX");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        /// <summary>
        /// Тест: получение валюты с пустым кодом.
        /// </summary>
        [Fact]
        public async Task Handle_EmptyCode_ShouldReturnNull()
        {
            // Arrange
            _currencyRepositoryMock.Setup(x => x.GetByCodeAsync("", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Currency?)null);

            var query = new GetCurrencyByCodeQuery("");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
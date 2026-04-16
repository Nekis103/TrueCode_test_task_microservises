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
    /// Тесты для запроса получения всех валют.
    /// </summary>
    public class GetAllCurrenciesQueryTests
    {
        private readonly Mock<ICurrencyRepository> _currencyRepositoryMock;
        private readonly Mock<ILogger<GetAllCurrenciesQueryHandler>> _loggerMock;
        private readonly GetAllCurrenciesQueryHandler _handler;

        /// <summary>
        /// Конструктор тестов получения всех валют.
        /// </summary>
        public GetAllCurrenciesQueryTests()
        {
            _currencyRepositoryMock = new Mock<ICurrencyRepository>();
            _loggerMock = new Mock<ILogger<GetAllCurrenciesQueryHandler>>();
            _handler = new GetAllCurrenciesQueryHandler(_currencyRepositoryMock.Object, _loggerMock.Object);
        }

        /// <summary>
        /// Тест: успешное получение списка всех валют.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldReturnAllCurrencies()
        {
            // Arrange
            var expectedCurrencies = CurrencyTestData.GetTestCurrencies();

            _currencyRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCurrencies);

            var query = new GetAllCurrenciesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(expectedCurrencies);
        }

        /// <summary>
        /// Тест: получение пустого списка валют.
        /// </summary>
        [Fact]
        public async Task Handle_NoCurrencies_ShouldReturnEmptyList()
        {
            // Arrange
            _currencyRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Currency>());

            var query = new GetAllCurrenciesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
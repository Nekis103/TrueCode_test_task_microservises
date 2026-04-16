using FinanceService.Application.Queries;
using FinanceService.Domain.Interfaces;
using FinanceService.Tests.TestData;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using Xunit;

namespace FinanceService.Tests.Application.Queries
{
    /// <summary>
    /// Тесты для запроса получения курсов избранных валют пользователя.
    /// </summary>
    public class GetUserFavoriteRatesQueryTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<ICurrencyRepository> _currencyRepositoryMock;
        private readonly Mock<ILogger<GetUserFavoriteRatesQueryHandler>> _loggerMock;
        private readonly GetUserFavoriteRatesQueryHandler _handler;

        /// <summary>
        /// Конструктор тестов получения избранных валют пользователя.
        /// </summary>
        public GetUserFavoriteRatesQueryTests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _currencyRepositoryMock = new Mock<ICurrencyRepository>();
            _loggerMock = new Mock<ILogger<GetUserFavoriteRatesQueryHandler>>();
            _handler = new GetUserFavoriteRatesQueryHandler(
                _httpClientFactoryMock.Object,
                _currencyRepositoryMock.Object,
                _loggerMock.Object);
        }

        /// <summary>
        /// Тест: успешное получение курсов избранных валют пользователя.
        /// </summary>
        [Fact]
        public async Task Handle_UserHasFavorites_ShouldReturnCurrencies()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetUserFavoriteRatesQuery(userId);
            var expectedCodes = CurrencyTestData.GetFavoriteCurrencyCodes();
            var expectedCurrencies = CurrencyTestData.GetTestCurrencies().Take(2).ToList();

            var httpMessageHandler = new Mock<HttpMessageHandler>();
            var response = CurrencyTestData.GetSuccessHttpResponse();

            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var httpClient = new HttpClient(httpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactoryMock.Setup(x => x.CreateClient("UserService"))
                .Returns(httpClient);

            _currencyRepositoryMock.Setup(x => x.GetByCodesAsync(expectedCodes, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCurrencies);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expectedCurrencies);
        }

        /// <summary>
        /// Тест: пользователь не найден, выбрасывается исключение.
        /// </summary>
        [Fact]
        public async Task Handle_UserNotFound_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetUserFavoriteRatesQuery(userId);

            var httpMessageHandler = new Mock<HttpMessageHandler>();
            var response = CurrencyTestData.GetNotFoundHttpResponse();

            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var httpClient = new HttpClient(httpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactoryMock.Setup(x => x.CreateClient("UserService"))
                .Returns(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        /// <summary>
        /// Тест: у пользователя нет избранных валют, возвращается пустой список.
        /// </summary>
        [Fact]
        public async Task Handle_UserHasNoFavorites_ShouldReturnEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetUserFavoriteRatesQuery(userId);

            var httpMessageHandler = new Mock<HttpMessageHandler>();
            var response = CurrencyTestData.GetEmptyFavoritesHttpResponse();

            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var httpClient = new HttpClient(httpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactoryMock.Setup(x => x.CreateClient("UserService"))
                .Returns(httpClient);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        /// <summary>
        /// Тест: ошибка HTTP при вызове UserService.
        /// </summary>
        [Fact]
        public async Task Handle_HttpError_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetUserFavoriteRatesQuery(userId);

            var httpMessageHandler = new Mock<HttpMessageHandler>();
            var response = CurrencyTestData.GetInternalServerErrorHttpResponse();

            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var httpClient = new HttpClient(httpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _httpClientFactoryMock.Setup(x => x.CreateClient("UserService"))
                .Returns(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }
    }
}
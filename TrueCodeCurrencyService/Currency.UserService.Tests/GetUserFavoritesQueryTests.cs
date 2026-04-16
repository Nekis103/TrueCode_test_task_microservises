using FluentAssertions;
using Moq;
using SharedKernel.Entities;
using UserService.Application.Queries;
using UserService.Domain.Interfaces;
using UserService.Tests.TestData;
using Xunit;

namespace UserService.Tests.Application.Queries
{
    /// <summary>
    /// Тесты для запроса получения избранных валют пользователя.
    /// </summary>
    public class GetUserFavoritesQueryTests
    {
        private readonly Mock<IUserFavoriteCurrencyRepository> _favoriteRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetUserFavoritesQueryHandler _handler;

        /// <summary>
        /// Конструктор тестов получения избранных валют.
        /// </summary>
        public GetUserFavoritesQueryTests()
        {
            _favoriteRepositoryMock = new Mock<IUserFavoriteCurrencyRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetUserFavoritesQueryHandler(
                _favoriteRepositoryMock.Object,
                _userRepositoryMock.Object);
        }

        /// <summary>
        /// Тест: успешное получение избранных валют существующего пользователя.
        /// </summary>
        [Fact]
        public async Task Handle_UserExists_ShouldReturnFavorites()
        {
            // Arrange
            var userId = UserTestData.GetTestUserId();
            var query = new GetUserFavoritesQuery(userId);
            var expectedCurrencies = UserTestData.GetTestFavoriteCurrencies();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(UserTestData.GetTestUser());

            _favoriteRepositoryMock.Setup(x => x.GetUserFavoritesAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCurrencies);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expectedCurrencies);
        }

        /// <summary>
        /// Тест: попытка получения избранного несуществующего пользователя.
        /// </summary>
        [Fact]
        public async Task Handle_UserNotFound_ShouldThrowException()
        {
            // Arrange
            var query = new GetUserFavoritesQuery(Guid.NewGuid());

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        /// <summary>
        /// Тест: получение пустого списка избранных валют у пользователя.
        /// </summary>
        [Fact]
        public async Task Handle_UserHasNoFavorites_ShouldReturnEmptyList()
        {
            // Arrange
            var userId = UserTestData.GetTestUserId();
            var query = new GetUserFavoritesQuery(userId);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(UserTestData.GetTestUser());

            _favoriteRepositoryMock.Setup(x => x.GetUserFavoritesAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Currency>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
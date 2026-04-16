using FluentAssertions;
using Moq;
using UserService.Application.Commands;
using UserService.Domain.Interfaces;
using UserService.Tests.TestData;
using Xunit;

namespace UserService.Tests.Application.Commands
{
    /// <summary>
    /// Тесты для команды удаления валюты из избранного пользователя.
    /// </summary>
    public class RemoveFavoriteCurrencyCommandTests
    {
        private readonly Mock<IUserFavoriteCurrencyRepository> _favoriteRepositoryMock;
        private readonly RemoveFavoriteCurrencyCommandHandler _handler;

        /// <summary>
        /// Конструктор тестов удаления валюты из избранного.
        /// </summary>
        public RemoveFavoriteCurrencyCommandTests()
        {
            _favoriteRepositoryMock = new Mock<IUserFavoriteCurrencyRepository>();
            _handler = new RemoveFavoriteCurrencyCommandHandler(_favoriteRepositoryMock.Object);
        }

        /// <summary>
        /// Тест: успешное удаление валюты из избранного.
        /// </summary>
        [Fact]
        public async Task Handle_ValidRequest_ShouldRemoveFromFavorites()
        {
            // Arrange
            var command = UserTestData.GetRemoveFavoriteCommand();

            _favoriteRepositoryMock.Setup(x => x.IsFavoriteAsync(command.UserId, command.CurrencyCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _favoriteRepositoryMock.Verify(x => x.RemoveFavoriteAsync(command.UserId, command.CurrencyCode, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Тест: попытка удаления валюты, которой нет в избранном.
        /// </summary>
        [Fact]
        public async Task Handle_CurrencyNotInFavorites_ShouldReturnFalse()
        {
            // Arrange
            var command = UserTestData.GetRemoveFavoriteCommand();

            _favoriteRepositoryMock.Setup(x => x.IsFavoriteAsync(command.UserId, command.CurrencyCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _favoriteRepositoryMock.Verify(x => x.RemoveFavoriteAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
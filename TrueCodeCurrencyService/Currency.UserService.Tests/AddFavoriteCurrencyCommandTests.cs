using FluentAssertions;
using Moq;
using SharedKernel.Entities;
using UserService.Application.Commands;
using UserService.Application.Interfaces;
using UserService.Domain.Interfaces;
using UserService.Tests.TestData;
using Xunit;

namespace UserService.Tests.Application.Commands
{
    /// <summary>
    /// Тесты для команды добавления валюты в избранное пользователя.
    /// </summary>
    public class AddFavoriteCurrencyCommandTests
    {
        private readonly Mock<IUserFavoriteCurrencyRepository> _favoriteRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IFinanceServiceClient> _financeServiceClientMock;
        private readonly AddFavoriteCurrencyCommandHandler _handler;

        /// <summary>
        /// Конструктор тестов добавления валюты в избранное.
        /// </summary>
        public AddFavoriteCurrencyCommandTests()
        {
            _favoriteRepositoryMock = new Mock<IUserFavoriteCurrencyRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _financeServiceClientMock = new Mock<IFinanceServiceClient>();
            _handler = new AddFavoriteCurrencyCommandHandler(
                _favoriteRepositoryMock.Object,
                _userRepositoryMock.Object,
                _financeServiceClientMock.Object);
        }

        /// <summary>
        /// Тест: успешное добавление валюты в избранное.
        /// </summary>
        [Fact]
        public async Task Handle_ValidRequest_ShouldAddToFavorites()
        {
            // Arrange
            var command = UserTestData.GetAddFavoriteCommand();
            var user = UserTestData.GetTestUser();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _financeServiceClientMock.Setup(x => x.CurrencyExistsAsync(command.CurrencyCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _favoriteRepositoryMock.Setup(x => x.IsFavoriteAsync(command.UserId, command.CurrencyCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _favoriteRepositoryMock.Verify(x => x.AddFavoriteAsync(command.UserId, command.CurrencyCode, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Тест: попытка добавления валюты, которая уже есть в избранном.
        /// </summary>
        [Fact]
        public async Task Handle_CurrencyAlreadyInFavorites_ShouldReturnFalse()
        {
            // Arrange
            var command = UserTestData.GetAddFavoriteCommand();
            var user = UserTestData.GetTestUser();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _financeServiceClientMock.Setup(x => x.CurrencyExistsAsync(command.CurrencyCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _favoriteRepositoryMock.Setup(x => x.IsFavoriteAsync(command.UserId, command.CurrencyCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _favoriteRepositoryMock.Verify(x => x.AddFavoriteAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// Тест: попытка добавления валюты несуществующим пользователем.
        /// </summary>
        [Fact]
        public async Task Handle_UserNotFound_ShouldThrowException()
        {
            // Arrange
            var command = UserTestData.GetAddFavoriteCommand();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        /// <summary>
        /// Тест: попытка добавления несуществующей валюты в избранное.
        /// </summary>
        [Fact]
        public async Task Handle_CurrencyNotFound_ShouldThrowException()
        {
            // Arrange
            var command = UserTestData.GetAddFavoriteCommand();
            var user = UserTestData.GetTestUser();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _financeServiceClientMock.Setup(x => x.CurrencyExistsAsync(command.CurrencyCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
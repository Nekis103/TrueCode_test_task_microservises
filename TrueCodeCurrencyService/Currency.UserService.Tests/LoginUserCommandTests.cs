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
    /// Тесты для команды авторизации пользователя.
    /// </summary>
    public class LoginUserCommandTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly LoginUserCommandHandler _handler;

        /// <summary>
        /// Конструктор тестов авторизации пользователя.
        /// </summary>
        public LoginUserCommandTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtServiceMock = new Mock<IJwtService>();
            _handler = new LoginUserCommandHandler(_userRepositoryMock.Object, _jwtServiceMock.Object);
        }

        /// <summary>
        /// Тест: успешная авторизация с правильными учетными данными.
        /// </summary>
        [Fact]
        public async Task Handle_ValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var command = UserTestData.GetLoginCommand();
            var user = UserTestData.GetTestUser();

            _userRepositoryMock.Setup(x => x.GetByUsernameAsync(command.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _jwtServiceMock.Setup(x => x.GenerateToken(user.Id, user.Name))
                .Returns(UserTestData.GetJwtToken());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Be(UserTestData.GetJwtToken());
        }

        /// <summary>
        /// Тест: попытка входа с несуществующим пользователем.
        /// </summary>
        [Fact]
        public async Task Handle_UserNotFound_ShouldThrowUnauthorizedException()
        {
            // Arrange
            var command = UserTestData.GetLoginCommandWithUnknownUser();

            _userRepositoryMock.Setup(x => x.GetByUsernameAsync(command.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        /// <summary>
        /// Тест: попытка входа с неверным паролем.
        /// </summary>
        [Fact]
        public async Task Handle_InvalidPassword_ShouldThrowUnauthorizedException()
        {
            // Arrange
            var command = UserTestData.GetLoginCommandWithWrongPassword();
            var user = UserTestData.GetTestUser();

            _userRepositoryMock.Setup(x => x.GetByUsernameAsync(command.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
using FluentAssertions;
using Moq;
using SharedKernel.Entities;
using UserService.Application.Commands;
using UserService.Domain.Interfaces;
using UserService.Tests.TestData;
using Xunit;

namespace UserService.Tests.Application.Commands
{
    /// <summary>
    /// Тесты для команды регистрации нового пользователя.
    /// </summary>
    public class RegisterUserCommandTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly RegisterUserCommandHandler _handler;

        /// <summary>
        /// Конструктор тестов регистрации пользователя.
        /// </summary>
        public RegisterUserCommandTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new RegisterUserCommandHandler(_userRepositoryMock.Object);
        }

        /// <summary>
        /// Тест: успешная регистрация нового пользователя.
        /// </summary>
        [Fact]
        public async Task Handle_ValidUser_ShouldRegisterSuccessfully()
        {
            // Arrange
            var command = UserTestData.GetRegisterCommand();

            _userRepositoryMock.Setup(x => x.GetByUsernameAsync(command.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty();
            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Тест: попытка регистрации с уже существующим именем пользователя.
        /// </summary>
        [Fact]
        public async Task Handle_UserAlreadyExists_ShouldThrowException()
        {
            // Arrange
            var command = UserTestData.GetRegisterCommandWithExistingName();
            var existingUser = UserTestData.GetExistingUser();

            _userRepositoryMock.Setup(x => x.GetByUsernameAsync(command.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Contain("уже существует");
        }
    }
}
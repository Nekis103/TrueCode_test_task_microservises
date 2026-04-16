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
    /// Тесты для запроса получения пользователя по идентификатору.
    /// </summary>
    public class GetUserByIdQueryTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetUserByIdQueryHandler _handler;

        /// <summary>
        /// Конструктор тестов получения пользователя по идентификатору.
        /// </summary>
        public GetUserByIdQueryTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetUserByIdQueryHandler(_userRepositoryMock.Object);
        }

        /// <summary>
        /// Тест: успешное получение существующего пользователя по идентификатору.
        /// </summary>
        [Fact]
        public async Task Handle_UserExists_ShouldReturnUser()
        {
            // Arrange
            var expectedUser = UserTestData.GetTestUser();
            var query = new GetUserByIdQuery(expectedUser.Id);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(expectedUser.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(expectedUser.Id);
            result.Name.Should().Be(expectedUser.Name);
        }

        /// <summary>
        /// Тест: попытка получения несуществующего пользователя.
        /// </summary>
        [Fact]
        public async Task Handle_UserNotFound_ShouldReturnNull()
        {
            // Arrange
            var query = new GetUserByIdQuery(Guid.NewGuid());

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
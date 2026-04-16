using MediatR;
using SharedKernel.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Commands
{
    /// <summary>
    /// Команда на регистрацию пользователя.
    /// </summary>
    /// <param name="Username"> Имя пользователя.</param>
    /// <param name="Password"> Пароль.</param>
    public record RegisterUserCommand(string Username, string Password) : IRequest<Guid>;

    /// <summary>
    /// Регистрация нового пользователя.
    /// </summary>
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        /// <summary>
        /// Репозиторий пользователя.
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Конструктор для регистрации пользователя.
        /// </summary>
        /// <param name="userRepository"> Репозиторий пользователя.</param>
        public RegisterUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Обработчик.
        /// </summary>
        /// <param name="request"> Запрос.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Идентификатор зарегистрированного пользователя.</returns>
        /// <exception cref="InvalidOperationException"> Пользователь с таким именем уже существует.</exception>
        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);

            if (existingUser != null)
            {
                throw new InvalidOperationException("Пользователь с таким именем уже существует.");
            }    

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await _userRepository.AddAsync(user, cancellationToken);
            return user.Id;
        }
    }
}
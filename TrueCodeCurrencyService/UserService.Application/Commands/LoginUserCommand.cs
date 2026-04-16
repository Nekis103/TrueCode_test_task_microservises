using MediatR;
using UserService.Domain.Interfaces;
using UserService.Application.Interfaces;

namespace UserService.Application.Commands
{
    /// <summary>
    /// Команда на авторизацию пользователя.
    /// </summary>
    /// <param name="Username"> Имя пользователя.</param>
    /// <param name="Password"> Пароль.</param>
    public record LoginUserCommand(string Username, string Password) : IRequest<string>;

    /// <summary>
    /// Авторизация пользователя через JWT токен.
    /// </summary>
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string>
    {
        /// <summary>
        /// Репозиторий пользователя.
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// JWT сервис для авторизации.
        /// </summary>
        private readonly IJwtService _jwtService;

        /// <summary>
        /// Констрктор авторизации пользователя.
        /// </summary>
        /// <param name="userRepository"> Репозиторий пользователя.</param>
        /// <param name="jwtService"> Сервис авторизации.</param>
        public LoginUserCommandHandler(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Обработчик.
        /// </summary>
        /// <param name="request"> Запрос.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> JWT токен.</returns>
        /// <exception cref="UnauthorizedAccessException"> Ошибка авторизации.</exception>
        public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
                
            return _jwtService.GenerateToken(user.Id, user.Name);
        }
    }
}
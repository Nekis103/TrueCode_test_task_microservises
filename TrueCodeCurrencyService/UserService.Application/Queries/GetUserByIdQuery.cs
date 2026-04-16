using MediatR;
using SharedKernel.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Queries
{
    /// <summary>
    /// Запрос на получение пользователя по идентификатору.
    /// </summary>
    /// <param name="UserId"> Идентификатор пользователя.</param>
    public record GetUserByIdQuery(Guid UserId) : IRequest<User?>;

    /// <summary>
    /// Обработчик запроса, для получения пользователя по идентфикатору.
    /// </summary>
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, User?>
    {
        /// <summary>
        /// Репозиторий для работы с пользователями.
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Конструктор обработчка запроса, для получения пользователя по идентфикатору.
        /// </summary>
        /// <param name="userRepository"> Репозиторй для работы с пользователями.</param>
        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Обработчик.
        /// </summary>
        /// <param name="request"> Запрос.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Пользователь.</returns>
        public async Task<User?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            return await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        }
    }
}

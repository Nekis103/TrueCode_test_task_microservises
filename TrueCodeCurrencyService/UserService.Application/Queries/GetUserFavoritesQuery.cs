using MediatR;
using SharedKernel.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Queries
{
    /// <summary>
    /// Запрос на получение списка избранных пользователем валют.
    /// </summary>
    /// <param name="UserId"> Идентификатор пользователя.</param>
    public record GetUserFavoritesQuery(Guid UserId) : IRequest<IEnumerable<Currency>>;

    /// <summary>
    /// Обработчик запроса, для получения списка валют избранных пользователем.
    /// </summary>
    public class GetUserFavoritesQueryHandler : IRequestHandler<GetUserFavoritesQuery, IEnumerable<Currency>>
    {
        /// <summary>
        /// Репозиторий для работы с валютами избранными пользователем.
        /// </summary>
        private readonly IUserFavoriteCurrencyRepository _favoriteRepository;

        /// <summary>
        /// Репозиторий для работы с пользователями.
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Конструктор запроса избранных пользователем валют.
        /// </summary>
        /// <param name="favoriteRepository"> Репозиторий для работы с валютами избранными пользователем.</param>
        /// <param name="userRepository"> Репозиторий для работы с пользователями.</param>
        public GetUserFavoritesQueryHandler(
            IUserFavoriteCurrencyRepository favoriteRepository,
            IUserRepository userRepository)
        {
            _favoriteRepository = favoriteRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Обработчик.
        /// </summary>
        /// <param name="request"> Запрос.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Список валют, добавленных пользователем в избранное.</returns>
        /// <exception cref="InvalidOperationException"> Пользователь не найден.</exception>
        public async Task<IEnumerable<Currency>> Handle(GetUserFavoritesQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
            {
                throw new InvalidOperationException("Пользователь не найден.");
            }
                
            return await _favoriteRepository.GetUserFavoritesAsync(request.UserId, cancellationToken);
        }
    }
}

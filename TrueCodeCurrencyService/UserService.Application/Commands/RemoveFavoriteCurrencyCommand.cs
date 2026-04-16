using MediatR;
using UserService.Domain.Interfaces;

namespace UserService.Application.Commands
{
    /// <summary>
    /// Команда на удаление валюты из избранного пользователя.
    /// </summary>
    /// <param name="UserId"> Идентификатор пользователя.</param>
    /// <param name="CurrencyCode"> Код валюты (USD, EUR).</param>
    public record RemoveFavoriteCurrencyCommand(Guid UserId, string CurrencyCode) : IRequest<bool>;

    /// <summary>
    /// Обработчик команды удаления валюты из избранного пользователя.
    /// </summary>
    public class RemoveFavoriteCurrencyCommandHandler : IRequestHandler<RemoveFavoriteCurrencyCommand, bool>
    {
        private readonly IUserFavoriteCurrencyRepository _favoriteRepository;

        /// <summary>
        /// Конструктор обработчика удаления валюты из избранного.
        /// </summary>
        /// <param name="favoriteRepository"> Репозиторий для работы с избранным пользователя.</param>
        public RemoveFavoriteCurrencyCommandHandler(IUserFavoriteCurrencyRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        /// <summary>
        /// Обработчик команды удаления валюты из избранного.
        /// </summary>
        /// <param name="request"> Команда с идентификатором пользователя и кодом валюты.</param>
        /// <param name="cancellationToken"> Токен отмены операции.</param>
        /// <returns> True - валюта успешно удалена, False - валюты не было в избранном.</returns>
        public async Task<bool> Handle(RemoveFavoriteCurrencyCommand request, CancellationToken cancellationToken)
        {
            var isFavorite = await _favoriteRepository.IsFavoriteAsync(request.UserId, request.CurrencyCode, cancellationToken);
            
            if (!isFavorite)
            {
                return false;
            }

            await _favoriteRepository.RemoveFavoriteAsync(request.UserId, request.CurrencyCode, cancellationToken);
            return true;
        }
    }
}
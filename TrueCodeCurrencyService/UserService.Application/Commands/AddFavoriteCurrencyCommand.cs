using MediatR;
using UserService.Domain.Interfaces;
using UserService.Application.Interfaces;

namespace UserService.Application.Commands
{
    /// <summary>
    /// Команда на добавление валюты в избранное пользователя.
    /// </summary>
    /// <param name="UserId"> Идентификатор пользователя.</param>
    /// <param name="CurrencyCode"> Код валюты (USD, EUR).</param>
    public record AddFavoriteCurrencyCommand(Guid UserId, string CurrencyCode) : IRequest<bool>;

    /// <summary>
    /// Обработчик команды добавления валюты в избранное пользователя.
    /// </summary>
    public class AddFavoriteCurrencyCommandHandler : IRequestHandler<AddFavoriteCurrencyCommand, bool>
    {
        /// <summary>
        /// Репозиторий для работы с избранными валютами пользователя.
        /// </summary>
        private readonly IUserFavoriteCurrencyRepository _favoriteRepository;

        /// <summary>
        /// Реползиторий для работы с пользователями.
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Сервис для работы с финансами.
        /// </summary>
        private readonly IFinanceServiceClient _financeServiceClient;

        /// <summary>
        /// Конструктор обработчика команды добавления валюты в избранное.
        /// </summary>
        /// <param name="favoriteRepository"> Репозиторий для работы с избранными валютами.</param>
        /// <param name="userRepository"> Репозиторий для работы с пользователями.</param>
        /// <param name="financeServiceClient"> Клиент для проверки существования валюты в FinanceService.</param>
        public AddFavoriteCurrencyCommandHandler(
            IUserFavoriteCurrencyRepository favoriteRepository,
            IUserRepository userRepository,
            IFinanceServiceClient financeServiceClient)
        {
            _favoriteRepository = favoriteRepository;
            _userRepository = userRepository;
            _financeServiceClient = financeServiceClient;
        }

        /// <summary>
        /// Обрабатывает команду добавления валюты в избранное пользователя.
        /// </summary>
        /// <param name="request"> Команда с идентификатором пользователя и кодом валюты.</param>
        /// <param name="cancellationToken"> Токен отмены операции.</param>
        /// <returns> True - валюта успешно добавлена, False - валюта уже была в избранном.</returns>
        /// <exception cref="InvalidOperationException"> Выбрасывается, если пользователь не найден или валюта не существует.</exception>
        public async Task<bool> Handle(AddFavoriteCurrencyCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            
            if (user == null)
            {
                throw new InvalidOperationException("Пользователь не найден.");
            }

            var currencyExists = await _financeServiceClient.CurrencyExistsAsync(request.CurrencyCode, cancellationToken);
            
            if (!currencyExists)
            {
                throw new InvalidOperationException($"Валюта с кодом {request.CurrencyCode} не найдена.");
            }

            var isFavorite = await _favoriteRepository.IsFavoriteAsync(request.UserId, request.CurrencyCode, cancellationToken);
            
            if (isFavorite)
            {
                return false;
            }

            await _favoriteRepository.AddFavoriteAsync(request.UserId, request.CurrencyCode, cancellationToken);
            return true;
        }
    }
}
using SharedKernel.Entities;

namespace UserService.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с избранными валютами пользователя.
    /// </summary>
    public interface IUserFavoriteCurrencyRepository
    {
        /// <summary>
        /// Получить список избранных валют пользователя.
        /// </summary>
        /// <param name="userId"> Идентификатор пользователя.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Список избранных валют.</returns>
        Task<IEnumerable<Currency>> GetUserFavoritesAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Добавить валюту в избранное пользователя.
        /// </summary>
        /// <param name="userId"> Идентификатор пользователя.</param>
        /// <param name="currencyCode"> Код валюты (USD, EUR).</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        Task AddFavoriteAsync(Guid userId, string currencyCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить валюту из избранного пользователя.
        /// </summary>
        /// <param name="userId"> Идентификатор пользователя.</param>
        /// <param name="currencyCode"> Код валюты (USD, EUR).</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        Task RemoveFavoriteAsync(Guid userId, string currencyCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Проверить, находится ли валюта в избранном пользователя.
        /// </summary>
        /// <param name="userId"> Идентификатор пользователя.</param>
        /// <param name="currencyCode"> Код валюты (USD, EUR).</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> True - валюта в избранном, иначе False.</returns>
        Task<bool> IsFavoriteAsync(Guid userId, string currencyCode, CancellationToken cancellationToken = default);
    }
}
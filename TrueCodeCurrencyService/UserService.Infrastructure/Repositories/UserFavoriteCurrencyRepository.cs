using Microsoft.EntityFrameworkCore;
using SharedKernel.Entities;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories
{
    /// <summary>
    /// Репозиторий для работы с избранными валютами пользователя.
    /// </summary>
    public class UserFavoriteCurrencyRepository : IUserFavoriteCurrencyRepository
    {
        /// <summary>
        /// Контекст базы данных.
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Конструктор репозитория избранных валют.
        /// </summary>
        /// <param name="context"> Контекст базы данных.</param>
        public UserFavoriteCurrencyRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Добавить валюту в избранное пользователя.
        /// </summary>
        /// <param name="userId"> Идентификатор пользователя.</param>
        /// <param name="currencyCode"> Код валюты (USD, EUR).</param>
        /// <param name="cancellationToken"> Токен отмены операции.</param>
        /// <exception cref="InvalidOperationException"> Выбрасывается, если валюта с указанным кодом не найдена.</exception>
        public async Task AddFavoriteAsync(Guid userId, string currencyCode, CancellationToken cancellationToken = default)
        {
            var currency = await _context.Currencies
                .FirstOrDefaultAsync(c => c.Code == currencyCode, cancellationToken);

            if (currency == null)
            {
                throw new InvalidOperationException($"Валюта с кодом {currencyCode} не найдена.");
            }

            var favorite = new UserFavoriteCurrency
            {
                UserId = userId,
                CurrencyId = currency.Id
            };

            await _context.UserFavoriteCurrencies.AddAsync(favorite, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Удалить валюту из избранного пользователя.
        /// </summary>
        /// <param name="userId"> Идентификатор пользователя.</param>
        /// <param name="currencyCode"> Код валюты (USD, EUR).</param>
        /// <param name="cancellationToken"> Токен отмены операции.</param>
        public async Task RemoveFavoriteAsync(Guid userId, string currencyCode, CancellationToken cancellationToken = default)
        {
            var currency = await _context.Currencies
                .FirstOrDefaultAsync(c => c.Code == currencyCode, cancellationToken);

            if (currency == null)
            {
                return;
            }

            var favorite = await _context.UserFavoriteCurrencies
                .FirstOrDefaultAsync(ufc => ufc.UserId == userId && ufc.CurrencyId == currency.Id, cancellationToken);

            if (favorite != null)
            {
                _context.UserFavoriteCurrencies.Remove(favorite);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Получить список избранных валют пользователя с актуальными курсами.
        /// </summary>
        /// <param name="userId"> Идентификатор пользователя.</param>
        /// <param name="cancellationToken"> Токен отмены операции.</param>
        /// <returns> Список валют, добавленных пользователем в избранное.</returns>
        public async Task<IEnumerable<Currency>> GetUserFavoritesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserFavoriteCurrencies
                .Where(ufc => ufc.UserId == userId)
                .Include(ufc => ufc.Currency)
                .Select(ufc => ufc.Currency)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Проверить, находится ли валюта в избранном пользователя.
        /// </summary>
        /// <param name="userId"> Идентификатор пользователя.</param>
        /// <param name="currencyCode"> Код валюты (USD, EUR).</param>
        /// <param name="cancellationToken"> Токен отмены операции.</param>
        /// <returns> True - валюта в избранном, False - не в избранном.</returns>
        public async Task<bool> IsFavoriteAsync(Guid userId, string currencyCode, CancellationToken cancellationToken = default)
        {
            var currency = await _context.Currencies
                .FirstOrDefaultAsync(c => c.Code == currencyCode, cancellationToken);

            if (currency == null)
            {
                return false;
            }

            return await _context.UserFavoriteCurrencies
                .AnyAsync(ufc => ufc.UserId == userId && ufc.CurrencyId == currency.Id, cancellationToken);
        }
    }
}
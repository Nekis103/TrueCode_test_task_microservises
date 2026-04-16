using Microsoft.EntityFrameworkCore;
using SharedKernel.Entities;
using FinanceService.Domain.Interfaces;
using FinanceService.Infrastructure.Data;

namespace FinanceService.Infrastructure.Repositories
{
    /// <summary>
    /// Репозиторий для работы с валютами.
    /// </summary>
    public class CurrencyRepository : ICurrencyRepository
    {
        /// <summary>
        /// Контекст.
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Конструктор для работы с валютами.
        /// </summary>
        /// <param name="context"> Контекст.</param>
        public CurrencyRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить валюту по идентификатору. 
        /// </summary>
        /// <param name="id"> Идентификатор валюты.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Валюта.</returns>
        public async Task<Currency?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Currencies
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        /// <summary>
        /// Получить валюту по коду валюты.
        /// </summary>
        /// <param name="code"> Код.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Валюта.</returns>
        public async Task<Currency?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Currencies
                .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
        }

        /// <summary>
        /// Получить все валюты.
        /// </summary>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Список валют.</returns>
        public async Task<IEnumerable<Currency>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Currencies
                .OrderBy(c => c.Code)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Получить валюты по списку кодов.
        /// </summary>
        /// <param name="codes"> Список кодов валют.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Список валют.</returns>
        public async Task<IEnumerable<Currency>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken cancellationToken = default)
        {
            if (codes == null || !codes.Any())
                return new List<Currency>();

            return await _context.Currencies
                .Where(c => codes.Contains(c.Code))
                .OrderBy(c => c.Code)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Добавить или обновить валюту.
        /// </summary>
        /// <param name="currency"> Валюта.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns></returns>
        public async Task AddOrUpdateAsync(Currency currency, CancellationToken cancellationToken = default)
        {
            var existing = await GetByCodeAsync(currency.Code, cancellationToken);

            if (existing != null)
            {
                existing.Rate = currency.Rate;
                existing.LastUpdated = currency.LastUpdated;
                _context.Currencies.Update(existing);
            }
            else
            {
                await _context.Currencies.AddAsync(currency, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Добавить или обновить список валют (оптимизированная версия).
        /// </summary>
        /// <param name="currencies"> Список валют.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns></returns>
        public async Task AddOrUpdateRangeAsync(IEnumerable<Currency> currencies, CancellationToken cancellationToken = default)
        {
            var currencyList = currencies.ToList();
            if (!currencyList.Any())
                return;

            var codes = currencyList.Select(c => c.Code).ToList();
            var existingCurrencies = await _context.Currencies
                .Where(c => codes.Contains(c.Code))
                .ToDictionaryAsync(c => c.Code, cancellationToken);

            foreach (var currency in currencyList)
            {
                if (existingCurrencies.TryGetValue(currency.Code, out var existing))
                {
                    existing.Rate = currency.Rate;
                    existing.LastUpdated = currency.LastUpdated;
                    _context.Currencies.Update(existing);
                }
                else
                {
                    await _context.Currencies.AddAsync(currency, cancellationToken);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
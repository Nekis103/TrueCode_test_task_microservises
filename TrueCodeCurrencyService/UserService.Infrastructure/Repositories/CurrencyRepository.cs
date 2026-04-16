using Microsoft.EntityFrameworkCore;
using SharedKernel.Entities;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories
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
        /// Конструктор репозитория валют.
        /// </summary>
        /// <param name="context"> Контекст.</param>
        public CurrencyRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Добавить валюту.
        /// </summary>
        /// <param name="currency"> Валюта.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns></returns>
        public async Task AddAsync(Currency currency, CancellationToken cancellationToken = default)
        {
            await _context.Currencies.AddAsync(currency, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Обновить валюту.
        /// </summary>
        /// <param name="currency"> Валюта.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns></returns>
        public async Task UpdateAsync(Currency currency, CancellationToken cancellationToken = default)
        {
            _context.Currencies.Update(currency);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Получить по идентификатору.
        /// </summary>
        /// <param name="id"> Иднтификатор валюты.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Валюта.</returns>
        public async Task<Currency?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Currencies
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        /// <summary>
        /// Получить валюту по названию.
        /// </summary>
        /// <param name="name"> Название.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Валюта.</returns>
        public async Task<Currency?> GetByCodeAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Currencies
                .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
        }

        /// <summary>
        /// Получить список всех валют.
        /// </summary>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Список валют.</returns>
        public async Task<IEnumerable<Currency>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Currencies.ToListAsync(cancellationToken);
        }
    }
}

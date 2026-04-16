using SharedKernel.Entities;

namespace FinanceService.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с валютами.
    /// </summary>
    public interface ICurrencyRepository
    {
        /// <summary>
        /// Получить валюту по идентификатору. 
        /// </summary>
        /// <param name="id"> Идентификатор валюты.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Валюта.</returns>
        Task<Currency?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить валюту по коду валюты.
        /// </summary>
        /// <param name="code"> Код.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Валюта.</returns>
        Task<Currency?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить все валюты.
        /// </summary>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Список валют.</returns>
        Task<IEnumerable<Currency>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить валюты по списку кодов.
        /// </summary>
        /// <param name="codes"> Список кодов валют.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Список валют.</returns>
        Task<IEnumerable<Currency>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken cancellationToken = default);

        /// <summary>
        /// Добавить или обновить валюту.
        /// </summary>
        /// <param name="currency"> Валюта.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns></returns>
        Task AddOrUpdateAsync(Currency currency, CancellationToken cancellationToken = default);

        /// <summary>
        /// Добавить или обновить список валют.
        /// </summary>
        /// <param name="currencies"> Список валют.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns></returns>
        Task AddOrUpdateRangeAsync(IEnumerable<Currency> currencies, CancellationToken cancellationToken = default);
    }
}
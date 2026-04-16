using SharedKernel.Entities;

namespace UserService.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий валют.
    /// </summary>
    public interface ICurrencyRepository
    {
        /// <summary>
        /// Добавление валюты.
        /// </summary>
        /// <param name="currency"> Валюта.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns></returns>
        Task AddAsync(Currency currency, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновление валюты.
        /// </summary>
        /// <param name="currency"> Валюта.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns></returns>
        Task UpdateAsync(Currency currency, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить валюту по идентификатору.
        /// </summary>
        /// <param name="id"> Идентификатор валюты.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Валюта.</returns>
        Task<Currency?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить валюту по названию.
        /// </summary>
        /// <param name="name"> Название валюты.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Валюта.</returns>
        Task<Currency?> GetByCodeAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить полный список валют.
        /// </summary>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Список всех валют.</returns>
        Task<IEnumerable<Currency>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}

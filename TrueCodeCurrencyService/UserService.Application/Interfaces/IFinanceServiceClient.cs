namespace UserService.Application.Interfaces
{
    /// <summary>
    /// Клиент для взаимодействия с FinanceService.
    /// </summary>
    public interface IFinanceServiceClient
    {
        /// <summary>
        /// Проверить, существует ли валюта с указанным кодом.
        /// </summary>
        /// <param name="code"> Код валюты (USD, EUR).</param>
        /// <param name="cancellationToken"> Токен отмены операции.</param>
        /// <returns> True - валюта существует, False - не найдена или ошибка.</returns>
        Task<bool> CurrencyExistsAsync(string code, CancellationToken cancellationToken = default);
    }
}

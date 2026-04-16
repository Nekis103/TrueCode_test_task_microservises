namespace FinanceService.API.DTOs
{
    /// <summary>
    /// Валюты.
    /// </summary>
    public class CurrencyResponse
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Код.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Название.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Значение валюты по отношению к Рублю.
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Дата изменения.
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }
}

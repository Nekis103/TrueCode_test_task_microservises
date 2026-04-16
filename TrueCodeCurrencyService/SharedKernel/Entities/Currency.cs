using System.Text.Json.Serialization;

namespace SharedKernel.Entities
{
    /// <summary>
    /// Валюта.
    /// </summary>
    public class Currency
    {
        /// <summary>
        /// Идентификатор валюты.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Иденентификатор из API ЦБ РФ ("R01010").
        /// </summary>
        public string CbrId { get; set; } = string.Empty;

        /// <summary>
        /// Код валюты("USD", "EUR", "CNY").
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Название валюты.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Курс к рублю.
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Время последнего обновления.
        /// </summary>
        public DateTime LastUpdated { get; set; } // Время последнего обновления

        /// <summary>
        /// Избранные валюты.
        /// </summary>
        [JsonIgnore]
        public ICollection<UserFavoriteCurrency> UserFavorites { get; set; } = new List<UserFavoriteCurrency>();
    }
}

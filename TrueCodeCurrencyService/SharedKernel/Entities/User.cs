using System.Text.Json.Serialization;

namespace SharedKernel.Entities
{
    /// <summary>
    /// Пользователь.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Пароль.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Изранные валюты.
        /// </summary>
        [JsonIgnore]
        public ICollection<UserFavoriteCurrency> FavoriteCurrencies { get; set; } = new List<UserFavoriteCurrency>();
    }
}

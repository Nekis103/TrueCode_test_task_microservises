namespace UserService.API.DTOs
{
    /// <summary>
    /// Избранные валюты пользователя.
    /// </summary>
    public class FavoriteCurrencyResponse
    {
        /// <summary>
        /// Код.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Название.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}

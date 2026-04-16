namespace SharedKernel.Entities
{
    /// <summary>
    /// Валюты в избранном пользователя.
    /// </summary>
    public class UserFavoriteCurrency
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Пользователь.
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Идентификатор валюты.
        /// </summary>
        public Guid CurrencyId { get; set; }

        /// <summary>
        /// Валюта.
        /// </summary>
        public Currency Currency { get; set; } = null!;
    }
}

namespace UserService.API.DTOs
{
    /// <summary>
    /// Регистрация.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Пароль.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}

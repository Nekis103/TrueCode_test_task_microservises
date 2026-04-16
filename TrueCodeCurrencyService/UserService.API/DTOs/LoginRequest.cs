namespace UserService.API.DTOs
{
    /// <summary>
    /// Авторизация.
    /// </summary>
    public class LoginRequest
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

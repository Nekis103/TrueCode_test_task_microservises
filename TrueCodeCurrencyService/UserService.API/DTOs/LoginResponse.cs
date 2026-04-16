namespace UserService.API.DTOs
{
    /// <summary>
    /// Авторизация.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Токен авторизации.
        /// </summary>
        public string Token { get; set; } = string.Empty;
    }
}

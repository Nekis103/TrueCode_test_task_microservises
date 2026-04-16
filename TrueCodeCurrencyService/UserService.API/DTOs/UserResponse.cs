namespace UserService.API.DTOs
{
    /// <summary>
    /// Пользотватель.
    /// </summary>
    public class UserResponse
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string Username { get; set; } = string.Empty;
    }
}

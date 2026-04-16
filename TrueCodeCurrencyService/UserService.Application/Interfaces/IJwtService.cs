namespace UserService.Application.Interfaces
{
    /// <summary>
    /// Генерация JWT токена.
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Сгенерировать токен авторизации.
        /// </summary>
        /// <param name="userId"> Идентификатор пользователя.</param>
        /// <param name="username"> Имя пользователя.</param>
        /// <returns> JWT токен.</returns>
        string GenerateToken(Guid userId, string username);
    }
}

using SharedKernel.Entities;

namespace UserService.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий пользователя.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Доравить пользователя.
        /// </summary>
        /// <param name="user"> Пользователь.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns></returns>
        Task AddAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Пользователя обновлен.
        /// </summary>
        /// <param name="user"> Пользователь.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns></returns>
        Task UpdateAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить пользователя.
        /// </summary>
        /// <param name="id"> Идентификатор пользователя.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns></returns>
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить пользователя по идентификатору.
        /// </summary>
        /// <param name="id"> Идентификатор пользователя.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Пользователь.</returns>
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить пользователя по имени пользователя.
        /// </summary>
        /// <param name="username"> Имя пользователя.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Пользователь.</returns>
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить всех пользователей.
        /// </summary>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Списко пользователей.</returns>
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Пользователь есть в системе.
        /// </summary>
        /// <param name="username"> Имя пользователя.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> True - пользователь есть в системе, false - нет.</returns>
        Task<bool> ExistsAsync(string username, CancellationToken cancellationToken = default);
    }
}

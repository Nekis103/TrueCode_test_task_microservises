using Microsoft.EntityFrameworkCore;
using SharedKernel.Entities;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories
{
    /// <summary>
    /// Репозиторий пользователя.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// Контекст.
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Конструктор репозитория пользователя.
        /// </summary>
        /// <param name="context"> Контекст.</param>
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Добавление пользователя.
        /// </summary>
        /// <param name="user"> Пользователь.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns></returns>
        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Обновление пользователя.
        /// </summary>
        /// <param name="user"> Пользователь.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns></returns>
        public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Удаление пользователя.
        /// </summary>
        /// <param name="id"> Идентификатор пользователя.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns></returns>
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await GetByIdAsync(id, cancellationToken);

            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Получить пользователя по идентификатору.
        /// </summary>
        /// <param name="id"> Идентификатор пользователя.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Пользователь.</returns>
        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.FavoriteCurrencies)
                .ThenInclude(ufc => ufc.Currency)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        /// <summary>
        /// Получить пользователя по имени.
        /// </summary>
        /// <param name="username"> Имя пользователя.</param>
        /// <param name="cancellationToken">  Токен отмены.</param>
        /// <returns> Пользователь.</returns>
        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Name == username, cancellationToken);
        }

        /// <summary>
        /// Получить всех пользователей.
        /// </summary>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> Списко пользователей.</returns>
        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Проверить наличие пользователя.
        /// </summary>
        /// <param name="username"> Имя пользователя.</param>
        /// <param name="cancellationToken"> Токен отмены.</param>
        /// <returns> True - пользователь есть в системе, false - пользователя нет в системе.</returns>
        public async Task<bool> ExistsAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AnyAsync(u => u.Name == username, cancellationToken);
        }
    }
}

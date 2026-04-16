using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Application.Interfaces;

namespace UserService.Infrastructure.Security
{
    /// <summary>
    /// JWT.
    /// </summary>
    public class JwtService : IJwtService
    {
        /// <summary>
        /// Конфигурационные ключи.
        /// </summary>
        private static class ConfigKeys
        {
            public const string Issuer = "Jwt:Issuer";
            public const string Secret = "Jwt:Secret";
            public const string Audience = "Jwt:Audience";
            public const string TokenDurationHours = "Jwt:TokenDurationHours";
        }

        #region Приватные поля.
        /// <summary>
        /// Конфигурация.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Стандартное значение длительности токена авторизации.
        /// </summary>
        private const int DefaultTokenDurationHours = 1;
        #endregion

        /// <summary>
        /// Конструктор создания сервиса по генерации JWT токена.
        /// </summary>
        /// <param name="configuration"> Файл конфигурации.</param>
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Генерация токена авторизации.
        /// </summary>
        /// <param name="userId"> Идентификатор пользователя.</param>
        /// <param name="username"> Имя пользователя.</param>
        /// <returns> Токен авторизации.</returns>
        public string GenerateToken(Guid userId, string username)
        {
            ValidateConfiguration();

            var durationHours = _configuration.GetValue<int>(ConfigKeys.TokenDurationHours, DefaultTokenDurationHours);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[ConfigKeys.Secret]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration[ConfigKeys.Issuer],
                audience: _configuration[ConfigKeys.Audience],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(durationHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Валидация JWT конфигурации.
        /// </summary>
        /// <exception cref="InvalidOperationException"> Токен не соответствует стандартам.</exception>
        private void ValidateConfiguration()
        {
            var secret = _configuration[ConfigKeys.Secret];
            if (string.IsNullOrEmpty(secret) || secret.Length < 32)
            {
                throw new InvalidOperationException("JWT Secret must be at least 32 characters");
            }

            if (string.IsNullOrEmpty(_configuration[ConfigKeys.Issuer]))
            {
                throw new InvalidOperationException("JWT Issuer is not configured");
            }

            if (string.IsNullOrEmpty(_configuration[ConfigKeys.Audience]))
            {
                throw new InvalidOperationException("JWT Audience is not configured");
            }
                
        }
    }
}
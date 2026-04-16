// UserService.Tests/TestData/UserTestData.cs
using SharedKernel.Entities;
using UserService.Application.Commands;

namespace UserService.Tests.TestData
{
    public static class UserTestData
    {
        public static Guid GetTestUserId()
        {
            return Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
        }

        public static Guid GetUsdCurrencyId()
        {
            return Guid.Parse("11111111-1111-1111-1111-111111111111");
        }

        public static Guid GetEurCurrencyId()
        {
            return Guid.Parse("22222222-2222-2222-2222-222222222222");
        }

        public static User GetTestUser()
        {
            return new User
            {
                Id = GetTestUserId(),
                Name = "testuser",
                Password = BCrypt.Net.BCrypt.HashPassword("password123")
            };
        }

        public static User GetExistingUser()
        {
            return new User
            {
                Id = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB"),
                Name = "existinguser",
                Password = BCrypt.Net.BCrypt.HashPassword("password123")
            };
        }

        public static RegisterUserCommand GetRegisterCommand()
        {
            return new RegisterUserCommand("testuser", "password123");
        }

        public static RegisterUserCommand GetRegisterCommandWithExistingName()
        {
            return new RegisterUserCommand("existinguser", "password123");
        }

        public static LoginUserCommand GetLoginCommand()
        {
            return new LoginUserCommand("testuser", "password123");
        }

        public static LoginUserCommand GetLoginCommandWithWrongPassword()
        {
            return new LoginUserCommand("testuser", "wrongpassword");
        }

        public static LoginUserCommand GetLoginCommandWithUnknownUser()
        {
            return new LoginUserCommand("unknown", "password123");
        }

        /// <summary>
        /// Получить команду добавления валюты в избранное.
        /// </summary>
        public static AddFavoriteCurrencyCommand GetAddFavoriteCommand()
        {
            return new AddFavoriteCurrencyCommand(GetTestUserId(), "USD");
        }

        /// <summary>
        /// Получить команду удаления валюты из избранного.
        /// </summary>
        public static RemoveFavoriteCurrencyCommand GetRemoveFavoriteCommand()
        {
            return new RemoveFavoriteCurrencyCommand(GetTestUserId(), "USD");
        }

        public static List<Currency> GetTestFavoriteCurrencies()
        {
            return new List<Currency>
            {
                new Currency { Id = GetUsdCurrencyId(), Code = "USD", Name = "Доллар США", Rate = 75.5m },
                new Currency { Id = GetEurCurrencyId(), Code = "EUR", Name = "Евро", Rate = 85.3m }
            };
        }

        public static string GetJwtToken()
        {
            return "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0dXNlciIsIm5hbWUiOiJ0ZXN0dXNlciJ9.test";
        }
    }
}
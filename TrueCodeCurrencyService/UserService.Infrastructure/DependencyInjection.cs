using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Interfaces;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Security;
using UserService.Infrastructure.Services;

namespace UserService.Infrastructure
{
    /// <summary>
    /// DI UserService.Infrastructure.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Дефолтное значение адреса сервиса финансов.
        /// </summary>
        private static readonly string DefaultFinanceServiceBaseUrl = "http://finance-service:80";

        /// <summary>
        /// Добавить зависимости инфраструктурного слоя.
        /// </summary>
        /// <param name="services"> Коллекция сервисов.</param>
        /// <param name="configuration"> Конфигурация приложения.</param>
        /// <returns> Коллекция сервисов с добавленными зависимостями.</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IUserFavoriteCurrencyRepository, UserFavoriteCurrencyRepository>();

            services.AddScoped<IJwtService, JwtService>();

            services.AddHttpClient<IFinanceServiceClient, FinanceServiceClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["FinanceService:BaseUrl"] ?? DefaultFinanceServiceBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            return services;
        }
    }
}
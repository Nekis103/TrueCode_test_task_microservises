using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FinanceService.Domain.Interfaces;
using FinanceService.Infrastructure.Data;
using FinanceService.Infrastructure.Repositories;
using FinanceService.Infrastructure.Services;

namespace FinanceService.Infrastructure
{
    /// <summary>
    /// DI FinanceService.Infrastructure.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Добавить зависимости.
        /// </summary>
        /// <param name="services"> Сервисы.</param>
        /// <param name="configuration"> Конфигурация.</param>
        /// <returns> Сервисы.</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ICurrencyRepository, CurrencyRepository>();

            services.AddScoped<ICurrencyParser, CurrencyParser>();

            services.AddHttpClient();

            services.AddHostedService<CurrencyBackgroundService>();

            return services;
        }
    }
}

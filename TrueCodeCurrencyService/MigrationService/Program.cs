using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserService.Infrastructure.Data;

namespace MigrationService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Начало работы сервиса миграций...");

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));
                })
                .Build();

            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                try
                {
                    Console.WriteLine("Проверка соединения с базой данных...");

                    var canConnect = await dbContext.Database.CanConnectAsync();

                    if (!canConnect)
                    {
                        Console.WriteLine("База данных не существует. Создание...");
                    }

                    Console.WriteLine("Применение миграции...");
                    await dbContext.Database.MigrateAsync();

                    Console.WriteLine("Миграция применена успешно!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при применении миграции: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    throw;
                }
            }

            Console.WriteLine("Сервис миграций завершил работу.");
        }
    }
}
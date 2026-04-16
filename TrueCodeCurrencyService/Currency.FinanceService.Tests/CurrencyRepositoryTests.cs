// FinanceService.Tests/Infrastructure/Repositories/CurrencyRepositoryTests.cs
using FinanceService.Infrastructure.Data;
using FinanceService.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Entities;
using Xunit;

namespace FinanceService.Tests.Infrastructure.Repositories
{
    /// <summary>
    /// Тесты для репозитория валют.
    /// </summary>
    public class CurrencyRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly CurrencyRepository _repository;

        /// <summary>
        /// Конструктор тестов репозитория валют.
        /// </summary>
        public CurrencyRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new CurrencyRepository(_context);
        }

        /// <summary>
        /// Тест: добавление новой валюты.
        /// </summary>
        [Fact]
        public async Task AddOrUpdateAsync_NewCurrency_ShouldAdd()
        {
            // Arrange
            var currency = new Currency
            {
                Id = Guid.NewGuid(),
                Code = "USD",
                CbrId = "R01235",
                Name = "Доллар США",
                Rate = 75.5m,
                LastUpdated = DateTime.UtcNow
            };

            // Act
            await _repository.AddOrUpdateAsync(currency, CancellationToken.None);

            // Assert
            var result = await _context.Currencies.FirstOrDefaultAsync(x => x.Code == "USD");
            result.Should().NotBeNull();
            result!.Rate.Should().Be(75.5m);
        }

        /// <summary>
        /// Тест: обновление существующей валюты.
        /// </summary>
        [Fact]
        public async Task AddOrUpdateAsync_ExistingCurrency_ShouldUpdate()
        {
            // Arrange
            var currency = new Currency
            {
                Id = Guid.NewGuid(),
                Code = "USD",
                CbrId = "R01235",
                Name = "Доллар США",
                Rate = 75.5m,
                LastUpdated = DateTime.UtcNow
            };

            await _repository.AddOrUpdateAsync(currency, CancellationToken.None);

            currency.Rate = 80.0m;

            // Act
            await _repository.AddOrUpdateAsync(currency, CancellationToken.None);

            // Assert
            var result = await _context.Currencies.FirstOrDefaultAsync(x => x.Code == "USD");
            result.Should().NotBeNull();
            result!.Rate.Should().Be(80.0m);
        }

        /// <summary>
        /// Тест: получение всех валют.
        /// </summary>
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCurrencies()
        {
            // Arrange
            var currencies = new List<Currency>
            {
                new Currency { Id = Guid.NewGuid(), Code = "USD", CbrId = "R01235", Name = "Доллар США", Rate = 75.5m, LastUpdated = DateTime.UtcNow },
                new Currency { Id = Guid.NewGuid(), Code = "EUR", CbrId = "R01239", Name = "Евро", Rate = 85.3m, LastUpdated = DateTime.UtcNow }
            };

            await _context.Currencies.AddRangeAsync(currencies);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync(CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
        }

        /// <summary>
        /// Тест: получение валюты по коду.
        /// </summary>
        [Fact]
        public async Task GetByCodeAsync_ExistingCode_ShouldReturnCurrency()
        {
            // Arrange
            var currency = new Currency
            {
                Id = Guid.NewGuid(),
                Code = "USD",
                CbrId = "R01235",
                Name = "Доллар США",
                Rate = 75.5m,
                LastUpdated = DateTime.UtcNow
            };

            await _context.Currencies.AddAsync(currency);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByCodeAsync("USD", CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Code.Should().Be("USD");
        }

        /// <summary>
        /// Тест: получение валют по списку кодов.
        /// </summary>
        [Fact]
        public async Task GetByCodesAsync_ExistingCodes_ShouldReturnCurrencies()
        {
            // Arrange
            var currencies = new List<Currency>
            {
                new Currency { Id = Guid.NewGuid(), Code = "USD", CbrId = "R01235", Name = "Доллар США", Rate = 75.5m, LastUpdated = DateTime.UtcNow },
                new Currency { Id = Guid.NewGuid(), Code = "EUR", CbrId = "R01239", Name = "Евро", Rate = 85.3m, LastUpdated = DateTime.UtcNow },
                new Currency { Id = Guid.NewGuid(), Code = "CNY", CbrId = "R01010", Name = "Китайский юань", Rate = 10.45m, LastUpdated = DateTime.UtcNow }
            };

            await _context.Currencies.AddRangeAsync(currencies);
            await _context.SaveChangesAsync();

            var codes = new List<string> { "USD", "EUR" };

            // Act
            var result = await _repository.GetByCodesAsync(codes, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Select(x => x.Code).Should().Contain("USD");
            result.Select(x => x.Code).Should().Contain("EUR");
        }

        /// <summary>
        /// Тест: получение валют по пустому списку кодов.
        /// </summary>
        [Fact]
        public async Task GetByCodesAsync_EmptyCodes_ShouldReturnEmptyList()
        {
            // Act
            var result = await _repository.GetByCodesAsync(new List<string>(), CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        /// <summary>
        /// Тест: получение валюты по несуществующему коду.
        /// </summary>
        [Fact]
        public async Task GetByCodeAsync_NonExistingCode_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetByCodeAsync("XXX", CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
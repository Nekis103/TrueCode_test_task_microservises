using Microsoft.EntityFrameworkCore;
using SharedKernel.Entities;

namespace FinanceService.Infrastructure.Data
{
    /// <summary>
    /// Контекст БД.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Конструктор контекста БД.
        /// </summary>
        /// <param name="options"> Настройки.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Валюты.
        /// </summary>
        public DbSet<Currency> Currencies { get; set; }

        /// <summary>
        /// Создание модели.
        /// </summary>
        /// <param name="modelBuilder"> Создатель модели.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.ToTable("Currencies");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(10);
                entity.Property(e => e.CbrId).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Rate).IsRequired().HasPrecision(18, 4);
                entity.Property(e => e.LastUpdated).IsRequired();
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.CbrId).IsUnique();

                entity.Ignore(e => e.UserFavorites);
            });
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SharedKernel.Entities;

namespace UserService.Infrastructure.Data
{
    /// <summary>
    /// Контекст БД.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Конструктор создания контекста.
        /// </summary>
        /// <param name="options"> Настройки.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Пользователи.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Валюты.
        /// </summary>
        public DbSet<Currency> Currencies { get; set; }

        /// <summary>
        /// Избранные пользователем валюты.
        /// </summary>
        public DbSet<UserFavoriteCurrency> UserFavoriteCurrencies { get; set; }

        /// <summary>
        /// Создание модели.
        /// </summary>
        /// <param name="modelBuilder"> Создатель модели.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired();
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.CbrId).IsUnique();
                entity.Property(e => e.Code).IsRequired().HasMaxLength(3);
                entity.Property(e => e.CbrId).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Rate).HasPrecision(18, 4);
                entity.Property(e => e.LastUpdated).IsRequired();
            });

            modelBuilder.Entity<UserFavoriteCurrency>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.CurrencyId });

                entity.HasOne(ufc => ufc.User)
                    .WithMany(u => u.FavoriteCurrencies)
                    .HasForeignKey(ufc => ufc.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ufc => ufc.Currency)
                    .WithMany(c => c.UserFavorites)
                    .HasForeignKey(ufc => ufc.CurrencyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

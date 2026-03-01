using Microsoft.EntityFrameworkCore;
using System.IO;

namespace InvestmentCalculator.Models;

public class AppDbContext : DbContext
{
    public DbSet<Calculation> Calculations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Получаем путь к папке, где лежит исполняемый файл (например, ...\bin\Debug\net9.0-windows\)
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;

        // Поднимаемся на три уровня вверх, чтобы попасть в корень проекта
        // Из "bin/Debug/net9.0-windows" -> "bin/Debug" -> "bin" -> корень проекта
        string projectRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\"));

        // Формируем полный путь к файлу базы данных в корне проекта
        string dbPath = Path.Combine(projectRoot, "investments.db");

        // Опционально: выводим путь для отладки (посмотреть в окно Output)
        System.Diagnostics.Debug.WriteLine($"Путь к БД: {dbPath}");

        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Calculation>().ToTable("Calculations");
    }
}
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace InvestmentCalculator.Models;

public class AppDbContext : DbContext
{
    public DbSet<Calculation> Calculations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "investments.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Calculation>().ToTable("Calculations");
    }
}
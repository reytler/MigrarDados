using Microsoft.EntityFrameworkCore;
using MigrarDados.Models;

namespace MigrarDados.Data;

public class SQLiteContext : DbContext
{
    public DbSet<CagedSQLite> CagedSQLite { get; set; } = null!;

    private readonly string _connectionString;
    public SQLiteContext(string connectionString) => _connectionString = connectionString;

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CagedSQLite>().ToTable("caged");
        modelBuilder.Entity<CagedSQLite>().HasNoKey(); // SQLite sem PK
    }
}

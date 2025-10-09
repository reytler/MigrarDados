using Microsoft.EntityFrameworkCore;
using MigrarDados.Models;


namespace MigrarDados.Data;

public class MySQLContext : DbContext
{
    public DbSet<Caged> Caged { get; set; } = null!;

    private readonly string _connectionString;
    public MySQLContext(string connectionString) => _connectionString = connectionString;

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
}
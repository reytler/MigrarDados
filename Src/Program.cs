using Microsoft.Extensions.Configuration;
using MigrarDados.Data;
using MigrarDados.Migration;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var sqliteConnectionString = config.GetConnectionString("SQLite");
if (string.IsNullOrWhiteSpace(sqliteConnectionString))
{
    sqliteConnectionString = @"Data Source=C:\Users\reytler\Downloads\eng_dados\amostra100pc.sqlite";
}

var mysqlConn = config.GetConnectionString("MySQL")!;

using var sqliteContext = new SQLiteContext(sqliteConnectionString);

var runner = new MigrationRunner();
await runner.RunAsync(sqliteContext, mysqlConn);

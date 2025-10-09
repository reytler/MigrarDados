
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MigrarDados.Data;
using MigrarDados.Models;
using System.Diagnostics;


var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var sqliteConnectionString = @"Data Source=C:\Users\reytler\Downloads\eng_dados\amostra100pc.sqlite";
var mysqlConn = config.GetConnectionString("MySQL")!;

using var sqliteContext = new SQLiteContext(sqliteConnectionString);
using var mysqlContext = new MySQLContext(mysqlConn);

mysqlContext.ChangeTracker.AutoDetectChangesEnabled = false;

const int batchSize = 20000;
var batch = new List<Caged>(batchSize);

long totalInserido = 0;
var stopwatch = Stopwatch.StartNew();

await foreach (var item in sqliteContext.CagedSQLite.AsNoTracking().AsAsyncEnumerable())
{
    // Mapear para MySQL
    batch.Add(new Caged
    {
        Secao = item.Secao,
        CdMunicipio = item.CdMunicipio,
        Municipio = item.Municipio,
        Uf = item.Uf,
        FaixaEmpregados = item.FaixaEmpregados,
        Competencia = item.Competencia,
        Fluxo = item.Fluxo
    });

    if (batch.Count >= batchSize)
    {
        await InserirLoteAsync(mysqlContext, batch);
        totalInserido += batch.Count;
        Console.WriteLine($"✅ Inseridos {totalInserido:N0} registros até agora ({stopwatch.Elapsed:mm\\:ss}).");
        batch.Clear();
    }
}

// Inserir o restante
if (batch.Any())
{
    await InserirLoteAsync(mysqlContext, batch);
    totalInserido += batch.Count;
}

stopwatch.Stop();
Console.WriteLine("🎉 Sincronização concluída!");
Console.WriteLine($"📊 Total inserido: {totalInserido:N0} registros em {stopwatch.Elapsed:mm\\:ss}.");

static async Task InserirLoteAsync(MySQLContext context, List<Caged> registros)
{
    const int maxTentativas = 3;
    int tentativa = 0;

    while (tentativa < maxTentativas)
    {
        try
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            await context.Caged.AddRangeAsync(registros);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            context.ChangeTracker.Clear();
            return;
        }
        catch (Exception ex)
        {
            tentativa++;
            Console.WriteLine($"⚠️ Erro ao inserir lote (tentativa {tentativa}/{maxTentativas}): {ex.Message}");

            if (tentativa >= maxTentativas)
                throw; // propaga se falhar tudo

            await Task.Delay(1000 * tentativa); // retry com backoff
        }
    }
}

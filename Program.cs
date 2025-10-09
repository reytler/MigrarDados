
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MigrarDados.Data;
using MigrarDados.Models;
using System.Diagnostics;
using System.Threading.Channels;




var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var sqliteConnectionString = @"Data Source=C:\Users\reytler\Downloads\eng_dados\amostra100pc.sqlite";
var mysqlConn = config.GetConnectionString("MySQL")!;

using var sqliteContext = new SQLiteContext(sqliteConnectionString);
using var mysqlContext = new MySQLContext(mysqlConn);

var initialCapacity = 2000;

var channel = Channel.CreateBounded<Caged>(new BoundedChannelOptions(initialCapacity)
{
    FullMode = BoundedChannelFullMode.Wait
});

var configStream = new AutoTuningConfig
{
    BatchSize = 1000,
    Capacity = initialCapacity,
    MinBatchSize = 500,
    MaxBatchSize = 5000,
    MinCapacity = 1000,
    MaxCapacity = 20000
};

var stopwatchTotal = Stopwatch.StartNew();
var stopwatchLeitura = new Stopwatch();
var stopwatchInsercao = new Stopwatch();

var totalLidos = 0;
var totalInseridos = 0;

var produtor = Task.Run(async () =>
{
    stopwatchLeitura.Start();
    await foreach (var item in sqliteContext.CagedSQLite.AsNoTracking().AsAsyncEnumerable())
    {
        var novo = new Caged
        {
            Secao = item.Secao,
            CdMunicipio = item.CdMunicipio,
            Municipio = item.Municipio,
            Uf = item.Uf,
            FaixaEmpregados = item.FaixaEmpregados,
            Competencia = item.Competencia,
            Fluxo = item.Fluxo
        };

        
        await channel.Writer.WriteAsync(novo);
        totalLidos++;
    }

    stopwatchLeitura.Stop();
    channel.Writer.Complete();
});

var consumidor = Task.Run(async () =>
{
    var batch = new List<Caged>();

    await foreach (var registro in channel.Reader.ReadAllAsync())
    {
        batch.Add(registro);

        if (batch.Count >= configStream.BatchSize)
        {
            stopwatchInsercao.Start();
            await InserirLoteAsync(mysqlContext, batch);
            stopwatchInsercao.Stop();

            totalInseridos += batch.Count;
            Console.WriteLine($"✅ Inseridos {totalInseridos:N0} registros | lote {batch.Count} | tempo: {stopwatchInsercao.ElapsedMilliseconds}ms | buffer: {configStream.Capacity}");

            batch.Clear();

            AjustarParametros(configStream, stopwatchInsercao.ElapsedMilliseconds);
        }
    }

    // Finaliza o que sobrou
    if (batch.Count > 0)
    {
        stopwatchInsercao.Start();
        await InserirLoteAsync(mysqlContext, batch);
        stopwatchInsercao.Stop();

        totalInseridos += batch.Count;
        Console.WriteLine($"✅ Inseridos {totalInseridos:N0} registros (final).");
    }
});

await Task.WhenAll(produtor, consumidor);
stopwatchTotal.Stop();


// ---------------- Relatório final ----------------
Console.WriteLine("\n===== Relatório de Migração =====");
Console.WriteLine($"Total registros lidos: {totalLidos:N0}");
Console.WriteLine($"Tempo total de leitura: {stopwatchLeitura.Elapsed}");
Console.WriteLine($"Tempo total de inserção: {stopwatchInsercao.Elapsed}");
Console.WriteLine($"Tempo total da migração: {stopwatchTotal.Elapsed}");


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

static void AjustarParametros(AutoTuningConfig config, long duracaoMs)
{
    // Se o lote foi processado muito rápido (< 200ms), aumentamos
    if (duracaoMs < 200)
    {
        config.BatchSize = Math.Min(config.BatchSize + 500, config.MaxBatchSize);
        config.Capacity = Math.Min(config.Capacity + 2000, config.MaxCapacity);
    }
    // Se foi muito lento (> 1000ms), reduzimos
    else if (duracaoMs > 1000)
    {
        config.BatchSize = Math.Max(config.BatchSize - 500, config.MinBatchSize);
        config.Capacity = Math.Max(config.Capacity - 2000, config.MinCapacity);
    }
}

record AutoTuningConfig
{
    public int BatchSize { get; set; }
    public int Capacity { get; set; }
    public int MinBatchSize { get; set; }
    public int MaxBatchSize { get; set; }
    public int MinCapacity { get; set; }
    public int MaxCapacity { get; set; }
}
using Microsoft.EntityFrameworkCore;
using MigrarDados.Data;
using MigrarDados.Models;
using System.Diagnostics;
using System.Threading.Channels;

namespace MigrarDados.Migration;

public sealed class MigrationRunner
{
    public async Task RunAsync(SQLiteContext sqliteContext, string mysqlConn, CancellationToken cancellationToken = default)
    {
        var metrics = new MigrationMetrics();
        var autoTuner = new AutoTuner(MigrationConstants.DefaultBatchSize);

        var channel = Channel.CreateBounded<Caged>(new BoundedChannelOptions(MigrationConstants.ChannelInitialCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        });

        var produtor = Task.Run(async () =>
        {
            metrics.ReadStopwatch.Start();

            await foreach (var item in sqliteContext.CagedSQLite.AsNoTracking().AsAsyncEnumerable().WithCancellation(cancellationToken))
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

                await channel.Writer.WriteAsync(novo, cancellationToken);
                metrics.AddRead();
            }

            metrics.ReadStopwatch.Stop();
            channel.Writer.Complete();
        }, cancellationToken);

        var consumidores = new List<Task>(capacity: MigrationConstants.ConsumerCount);

        for (var i = 0; i < MigrationConstants.ConsumerCount; i++)
        {
            consumidores.Add(Task.Run(async () =>
            {
                var batch = new List<Caged>();
                var stopwatchInsercaoLote = new Stopwatch();

                async Task FlushAsync(List<Caged> lote)
                {
                    stopwatchInsercaoLote.Restart();
                    await BatchInserter.InsertBatchAsync(mysqlConn, lote, cancellationToken);
                    stopwatchInsercaoLote.Stop();

                    metrics.AddInsertMilliseconds(stopwatchInsercaoLote.ElapsedMilliseconds);
                    var novoTotal = metrics.AddInserted(lote.Count);
                    var novoBatchSize = autoTuner.AdjustAfterBatch(stopwatchInsercaoLote.ElapsedMilliseconds);

                    Console.WriteLine(
                        $"✅ [Thread {Task.CurrentId}] Inseridos {novoTotal:N0} registros | lote {lote.Count} | tempo lote: {stopwatchInsercaoLote.ElapsedMilliseconds}ms | batch: {novoBatchSize}");
                }

                await foreach (var registro in channel.Reader.ReadAllAsync(cancellationToken))
                {
                    batch.Add(registro);

                    if (batch.Count >= autoTuner.CurrentBatchSize)
                    {
                        var lote = batch.ToList();
                        batch.Clear();
                        await FlushAsync(lote);
                    }
                }

                if (batch.Count > 0)
                {
                    var lote = batch.ToList();
                    batch.Clear();
                    await FlushAsync(lote);
                }
            }, cancellationToken));
        }

        await Task.WhenAll(produtor);
        await Task.WhenAll(consumidores);

        metrics.TotalStopwatch.Stop();

        Console.WriteLine("\n===== Relatorio de Migracao =====");
        Console.WriteLine($"Total registros lidos: {metrics.TotalRead:N0}");
        Console.WriteLine($"Tempo total de leitura: {metrics.ReadStopwatch.Elapsed}");
        Console.WriteLine($"Tempo total de insercao: {metrics.TotalInsertDuration}");
        Console.WriteLine($"Tempo total da migracao: {metrics.TotalStopwatch.Elapsed}");
    }
}

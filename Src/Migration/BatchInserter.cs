using MigrarDados.Data;
using MigrarDados.Models;

namespace MigrarDados.Migration;

internal static class BatchInserter
{
    public static async Task InsertBatchAsync(string mysqlConn, List<Caged> registros, CancellationToken cancellationToken = default)
    {
        using var context = new MySQLContext(mysqlConn);
        var tentativa = 0;

        while (tentativa < MigrationConstants.MaxInsertAttempts)
        {
            try
            {
                await context.Caged.AddRangeAsync(registros, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
                context.ChangeTracker.Clear();
                return;
            }
            catch (Exception ex)
            {
                tentativa++;
                Console.WriteLine($"⚠️ Erro ao inserir lote (tentativa {tentativa}/{MigrationConstants.MaxInsertAttempts}): {ex.Message}");

                if (tentativa >= MigrationConstants.MaxInsertAttempts)
                {
                    throw;
                }

                await Task.Delay(MigrationConstants.RetryBaseDelayMs * tentativa, cancellationToken);
            }
        }
    }
}

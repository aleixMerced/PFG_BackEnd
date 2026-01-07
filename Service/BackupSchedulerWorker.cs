using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using PFG_BackEnd.Helper;

namespace PFG_BackEnd.Service.Extern;

public class BackupSchedulerWorker : BackgroundService
{
    private readonly DbBackupService backupService;
    private readonly DbBackupOptions options;
    private readonly ILogger<BackupSchedulerWorker> logger;

    public BackupSchedulerWorker(DbBackupService backup, IOptions<DbBackupOptions> opt, ILogger<BackupSchedulerWorker> log)
    {
        backupService = backup;
        options = opt.Value;
        logger = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunIfDueAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al job de backup");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task RunIfDueAsync(CancellationToken ct)
    {
        await using var conn = new SqlConnection(options.ConnectionString);
        await conn.OpenAsync(ct);

        await EnsureMetaTableAsync(conn, ct);

        var last = await GetDiaBackUpAsync(conn, ct);
        if (last != null &&
            (DateTime.UtcNow - last.Value) < TimeSpan.FromDays(options.Frequency))
        {
            logger.LogInformation("No toca backup. Últim: {Last}", last);
            return;
        }

        var path = await backupService.RunBackupAsync(ct);
        await SetDiaBackUpAsync(conn, DateTime.UtcNow, ct);

        logger.LogInformation("Backup completat: {Path}", path);
    }

    private static async Task EnsureMetaTableAsync(SqlConnection conn, CancellationToken ct)
    {
        var sql = """
        IF OBJECT_ID('dbo.BACKUP_META','U') IS NULL
        CREATE TABLE dbo.BACKUP_META(
            Id int NOT NULL CONSTRAINT PK_BACKUP_META PRIMARY KEY,
            DiaBackUp datetime2 NULL
        );
        IF NOT EXISTS (SELECT 1 FROM dbo.BACKUP_META WHERE Id = 1)
            INSERT INTO dbo.BACKUP_META(Id, DiaBackUp) VALUES (1, NULL);
        """;
        await using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static async Task<DateTime?> GetDiaBackUpAsync(SqlConnection conn, CancellationToken ct)
    {
        await using var cmd = new SqlCommand(
            "SELECT DiaBackUp FROM dbo.BACKUP_META WHERE Id=1;", conn);
        var obj = await cmd.ExecuteScalarAsync(ct);
        return obj == null || obj == DBNull.Value ? null : (DateTime)obj;
    }

    private static async Task SetDiaBackUpAsync(
        SqlConnection conn, DateTime utc, CancellationToken ct)
    {
        await using var cmd = new SqlCommand(
            "UPDATE dbo.BACKUP_META SET DiaBackUp=@d WHERE Id=1;", conn);
        cmd.Parameters.AddWithValue("@d", utc);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
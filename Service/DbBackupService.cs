using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using PFG_BackEnd.Helper;

namespace PFG_BackEnd.Service.Extern;

public class DbBackupService
{
    private readonly DbBackupOptions options;
    private readonly ILogger<DbBackupService> logger;

    public DbBackupService(IOptions<DbBackupOptions> opt, ILogger<DbBackupService> log)
    {
        options = opt.Value;
        logger = log;
    }

    public async Task<string> RunBackupAsync(CancellationToken ct)
    {
        Directory.CreateDirectory(options.BackupFolder);

        var stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var path = Path.Combine(
            options.BackupFolder,
            $"{options.DatabaseName}_{stamp}.bak");

        await using var conn = new SqlConnection(options.ConnectionString);
        await conn.OpenAsync(ct);

        var sql = $"""
                   BACKUP DATABASE [{options.DatabaseName}]
                   TO DISK = @path
                   WITH INIT, CHECKSUM;
                   """;

        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add(
            new SqlParameter("@path", SqlDbType.NVarChar, 4000) { Value = path });

        cmd.CommandTimeout = 60 * 60;

        logger.LogInformation("Iniciant backup: {Path}", path);
        await cmd.ExecuteNonQueryAsync(ct);

        return path;
    }

}
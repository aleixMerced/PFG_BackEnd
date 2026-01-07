namespace PFG_BackEnd.Helper;

public class DbBackupOptions
{
    public string ConnectionString { get; set; } = "";
    public string DatabaseName { get; set; } = "";
    public string BackupFolder { get; set; } = "";
    public int Frequency { get; set; } = 15;
}
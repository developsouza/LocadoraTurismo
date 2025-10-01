using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using RentalTourismSystem.Models.Configuration;

namespace RentalTourismSystem.Services
{
    public interface IBackupService
    {
        Task<bool> CriarBackupAsync(string nomeBackup = "");
        Task<bool> RestaurarBackupAsync(string caminhoBackup);
        Task<List<string>> ListarBackupsAsync();
        Task<bool> LimparBackupsAntigosAsync();
    }

    public class BackupService : IBackupService
    {
        private readonly string _connectionString;
        private readonly AppSettings _appSettings;
        private readonly ILogger<BackupService> _logger;
        private readonly string _backupDirectory;

        public BackupService(IConfiguration configuration, IOptions<AppSettings> appSettings, ILogger<BackupService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _appSettings = appSettings.Value;
            _logger = logger;
            _backupDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Backups");

            if (!Directory.Exists(_backupDirectory))
                Directory.CreateDirectory(_backupDirectory);
        }

        public async Task<bool> CriarBackupAsync(string nomeBackup = "")
        {
            try
            {
                if (string.IsNullOrEmpty(nomeBackup))
                    nomeBackup = $"RentalTourism_Backup_{DateTime.Now:yyyyMMdd_HHmmss}";

                var caminhoBackup = Path.Combine(_backupDirectory, $"{nomeBackup}.bak");
                var databaseName = new SqlConnectionStringBuilder(_connectionString).InitialCatalog;

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = $@"
                    BACKUP DATABASE [{databaseName}] 
                    TO DISK = '{caminhoBackup}' 
                    WITH FORMAT, INIT, 
                    NAME = '{nomeBackup}', 
                    SKIP, NOREWIND, NOUNLOAD, STATS = 10";

                using var command = new SqlCommand(sql, connection);
                command.CommandTimeout = 300; // 5 minutos

                await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Backup criado com sucesso: {CaminhoBackup}", caminhoBackup);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar backup");
                return false;
            }
        }

        public async Task<bool> RestaurarBackupAsync(string caminhoBackup)
        {
            try
            {
                if (!File.Exists(caminhoBackup))
                {
                    _logger.LogWarning("Arquivo de backup não encontrado: {CaminhoBackup}", caminhoBackup);
                    return false;
                }

                var databaseName = new SqlConnectionStringBuilder(_connectionString).InitialCatalog;
                var masterConnectionString = _connectionString.Replace($"Initial Catalog={databaseName}", "Initial Catalog=master");

                using var connection = new SqlConnection(masterConnectionString);
                await connection.OpenAsync();

                // Colocar o banco em modo single user
                var sql = $@"
                    ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    
                    RESTORE DATABASE [{databaseName}] 
                    FROM DISK = '{caminhoBackup}' 
                    WITH REPLACE, STATS = 10;
                    
                    ALTER DATABASE [{databaseName}] SET MULTI_USER;";

                using var command = new SqlCommand(sql, connection);
                command.CommandTimeout = 600; // 10 minutos

                await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Backup restaurado com sucesso: {CaminhoBackup}", caminhoBackup);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao restaurar backup: {CaminhoBackup}", caminhoBackup);
                return false;
            }
        }

        public async Task<List<string>> ListarBackupsAsync()
        {
            return await Task.Run(() =>
            {
                return Directory.GetFiles(_backupDirectory, "*.bak")
                    .Select(Path.GetFileName)
                    .Where(f => f != null)
                    .Cast<string>()
                    .OrderByDescending(f => f)
                    .ToList();
            });
        }

        public async Task<bool> LimparBackupsAntigosAsync()
        {
            try
            {
                var dataLimite = DateTime.Now.AddDays(-_appSettings.BackupRetentionDays);
                var arquivos = Directory.GetFiles(_backupDirectory, "*.bak");

                foreach (var arquivo in arquivos)
                {
                    var dataArquivo = File.GetCreationTime(arquivo);
                    if (dataArquivo < dataLimite)
                    {
                        File.Delete(arquivo);
                        _logger.LogInformation("Backup antigo removido: {Arquivo}", arquivo);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao limpar backups antigos");
                return false;
            }
        }
    }
}
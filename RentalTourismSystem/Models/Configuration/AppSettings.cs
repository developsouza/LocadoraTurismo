namespace RentalTourismSystem.Models.Configuration
{
    public class AppSettings
    {
        public string ApplicationName { get; set; } = "";
        public string Version { get; set; } = "";
        public string SupportEmail { get; set; } = "";
        public int BackupRetentionDays { get; set; }
        public bool EnableDetailedErrors { get; set; }
        public int DefaultPageSize { get; set; }
        public long MaxFileUploadSize { get; set; }
        public string[] AllowedImageTypes { get; set; } = Array.Empty<string>();
        public string Currency { get; set; } = "";
        public string TimeZone { get; set; } = "";
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; } = "";
        public int SmtpPort { get; set; }
        public bool UseSsl { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string FromEmail { get; set; } = "";
        public string FromName { get; set; } = "";
    }
}
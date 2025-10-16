using AutoNightMode.Models;

namespace AutoNightMode.Services
{
    public class LoggingService
    {
        private readonly string _logPath;
        private readonly AppConfiguration _configuration;

        public LoggingService(AppConfiguration configuration)
        {
            _configuration = configuration;
            _logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "AutoNightMode",
                "logs",
                $"AutoNightMode_{DateTime.Now:yyyyMMdd}.log");
            
            EnsureLogDirectory();
        }

        private void EnsureLogDirectory()
        {
            var directory = Path.GetDirectoryName(_logPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }
        }

        public void LogInfo(string message)
        {
            if (ShouldLog("Info"))
            {
                WriteLog("INFO", message);
            }
        }

        public void LogWarning(string message)
        {
            if (ShouldLog("Warning"))
            {
                WriteLog("WARN", message);
            }
        }

        public void LogError(string message, Exception? exception = null)
        {
            if (ShouldLog("Error"))
            {
                var fullMessage = exception != null ? $"{message} - {exception}" : message;
                WriteLog("ERROR", fullMessage);
            }
        }

        public void LogDebug(string message)
        {
            if (ShouldLog("Debug") && _configuration.General.DebugMode)
            {
                WriteLog("DEBUG", message);
            }
        }

        private bool ShouldLog(string level)
        {
            var configLevel = _configuration.General.LogLevel;
            return configLevel switch
            {
                "Debug" => true,
                "Info" => level != "Debug",
                "Warning" => level == "Warning" || level == "Error",
                "Error" => level == "Error",
                _ => true
            };
        }

        private void WriteLog(string level, string message)
        {
            try
            {
                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
                File.AppendAllText(_logPath, logEntry + Environment.NewLine);
            }
            catch
            {
                // Ignore logging errors to prevent infinite loops
            }
        }

        public void CleanOldLogs(int daysToKeep = 30)
        {
            try
            {
                var logDirectory = Path.GetDirectoryName(_logPath);
                if (Directory.Exists(logDirectory))
                {
                    var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
                    var logFiles = Directory.GetFiles(logDirectory, "AutoNightMode_*.log");
                    
                    foreach (var file in logFiles)
                    {
                        var fileInfo = new FileInfo(file);
                        if (fileInfo.CreationTime < cutoffDate)
                        {
                            fileInfo.Delete();
                        }
                    }
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        public string GetLogPath() => _logPath;
    }
}

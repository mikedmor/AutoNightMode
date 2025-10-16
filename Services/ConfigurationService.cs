using AutoNightMode.Models;
using Newtonsoft.Json;

namespace AutoNightMode.Services
{
    public class ConfigurationService
    {
        private readonly string _configPath;
        private AppConfiguration _configuration = new AppConfiguration();

        public ConfigurationService()
        {
            _configPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "AutoNightMode",
                "config.json");
            
            EnsureConfigDirectory();
            LoadConfiguration();
        }

        public AppConfiguration Configuration => _configuration;

        public event EventHandler<AppConfiguration>? ConfigurationChanged;

        private void EnsureConfigDirectory()
        {
            var directory = Path.GetDirectoryName(_configPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }
        }

        public void LoadConfiguration()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    var json = File.ReadAllText(_configPath);
                    _configuration = JsonConvert.DeserializeObject<AppConfiguration>(json) ?? new AppConfiguration();
                }
                else
                {
                    _configuration = new AppConfiguration();
                    SaveConfiguration(); // Create default config file
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading configuration: {ex.Message}\n\nUsing default settings.", 
                    "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _configuration = new AppConfiguration();
            }
        }

        public void SaveConfiguration()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_configuration, Formatting.Indented);
                File.WriteAllText(_configPath, json);
                ConfigurationChanged?.Invoke(this, _configuration);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving configuration: {ex.Message}", 
                    "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void UpdateConfiguration(AppConfiguration newConfiguration)
        {
            _configuration = newConfiguration;
            SaveConfiguration();
        }

        public void ResetToDefault()
        {
            _configuration = new AppConfiguration();
            SaveConfiguration();
        }

        public string GetConfigurationPath() => _configPath;
    }
}

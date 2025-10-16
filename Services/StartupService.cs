using Microsoft.Win32;

namespace AutoNightMode.Services
{
    public class StartupService
    {
        private const string REGISTRY_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string APP_NAME = "AutoNightModeController";
        
        public bool IsStartupEnabled()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY);
                var value = key?.GetValue(APP_NAME);
                return value != null;
            }
            catch
            {
                return false;
            }
        }

        public bool SetStartupEnabled(bool enabled)
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY, true);
                if (key == null)
                    return false;

                if (enabled)
                {
                    var exePath = Application.ExecutablePath;
                    key.SetValue(APP_NAME, $"\"{exePath}\"");
                }
                else
                {
                    key.DeleteValue(APP_NAME, false);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting startup configuration: {ex.Message}", 
                    "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        public string GetStartupPath()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY);
                var value = key?.GetValue(APP_NAME) as string;
                return value?.Trim('"') ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}

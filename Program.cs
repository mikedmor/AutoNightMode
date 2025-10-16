using AutoNightMode.Forms;
using AutoNightMode.Services;

namespace AutoNightMode
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Global exception handler
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                File.WriteAllText("error.log", e.ExceptionObject.ToString());
                MessageBox.Show("An unexpected error occurred. Check error.log for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            // Check if application is already running
            bool createdNew;
            using (var mutex = new Mutex(true, "AutoNightModeController", out createdNew))
            {
                if (!createdNew)
                {
                    MessageBox.Show("Auto Night Mode Controller is already running.", "Already Running", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Initialize services
                var configService = new ConfigurationService();
                var schedulerService = new SchedulerService();
                var nightModeService = new NightModeService();
                var startupService = new StartupService();

                // Create and run the main form (system tray)
                var mainForm = new MainForm(configService, schedulerService, nightModeService, startupService);
                
                // Keep mutex alive for the lifetime of the application
                Application.Run(mainForm);
            }
        }
    }
}

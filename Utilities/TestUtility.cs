using AutoNightMode.Services;
using AutoNightMode.Models;

namespace AutoNightMode.Utilities
{
    public static class TestUtility
    {
        public static async Task RunDiagnostics()
        {
            Console.WriteLine("Auto Night Mode Controller - Diagnostics");
            Console.WriteLine("========================================");
            Console.WriteLine();

            // Test 1: Configuration Service
            Console.WriteLine("1. Testing Configuration Service...");
            try
            {
                var configService = new ConfigurationService();
                Console.WriteLine($"   ✓ Configuration loaded from: {configService.GetConfigurationPath()}");
                Console.WriteLine($"   ✓ Schedule enabled: {configService.Configuration.Schedule.EnableSchedule}");
                Console.WriteLine($"   ✓ Start time: {configService.Configuration.Schedule.NightModeStartTime}");
                Console.WriteLine($"   ✓ End time: {configService.Configuration.Schedule.NightModeEndTime}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ Configuration test failed: {ex.Message}");
            }
            Console.WriteLine();

            // Test 2: Night Mode Service
            Console.WriteLine("2. Testing Night Mode Service...");
            try
            {
                var configService = new ConfigurationService();
                var nightModeService = new NightModeService();
                nightModeService.Initialize(configService.Configuration);
                
                Console.WriteLine($"   ✓ Night Mode Service initialized");
                Console.WriteLine($"   ✓ Controller type: {configService.Configuration.Controller.ControllerType}");
                Console.WriteLine($"   ✓ Toggle method: {(configService.Configuration.Controller.UseKeyboardEmulation ? "Keyboard" : "Serial")}");
                
                if (configService.Configuration.Controller.UseKeyboardEmulation)
                {
                    Console.WriteLine($"   ✓ Toggle key: {configService.Configuration.Controller.ToggleKeyCode}");
                }
                else
                {
                    Console.WriteLine($"   ✓ Serial port: {configService.Configuration.Controller.ControllerPort}");
                    Console.WriteLine($"   ✓ Baud rate: {configService.Configuration.Controller.ControllerBaudRate}");
                }

                var currentStatus = await nightModeService.CheckCurrentNightModeStatus();
                Console.WriteLine($"   ✓ Current night mode status: {currentStatus}");
                
                nightModeService.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ Night Mode Service test failed: {ex.Message}");
            }
            Console.WriteLine();

            // Test 3: Scheduler Service
            Console.WriteLine("3. Testing Scheduler Service...");
            try
            {
                var configService = new ConfigurationService();
                var schedulerService = new SchedulerService();
                schedulerService.Initialize(configService.Configuration);
                
                Console.WriteLine($"   ✓ Scheduler Service initialized");
                Console.WriteLine($"   ✓ Check interval: {configService.Configuration.General.CheckInterval} seconds");
                
                var nextEvent = schedulerService.GetNextScheduledEvent();
                Console.WriteLine($"   ✓ Next scheduled event: {nextEvent}");
                
                schedulerService.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ Scheduler Service test failed: {ex.Message}");
            }
            Console.WriteLine();

            // Test 4: Startup Service
            Console.WriteLine("4. Testing Startup Service...");
            try
            {
                var startupService = new StartupService();
                var isEnabled = startupService.IsStartupEnabled();
                Console.WriteLine($"   ✓ Startup Service initialized");
                Console.WriteLine($"   ✓ Start with Windows: {isEnabled}");
                
                if (isEnabled)
                {
                    var startupPath = startupService.GetStartupPath();
                    Console.WriteLine($"   ✓ Startup path: {startupPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ Startup Service test failed: {ex.Message}");
            }
            Console.WriteLine();

            // Test 5: System Information
            Console.WriteLine("5. System Information...");
            try
            {
                Console.WriteLine($"   ✓ OS Version: {Environment.OSVersion}");
                Console.WriteLine($"   ✓ .NET Version: {Environment.Version}");
                Console.WriteLine($"   ✓ Machine Name: {Environment.MachineName}");
                Console.WriteLine($"   ✓ User Name: {Environment.UserName}");
                Console.WriteLine($"   ✓ Working Directory: {Environment.CurrentDirectory}");
                Console.WriteLine($"   ✓ Application Data: {Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ System Information test failed: {ex.Message}");
            }
            Console.WriteLine();

            Console.WriteLine("Diagnostics completed!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public static async Task TestNightModeToggle()
        {
            Console.WriteLine("Night Mode Toggle Test");
            Console.WriteLine("=====================");
            Console.WriteLine();

            try
            {
                var configService = new ConfigurationService();
                var nightModeService = new NightModeService();
                nightModeService.Initialize(configService.Configuration);

                Console.WriteLine("Current configuration:");
                Console.WriteLine($"  Controller: {configService.Configuration.Controller.ControllerType}");
                Console.WriteLine($"  Method: {(configService.Configuration.Controller.UseKeyboardEmulation ? "Keyboard" : "Serial")}");
                
                if (configService.Configuration.Controller.UseKeyboardEmulation)
                {
                    Console.WriteLine($"  Key: {configService.Configuration.Controller.ToggleKeyCode}");
                    Console.WriteLine($"  Delay: {configService.Configuration.Controller.ToggleDelay}ms");
                }
                else
                {
                    Console.WriteLine($"  Port: {configService.Configuration.Controller.ControllerPort}");
                    Console.WriteLine($"  Baud: {configService.Configuration.Controller.ControllerBaudRate}");
                }
                Console.WriteLine();

                Console.WriteLine("Checking current night mode status...");
                var currentStatus = await nightModeService.CheckCurrentNightModeStatus();
                Console.WriteLine($"Current status: {currentStatus}");
                Console.WriteLine();

                Console.WriteLine("Press 'T' to test toggle, or any other key to exit...");
                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.KeyChar == 't' || key.KeyChar == 'T')
                {
                    Console.WriteLine("Sending toggle command...");
                    var result = await nightModeService.ToggleNightMode(true);
                    Console.WriteLine($"Toggle result: {(result ? "Success" : "Failed")}");
                    
                    Console.WriteLine();
                    Console.WriteLine("Waiting 2 seconds before checking status again...");
                    await Task.Delay(2000);
                    
                    var newStatus = await nightModeService.CheckCurrentNightModeStatus();
                    Console.WriteLine($"New status: {newStatus}");
                }

                nightModeService.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}

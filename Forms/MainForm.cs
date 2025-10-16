using AutoNightMode.Models;
using AutoNightMode.Services;

namespace AutoNightMode.Forms
{
    public partial class MainForm : Form
    {
        private readonly ConfigurationService _configService;
        private readonly SchedulerService _schedulerService;
        private readonly NightModeService _nightModeService;
        private readonly StartupService _startupService;
        private readonly SunsetService _sunsetService = new SunsetService(); // Add SunsetService

        private NotifyIcon? _notifyIcon;
        private ContextMenuStrip? _contextMenu;
        private System.Timers.Timer? _statusUpdateTimer;

        public MainForm(ConfigurationService configService, SchedulerService schedulerService, 
            NightModeService nightModeService, StartupService startupService)
        {
            _configService = configService;
            _schedulerService = schedulerService;
            _nightModeService = nightModeService;
            _startupService = startupService;

            InitializeComponent();
            
            // This is a tray-only application - the form itself should NEVER be visible
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.Opacity = 0;
            
            InitializeSystemTray();
            InitializeServices();
        }

        private void InitializeSystemTray()
        {
            // Create context menu
            _contextMenu = new ContextMenuStrip();
            _contextMenu.Items.Add("Show Settings", null, ShowSettings_Click);
            _contextMenu.Items.Add("Serial Monitor", null, ShowSerialMonitor_Click);
            _contextMenu.Items.Add("Toggle Night Mode", null, ToggleNightMode_Click);
            _contextMenu.Items.Add("-");
            _contextMenu.Items.Add("Enable/Disable Scheduler", null, ToggleScheduler_Click);
            _contextMenu.Items.Add("-");
            _contextMenu.Items.Add("Exit", null, Exit_Click);

            // Create notify icon
            _notifyIcon = new NotifyIcon
            {
                Icon = new Icon("icon.ico"), // Use the custom icon file
                Text = "Auto Night Mode Controller",
                Visible = true,
                ContextMenuStrip = _contextMenu
            };

            _notifyIcon.DoubleClick += ShowSettings_Click;
            
            // Show settings on startup if requested
            if (_configService.Configuration.General.ShowSettingsOnStartup)
            {
                System.Windows.Forms.Application.DoEvents();  // Process tray icon first
                ShowSettings_Click(this, EventArgs.Empty);
            }
            else if (_configService.Configuration.General.ShowNotifications)
            {
                // Show startup notification only if not showing settings
                _notifyIcon.ShowBalloonTip(2000, "Auto Night Mode", "Running in system tray", ToolTipIcon.Info);
            }
        }

        private void InitializeServices()
        {
            // Initialize services with current configuration
            _nightModeService.Initialize(_configService.Configuration);
            _schedulerService.Initialize(_configService.Configuration);

            // Provide sunset service to scheduler so it can compute sunset-based schedules
            _schedulerService.SetSunsetService(_sunsetService);

            // Wire up events
            _schedulerService.NightModeScheduleTriggered += OnScheduleTriggered;
            _schedulerService.StatusChanged += OnSchedulerStatusChanged;
            _nightModeService.StateChanged += OnNightModeStateChanged;
            _nightModeService.StatusMessage += OnNightModeStatusMessage;
            _configService.ConfigurationChanged += OnConfigurationChanged;

            // Start status update timer
            _statusUpdateTimer = new System.Timers.Timer(5000); // Update every 5 seconds
            _statusUpdateTimer.Elapsed += UpdateStatusDisplay;
            _statusUpdateTimer.Start();

            // Update startup setting
            _startupService.SetStartupEnabled(_configService.Configuration.General.StartWithWindows);
        }

        private async void OnScheduleTriggered(object? sender, bool enableNightMode)
        {
            await _nightModeService.HandleScheduledToggle(enableNightMode);
        }

        private void OnSchedulerStatusChanged(object? sender, string status)
        {
            if (_configService.Configuration.General.ShowNotifications)
            {
                _notifyIcon?.ShowBalloonTip(3000, "Scheduler", status, ToolTipIcon.Info);
            }
        }

        private void OnNightModeStateChanged(object? sender, NightModeState state)
        {
            var status = state.Status switch
            {
                NightModeStatus.Enabled => "Night Mode: ON",
                NightModeStatus.Disabled => "Night Mode: OFF",
                _ => "Night Mode: Unknown"
            };

            if (_notifyIcon != null)
            {
                _notifyIcon.Text = $"Auto Night Mode Controller - {status}";
            }

            if (_configService.Configuration.General.ShowNotifications && state.LastToggled.HasValue)
            {
                var message = $"Night mode {(state.Status == NightModeStatus.Enabled ? "enabled" : "disabled")} by {state.Source}";
                _notifyIcon?.ShowBalloonTip(3000, "Night Mode", message, ToolTipIcon.Info);
            }
        }

        private void OnNightModeStatusMessage(object? sender, string message)
        {
            if (_configService.Configuration.General.DebugMode)
            {
                _notifyIcon?.ShowBalloonTip(2000, "Debug", message, ToolTipIcon.Info);
            }
        }

        private void OnConfigurationChanged(object? sender, AppConfiguration configuration)
        {
            _nightModeService.UpdateConfiguration(configuration);
            _schedulerService.UpdateConfiguration(configuration);
            _startupService.SetStartupEnabled(configuration.General.StartWithWindows);
        }

        private void UpdateStatusDisplay(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateStatusDisplay(sender, e)));
                return;
            }

            try
            {
                // Update tooltip with current status
                var nextEvent = _schedulerService.GetNextScheduledEvent();
                var currentState = _nightModeService.GetCurrentState();
                
                var tooltip = $"Auto Night Mode Controller\n" +
                             $"Status: {currentState.Status}\n" +
                             $"Next: {nextEvent}";
                
                if (_notifyIcon != null)
                {
                    _notifyIcon.Text = tooltip.Length > 63 ? tooltip.Substring(0, 60) + "..." : tooltip;
                }
            }
            catch
            {
                // Ignore errors in status update
            }
        }

        private void ShowSettings_Click(object? sender, EventArgs e)
        {
            var settingsForm = new SettingsForm(_configService);
            settingsForm.ShowDialog();
        }

        private void ShowSerialMonitor_Click(object? sender, EventArgs e)
        {
            var serialMonitorForm = new SerialMonitorForm();
            serialMonitorForm.Show();
        }

        private async void ToggleNightMode_Click(object? sender, EventArgs e)
        {
            try
            {
                var currentState = _nightModeService.GetCurrentState();
                var newState = currentState.Status != NightModeStatus.Enabled;
                
                await _nightModeService.ToggleNightMode(newState);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error toggling night mode: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToggleScheduler_Click(object? sender, EventArgs e)
        {
            var config = _configService.Configuration;
            config.Schedule.EnableSchedule = !config.Schedule.EnableSchedule;
            _configService.SaveConfiguration();

            if (config.Schedule.EnableSchedule)
            {
                _schedulerService.Start();
            }
            else
            {
                _schedulerService.Stop();
            }
        }

        private void Exit_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void SetVisibleCore(bool value)
        {
            // Form should always stay hidden - this is a tray-only app
            base.SetVisibleCore(false);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                ShowInTaskbar = false;
                
                if (_configService.Configuration.General.ShowNotifications)
                {
                    _notifyIcon?.ShowBalloonTip(2000, "Auto Night Mode", 
                        "Application was minimized to tray", ToolTipIcon.Info);
                }
            }
            else
            {
                _statusUpdateTimer?.Stop();
                _statusUpdateTimer?.Dispose();
                _schedulerService?.Stop();
                _nightModeService?.Dispose();
                _notifyIcon?.Dispose();
                base.OnFormClosing(e);
            }
        }

        private async void FetchAndDisplaySunsetTime()
        {
            var latitude = 36.7201600; // Example latitude
            var longitude = -4.4203400; // Example longitude

            var (sunset, error) = await _sunsetService.GetSunsetTimeAsync(latitude, longitude);

            if (sunset != null)
            {
                MessageBox.Show($"Today's sunset time: {sunset}", "Sunset Time", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Failed to fetch sunset time: {error}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

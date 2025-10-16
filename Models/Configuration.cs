using Newtonsoft.Json;

namespace AutoNightMode.Models
{
    public class AppConfiguration
    {
        public ScheduleSettings Schedule { get; set; } = new ScheduleSettings();
        public ControllerSettings Controller { get; set; } = new ControllerSettings();
        public GeneralSettings General { get; set; } = new GeneralSettings();
    }

    public class ScheduleSettings
    {
        public bool EnableSchedule { get; set; } = true;
        public TimeSpan NightModeStartTime { get; set; } = new TimeSpan(22, 0, 0); // 10:00 PM
        public TimeSpan NightModeEndTime { get; set; } = new TimeSpan(8, 0, 0);   // 8:00 AM
        public List<DayOfWeek> ActiveDays { get; set; } = new List<DayOfWeek>
        {
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
            DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
        };
        public bool UseCustomSchedule { get; set; } = false;
        public List<CustomScheduleEntry> CustomSchedules { get; set; } = new List<CustomScheduleEntry>();
        public bool EnablePerDaySchedule { get; set; } = false; // New setting to enable per-day schedules
        public Dictionary<DayOfWeek, TimeSpan> PerDayStartTimes { get; set; } = new(); // Start times for each day
        public Dictionary<DayOfWeek, TimeSpan> PerDayEndTimes { get; set; } = new(); // End times for each day

        // Sunset integration
        public bool UseSunset { get; set; } = false; // Use sunset as the night mode start time
        public double SunsetLatitude { get; set; } = 0.0; // Default coords (unset)
        public double SunsetLongitude { get; set; } = 0.0;
        public int SunsetOffsetMinutes { get; set; } = 0; // Minutes to add/subtract from sunset
    }

    public class CustomScheduleEntry
    {
        public string Name { get; set; } = string.Empty;
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool Enabled { get; set; } = true;
    }

    public class ControllerSettings
    {
        public string ControllerType { get; set; } = "ArnozDudeCab";
        public string ToggleKeyCode { get; set; } = "F12"; // Default key to emulate
        public int ToggleDelay { get; set; } = 500; // Milliseconds
        public string ControllerPort { get; set; } = "COM1";
        public int ControllerBaudRate { get; set; } = 9600;
        public bool UseSerialCommunication { get; set; } = false;
        public bool UseKeyboardEmulation { get; set; } = true;
    }

    public class GeneralSettings
    {
        public bool StartWithWindows { get; set; } = false;
        public bool ShowSettingsOnStartup { get; set; } = false;  // Renamed from StartMinimized
        public bool ShowNotifications { get; set; } = false;
        public int CheckInterval { get; set; } = 30; // Seconds
        public bool DebugMode { get; set; } = false;
        public string LogLevel { get; set; } = "Info";
    }

    public enum NightModeStatus
    {
        Unknown,
        Enabled,
        Disabled
    }

    public class NightModeState
    {
        public NightModeStatus Status { get; set; } = NightModeStatus.Unknown;
        public DateTime LastChecked { get; set; } = DateTime.Now;
        public DateTime? LastToggled { get; set; }
        public string Source { get; set; } = "Unknown"; // "Manual", "Scheduled", "Application"
    }
}

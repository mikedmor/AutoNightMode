using AutoNightMode.Models;

namespace AutoNightMode.Services
{
    public class SchedulerService
    {
        private readonly System.Timers.Timer _checkTimer;
        private AppConfiguration? _configuration;
        
        public event EventHandler<bool>? NightModeScheduleTriggered;
        public event EventHandler<string>? StatusChanged;

        private bool? _lastNightModeState; // Track the last state to prevent repeated triggers

        private SunsetService? _sunsetService;

        public SchedulerService()
        {
            _checkTimer = new System.Timers.Timer();
            _checkTimer.Elapsed += OnTimerElapsed;
            _checkTimer.AutoReset = true;
        }

        public void Initialize(AppConfiguration configuration)
        {
            _configuration = configuration;
            UpdateCheckInterval();
            Start();
        }

        public void UpdateConfiguration(AppConfiguration configuration)
        {
            _configuration = configuration;
            UpdateCheckInterval();
        }

        private void UpdateCheckInterval()
        {
            if (_configuration != null)
            {
                _checkTimer.Interval = _configuration.General.CheckInterval * 1000; // Convert to milliseconds
            }
        }

        public void Start()
        {
            if (_configuration?.Schedule.EnableSchedule == true)
            {
                _checkTimer.Start();
                StatusChanged?.Invoke(this, "Scheduler started");
            }
        }

        public void Stop()
        {
            _checkTimer.Stop();
            StatusChanged?.Invoke(this, "Scheduler stopped");
        }

        // Allow SunsetService injection/assignment if caller wants sunset-based scheduling
        public void SetSunsetService(SunsetService sunsetService)
        {
            _sunsetService = sunsetService;
        }

        private async void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (_configuration == null || !_configuration.Schedule.EnableSchedule)
                return;

            var now = DateTime.Now;
            var today = now.DayOfWeek;

            bool shouldEnableNightMode = false;

            // Priority 1: Per-day schedules (if enabled)
            if (_configuration.Schedule.EnablePerDaySchedule && 
                _configuration.Schedule.PerDayStartTimes.ContainsKey(today) && 
                _configuration.Schedule.PerDayEndTimes.ContainsKey(today))
            {
                var start = _configuration.Schedule.PerDayStartTimes[today];
                var end = _configuration.Schedule.PerDayEndTimes[today];
                
                if (start > end)
                {
                    // Overnight schedule
                    shouldEnableNightMode = now.TimeOfDay >= start || now.TimeOfDay <= end;
                }
                else
                {
                    shouldEnableNightMode = now.TimeOfDay >= start && now.TimeOfDay <= end;
                }
            }
            // Priority 2: Custom schedules
            else if (_configuration.Schedule.UseCustomSchedule)
            {
                var customSchedule = _configuration.Schedule.CustomSchedules
                    .FirstOrDefault(s => s.DayOfWeek == today && s.Enabled);

                if (customSchedule != null)
                {
                    shouldEnableNightMode = now.TimeOfDay >= customSchedule.StartTime && now.TimeOfDay < customSchedule.EndTime;
                }
            }
            // Priority 3: Sunset-based schedule
            else if (_configuration.Schedule.UseSunset && _sunsetService != null && _configuration.Schedule.SunsetLatitude != 0 && _configuration.Schedule.SunsetLongitude != 0)
            {
                // Compute sunset-based start time with offset (async)
                try
                {
                    var (sunset, err) = await _sunsetService.GetSunsetTimeAsync(_configuration.Schedule.SunsetLatitude, _configuration.Schedule.SunsetLongitude);
                    if (sunset != null)
                    {
                        var start = sunset.Value.Add(TimeSpan.FromMinutes(_configuration.Schedule.SunsetOffsetMinutes));
                        var end = _configuration.Schedule.NightModeEndTime; // End still from config

                        if (start > end)
                        {
                            shouldEnableNightMode = now.TimeOfDay >= start || now.TimeOfDay <= end;
                        }
                        else
                        {
                            shouldEnableNightMode = now.TimeOfDay >= start && now.TimeOfDay <= end;
                        }
                    }
                    else
                    {
                        // Fallback to default behavior if sunset fetch failed
                        if (_configuration.Schedule.ActiveDays.Contains(today))
                        {
                            shouldEnableNightMode = now.TimeOfDay >= _configuration.Schedule.NightModeStartTime && now.TimeOfDay < _configuration.Schedule.NightModeEndTime;
                        }
                    }
                }
                catch
                {
                    if (_configuration.Schedule.ActiveDays.Contains(today))
                    {
                        shouldEnableNightMode = now.TimeOfDay >= _configuration.Schedule.NightModeStartTime && now.TimeOfDay < _configuration.Schedule.NightModeEndTime;
                    }
                }
            }
            else if (_configuration.Schedule.ActiveDays.Contains(today))
            {
                shouldEnableNightMode = now.TimeOfDay >= _configuration.Schedule.NightModeStartTime && now.TimeOfDay < _configuration.Schedule.NightModeEndTime;
            }

            if (_lastNightModeState != shouldEnableNightMode)
            {
                _lastNightModeState = shouldEnableNightMode;
                NightModeScheduleTriggered?.Invoke(this, shouldEnableNightMode);
            }
        }

        private void CheckSchedule()
        {
            if (_configuration?.Schedule == null || !_configuration.Schedule.EnableSchedule)
                return;

            var now = DateTime.Now;
            var currentTime = now.TimeOfDay;
            var currentDay = now.DayOfWeek;

            bool shouldBeNightMode = false;

            if (_configuration.Schedule.UseCustomSchedule)
            {
                // Check custom schedules
                shouldBeNightMode = CheckCustomSchedules(currentDay, currentTime);
            }
            else
            {
                // Check default schedule
                shouldBeNightMode = CheckDefaultSchedule(currentDay, currentTime);
            }

            // Only trigger if the computed desired state differs from the last known scheduled state
            if (_lastNightModeState != shouldBeNightMode)
            {
                _lastNightModeState = shouldBeNightMode;
                NightModeScheduleTriggered?.Invoke(this, shouldBeNightMode);
            }
        }

        private bool CheckDefaultSchedule(DayOfWeek currentDay, TimeSpan currentTime)
        {
            if (!_configuration!.Schedule.ActiveDays.Contains(currentDay))
                return false;

            var startTime = _configuration.Schedule.NightModeStartTime;
            var endTime = _configuration.Schedule.NightModeEndTime;

            // Handle overnight schedules (e.g., 10 PM to 8 AM)
            if (startTime > endTime)
            {
                return currentTime >= startTime || currentTime <= endTime;
            }
            else
            {
                return currentTime >= startTime && currentTime <= endTime;
            }
        }

        private bool CheckCustomSchedules(DayOfWeek currentDay, TimeSpan currentTime)
        {
            var activeSchedules = _configuration!.Schedule.CustomSchedules
                .Where(s => s.Enabled && s.DayOfWeek == currentDay);

            foreach (var schedule in activeSchedules)
            {
                bool inSchedule;
                if (schedule.StartTime > schedule.EndTime)
                {
                    // Overnight schedule
                    inSchedule = currentTime >= schedule.StartTime || currentTime <= schedule.EndTime;
                }
                else
                {
                    inSchedule = currentTime >= schedule.StartTime && currentTime <= schedule.EndTime;
                }

                if (inSchedule)
                    return true;
            }

            return false;
        }

        private bool IsScheduleTransitionTime(TimeSpan currentTime)
        {
            // Only trigger at transition times to avoid constant checking
            // Check if we're within 1 minute of a start or end time
            var tolerance = TimeSpan.FromMinutes(1);

            if (_configuration!.Schedule.UseCustomSchedule)
            {
                foreach (var schedule in _configuration.Schedule.CustomSchedules.Where(s => s.Enabled))
                {
                    if (IsWithinTolerance(currentTime, schedule.StartTime, tolerance) ||
                        IsWithinTolerance(currentTime, schedule.EndTime, tolerance))
                        return true;
                }
            }
            else
            {
                if (IsWithinTolerance(currentTime, _configuration.Schedule.NightModeStartTime, tolerance) ||
                    IsWithinTolerance(currentTime, _configuration.Schedule.NightModeEndTime, tolerance))
                    return true;
            }

            return false;
        }

        private bool IsWithinTolerance(TimeSpan current, TimeSpan target, TimeSpan tolerance)
        {
            var diff = (current - target).Duration();
            return diff <= tolerance;
        }

        public string GetNextScheduledEvent()
        {
            if (_configuration?.Schedule == null || !_configuration.Schedule.EnableSchedule)
                return "Scheduling disabled";

            var now = DateTime.Now;
            
            // This is a simplified version - you might want to make this more sophisticated
            if (_configuration.Schedule.UseCustomSchedule)
            {
                return "Custom schedule active";
            }
            else
            {
                var today = now.Date;
                var startDateTime = today.Add(_configuration.Schedule.NightModeStartTime);
                var endDateTime = today.Add(_configuration.Schedule.NightModeEndTime);

                if (_configuration.Schedule.NightModeStartTime > _configuration.Schedule.NightModeEndTime)
                {
                    // Overnight schedule
                    if (now.TimeOfDay < _configuration.Schedule.NightModeEndTime)
                    {
                        return $"Night mode ends at {_configuration.Schedule.NightModeEndTime:hh\\:mm}";
                    }
                    else
                    {
                        return $"Night mode starts at {_configuration.Schedule.NightModeStartTime:hh\\:mm}";
                    }
                }
                else
                {
                    if (now < startDateTime)
                    {
                        return $"Night mode starts at {_configuration.Schedule.NightModeStartTime:hh\\:mm}";
                    }
                    else if (now < endDateTime)
                    {
                        return $"Night mode ends at {_configuration.Schedule.NightModeEndTime:hh\\:mm}";
                    }
                    else
                    {
                        startDateTime = startDateTime.AddDays(1);
                        return $"Night mode starts tomorrow at {_configuration.Schedule.NightModeStartTime:hh\\:mm}";
                    }
                }
            }
        }

        public void Dispose()
        {
            _checkTimer?.Dispose();
        }
    }
}

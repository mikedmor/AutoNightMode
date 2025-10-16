using AutoNightMode.Models;
using AutoNightMode.Services;

namespace AutoNightMode.Forms
{
    public partial class SettingsForm : Form
    {
        private readonly ConfigurationService _configService;
        private AppConfiguration _workingConfiguration;

        public SettingsForm(ConfigurationService configService)
        {
            _configService = configService;
            _workingConfiguration = CloneConfiguration(_configService.Configuration);
            
            InitializeComponent();
            LoadConfigurationToControls();
        }

        private AppConfiguration CloneConfiguration(AppConfiguration original)
        {
            // Simple clone - in production you might want a more robust solution
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(original);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<AppConfiguration>(json)!;
        }

        private static readonly Dictionary<string, (double Lat, double Lon)> CityLookup = new()
        {
            ["Auto detect"] = (0.0, 0.0),
            // United States - Major Cities
            ["New York, NY"] = (40.7128, -74.0060),
            ["Los Angeles, CA"] = (34.0522, -118.2437),
            ["Chicago, IL"] = (41.8781, -87.6298),
            ["Houston, TX"] = (29.7604, -95.3698),
            ["Phoenix, AZ"] = (33.4484, -112.0740),
            ["Philadelphia, PA"] = (39.9526, -75.1652),
            ["San Antonio, TX"] = (29.4241, -98.4936),
            ["San Diego, CA"] = (32.7157, -117.1611),
            ["Dallas, TX"] = (32.7767, -96.7970),
            ["San Jose, CA"] = (37.3382, -121.8863),
            ["Austin, TX"] = (30.2672, -97.7431),
            ["Jacksonville, FL"] = (30.3322, -81.6557),
            ["Fort Worth, TX"] = (32.7555, -97.3308),
            ["Columbus, OH"] = (39.9612, -82.9988),
            ["San Francisco, CA"] = (37.7749, -122.4194),
            ["Charlotte, NC"] = (35.2271, -80.8431),
            ["Indianapolis, IN"] = (39.7684, -86.1581),
            ["Seattle, WA"] = (47.6062, -122.3321),
            ["Denver, CO"] = (39.7392, -104.9903),
            ["Washington, DC"] = (38.9072, -77.0369),
            ["Boston, MA"] = (42.3601, -71.0589),
            ["Nashville, TN"] = (36.1627, -86.7816),
            ["Detroit, MI"] = (42.3314, -83.0458),
            ["Portland, OR"] = (45.5152, -122.6784),
            ["Las Vegas, NV"] = (36.1699, -115.1398),
            ["Miami, FL"] = (25.7617, -80.1918),
            ["Atlanta, GA"] = (33.7490, -84.3880),
            // International - Major Cities
            ["London, UK"] = (51.5074, -0.1278),
            ["Paris, France"] = (48.8566, 2.3522),
            ["Tokyo, Japan"] = (35.6762, 139.6503),
            ["Sydney, Australia"] = (-33.8688, 151.2093),
            ["Berlin, Germany"] = (52.5200, 13.4050),
            ["Toronto, Canada"] = (43.6532, -79.3832),
            ["Mexico City, Mexico"] = (19.4326, -99.1332),
            ["Madrid, Spain"] = (40.4168, -3.7038),
            ["Rome, Italy"] = (41.9028, 12.4964),
            ["Amsterdam, Netherlands"] = (52.3676, 4.9041),
            ["Dubai, UAE"] = (25.2048, 55.2708),
            ["Singapore"] = (1.3521, 103.8198),
            ["Hong Kong"] = (22.3193, 114.1694),
            ["Mumbai, India"] = (19.0760, 72.8777),
            ["São Paulo, Brazil"] = (-23.5505, -46.6333),
        };

        private void LoadConfigurationToControls()
        {
            // Schedule Settings
            chkEnableSchedule.Checked = _workingConfiguration.Schedule.EnableSchedule;
            dtpStartTime.Value = DateTime.Today.Add(_workingConfiguration.Schedule.NightModeStartTime);
            dtpEndTime.Value = DateTime.Today.Add(_workingConfiguration.Schedule.NightModeEndTime);
            
            // Active Days
            chkMonday.Checked = _workingConfiguration.Schedule.ActiveDays.Contains(DayOfWeek.Monday);
            chkTuesday.Checked = _workingConfiguration.Schedule.ActiveDays.Contains(DayOfWeek.Tuesday);
            chkWednesday.Checked = _workingConfiguration.Schedule.ActiveDays.Contains(DayOfWeek.Wednesday);
            chkThursday.Checked = _workingConfiguration.Schedule.ActiveDays.Contains(DayOfWeek.Thursday);
            chkFriday.Checked = _workingConfiguration.Schedule.ActiveDays.Contains(DayOfWeek.Friday);
            chkSaturday.Checked = _workingConfiguration.Schedule.ActiveDays.Contains(DayOfWeek.Saturday);
            chkSunday.Checked = _workingConfiguration.Schedule.ActiveDays.Contains(DayOfWeek.Sunday);

            // Populate city/state dropdown
            cmbCityState.Items.Clear();
            foreach (var k in CityLookup.Keys)
            {
                cmbCityState.Items.Add(k);
            }

            // Sunset settings
            chkUseSunset.Checked = _workingConfiguration.Schedule.UseSunset;
            numSunsetOffset.Value = _workingConfiguration.Schedule.SunsetOffsetMinutes;
            
            // Set the appropriate radio button based on configuration
            if (_workingConfiguration.Schedule.EnablePerDaySchedule)
            {
                rbPerDaySchedule.Checked = true;
            }
            else if (_workingConfiguration.Schedule.UseSunset)
            {
                rbSunsetSchedule.Checked = true;
            }
            else
            {
                rbDefaultSchedule.Checked = true;
            }
            
            // If coordinates match a lookup, select it (before updating preview)
            var match = CityLookup.FirstOrDefault(k => 
                Math.Abs(k.Value.Lat - _workingConfiguration.Schedule.SunsetLatitude) < 0.01 && 
                Math.Abs(k.Value.Lon - _workingConfiguration.Schedule.SunsetLongitude) < 0.01);
            
            if (!string.IsNullOrEmpty(match.Key))
                cmbCityState.SelectedItem = match.Key;
            else if (cmbCityState.Items.Count > 0)
                cmbCityState.SelectedIndex = 0;  // Default to "Auto detect"
            
            // Update visibility based on selections (now that offset and city are set)
            UpdatePerDayVisibility();
            UpdateSunsetPreview();

            // Per-day checkbox
            chkEnablePerDay.Checked = _workingConfiguration.Schedule.EnablePerDaySchedule;
            grpPerDaySchedules.Visible = chkEnablePerDay.Checked;

            // Load per-day times into the controls if present
            try
            {
                dtpMonStart.Value = DateTime.Today.Add(_workingConfiguration.Schedule.PerDayStartTimes.GetValueOrDefault(DayOfWeek.Monday, _workingConfiguration.Schedule.NightModeStartTime));
                dtpMonEnd.Value = DateTime.Today.Add(_workingConfiguration.Schedule.PerDayEndTimes.GetValueOrDefault(DayOfWeek.Monday, _workingConfiguration.Schedule.NightModeEndTime));
                dtpTueStart.Value = DateTime.Today.Add(_workingConfiguration.Schedule.PerDayStartTimes.GetValueOrDefault(DayOfWeek.Tuesday, _workingConfiguration.Schedule.NightModeStartTime));
                dtpTueEnd.Value = DateTime.Today.Add(_workingConfiguration.Schedule.PerDayEndTimes.GetValueOrDefault(DayOfWeek.Tuesday, _workingConfiguration.Schedule.NightModeEndTime));
                dtpWedStart.Value = DateTime.Today.Add(_workingConfiguration.Schedule.PerDayStartTimes.GetValueOrDefault(DayOfWeek.Wednesday, _workingConfiguration.Schedule.NightModeStartTime));
                dtpWedEnd.Value = DateTime.Today.Add(_workingConfiguration.Schedule.PerDayEndTimes.GetValueOrDefault(DayOfWeek.Wednesday, _workingConfiguration.Schedule.NightModeEndTime));
                dtpThuStart.Value = DateTime.Today.Add(_workingConfiguration.Schedule.PerDayStartTimes.GetValueOrDefault(DayOfWeek.Thursday, _workingConfiguration.Schedule.NightModeStartTime));
                dtpThuEnd.Value = DateTime.Today.Add(_workingConfiguration.Schedule.PerDayEndTimes.GetValueOrDefault(DayOfWeek.Thursday, _workingConfiguration.Schedule.NightModeEndTime));
                dtpFriStart.Value = DateTime.Today.Add(_workingConfiguration.Schedule.PerDayStartTimes.GetValueOrDefault(DayOfWeek.Friday, _workingConfiguration.Schedule.NightModeStartTime));
                dtpFriEnd.Value = DateTime.Today.Add(_workingConfiguration.Schedule.PerDayEndTimes.GetValueOrDefault(DayOfWeek.Friday, _workingConfiguration.Schedule.NightModeEndTime));
                dtpSatStart.Value = DateTime.Today.Add(_workingConfiguration.Schedule.PerDayStartTimes.GetValueOrDefault(DayOfWeek.Saturday, _workingConfiguration.Schedule.NightModeStartTime));
                dtpSatEnd.Value = DateTime.Today.Add(_workingConfiguration.Schedule.PerDayEndTimes.GetValueOrDefault(DayOfWeek.Saturday, _workingConfiguration.Schedule.NightModeEndTime));
                dtpSunStart.Value = DateTime.Today.Add(_workingConfiguration.Schedule.PerDayStartTimes.GetValueOrDefault(DayOfWeek.Sunday, _workingConfiguration.Schedule.NightModeStartTime));
                dtpSunEnd.Value = DateTime.Today.Add(_workingConfiguration.Schedule.PerDayEndTimes.GetValueOrDefault(DayOfWeek.Sunday, _workingConfiguration.Schedule.NightModeEndTime));
            }
            catch
            {
                // ignore if controls not present or values invalid
            }

            // Controller Settings - keyboard only now
            cmbToggleKey.Text = _workingConfiguration.Controller.ToggleKeyCode;
            numToggleDelay.Value = _workingConfiguration.Controller.ToggleDelay;
            rbKeyboardEmulation.Checked = true;  // Always keyboard mode

            // General Settings
            chkStartWithWindows.Checked = _workingConfiguration.General.StartWithWindows;
            chkStartMinimized.Checked = !_workingConfiguration.General.ShowSettingsOnStartup;  // Inverted: checkbox is "start minimized", property is "show settings"
            chkShowNotifications.Checked = _workingConfiguration.General.ShowNotifications;
            numCheckInterval.Value = _workingConfiguration.General.CheckInterval;
            chkDebugMode.Checked = _workingConfiguration.General.DebugMode;

            UpdateControlStates();
        }

        private void cmbCityState_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cmbCityState.SelectedItem == null) return;
            var key = cmbCityState.SelectedItem.ToString();
            if (string.IsNullOrEmpty(key)) return;

            if (CityLookup.TryGetValue(key, out var coords))
            {
                if (coords.Lat == 0 && coords.Lon == 0)
                {
                    // Auto detect - could implement IP geolocation here
                    return;
                }

                // Update hidden fields (used internally)
                _workingConfiguration.Schedule.SunsetLatitude = coords.Lat;
                _workingConfiguration.Schedule.SunsetLongitude = coords.Lon;
            }
        }

        private void SaveConfigurationFromControls()
        {
            // Schedule Settings
            _workingConfiguration.Schedule.EnableSchedule = chkEnableSchedule.Checked;
            _workingConfiguration.Schedule.NightModeStartTime = dtpStartTime.Value.TimeOfDay;
            _workingConfiguration.Schedule.NightModeEndTime = dtpEndTime.Value.TimeOfDay;
            
            // Active Days
            _workingConfiguration.Schedule.ActiveDays.Clear();
            if (chkMonday.Checked) _workingConfiguration.Schedule.ActiveDays.Add(DayOfWeek.Monday);
            if (chkTuesday.Checked) _workingConfiguration.Schedule.ActiveDays.Add(DayOfWeek.Tuesday);
            if (chkWednesday.Checked) _workingConfiguration.Schedule.ActiveDays.Add(DayOfWeek.Wednesday);
            if (chkThursday.Checked) _workingConfiguration.Schedule.ActiveDays.Add(DayOfWeek.Thursday);
            if (chkFriday.Checked) _workingConfiguration.Schedule.ActiveDays.Add(DayOfWeek.Friday);
            if (chkSaturday.Checked) _workingConfiguration.Schedule.ActiveDays.Add(DayOfWeek.Saturday);
            if (chkSunday.Checked) _workingConfiguration.Schedule.ActiveDays.Add(DayOfWeek.Sunday);

            // Sunset settings (lat/lon already set by city selector)
            _workingConfiguration.Schedule.UseSunset = chkUseSunset.Checked;
            _workingConfiguration.Schedule.SunsetOffsetMinutes = (int)numSunsetOffset.Value;
            
            // Per-day schedules
            _workingConfiguration.Schedule.EnablePerDaySchedule = chkEnablePerDay.Checked;

            // Save per-day schedule times
            try
            {
                _workingConfiguration.Schedule.PerDayStartTimes[DayOfWeek.Monday] = dtpMonStart.Value.TimeOfDay;
                _workingConfiguration.Schedule.PerDayEndTimes[DayOfWeek.Monday] = dtpMonEnd.Value.TimeOfDay;
                _workingConfiguration.Schedule.PerDayStartTimes[DayOfWeek.Tuesday] = dtpTueStart.Value.TimeOfDay;
                _workingConfiguration.Schedule.PerDayEndTimes[DayOfWeek.Tuesday] = dtpTueEnd.Value.TimeOfDay;
                _workingConfiguration.Schedule.PerDayStartTimes[DayOfWeek.Wednesday] = dtpWedStart.Value.TimeOfDay;
                _workingConfiguration.Schedule.PerDayEndTimes[DayOfWeek.Wednesday] = dtpWedEnd.Value.TimeOfDay;
                _workingConfiguration.Schedule.PerDayStartTimes[DayOfWeek.Thursday] = dtpThuStart.Value.TimeOfDay;
                _workingConfiguration.Schedule.PerDayEndTimes[DayOfWeek.Thursday] = dtpThuEnd.Value.TimeOfDay;
                _workingConfiguration.Schedule.PerDayStartTimes[DayOfWeek.Friday] = dtpFriStart.Value.TimeOfDay;
                _workingConfiguration.Schedule.PerDayEndTimes[DayOfWeek.Friday] = dtpFriEnd.Value.TimeOfDay;
                _workingConfiguration.Schedule.PerDayStartTimes[DayOfWeek.Saturday] = dtpSatStart.Value.TimeOfDay;
                _workingConfiguration.Schedule.PerDayEndTimes[DayOfWeek.Saturday] = dtpSatEnd.Value.TimeOfDay;
                _workingConfiguration.Schedule.PerDayStartTimes[DayOfWeek.Sunday] = dtpSunStart.Value.TimeOfDay;
                _workingConfiguration.Schedule.PerDayEndTimes[DayOfWeek.Sunday] = dtpSunEnd.Value.TimeOfDay;
            }
            catch
            {
                // ignore errors when saving per-day times
            }

            // Controller Settings - keyboard emulation only
            _workingConfiguration.Controller.ToggleKeyCode = cmbToggleKey.Text;
            _workingConfiguration.Controller.ToggleDelay = (int)numToggleDelay.Value;
            _workingConfiguration.Controller.UseKeyboardEmulation = true;
            _workingConfiguration.Controller.UseSerialCommunication = false;

            // General Settings
            _workingConfiguration.General.StartWithWindows = chkStartWithWindows.Checked;
            _workingConfiguration.General.ShowSettingsOnStartup = !chkStartMinimized.Checked;  // Inverted: checkbox is "start minimized", property is "show settings"
            _workingConfiguration.General.ShowNotifications = chkShowNotifications.Checked;
            _workingConfiguration.General.CheckInterval = (int)numCheckInterval.Value;
            _workingConfiguration.General.DebugMode = chkDebugMode.Checked;
        }

        private void UpdateControlStates()
        {
            // Enable/disable controls based on settings
            grpSchedule.Enabled = chkEnableSchedule.Checked;
            grpSerialSettings.Enabled = rbSerialCommunication.Checked;
            grpKeyboardSettings.Enabled = rbKeyboardEmulation.Checked;
        }

        private void chkEnableSchedule_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlStates();
        }

        private void rbSerialCommunication_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlStates();
        }

        private void rbKeyboardEmulation_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlStates();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveConfigurationFromControls();
                _configService.UpdateConfiguration(_workingConfiguration);
                MessageBox.Show("Settings saved successfully!", "Settings", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnTestNightMode_Click(object sender, EventArgs e)
        {
            try
            {
                // Save current settings temporarily for testing
                SaveConfigurationFromControls();
                
                // Create a temporary night mode service for testing
                var testService = new NightModeService();
                testService.Initialize(_workingConfiguration);
                
                // Test the toggle
                Task.Run(async () =>
                {
                    var result = await testService.ToggleNightMode(true);
                    this.Invoke(() =>
                    {
                        if (result)
                        {
                            MessageBox.Show("Test successful! Night mode toggle command sent.", "Test Result",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Test failed. Please check your controller settings.", "Test Result",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    });
                    
                    testService.Dispose();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Test error: {ex.Message}", "Test Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnResetDefaults_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Reset all settings to default values?", "Reset Settings", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _workingConfiguration = new AppConfiguration();
                LoadConfigurationToControls();
            }
        }

        private async void UpdateSunsetPreview()
        {
            if (!rbSunsetSchedule.Checked || cmbCityState.SelectedItem == null)
            {
                lblSunsetPreview.Text = "Select a location to see sunset times...";
                return;
            }

            try
            {
                var cityKey = cmbCityState.SelectedItem.ToString();
                (double Lat, double Lon) coords;
                
                if (cityKey == "Auto detect")
                {
                    // Try to determine location based on timezone
                    var detectedLocation = GetAutoDetectedLocation();
                    
                    // Get coordinates from the detected city
                    if (!CityLookup.TryGetValue(detectedLocation, out coords))
                    {
                        lblSunsetPreview.Text = $"Auto-detect: Using {detectedLocation}\n(Unable to calculate sunset times for this location)";
                        return;
                    }
                }
                else if (!CityLookup.TryGetValue(cityKey!, out coords))
                {
                    lblSunsetPreview.Text = "Unable to calculate sunset times.";
                    return;
                }

                var sunsetService = new SunsetService();
                var offset = (int)numSunsetOffset.Value;
                var activeDays = new List<DayOfWeek>();
                
                if (chkMonday.Checked) activeDays.Add(DayOfWeek.Monday);
                if (chkTuesday.Checked) activeDays.Add(DayOfWeek.Tuesday);
                if (chkWednesday.Checked) activeDays.Add(DayOfWeek.Wednesday);
                if (chkThursday.Checked) activeDays.Add(DayOfWeek.Thursday);
                if (chkFriday.Checked) activeDays.Add(DayOfWeek.Friday);
                if (chkSaturday.Checked) activeDays.Add(DayOfWeek.Saturday);
                if (chkSunday.Checked) activeDays.Add(DayOfWeek.Sunday);

                if (activeDays.Count == 0)
                {
                    lblSunsetPreview.Text = "No active days selected.";
                    return;
                }

                lblSunsetPreview.Text = "Fetching sunset times...";
                
                // Get sunset time (it doesn't vary much day to day, so we'll use today's time)
                var result = await sunsetService.GetSunsetTimeAsync(coords.Lat, coords.Lon);
                
                if (result.Error != null)
                {
                    lblSunsetPreview.Text = $"Error: {result.Error}";
                    return;
                }

                if (result.Sunset == null)
                {
                    lblSunsetPreview.Text = "Unable to calculate sunset time.";
                    return;
                }

                var sunsetTime = result.Sunset.Value;
                var adjustedTime = sunsetTime.Add(TimeSpan.FromMinutes(offset));
                
                var preview = new System.Text.StringBuilder();
                
                // Add auto-detect info if applicable
                if (cityKey == "Auto detect")
                {
                    var detectedLocation = GetAutoDetectedLocation();
                    preview.AppendLine($"Auto-detect: Using {detectedLocation}");
                    preview.AppendLine();
                }
                
                preview.AppendLine("Night mode will activate at approximately:");
                preview.AppendLine($"{DateTime.Today.Add(adjustedTime):h:mm tt}");
                preview.AppendLine();
                preview.Append($"Active on: {string.Join(", ", activeDays.Select(d => d.ToString().Substring(0, 3)))}");

                lblSunsetPreview.Text = preview.ToString().TrimEnd();
            }
            catch (Exception ex)
            {
                lblSunsetPreview.Text = $"Error calculating sunset times: {ex.Message}";
            }
        }

        private DateTime GetNextOccurrence(DayOfWeek targetDay)
        {
            var today = DateTime.Today;
            int daysUntilTarget = ((int)targetDay - (int)today.DayOfWeek + 7) % 7;
            if (daysUntilTarget == 0) return today;
            return today.AddDays(daysUntilTarget);
        }

        private void UpdatePerDayVisibility()
        {
            if (!rbPerDaySchedule.Checked) return;

            // Map of day abbreviations to checkbox state and DateTimePickers
            var dayConfigs = new[]
            {
                new { Abbr = "Mon", Enabled = chkMonday.Checked, Start = dtpMonStart, End = dtpMonEnd },
                new { Abbr = "Tue", Enabled = chkTuesday.Checked, Start = dtpTueStart, End = dtpTueEnd },
                new { Abbr = "Wed", Enabled = chkWednesday.Checked, Start = dtpWedStart, End = dtpWedEnd },
                new { Abbr = "Thu", Enabled = chkThursday.Checked, Start = dtpThuStart, End = dtpThuEnd },
                new { Abbr = "Fri", Enabled = chkFriday.Checked, Start = dtpFriStart, End = dtpFriEnd },
                new { Abbr = "Sat", Enabled = chkSaturday.Checked, Start = dtpSatStart, End = dtpSatEnd },
                new { Abbr = "Sun", Enabled = chkSunday.Checked, Start = dtpSunStart, End = dtpSunEnd }
            };

            // Track position for enabled days only
            int columnIndex = 0;
            
            foreach (var day in dayConfigs)
            {
                // Find all controls for this day
                var dayControls = grpPerDaySchedules.Controls.Cast<Control>()
                    .Where(c => c.Tag != null && c.Tag.ToString()!.Contains(day.Abbr))
                    .ToList();

                if (day.Enabled)
                {
                    // Show and reposition
                    int xPos = 10 + columnIndex * 100;
                    
                    foreach (var ctrl in dayControls)
                    {
                        ctrl.Visible = true;
                        
                        // Update X position while keeping Y position
                        var tag = ctrl.Tag?.ToString() ?? "";
                        if (tag.StartsWith("day_"))
                            ctrl.Location = new Point(xPos, 25);
                        else if (tag.StartsWith("start_"))
                            ctrl.Location = new Point(xPos, 50);
                        else if (tag.StartsWith("end_"))
                            ctrl.Location = new Point(xPos, 93);
                        else if (ctrl is DateTimePicker dtp)
                        {
                            if (dtp == day.Start)
                                ctrl.Location = new Point(xPos, 68);
                            else if (dtp == day.End)
                                ctrl.Location = new Point(xPos, 111);
                        }
                    }
                    
                    day.Start.Visible = true;
                    day.End.Visible = true;
                    day.Start.Location = new Point(xPos, 68);
                    day.End.Location = new Point(xPos, 111);
                    
                    columnIndex++;
                }
                else
                {
                    // Hide
                    foreach (var ctrl in dayControls)
                        ctrl.Visible = false;
                    
                    day.Start.Visible = false;
                    day.End.Visible = false;
                }
            }
        }

        private string GetAutoDetectedLocation()
        {
            try
            {
                var timeZone = TimeZoneInfo.Local;
                var tzId = timeZone.Id;

                // Map common timezone IDs to cities
                var timezoneMapping = new Dictionary<string, string>
                {
                    ["Eastern Standard Time"] = "New York, NY",
                    ["Central Standard Time"] = "Chicago, IL",
                    ["Mountain Standard Time"] = "Denver, CO",
                    ["Pacific Standard Time"] = "Los Angeles, CA",
                    ["Arizona"] = "Phoenix, AZ",
                    ["Alaskan Standard Time"] = "Anchorage, AK",
                    ["Hawaiian Standard Time"] = "Honolulu, HI"
                };

                if (timezoneMapping.TryGetValue(tzId, out var city))
                {
                    return city;
                }

                // Fallback to timezone name
                return $"timezone ({timeZone.StandardName})";
            }
            catch
            {
                return "system location";
            }
        }
    }
}

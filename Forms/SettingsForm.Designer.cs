namespace AutoNightMode.Forms
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private TabControl tabControl;
        private TabPage tabSchedule;
        private TabPage tabController;
        private TabPage tabGeneral;
        
        // Schedule controls
        private CheckBox chkEnableSchedule;
        private GroupBox grpSchedule;
        private Label lblStartTime;
        private DateTimePicker dtpStartTime;
        private Label lblEndTime;
        private DateTimePicker dtpEndTime;
        private GroupBox grpActiveDays;
        private CheckBox chkMonday;
        private CheckBox chkTuesday;
        private CheckBox chkWednesday;
        private CheckBox chkThursday;
        private CheckBox chkFriday;
        private CheckBox chkSaturday;
        private CheckBox chkSunday;
        
        // Sunset/per-day controls (now fields)
        private ComboBox cmbCityState;
        private Label lblCityState;
        private CheckBox chkUseSunset;
        private Label lblSunsetLat;
        private TextBox txtSunsetLat;
        private Label lblSunsetLon;
        private TextBox txtSunsetLon;
        private Label lblSunsetOffset;
        private NumericUpDown numSunsetOffset;
        private CheckBox chkEnablePerDay;
        private RadioButton rbDefaultSchedule;
        private RadioButton rbSunsetSchedule;
        private RadioButton rbPerDaySchedule;
        private Label lblSunsetPreview;
        
        // Controller controls
        private GroupBox grpControllerType;
        private ComboBox cmbControllerType;
        private Label lblControllerType;
        private RadioButton rbKeyboardEmulation;
        private RadioButton rbSerialCommunication;
        private GroupBox grpKeyboardSettings;
        private Label lblToggleKey;
        private ComboBox cmbToggleKey;
        private Label lblToggleDelay;
        private NumericUpDown numToggleDelay;
        private GroupBox grpSerialSettings;
        private Label lblControllerPort;
        private TextBox txtControllerPort;
        private Label lblBaudRate;
        private NumericUpDown numBaudRate;
        private Button btnTestNightMode;
        
        // General controls
        private CheckBox chkStartWithWindows;
        private CheckBox chkStartMinimized;
        private CheckBox chkShowNotifications;
        private Label lblCheckInterval;
        private NumericUpDown numCheckInterval;
        private CheckBox chkDebugMode;
        
        // Form controls
        private Button btnSave;
        private Button btnClose;
        private Button btnResetDefaults;

        // Per-day schedule controls
        private GroupBox grpPerDaySchedules;
        private DateTimePicker dtpMonStart;
        private DateTimePicker dtpMonEnd;
        private DateTimePicker dtpTueStart;
        private DateTimePicker dtpTueEnd;
        private DateTimePicker dtpWedStart;
        private DateTimePicker dtpWedEnd;
        private DateTimePicker dtpThuStart;
        private DateTimePicker dtpThuEnd;
        private DateTimePicker dtpFriStart;
        private DateTimePicker dtpFriEnd;
        private DateTimePicker dtpSatStart;
        private DateTimePicker dtpSatEnd;
        private DateTimePicker dtpSunStart;
        private DateTimePicker dtpSunEnd;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControl = new TabControl();
            this.tabSchedule = new TabPage();
            this.tabController = new TabPage();
            this.tabGeneral = new TabPage();
            
            // Schedule tab controls
            this.chkEnableSchedule = new CheckBox();
            this.grpSchedule = new GroupBox();
            this.lblStartTime = new Label();
            this.dtpStartTime = new DateTimePicker();
            this.lblEndTime = new Label();
            this.dtpEndTime = new DateTimePicker();
            this.grpActiveDays = new GroupBox();
            this.chkMonday = new CheckBox();
            this.chkTuesday = new CheckBox();
            this.chkWednesday = new CheckBox();
            this.chkThursday = new CheckBox();
            this.chkFriday = new CheckBox();
            this.chkSaturday = new CheckBox();
            this.chkSunday = new CheckBox();
            
            // Controller tab controls
            this.grpControllerType = new GroupBox();
            this.cmbControllerType = new ComboBox();
            this.lblControllerType = new Label();
            this.rbKeyboardEmulation = new RadioButton();
            this.rbSerialCommunication = new RadioButton();
            this.grpKeyboardSettings = new GroupBox();
            this.lblToggleKey = new Label();
            this.cmbToggleKey = new ComboBox();
            this.lblToggleDelay = new Label();
            this.numToggleDelay = new NumericUpDown();
            this.grpSerialSettings = new GroupBox();
            this.lblControllerPort = new Label();
            this.txtControllerPort = new TextBox();
            this.lblBaudRate = new Label();
            this.numBaudRate = new NumericUpDown();
            this.btnTestNightMode = new Button();
            
            // General tab controls
            this.chkStartWithWindows = new CheckBox();
            this.chkStartMinimized = new CheckBox();
            this.chkShowNotifications = new CheckBox();
            this.lblCheckInterval = new Label();
            this.numCheckInterval = new NumericUpDown();
            this.chkDebugMode = new CheckBox();
            
            // Form controls
            this.btnSave = new Button();
            this.btnClose = new Button();
            this.btnResetDefaults = new Button();

            this.SuspendLayout();

            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabSchedule);
            this.tabControl.Controls.Add(this.tabController);
            this.tabControl.Controls.Add(this.tabGeneral);
            this.tabControl.Location = new Point(12, 12);
            this.tabControl.Size = new Size(760, 400);
            this.tabControl.SelectedIndex = 0;

            // 
            // Schedule Tab
            // 
            this.tabSchedule.Text = "Schedule";
            this.tabSchedule.UseVisualStyleBackColor = true;
            this.tabSchedule.AutoScroll = true;  // Enable scrolling for long content
            
            // 1. Enable checkbox
            this.chkEnableSchedule.Text = "Enable automatic scheduling";
            this.chkEnableSchedule.Location = new Point(6, 6);
            this.chkEnableSchedule.Size = new Size(200, 24);
            this.chkEnableSchedule.CheckedChanged += chkEnableSchedule_CheckedChanged;
            this.tabSchedule.Controls.Add(this.chkEnableSchedule);

            // Declare grpSunset early so it can be referenced in event handlers
            var grpSunset = new GroupBox();

            // 2. Schedule Mode Selection
            var grpScheduleMode = new GroupBox();
            grpScheduleMode.Text = "Schedule Mode";
            grpScheduleMode.Location = new Point(6, 36);
            grpScheduleMode.Size = new Size(730, 80);
            
            this.rbDefaultSchedule = new RadioButton();
            rbDefaultSchedule.Text = "Use default schedule (same time every day)";
            rbDefaultSchedule.Location = new Point(10, 20);
            rbDefaultSchedule.Size = new Size(500, 20);
            rbDefaultSchedule.Tag = "default";
            rbDefaultSchedule.CheckedChanged += (s, e) => {
                if (rbDefaultSchedule.Checked) {
                    chkEnablePerDay.Checked = false;
                    chkUseSunset.Checked = false;
                    this.grpSchedule.Visible = true;
                    grpSunset.Visible = false;
                    this.grpPerDaySchedules.Visible = false;
                }
            };
            grpScheduleMode.Controls.Add(rbDefaultSchedule);
            
            this.rbSunsetSchedule = new RadioButton();
            rbSunsetSchedule.Text = "Use sunset-based schedule (sunset triggers night mode)";
            rbSunsetSchedule.Location = new Point(10, 40);
            rbSunsetSchedule.Size = new Size(500, 20);
            rbSunsetSchedule.Tag = "sunset";
            rbSunsetSchedule.CheckedChanged += (s, e) => {
                if (rbSunsetSchedule.Checked) {
                    chkUseSunset.Checked = true;
                    chkEnablePerDay.Checked = false;
                    this.grpSchedule.Visible = false;
                    grpSunset.Visible = true;
                    this.grpPerDaySchedules.Visible = false;
                    UpdateSunsetPreview();
                }
            };
            grpScheduleMode.Controls.Add(rbSunsetSchedule);
            
            this.rbPerDaySchedule = new RadioButton();
            rbPerDaySchedule.Text = "Use per-day schedules (different times for each day)";
            rbPerDaySchedule.Location = new Point(10, 60);
            rbPerDaySchedule.Size = new Size(500, 20);
            rbPerDaySchedule.Tag = "perday";
            rbPerDaySchedule.CheckedChanged += (s, e) => {
                if (rbPerDaySchedule.Checked) {
                    chkEnablePerDay.Checked = true;
                    chkUseSunset.Checked = false;
                    this.grpSchedule.Visible = false;
                    grpSunset.Visible = false;
                    this.grpPerDaySchedules.Visible = true;
                    UpdatePerDayVisibility();
                }
            };
            grpScheduleMode.Controls.Add(rbPerDaySchedule);
            
            this.tabSchedule.Controls.Add(grpScheduleMode);
            
            // 3. Active Days
            this.grpActiveDays.Text = "Active Days";
            this.grpActiveDays.Location = new Point(6, 122);
            this.grpActiveDays.Size = new Size(730, 55);
            
            var dayCheckBoxes = new[] { chkMonday, chkTuesday, chkWednesday, chkThursday, chkFriday, chkSaturday, chkSunday };
            var dayNames = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            
            for (int i = 0; i < dayCheckBoxes.Length; i++)
            {
                dayCheckBoxes[i].Text = dayNames[i];
                // Arrange in single row with even spacing
                dayCheckBoxes[i].Location = new Point(10 + (i * 103), 25);
                dayCheckBoxes[i].Size = new Size(100, 24);
                dayCheckBoxes[i].CheckedChanged += (s, e) => {
                    UpdatePerDayVisibility();
                    UpdateSunsetPreview();
                };
                this.grpActiveDays.Controls.Add(dayCheckBoxes[i]);
            }
            
            this.tabSchedule.Controls.Add(this.grpActiveDays);

            // 4a. Default Schedule Section (visible when default mode selected)
            this.grpSchedule.Text = "Default Schedule";
            this.grpSchedule.Location = new Point(6, 183);
            this.grpSchedule.Size = new Size(730, 80);
            this.grpSchedule.Visible = true;  // Default
            
            this.lblStartTime.Text = "Start Time:";
            this.lblStartTime.Location = new Point(10, 25);
            this.lblStartTime.Size = new Size(80, 23);
            
            this.dtpStartTime.Format = DateTimePickerFormat.Time;
            this.dtpStartTime.ShowUpDown = true;
            this.dtpStartTime.Location = new Point(95, 22);
            this.dtpStartTime.Size = new Size(100, 23);
            
            this.lblEndTime.Text = "End Time:";
            this.lblEndTime.Location = new Point(220, 25);
            this.lblEndTime.Size = new Size(70, 23);
            
            this.dtpEndTime.Format = DateTimePickerFormat.Time;
            this.dtpEndTime.ShowUpDown = true;
            this.dtpEndTime.Location = new Point(295, 22);
            this.dtpEndTime.Size = new Size(100, 23);
            
            // Add help text
            var lblScheduleHelp = new Label();
            lblScheduleHelp.Text = "Night mode will be enabled during this time window";
            lblScheduleHelp.Location = new Point(10, 50);
            lblScheduleHelp.Size = new Size(400, 20);
            lblScheduleHelp.ForeColor = Color.Gray;
            lblScheduleHelp.Font = new Font(lblScheduleHelp.Font.FontFamily, 8);
            this.grpSchedule.Controls.Add(lblScheduleHelp);
            
            this.grpSchedule.Controls.Add(this.lblStartTime);
            this.grpSchedule.Controls.Add(this.dtpStartTime);
            this.grpSchedule.Controls.Add(this.lblEndTime);
            this.grpSchedule.Controls.Add(this.dtpEndTime);
            
            this.tabSchedule.Controls.Add(this.grpSchedule);

            // Configure grpSunset (declared earlier)
            grpSunset.Text = "Sunset Settings";
            grpSunset.Location = new Point(6, 183);
            grpSunset.Size = new Size(730, 120);
            grpSunset.Visible = false;  // Hidden by default

            // Hide the checkbox - controlled by radio buttons now
            this.chkUseSunset = new CheckBox();
            this.chkUseSunset.Visible = false;

            this.lblCityState = new Label();
            this.lblCityState.Text = "Location:";
            this.lblCityState.Location = new Point(10, 25);
            this.lblCityState.Size = new Size(70, 23);
            grpSunset.Controls.Add(this.lblCityState);

            this.cmbCityState = new ComboBox();
            this.cmbCityState.Location = new Point(85, 25);
            this.cmbCityState.Size = new Size(250, 23);
            this.cmbCityState.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbCityState.SelectedIndexChanged += (s, e) => {
                cmbCityState_SelectedIndexChanged(s, e);
                UpdateSunsetPreview();
            };
            grpSunset.Controls.Add(this.cmbCityState);

            this.lblSunsetOffset = new Label();
            this.lblSunsetOffset.Text = "Offset (minutes):";
            this.lblSunsetOffset.Location = new Point(345, 25);
            this.lblSunsetOffset.Size = new Size(100, 23);
            grpSunset.Controls.Add(this.lblSunsetOffset);

            this.numSunsetOffset = new NumericUpDown();
            this.numSunsetOffset.Location = new Point(450, 25);
            this.numSunsetOffset.Size = new Size(60, 23);
            this.numSunsetOffset.Minimum = -120;
            this.numSunsetOffset.Maximum = 120;
            this.numSunsetOffset.Value = 0;
            this.numSunsetOffset.ValueChanged += (s, e) => UpdateSunsetPreview();
            grpSunset.Controls.Add(this.numSunsetOffset);

            // Sunset time preview
            this.lblSunsetPreview = new Label();
            this.lblSunsetPreview.Text = "Sunset times will be shown here...";
            this.lblSunsetPreview.Location = new Point(10, 55);
            this.lblSunsetPreview.Size = new Size(500, 60);
            this.lblSunsetPreview.ForeColor = Color.Gray;
            this.lblSunsetPreview.Font = new Font(this.lblSunsetPreview.Font.FontFamily, 8);
            grpSunset.Controls.Add(this.lblSunsetPreview);
            grpSunset.Controls.Add(this.numSunsetOffset);

            // Hidden lat/lon (still available for manual override via config file)
            this.lblSunsetLat = new Label();
            this.txtSunsetLat = new TextBox();
            this.lblSunsetLon = new Label();
            this.txtSunsetLon = new TextBox();
            this.txtSunsetLat.Visible = false;
            this.txtSunsetLon.Visible = false;
            this.lblSunsetLat.Visible = false;
            this.lblSunsetLon.Visible = false;

            this.tabSchedule.Controls.Add(grpSunset);

            // 4c. Per-day schedule section (visible when per-day mode selected)
            // Per-day schedule checkbox (hidden, controlled by radio buttons)
            this.chkEnablePerDay = new CheckBox();
            this.chkEnablePerDay.Visible = false;

            // Per-day schedules group
            this.grpPerDaySchedules = new GroupBox();
            this.grpPerDaySchedules.Text = "Per-day Schedules";
            this.grpPerDaySchedules.Location = new Point(6, 183);
            this.grpPerDaySchedules.Size = new Size(730, 140);
            this.grpPerDaySchedules.Visible = false;  // Hidden by default

            // Monday
            this.dtpMonStart = new DateTimePicker();
            this.dtpMonEnd = new DateTimePicker();
            // Tuesday
            this.dtpTueStart = new DateTimePicker();
            this.dtpTueEnd = new DateTimePicker();
            // Wednesday
            this.dtpWedStart = new DateTimePicker();
            this.dtpWedEnd = new DateTimePicker();
            // Thursday
            this.dtpThuStart = new DateTimePicker();
            this.dtpThuEnd = new DateTimePicker();
            // Friday
            this.dtpFriStart = new DateTimePicker();
            this.dtpFriEnd = new DateTimePicker();
            // Saturday
            this.dtpSatStart = new DateTimePicker();
            this.dtpSatEnd = new DateTimePicker();
            // Sunday
            this.dtpSunStart = new DateTimePicker();
            this.dtpSunEnd = new DateTimePicker();

            var dayLabels = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            var starts = new[] { dtpMonStart, dtpTueStart, dtpWedStart, dtpThuStart, dtpFriStart, dtpSatStart, dtpSunStart };
            var ends = new[] { dtpMonEnd, dtpTueEnd, dtpWedEnd, dtpThuEnd, dtpFriEnd, dtpSatEnd, dtpSunEnd };

            for (int i = 0; i < 7; i++)
            {
                var lbl = new Label();
                lbl.Text = dayLabels[i] + ":";
                lbl.Location = new Point(10 + i * 100, 25);
                lbl.Size = new Size(90, 23);
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                lbl.Tag = "day_" + dayLabels[i];  // Tag for identification
                this.grpPerDaySchedules.Controls.Add(lbl);

                var lblStart = new Label();
                lblStart.Text = "Start";
                lblStart.Location = new Point(10 + i * 100, 50);
                lblStart.Size = new Size(90, 15);
                lblStart.Font = new Font(lblStart.Font, FontStyle.Regular);
                lblStart.ForeColor = Color.Gray;
                lblStart.Tag = "start_" + dayLabels[i];  // Tag for identification
                this.grpPerDaySchedules.Controls.Add(lblStart);

                starts[i].Format = DateTimePickerFormat.Time;
                starts[i].ShowUpDown = true;
                starts[i].Location = new Point(10 + i * 100, 68);
                starts[i].Size = new Size(90, 23);
                starts[i].Tag = "dtp_" + dayLabels[i];  // Tag for identification
                this.grpPerDaySchedules.Controls.Add(starts[i]);

                var lblEnd = new Label();
                lblEnd.Text = "End";
                lblEnd.Location = new Point(10 + i * 100, 93);
                lblEnd.Size = new Size(90, 15);
                lblEnd.Font = new Font(lblEnd.Font, FontStyle.Regular);
                lblEnd.ForeColor = Color.Gray;
                lblEnd.Tag = "end_" + dayLabels[i];  // Tag for identification
                this.grpPerDaySchedules.Controls.Add(lblEnd);

                ends[i].Format = DateTimePickerFormat.Time;
                ends[i].ShowUpDown = true;
                ends[i].Location = new Point(10 + i * 100, 111);
                ends[i].Size = new Size(90, 23);
                ends[i].Tag = "dtp_" + dayLabels[i];  // Tag for identification
                this.grpPerDaySchedules.Controls.Add(ends[i]);
            }

            this.tabSchedule.Controls.Add(this.grpPerDaySchedules);

            // 
            // Controller Tab
            // 
            this.tabController.Text = "Controller";
            this.tabController.UseVisualStyleBackColor = true;
            
            this.grpControllerType.Text = "Controller Type";
            this.grpControllerType.Location = new Point(6, 6);
            this.grpControllerType.Size = new Size(540, 60);
            
            this.rbKeyboardEmulation.Text = "Keyboard Emulation (Send keypress to toggle night mode)";
            this.rbKeyboardEmulation.Location = new Point(10, 25);
            this.rbKeyboardEmulation.Size = new Size(400, 24);
            this.rbKeyboardEmulation.Checked = true;
            this.rbKeyboardEmulation.CheckedChanged += rbKeyboardEmulation_CheckedChanged;
            
            // Remove serial communication option
            this.rbSerialCommunication.Visible = false;
            
            this.grpKeyboardSettings.Text = "Keyboard Settings";
            this.grpKeyboardSettings.Location = new Point(6, 72);
            this.grpKeyboardSettings.Size = new Size(540, 100);
            
            this.lblToggleKey.Text = "Toggle Key:";
            this.lblToggleKey.Location = new Point(10, 25);
            this.lblToggleKey.Size = new Size(80, 23);
            
            this.cmbToggleKey.Items.AddRange(new[] { "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "SPACE", "ENTER" });
            this.cmbToggleKey.Location = new Point(100, 22);
            this.cmbToggleKey.Size = new Size(100, 23);
            this.cmbToggleKey.DropDownStyle = ComboBoxStyle.DropDownList;
            
            this.lblToggleDelay.Text = "Delay (ms):";
            this.lblToggleDelay.Location = new Point(10, 55);
            this.lblToggleDelay.Size = new Size(80, 23);
            
            this.numToggleDelay.Location = new Point(100, 52);
            this.numToggleDelay.Size = new Size(100, 23);
            this.numToggleDelay.Minimum = 0;
            this.numToggleDelay.Maximum = 5000;
            this.numToggleDelay.Value = 500;
            
            // Hide serial settings completely
            this.grpSerialSettings.Visible = false;
            this.grpSerialSettings.Location = new Point(6, 500);  // Move off-screen
            this.grpSerialSettings.Size = new Size(1, 1);
            
            this.btnTestNightMode.Text = "Test Night Mode Toggle";
            this.btnTestNightMode.Location = new Point(6, 178);
            this.btnTestNightMode.Size = new Size(180, 30);
            this.btnTestNightMode.Click += btnTestNightMode_Click;
            
            this.grpControllerType.Controls.Add(this.rbKeyboardEmulation);
            
            this.grpKeyboardSettings.Controls.Add(this.lblToggleKey);
            this.grpKeyboardSettings.Controls.Add(this.cmbToggleKey);
            this.grpKeyboardSettings.Controls.Add(this.lblToggleDelay);
            this.grpKeyboardSettings.Controls.Add(this.numToggleDelay);
            
            this.tabController.Controls.Add(this.grpControllerType);
            this.tabController.Controls.Add(this.grpKeyboardSettings);
            this.tabController.Controls.Add(this.btnTestNightMode);

            // 
            // General Tab
            // 
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            
            this.chkStartWithWindows.Text = "Start with Windows";
            this.chkStartWithWindows.Location = new Point(6, 6);
            this.chkStartWithWindows.Size = new Size(200, 24);
            
            this.chkStartMinimized.Text = "Start minimized (hide settings window on startup)";
            this.chkStartMinimized.Location = new Point(6, 36);
            this.chkStartMinimized.Size = new Size(350, 24);
            
            this.chkShowNotifications.Text = "Show notifications";
            this.chkShowNotifications.Location = new Point(6, 66);
            this.chkShowNotifications.Size = new Size(200, 24);
            
            this.lblCheckInterval.Text = "Check interval (seconds):";
            this.lblCheckInterval.Location = new Point(6, 96);
            this.lblCheckInterval.Size = new Size(150, 23);
            
            this.numCheckInterval.Location = new Point(162, 93);
            this.numCheckInterval.Size = new Size(80, 23);
            this.numCheckInterval.Minimum = 5;
            this.numCheckInterval.Maximum = 300;
            this.numCheckInterval.Value = 30;
            
            this.chkDebugMode.Text = "Debug mode";
            this.chkDebugMode.Location = new Point(6, 126);
            this.chkDebugMode.Size = new Size(200, 24);
            
            this.tabGeneral.Controls.Add(this.chkStartWithWindows);
            this.tabGeneral.Controls.Add(this.chkStartMinimized);
            this.tabGeneral.Controls.Add(this.chkShowNotifications);
            this.tabGeneral.Controls.Add(this.lblCheckInterval);
            this.tabGeneral.Controls.Add(this.numCheckInterval);
            this.tabGeneral.Controls.Add(this.chkDebugMode);

            // 
            // Form buttons
            // 
            this.btnSave.Text = "Save";
            this.btnSave.Location = new Point(620, 418);
            this.btnSave.Size = new Size(80, 30);
            this.btnSave.Click += btnSave_Click;
            
            this.btnClose.Text = "Close";
            this.btnClose.Location = new Point(708, 418);
            this.btnClose.Size = new Size(80, 30);
            this.btnClose.Click += btnClose_Click;
            
            this.btnResetDefaults.Text = "Reset Defaults";
            this.btnResetDefaults.Location = new Point(12, 418);
            this.btnResetDefaults.Size = new Size(100, 30);
            this.btnResetDefaults.Click += btnResetDefaults_Click;

            // 
            // SettingsForm
            // 
            this.ClientSize = new Size(800, 460);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnResetDefaults);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Auto Night Mode Settings";

            this.ResumeLayout(false);
        }
    }
}

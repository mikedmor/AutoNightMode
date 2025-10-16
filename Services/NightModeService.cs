using AutoNightMode.Models;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices;

namespace AutoNightMode.Services
{
    public class NightModeService
    {
        private AppConfiguration? _configuration;
        private NightModeState _currentState;
        private SerialPort? _serialPort;

        public event EventHandler<NightModeState>? StateChanged;
        public event EventHandler<string>? StatusMessage;

        // Windows API for sending keystrokes
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private const uint KEYEVENTF_KEYUP = 0x0002;

        public NightModeService()
        {
            _currentState = new NightModeState();
        }

        public void Initialize(AppConfiguration configuration)
        {
            _configuration = configuration;
            InitializeController();
        }

        public void UpdateConfiguration(AppConfiguration configuration)
        {
            _configuration = configuration;
            InitializeController();
        }

        private void InitializeController()
        {
            try
            {
                // Close existing serial connection if any
                _serialPort?.Close();
                _serialPort?.Dispose();
                _serialPort = null;

                if (_configuration?.Controller.UseSerialCommunication == true)
                {
                    InitializeSerialConnection();
                }

                StatusMessage?.Invoke(this, "Controller initialized");
            }
            catch (Exception ex)
            {
                StatusMessage?.Invoke(this, $"Controller initialization error: {ex.Message}");
            }
        }

        private void InitializeSerialConnection()
        {
            try
            {
                _serialPort = new SerialPort(
                    _configuration!.Controller.ControllerPort,
                    _configuration.Controller.ControllerBaudRate);

                _serialPort.Open();
                StatusMessage?.Invoke(this, $"Serial connection opened on {_configuration.Controller.ControllerPort}");
            }
            catch (Exception ex)
            {
                StatusMessage?.Invoke(this, $"Serial connection error: {ex.Message}");
                _serialPort = null;
            }
        }

        public async Task<NightModeStatus> CheckCurrentNightModeStatus()
        {
            try
            {
                if (_configuration?.Controller.UseSerialCommunication == true)
                {
                    return await CheckStatusViaSerial();
                }
                else
                {
                    // For Arnoz Dude Cab controller, we might need to check via other means
                    // This is a placeholder - you'll need to implement based on your controller's capabilities
                    return await CheckStatusViaProcess();
                }
            }
            catch (Exception ex)
            {
                StatusMessage?.Invoke(this, $"Status check error: {ex.Message}");
                return NightModeStatus.Unknown;
            }
        }

        private async Task<NightModeStatus> CheckStatusViaSerial()
        {
            if (_serialPort?.IsOpen != true)
                return NightModeStatus.Unknown;

            try
            {
                // Send status query command - adjust based on your controller's protocol
                _serialPort.WriteLine("STATUS?");
                await Task.Delay(100); // Wait for response

                if (_serialPort.BytesToRead > 0)
                {
                    var response = _serialPort.ReadLine();
                    // Parse response based on your controller's protocol
                    if (response.Contains("NIGHT:ON"))
                        return NightModeStatus.Enabled;
                    else if (response.Contains("NIGHT:OFF"))
                        return NightModeStatus.Disabled;
                }
            }
            catch (Exception ex)
            {
                StatusMessage?.Invoke(this, $"Serial status check error: {ex.Message}");
            }

            return NightModeStatus.Unknown;
        }

        private async Task<NightModeStatus> CheckStatusViaProcess()
        {
            // This is a placeholder implementation
            // You might need to check for specific processes, registry keys, or files
            // that indicate night mode status for your specific controller

            await Task.Delay(10); // Placeholder

            // Example: Check if a specific process is running that indicates night mode
            var processes = Process.GetProcessesByName("ArnozDudeCab"); // Adjust process name
            if (processes.Length > 0)
            {
                // Could check command line arguments, window titles, etc.
                // This is very controller-specific
            }

            return NightModeStatus.Unknown;
        }

        public async Task<bool> ToggleNightMode(bool enableNightMode)
        {
            try
            {
                _currentState.LastToggled = DateTime.Now;
                _currentState.Source = "Application";

                if (_configuration?.Controller.UseSerialCommunication == true)
                {
                    return await ToggleViaSerial(enableNightMode);
                }
                else if (_configuration?.Controller.UseKeyboardEmulation == true)
                {
                    return await ToggleViaKeyboard();
                }

                return false;
            }
            catch (Exception ex)
            {
                StatusMessage?.Invoke(this, $"Toggle error: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> ToggleViaSerial(bool enableNightMode)
        {
            if (_serialPort?.IsOpen != true)
            {
                StatusMessage?.Invoke(this, "Serial port not available");
                return false;
            }

            try
            {
                var command = enableNightMode ? "NIGHT:ON" : "NIGHT:OFF";
                _serialPort.WriteLine(command);
                await Task.Delay(_configuration!.Controller.ToggleDelay);

                StatusMessage?.Invoke(this, $"Sent serial command: {command}");
                return true;
            }
            catch (Exception ex)
            {
                StatusMessage?.Invoke(this, $"Serial toggle error: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> ToggleViaKeyboard()
        {
            try
            {
                var keyCode = GetKeyCode(_configuration!.Controller.ToggleKeyCode);
                if (keyCode == 0)
                {
                    StatusMessage?.Invoke(this, $"Invalid key code: {_configuration.Controller.ToggleKeyCode}");
                    return false;
                }

                // Send key down
                keybd_event(keyCode, 0, 0, UIntPtr.Zero);
                await Task.Delay(50);

                // Send key up
                keybd_event(keyCode, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                await Task.Delay(_configuration.Controller.ToggleDelay);

                StatusMessage?.Invoke(this, $"Sent keyboard command: {_configuration.Controller.ToggleKeyCode}");
                return true;
            }
            catch (Exception ex)
            {
                StatusMessage?.Invoke(this, $"Keyboard toggle error: {ex.Message}");
                return false;
            }
        }

        private byte GetKeyCode(string keyName)
        {
            // Map common key names to virtual key codes
            var keyMap = new Dictionary<string, byte>
            {
                ["F1"] = 0x70, ["F2"] = 0x71, ["F3"] = 0x72, ["F4"] = 0x73,
                ["F5"] = 0x74, ["F6"] = 0x75, ["F7"] = 0x76, ["F8"] = 0x77,
                ["F9"] = 0x78, ["F10"] = 0x79, ["F11"] = 0x7A, ["F12"] = 0x7B,
                ["SPACE"] = 0x20, ["ENTER"] = 0x0D, ["ESC"] = 0x1B,
                ["TAB"] = 0x09, ["SHIFT"] = 0x10, ["CTRL"] = 0x11, ["ALT"] = 0x12
            };

            return keyMap.TryGetValue(keyName.ToUpper(), out var code) ? code : (byte)0;
        }

        public async Task HandleScheduledToggle(bool shouldEnableNightMode)
        {
            try
            {
                var currentStatus = await CheckCurrentNightModeStatus();

                // If status cannot be determined, prefer using the last known state to
                // make a safe decision. If we have no last known state, skip the
                // scheduled action to avoid blind repeated toggles.
                if (currentStatus == NightModeStatus.Unknown)
                {
                    if (_currentState.Status != NightModeStatus.Unknown)
                    {
                        currentStatus = _currentState.Status;
                        StatusMessage?.Invoke(this, "Status unknown — using last known state for scheduled decision");
                    }
                    else
                    {
                        StatusMessage?.Invoke(this, "Status unknown — skipping scheduled action to avoid blind toggles");
                        return;
                    }
                }

                _currentState.Status = currentStatus;
                _currentState.LastChecked = DateTime.Now;

                bool needsToggle = false;
                string action = "";

                if (shouldEnableNightMode && currentStatus != NightModeStatus.Enabled)
                {
                    needsToggle = true;
                    action = "enable";
                }
                else if (!shouldEnableNightMode && currentStatus != NightModeStatus.Disabled)
                {
                    needsToggle = true;
                    action = "disable";
                }

                if (needsToggle)
                {
                    StatusMessage?.Invoke(this, $"Scheduled action: attempting to {action} night mode");
                    var success = await ToggleNightMode(shouldEnableNightMode);
                    
                    if (success)
                    {
                        _currentState.Status = shouldEnableNightMode ? NightModeStatus.Enabled : NightModeStatus.Disabled;
                        _currentState.Source = "Scheduled";
                        StatusMessage?.Invoke(this, $"Successfully {action}d night mode");
                    }
                }
                else
                {
                    StatusMessage?.Invoke(this, $"Night mode already in correct state ({currentStatus})");
                }

                StateChanged?.Invoke(this, _currentState);
            }
            catch (Exception ex)
            {
                StatusMessage?.Invoke(this, $"Scheduled toggle error: {ex.Message}");
            }
        }

        public NightModeState GetCurrentState() => _currentState;

        public void Dispose()
        {
            _serialPort?.Close();
            _serialPort?.Dispose();
        }
    }
}

using System.IO.Ports;
using System.Text;

namespace AutoNightMode.Forms
{
    public partial class SerialMonitorForm : Form
    {
        private SerialPort? _serialPort;
        private bool _isConnected = false;
        private readonly object _logLock = new object();
        private StringBuilder _logBuffer = new StringBuilder();

        public SerialMonitorForm()
        {
            InitializeComponent();
            LoadAvailablePorts();
            UpdateConnectionState();
        }

        private void LoadAvailablePorts()
        {
            cmbPort.Items.Clear();
            var ports = SerialPort.GetPortNames();
            
            if (ports.Length == 0)
            {
                cmbPort.Items.Add("No ports available");
                cmbPort.SelectedIndex = 0;
                cmbPort.Enabled = false;
            }
            else
            {
                foreach (var port in ports)
                {
                    cmbPort.Items.Add(port);
                }
                if (cmbPort.Items.Count > 0)
                    cmbPort.SelectedIndex = 0;
                cmbPort.Enabled = true;
            }
        }

        private void btnRefreshPorts_Click(object sender, EventArgs e)
        {
            LoadAvailablePorts();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (_isConnected)
            {
                Disconnect();
            }
            else
            {
                Connect();
            }
        }

        private void Connect()
        {
            try
            {
                if (cmbPort.SelectedItem == null || cmbPort.SelectedItem.ToString() == "No ports available")
                {
                    MessageBox.Show("Please select a valid COM port.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (cmbBaudRate.SelectedItem == null || cmbDataBits.SelectedItem == null ||
                    cmbStopBits.SelectedItem == null || cmbParity.SelectedItem == null)
                {
                    MessageBox.Show("Please ensure all connection settings are selected.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _serialPort = new SerialPort
                {
                    PortName = cmbPort.SelectedItem.ToString()!,
                    BaudRate = int.Parse(cmbBaudRate.SelectedItem.ToString()!),
                    DataBits = int.Parse(cmbDataBits.SelectedItem.ToString()!),
                    StopBits = (StopBits)Enum.Parse(typeof(StopBits), cmbStopBits.SelectedItem.ToString()!),
                    Parity = (Parity)Enum.Parse(typeof(Parity), cmbParity.SelectedItem.ToString()!),
                    ReadTimeout = 3000,
                    WriteTimeout = 3000,
                    DtrEnable = true,
                    RtsEnable = true
                };

                _serialPort.DataReceived += SerialPort_DataReceived;
                _serialPort.Open();
                
                _isConnected = true;
                UpdateConnectionState();
                LogMessage($"Connected to {_serialPort.PortName} at {_serialPort.BaudRate} baud", MessageType.System);
                LogMessage($"Note: Only RX (received) data will be visible. TX from other apps cannot be monitored due to Windows COM port exclusivity.", MessageType.System);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect: {ex.Message}", "Connection Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _isConnected = false;
                UpdateConnectionState();
            }
        }

        private void Disconnect()
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.DataReceived -= SerialPort_DataReceived;
                    _serialPort.Close();
                    _serialPort.Dispose();
                    _serialPort = null;
                }
                
                _isConnected = false;
                UpdateConnectionState();
                LogMessage("Disconnected", MessageType.System);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during disconnect: {ex.Message}", "Disconnect Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateConnectionState()
        {
            btnConnect.Text = _isConnected ? "Disconnect" : "Connect";
            btnConnect.BackColor = _isConnected ? Color.Tomato : Color.LightGreen;
            
            cmbPort.Enabled = !_isConnected;
            cmbBaudRate.Enabled = !_isConnected;
            cmbDataBits.Enabled = !_isConnected;
            cmbStopBits.Enabled = !_isConnected;
            cmbParity.Enabled = !_isConnected;
            btnRefreshPorts.Enabled = !_isConnected;
            
            txtSendData.Enabled = _isConnected;
            btnSend.Enabled = _isConnected;
            chkAppendNewline.Enabled = _isConnected;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    string data = _serialPort.ReadExisting();
                    if (!string.IsNullOrEmpty(data))
                    {
                        this.Invoke(() => LogMessage(data, MessageType.Received));
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // Port was closed, ignore
            }
            catch (TimeoutException)
            {
                // Read timeout, ignore and wait for next event
            }
            catch (Exception ex)
            {
                try
                {
                    this.Invoke(() => LogMessage($"Error reading data: {ex.Message}", MessageType.Error));
                }
                catch
                {
                    // Form may be closing, ignore
                }
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendData();
        }

        private void txtSendData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && _isConnected)
            {
                e.SuppressKeyPress = true;
                SendData();
            }
        }

        private void SendData()
        {
            if (!_isConnected || _serialPort == null || !_serialPort.IsOpen)
                return;

            try
            {
                string data = txtSendData.Text;
                
                if (string.IsNullOrEmpty(data))
                    return;

                // Send the data without clearing buffers (don't interfere with device protocol)
                if (chkAppendNewline.Checked)
                {
                    _serialPort.WriteLine(data);
                }
                else
                {
                    _serialPort.Write(data);
                }

                LogMessage(data, MessageType.Sent);
                txtSendData.Clear();
                txtSendData.Focus();
            }
            catch (TimeoutException)
            {
                LogMessage($"Send timeout - device may not be responding or is busy", MessageType.Error);
            }
            catch (InvalidOperationException ex)
            {
                LogMessage($"Port closed or unavailable: {ex.Message}", MessageType.Error);
                Disconnect();
            }
            catch (Exception ex)
            {
                LogMessage($"Error sending data: {ex.Message}", MessageType.Error);
            }
        }

        private void LogMessage(string message, MessageType type)
        {
            lock (_logLock)
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                string prefix = type switch
                {
                    MessageType.Sent => "TX",
                    MessageType.Received => "RX",
                    MessageType.System => "SYS",
                    MessageType.Error => "ERR",
                    _ => "   "
                };

                Color color = type switch
                {
                    MessageType.Sent => Color.Blue,
                    MessageType.Received => Color.Green,
                    MessageType.System => Color.Gray,
                    MessageType.Error => Color.Red,
                    _ => Color.Black
                };

                // Append to buffer
                _logBuffer.AppendLine($"[{timestamp}] {prefix}: {message}");

                // Update UI
                if (txtLog.InvokeRequired)
                {
                    txtLog.Invoke(() => AppendToLog(timestamp, prefix, message, color));
                }
                else
                {
                    AppendToLog(timestamp, prefix, message, color);
                }
            }
        }

        private void AppendToLog(string timestamp, string prefix, string message, Color color)
        {
            int startIndex = txtLog.TextLength;
            txtLog.AppendText($"[{timestamp}] {prefix}: {message}\n");
            
            // Color the prefix
            txtLog.Select(startIndex + timestamp.Length + 3, prefix.Length);
            txtLog.SelectionColor = color;
            
            // Reset selection
            txtLog.SelectionLength = 0;
            txtLog.SelectionColor = txtLog.ForeColor;
            
            // Auto-scroll to bottom
            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.ScrollToCaret();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
            lock (_logLock)
            {
                _logBuffer.Clear();
            }
        }

        private void btnSaveLog_Click(object sender, EventArgs e)
        {
            using var saveDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|Log Files (*.log)|*.log|All Files (*.*)|*.*",
                DefaultExt = "txt",
                FileName = $"SerialLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    lock (_logLock)
                    {
                        File.WriteAllText(saveDialog.FileName, _logBuffer.ToString());
                    }
                    MessageBox.Show($"Log saved to:\n{saveDialog.FileName}", "Log Saved", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving log: {ex.Message}", "Save Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Disconnect();
            base.OnFormClosing(e);
        }

        private enum MessageType
        {
            Sent,
            Received,
            System,
            Error
        }
    }
}

namespace AutoNightMode.Forms
{
    partial class SerialMonitorForm
    {
        private System.ComponentModel.IContainer components = null;
        private ComboBox cmbPort;
        private ComboBox cmbBaudRate;
        private ComboBox cmbDataBits;
        private ComboBox cmbStopBits;
        private ComboBox cmbParity;
        private Button btnConnect;
        private Button btnRefreshPorts;
        private RichTextBox txtLog;
        private TextBox txtSendData;
        private Button btnSend;
        private Button btnClear;
        private Button btnSaveLog;
        private CheckBox chkAppendNewline;
        private GroupBox grpConnection;
        private GroupBox grpLog;
        private GroupBox grpSend;
        private Label lblPort;
        private Label lblBaudRate;
        private Label lblDataBits;
        private Label lblStopBits;
        private Label lblParity;

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
            this.components = new System.ComponentModel.Container();
            
            // Form
            this.Text = "Serial Monitor";
            this.ClientSize = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 500);

            // Connection Group
            this.grpConnection = new GroupBox();
            this.grpConnection.Text = "Connection Settings";
            this.grpConnection.Location = new Point(12, 12);
            this.grpConnection.Size = new Size(876, 100);
            this.grpConnection.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Port Label and ComboBox
            this.lblPort = new Label();
            this.lblPort.Text = "Port:";
            this.lblPort.Location = new Point(10, 25);
            this.lblPort.Size = new Size(40, 23);

            this.cmbPort = new ComboBox();
            this.cmbPort.Location = new Point(55, 22);
            this.cmbPort.Size = new Size(100, 23);
            this.cmbPort.DropDownStyle = ComboBoxStyle.DropDownList;

            // Refresh Ports Button
            this.btnRefreshPorts = new Button();
            this.btnRefreshPorts.Text = "🔄";
            this.btnRefreshPorts.Location = new Point(160, 22);
            this.btnRefreshPorts.Size = new Size(30, 23);
            this.btnRefreshPorts.Click += btnRefreshPorts_Click;

            // Baud Rate Label and ComboBox
            this.lblBaudRate = new Label();
            this.lblBaudRate.Text = "Baud:";
            this.lblBaudRate.Location = new Point(200, 25);
            this.lblBaudRate.Size = new Size(40, 23);

            this.cmbBaudRate = new ComboBox();
            this.cmbBaudRate.Location = new Point(245, 22);
            this.cmbBaudRate.Size = new Size(90, 23);
            this.cmbBaudRate.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbBaudRate.Items.AddRange(new object[] { 
                "300", "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200", "230400" 
            });
            this.cmbBaudRate.SelectedIndex = 4; // 9600

            // Data Bits Label and ComboBox
            this.lblDataBits = new Label();
            this.lblDataBits.Text = "Data:";
            this.lblDataBits.Location = new Point(345, 25);
            this.lblDataBits.Size = new Size(35, 23);

            this.cmbDataBits = new ComboBox();
            this.cmbDataBits.Location = new Point(385, 22);
            this.cmbDataBits.Size = new Size(60, 23);
            this.cmbDataBits.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbDataBits.Items.AddRange(new object[] { "7", "8" });
            this.cmbDataBits.SelectedIndex = 1; // 8

            // Stop Bits Label and ComboBox
            this.lblStopBits = new Label();
            this.lblStopBits.Text = "Stop:";
            this.lblStopBits.Location = new Point(455, 25);
            this.lblStopBits.Size = new Size(35, 23);

            this.cmbStopBits = new ComboBox();
            this.cmbStopBits.Location = new Point(495, 22);
            this.cmbStopBits.Size = new Size(60, 23);
            this.cmbStopBits.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStopBits.Items.AddRange(new object[] { "None", "One", "Two", "OnePointFive" });
            this.cmbStopBits.SelectedIndex = 1; // One

            // Parity Label and ComboBox
            this.lblParity = new Label();
            this.lblParity.Text = "Parity:";
            this.lblParity.Location = new Point(565, 25);
            this.lblParity.Size = new Size(45, 23);

            this.cmbParity = new ComboBox();
            this.cmbParity.Location = new Point(615, 22);
            this.cmbParity.Size = new Size(80, 23);
            this.cmbParity.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbParity.Items.AddRange(new object[] { "None", "Odd", "Even", "Mark", "Space" });
            this.cmbParity.SelectedIndex = 0; // None

            // Connect Button
            this.btnConnect = new Button();
            this.btnConnect.Text = "Connect";
            this.btnConnect.Location = new Point(720, 22);
            this.btnConnect.Size = new Size(140, 60);
            this.btnConnect.BackColor = Color.LightGreen;
            this.btnConnect.Font = new Font(this.btnConnect.Font.FontFamily, 12, FontStyle.Bold);
            this.btnConnect.Click += btnConnect_Click;

            // Add controls to connection group
            this.grpConnection.Controls.Add(this.lblPort);
            this.grpConnection.Controls.Add(this.cmbPort);
            this.grpConnection.Controls.Add(this.btnRefreshPorts);
            this.grpConnection.Controls.Add(this.lblBaudRate);
            this.grpConnection.Controls.Add(this.cmbBaudRate);
            this.grpConnection.Controls.Add(this.lblDataBits);
            this.grpConnection.Controls.Add(this.cmbDataBits);
            this.grpConnection.Controls.Add(this.lblStopBits);
            this.grpConnection.Controls.Add(this.cmbStopBits);
            this.grpConnection.Controls.Add(this.lblParity);
            this.grpConnection.Controls.Add(this.cmbParity);
            this.grpConnection.Controls.Add(this.btnConnect);

            // Log Group
            this.grpLog = new GroupBox();
            this.grpLog.Text = "Communication Log";
            this.grpLog.Location = new Point(12, 118);
            this.grpLog.Size = new Size(876, 380);
            this.grpLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // Log TextBox
            this.txtLog = new RichTextBox();
            this.txtLog.Location = new Point(10, 22);
            this.txtLog.Size = new Size(856, 320);
            this.txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.txtLog.Font = new Font("Consolas", 9);
            this.txtLog.ReadOnly = true;
            this.txtLog.BackColor = Color.Black;
            this.txtLog.ForeColor = Color.White;
            this.txtLog.WordWrap = false;

            // Clear Button
            this.btnClear = new Button();
            this.btnClear.Text = "Clear Log";
            this.btnClear.Location = new Point(10, 348);
            this.btnClear.Size = new Size(100, 25);
            this.btnClear.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnClear.Click += btnClear_Click;

            // Save Log Button
            this.btnSaveLog = new Button();
            this.btnSaveLog.Text = "Save Log";
            this.btnSaveLog.Location = new Point(115, 348);
            this.btnSaveLog.Size = new Size(100, 25);
            this.btnSaveLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnSaveLog.Click += btnSaveLog_Click;

            // Add controls to log group
            this.grpLog.Controls.Add(this.txtLog);
            this.grpLog.Controls.Add(this.btnClear);
            this.grpLog.Controls.Add(this.btnSaveLog);

            // Send Group
            this.grpSend = new GroupBox();
            this.grpSend.Text = "Send Data";
            this.grpSend.Location = new Point(12, 504);
            this.grpSend.Size = new Size(876, 70);
            this.grpSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // Send TextBox
            this.txtSendData = new TextBox();
            this.txtSendData.Location = new Point(10, 22);
            this.txtSendData.Size = new Size(720, 23);
            this.txtSendData.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.txtSendData.Font = new Font("Consolas", 9);
            this.txtSendData.KeyDown += txtSendData_KeyDown;

            // Append Newline Checkbox
            this.chkAppendNewline = new CheckBox();
            this.chkAppendNewline.Text = "Append \\r\\n";
            this.chkAppendNewline.Location = new Point(10, 48);
            this.chkAppendNewline.Size = new Size(100, 20);
            this.chkAppendNewline.Checked = true;

            // Send Button
            this.btnSend = new Button();
            this.btnSend.Text = "Send";
            this.btnSend.Location = new Point(736, 22);
            this.btnSend.Size = new Size(130, 23);
            this.btnSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnSend.Click += btnSend_Click;

            // Add controls to send group
            this.grpSend.Controls.Add(this.txtSendData);
            this.grpSend.Controls.Add(this.chkAppendNewline);
            this.grpSend.Controls.Add(this.btnSend);

            // Add all groups to form
            this.Controls.Add(this.grpConnection);
            this.Controls.Add(this.grpLog);
            this.Controls.Add(this.grpSend);

            this.ResumeLayout(false);
        }
    }
}

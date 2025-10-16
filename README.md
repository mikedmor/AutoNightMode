# Auto Night Mode Controller

A Windows application for automatically controlling night mode on pinball tables, specifically designed for Arnoz Dude Cab controllers.

## Features

- **Automatic Scheduling**: Set specific times for night mode to enable/disable
- **Controller Support**: Works with keyboard emulation or serial communication
- **Smart Detection**: Checks current night mode status to avoid unnecessary toggles
- **System Tray Operation**: Runs quietly in the background
- **Windows Startup**: Optionally starts with Windows
- **Customizable Schedule**: Set different schedules for different days
- **Easy Configuration**: Simple settings interface

## Installation

### Method 1: Using Setup Script (Recommended)
1. Download or clone this repository
2. Open command prompt in the project directory
3. Run: `setup.bat`
4. Follow the guided setup process

### Method 2: Manual Build
1. Install .NET 8.0 SDK from https://dotnet.microsoft.com/download/dotnet/8.0
2. Open command prompt in the project directory
3. Run: `dotnet build --configuration Release`
4. The executable will be in `bin\Release\net8.0-windows\`

## Getting Started

### Quick Start
```cmd
setup.bat
```
This single script will:
- ✅ Check your .NET installation status
- ✅ Guide you through .NET 8.0 SDK installation if needed
- ✅ Build the application
- ✅ Create a portable package for distribution

### Manual Steps
If you prefer to do it manually:
1. **Install .NET 8.0 SDK**: https://dotnet.microsoft.com/download/dotnet/8.0
2. **Build**: `dotnet build --configuration Release`
3. **Run**: `bin\Release\net8.0-windows\AutoNightMode.exe`

### Schedule Settings
- **Enable automatic scheduling**: Turn the scheduler on/off
- **Start/End times**: When night mode should activate/deactivate
- **Active days**: Which days of the week the schedule applies

### Controller Settings
Choose between two control methods:

#### Keyboard Emulation (Default)
- **Toggle Key**: Which key to simulate (default: F12)
- **Delay**: How long to wait after sending the key

#### Serial Communication
- **Port**: COM port your controller is connected to
- **Baud Rate**: Communication speed (usually 9600)

### General Settings
- **Start with Windows**: Automatically start when Windows boots
- **Start minimized**: Start in system tray
- **Show notifications**: Display status messages
- **Check interval**: How often to check the schedule (in seconds)
- **Debug mode**: Show detailed status messages

## Configuration

### First Time Setup
1. Right-click the system tray icon and select "Show Settings"
2. Configure your schedule in the "Schedule" tab
3. Set up your controller in the "Controller" tab
4. Adjust general settings in the "General" tab

## Usage

### System Tray Menu
Right-click the system tray icon to access:
- **Show Settings**: Open configuration window
- **Toggle Night Mode**: Manually toggle night mode
- **Enable/Disable Scheduler**: Turn automatic scheduling on/off
- **Exit**: Close the application

### Automatic Operation
Once configured, the application will:
1. Check your schedule every few seconds (configurable)
2. At transition times, check if night mode needs to be toggled
3. Only send toggle commands when needed (smart detection)
4. Show notifications when actions are taken

## Controller Compatibility

### Arnoz Dude Cab Controller
This application is designed primarily for Arnoz Dude Cab controllers. The default configuration uses:
- **Keyboard emulation** with the **F12** key
- This should trigger the night mode toggle in most setups

### Other Controllers
You can adapt this for other controllers by:
1. Changing the toggle key in settings
2. Using serial communication if your controller supports it
3. Modifying the source code for custom protocols

## Troubleshooting

### Night Mode Not Toggling
1. **Test the toggle**: Use "Test Night Mode Toggle" button in controller settings
2. **Check the key**: Ensure the toggle key matches your controller's configuration
3. **Verify timing**: Make sure the schedule times are correct
4. **Check permissions**: Run as administrator if needed

### Serial Communication Issues
1. **Verify port**: Check Device Manager for the correct COM port
2. **Check connections**: Ensure the controller is properly connected
3. **Test baud rate**: Try different baud rates (9600, 19200, 38400)
4. **Driver issues**: Make sure controller drivers are installed

### Application Not Starting
1. **Install .NET 8**: Download from Microsoft's website
2. **Check antivirus**: Some antivirus software may block the application
3. **Run as administrator**: Right-click and "Run as administrator"

## Files and Folders

The application creates the following:
- **Configuration**: `%APPDATA%\AutoNightMode\config.json`
- **Registry entry**: For Windows startup (if enabled)

## Development

### Building
```bash
dotnet build --configuration Release
```

### Publishing
```bash
dotnet publish --configuration Release --self-contained true --runtime win-x64
```

### Contributing
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support, please:
1. Check this README for common solutions
2. Create an issue on GitHub with:
   - Your controller model
   - Configuration settings (without sensitive info)
   - Error messages or unexpected behavior
   - Steps to reproduce the issue

## Version History

### v1.0.0
- Initial release
- Basic scheduling functionality
- Keyboard emulation support
- Serial communication support
- System tray operation
- Windows startup integration

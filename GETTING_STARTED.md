# Getting Started - .NET Installation Guide

## Quick Start Options

### Option 1: Install .NET 8.0 SDK (Recommended for building from source)

1. **Download .NET 8.0 SDK**:
   - Go to: https://dotnet.microsoft.com/download/dotnet/8.0
   - Click "Download .NET 8.0 SDK" (not just runtime)
   - Choose the installer for your system (x64 for most modern PCs)

2. **Install**:
   - Run the downloaded installer
   - Follow the installation wizard (default settings are fine)
   - Restart your command prompt/terminal after installation

3. **Verify Installation**:
   ```cmd
   dotnet --version
   ```
   Should show version 8.0.x

4. **Build the Application**:
   ```cmd
   setup.bat
   ```
   Or directly:
   ```cmd
   build.bat
   ```

### Option 2: Use Pre-built Release (No .NET installation required)

1. **Download Release**:
   - Go to the GitHub releases page
   - Download the latest `AutoNightMode-Release.zip`
   - Extract to your desired location

2. **Run**:
   - No installation needed
   - Double-click `AutoNightMode.exe`
   - Configure via system tray icon

## Troubleshooting .NET Installation

### "No .NET SDKs were found" Error

**Cause**: .NET SDK is not installed or not in PATH

**Solutions**:
1. Make sure you downloaded the **SDK**, not just the runtime
2. Restart your command prompt after installation
3. Restart your computer if the problem persists
4. Check if .NET is in your PATH:
   - Open Command Prompt as Administrator
   - Type: `where dotnet`
   - Should show path like `C:\Program Files\dotnet\dotnet.exe`

### Installation Verification Steps

1. **Check Windows Version**:
   ```cmd
   ver
   ```
   Should be Windows 10 version 1607+ or Windows 11

2. **Check Available Space**:
   - Need at least 500MB for .NET SDK
   - Need additional space for your application

3. **Check Installation**:
   ```cmd
   dotnet --info
   ```
   Should show detailed information about installed SDKs and runtimes

### Alternative: Using Visual Studio

If you prefer a full development environment:

1. **Download Visual Studio Community** (free):
   - https://visualstudio.microsoft.com/vs/community/

2. **During Installation**:
   - Select ".NET desktop development" workload
   - This includes .NET 8.0 SDK automatically

3. **Open Project**:
   - Open `AutoNightMode.csproj` in Visual Studio
   - Press F5 to build and run

## System Requirements

### Minimum Requirements
- **OS**: Windows 10 version 1607 or later, Windows 11
- **Architecture**: x64 (64-bit)
- **RAM**: 512 MB available
- **Disk Space**: 500 MB for .NET SDK + 50 MB for application

### Recommended Requirements
- **OS**: Windows 10 version 20H2 or later, Windows 11
- **RAM**: 1 GB available
- **Disk Space**: 1 GB free space

## Building Options

### Quick Build (Debug)
```cmd
dotnet build
```

### Release Build
```cmd
dotnet build --configuration Release
```

### Self-Contained Executable
```cmd
dotnet publish --configuration Release --self-contained true --runtime win-x64
```

### Portable Package
```cmd
build.bat
```
Then select "y" when prompted to create portable package.

## Getting Help

If you're still having issues:

1. **Check this guide first** - most issues are covered here
2. **Run the setup script**: `setup.bat` - it will diagnose common problems
3. **Check the main README.md** for application-specific help
4. **Create an issue on GitHub** with:
   - Your Windows version (`ver` command output)
   - .NET version (`dotnet --version` output)
   - Complete error message
   - Steps you've already tried

## Quick Reference

| Task | Command |
|------|---------|
| Check .NET version | `dotnet --version` |
| Build application | `dotnet build` |
| Run setup wizard | `setup.bat` |
| Build with script | `build.bat` |
| Create release | `publish.bat` |
| Quick test | `dotnet run` |

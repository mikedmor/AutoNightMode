@echo off

echo =================================================================
echo           Auto Night Mode Controller - Setup and Build  
echo =================================================================
echo.

:menu
echo What would you like to do?
echo.
echo 1. Build the application
echo 2. Check .NET status
echo 3. Install .NET 8.0 SDK  
echo 4. Open .NET download page
echo 5. Package Release (create zip)
echo 6. Exit
echo.

set /p choice=Enter your choice (1-6): 

if "%choice%"=="1" goto build
if "%choice%"=="2" goto status
if "%choice%"=="3" goto install
if "%choice%"=="4" goto download
if "%choice%"=="5" goto package
if "%choice%"=="6" goto exit

echo.
echo Invalid choice. Please try again.
echo.
goto menu

:build
echo.
echo =================================================================
echo                    Building Application
echo =================================================================
echo.

REM Check for .NET 8.0 SDK
dotnet --list-sdks | findstr /C:"8." > nul 2>&1
if errorlevel 1 (
    echo [X] .NET 8.0 SDK not found!
    echo.
    echo You need to install .NET 8.0 SDK first.
    echo Use option 3 to install it.
    echo.
    pause
    goto menu
)

echo [OK] .NET 8.0 SDK found. Building...
echo.

REM Clean previous builds
if exist "bin\Release" rmdir /s /q "bin\Release"
if exist "obj" rmdir /s /q "obj"

REM Build the .NET 8.0 project specifically
echo Building AutoNightMode.csproj...
dotnet build AutoNightMode.csproj --configuration Release --verbosity minimal

if errorlevel 1 (
    echo.
    echo [X] Build failed with .NET SDK error.
    echo.
    echo This appears to be a .NET SDK corruption issue.
    echo Here are the steps to fix it:
    echo.
    echo 1. Uninstall .NET 8.0 SDK:
    echo    - Go to Settings ^> Apps ^> Apps ^& Features
    echo    - Search for "Microsoft .NET SDK 8"
    echo    - Uninstall it
    echo.
    echo 2. Download and reinstall .NET 8.0 SDK:
    echo    - Use option 4 to open download page
    echo    - Download fresh installer
    echo    - Run installer with default settings
    echo.
    echo 3. Restart command prompt and try building again
    echo.
    echo Alternative: Try using Visual Studio to build the project
    echo if you have it installed.
    echo.
    pause
    goto menu
)

REM Check if executable exists
if exist "bin\Release\net8.0-windows\AutoNightMode.exe" (
    echo.
    echo [OK] Build successful!
    echo [OK] Executable: bin\Release\net8.0-windows\AutoNightMode.exe
    echo.
) else (
    echo.
    echo [X] Build completed but executable not found.
    echo Expected: bin\Release\net8.0-windows\AutoNightMode.exe
)

echo.
pause
goto menu

:package
echo.
echo =================================================================
echo                  Package Release Build
echo =================================================================
echo.

REM Check if Release build exists
if not exist "bin\Release\net8.0-windows\AutoNightMode.exe" (
    echo [X] Release build not found!
    echo.
    echo Please build the application first ^(option 1^).
    echo.
    pause
    goto menu
)

echo [OK] Release build found.
echo.

REM Get version or use timestamp
for /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%c%%a%%b)
for /f "tokens=1-2 delims=/:" %%a in ('time /t') do (set mytime=%%a%%b)
set mytime=%mytime: =0%

set PACKAGE_NAME=AutoNightMode-v1.0-%mydate%-%mytime%

echo Creating package: %PACKAGE_NAME%.zip
echo.

REM Create temporary packaging directory
if exist "temp_package" rmdir /s /q "temp_package"
mkdir "temp_package"
mkdir "temp_package\AutoNightMode"

REM Copy release files
echo Copying files...
xcopy "bin\Release\net8.0-windows\*.*" "temp_package\AutoNightMode\" /E /I /Y > nul

REM Copy readme and other docs
if exist "README.md" copy "README.md" "temp_package\AutoNightMode\" > nul
if exist "GETTING_STARTED.md" copy "GETTING_STARTED.md" "temp_package\AutoNightMode\" > nul

REM Create zip using PowerShell
echo Compressing to zip file...
powershell -command "Compress-Archive -Path 'temp_package\AutoNightMode\*' -DestinationPath '%PACKAGE_NAME%.zip' -Force"

if errorlevel 1 (
    echo.
    echo [X] Failed to create zip file.
    echo.
    rmdir /s /q "temp_package"
    pause
    goto menu
)

REM Cleanup
rmdir /s /q "temp_package"

echo.
echo [OK] Package created successfully!
echo [OK] File: %PACKAGE_NAME%.zip
echo.
echo Package contents:
echo - AutoNightMode.exe
echo - All runtime dependencies
echo - Configuration files
echo - Documentation ^(README.md, GETTING_STARTED.md^)
echo.

pause
goto menu

:status
echo.
echo =================================================================
echo                 .NET Installation Status
echo =================================================================
echo.

echo Testing .NET installation...
echo.

REM Basic test
dotnet --version > nul 2>&1
if errorlevel 1 (
    echo [X] .NET not found or not working
    echo.
    pause
    goto menu
)

echo [OK] .NET is working
echo.

REM Check SDKs
echo Installed SDKs:
dotnet --list-sdks 2>&1
echo.

REM Check for .NET 8.0 SDK specifically
dotnet --list-sdks | findstr /C:"8." > nul 2>&1
if errorlevel 1 (
    echo [X] .NET 8.0 SDK MISSING - needed for building
) else (
    echo [OK] .NET 8.0 SDK FOUND - ready to build!
)

echo.
echo Installed Runtimes:
dotnet --list-runtimes 2>&1

echo.
pause
goto menu

:install
echo.
echo =================================================================
echo                Install .NET 8.0 SDK
echo =================================================================
echo.

REM Check if already installed
dotnet --list-sdks | findstr /C:"8." > nul 2>&1
if not errorlevel 1 (
    echo [OK] .NET 8.0 SDK already installed!
    echo You can go back and build the application.
    echo.
    pause
    goto menu
)

echo The .NET 8.0 SDK (about 200MB) is needed to build this application.
echo It's safe to install alongside your existing .NET installation.
echo.

echo Choose installation method:
echo.
echo 1. Open download page in browser (recommended)
echo 2. Back to main menu
echo.

set /p install_choice=Enter choice (1-2): 

if "%install_choice%"=="1" goto download
if "%install_choice%"=="2" goto menu

echo Invalid choice.
goto install

:download
echo.
echo Opening .NET 8.0 SDK download page...
start https://dotnet.microsoft.com/download/dotnet/8.0

echo.
echo [OK] Download page opened in your browser.
echo.
echo Instructions:
echo 1. Find ".NET 8.0 SDK" section (NOT runtime)
echo 2. Click "Download x64" 
echo 3. Run the installer when downloaded
echo 4. Use default settings (just click Next/Install)
echo 5. After installation, restart this command prompt
echo 6. Come back and use option 1 to build
echo.

pause
goto menu

:exit
echo.
echo Thanks for using Auto Night Mode Controller!
echo.
pause

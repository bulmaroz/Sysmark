# Script de Instalación Automatizada - SysMark Moderno
# Ejecutar como Administrador

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host " SysMark Moderno - Instalador" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si se ejecuta como administrador
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "❌ ADVERTENCIA: Este script debe ejecutarse como Administrador" -ForegroundColor Red
    Write-Host "   Click derecho > Ejecutar como Administrador" -ForegroundColor Yellow
    pause
    exit
}

# 1. Verificar .NET 8.0
Write-Host "🔍 Verificando .NET 8.0 Runtime..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version 2>$null

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ .NET no está instalado" -ForegroundColor Red
    Write-Host "📥 Descargando .NET 8.0 Desktop Runtime..." -ForegroundColor Yellow
    
    $dotnetUrl = "https://download.visualstudio.microsoft.com/download/pr/907765b0-2bf8-494e-93aa-5ef9553c5d68/a9308dc010617e6716c0e6abd53b05ce/windowsdesktop-runtime-8.0.0-win-x64.exe"
    $dotnetInstaller = "$env:TEMP\dotnet-desktop-runtime-8.0.exe"
    
    try {
        Invoke-WebRequest -Uri $dotnetUrl -OutFile $dotnetInstaller
        Write-Host "✅ Instalando .NET 8.0..." -ForegroundColor Green
        Start-Process -FilePath $dotnetInstaller -ArgumentList "/quiet /norestart" -Wait
        Write-Host "✅ .NET 8.0 instalado correctamente" -ForegroundColor Green
    } catch {
        Write-Host "❌ Error al descargar .NET. Descárgalo manualmente de:" -ForegroundColor Red
        Write-Host "   https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
        pause
        exit
    }
} else {
    Write-Host "✅ .NET está instalado (versión $dotnetVersion)" -ForegroundColor Green
}

# 2. Configurar Firewall para SQL Server
Write-Host ""
Write-Host "🔥 Configurando Firewall para SQL Server..." -ForegroundColor Yellow

$firewallRule = Get-NetFirewallRule -DisplayName "SQL Server" -ErrorAction SilentlyContinue

if (-not $firewallRule) {
    New-NetFirewallRule -DisplayName "SQL Server" -Direction Inbound -Protocol TCP -LocalPort 1433 -Action Allow | Out-Null
    Write-Host "✅ Regla de Firewall creada" -ForegroundColor Green
} else {
    Write-Host "✅ Regla de Firewall ya existe" -ForegroundColor Green
}

# 3. Obtener información del servidor
Write-Host ""
Write-Host "📡 Información de Red:" -ForegroundColor Yellow
Write-Host "   Nombre del equipo: $env:COMPUTERNAME" -ForegroundColor Cyan

$ipAddress = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.InterfaceAlias -notlike "*Loopback*" -and $_.IPAddress -notlike "169.254.*" } | Select-Object -First 1).IPAddress
Write-Host "   Dirección IP: $ipAddress" -ForegroundColor Cyan

# 4. Crear carpeta de instalación
Write-Host ""
Write-Host "📁 Creando carpeta de instalación..." -ForegroundColor Yellow
$installPath = "C:\Program Files\SysMarkModerno"

if (-not (Test-Path $installPath)) {
    New-Item -ItemType Directory -Path $installPath -Force | Out-Null
    Write-Host "✅ Carpeta creada: $installPath" -ForegroundColor Green
} else {
    Write-Host "✅ Carpeta ya existe: $installPath" -ForegroundColor Green
}

# 5. Copiar archivos (si están en la misma carpeta)
Write-Host ""
Write-Host "📋 Para completar la instalación:" -ForegroundColor Yellow
Write-Host "   1. Copia los archivos compilados a: $installPath" -ForegroundColor White
Write-Host "   2. Edita App.xaml.cs con la conexión a SQL Server" -ForegroundColor White
Write-Host "   3. Usa esta información:" -ForegroundColor White
Write-Host ""
Write-Host "   Server=$ipAddress;Database=SysMark;User Id=sysmark_user;Password=epicor;TrustServerCertificate=True;" -ForegroundColor Cyan
Write-Host ""

# 6. Crear acceso directo en el escritorio
Write-Host "🔗 ¿Deseas crear un acceso directo en el escritorio? (S/N)" -ForegroundColor Yellow
$createShortcut = Read-Host

if ($createShortcut -eq "S" -or $createShortcut -eq "s") {
    $exePath = Join-Path $installPath "SysMarkModerno.exe"
    
    if (Test-Path $exePath) {
        $WshShell = New-Object -comObject WScript.Shell
        $Shortcut = $WshShell.CreateShortcut("$env:USERPROFILE\Desktop\SysMark.lnk")
        $Shortcut.TargetPath = $exePath
        $Shortcut.WorkingDirectory = $installPath
        $Shortcut.Description = "Sistema de Marketing SysMark"
        $Shortcut.Save()
        Write-Host "✅ Acceso directo creado en el escritorio" -ForegroundColor Green
    } else {
        Write-Host "⚠️  El ejecutable aún no está en $installPath" -ForegroundColor Yellow
    }
}

# 7. Verificar servicios SQL Server
Write-Host ""
Write-Host "🔍 Verificando servicios de SQL Server..." -ForegroundColor Yellow
$sqlService = Get-Service -Name "MSSQLSERVER" -ErrorAction SilentlyContinue

if ($sqlService) {
    if ($sqlService.Status -eq "Running") {
        Write-Host "✅ SQL Server está corriendo" -ForegroundColor Green
    } else {
        Write-Host "⚠️  SQL Server está detenido. Iniciando..." -ForegroundColor Yellow
        Start-Service -Name "MSSQLSERVER"
        Write-Host "✅ SQL Server iniciado" -ForegroundColor Green
    }
} else {
    Write-Host "⚠️  SQL Server no está instalado en esta PC" -ForegroundColor Yellow
    Write-Host "   Esta PC puede ser cliente conectándose a otro servidor" -ForegroundColor Cyan
}

# 8. Resumen final
Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host " Instalación Completada" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📝 Próximos pasos:" -ForegroundColor Yellow
Write-Host "   1. Compila el proyecto en Visual Studio" -ForegroundColor White
Write-Host "   2. Copia el EXE a: $installPath" -ForegroundColor White
Write-Host "   3. Configura la cadena de conexión en App.xaml.cs" -ForegroundColor White
Write-Host "   4. Ejecuta SysMarkModerno.exe" -ForegroundColor White
Write-Host ""
Write-Host "🌐 Para otras PCs, usa esta cadena de conexión:" -ForegroundColor Yellow
Write-Host "   Server=$ipAddress;Database=SysMark;User Id=sysmark_user;Password=epicor;TrustServerCertificate=True;" -ForegroundColor Cyan
Write-Host ""

pause

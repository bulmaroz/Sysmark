@echo off
echo ========================================
echo  SysMark Moderno - Script de Compilacion
echo ========================================
echo.

echo [1/3] Restaurando paquetes NuGet...
dotnet restore
if %errorlevel% neq 0 (
    echo Error al restaurar paquetes
    pause
    exit /b 1
)

echo.
echo [2/3] Compilando proyecto...
dotnet build -c Release
if %errorlevel% neq 0 (
    echo Error al compilar
    pause
    exit /b 1
)

echo.
echo [3/3] Publicando aplicacion...
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=false
if %errorlevel% neq 0 (
    echo Error al publicar
    pause
    exit /b 1
)

echo.
echo ========================================
echo  Compilacion Exitosa!
echo ========================================
echo.
echo El ejecutable esta en:
echo bin\Release\net8.0-windows\win-x64\publish\
echo.
pause

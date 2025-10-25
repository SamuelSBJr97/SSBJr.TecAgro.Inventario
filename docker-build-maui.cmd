@echo off
REM Script para build do App MAUI usando Docker no Windows

echo ======================================
echo TecAgro Inventario - Build App MAUI
echo ======================================
echo.

REM Verificar se Docker esta instalado
where docker >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo Erro: Docker nao encontrado. Por favor, instale o Docker Desktop.
    pause
    exit /b 1
)

echo Construindo APK do aplicativo Android...
echo.

REM Build da imagem Docker
docker build -f Dockerfile.maui -t tecagro-maui-builder .

REM Criar diretorio de output se nao existir
if not exist ".\output\android" mkdir ".\output\android"

REM Extrair o APK do container
echo Extraindo APK...
docker create --name temp-maui-container tecagro-maui-builder
docker cp temp-maui-container:/app/publish/. .\output\android\
docker rm temp-maui-container

echo.
echo ======================================
echo Build concluido!
echo APK disponivel em: .\output\android\
echo ======================================
echo.
pause

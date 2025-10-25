@echo off
echo ========================================
echo Executando Testes - TecAgro Inventario
echo ========================================
echo.

echo [1/4] Executando testes do Domain...
dotnet test tests/SSBJr.TecAgro.Inventario.Domain.Tests --verbosity minimal
if %errorlevel% neq 0 (
    echo ERRO: Testes do Domain falharam!
    exit /b %errorlevel%
)
echo.

echo [2/4] Executando testes da Infrastructure...
dotnet test tests/SSBJr.TecAgro.Inventario.Infrastructure.Tests --verbosity minimal
if %errorlevel% neq 0 (
    echo ERRO: Testes da Infrastructure falharam!
    exit /b %errorlevel%
)
echo.

echo [3/4] Executando testes do Server...
dotnet test tests/SSBJr.TecAgro.Inventario.Server.Tests --verbosity minimal
if %errorlevel% neq 0 (
  echo ERRO: Testes do Server falharam!
    exit /b %errorlevel%
)
echo.

echo [4/4] Executando todos os testes...
dotnet test --verbosity minimal
if %errorlevel% neq 0 (
    echo ERRO: Alguns testes falharam!
    exit /b %errorlevel%
)
echo.

echo ========================================
echo SUCESSO: Todos os testes passaram!
echo ========================================

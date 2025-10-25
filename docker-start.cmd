@echo off
REM Script para iniciar o ambiente Docker no Windows

echo ======================================
echo TecAgro Inventario - Iniciar Docker
echo ======================================
echo.

REM Verificar se Docker esta instalado
where docker >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo Erro: Docker nao encontrado. Por favor, instale o Docker Desktop.
    pause
    exit /b 1
)

REM Perguntar qual ambiente
echo Selecione o ambiente:
echo 1^) Desenvolvimento ^(apenas banco de dados^)
echo 2^) Producao ^(banco de dados + API + Nginx^)
set /p option="Opcao (1 ou 2): "

if "%option%"=="1" (
    echo.
    echo Iniciando ambiente de DESENVOLVIMENTO...
    docker-compose -f docker-compose.dev.yml up -d
    echo.
    echo [OK] Banco de dados SQL Server iniciado!
echo   - Host: localhost
    echo   - Porta: 1433
    echo   - Usuario: sa
    echo   - Senha: Dev@2025!Pass
    echo   - Database: InventarioDb
    echo.
    echo Connection String:
    echo Server=localhost,1433;Database=InventarioDb;User Id=sa;Password=Dev@2025!Pass;TrustServerCertificate=True;
    echo.
    echo Para parar os containers:
 echo   docker-compose -f docker-compose.dev.yml down
) else if "%option%"=="2" (
    echo.
    echo Iniciando ambiente de PRODUCAO...
    docker-compose up -d --build
    echo.
    echo [OK] Todos os servicos iniciados!
    echo.
    echo Servicos disponiveis:
    echo   - API: http://localhost:5000
    echo   - Swagger: http://localhost:5000/swagger
    echo   - Nginx: http://localhost:80
    echo   - SQL Server: localhost:1433
    echo.
    echo Para ver os logs:
    echo   docker-compose logs -f
    echo.
    echo Para parar os containers:
    echo   docker-compose down
) else (
  echo Opcao invalida!
    pause
    exit /b 1
)

echo.
pause

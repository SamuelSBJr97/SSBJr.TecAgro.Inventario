# Script de Deploy PowerShell - SSBJr.TecAgro.Inventario
# Este script facilita o deploy da aplica��o usando Docker no Windows

param(
    [Parameter(Mandatory=$true)]
  [ValidateSet('deploy','build','start','stop','restart','logs','cleanup')]
    [string]$Action
)

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "SSBJr TecAgro Invent�rio - Deploy Script" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se Docker est� instalado
$dockerInstalled = Get-Command docker -ErrorAction SilentlyContinue
if (-not $dockerInstalled) {
    Write-Host "? Docker n�o est� instalado. Instale o Docker Desktop primeiro." -ForegroundColor Red
    exit 1
}

# Verificar se Docker Compose est� dispon�vel
$composeInstalled = docker compose version 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Docker Compose n�o est� dispon�vel. Certifique-se de ter o Docker Desktop atualizado." -ForegroundColor Red
    exit 1
}

function Cleanup {
    Write-Host "?? Limpando containers antigos..." -ForegroundColor Yellow
    docker compose down -v
}

function Build {
    Write-Host "?? Construindo imagens Docker..." -ForegroundColor Yellow
    docker compose build --no-cache
}

function Start {
    Write-Host "?? Iniciando containers..." -ForegroundColor Yellow
    docker compose up -d
    
    Write-Host ""
    Write-Host "? Aguardando servi�os ficarem prontos..." -ForegroundColor Yellow
    Start-Sleep -Seconds 10
    
    Write-Host ""
    Write-Host "? Deploy conclu�do com sucesso!" -ForegroundColor Green
    Write-Host ""
  Write-Host "?? Status dos containers:" -ForegroundColor Cyan
    docker compose ps
    Write-Host ""
    Write-Host "?? Acesse a API em: http://localhost:5000" -ForegroundColor Green
    Write-Host "?? Swagger UI: http://localhost:5000/swagger" -ForegroundColor Green
    Write-Host ""
    Write-Host "?? Usu�rio padr�o:" -ForegroundColor Cyan
    Write-Host "   Login: admin" -ForegroundColor White
    Write-Host "   Senha: admin123" -ForegroundColor White
    Write-Host ""
    Write-Host "?? Para ver logs: .\deploy.ps1 -Action logs" -ForegroundColor Yellow
    Write-Host "?? Para parar: .\deploy.ps1 -Action stop" -ForegroundColor Yellow
}

function Logs {
    Write-Host "?? Exibindo logs..." -ForegroundColor Yellow
    docker compose logs -f
}

function Restart {
 Write-Host "?? Reiniciando aplica��o..." -ForegroundColor Yellow
    docker compose restart
}

function Stop {
    Write-Host "?? Parando aplica��o..." -ForegroundColor Yellow
    docker compose down
}

# Executar a��o
switch ($Action) {
    'cleanup' { Cleanup }
    'build' { Build }
    'start' { Start }
    'deploy' {
        Cleanup
      Build
 Start
    }
  'logs' { Logs }
    'restart' { Restart }
    'stop' { Stop }
}

# Script PowerShell para iniciar Docker Compose antes do debug
# Este script e executado automaticamente pelo Visual Studio

param(
    [string]$ComposeFile = "docker-compose.debug.yml",
    [switch]$Stop
)

Write-Host "======================================"
Write-Host "TecAgro Inventario - Docker Debug"
Write-Host "======================================"
Write-Host ""

$ErrorActionPreference = "Stop"

# Verificar se Docker esta rodando
try {
    docker ps | Out-Null
} catch {
    Write-Error "Docker nao esta rodando. Por favor, inicie o Docker Desktop."
    exit 1
}

if ($Stop) {
    Write-Host "Parando containers..."
    docker-compose -f $ComposeFile down
    Write-Host "Containers parados com sucesso!"
    exit 0
}

# Verificar se os containers ja estao rodando
$runningContainers = docker-compose -f $ComposeFile ps -q

if ($runningContainers) {
 Write-Host "Containers ja estao rodando."
    Write-Host "Use 'docker-compose -f $ComposeFile ps' para ver o status."
} else {
    Write-Host "Iniciando containers do Docker Compose..."
    Write-Host ""
    
    # Iniciar containers
    docker-compose -f $ComposeFile up -d
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "? Containers iniciados com sucesso!"
        Write-Host ""
        Write-Host "Servicos disponiveis:"
        Write-Host "  - API: http://localhost:5000"
        Write-Host "  - Swagger: http://localhost:5000/swagger"
        Write-Host "  - SQL Server: localhost:1433"
        Write-Host ""
        
    # Aguardar API estar pronta
        Write-Host "Aguardando API estar pronta..."
   $maxAttempts = 30
    $attempt = 0
        
        do {
            $attempt++
   Start-Sleep -Seconds 2
       
      try {
                $response = Invoke-WebRequest -Uri "http://localhost:5000/swagger/index.html" -UseBasicParsing -TimeoutSec 2
  if ($response.StatusCode -eq 200) {
          Write-Host "? API esta pronta!"
             break
           }
    } catch {
          Write-Host "Tentativa $attempt/$maxAttempts..."
         }
        } while ($attempt -lt $maxAttempts)
        
        if ($attempt -eq $maxAttempts) {
        Write-Warning "API nao respondeu no tempo esperado. Continue mesmo assim."
        }
        
        Write-Host ""
        Write-Host "Pronto para iniciar o debug do app MAUI!"
        Write-Host ""
    } else {
        Write-Error "Erro ao iniciar containers. Verifique os logs com 'docker-compose -f $ComposeFile logs'"
        exit 1
    }
}

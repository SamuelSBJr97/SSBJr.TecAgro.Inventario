# Guia Docker - TecAgro Invent�rio

## ?? Vis�o Geral

Este guia explica como usar Docker para rodar e desenvolver o sistema TecAgro Invent�rio.

## ?? Pr�-requisitos

### Windows
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e rodando
- WSL 2 ativado (para melhor performance)

### Linux/Mac
- Docker Engine instalado
- Docker Compose instalado

## ?? In�cio R�pido

### 1. Ambiente de Desenvolvimento (Apenas Banco de Dados)

**Windows:**
```cmd
docker-start.cmd
# Escolha op��o 1
```

**Linux/Mac:**
```bash
chmod +x docker-start.sh
./docker-start.sh
# Escolha op��o 1
```

**Ou manualmente:**
```bash
docker-compose -f docker-compose.dev.yml up -d
```

### 2. Ambiente Completo (Banco + API + Nginx)

**Windows:**
```cmd
docker-start.cmd
# Escolha op��o 2
```

**Linux/Mac:**
```bash
./docker-start.sh
# Escolha op��o 2
```

**Ou manualmente:**
```bash
docker-compose up -d --build
```

## ?? Servi�os Dispon�veis

### Desenvolvimento (docker-compose.dev.yml)
| Servi�o | Porta | Descri��o |
|---------|-------|-----------|
| SQL Server | 1433 | Banco de dados |

**Connection String:**
```
Server=localhost,1433;Database=InventarioDb;User Id=sa;Password=Dev@2025!Pass;TrustServerCertificate=True;
```

### Produ��o (docker-compose.yml)
| Servi�o | Porta | URL | Descri��o |
|---------|-------|-----|-----------|
| SQL Server | 1433 | - | Banco de dados |
| API | 5000 | http://localhost:5000 | Backend API |
| Swagger | 5000 | http://localhost:5000/swagger | Documenta��o API |
| Nginx | 80 | http://localhost | Reverse Proxy |

## ??? Comandos �teis

### Gerenciamento de Containers

```bash
# Ver containers rodando
docker-compose ps

# Ver logs de todos os servi�os
docker-compose logs -f

# Ver logs de um servi�o espec�fico
docker-compose logs -f api
docker-compose logs -f db

# Parar todos os containers
docker-compose down

# Parar e remover volumes (CUIDADO: apaga dados!)
docker-compose down -v

# Reiniciar um servi�o espec�fico
docker-compose restart api

# Rebuild de um servi�o
docker-compose up -d --build api
```

### Acesso ao Banco de Dados

**Via SQL Server Management Studio (SSMS):**
- Server: `localhost,1433`
- Authentication: SQL Server Authentication
- Login: `sa`
- Password: `Dev@2025!Pass` (dev) ou `TecAgro@2025!Strong` (prod)

**Via linha de comando:**
```bash
# Entrar no container do SQL Server
docker exec -it tecagro-inventario-db /bin/bash

# Conectar ao SQL Server
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Dev@2025!Pass" -C

# Listar databases
SELECT name FROM sys.databases;
GO
```

### Limpeza e Manuten��o

```bash
# Remover containers parados
docker container prune

# Remover imagens n�o utilizadas
docker image prune

# Remover volumes n�o utilizados
docker volume prune

# Limpeza completa (CUIDADO!)
docker system prune -a --volumes
```

## ?? Configura��o do App MAUI

### Apontando para API Dockerizada

No app MAUI, configure a URL da API:

**Android Emulator:**
```csharp
// Para acessar localhost do host
var serverUrl = "http://10.0.2.2:5000";
```

**Android Device (mesmo WiFi):**
```csharp
// Use o IP da m�quina host
var serverUrl = "http://192.168.1.XXX:5000";
```

**iOS Simulator:**
```csharp
// Use localhost
var serverUrl = "http://localhost:5000";
```

### Configurando no PreferencesService

```csharp
// No primeiro uso do app
_preferencesService.SaveServerUrl("http://10.0.2.2:5000");
```

## ?? Build do App MAUI com Docker

### Android APK

**Windows:**
```cmd
docker-build-maui.cmd
```

**Linux/Mac:**
```bash
chmod +x docker-build-maui.sh
./docker-build-maui.sh
```

O APK ser� gerado em: `./output/android/`

## ?? Troubleshooting

### Problema: Container n�o inicia

```bash
# Verificar logs
docker-compose logs

# Verificar status
docker-compose ps

# Rebuild for�ado
docker-compose up -d --build --force-recreate
```

### Problema: Banco de dados n�o conecta

```bash
# Verificar se o container est� rodando
docker ps | grep sql

# Ver logs do SQL Server
docker-compose logs db

# Testar conex�o
docker exec tecagro-inventario-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Dev@2025!Pass" -Q "SELECT 1" -C
```

### Problema: API n�o conecta ao banco

```bash
# Verificar vari�veis de ambiente
docker exec tecagro-inventario-api env | grep ConnectionStrings

# Verificar conectividade entre containers
docker exec tecagro-inventario-api ping db
```

### Problema: Porta j� em uso

```bash
# Encontrar processo usando a porta
# Windows:
netstat -ano | findstr :5000

# Linux/Mac:
lsof -i :5000

# Parar container usando a porta
docker-compose stop api

# Ou mudar a porta no docker-compose.yml
```

## ?? Seguran�a

### Senhas Padr�o

?? **IMPORTANTE**: As senhas padr�o s�o apenas para desenvolvimento!

**Desenvolvimento:**
- SA Password: `Dev@2025!Pass`

**Produ��o:**
- SA Password: `TecAgro@2025!Strong`
- JWT Secret: Configurado em vari�vel de ambiente

### Alterando Senhas

**SQL Server:**
```yaml
# No docker-compose.yml
environment:
  - SA_PASSWORD=SuaSenhaSegura@2025!
```

**JWT Secret:**
```yaml
environment:
  - Jwt__Secret=SeuSecretoMuitoSeguroELongo
```

## ?? Monitoramento

### Health Checks

```bash
# API Health
curl http://localhost:5000/swagger/index.html

# Database Health
docker inspect tecagro-inventario-db --format='{{.State.Health.Status}}'
```

### Recursos do Sistema

```bash
# Ver uso de recursos
docker stats

# Ver uso de espa�o
docker system df
```

## ?? Backup e Restore

### Backup do Banco de Dados

```bash
# Criar backup
docker exec tecagro-inventario-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Dev@2025!Pass" -C -Q "BACKUP DATABASE InventarioDb TO DISK = '/var/opt/mssql/backup/InventarioDb.bak'"

# Copiar backup para host
docker cp tecagro-inventario-db:/var/opt/mssql/backup/InventarioDb.bak ./backup/
```

### Restore do Banco de Dados

```bash
# Copiar backup para container
docker cp ./backup/InventarioDb.bak tecagro-inventario-db:/var/opt/mssql/backup/

# Restaurar
docker exec tecagro-inventario-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Dev@2025!Pass" -C -Q "RESTORE DATABASE InventarioDb FROM DISK = '/var/opt/mssql/backup/InventarioDb.bak' WITH REPLACE"
```

## ?? Melhores Pr�ticas

1. **Desenvolvimento Local**: Use `docker-compose.dev.yml` para rodar apenas o banco
2. **Testes**: Use `docker-compose.yml` para testar o ambiente completo
3. **Produ��o**: Use orquestrador como Kubernetes para deploy real
4. **Volumes**: Fa�a backup regular dos volumes Docker
5. **Logs**: Monitore logs regularmente para identificar problemas
6. **Seguran�a**: Nunca use senhas padr�o em produ��o
7. **Network**: Use redes Docker para isolamento de servi�os

## ?? Recursos Adicionais

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [SQL Server on Docker](https://learn.microsoft.com/en-us/sql/linux/sql-server-linux-docker-container-deployment)
- [ASP.NET Core on Docker](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/)

## ?? Suporte

Para problemas ou d�vidas:
1. Verifique os logs: `docker-compose logs`
2. Consulte a documenta��o acima
3. Abra uma issue no reposit�rio do projeto

---

**�ltima atualiza��o**: 2025-01-24

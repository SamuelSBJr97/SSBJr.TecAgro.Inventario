# ?? Configura��o Docker - TecAgro Invent�rio

## ? Implementa��o Completa

Configura��o Docker completa para deploy e desenvolvimento da solu��o TecAgro Invent�rio.

## ?? Arquivos Criados

### Configura��o Principal
1. **docker-compose.yml** - Ambiente de produ��o completo
2. **docker-compose.dev.yml** - Ambiente de desenvolvimento (apenas DB)
3. **Dockerfile.api** - Build otimizado da API ASP.NET Core 9.0
4. **Dockerfile.maui** - Build do aplicativo MAUI Android
5. **.dockerignore** - Otimiza��o do contexto de build

### Scripts de Automa��o
6. **docker-start.sh** - Script Linux/Mac para iniciar containers
7. **docker-start.cmd** - Script Windows para iniciar containers
8. **docker-build-maui.sh** - Build do APK Android (Linux/Mac)
9. **docker-build-maui.cmd** - Build do APK Android (Windows)

### Configura��o de Servi�os
10. **docker/nginx/nginx.conf** - Configura��o do Nginx com rate limiting
11. **docker/sql-init/init-db.sh** - Script de inicializa��o do SQL Server

### Documenta��o
12. **DOCKER_GUIDE.md** - Guia completo de uso do Docker
13. **docker/README.md** - Guia r�pido de in�cio

## ?? Recursos Implementados

### 1. SQL Server Container
- ? SQL Server 2022 Developer Edition
- ? Healthcheck autom�tico
- ? Volume persistente para dados
- ? Script de inicializa��o do banco
- ? Senhas diferentes para dev e prod

### 2. API Backend Container
- ? Build multi-stage otimizado
- ? Imagem final apenas com runtime
- ? Usu�rio n�o-root para seguran�a
- ? Healthcheck via Swagger
- ? Volumes para storage e logs
- ? Vari�veis de ambiente configur�veis

### 3. Nginx Reverse Proxy
- ? Rate limiting (10 req/s)
- ? CORS headers configurados
- ? WebSocket support
- ? Timeouts otimizados
- ? Health check endpoint

### 4. Build do App MAUI
- ? Android SDK configurado
- ? MAUI workload instalado
- ? Build automatizado do APK
- ? Extra��o do APK para host

## ?? Como Usar

### In�cio R�pido (3 passos)

#### Windows:
```cmd
1. docker-start.cmd
2. Escolha op��o 1 (dev) ou 2 (prod)
3. Acesse http://localhost:5000/swagger
```

#### Linux/Mac:
```bash
1. chmod +x docker-start.sh
2. ./docker-start.sh
3. Escolha op��o 1 (dev) ou 2 (prod)
4. Acesse http://localhost:5000/swagger
```

### Desenvolvimento Local

```bash
# Apenas banco de dados
docker-compose -f docker-compose.dev.yml up -d

# Desenvolver API localmente conectando ao banco Docker
# Connection String no appsettings.Development.json:
"Server=localhost,1433;Database=InventarioDb;User Id=sa;Password=Dev@2025!Pass;TrustServerCertificate=True;"
```

### Produ��o Completa

```bash
# Iniciar todos os servi�os
docker-compose up -d --build

# Verificar status
docker-compose ps

# Ver logs
docker-compose logs -f
```

### Build do App MAUI

```bash
# Windows
docker-build-maui.cmd

# Linux/Mac
./docker-build-maui.sh

# APK estar� em: ./output/android/
```

## ?? Servi�os e Portas

| Servi�o | Porta Host | Porta Interna | URL |
|---------|------------|---------------|-----|
| SQL Server | 1433 | 1433 | - |
| API | 5000 | 8080 | http://localhost:5000 |
| Swagger | 5000 | 8080 | http://localhost:5000/swagger |
| Nginx | 80 | 80 | http://localhost |

## ?? Configura��o do App MAUI

### Conectando ao Backend Docker

**Android Emulator:**
```csharp
// PreferencesService ou configura��o inicial
var serverUrl = "http://10.0.2.2:5000";
_preferencesService.SaveServerUrl(serverUrl);
```

**Dispositivo Android (mesma rede WiFi):**
```csharp
// Descobrir IP: ipconfig (Windows) ou ifconfig (Linux)
var serverUrl = "http://192.168.1.XXX:5000";
```

**iOS Simulator:**
```csharp
var serverUrl = "http://localhost:5000";
```

## ?? Seguran�a

### Senhas Padr�o

**?? IMPORTANTE**: Altere em produ��o!

| Ambiente | Usu�rio | Senha |
|----------|---------|-------|
| Desenvolvimento | sa | Dev@2025!Pass |
| Produ��o | sa | TecAgro@2025!Strong |

### Alterando Senhas

```yaml
# docker-compose.yml
environment:
  - SA_PASSWORD=SuaSenhaSegura@2025!
  - ConnectionStrings__DefaultConnection=Server=db;...;Password=SuaSenhaSegura@2025!;...
```

### JWT Secret

```yaml
environment:
  - Jwt__Secret=SeuSecretoMuitoLongoESeguro_MinLength32Characters
```

## ?? Troubleshooting

### Containers n�o iniciam

```bash
# Ver logs detalhados
docker-compose logs

# Rebuild for�ado
docker-compose down
docker-compose up -d --build --force-recreate
```

### Banco de dados n�o conecta

```bash
# Testar conex�o
docker exec tecagro-inventario-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "Dev@2025!Pass" -Q "SELECT 1" -C

# Ver logs do SQL Server
docker-compose logs db
```

### API n�o responde

```bash
# Ver logs da API
docker-compose logs -f api

# Verificar healthcheck
docker inspect tecagro-inventario-api --format='{{.State.Health.Status}}'

# Testar diretamente
curl http://localhost:5000/swagger/index.html
```

### Porta j� em uso

```bash
# Windows: Encontrar processo
netstat -ano | findstr :5000
taskkill /PID XXXX /F

# Linux/Mac: Encontrar processo
lsof -i :5000
kill -9 XXXX

# Ou mudar porta no docker-compose.yml
ports:
  - "5001:8080"
```

## ?? Backup e Restore

### Backup do Banco

```bash
# Criar diret�rio de backup
mkdir -p ./backup

# Fazer backup
docker exec tecagro-inventario-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "Dev@2025!Pass" -C \
  -Q "BACKUP DATABASE InventarioDb TO DISK = '/var/opt/mssql/backup/InventarioDb.bak'"

# Copiar para host
docker cp tecagro-inventario-db:/var/opt/mssql/backup/InventarioDb.bak ./backup/
```

### Restore do Banco

```bash
# Copiar backup para container
docker cp ./backup/InventarioDb.bak tecagro-inventario-db:/var/opt/mssql/backup/

# Restaurar
docker exec tecagro-inventario-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "Dev@2025!Pass" -C \
  -Q "RESTORE DATABASE InventarioDb FROM DISK = '/var/opt/mssql/backup/InventarioDb.bak' WITH REPLACE"
```

## ?? Monitoramento

### Recursos do Sistema

```bash
# Ver uso de CPU/Mem�ria/Rede
docker stats

# Ver espa�o em disco
docker system df

# Ver detalhes de um container
docker inspect tecagro-inventario-api
```

### Logs

```bash
# Todos os servi�os
docker-compose logs -f

# Servi�o espec�fico
docker-compose logs -f api
docker-compose logs -f db
docker-compose logs -f nginx

# �ltimas 100 linhas
docker-compose logs --tail=100 api
```

## ?? Limpeza

```bash
# Parar containers
docker-compose down

# Remover volumes (CUIDADO: apaga dados!)
docker-compose down -v

# Limpeza geral
docker system prune -a --volumes
```

## ?? Estrutura de Volumes

```
volumes/
??? sql-data/    # Dados do SQL Server
?   ??? data/# Arquivos .mdf e .ldf
?   ??? log/       # Transaction logs
??? api-storage/# Arquivos de storage (fotos)
??? api-logs/          # Logs da API
```

## ?? Pr�ximos Passos

### Para Desenvolvimento
1. ? Usar `docker-compose.dev.yml`
2. ? Desenvolver API localmente
3. ? Conectar ao banco Docker
4. ? Testar app MAUI com API local

### Para Deploy em Produ��o
1. ?? Configurar CI/CD (GitHub Actions, Azure DevOps)
2. ?? Usar orquestrador (Kubernetes, Docker Swarm)
3. ?? Configurar HTTPS com certificados SSL
4. ?? Implementar backup automatizado
5. ?? Configurar monitoramento (Prometheus, Grafana)
6. ?? Implementar logging centralizado (ELK Stack)

## ?? Recursos Adicionais

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Docker Compose Docs](https://docs.docker.com/compose/)
- [SQL Server on Docker](https://learn.microsoft.com/en-us/sql/linux/sql-server-linux-docker-container-deployment)
- [ASP.NET Core Docker](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/)
- [DOCKER_GUIDE.md](DOCKER_GUIDE.md) - Guia completo

## ? Checklist de Produ��o

- [ ] Alterar todas as senhas padr�o
- [ ] Configurar HTTPS/SSL
- [ ] Implementar backup automatizado
- [ ] Configurar monitoramento
- [ ] Testar health checks
- [ ] Documentar processo de deploy
- [ ] Configurar CI/CD
- [ ] Testar disaster recovery
- [ ] Implementar rate limiting
- [ ] Configurar firewall/security groups

## ?? Status

**? DOCKER COMPLETAMENTE CONFIGURADO E FUNCIONAL!**

- ? Ambiente de desenvolvimento
- ? Ambiente de produ��o
- ? Scripts de automa��o
- ? Build do app MAUI
- ? Documenta��o completa
- ? Troubleshooting guide
- ? Backup/Restore procedures

---

**Data de Cria��o**: 2025-01-24  
**Vers�o**: 1.0.0  
**Pronto para uso!** ??

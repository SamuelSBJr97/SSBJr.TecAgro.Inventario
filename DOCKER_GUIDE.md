# Guia Docker - TecAgro Inventário

## ?? Visão Geral

Este guia explica como usar Docker para rodar e desenvolver o sistema TecAgro Inventário.

## ?? Pré-requisitos

### Windows
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e rodando
- WSL 2 ativado (para melhor performance)

### Linux/Mac
- Docker Engine instalado
- Docker Compose instalado

## ?? Início Rápido

### 1. Ambiente de Desenvolvimento (Apenas Banco de Dados)

**Windows:**
```cmd
docker-start.cmd
# Escolha opção 1
```

**Linux/Mac:**
```bash
chmod +x docker-start.sh
./docker-start.sh
# Escolha opção 1
```

**Ou manualmente:**
```bash
docker-compose -f docker-compose.dev.yml up -d
```

### 2. Ambiente Completo (Banco + API + Nginx)

**Windows:**
```cmd
docker-start.cmd
# Escolha opção 2
```

**Linux/Mac:**
```bash
./docker-start.sh
# Escolha opção 2
```

**Ou manualmente:**
```bash
docker-compose up -d --build
```

## ?? Serviços Disponíveis

### Desenvolvimento (docker-compose.dev.yml)
| Serviço | Porta | Descrição |
|---------|-------|-----------|
| SQL Server | 1433 | Banco de dados |

**Connection String:**
```
Server=localhost,1433;Database=InventarioDb;User Id=sa;Password=Dev@2025!Pass;TrustServerCertificate=True;
```

### Produção (docker-compose.yml)
| Serviço | Porta | URL | Descrição |
|---------|-------|-----|-----------|
| SQL Server | 1433 | - | Banco de dados |
| API | 5000 | http://localhost:5000 | Backend API |
| Swagger | 5000 | http://localhost:5000/swagger | Documentação API |
| Nginx | 80 | http://localhost | Reverse Proxy |

## ??? Comandos Úteis

### Gerenciamento de Containers

```bash
# Ver containers rodando
docker-compose ps

# Ver logs de todos os serviços
docker-compose logs -f

# Ver logs de um serviço específico
docker-compose logs -f api
docker-compose logs -f db

# Parar todos os containers
docker-compose down

# Parar e remover volumes (CUIDADO: apaga dados!)
docker-compose down -v

# Reiniciar um serviço específico
docker-compose restart api

# Rebuild de um serviço
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

### Limpeza e Manutenção

```bash
# Remover containers parados
docker container prune

# Remover imagens não utilizadas
docker image prune

# Remover volumes não utilizados
docker volume prune

# Limpeza completa (CUIDADO!)
docker system prune -a --volumes
```

## ?? Configuração do App MAUI

### Apontando para API Dockerizada

No app MAUI, configure a URL da API:

**Android Emulator:**
```csharp
// Para acessar localhost do host
var serverUrl = "http://10.0.2.2:5000";
```

**Android Device (mesmo WiFi):**
```csharp
// Use o IP da máquina host
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

O APK será gerado em: `./output/android/`

## ?? Troubleshooting

### Problema: Container não inicia

```bash
# Verificar logs
docker-compose logs

# Verificar status
docker-compose ps

# Rebuild forçado
docker-compose up -d --build --force-recreate
```

### Problema: Banco de dados não conecta

```bash
# Verificar se o container está rodando
docker ps | grep sql

# Ver logs do SQL Server
docker-compose logs db

# Testar conexão
docker exec tecagro-inventario-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Dev@2025!Pass" -Q "SELECT 1" -C
```

### Problema: API não conecta ao banco

```bash
# Verificar variáveis de ambiente
docker exec tecagro-inventario-api env | grep ConnectionStrings

# Verificar conectividade entre containers
docker exec tecagro-inventario-api ping db
```

### Problema: Porta já em uso

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

## ?? Segurança

### Senhas Padrão

?? **IMPORTANTE**: As senhas padrão são apenas para desenvolvimento!

**Desenvolvimento:**
- SA Password: `Dev@2025!Pass`

**Produção:**
- SA Password: `TecAgro@2025!Strong`
- JWT Secret: Configurado em variável de ambiente

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

# Ver uso de espaço
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

## ?? Melhores Práticas

1. **Desenvolvimento Local**: Use `docker-compose.dev.yml` para rodar apenas o banco
2. **Testes**: Use `docker-compose.yml` para testar o ambiente completo
3. **Produção**: Use orquestrador como Kubernetes para deploy real
4. **Volumes**: Faça backup regular dos volumes Docker
5. **Logs**: Monitore logs regularmente para identificar problemas
6. **Segurança**: Nunca use senhas padrão em produção
7. **Network**: Use redes Docker para isolamento de serviços

## ?? Recursos Adicionais

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [SQL Server on Docker](https://learn.microsoft.com/en-us/sql/linux/sql-server-linux-docker-container-deployment)
- [ASP.NET Core on Docker](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/)

## ?? Suporte

Para problemas ou dúvidas:
1. Verifique os logs: `docker-compose logs`
2. Consulte a documentação acima
3. Abra uma issue no repositório do projeto

---

**Última atualização**: 2025-01-24

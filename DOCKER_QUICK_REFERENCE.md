# ?? Docker - Comandos Rápidos

## Início Rápido

### Desenvolvimento (apenas banco)
```bash
docker-compose -f docker-compose.dev.yml up -d
```

### Produção (todos os serviços)
```bash
docker-compose up -d --build
```

## Comandos Essenciais

### Gerenciamento
```bash
# Ver status
docker-compose ps

# Ver logs
docker-compose logs -f

# Ver logs de um serviço
docker-compose logs -f api

# Parar
docker-compose down

# Parar e remover volumes
docker-compose down -v

# Reiniciar serviço
docker-compose restart api

# Rebuild
docker-compose up -d --build api
```

### Banco de Dados
```bash
# Conectar ao SQL Server
docker exec -it tecagro-inventario-db /bin/bash
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Dev@2025!Pass" -C

# Testar conexão
docker exec tecagro-inventario-db /opt/mssql-tools18/bin/sqlcmd \
-S localhost -U sa -P "Dev@2025!Pass" -Q "SELECT 1" -C

# Backup
docker exec tecagro-inventario-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "Dev@2025!Pass" -C \
  -Q "BACKUP DATABASE InventarioDb TO DISK = '/var/opt/mssql/backup/backup.bak'"

# Copiar backup
docker cp tecagro-inventario-db:/var/opt/mssql/backup/backup.bak ./backup/
```

### API
```bash
# Ver logs da API
docker logs -f tecagro-inventario-api

# Entrar no container
docker exec -it tecagro-inventario-api /bin/bash

# Ver variáveis de ambiente
docker exec tecagro-inventario-api env | grep ConnectionStrings
```

### Monitoramento
```bash
# Recursos
docker stats

# Espaço em disco
docker system df

# Health check
docker inspect tecagro-inventario-api --format='{{.State.Health.Status}}'
```

### Limpeza
```bash
# Remover containers parados
docker container prune

# Remover imagens não usadas
docker image prune

# Remover volumes não usados
docker volume prune

# Limpeza completa (CUIDADO!)
docker system prune -a --volumes
```

## URLs Rápidas

```
API: http://localhost:5000
Swagger: http://localhost:5000/swagger
Nginx: http://localhost:80
SQL Server: localhost:1433
```

## Connection Strings

### Desenvolvimento
```
Server=localhost,1433;Database=InventarioDb;User Id=sa;Password=Dev@2025!Pass;TrustServerCertificate=True;
```

### Produção
```
Server=localhost,1433;Database=InventarioDb;User Id=sa;Password=TecAgro@2025!Strong;TrustServerCertificate=True;
```

## App MAUI URLs

```csharp
// Android Emulator
"http://10.0.2.2:5000"

// Android Device (WiFi)
"http://192.168.1.XXX:5000"

// iOS Simulator
"http://localhost:5000"
```

## Build MAUI

```bash
# Windows
docker-build-maui.cmd

# Linux/Mac
./docker-build-maui.sh
```

## Troubleshooting

### Porta em uso
```bash
# Windows
netstat -ano | findstr :5000

# Linux/Mac
lsof -i :5000
```

### Ver erros
```bash
docker-compose logs
docker inspect <container-name>
```

### Rebuild forçado
```bash
docker-compose down
docker-compose up -d --build --force-recreate
```

---

Para mais detalhes, consulte [DOCKER_GUIDE.md](DOCKER_GUIDE.md)

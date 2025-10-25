# ?? Docker - TecAgro Inventário

## Início Rápido

### Opção 1: Apenas Banco de Dados (Desenvolvimento)

```bash
# Windows
docker-start.cmd

# Linux/Mac
chmod +x docker-start.sh
./docker-start.sh
```

Escolha a opção **1** para iniciar apenas o SQL Server.

### Opção 2: Ambiente Completo (Produção)

Escolha a opção **2** no script acima para iniciar:
- SQL Server
- API Backend
- Nginx Reverse Proxy

## ?? O que está incluído?

### Arquivos Docker

| Arquivo | Descrição |
|---------|-----------|
| `docker-compose.yml` | Configuração principal (produção) |
| `docker-compose.dev.yml` | Configuração para desenvolvimento |
| `Dockerfile.api` | Build da API ASP.NET Core |
| `Dockerfile.maui` | Build do App MAUI Android |
| `.dockerignore` | Arquivos ignorados no build |
| `docker-start.sh/.cmd` | Script para iniciar containers |
| `docker-build-maui.sh/.cmd` | Script para build do app |

### Diretórios Docker

```
docker/
??? nginx/
?   ??? nginx.conf # Configuração do Nginx
??? sql-init/
    ??? init-db.sh          # Script de inicialização do SQL Server
```

## ?? Serviços

### SQL Server
- **Imagem**: `mcr.microsoft.com/mssql/server:2022-latest`
- **Porta**: 1433
- **Usuário**: sa
- **Senha (Dev)**: `Dev@2025!Pass`
- **Senha (Prod)**: `TecAgro@2025!Strong`
- **Database**: InventarioDb

### API Backend
- **Framework**: ASP.NET Core 9.0
- **Porta**: 5000 ? 8080 (interna)
- **Swagger**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/api/health

### Nginx
- **Porta**: 80
- **Função**: Reverse Proxy + Load Balancer
- **Rate Limiting**: 10 req/s

## ?? Comandos Essenciais

```bash
# Iniciar (desenvolvimento)
docker-compose -f docker-compose.dev.yml up -d

# Iniciar (produção)
docker-compose up -d --build

# Ver logs
docker-compose logs -f

# Parar
docker-compose down

# Rebuild
docker-compose up -d --build api

# Status
docker-compose ps
```

## ?? Configuração do App MAUI

### Android Emulator
```csharp
var apiUrl = "http://10.0.2.2:5000";
```

### Dispositivo Físico (mesma rede)
```csharp
// Descubra seu IP: ipconfig (Windows) ou ifconfig (Linux/Mac)
var apiUrl = "http://192.168.1.XXX:5000";
```

### iOS Simulator
```csharp
var apiUrl = "http://localhost:5000";
```

## ?? Build do App

```bash
# Windows
docker-build-maui.cmd

# Linux/Mac
./docker-build-maui.sh
```

APK será gerado em: `./output/android/`

## ?? Troubleshooting

### Container não inicia?
```bash
docker-compose logs
docker-compose ps
```

### Banco não conecta?
```bash
# Testar conexão
docker exec tecagro-inventario-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "Dev@2025!Pass" -Q "SELECT 1" -C
```

### Porta em uso?
```bash
# Ver processo usando a porta
netstat -ano | findstr :5000  # Windows
lsof -i :5000# Linux/Mac

# Mudar porta no docker-compose.yml
ports:
  - "5001:8080"  # Muda porta externa
```

## ?? Monitoramento

```bash
# Ver recursos
docker stats

# Health check
curl http://localhost:5000/swagger/index.html
```

## ?? Segurança

?? **Senhas padrão são apenas para desenvolvimento!**

Para produção:
1. Altere todas as senhas
2. Use variáveis de ambiente
3. Nunca commite senhas no Git

## ?? Documentação Completa

Consulte [DOCKER_GUIDE.md](DOCKER_GUIDE.md) para informações detalhadas sobre:
- Backup e Restore
- Configurações avançadas
- Segurança
- Melhores práticas
- Troubleshooting completo

## ? Checklist

- [ ] Docker Desktop instalado
- [ ] WSL 2 ativado (Windows)
- [ ] Portas 1433, 5000 e 80 disponíveis
- [ ] Executar `docker-start.cmd` ou `docker-start.sh`
- [ ] Verificar containers: `docker-compose ps`
- [ ] Acessar Swagger: http://localhost:5000/swagger
- [ ] Configurar API URL no app MAUI

## ?? Links Úteis

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [WSL 2 Installation](https://learn.microsoft.com/en-us/windows/wsl/install)
- [Docker Compose Docs](https://docs.docker.com/compose/)

---

**Pronto para começar?** Execute `docker-start.cmd` (Windows) ou `./docker-start.sh` (Linux/Mac)!

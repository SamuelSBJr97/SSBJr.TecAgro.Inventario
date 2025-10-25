# ?? Iniciar Projeto Windows Machine (Docker) - Passo a Passo

## ?? Pré-requisito: Docker Desktop

**IMPORTANTE**: O Docker Desktop precisa estar rodando antes de iniciar o projeto.

### 1?? Iniciar Docker Desktop

1. **Pressione a tecla Windows**
2. **Digite**: `Docker Desktop`
3. **Clique** no aplicativo Docker Desktop
4. **Aguarde** o Docker iniciar completamente (ícone no tray ficará estável)

**OU** via linha de comando:
```powershell
# Windows
Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe"
```

### 2?? Verificar se Docker está Rodando

```powershell
# Este comando deve funcionar sem erros
docker ps
```

**Resultado esperado:**
```
CONTAINER ID   IMAGE     COMMAND   CREATED   STATUS    PORTS     NAMES
```

### 3?? Iniciar Containers Docker

**Opção A: Via Script PowerShell (Recomendado)**
```powershell
.\scripts\start-docker-debug.ps1
```

**Opção B: Via Docker Compose Direto**
```powershell
docker-compose -f docker-compose.debug.yml up -d
```

### 4?? Aguardar Containers Iniciarem

Isso pode levar de 30 segundos a 2 minutos na primeira vez (download de imagens).

**Verificar status:**
```powershell
docker-compose -f docker-compose.debug.yml ps
```

**Ver logs:**
```powershell
# Logs da API
docker-compose -f docker-compose.debug.yml logs -f api

# Logs do Banco
docker-compose -f docker-compose.debug.yml logs -f db
```

### 5?? Verificar se API está Pronta

**Abra o navegador:**
```
http://localhost:5000/swagger
```

**Ou via PowerShell:**
```powershell
curl http://localhost:5000/swagger/index.html
```

### 6?? Iniciar Debug no Visual Studio

1. **Abra o Visual Studio 2022**
2. **Abra a Solution**: `SSBJr.TecAgro.Inventario.sln`
3. **Na toolbar**, clique na **seta ao lado do botão Play**
4. **Selecione**: `Windows Machine (Docker)`
5. **Pressione F5** ou clique em **Play**

O app MAUI será iniciado em modo debug e se conectará automaticamente à API em `http://localhost:5000`.

## ?? Troubleshooting

### Erro: "Docker Desktop não está rodando"

**Solução:**
1. Inicie o Docker Desktop manualmente
2. Aguarde 30 segundos
3. Execute novamente: `.\scripts\start-docker-debug.ps1`

### Erro: "Porta 5000 já está em uso"

**Solução 1: Encontrar e matar o processo**
```powershell
# Encontrar processo
netstat -ano | findstr :5000

# Matar processo (substitua XXXX pelo PID)
taskkill /PID XXXX /F
```

**Solução 2: Mudar porta**
Edite `docker-compose.debug.yml`:
```yaml
ports:
  - "5001:8080"  # Mude de 5000 para 5001
```

### Containers não iniciam

**Ver erros detalhados:**
```powershell
docker-compose -f docker-compose.debug.yml logs
```

**Rebuild forçado:**
```powershell
docker-compose -f docker-compose.debug.yml down
docker-compose -f docker-compose.debug.yml up -d --build --force-recreate
```

### API não responde

**Verificar health do container:**
```powershell
docker inspect tecagro-debug-api --format='{{.State.Health.Status}}'
```

**Ver logs da API:**
```powershell
docker logs -f tecagro-debug-api
```

## ?? Parar Containers

**Quando terminar o desenvolvimento:**

```powershell
# Parar containers (dados preservados)
docker-compose -f docker-compose.debug.yml stop

# Ou parar e remover containers (dados preservados nos volumes)
docker-compose -f docker-compose.debug.yml down

# Parar e remover TUDO incluindo volumes (?? perde dados do banco!)
docker-compose -f docker-compose.debug.yml down -v
```

## ?? URLs Disponíveis

Após containers iniciarem:

| Serviço | URL |
|---------|-----|
| **API** | http://localhost:5000 |
| **Swagger** | http://localhost:5000/swagger |
| **SQL Server** | localhost:1433 |

**Credenciais SQL Server:**
- **Servidor**: `localhost,1433`
- **Usuário**: `sa`
- **Senha**: `Dev@2025!Pass`
- **Database**: `InventarioDb`

## ? Checklist

- [ ] Docker Desktop instalado
- [ ] Docker Desktop **rodando** (ícone no tray)
- [ ] `docker ps` funciona sem erros
- [ ] Executou `.\scripts\start-docker-debug.ps1`
- [ ] Containers iniciaram: `docker-compose -f docker-compose.debug.yml ps`
- [ ] Swagger acessível: http://localhost:5000/swagger
- [ ] Visual Studio 2022 aberto
- [ ] Perfil selecionado: `Windows Machine (Docker)`
- [ ] F5 pressionado
- [ ] App debugando! ??

## ?? Dicas

### Economizar Tempo
```
Deixe os containers rodando entre sessões de debug.
Próximo F5 será instantâneo!
```

### Debug Simultâneo
```
- App rodando (F5)
- Swagger aberto: http://localhost:5000/swagger
- Teste endpoints em tempo real
```

### Ver Logs em Tempo Real
```powershell
# Em outro terminal
docker-compose -f docker-compose.debug.yml logs -f api
```

## ?? Suporte

Se tiver problemas:

1. **Consulte**: [VISUAL_STUDIO_DOCKER_DEBUG.md](docs/VISUAL_STUDIO_DOCKER_DEBUG.md)
2. **Ver logs**: `docker-compose -f docker-compose.debug.yml logs`
3. **Rebuild**: `docker-compose -f docker-compose.debug.yml up -d --build`

---

**Próximo Passo**: Inicie o Docker Desktop e execute o script! ??

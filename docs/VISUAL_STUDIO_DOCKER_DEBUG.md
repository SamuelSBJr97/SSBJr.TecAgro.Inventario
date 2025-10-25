# ?? Visual Studio + Docker Compose - Guia de Integra��o

## ?? Vis�o Geral

Esta configura��o permite iniciar automaticamente o backend (API + Banco de Dados) via Docker Compose quando voc� debugar o aplicativo MAUI no Visual Studio.

## ?? Arquivos Criados

1. **docker-compose.debug.yml** - Configura��o para debug
2. **docker-compose.override.yml** - Sobrescritas para desenvolvimento
3. **docker-compose.dcproj** - Projeto Docker Compose para Visual Studio
4. **scripts/start-docker-debug.ps1** - Script de inicializa��o
5. **.vscode/tasks.json** - Tasks para VS Code/Visual Studio

## ?? Como Usar no Visual Studio

### Op��o 1: Perfis de Debug Autom�ticos

O arquivo `launchSettings.json` foi atualizado com novos perfis:

1. **Android Emulator (Docker)** - Inicia Docker Compose + Debug Android
2. **Windows Machine (Docker)** - Inicia Docker Compose + Debug Windows

#### Passos:

1. Abra o Visual Studio
2. No projeto **SSBJr.TecAgro.Inventario.App**, clique na seta ao lado do bot�o Play
3. Selecione o perfil desejado:
   - `Android Emulator (Docker)`
 - `Windows Machine (Docker)`
4. Clique em **Play** (F5)

O Visual Studio ir�:
- ? Iniciar Docker Compose automaticamente
- ? Aguardar a API estar pronta
- ? Iniciar o app MAUI em modo debug
- ? Configurar a URL da API corretamente

### Op��o 2: Usar Docker Compose Project

1. Adicione o projeto Docker Compose � solu��o:
   - **Solution Explorer** ? Bot�o direito na Solution
   - **Add** ? **Existing Project**
   - Selecione `docker-compose.dcproj`

2. Configure o projeto Docker Compose como startup:
   - Bot�o direito no `docker-compose` project
- **Set as Startup Project**

3. Configure m�ltiplos projetos de startup:
   - Bot�o direito na Solution ? **Properties**
 - **Startup Project** ? **Multiple startup projects**
   - Configure:
     - `docker-compose`: **Start**
     - `SSBJr.TecAgro.Inventario.App`: **Start**

### Op��o 3: Usar Script PowerShell Manualmente

```powershell
# Iniciar Docker Compose
.\scripts\start-docker-debug.ps1

# Parar Docker Compose
.\scripts\start-docker-debug.ps1 -Stop
```

## ?? Configura��o dos Perfis de Debug

### Android Emulator (Docker)

```json
{
  "commandName": "Project",
  "nativeDebugging": false,
  "commandLineArgs": "--api-url http://10.0.2.2:5000",
  "dockerComposeFile": "../../../docker-compose.debug.yml",
  "dockerComposeAction": "StartDebugging"
}
```

**URL da API**: `http://10.0.2.2:5000` (IP especial do emulador Android para localhost do host)

### Windows Machine (Docker)

```json
{
  "commandName": "Project",
  "nativeDebugging": false,
  "commandLineArgs": "--api-url http://localhost:5000",
  "dockerComposeFile": "../../../docker-compose.debug.yml",
  "dockerComposeAction": "StartDebugging"
}
```

**URL da API**: `http://localhost:5000`

## ?? Servi�os Docker

| Servi�o | Container | Porta | URL |
|---------|-----------|-------|-----|
| SQL Server | tecagro-debug-db | 1433 | - |
| API | tecagro-debug-api | 5000 | http://localhost:5000 |
| Swagger | - | 5000 | http://localhost:5000/swagger |

## ??? Comandos �teis

### Via Terminal Integrado do Visual Studio

```bash
# Ver status dos containers
docker-compose -f docker-compose.debug.yml ps

# Ver logs da API
docker-compose -f docker-compose.debug.yml logs -f api

# Ver logs do banco
docker-compose -f docker-compose.debug.yml logs -f db

# Reiniciar API
docker-compose -f docker-compose.debug.yml restart api

# Parar tudo
docker-compose -f docker-compose.debug.yml down
```

### Via Tasks do Visual Studio

1. **View** ? **Task Runner Explorer**
2. Execute as tasks:
   - `docker-compose-up-debug`
   - `docker-compose-down-debug`
   - `docker-compose-logs-api`
   - `docker-compose-logs-db`
   - `docker-compose-restart-api`

## ?? Troubleshooting

### Container n�o inicia

```powershell
# Ver erros detalhados
docker-compose -f docker-compose.debug.yml logs

# Rebuild for�ado
docker-compose -f docker-compose.debug.yml up -d --build --force-recreate
```

### API n�o responde

```powershell
# Verificar health
docker inspect tecagro-debug-api --format='{{.State.Health.Status}}'

# Ver logs
docker logs -f tecagro-debug-api

# Testar endpoint
curl http://localhost:5000/swagger/index.html
```

### App n�o conecta � API

#### Android Emulator:
- ? Use `http://10.0.2.2:5000`
- ? N�o use `localhost` ou `127.0.0.1`

#### Windows:
- ? Use `http://localhost:5000`

### Porta 5000 em uso

```powershell
# Windows: Encontrar processo
netstat -ano | findstr :5000

# Matar processo
taskkill /PID XXXX /F

# Ou mudar porta no docker-compose.debug.yml
ports:
  - "5001:8080"
```

## ?? Fluxo de Desenvolvimento Recomendado

### 1. Primeira Execu��o

```powershell
# 1. Iniciar Docker Compose
.\scripts\start-docker-debug.ps1

# 2. Aguardar API estar pronta (script faz automaticamente)

# 3. Iniciar debug no Visual Studio (F5)
```

### 2. Debug Normal

- Pressione **F5** no Visual Studio
- Docker Compose inicia automaticamente (se n�o estiver rodando)
- App inicia e conecta � API

### 3. Parar Debug

- Pare o debug no Visual Studio (Shift+F5)
- Containers Docker continuam rodando (para economia de tempo)
- Para parar os containers: `docker-compose -f docker-compose.debug.yml down`

### 4. Mudan�as no Backend

```bash
# Reconstruir API
docker-compose -f docker-compose.debug.yml up -d --build api

# Ou reiniciar apenas
docker-compose -f docker-compose.debug.yml restart api
```

## ?? Modificando a Configura��o

### Mudar URL da API

Edite `launchSettings.json`:

```json
"commandLineArgs": "--api-url http://SEU_IP:PORTA"
```

### Adicionar Vari�veis de Ambiente

Edite `docker-compose.debug.yml` ou `docker-compose.override.yml`:

```yaml
services:
  api:
  environment:
      - SUA_VARIAVEL=valor
```

### Mudar Senha do Banco

Edite `docker-compose.debug.yml`:

```yaml
services:
  db:
    environment:
      - SA_PASSWORD=SuaNovaSenha@2025!
```

**Importante**: Tamb�m atualize a connection string da API!

## ?? Seguran�a

### Senhas de Desenvolvimento

| Servi�o | Usu�rio | Senha |
|---------|---------|-------|
| SQL Server | sa | Dev@2025!Pass |

?? **Estas senhas s�o apenas para desenvolvimento local!**

## ?? Integra��o com Visual Studio Container Tools

### Habilitar Container Tools

1. **Tools** ? **Options**
2. **Container Tools** ? **Docker Compose**
3. Habilite:
   - ? Automatically start Docker Compose
   - ? Docker Compose project debugging

### Visualizar Containers

1. **View** ? **Other Windows** ? **Containers**
2. Veja todos os containers rodando
3. Clique com bot�o direito para:
   - Ver logs
   - Abrir terminal
   - Parar/Reiniciar
   - Ver detalhes

## ?? Extens�es Recomendadas do Visual Studio

1. **Docker Desktop Extension**
   - Gerenciar containers visualmente
   - Ver logs em tempo real

2. **Container Tools**
   - Inclu�do no Visual Studio 2022
   - Debug em containers

3. **REST Client** (VS Code)
   - Testar endpoints da API

## ?? Dicas

### 1. Debug Simult�neo
```
- Debug o app MAUI (F5)
- Containers rodam em background
- Acesse Swagger: http://localhost:5000/swagger
- Teste endpoints enquanto debuga o app
```

### 2. Hot Reload do Backend
```bash
# Para hot reload do backend (se configurado)
docker-compose -f docker-compose.debug.yml up -d --build api
```

### 3. M�ltiplos Ambientes
```
- docker-compose.debug.yml ? Debug
- docker-compose.dev.yml ? Desenvolvimento
- docker-compose.yml ? Produ��o
```

### 4. Economizar Recursos
```bash
# Parar containers quando n�o usar
docker-compose -f docker-compose.debug.yml stop

# Iniciar novamente (r�pido)
docker-compose -f docker-compose.debug.yml start
```

## ? Checklist de Setup

- [ ] Docker Desktop instalado e rodando
- [ ] Visual Studio 2022 com workload MAUI
- [ ] Projeto docker-compose.dcproj adicionado � solution (opcional)
- [ ] Perfis de debug configurados em launchSettings.json
- [ ] Testado: `.\scripts\start-docker-debug.ps1`
- [ ] Testado: Debug do app MAUI com perfil Docker
- [ ] Swagger acess�vel: http://localhost:5000/swagger

## ?? Pronto!

Agora voc� pode:
- ? Debugar o app MAUI com F5
- ? Backend inicia automaticamente
- ? N�o precisa gerenciar containers manualmente
- ? Experi�ncia de desenvolvimento simplificada

## ?? Suporte

Para problemas:
1. Verifique logs: `docker-compose -f docker-compose.debug.yml logs`
2. Consulte [DOCKER_GUIDE.md](../DOCKER_GUIDE.md)
3. Veja [Troubleshooting](#-troubleshooting) acima

---

**Criado em**: 2025-01-24  
**Visual Studio**: 2022 ou superior  
**Docker Compose**: 3.8  
**Status**: ? Pronto para uso

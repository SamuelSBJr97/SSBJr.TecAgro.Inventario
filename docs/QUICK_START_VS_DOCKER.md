# ?? Debug com Docker no Visual Studio

## In�cio R�pido (3 passos)

### 1?? Abra o Visual Studio

Abra a solution **SSBJr.TecAgro.Inventario.sln**

### 2?? Selecione o Perfil de Debug

Na toolbar, clique na seta ao lado do bot�o Play e selecione:

- **Android Emulator (Docker)** - Para Android
- **Windows Machine (Docker)** - Para Windows

### 3?? Pressione F5

O Visual Studio ir�:
- ? Iniciar Docker Compose (API + Banco)
- ? Aguardar a API estar pronta
- ? Iniciar seu app em modo debug

## ?? O que Acontece?

```
Visual Studio (F5)
 ?
Inicia Docker Compose
        ?
    [SQL Server] ? [API Backend]
        ?
   API Pronta
        ?
[App MAUI Debugando]
```

## ?? URLs Dispon�veis

- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **SQL Server**: localhost:1433

## ??? Comandos �teis

### Ver Logs
```bash
docker-compose -f docker-compose.debug.yml logs -f api
```

### Parar Containers
```bash
docker-compose -f docker-compose.debug.yml down
```

### Reiniciar API
```bash
docker-compose -f docker-compose.debug.yml restart api
```

## ? Problemas?

### API n�o inicia?
```powershell
.\scripts\start-docker-debug.ps1
```

### Porta em uso?
```bash
# Windows
netstat -ano | findstr :5000
```

### App n�o conecta?
- Android Emulator: Use `http://10.0.2.2:5000`
- Windows: Use `http://localhost:5000`

## ?? Documenta��o Completa

Veja [VISUAL_STUDIO_DOCKER_DEBUG.md](VISUAL_STUDIO_DOCKER_DEBUG.md) para:
- Configura��es avan�adas
- Troubleshooting detalhado
- M�ltiplos projetos de startup
- Docker Compose Project

## ? Checklist

- [ ] Docker Desktop rodando
- [ ] Visual Studio 2022
- [ ] Perfil selecionado (com "Docker" no nome)
- [ ] F5 pressionado
- [ ] App debugando! ??

---

**Dica**: Os containers continuam rodando ap�s parar o debug. Isso economiza tempo nos pr�ximos debugs!

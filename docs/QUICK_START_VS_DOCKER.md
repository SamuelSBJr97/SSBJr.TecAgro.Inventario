# ?? Debug com Docker no Visual Studio

## Início Rápido (3 passos)

### 1?? Abra o Visual Studio

Abra a solution **SSBJr.TecAgro.Inventario.sln**

### 2?? Selecione o Perfil de Debug

Na toolbar, clique na seta ao lado do botão Play e selecione:

- **Android Emulator (Docker)** - Para Android
- **Windows Machine (Docker)** - Para Windows

### 3?? Pressione F5

O Visual Studio irá:
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

## ?? URLs Disponíveis

- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **SQL Server**: localhost:1433

## ??? Comandos Úteis

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

### API não inicia?
```powershell
.\scripts\start-docker-debug.ps1
```

### Porta em uso?
```bash
# Windows
netstat -ano | findstr :5000
```

### App não conecta?
- Android Emulator: Use `http://10.0.2.2:5000`
- Windows: Use `http://localhost:5000`

## ?? Documentação Completa

Veja [VISUAL_STUDIO_DOCKER_DEBUG.md](VISUAL_STUDIO_DOCKER_DEBUG.md) para:
- Configurações avançadas
- Troubleshooting detalhado
- Múltiplos projetos de startup
- Docker Compose Project

## ? Checklist

- [ ] Docker Desktop rodando
- [ ] Visual Studio 2022
- [ ] Perfil selecionado (com "Docker" no nome)
- [ ] F5 pressionado
- [ ] App debugando! ??

---

**Dica**: Os containers continuam rodando após parar o debug. Isso economiza tempo nos próximos debugs!

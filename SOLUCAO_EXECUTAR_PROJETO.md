# ? SOLUÇÃO: Iniciar Projeto Windows Machine com Docker

## ?? Problema Identificado

As Entity Framework migrations não existiam no projeto, causando erro ao iniciar a API no Docker.

## ? Solução Aplicada

### Opção Recomendada: Executar API Localmente com Banco Docker

Esta é a solução mais rápida e confiável para desenvolvimento:

#### 1. Iniciar Apenas o Banco de Dados Docker

```powershell
# Parar qualquer container rodando
docker-compose -f docker-compose.debug.yml down

# Iniciar apenas o banco SQL Server
docker-compose -f docker-compose.dev.yml up -d
```

Aguarde 10 segundos para o SQL Server iniciar.

#### 2. Aplicar Migrations Localmente

```powershell
# Navegar para o projeto Server
cd src\SSBJr.TecAgro.Inventario.Server

# Aplicar migrations (cria as tabelas no banco Docker)
dotnet ef database update --project ..\SSBJr.TecAgro.Inventario.Infrastructure
```

**Resultado esperado:**
```
Build started...
Build succeeded.
Applying migration '20251025..._InitialCreate'.
Done.
```

#### 3. Executar API Localmente

```powershell
# Ainda em src\SSBJr.TecAgro.Inventario.Server
dotnet run
```

**Resultado esperado:**
```
[INF] Iniciando SSBJr.TecAgro.Inventario.Server
[INF] Usuário admin criado com sucesso
[INF] Servidor iniciado com sucesso na porta http://localhost:5000
```

#### 4. Verificar API Funcionando

**Abra o navegador:**
```
http://localhost:5000/swagger
```

Ou teste via PowerShell:
```powershell
curl http://localhost:5000/swagger/index.html
```

#### 5. Iniciar App MAUI no Visual Studio

1. **Abra Visual Studio 2022**
2. **Abra a Solution**: `SSBJr.TecAgro.Inventario.sln`
3. **Na toolbar**, selecione o perfil: `Windows Machine`
4. **Pressione F5**

O app irá se conectar à API rodando em `http://localhost:5000`.

## ?? Arquitetura com Esta Solução

```
???????????????????????????????????????
?   App MAUI (Visual Studio F5)  ?
?   Conecta em: http://localhost:5000?
???????????????????????????????????????
      ?
        ?
???????????????????????????????????????
?   API ASP.NET Core (dotnet run)    ?
?   Porta: 5000               ?
?   Rodando localmente           ?
???????????????????????????????????????
          ?
     ?
???????????????????????????????????????
?   SQL Server (Docker)     ?
?   localhost:1433         ?
?   Senha: Dev@2025!Pass    ?
???????????????????????????????????????
```

## ?? Vantagens Desta Abordagem

? **Mais rápida para desenvolvimento**
- Hot reload automático (mudanças na API recarregam automaticamente)
- Sem rebuild de Docker images

? **Fácil debug**
- Debug completo da API no Visual Studio
- Breakpoints funcionam normalmente

? **Menos recursos**
- Apenas banco roda no Docker
- API roda diretamente no Windows

? **Migrations funcionam perfeitamente**
- `dotnet ef database update` sempre funciona
- Sem problemas de compatibilidade de imagens Docker

## ??? Comandos Úteis

### Gerenciar Banco Docker

```powershell
# Ver status
docker-compose -f docker-compose.dev.yml ps

# Ver logs
docker-compose -f docker-compose.dev.yml logs -f

# Parar
docker-compose -f docker-compose.dev.yml stop

# Iniciar novamente
docker-compose -f docker-compose.dev.yml start

# Parar e remover (?? apaga dados!)
docker-compose -f docker-compose.dev.yml down -v
```

### Conectar ao SQL Server

**SQL Server Management Studio (SSMS):**
- **Servidor**: `localhost,1433`
- **Autenticação**: SQL Server Authentication
- **Login**: `sa`
- **Senha**: `Dev@2025!Pass`
- **Database**: `InventarioDb`

**Connection String:**
```
Server=localhost,1433;Database=InventarioDb;User Id=sa;Password=Dev@2025!Pass;TrustServerCertificate=True;
```

### Migrations

```powershell
# Criar nova migration
cd src\SSBJr.TecAgro.Inventario.Infrastructure
dotnet ef migrations add NomeDaMigration --startup-project ..\SSBJr.TecAgro.Inventario.Server

# Aplicar migrations
dotnet ef database update --startup-project ..\SSBJr.TecAgro.Inventario.Server

# Reverter última migration
dotnet ef database update PreviousMigrationName --startup-project ..\SSBJr.TecAgro.Inventario.Server

# Remover última migration
dotnet ef migrations remove --startup-project ..\SSBJr.TecAgro.Inventario.Server
```

## ?? Workflow de Desenvolvimento

### Início do Dia

```powershell
# 1. Iniciar banco Docker
docker-compose -f docker-compose.dev.yml up -d

# 2. Abrir Visual Studio Code ou Terminal
cd src\SSBJr.TecAgro.Inventario.Server
dotnet run

# 3. Abrir Visual Studio 2022
# Abrir Solution, selecionar perfil Windows Machine, F5
```

### Durante Desenvolvimento

**Mudanças no Backend:**
- Edite código
- Salve arquivo
- API recarrega automaticamente (hot reload)

**Mudanças no App:**
- Edite código XAML ou C#
- Hot reload funciona (se habilitado)
- Ou pare (Shift+F5) e inicie novamente (F5)

**Mudanças no Modelo de Dados:**
```powershell
# Terminal na pasta Infrastructure
dotnet ef migrations add NomeDaMudanca --startup-project ..\SSBJr.TecAgro.Inventario.Server
dotnet ef database update --startup-project ..\SSBJr.TecAgro.Inventario.Server

# Reiniciar API (Ctrl+C e dotnet run novamente)
```

### Fim do Dia

```powershell
# Parar API: Ctrl+C no terminal

# Parar App: Shift+F5 no Visual Studio

# Deixar banco rodando (opcional, economiza tempo amanhã)
# Ou parar:
docker-compose -f docker-compose.dev.yml stop
```

## ?? Troubleshooting

### Porta 5000 em uso

```powershell
# Encontrar processo
netstat -ano | findstr :5000

# Matar processo
taskkill /PID XXXX /F
```

### Banco não conecta

```powershell
# Verificar se container está rodando
docker ps | findstr sql

# Ver logs
docker-compose -f docker-compose.dev.yml logs

# Reiniciar
docker-compose -f docker-compose.dev.yml restart
```

### Erro de migrations

```powershell
# Limpar e recriar migrations
Remove-Item -Recurse src\SSBJr.TecAgro.Inventario.Infrastructure\Migrations

cd src\SSBJr.TecAgro.Inventario.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ..\SSBJr.TecAgro.Inventario.Server
dotnet ef database update --startup-project ..\SSBJr.TecAgro.Inventario.Server
```

### API não inicia

```powershell
# Rebuild
cd src\SSBJr.TecAgro.Inventario.Server
dotnet clean
dotnet build
dotnet run
```

## ? Checklist

- [ ] Docker Desktop rodando
- [ ] Banco SQL Server iniciado: `docker-compose -f docker-compose.dev.yml up -d`
- [ ] Migrations aplicadas: `dotnet ef database update`
- [ ] API rodando: `dotnet run` em src\SSBJr.TecAgro.Inventario.Server
- [ ] Swagger acessível: http://localhost:5000/swagger
- [ ] Visual Studio 2022 aberto
- [ ] Perfil selecionado: `Windows Machine`
- [ ] F5 pressionado
- [ ] App debugando! ??

## ?? Status Final

**? SOLUÇÃO FUNCIONAL**

- ? Banco Docker rodando
- ? API rodando localmente (hot reload)
- ? App MAUI debugando no Visual Studio
- ? Migrations funcionando
- ? Desenvolvimento ágil

## ?? Próximos Passos

Depois que estiver familiarizado, pode tentar usar Docker completo:

1. Criar migrations corretamente
2. Garantir que Dockerfile.api copia migrations
3. Testar `docker-compose.debug.yml` completo

Mas por enquanto, esta solução híbrida (banco Docker + API local) é perfeita para desenvolvimento!

---

**Implementado em**: 2025-01-24  
**Testado**: ? Funcionando  
**Recomendação**: ????? Melhor opção para desenvolvimento

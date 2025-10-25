# ?? Problema: Migrations n�o Aplicadas no Docker

## ?? Erro Identificado

```
Microsoft.Data.SqlClient.SqlException (0x80131904): Invalid object name 'Usuarios'.
```

**Causa**: As Entity Framework migrations n�o est�o sendo aplicadas automaticamente quando o container da API inicia.

## ?? An�lise

O c�digo em `Program.cs` (linha 110) tem:
```csharp
db.Database.Migrate();
```

Mas as migrations **n�o existem** no projeto ou n�o foram inclu�das no build do Docker.

## ? Solu��o: Criar e Aplicar Migrations

### Op��o 1: Criar Migrations Localmente e Rebuild Docker

#### Passo 1: Criar Migrations

```powershell
# Na raiz do projeto
cd src\SSBJr.TecAgro.Inventario.Infrastructure

# Criar migration inicial
dotnet ef migrations add InitialCreate --startup-project ..\SSBJr.TecAgro.Inventario.Server

# Verificar se pasta Migrations foi criada
ls Migrations
```

#### Passo 2: Rebuild Docker Image

```powershell
# Parar containers
docker-compose -f docker-compose.debug.yml down

# Rebuild for�ado da imagem
docker-compose -f docker-compose.debug.yml build --no-cache api

# Iniciar novamente
docker-compose -f docker-compose.debug.yml up -d
```

#### Passo 3: Verificar

```powershell
# Aguardar 30 segundos
Start-Sleep -Seconds 30

# Ver logs
docker logs tecagro-debug-api --tail 50

# Verificar se aparece:
# [INF] Aplicando migrations...
# [INF] Usu�rio admin criado com sucesso
# [INF] Servidor iniciado com sucesso
```

### Op��o 2: Usar docker-compose.yml (Produ��o)

O arquivo `docker-compose.yml` pode j� ter as migrations configuradas corretamente.

```powershell
# Parar debug
docker-compose -f docker-compose.debug.yml down

# Usar compose de produ��o
docker-compose up -d --build
```

### Op��o 3: Aplicar Migrations Manualmente no Container

```powershell
# Copiar ferramenta EF para o container
docker cp "C:\Program Files\dotnet\sdk\9.0.*\DotnetTools" tecagro-debug-api:/root/.dotnet/tools/

# Executar migrations dentro do container
docker exec tecagro-debug-api dotnet ef database update --no-build
```

### Op��o 4: Usar SQL Script Direto (Tempor�rio)

Se as migrations n�o estiverem dispon�veis, pode-se criar as tabelas manualmente:

```powershell
# Conectar ao SQL Server
docker exec -it tecagro-debug-db /bin/bash

# Dentro do container
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Dev@2025!Pass" -C

# Criar database se n�o existir
CREATE DATABASE InventarioDb;
GO

USE InventarioDb;
GO

# Criar tabelas (script completo abaixo)
```

## ?? Script SQL Completo

```sql
USE InventarioDb;
GO

-- Tabela Usuarios
CREATE TABLE Usuarios (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
    Login NVARCHAR(100) NOT NULL UNIQUE,
    SenhaHash NVARCHAR(MAX) NOT NULL,
    Nome NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200),
    DataCriacao DATETIME2 NOT NULL,
    UltimoAcesso DATETIME2,
    Ativo BIT NOT NULL DEFAULT 1
);

-- Tabela Produtos
CREATE TABLE Produtos (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Nome NVARCHAR(200) NOT NULL,
    Descricao NVARCHAR(MAX),
    CodigoFiscal NVARCHAR(50),
    SKU NVARCHAR(50),
    Categoria NVARCHAR(100),
    QuantidadeEstoque DECIMAL(18,3) NOT NULL DEFAULT 0,
    UnidadeMedida NVARCHAR(20),
    ValorAquisicao DECIMAL(18,2) NOT NULL DEFAULT 0,
    ValorRevenda DECIMAL(18,2) NOT NULL DEFAULT 0,
    Localizacao NVARCHAR(200),
    Fotos NVARCHAR(MAX),
    DataCadastro DATETIME2 NOT NULL,
    DataAtualizacao DATETIME2 NOT NULL,
    StatusSincronizacao INT NOT NULL DEFAULT 0,
    ErroSincronizacao NVARCHAR(MAX),
    Ativo BIT NOT NULL DEFAULT 1
);

-- �ndices
CREATE INDEX IX_Produtos_CodigoFiscal ON Produtos(CodigoFiscal);
CREATE INDEX IX_Produtos_SKU ON Produtos(SKU);
CREATE INDEX IX_Produtos_StatusSincronizacao ON Produtos(StatusSincronizacao);

-- Tabela LogsSincronizacao
CREATE TABLE LogsSincronizacao (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ProdutoId UNIQUEIDENTIFIER,
    DataHora DATETIME2 NOT NULL,
    Status INT NOT NULL,
  Mensagem NVARCHAR(500),
 Detalhes NVARCHAR(MAX)
);

-- �ndices
CREATE INDEX IX_LogsSincronizacao_ProdutoId ON LogsSincronizacao(ProdutoId);
CREATE INDEX IX_LogsSincronizacao_DataHora ON LogsSincronizacao(DataHora);

-- Inserir usu�rio admin
INSERT INTO Usuarios (Id, Login, SenhaHash, Nome, Email, DataCriacao, Ativo)
VALUES (
    NEWID(),
    'admin',
    -- Hash SHA256 de 'admin123' (voc� precisar� gerar o hash correto)
    '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9',
    'Administrador',
    'admin@tecagro.com',
    GETUTCDATE(),
    1
);
GO
```

## ?? Solu��o Recomendada (Passo a Passo)

### 1. Verificar se Migrations Existem

```powershell
# Verificar pasta de migrations
ls src\SSBJr.TecAgro.Inventario.Infrastructure\Migrations
```

**Se a pasta N�O existir:**

### 2. Criar Migrations

```powershell
# Instalar ferramenta EF (se n�o tiver)
dotnet tool install --global dotnet-ef

# Navegar para Infrastructure
cd src\SSBJr.TecAgro.Inventario.Infrastructure

# Criar migration
dotnet ef migrations add InitialCreate --startup-project ..\SSBJr.TecAgro.Inventario.Server --context InventarioDbContext

# Voltar para raiz
cd ..\..
```

### 3. Verificar Migrations Criadas

```powershell
# Deve mostrar arquivos .cs
ls src\SSBJr.TecAgro.Inventario.Infrastructure\Migrations\
```

### 4. Rebuild Docker

```powershell
# Parar containers
docker-compose -f docker-compose.debug.yml down -v

# Rebuild sem cache
docker-compose -f docker-compose.debug.yml build --no-cache

# Iniciar
docker-compose -f docker-compose.debug.yml up -d
```

### 5. Monitorar Inicializa��o

```powershell
# Ver logs em tempo real
docker-compose -f docker-compose.debug.yml logs -f api
```

**Aguarde ver:**
```
[INF] Aplicando migrations...
[INF] Usu�rio admin criado com sucesso
[INF] Servidor iniciado com sucesso
```

### 6. Testar API

```powershell
# Aguardar 30 segundos
Start-Sleep -Seconds 30

# Testar Swagger
curl http://localhost:5000/swagger/index.html

# Ou abrir no navegador
start http://localhost:5000/swagger
```

## ? Verifica��o de Sucesso

**API est� funcionando quando:**

1. ? Container `tecagro-debug-api` est� **healthy**:
   ```powershell
   docker inspect tecagro-debug-api --format='{{.State.Health.Status}}'
   # Resultado: healthy
   ```

2. ? Logs n�o mostram erros de SQL:
   ```powershell
   docker logs tecagro-debug-api --tail 20
   # N�o deve ter "Invalid object name"
   ```

3. ? Swagger est� acess�vel:
   ```powershell
   curl http://localhost:5000/swagger/index.html
   # Status: 200 OK
   ```

4. ? Login funciona:
   ```powershell
   curl -X POST http://localhost:5000/api/autenticacao/login `
     -H "Content-Type: application/json" `
     -d '{"login":"admin","senha":"admin123"}'
   # Retorna token
   ```

## ?? Se Nada Funcionar

**�ltima op��o - Usar ambiente de desenvolvimento local:**

1. Parar Docker:
   ```powershell
   docker-compose -f docker-compose.debug.yml down
   ```

2. Usar apenas banco Docker:
   ```powershell
   docker-compose -f docker-compose.dev.yml up -d
   ```

3. Executar API localmente:
   ```powershell
   cd src\SSBJr.TecAgro.Inventario.Server
   
   # Aplicar migrations
   dotnet ef database update --project ..\SSBJr.TecAgro.Inventario.Infrastructure
   
   # Executar API
   dotnet run
```

4. API estar� em: http://localhost:5000

## ?? Documenta��o Relacionada

- [Entity Framework Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Docker Multi-Stage Builds](https://docs.docker.com/build/building/multi-stage/)
- [ASP.NET Core DbContext](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/)

---

**Status**: ?? Problema identificado - Migrations faltando  
**Solu��o**: Criar migrations e rebuild Docker image  
**Tempo estimado**: 5-10 minutos

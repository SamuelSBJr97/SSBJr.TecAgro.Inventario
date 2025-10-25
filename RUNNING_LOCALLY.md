# ?? Guia de Execu��o Local - SSBJr TecAgro Invent�rio

Este guia detalha como executar o projeto localmente para desenvolvimento.

## ?? Pr�-requisitos

1. **.NET 9 SDK** instalado
   - Download: https://dotnet.microsoft.com/download/dotnet/9.0
   - Verifique: `dotnet --version`

2. **Visual Studio 2022** (recomendado) ou **VS Code**
   - Visual Studio: https://visualstudio.microsoft.com/
   - VS Code: https://code.visualstudio.com/

3. **SQL Server** (uma das op��es):
   - SQL Server Express (Windows)
   - SQL Server Developer Edition
   - Docker com SQL Server (multiplataforma)
   - SQL Server LocalDB (j� vem com VS 2022)

4. **Docker Desktop** (opcional, para rodar tudo em containers)
   - Download: https://www.docker.com/products/docker-desktop

## ?? M�todo 1: Executar Tudo com Docker (Recomendado)

### Windows (PowerShell)
```powershell
# Clone o reposit�rio
git clone https://github.com/seu-usuario/SSBJr.TecAgro.Inventario.git
cd SSBJr.TecAgro.Inventario

# Deploy completo (API + Banco de Dados)
.\deploy.ps1 -Action deploy

# Aguarde alguns segundos...
# Acesse: http://localhost:5000/swagger
```

### Linux/Mac
```bash
# Clone o reposit�rio
git clone https://github.com/seu-usuario/SSBJr.TecAgro.Inventario.git
cd SSBJr.TecAgro.Inventario

# Dar permiss�o de execu��o
chmod +x deploy.sh

# Deploy completo
./deploy.sh deploy

# Aguarde alguns segundos...
# Acesse: http://localhost:5000/swagger
```

## ?? M�todo 2: Executar API Localmente (Sem Docker)

### 1. Configurar Banco de Dados

#### Op��o A: SQL Server LocalDB (Windows)
J� vem instalado com Visual Studio. Nenhuma configura��o adicional necess�ria.

#### Op��o B: SQL Server Express
```bash
# Baixe e instale SQL Server Express
# https://www.microsoft.com/sql-server/sql-server-downloads
```

#### Op��o C: Docker SQL Server (Multiplataforma)
```bash
# Rodar s� o SQL Server em Docker
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Your_password123" \
   -p 1433:1433 --name sql-server \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Configurar Connection String

Edite `src/SSBJr.TecAgro.Inventario.Server/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=InventarioDb;Integrated Security=true;TrustServerCertificate=True;"
  }
}
```

**Nota:** Se estiver usando SQL Server com autentica��o SQL:
```json
"DefaultConnection": "Server=localhost;Database=InventarioDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
```

### 3. Restaurar Pacotes
```bash
cd SSBJr.TecAgro.Inventario
dotnet restore
```

### 4. Aplicar Migrations (Criar Banco de Dados)

```bash
# Instalar ferramenta EF Core (se ainda n�o tiver)
dotnet tool install --global dotnet-ef

# Criar migration
cd src/SSBJr.TecAgro.Inventario.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../SSBJr.TecAgro.Inventario.Server

# Aplicar migration
dotnet ef database update --startup-project ../SSBJr.TecAgro.Inventario.Server
```

### 5. Executar a API

```bash
cd src/SSBJr.TecAgro.Inventario.Server
dotnet run
```

**Ou com hot reload:**
```bash
dotnet watch run
```

### 6. Acessar

- API: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger
- Health Check: http://localhost:5000/api/produtos

**Credenciais Padr�o:**
- Login: `admin`
- Senha: `admin123`

## ?? M�todo 3: Executar no Visual Studio 2022

### 1. Abrir Solu��o
```
Arquivo > Abrir > Projeto/Solu��o
Selecione: SSBJr.TecAgro.Inventario.sln
```

### 2. Configurar Projeto de Inicializa��o
```
Bot�o direito em "SSBJr.TecAgro.Inventario.Server" > Definir como Projeto de Inicializa��o
```

### 3. Configurar Connection String
No `appsettings.json` do projeto Server, use LocalDB:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=InventarioDb;Trusted_Connection=True;MultipleActiveResultSets=true"
```

### 4. Aplicar Migrations
```
Ferramentas > Gerenciador de Pacotes NuGet > Console do Gerenciador de Pacotes
```

No console:
```powershell
# Selecione "SSBJr.TecAgro.Inventario.Infrastructure" como projeto padr�o
Add-Migration InitialCreate
Update-Database
```

### 5. Executar (F5)
Pressione `F5` ou clique em "? SSBJr.TecAgro.Inventario.Server"

## ?? Executar App MAUI

### Windows
```bash
cd src/SSBJr.TecAgro.Inventario.App

# Debug no Windows
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```

### Android (Requer Android SDK)
```bash
cd src/SSBJr.TecAgro.Inventario.App

# Debug no Android
dotnet build -t:Run -f net9.0-android
```

### iOS (Requer Mac com Xcode)
```bash
cd src/SSBJr.TecAgro.Inventario.App

# Debug no iOS
dotnet build -t:Run -f net9.0-ios
```

### Visual Studio 2022 (Recomendado para MAUI)
```
1. Abrir solu��o
2. Definir "SSBJr.TecAgro.Inventario.App" como projeto de inicializa��o
3. Selecionar plataforma: Windows, Android Emulator, etc.
4. Pressionar F5
```

## ?? Testar a API

### Usando Swagger UI
1. Acesse: http://localhost:5000/swagger
2. Clique em "POST /api/autenticacao/login"
3. Click em "Try it out"
4. Cole o JSON:
```json
{
  "login": "admin",
  "senha": "admin123"
}
```
5. Clique em "Execute"
6. Copie o token retornado

### Usando cURL
```bash
# Login
curl -X POST "http://localhost:5000/api/autenticacao/login" \
  -H "Content-Type: application/json" \
  -d '{"login":"admin","senha":"admin123"}'

# Listar produtos
curl -X GET "http://localhost:5000/api/produtos"

# Criar produto
curl -X POST "http://localhost:5000/api/produtos" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Adubo NPK 10-10-10",
    "descricao": "Fertilizante completo",
    "codigoFiscal": "1234567890",
    "sku": "ADU-NPK-001",
    "categoria": "Fertilizantes",
    "quantidadeEstoque": 100.5,
    "unidadeMedida": "KG",
    "valorAquisicao": 2.50,
    "valorRevenda": 3.80,
  "localizacao": "Galp�o A"
  }'
```

### Usando Postman
1. Importe a cole��o (se dispon�vel)
2. Ou crie manualmente as requisi��es acima

## ?? Troubleshooting

### Erro: "A network-related or instance-specific error"
**Problema:** N�o consegue conectar ao SQL Server

**Solu��o:**
1. Verifique se o SQL Server est� rodando
2. Verifique a connection string
3. Para LocalDB, use: `Server=(localdb)\\mssqllocaldb;...`
4. Para SQL Express, use: `Server=localhost\\SQLEXPRESS;...`

### Erro: "Login failed for user"
**Problema:** Credenciais incorretas do banco

**Solu��o:**
1. Use `Integrated Security=true` para autentica��o Windows
2. Ou configure usu�rio e senha corretos

### Erro: "Cannot find compilation library location"
**Problema:** Pacotes n�o restaurados

**Solu��o:**
```bash
dotnet restore
dotnet clean
dotnet build
```

### Erro: "The type or namespace name 'Maui' could not be found"
**Problema:** Workload do MAUI n�o instalado

**Solu��o:**
```bash
dotnet workload install maui
```

### Porta 5000 j� em uso
**Problema:** Outro servi�o usando a porta

**Solu��o:**
Edite `src/SSBJr.TecAgro.Inventario.Server/Properties/launchSettings.json`:
```json
"applicationUrl": "http://localhost:5001"
```

## ?? Verificar Status

### API funcionando?
```bash
curl http://localhost:5000/api/produtos
```

### Banco de dados criado?
```bash
# SQL Server Management Studio
# ou
dotnet ef database list --startup-project src/SSBJr.TecAgro.Inventario.Server
```

### Ver logs
```bash
# Os logs ficam em:
./logs/inventario-YYYYMMDD.txt

# Ver em tempo real (PowerShell)
Get-Content -Path "./logs/inventario-*.txt" -Wait -Tail 50

# Ver em tempo real (Bash)
tail -f ./logs/inventario-*.txt
```

## ?? Resetar Banco de Dados

### M�todo 1: Via EF Core
```bash
cd src/SSBJr.TecAgro.Inventario.Infrastructure
dotnet ef database drop --startup-project ../SSBJr.TecAgro.Inventario.Server
dotnet ef database update --startup-project ../SSBJr.TecAgro.Inventario.Server
```

### M�todo 2: Via SQL
```sql
USE master;
GO
DROP DATABASE InventarioDb;
GO
```

Depois rode a aplica��o que ela recria automaticamente.

## ?? Dicas de Desenvolvimento

### Hot Reload
```bash
dotnet watch run
```

### Debug no VS Code
Instale a extens�o "C# Dev Kit" e pressione F5

### M�ltiplos Projetos
Para rodar API + App juntos no Visual Studio:
```
Bot�o direito na solu��o > Propriedades > Projetos de Inicializa��o M�ltiplos
```

### Seed de Dados de Teste
Adicione produtos de teste em `Program.cs` ap�s criar o usu�rio admin.

---

## ?? Precisa de Ajuda?

- ?? Email: suporte@ssbjr.com
- ?? Issues: [GitHub Issues](https://github.com/seu-usuario/SSBJr.TecAgro.Inventario/issues)
- ?? Discord: [Link do Discord](https://discord.gg/seu-server)

**Desenvolvido com ?? pela equipe SSB Jr.**

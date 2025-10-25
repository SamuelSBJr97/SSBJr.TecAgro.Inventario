# ?? SSBJr TecAgro Invent�rio

Sistema completo de invent�rio fiscal para agropecu�ria com suporte offline e sincroniza��o autom�tica.

## ??? Arquitetura

```
SSBJr.TecAgro.Inventario/
??? src/
?   ??? SSBJr.TecAgro.Inventario.Domain/          # Entidades e contratos
?   ??? SSBJr.TecAgro.Inventario.Infrastructure/  # Reposit�rios e servi�os
?   ??? SSBJr.TecAgro.Inventario.Server/          # API Backend (ASP.NET Core)
?   ??? SSBJr.TecAgro.Inventario.App/ # App MAUI (multiplataforma)
??? Dockerfile   # Container da API
??? docker-compose.yml    # Orquestra��o completa
??? deploy.sh        # Script de deploy (Linux/Mac)
??? deploy.ps1          # Script de deploy (Windows)
```

## ?? Tecnologias Utilizadas

### Backend
- **.NET 9** - Framework principal
- **ASP.NET Core Web API** - API REST
- **Entity Framework Core 9** - ORM
- **SQL Server** - Banco de dados
- **MediatR** - CQRS e Event Sourcing
- **Serilog** - Logging estruturado
- **Swagger/OpenAPI** - Documenta��o da API

### Mobile
- **.NET MAUI** - Framework multiplataforma (Android, iOS, Windows)
- **CommunityToolkit.Mvvm** - Padr�o MVVM
- **SQLite** - Banco de dados local
- **Material Design 3** - Design moderno

### Infraestrutura
- **Docker & Docker Compose** - Containeriza��o
- **Cloudflare R2** (opcional) - Armazenamento de arquivos
- **JWT** - Autentica��o

## ?? Pr�-requisitos

### Para Desenvolvimento
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Para Deploy em Produ��o
- Docker & Docker Compose
- SQL Server (ou usar o container fornecido)

## ? Quick Start

### 1. Clone o Reposit�rio
```bash
git clone https://github.com/seu-usuario/SSBJr.TecAgro.Inventario.git
cd SSBJr.TecAgro.Inventario
```

### 2. Configure as Vari�veis de Ambiente
```bash
cp .env.example .env
# Edite o arquivo .env conforme necess�rio
```

### 3. Deploy com Docker (Recomendado)

#### Windows (PowerShell)
```powershell
.\deploy.ps1 -Action deploy
```

#### Linux/Mac
```bash
chmod +x deploy.sh
./deploy.sh deploy
```

### 4. Acesse a Aplica��o

- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger

**Credenciais Padr�o:**
- Login: `admin`
- Senha: `admin123`

## ??? Desenvolvimento Local

### Executar o Backend (API)

```bash
cd src/SSBJr.TecAgro.Inventario.Server
dotnet run
```

### Executar o App MAUI

#### Windows
```bash
cd src/SSBJr.TecAgro.Inventario.App
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```

#### Android
```bash
cd src/SSBJr.TecAgro.Inventario.App
dotnet build -t:Run -f net9.0-android
```

#### iOS (requer Mac)
```bash
cd src/SSBJr.TecAgro.Inventario.App
dotnet build -t:Run -f net9.0-ios
```

## ?? Estrutura do Banco de Dados

### Tabela: Produtos
```sql
- Id (GUID, PK)
- Nome (string, required)
- Descricao (string)
- CodigoFiscal (string)
- SKU (string)
- Categoria (string)
- QuantidadeEstoque (decimal)
- UnidadeMedida (string)
- ValorAquisicao (decimal)
- ValorRevenda (decimal)
- Localizacao (string)
- Fotos (string[])
- DataCadastro (DateTime)
- DataAtualizacao (DateTime)
- StatusSincronizacao (enum)
- ErroSincronizacao (string, nullable)
- Ativo (bool)
```

### Tabela: Usuarios
```sql
- Id (GUID, PK)
- Login (string, unique)
- SenhaHash (string)
- Nome (string)
- Email (string)
- DataCriacao (DateTime)
- UltimoAcesso (DateTime, nullable)
- Ativo (bool)
```

### Tabela: LogsSincronizacao
```sql
- Id (GUID, PK)
- ProdutoId (GUID, FK)
- DataHora (DateTime)
- Status (enum)
- Mensagem (string)
- Detalhes (string, nullable)
```

## ?? Endpoints da API

### Autentica��o

#### POST /api/autenticacao/login
Realiza login e retorna JWT token.

**Request:**
```json
{
  "login": "admin",
  "senha": "admin123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

#### POST /api/autenticacao/validar
Valida um token JWT.

**Request:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

**Response:**
```json
{
  "valido": true
}
```

### Produtos

#### GET /api/produtos
Lista todos os produtos ativos.

#### GET /api/produtos/{id}
Busca um produto por ID.

#### GET /api/produtos/search?termo=abc
Pesquisa produtos por termo.

#### POST /api/produtos
Cria um novo produto.

**Request:**
```json
{
  "nome": "Adubo NPK 10-10-10",
  "descricao": "Fertilizante completo",
  "codigoFiscal": "1234567890",
"sku": "ADU-NPK-001",
  "categoria": "Fertilizantes",
  "quantidadeEstoque": 100.500,
  "unidadeMedida": "KG",
"valorAquisicao": 2.50,
  "valorRevenda": 3.80,
  "localizacao": "Galp�o A - Prateleira 3",
  "fotos": []
}
```

#### PUT /api/produtos/{id}
Atualiza um produto existente.

#### DELETE /api/produtos/{id}
Remove um produto (soft delete).

## ?? Comandos Docker

### Deploy Completo
```bash
# Windows
.\deploy.ps1 -Action deploy

# Linux/Mac
./deploy.sh deploy
```

### Rebuild das Imagens
```bash
# Windows
.\deploy.ps1 -Action build

# Linux/Mac
./deploy.sh build
```

### Ver Logs
```bash
# Windows
.\deploy.ps1 -Action logs

# Linux/Mac
./deploy.sh logs

# Ou diretamente:
docker-compose logs -f
```

### Parar Servi�os
```bash
# Windows
.\deploy.ps1 -Action stop

# Linux/Mac
./deploy.sh stop

# Ou diretamente:
docker-compose down
```

### Reiniciar Servi�os
```bash
# Windows
.\deploy.ps1 -Action restart

# Linux/Mac
./deploy.sh restart
```

## ?? Deploy na Cloudflare

### 1. Configurar Cloudflare R2 (Storage)

1. Acesse [Cloudflare Dashboard](https://dash.cloudflare.com/)
2. V� em **R2 Object Storage**
3. Crie um bucket chamado `inventario-storage`
4. Gere as credenciais de acesso (Access Key ID e Secret Access Key)

### 2. Configurar Vari�veis de Ambiente

Edite o arquivo `.env`:

```bash
CLOUDFLARE_ACCOUNT_ID=seu_account_id
CLOUDFLARE_ACCESS_KEY_ID=sua_access_key
CLOUDFLARE_SECRET_ACCESS_KEY=sua_secret_key
CLOUDFLARE_BUCKET_NAME=inventario-storage
CLOUDFLARE_R2_ENDPOINT=https://seu-account.r2.cloudflarestorage.com
```

### 3. Deploy da API

Voc� pode fazer deploy da API em:
- **Cloudflare Workers** (Serverless)
- **VPS com Docker** (DigitalOcean, AWS, Azure, etc.)
- **Kubernetes** (GKE, EKS, AKS)

## ?? Recursos do App MAUI

### Funcionalidades Offline-First
- ? Cadastro de produtos offline
- ? Captura de fotos via c�mera ou galeria
- ? Sincroniza��o autom�tica ao detectar internet
- ? Indicadores visuais de status de sincroniza��o
- ? Logs detalhados de erros

### Interface
- ?? Design responsivo Material Design 3
- ?? Temas claro e escuro (autom�tico)
- ?? Busca e filtros avan�ados
- ?? Suporte a m�ltiplas fotos por produto
- ?? Dashboard com estat�sticas

## ?? Seguran�a

- ? Autentica��o JWT
- ? Senhas com hash SHA256
- ? Valida��o de dados em todas as camadas
- ? CORS configur�vel
- ? HTTPS obrigat�rio em produ��o
- ? SQL Injection protection (Entity Framework)

## ?? Monitoramento e Logs

### Logs da API
Os logs ficam em:
- Console (stdout)
- Arquivo: `./logs/inventario-YYYYMMDD.txt`

### Visualizar Logs em Tempo Real
```bash
# Docker
docker-compose logs -f api

# Local
tail -f logs/inventario-*.txt
```

## ?? Testes

```bash
# Restaurar depend�ncias
dotnet restore

# Build da solu��o
dotnet build

# Executar testes (quando implementados)
dotnet test
```

## ?? Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudan�as (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

## ?? Licen�a

Este projeto est� sob a licen�a MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## ????? Autor

**SSB Jr. - Empresa J�nior de Tecnologia**

## ?? Suporte

- ?? Email: suporte@ssbjr.com
- ?? Issues: [GitHub Issues](https://github.com/seu-usuario/SSBJr.TecAgro.Inventario/issues)
- ?? Docs: [Wiki](https://github.com/seu-usuario/SSBJr.TecAgro.Inventario/wiki)

---

? **Desenvolvido com .NET 9 e ?? pela equipe SSB Jr.**

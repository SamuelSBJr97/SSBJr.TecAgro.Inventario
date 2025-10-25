# Sistema de Controle de Invent�rio TecAgro

## Vis�o Geral
Sistema completo de controle de invent�rio fiscal para agropecu�ria, com aplicativo m�vel .NET MAUI e backend API ASP.NET Core.

## Arquitetura da Solu��o

### Projetos

#### 1. SSBJr.TecAgro.Inventario.Domain
**Responsabilidade**: Camada de dom�nio com entidades e interfaces  
**Conte�do**:
- `Entities/`: Produto, Usuario, LogSincronizacao
- `Repositories/`: Interfaces IProdutoRepository, IUsuarioRepository, ILogSincronizacaoRepository
- `Services/`: IAutenticacaoService, IArmazenamentoService, ISincronizacaoService
- `Events/`: Eventos de dom�nio (ProdutoCriadoEvent, ProdutoAtualizadoEvent, etc.)

#### 2. SSBJr.TecAgro.Inventario.Infrastructure
**Responsabilidade**: Implementa��o de reposit�rios e servi�os  
**Conte�do**:
- `Data/InventarioDbContext.cs`: Contexto Entity Framework com SQL Server
- `Repositories/`: Implementa��es dos reposit�rios
- `Services/`:
  - `AutenticacaoService`: JWT, hash de senhas
  - `SincronizacaoService`: Sincroniza��o online/offline
  - `ArmazenamentoLocalService`: Gerenciamento de fotos

#### 3. SSBJr.TecAgro.Inventario.Server
**Responsabilidade**: API REST com ASP.NET Core 9.0  
**Controllers**:
- `ProdutosController`: CRUD de produtos
- `AutenticacaoController`: Login e valida��o de tokens
- `RelatoriosController`: Relat�rios e estat�sticas
- `SincronizacaoController`: Gerenciamento de sincroniza��o

**Caracter�sticas**:
- Swagger/OpenAPI
- Serilog para logging
- CORS configurado
- MediatR para eventos
- Migra��o autom�tica do banco
- Seed de usu�rio admin (login: admin, senha: admin123)

#### 4. SSBJr.TecAgro.Inventario.App
**Responsabilidade**: Aplicativo m�vel .NET MAUI  
**Plataformas**: Android, iOS, macOS Catalyst, Windows

**Servi�os**:
- `ApiService`: Comunica��o com backend
- `DatabaseService`: SQLite local
- `PreferencesService`: Prefer�ncias do usu�rio

**ViewModels**:
- `ProdutosViewModel`: Lista de produtos
- `ProdutoDetailViewModel`: Cria��o/edi��o de produtos
- `LoginViewModel`: Autentica��o
- `RelatoriosViewModel`: Relat�rios e exporta��o CSV

**Views**:
- `ProdutosPage`: Lista com pesquisa e filtros
- `ProdutoDetailPage`: Formul�rio de produto
- Navigation por abas

#### 5. Projetos de Testes
- `Domain.Tests`: 24 testes unit�rios
- `Infrastructure.Tests`: 38 testes de integra��o
- `Server.Tests`: 12 testes de controllers
- **Total**: 74 testes (100% pass rate)

## Funcionalidades Implementadas

### 1. Gest�o de Produtos
- ? Cadastro completo com:
  - Dados fiscais (C�digo Fiscal, SKU)
  - Controle de estoque (quantidade, unidade)
  - Valores (aquisi��o, revenda)
  - Localiza��o f�sica
  - Fotos (m�ltiplas)
  - Categorias predefinidas
- ? Pesquisa por nome, descri��o, c�digo, SKU, categoria
- ? Edi��o e exclus�o (soft delete)
- ? Ordena��o por data de atualiza��o

### 2. Sincroniza��o Online/Offline
- ? Detec��o autom�tica de conectividade
- ? Fila de produtos pendentes
- ? Status de sincroniza��o por produto:
  - Pendente
  - Sincronizado
  - Erro
  - Em Processamento
- ? Logs detalhados de sincroniza��o
- ? Sincroniza��o manual ou autom�tica
- ? Tratamento de erros e retry

### 3. Autentica��o e Seguran�a
- ? Login com usu�rio e senha
- ? JWT tokens
- ? Hash SHA256 para senhas
- ? Valida��o de tokens
- ? Sess�o persistente

### 4. Relat�rios e Analytics
- ? Dashboard com estat�sticas:
  - Total de produtos
- Produtos pendentes de sincroniza��o
  - Valor total do estoque
  - Quantidade total
  - Produtos com estoque baixo
  - Distribui��o por categoria
- ? Exporta��o para CSV
- ? Compartilhamento de relat�rios

### 5. Armazenamento Local
- ? SQLite para dados offline
- ? Gerenciamento de fotos locais
- ? Prefer�ncias do usu�rio
- ? Cache de dados

## Tecnologias Utilizadas

### Backend
- ASP.NET Core 9.0
- Entity Framework Core 9.0
- SQL Server
- MediatR
- Serilog
- Swagger/OpenAPI
- JWT Authentication

### Frontend MAUI
- .NET MAUI 9.0
- CommunityToolkit.MVVM
- SQLite-net-pcl
- XAML
- Binding e MVVM pattern

### Testes
- xUnit
- FluentAssertions
- Moq
- EF Core InMemory
- ASP.NET Core Testing

## Configura��o e Execu��o

### Backend (API)

```bash
cd src/SSBJr.TecAgro.Inventario.Server

# Configurar connection string em appsettings.json
# Padr�o: "Server=db;Database=InventarioDb;..."

# Executar migra��es
dotnet ef database update

# Executar servidor
dotnet run

# API dispon�vel em: http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

### App MAUI

```bash
cd src/SSBJr.TecAgro.Inventario.App

# Android
dotnet build -t:Run -f net9.0-android

# iOS (necess�rio Mac)
dotnet build -t:Run -f net9.0-ios

# Windows
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```

### Testes

```bash
# Todos os testes
dotnet test

# Por projeto
dotnet test tests/SSBJr.TecAgro.Inventario.Domain.Tests
dotnet test tests/SSBJr.TecAgro.Inventario.Infrastructure.Tests
dotnet test tests/SSBJr.TecAgro.Inventario.Server.Tests

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## Endpoints da API

### Produtos
- `GET /api/produtos` - Listar todos
- `GET /api/produtos/{id}` - Buscar por ID
- `GET /api/produtos/search?termo={termo}` - Pesquisar
- `POST /api/produtos` - Criar
- `PUT /api/produtos/{id}` - Atualizar
- `DELETE /api/produtos/{id}` - Deletar (soft)

### Autentica��o
- `POST /api/autenticacao/login` - Login
- `POST /api/autenticacao/validar` - Validar token

### Relat�rios
- `GET /api/relatorios/resumo` - Dashboard
- `GET /api/relatorios/estoque-baixo` - Produtos com estoque baixo
- `GET /api/relatorios/por-categoria` - Agrupamento por categoria

### Sincroniza��o
- `GET /api/sincronizacao/status` - Status geral
- `POST /api/sincronizacao/sincronizar-produto/{id}` - Sincronizar um produto
- `POST /api/sincronizacao/sincronizar-pendentes` - Sincronizar todos pendentes
- `GET /api/sincronizacao/logs/{produtoId}` - Logs de um produto
- `GET /api/sincronizacao/logs-recentes` - Logs recentes

## Modelo de Dados

### Produto
```csharp
- Id: Guid
- Nome: string (obrigat�rio)
- Descricao: string
- CodigoFiscal: string (�ndice)
- SKU: string (�ndice)
- Categoria: string
- QuantidadeEstoque: decimal
- UnidadeMedida: string
- ValorAquisicao: decimal
- ValorRevenda: decimal
- Localizacao: string
- Fotos: List<string>
- DataCadastro: DateTime
- DataAtualizacao: DateTime
- StatusSincronizacao: enum
- ErroSincronizacao: string?
- Ativo: bool
```

### Usuario
```csharp
- Id: Guid
- Login: string (�nico, obrigat�rio)
- SenhaHash: string (obrigat�rio)
- Nome: string (obrigat�rio)
- Email: string
- DataCriacao: DateTime
- UltimoAcesso: DateTime?
- Ativo: bool
```

### LogSincronizacao
```csharp
- Id: Guid
- ProdutoId: Guid (�ndice)
- DataHora: DateTime (�ndice)
- Status: StatusSincronizacao
- Mensagem: string?
- Detalhes: string?
```

## Padr�es de C�digo

### MVVM
- ViewModels com CommunityToolkit.MVVM
- ObservableProperty e RelayCommand
- Data binding bidirecional

### Repository Pattern
- Interfaces na camada Domain
- Implementa��es na Infrastructure
- Inje��o de depend�ncia

### Clean Architecture
- Domain: Entidades e regras de neg�cio
- Infrastructure: Detalhes de implementa��o
- App/Server: UI e API

### Event-Driven
- MediatR para eventos de dom�nio
- Handlers ass�ncronos
- Desacoplamento de funcionalidades

## Pr�ximos Passos

### Melhorias Sugeridas
- [ ] Autentica��o com refresh tokens
- [ ] Pagina��o na API
- [ ] Cache distribu�do (Redis)
- [ ] SignalR para sincroniza��o em tempo real
- [ ] Backup autom�tico
- [ ] Importa��o/exporta��o em massa
- [ ] Auditoria completa
- [ ] Relat�rios em PDF
- [ ] Gr�ficos e dashboards avan�ados
- [ ] Notifica��es push
- [ ] Suporte multi-idioma
- [ ] Temas claro/escuro

### Melhorias de Teste
- [ ] Testes E2E com Playwright
- [ ] Testes de carga com K6
- [ ] Testes de UI com Appium
- [ ] Aumento de cobertura para 90%+

## Cr�ditos
- Desenvolvido para SSBJr TecAgro
- Framework: .NET 9.0
- Padr�es: Clean Architecture, SOLID, DRY

## Licen�a
Propriedade de SSBJr TecAgro - Todos os direitos reservados

# Sistema de Controle de Inventário TecAgro

## Visão Geral
Sistema completo de controle de inventário fiscal para agropecuária, com aplicativo móvel .NET MAUI e backend API ASP.NET Core.

## Arquitetura da Solução

### Projetos

#### 1. SSBJr.TecAgro.Inventario.Domain
**Responsabilidade**: Camada de domínio com entidades e interfaces  
**Conteúdo**:
- `Entities/`: Produto, Usuario, LogSincronizacao
- `Repositories/`: Interfaces IProdutoRepository, IUsuarioRepository, ILogSincronizacaoRepository
- `Services/`: IAutenticacaoService, IArmazenamentoService, ISincronizacaoService
- `Events/`: Eventos de domínio (ProdutoCriadoEvent, ProdutoAtualizadoEvent, etc.)

#### 2. SSBJr.TecAgro.Inventario.Infrastructure
**Responsabilidade**: Implementação de repositórios e serviços  
**Conteúdo**:
- `Data/InventarioDbContext.cs`: Contexto Entity Framework com SQL Server
- `Repositories/`: Implementações dos repositórios
- `Services/`:
  - `AutenticacaoService`: JWT, hash de senhas
  - `SincronizacaoService`: Sincronização online/offline
  - `ArmazenamentoLocalService`: Gerenciamento de fotos

#### 3. SSBJr.TecAgro.Inventario.Server
**Responsabilidade**: API REST com ASP.NET Core 9.0  
**Controllers**:
- `ProdutosController`: CRUD de produtos
- `AutenticacaoController`: Login e validação de tokens
- `RelatoriosController`: Relatórios e estatísticas
- `SincronizacaoController`: Gerenciamento de sincronização

**Características**:
- Swagger/OpenAPI
- Serilog para logging
- CORS configurado
- MediatR para eventos
- Migração automática do banco
- Seed de usuário admin (login: admin, senha: admin123)

#### 4. SSBJr.TecAgro.Inventario.App
**Responsabilidade**: Aplicativo móvel .NET MAUI  
**Plataformas**: Android, iOS, macOS Catalyst, Windows

**Serviços**:
- `ApiService`: Comunicação com backend
- `DatabaseService`: SQLite local
- `PreferencesService`: Preferências do usuário

**ViewModels**:
- `ProdutosViewModel`: Lista de produtos
- `ProdutoDetailViewModel`: Criação/edição de produtos
- `LoginViewModel`: Autenticação
- `RelatoriosViewModel`: Relatórios e exportação CSV

**Views**:
- `ProdutosPage`: Lista com pesquisa e filtros
- `ProdutoDetailPage`: Formulário de produto
- Navigation por abas

#### 5. Projetos de Testes
- `Domain.Tests`: 24 testes unitários
- `Infrastructure.Tests`: 38 testes de integração
- `Server.Tests`: 12 testes de controllers
- **Total**: 74 testes (100% pass rate)

## Funcionalidades Implementadas

### 1. Gestão de Produtos
- ? Cadastro completo com:
  - Dados fiscais (Código Fiscal, SKU)
  - Controle de estoque (quantidade, unidade)
  - Valores (aquisição, revenda)
  - Localização física
  - Fotos (múltiplas)
  - Categorias predefinidas
- ? Pesquisa por nome, descrição, código, SKU, categoria
- ? Edição e exclusão (soft delete)
- ? Ordenação por data de atualização

### 2. Sincronização Online/Offline
- ? Detecção automática de conectividade
- ? Fila de produtos pendentes
- ? Status de sincronização por produto:
  - Pendente
  - Sincronizado
  - Erro
  - Em Processamento
- ? Logs detalhados de sincronização
- ? Sincronização manual ou automática
- ? Tratamento de erros e retry

### 3. Autenticação e Segurança
- ? Login com usuário e senha
- ? JWT tokens
- ? Hash SHA256 para senhas
- ? Validação de tokens
- ? Sessão persistente

### 4. Relatórios e Analytics
- ? Dashboard com estatísticas:
  - Total de produtos
- Produtos pendentes de sincronização
  - Valor total do estoque
  - Quantidade total
  - Produtos com estoque baixo
  - Distribuição por categoria
- ? Exportação para CSV
- ? Compartilhamento de relatórios

### 5. Armazenamento Local
- ? SQLite para dados offline
- ? Gerenciamento de fotos locais
- ? Preferências do usuário
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

## Configuração e Execução

### Backend (API)

```bash
cd src/SSBJr.TecAgro.Inventario.Server

# Configurar connection string em appsettings.json
# Padrão: "Server=db;Database=InventarioDb;..."

# Executar migrações
dotnet ef database update

# Executar servidor
dotnet run

# API disponível em: http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

### App MAUI

```bash
cd src/SSBJr.TecAgro.Inventario.App

# Android
dotnet build -t:Run -f net9.0-android

# iOS (necessário Mac)
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

### Autenticação
- `POST /api/autenticacao/login` - Login
- `POST /api/autenticacao/validar` - Validar token

### Relatórios
- `GET /api/relatorios/resumo` - Dashboard
- `GET /api/relatorios/estoque-baixo` - Produtos com estoque baixo
- `GET /api/relatorios/por-categoria` - Agrupamento por categoria

### Sincronização
- `GET /api/sincronizacao/status` - Status geral
- `POST /api/sincronizacao/sincronizar-produto/{id}` - Sincronizar um produto
- `POST /api/sincronizacao/sincronizar-pendentes` - Sincronizar todos pendentes
- `GET /api/sincronizacao/logs/{produtoId}` - Logs de um produto
- `GET /api/sincronizacao/logs-recentes` - Logs recentes

## Modelo de Dados

### Produto
```csharp
- Id: Guid
- Nome: string (obrigatório)
- Descricao: string
- CodigoFiscal: string (índice)
- SKU: string (índice)
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
- Login: string (único, obrigatório)
- SenhaHash: string (obrigatório)
- Nome: string (obrigatório)
- Email: string
- DataCriacao: DateTime
- UltimoAcesso: DateTime?
- Ativo: bool
```

### LogSincronizacao
```csharp
- Id: Guid
- ProdutoId: Guid (índice)
- DataHora: DateTime (índice)
- Status: StatusSincronizacao
- Mensagem: string?
- Detalhes: string?
```

## Padrões de Código

### MVVM
- ViewModels com CommunityToolkit.MVVM
- ObservableProperty e RelayCommand
- Data binding bidirecional

### Repository Pattern
- Interfaces na camada Domain
- Implementações na Infrastructure
- Injeção de dependência

### Clean Architecture
- Domain: Entidades e regras de negócio
- Infrastructure: Detalhes de implementação
- App/Server: UI e API

### Event-Driven
- MediatR para eventos de domínio
- Handlers assíncronos
- Desacoplamento de funcionalidades

## Próximos Passos

### Melhorias Sugeridas
- [ ] Autenticação com refresh tokens
- [ ] Paginação na API
- [ ] Cache distribuído (Redis)
- [ ] SignalR para sincronização em tempo real
- [ ] Backup automático
- [ ] Importação/exportação em massa
- [ ] Auditoria completa
- [ ] Relatórios em PDF
- [ ] Gráficos e dashboards avançados
- [ ] Notificações push
- [ ] Suporte multi-idioma
- [ ] Temas claro/escuro

### Melhorias de Teste
- [ ] Testes E2E com Playwright
- [ ] Testes de carga com K6
- [ ] Testes de UI com Appium
- [ ] Aumento de cobertura para 90%+

## Créditos
- Desenvolvido para SSBJr TecAgro
- Framework: .NET 9.0
- Padrões: Clean Architecture, SOLID, DRY

## Licença
Propriedade de SSBJr TecAgro - Todos os direitos reservados

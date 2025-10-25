# Projetos de Testes - SSBJr.TecAgro.Inventario

Este documento descreve os projetos de testes criados para a solução de Inventário TecAgro.

## Estrutura dos Projetos de Teste

A solução contém três projetos de testes, organizados por camada:

### 1. SSBJr.TecAgro.Inventario.Domain.Tests
Testes unitários da camada de domínio.

**Cobertura:**
- `Entities/ProdutoTests.cs` - Testes da entidade Produto
- `Entities/UsuarioTests.cs` - Testes da entidade Usuario
- `Entities/LogSincronizacaoTests.cs` - Testes da entidade LogSincronizacao
- `Events/ProdutoEventsTests.cs` - Testes dos eventos de domínio

**Tecnologias:**
- xUnit
- FluentAssertions
- Moq

### 2. SSBJr.TecAgro.Inventario.Infrastructure.Tests
Testes de integração da camada de infraestrutura.

**Cobertura:**
- `Data/InventarioDbContextTests.cs` - Testes do contexto do Entity Framework
- `Repositories/ProdutoRepositoryTests.cs` - Testes do repositório de produtos
- `Repositories/UsuarioRepositoryTests.cs` - Testes do repositório de usuários
- `Repositories/LogSincronizacaoRepositoryTests.cs` - Testes do repositório de logs
- `Services/AutenticacaoServiceTests.cs` - Testes do serviço de autenticação

**Tecnologias:**
- xUnit
- FluentAssertions
- Moq
- Entity Framework Core InMemory

### 3. SSBJr.TecAgro.Inventario.Server.Tests
Testes da camada de API (Controllers).

**Cobertura:**
- `Controllers/ProdutosControllerTests.cs` - Testes do controller de produtos

**Tecnologias:**
- xUnit
- FluentAssertions
- Moq
- ASP.NET Core Testing

## Executar os Testes

### Executar todos os testes
```bash
dotnet test
```

### Executar testes de um projeto específico
```bash
dotnet test tests/SSBJr.TecAgro.Inventario.Domain.Tests
dotnet test tests/SSBJr.TecAgro.Inventario.Infrastructure.Tests
dotnet test tests/SSBJr.TecAgro.Inventario.Server.Tests
```

### Executar com cobertura de código
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Executar com verbosidade detalhada
```bash
dotnet test --verbosity detailed
```

## Padrões de Teste Utilizados

### Arrange-Act-Assert (AAA)
Todos os testes seguem o padrão AAA:
- **Arrange**: Configuração dos dados e dependências
- **Act**: Execução do método testado
- **Assert**: Verificação dos resultados

### Nomenclatura
Os testes seguem a convenção:
```
[MetodoTestado]_[Cenario]_[ResultadoEsperado]
```

Exemplo: `GetById_DeveRetornarOkComProdutoQuandoExistir`

## Mocks e Stubs

Os testes utilizam o framework Moq para criar mocks de dependências:
- Repositórios
- Serviços
- Loggers
- Mediator (MediatR)

## Banco de Dados em Memória

Os testes de infraestrutura utilizam Entity Framework Core InMemory para simular o banco de dados:
- Cada teste cria uma instância isolada do banco
- Cleanup automático após cada teste via `IDisposable`
- Não requer configuração de banco de dados real

## Asserções

Utilizamos FluentAssertions para asserções mais legíveis:

```csharp
// Ao invés de:
Assert.Equal(expected, actual);

// Usamos:
actual.Should().Be(expected);
```

## Cobertura de Cenários

### Cenários de Sucesso
- ? Operações CRUD bem-sucedidas
- ? Autenticação com credenciais válidas
- ? Busca e filtros retornando dados

### Cenários de Erro
- ? Entidades não encontradas (NotFound)
- ? Validações de entrada (BadRequest)
- ? Autenticação com credenciais inválidas
- ? Exceções do banco de dados

### Cenários de Validação
- ? Validação de IDs
- ? Validação de campos obrigatórios
- ? Validação de formatos

## Melhores Práticas Implementadas

1. **Isolamento**: Cada teste é independente
2. **Nomenclatura Clara**: Testes auto-documentados
3. **Single Responsibility**: Um teste verifica um comportamento
4. **Determinístico**: Testes produzem o mesmo resultado sempre
5. **Rápidos**: Uso de mocks e banco em memória
6. **Manuteníveis**: Código limpo e organizado

## Próximos Passos

Áreas que podem ser expandidas:
- [ ] Testes de integração end-to-end
- [ ] Testes de performance
- [ ] Testes de carga
- [ ] Testes do aplicativo MAUI
- [ ] Testes de sincronização
- [ ] Relatórios de cobertura de código

## Dependências

Todos os projetos de teste incluem:
- `Microsoft.NET.Test.Sdk` (17.12.0)
- `xunit` (2.9.2)
- `xunit.runner.visualstudio` (2.8.2)
- `FluentAssertions` (7.0.0)
- `Moq` (4.20.72)
- `coverlet.collector` (6.0.2)

Dependências específicas:
- Infrastructure.Tests: `Microsoft.EntityFrameworkCore.InMemory` (9.0.10)
- Server.Tests: `Microsoft.AspNetCore.Mvc.Testing` (9.0.10)

## Contribuindo

Ao adicionar novos testes:
1. Siga o padrão AAA
2. Use nomenclatura descritiva
3. Adicione testes para casos de sucesso e erro
4. Mantenha os testes independentes
5. Documente cenários complexos com comentários

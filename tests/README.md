# Projetos de Testes - SSBJr.TecAgro.Inventario

Este documento descreve os projetos de testes criados para a solu��o de Invent�rio TecAgro.

## Estrutura dos Projetos de Teste

A solu��o cont�m tr�s projetos de testes, organizados por camada:

### 1. SSBJr.TecAgro.Inventario.Domain.Tests
Testes unit�rios da camada de dom�nio.

**Cobertura:**
- `Entities/ProdutoTests.cs` - Testes da entidade Produto
- `Entities/UsuarioTests.cs` - Testes da entidade Usuario
- `Entities/LogSincronizacaoTests.cs` - Testes da entidade LogSincronizacao
- `Events/ProdutoEventsTests.cs` - Testes dos eventos de dom�nio

**Tecnologias:**
- xUnit
- FluentAssertions
- Moq

### 2. SSBJr.TecAgro.Inventario.Infrastructure.Tests
Testes de integra��o da camada de infraestrutura.

**Cobertura:**
- `Data/InventarioDbContextTests.cs` - Testes do contexto do Entity Framework
- `Repositories/ProdutoRepositoryTests.cs` - Testes do reposit�rio de produtos
- `Repositories/UsuarioRepositoryTests.cs` - Testes do reposit�rio de usu�rios
- `Repositories/LogSincronizacaoRepositoryTests.cs` - Testes do reposit�rio de logs
- `Services/AutenticacaoServiceTests.cs` - Testes do servi�o de autentica��o

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

### Executar testes de um projeto espec�fico
```bash
dotnet test tests/SSBJr.TecAgro.Inventario.Domain.Tests
dotnet test tests/SSBJr.TecAgro.Inventario.Infrastructure.Tests
dotnet test tests/SSBJr.TecAgro.Inventario.Server.Tests
```

### Executar com cobertura de c�digo
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Executar com verbosidade detalhada
```bash
dotnet test --verbosity detailed
```

## Padr�es de Teste Utilizados

### Arrange-Act-Assert (AAA)
Todos os testes seguem o padr�o AAA:
- **Arrange**: Configura��o dos dados e depend�ncias
- **Act**: Execu��o do m�todo testado
- **Assert**: Verifica��o dos resultados

### Nomenclatura
Os testes seguem a conven��o:
```
[MetodoTestado]_[Cenario]_[ResultadoEsperado]
```

Exemplo: `GetById_DeveRetornarOkComProdutoQuandoExistir`

## Mocks e Stubs

Os testes utilizam o framework Moq para criar mocks de depend�ncias:
- Reposit�rios
- Servi�os
- Loggers
- Mediator (MediatR)

## Banco de Dados em Mem�ria

Os testes de infraestrutura utilizam Entity Framework Core InMemory para simular o banco de dados:
- Cada teste cria uma inst�ncia isolada do banco
- Cleanup autom�tico ap�s cada teste via `IDisposable`
- N�o requer configura��o de banco de dados real

## Asser��es

Utilizamos FluentAssertions para asser��es mais leg�veis:

```csharp
// Ao inv�s de:
Assert.Equal(expected, actual);

// Usamos:
actual.Should().Be(expected);
```

## Cobertura de Cen�rios

### Cen�rios de Sucesso
- ? Opera��es CRUD bem-sucedidas
- ? Autentica��o com credenciais v�lidas
- ? Busca e filtros retornando dados

### Cen�rios de Erro
- ? Entidades n�o encontradas (NotFound)
- ? Valida��es de entrada (BadRequest)
- ? Autentica��o com credenciais inv�lidas
- ? Exce��es do banco de dados

### Cen�rios de Valida��o
- ? Valida��o de IDs
- ? Valida��o de campos obrigat�rios
- ? Valida��o de formatos

## Melhores Pr�ticas Implementadas

1. **Isolamento**: Cada teste � independente
2. **Nomenclatura Clara**: Testes auto-documentados
3. **Single Responsibility**: Um teste verifica um comportamento
4. **Determin�stico**: Testes produzem o mesmo resultado sempre
5. **R�pidos**: Uso de mocks e banco em mem�ria
6. **Manuten�veis**: C�digo limpo e organizado

## Pr�ximos Passos

�reas que podem ser expandidas:
- [ ] Testes de integra��o end-to-end
- [ ] Testes de performance
- [ ] Testes de carga
- [ ] Testes do aplicativo MAUI
- [ ] Testes de sincroniza��o
- [ ] Relat�rios de cobertura de c�digo

## Depend�ncias

Todos os projetos de teste incluem:
- `Microsoft.NET.Test.Sdk` (17.12.0)
- `xunit` (2.9.2)
- `xunit.runner.visualstudio` (2.8.2)
- `FluentAssertions` (7.0.0)
- `Moq` (4.20.72)
- `coverlet.collector` (6.0.2)

Depend�ncias espec�ficas:
- Infrastructure.Tests: `Microsoft.EntityFrameworkCore.InMemory` (9.0.10)
- Server.Tests: `Microsoft.AspNetCore.Mvc.Testing` (9.0.10)

## Contribuindo

Ao adicionar novos testes:
1. Siga o padr�o AAA
2. Use nomenclatura descritiva
3. Adicione testes para casos de sucesso e erro
4. Mantenha os testes independentes
5. Documente cen�rios complexos com coment�rios

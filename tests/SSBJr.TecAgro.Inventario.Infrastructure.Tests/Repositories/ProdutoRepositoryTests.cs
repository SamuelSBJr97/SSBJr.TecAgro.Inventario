using Microsoft.EntityFrameworkCore;
using SSBJr.TecAgro.Inventario.Domain.Entities;
using SSBJr.TecAgro.Inventario.Infrastructure.Data;
using SSBJr.TecAgro.Inventario.Infrastructure.Repositories;

namespace SSBJr.TecAgro.Inventario.Infrastructure.Tests.Repositories;

public class ProdutoRepositoryTests : IDisposable
{
    private readonly InventarioDbContext _context;
    private readonly ProdutoRepository _repository;

    public ProdutoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<InventarioDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new InventarioDbContext(options);
        _repository = new ProdutoRepository(_context);
    }

    [Fact]
    public async Task AddAsync_DeveAdicionarProdutoComSucesso()
    {
   // Arrange
        var produto = new Produto
        {
            Id = Guid.NewGuid(),
      Nome = "Produto Teste",
            Descricao = "Descrição Teste",
      CodigoFiscal = "CF123",
            SKU = "SKU123",
      Categoria = "Categoria A",
            QuantidadeEstoque = 10,
 UnidadeMedida = "UN",
            ValorAquisicao = 50,
          ValorRevenda = 75,
       Localizacao = "Prateleira 1",
     DataCadastro = DateTime.UtcNow,
   DataAtualizacao = DateTime.UtcNow,
        StatusSincronizacao = StatusSincronizacao.Sincronizado,
            Ativo = true
        };

   // Act
   await _repository.AddAsync(produto);
      var produtoSalvo = await _repository.GetByIdAsync(produto.Id);

        // Assert
        produtoSalvo.Should().NotBeNull();
        produtoSalvo!.Nome.Should().Be("Produto Teste");
        produtoSalvo.CodigoFiscal.Should().Be("CF123");
    }

 [Fact]
    public async Task GetByIdAsync_DeveRetornarProdutoQuandoExistir()
    {
        // Arrange
        var produto = new Produto
      {
        Id = Guid.NewGuid(),
  Nome = "Produto Teste",
 Ativo = true
        };
        await _repository.AddAsync(produto);

        // Act
        var resultado = await _repository.GetByIdAsync(produto.Id);

        // Assert
    resultado.Should().NotBeNull();
  resultado!.Id.Should().Be(produto.Id);
        resultado.Nome.Should().Be("Produto Teste");
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarNuloQuandoNaoExistir()
    {
        // Arrange
    var idInexistente = Guid.NewGuid();

    // Act
        var resultado = await _repository.GetByIdAsync(idInexistente);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarTodosProdutos()
    {
  // Arrange
        var produtos = new[]
        {
     new Produto { Id = Guid.NewGuid(), Nome = "Produto 1", Ativo = true },
            new Produto { Id = Guid.NewGuid(), Nome = "Produto 2", Ativo = true },
      new Produto { Id = Guid.NewGuid(), Nome = "Produto 3", Ativo = true }
        };

        foreach (var produto in produtos)
        {
await _repository.AddAsync(produto);
        }

     // Act
        var resultado = await _repository.GetAllAsync();

    // Assert
   resultado.Should().HaveCount(3);
        resultado.Select(p => p.Nome).Should().Contain(new[] { "Produto 1", "Produto 2", "Produto 3" });
    }

    [Fact]
    public async Task UpdateAsync_DeveAtualizarProdutoComSucesso()
    {
        // Arrange
        var produto = new Produto
        {
            Id = Guid.NewGuid(),
            Nome = "Produto Original",
    Ativo = true
        };
        await _repository.AddAsync(produto);

    // Act
        produto.Nome = "Produto Atualizado";
        await _repository.UpdateAsync(produto);
        var produtoAtualizado = await _repository.GetByIdAsync(produto.Id);

     // Assert
        produtoAtualizado.Should().NotBeNull();
 produtoAtualizado!.Nome.Should().Be("Produto Atualizado");
    }

    [Fact]
    public async Task DeleteAsync_DeveRemoverProdutoComSucesso()
    {
        // Arrange
   var produto = new Produto
        {
  Id = Guid.NewGuid(),
     Nome = "Produto para Deletar",
    Ativo = true
        };
   await _repository.AddAsync(produto);

      // Act
        await _repository.DeleteAsync(produto.Id);
        var produtoDeletado = await _repository.GetByIdAsync(produto.Id);

        // Assert
        // O DeleteAsync faz soft delete, então o produto ainda existe mas com Ativo = false
        produtoDeletado.Should().NotBeNull();
   produtoDeletado!.Ativo.Should().BeFalse();
  }

    [Fact]
  public async Task GetPendentesAsync_DeveRetornarApenasProdutosPendentes()
    {
        // Arrange
  var produtos = new[]
        {
  new Produto { Id = Guid.NewGuid(), Nome = "Produto Pendente", StatusSincronizacao = StatusSincronizacao.Pendente, Ativo = true },
          new Produto { Id = Guid.NewGuid(), Nome = "Produto Sincronizado", StatusSincronizacao = StatusSincronizacao.Sincronizado, Ativo = true },
            new Produto { Id = Guid.NewGuid(), Nome = "Produto com Erro", StatusSincronizacao = StatusSincronizacao.Erro, Ativo = true }
        };

        foreach (var produto in produtos)
        {
         await _repository.AddAsync(produto);
        }

        // Act
   var resultado = await _repository.GetPendentesAsync();

 // Assert
        resultado.Should().HaveCount(1);
        resultado.First().Nome.Should().Be("Produto Pendente");
    }

    [Fact]
    public async Task SearchAsync_DeveRetornarProdutosPorNome()
    {
     // Arrange
        var produtos = new[]
        {
     new Produto { Id = Guid.NewGuid(), Nome = "Notebook Dell", Ativo = true },
     new Produto { Id = Guid.NewGuid(), Nome = "Mouse Microsoft", Ativo = true },
            new Produto { Id = Guid.NewGuid(), Nome = "Teclado Dell", Ativo = true }
    };

        foreach (var produto in produtos)
        {
       await _repository.AddAsync(produto);
        }

        // Act
    var resultado = await _repository.SearchAsync("Dell");

 // Assert
        resultado.Should().HaveCount(2);
        resultado.All(p => p.Nome.Contains("Dell")).Should().BeTrue();
    }

    [Fact]
    public async Task CountAsync_DeveRetornarQuantidadeCorretaDeProdutos()
    {
        // Arrange
   var produtos = new[]
        {
       new Produto { Id = Guid.NewGuid(), Nome = "Produto 1", Ativo = true },
       new Produto { Id = Guid.NewGuid(), Nome = "Produto 2", Ativo = true },
  new Produto { Id = Guid.NewGuid(), Nome = "Produto 3", Ativo = true }
        };

        foreach (var produto in produtos)
        {
await _repository.AddAsync(produto);
        }

 // Act
        var count = await _repository.CountAsync();

        // Assert
        count.Should().Be(3);
    }

    public void Dispose()
    {
 _context.Database.EnsureDeleted();
        _context.Dispose();
     GC.SuppressFinalize(this);
  }
}

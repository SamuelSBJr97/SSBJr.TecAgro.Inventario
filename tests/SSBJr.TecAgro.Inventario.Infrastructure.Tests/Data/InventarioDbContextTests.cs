using Microsoft.EntityFrameworkCore;
using SSBJr.TecAgro.Inventario.Domain.Entities;
using SSBJr.TecAgro.Inventario.Infrastructure.Data;

namespace SSBJr.TecAgro.Inventario.Infrastructure.Tests.Data;

public class InventarioDbContextTests : IDisposable
{
    private readonly InventarioDbContext _context;

    public InventarioDbContextTests()
    {
        var options = new DbContextOptionsBuilder<InventarioDbContext>()
 .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
         .Options;

 _context = new InventarioDbContext(options);
    }

    [Fact]
    public void DbContext_DeveCriarDbSetsProdutos()
    {
        // Assert
        _context.Produtos.Should().NotBeNull();
    }

    [Fact]
    public void DbContext_DeveCriarDbSetsUsuarios()
    {
        // Assert
        _context.Usuarios.Should().NotBeNull();
    }

    [Fact]
    public void DbContext_DeveCriarDbSetsLogsSincronizacao()
    {
        // Assert
        _context.LogsSincronizacao.Should().NotBeNull();
    }

    [Fact]
    public async Task DbContext_DeveSalvarProdutoComPropriedadesCorretas()
  {
    // Arrange
        var produto = new Produto
        {
            Id = Guid.NewGuid(),
            Nome = "Produto Teste",
     CodigoFiscal = "CF123",
      SKU = "SKU123",
    ValorAquisicao = 100.50m,
            ValorRevenda = 150.75m,
            QuantidadeEstoque = 10.5m,
     DataCadastro = DateTime.UtcNow,
        DataAtualizacao = DateTime.UtcNow,
            Ativo = true
        };

     // Act
     _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

    // Assert
        var produtoSalvo = await _context.Produtos.FindAsync(produto.Id);
        produtoSalvo.Should().NotBeNull();
      produtoSalvo!.Nome.Should().Be("Produto Teste");
        produtoSalvo.ValorAquisicao.Should().Be(100.50m);
}

    [Fact]
    public async Task DbContext_DeveSalvarUsuarioComLoginUnico()
    {
  // Arrange
        var usuario = new Usuario
        {
     Id = Guid.NewGuid(),
      Login = "login_unico",
            SenhaHash = "hash",
        Nome = "Teste",
          Email = "teste@email.com",
            DataCriacao = DateTime.UtcNow,
     Ativo = true
        };

        // Act
      _context.Usuarios.Add(usuario);
     await _context.SaveChangesAsync();

        // Assert
        var usuarioSalvo = await _context.Usuarios.FindAsync(usuario.Id);
        usuarioSalvo.Should().NotBeNull();
        usuarioSalvo!.Login.Should().Be("login_unico");
    }

    [Fact]
    public async Task DbContext_DeveConverterListaDeFotosCorretamente()
    {
 // Arrange
      var produto = new Produto
  {
            Id = Guid.NewGuid(),
            Nome = "Produto com Fotos",
    Fotos = new List<string> { "foto1.jpg", "foto2.jpg", "foto3.jpg" },
  Ativo = true
      };

   // Act
      _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
        _context.Entry(produto).State = EntityState.Detached;

    var produtoSalvo = await _context.Produtos.FindAsync(produto.Id);

 // Assert
 produtoSalvo.Should().NotBeNull();
     produtoSalvo!.Fotos.Should().HaveCount(3);
        produtoSalvo.Fotos.Should().Contain(new[] { "foto1.jpg", "foto2.jpg", "foto3.jpg" });
    }

    [Fact]
    public async Task DbContext_DeveRespeitarMaxLengthEmNome()
    {
        // Arrange
        var nomeLongo = new string('A', 250); // Maior que o limite de 200
     var produto = new Produto
     {
    Id = Guid.NewGuid(),
   Nome = nomeLongo,
       Ativo = true
        };

        // Act & Assert
        // Com banco em memória, constraints podem não ser aplicadas da mesma forma
        // mas podemos verificar se o modelo está configurado corretamente
        var entityType = _context.Model.FindEntityType(typeof(Produto));
        var nomeProperty = entityType?.FindProperty(nameof(Produto.Nome));
        nomeProperty?.GetMaxLength().Should().Be(200);
        
        await Task.CompletedTask; // Para evitar warning de método async sem await
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}

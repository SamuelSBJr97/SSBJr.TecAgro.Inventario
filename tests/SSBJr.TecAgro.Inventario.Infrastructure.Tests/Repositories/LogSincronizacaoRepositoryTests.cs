using Microsoft.EntityFrameworkCore;
using SSBJr.TecAgro.Inventario.Domain.Entities;
using SSBJr.TecAgro.Inventario.Infrastructure.Data;
using SSBJr.TecAgro.Inventario.Infrastructure.Repositories;

namespace SSBJr.TecAgro.Inventario.Infrastructure.Tests.Repositories;

public class LogSincronizacaoRepositoryTests : IDisposable
{
  private readonly InventarioDbContext _context;
    private readonly LogSincronizacaoRepository _repository;

    public LogSincronizacaoRepositoryTests()
    {
     var options = new DbContextOptionsBuilder<InventarioDbContext>()
   .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .Options;

  _context = new InventarioDbContext(options);
      _repository = new LogSincronizacaoRepository(_context);
    }

 [Fact]
    public async Task AddAsync_DeveAdicionarLogComSucesso()
  {
        // Arrange
     var log = new LogSincronizacao
        {
         Id = Guid.NewGuid(),
          ProdutoId = Guid.NewGuid(),
     DataHora = DateTime.UtcNow,
    Status = StatusSincronizacao.Sincronizado,
          Mensagem = "Sincronização bem-sucedida",
            Detalhes = "Detalhes do processo"
      };

// Act
   await _repository.AddAsync(log);
        var logs = await _repository.GetByProdutoIdAsync(log.ProdutoId);

        // Assert
        logs.Should().ContainSingle();
     logs.First().Mensagem.Should().Be("Sincronização bem-sucedida");
    }

    [Fact]
public async Task GetByProdutoIdAsync_DeveRetornarLogsDoProduto()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var logs = new[]
        {
     new LogSincronizacao { Id = Guid.NewGuid(), ProdutoId = produtoId, DataHora = DateTime.UtcNow.AddHours(-2), Status = StatusSincronizacao.Pendente },
   new LogSincronizacao { Id = Guid.NewGuid(), ProdutoId = produtoId, DataHora = DateTime.UtcNow.AddHours(-1), Status = StatusSincronizacao.Sincronizado },
   new LogSincronizacao { Id = Guid.NewGuid(), ProdutoId = Guid.NewGuid(), DataHora = DateTime.UtcNow, Status = StatusSincronizacao.Erro }
        };

    foreach (var log in logs)
   {
    await _repository.AddAsync(log);
     }

     // Act
        var resultado = await _repository.GetByProdutoIdAsync(produtoId);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.All(l => l.ProdutoId == produtoId).Should().BeTrue();
    }

    [Fact]
    public async Task GetRecentesAsync_DeveRetornarLogsRecentes()
    {
  // Arrange
        for (int i = 0; i < 150; i++)
  {
       var log = new LogSincronizacao
          {
     Id = Guid.NewGuid(),
 ProdutoId = Guid.NewGuid(),
      DataHora = DateTime.UtcNow.AddMinutes(-i),
Status = StatusSincronizacao.Sincronizado,
      Mensagem = $"Log {i}"
   };
    await _repository.AddAsync(log);
      }

  // Act
        var resultado = await _repository.GetRecentesAsync(100);

   // Assert
      resultado.Should().HaveCount(100);
   // Verifica se estão ordenados por data (mais recentes primeiro)
        var dataHoras = resultado.Select(l => l.DataHora).ToList();
   dataHoras.Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task GetRecentesAsync_DeveUsarQuantidadePadrao()
    {
 // Arrange
    for (int i = 0; i < 50; i++)
        {
       var log = new LogSincronizacao
     {
             Id = Guid.NewGuid(),
 ProdutoId = Guid.NewGuid(),
      DataHora = DateTime.UtcNow.AddMinutes(-i),
    Status = StatusSincronizacao.Sincronizado
};
     await _repository.AddAsync(log);
        }

   // Act
   var resultado = await _repository.GetRecentesAsync();

        // Assert
        resultado.Should().HaveCount(50); // Menos que o padrão de 100
    }

    [Fact]
    public async Task GetByProdutoIdAsync_DeveRetornarVazioQuandoNaoHouverLogs()
    {
        // Arrange
   var produtoId = Guid.NewGuid();

        // Act
     var resultado = await _repository.GetByProdutoIdAsync(produtoId);

   // Assert
        resultado.Should().BeEmpty();
  }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}

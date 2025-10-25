using SSBJr.TecAgro.Inventario.Domain.Events;

namespace SSBJr.TecAgro.Inventario.Domain.Tests.Events;

public class ProdutoEventsTests
{
    [Fact]
    public void ProdutoCriadoEvent_DeveCriarEventoCorretamente()
    {
 // Arrange
        var produtoId = Guid.NewGuid();
        var nome = "Produto Teste";
   var dataCriacao = DateTime.UtcNow;

        // Act
        var evento = new ProdutoCriadoEvent(produtoId, nome, dataCriacao);

        // Assert
        evento.ProdutoId.Should().Be(produtoId);
      evento.Nome.Should().Be(nome);
  evento.DataCriacao.Should().Be(dataCriacao);
    }

    [Fact]
    public void ProdutoAtualizadoEvent_DeveCriarEventoCorretamente()
    {
   // Arrange
        var produtoId = Guid.NewGuid();
      var dataAtualizacao = DateTime.UtcNow;

        // Act
        var evento = new ProdutoAtualizadoEvent(produtoId, dataAtualizacao);

   // Assert
        evento.ProdutoId.Should().Be(produtoId);
        evento.DataAtualizacao.Should().Be(dataAtualizacao);
    }

    [Fact]
    public void ProdutoSincronizadoEvent_DeveCriarEventoCorretamente()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var dataSincronizacao = DateTime.UtcNow;

        // Act
    var evento = new ProdutoSincronizadoEvent(produtoId, dataSincronizacao);

  // Assert
        evento.ProdutoId.Should().Be(produtoId);
        evento.DataSincronizacao.Should().Be(dataSincronizacao);
    }

    [Fact]
    public void ErroSincronizacaoEvent_DeveCriarEventoCorretamente()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var mensagem = "Erro ao sincronizar";
  var dataErro = DateTime.UtcNow;

    // Act
   var evento = new ErroSincronizacaoEvent(produtoId, mensagem, dataErro);

        // Assert
        evento.ProdutoId.Should().Be(produtoId);
        evento.Mensagem.Should().Be(mensagem);
        evento.DataErro.Should().Be(dataErro);
    }
}

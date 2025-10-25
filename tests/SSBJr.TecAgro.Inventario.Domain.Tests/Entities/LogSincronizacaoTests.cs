using SSBJr.TecAgro.Inventario.Domain.Entities;

namespace SSBJr.TecAgro.Inventario.Domain.Tests.Entities;

public class LogSincronizacaoTests
{
    [Fact]
    public void LogSincronizacao_DeveTerPropriedadesInicializadasCorretamente()
    {
        // Arrange & Act
        var log = new LogSincronizacao();

        // Assert
   log.Id.Should().Be(Guid.Empty);
        log.ProdutoId.Should().Be(Guid.Empty);
        log.DataHora.Should().Be(DateTime.MinValue);
    log.Status.Should().Be(StatusSincronizacao.Pendente);
     log.Mensagem.Should().BeNull();
  log.Detalhes.Should().BeNull();
 }

    [Fact]
    public void LogSincronizacao_DevePermitirAtribuicaoDePropriedades()
    {
        // Arrange
        var id = Guid.NewGuid();
        var produtoId = Guid.NewGuid();
   var dataHora = DateTime.UtcNow;

    // Act
        var log = new LogSincronizacao
     {
       Id = id,
  ProdutoId = produtoId,
    DataHora = dataHora,
            Status = StatusSincronizacao.Sincronizado,
      Mensagem = "Sincronização realizada com sucesso",
  Detalhes = "Detalhes da sincronização"
        };

        // Assert
        log.Id.Should().Be(id);
   log.ProdutoId.Should().Be(produtoId);
        log.DataHora.Should().Be(dataHora);
  log.Status.Should().Be(StatusSincronizacao.Sincronizado);
        log.Mensagem.Should().Be("Sincronização realizada com sucesso");
        log.Detalhes.Should().Be("Detalhes da sincronização");
    }

    [Theory]
    [InlineData(StatusSincronizacao.Pendente)]
    [InlineData(StatusSincronizacao.Sincronizado)]
    [InlineData(StatusSincronizacao.Erro)]
    [InlineData(StatusSincronizacao.EmProcessamento)]
    public void LogSincronizacao_DeveAceitarTodosStatusSincronizacao(StatusSincronizacao status)
    {
        // Arrange & Act
        var log = new LogSincronizacao { Status = status };

        // Assert
        log.Status.Should().Be(status);
    }
}

using SSBJr.TecAgro.Inventario.Domain.Entities;

namespace SSBJr.TecAgro.Inventario.Domain.Tests.Entities;

public class ProdutoTests
{
  [Fact]
    public void Produto_DeveTerPropriedadesInicializadasCorretamente()
    {
      // Arrange & Act
        var produto = new Produto();

     // Assert
        produto.Id.Should().Be(Guid.Empty);
        produto.Nome.Should().BeEmpty();
      produto.Descricao.Should().BeEmpty();
        produto.CodigoFiscal.Should().BeEmpty();
    produto.SKU.Should().BeEmpty();
        produto.Categoria.Should().BeEmpty();
   produto.QuantidadeEstoque.Should().Be(0);
 produto.UnidadeMedida.Should().BeEmpty();
        produto.ValorAquisicao.Should().Be(0);
        produto.ValorRevenda.Should().Be(0);
        produto.Localizacao.Should().BeEmpty();
    produto.Fotos.Should().NotBeNull().And.BeEmpty();
      produto.StatusSincronizacao.Should().Be(StatusSincronizacao.Pendente);
  produto.ErroSincronizacao.Should().BeNull();
        produto.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Produto_DevePermitirAtribuicaoDePropriedades()
    {
// Arrange
        var id = Guid.NewGuid();
   var dataCadastro = DateTime.UtcNow;
        var dataAtualizacao = DateTime.UtcNow;

        // Act
        var produto = new Produto
 {
      Id = id,
   Nome = "Produto Teste",
            Descricao = "Descrição do produto",
          CodigoFiscal = "CF123",
     SKU = "SKU123",
   Categoria = "Categoria A",
            QuantidadeEstoque = 100.5m,
            UnidadeMedida = "UN",
     ValorAquisicao = 50.00m,
        ValorRevenda = 75.00m,
         Localizacao = "Prateleira 1",
            Fotos = new List<string> { "foto1.jpg", "foto2.jpg" },
            DataCadastro = dataCadastro,
 DataAtualizacao = dataAtualizacao,
       StatusSincronizacao = StatusSincronizacao.Sincronizado,
       Ativo = true
      };

        // Assert
        produto.Id.Should().Be(id);
        produto.Nome.Should().Be("Produto Teste");
        produto.Descricao.Should().Be("Descrição do produto");
   produto.CodigoFiscal.Should().Be("CF123");
        produto.SKU.Should().Be("SKU123");
 produto.Categoria.Should().Be("Categoria A");
  produto.QuantidadeEstoque.Should().Be(100.5m);
        produto.UnidadeMedida.Should().Be("UN");
        produto.ValorAquisicao.Should().Be(50.00m);
      produto.ValorRevenda.Should().Be(75.00m);
        produto.Localizacao.Should().Be("Prateleira 1");
        produto.Fotos.Should().HaveCount(2);
        produto.DataCadastro.Should().Be(dataCadastro);
   produto.DataAtualizacao.Should().Be(dataAtualizacao);
        produto.StatusSincronizacao.Should().Be(StatusSincronizacao.Sincronizado);
        produto.Ativo.Should().BeTrue();
    }

    [Theory]
    [InlineData(StatusSincronizacao.Pendente)]
    [InlineData(StatusSincronizacao.Sincronizado)]
    [InlineData(StatusSincronizacao.Erro)]
    [InlineData(StatusSincronizacao.EmProcessamento)]
    public void Produto_DeveAceitarTodosStatusSincronizacao(StatusSincronizacao status)
    {
        // Arrange & Act
        var produto = new Produto { StatusSincronizacao = status };

        // Assert
        produto.StatusSincronizacao.Should().Be(status);
    }

    [Fact]
    public void Produto_DevePermitirListaDeFotosVazia()
    {
    // Arrange & Act
    var produto = new Produto { Fotos = new List<string>() };

        // Assert
      produto.Fotos.Should().NotBeNull().And.BeEmpty();
 }

    [Fact]
    public void Produto_DevePermitirErroSincronizacaoNulo()
    {
        // Arrange & Act
 var produto = new Produto { ErroSincronizacao = null };

 // Assert
        produto.ErroSincronizacao.Should().BeNull();
    }

    [Fact]
    public void Produto_DevePermitirErroSincronizacaoComMensagem()
    {
        // Arrange
  var mensagemErro = "Erro ao sincronizar produto";

      // Act
 var produto = new Produto { ErroSincronizacao = mensagemErro };

        // Assert
        produto.ErroSincronizacao.Should().Be(mensagemErro);
    }
}

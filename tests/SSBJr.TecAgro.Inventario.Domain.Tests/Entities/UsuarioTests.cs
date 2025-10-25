using SSBJr.TecAgro.Inventario.Domain.Entities;

namespace SSBJr.TecAgro.Inventario.Domain.Tests.Entities;

public class UsuarioTests
{
    [Fact]
    public void Usuario_DeveTerPropriedadesInicializadasCorretamente()
    {
      // Arrange & Act
        var usuario = new Usuario();

     // Assert
        usuario.Id.Should().Be(Guid.Empty);
        usuario.Login.Should().BeEmpty();
        usuario.SenhaHash.Should().BeEmpty();
        usuario.Nome.Should().BeEmpty();
        usuario.Email.Should().BeEmpty();
        usuario.DataCriacao.Should().Be(DateTime.MinValue);
        usuario.UltimoAcesso.Should().BeNull();
        usuario.Ativo.Should().BeTrue();
    }

  [Fact]
    public void Usuario_DevePermitirAtribuicaoDePropriedades()
    {
  // Arrange
        var id = Guid.NewGuid();
   var dataCriacao = DateTime.UtcNow;
        var ultimoAcesso = DateTime.UtcNow;

        // Act
      var usuario = new Usuario
        {
  Id = id,
 Login = "usuario_teste",
       SenhaHash = "hash_senha_teste",
            Nome = "Usuário Teste",
    Email = "usuario@teste.com",
            DataCriacao = dataCriacao,
            UltimoAcesso = ultimoAcesso,
     Ativo = true
        };

        // Assert
        usuario.Id.Should().Be(id);
        usuario.Login.Should().Be("usuario_teste");
        usuario.SenhaHash.Should().Be("hash_senha_teste");
        usuario.Nome.Should().Be("Usuário Teste");
        usuario.Email.Should().Be("usuario@teste.com");
 usuario.DataCriacao.Should().Be(dataCriacao);
        usuario.UltimoAcesso.Should().Be(ultimoAcesso);
        usuario.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Usuario_DevePermitirUltimoAcessoNulo()
    {
// Arrange & Act
        var usuario = new Usuario { UltimoAcesso = null };

        // Assert
    usuario.UltimoAcesso.Should().BeNull();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Usuario_DevePermitirStatusAtivo(bool ativo)
    {
        // Arrange & Act
        var usuario = new Usuario { Ativo = ativo };

      // Assert
        usuario.Ativo.Should().Be(ativo);
    }
}

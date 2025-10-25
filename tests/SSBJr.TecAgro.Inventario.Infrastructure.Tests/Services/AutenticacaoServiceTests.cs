using SSBJr.TecAgro.Inventario.Domain.Entities;
using SSBJr.TecAgro.Inventario.Domain.Repositories;
using SSBJr.TecAgro.Inventario.Infrastructure.Services;

namespace SSBJr.TecAgro.Inventario.Infrastructure.Tests.Services;

public class AutenticacaoServiceTests
{
    private readonly Mock<IUsuarioRepository> _mockRepository;
    private readonly AutenticacaoService _service;

    public AutenticacaoServiceTests()
    {
 _mockRepository = new Mock<IUsuarioRepository>();
        _service = new AutenticacaoService(_mockRepository.Object);
 }

    [Fact]
    public void GerarHashSenha_DeveGerarHashConsistente()
    {
   // Arrange
  var senha = "senha123";

   // Act
        var hash1 = _service.GerarHashSenha(senha);
        var hash2 = _service.GerarHashSenha(senha);

        // Assert
        hash1.Should().NotBeNullOrEmpty();
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void GerarHashSenha_DeveGerarHashesDiferentesParaSenhasDiferentes()
    {
        // Arrange
  var senha1 = "senha123";
        var senha2 = "senha456";

// Act
  var hash1 = _service.GerarHashSenha(senha1);
        var hash2 = _service.GerarHashSenha(senha2);

     // Assert
     hash1.Should().NotBe(hash2);
    }

    [Fact]
  public void VerificarSenha_DeveRetornarTrueParaSenhaCorreta()
    {
        // Arrange
  var senha = "senha123";
        var hash = _service.GerarHashSenha(senha);

        // Act
        var resultado = _service.VerificarSenha(senha, hash);

        // Assert
  resultado.Should().BeTrue();
    }

    [Fact]
    public void VerificarSenha_DeveRetornarFalseParaSenhaIncorreta()
    {
        // Arrange
        var senha = "senha123";
        var senhaIncorreta = "senha456";
        var hash = _service.GerarHashSenha(senha);

        // Act
        var resultado = _service.VerificarSenha(senhaIncorreta, hash);

    // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task AutenticarAsync_DeveRetornarTokenQuandoCredenciaisValidas()
    {
   // Arrange
      var login = "usuario_teste";
   var senha = "senha123";
  var usuario = new Usuario
        {
          Id = Guid.NewGuid(),
     Login = login,
          SenhaHash = _service.GerarHashSenha(senha),
  Nome = "Teste",
          Email = "teste@email.com",
    Ativo = true
        };

        _mockRepository.Setup(r => r.GetByLoginAsync(login)).ReturnsAsync(usuario);
  _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Usuario>())).Returns(Task.CompletedTask);

        // Act
        var token = await _service.AutenticarAsync(login, senha);

      // Assert
 token.Should().NotBeNullOrEmpty();
   _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task AutenticarAsync_DeveRetornarNuloQuandoUsuarioNaoExiste()
    {
        // Arrange
    var login = "usuario_inexistente";
        var senha = "senha123";

   _mockRepository.Setup(r => r.GetByLoginAsync(login)).ReturnsAsync((Usuario?)null);

        // Act
      var token = await _service.AutenticarAsync(login, senha);

      // Assert
    token.Should().BeNull();
    }

    [Fact]
    public async Task AutenticarAsync_DeveRetornarNuloQuandoSenhaIncorreta()
    {
    // Arrange
      var login = "usuario_teste";
        var senhaCorreta = "senha123";
        var senhaIncorreta = "senha_errada";
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Login = login,
            SenhaHash = _service.GerarHashSenha(senhaCorreta),
      Nome = "Teste",
            Email = "teste@email.com",
     Ativo = true
        };

        _mockRepository.Setup(r => r.GetByLoginAsync(login)).ReturnsAsync(usuario);

     // Act
        var token = await _service.AutenticarAsync(login, senhaIncorreta);

   // Assert
        token.Should().BeNull();
    }

    [Fact]
    public async Task ValidarTokenAsync_DeveRetornarTrueParaTokenValido()
    {
        // Arrange
   var login = "usuario_teste";
  var senha = "senha123";
 var usuario = new Usuario
    {
       Id = Guid.NewGuid(),
            Login = login,
  SenhaHash = _service.GerarHashSenha(senha),
            Nome = "Teste",
            Email = "teste@email.com",
         Ativo = true
 };

        _mockRepository.Setup(r => r.GetByLoginAsync(login)).ReturnsAsync(usuario);
_mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Usuario>())).Returns(Task.CompletedTask);

        var token = await _service.AutenticarAsync(login, senha);

        // Act
        var resultado = await _service.ValidarTokenAsync(token!);

        // Assert
        resultado.Should().BeTrue();
  }

    [Fact]
    public async Task ValidarTokenAsync_DeveRetornarFalseParaTokenInvalido()
    {
 // Arrange
   var tokenInvalido = "token_invalido";

   // Act
        var resultado = await _service.ValidarTokenAsync(tokenInvalido);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task ValidarTokenAsync_DeveRetornarFalseParaTokenVazio()
    {
   // Act
        var resultado = await _service.ValidarTokenAsync("");

 // Assert
        resultado.Should().BeFalse();
    }
}

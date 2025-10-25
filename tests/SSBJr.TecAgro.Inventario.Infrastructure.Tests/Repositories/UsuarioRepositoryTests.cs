using Microsoft.EntityFrameworkCore;
using SSBJr.TecAgro.Inventario.Domain.Entities;
using SSBJr.TecAgro.Inventario.Infrastructure.Data;
using SSBJr.TecAgro.Inventario.Infrastructure.Repositories;

namespace SSBJr.TecAgro.Inventario.Infrastructure.Tests.Repositories;

public class UsuarioRepositoryTests : IDisposable
{
    private readonly InventarioDbContext _context;
    private readonly UsuarioRepository _repository;

    public UsuarioRepositoryTests()
    {
 var options = new DbContextOptionsBuilder<InventarioDbContext>()
         .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
         .Options;

   _context = new InventarioDbContext(options);
 _repository = new UsuarioRepository(_context);
    }

    [Fact]
public async Task AddAsync_DeveAdicionarUsuarioComSucesso()
{
        // Arrange
      var usuario = new Usuario
        {
     Id = Guid.NewGuid(),
      Login = "usuario_teste",
      SenhaHash = "hash_teste",
      Nome = "Usuário Teste",
     Email = "usuario@teste.com",
 DataCriacao = DateTime.UtcNow,
 Ativo = true
      };

  // Act
    await _repository.AddAsync(usuario);
        var usuarioSalvo = await _repository.GetByIdAsync(usuario.Id);

        // Assert
        usuarioSalvo.Should().NotBeNull();
        usuarioSalvo!.Login.Should().Be("usuario_teste");
  usuarioSalvo.Nome.Should().Be("Usuário Teste");
 }

    [Fact]
    public async Task GetByLoginAsync_DeveRetornarUsuarioQuandoExistir()
{
  // Arrange
     var usuario = new Usuario
        {
          Id = Guid.NewGuid(),
      Login = "usuario_login",
   SenhaHash = "hash",
  Nome = "Teste",
Email = "teste@email.com",
     DataCriacao = DateTime.UtcNow,
Ativo = true
     };
    await _repository.AddAsync(usuario);

  // Act
    var resultado = await _repository.GetByLoginAsync("usuario_login");

        // Assert
        resultado.Should().NotBeNull();
    resultado!.Login.Should().Be("usuario_login");
    }

    [Fact]
    public async Task GetByLoginAsync_DeveRetornarNuloQuandoNaoExistir()
    {
   // Act
      var resultado = await _repository.GetByLoginAsync("login_inexistente");

 // Assert
  resultado.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarUsuarioQuandoExistir()
    {
        // Arrange
        var usuario = new Usuario
  {
       Id = Guid.NewGuid(),
            Login = "usuario_id",
            SenhaHash = "hash",
    Nome = "Teste",
      Email = "teste@email.com",
     DataCriacao = DateTime.UtcNow,
            Ativo = true
        };
        await _repository.AddAsync(usuario);

        // Act
        var resultado = await _repository.GetByIdAsync(usuario.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(usuario.Id);
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
  public async Task UpdateAsync_DeveAtualizarUsuarioComSucesso()
    {
 // Arrange
        var usuario = new Usuario
      {
       Id = Guid.NewGuid(),
       Login = "usuario_update",
     SenhaHash = "hash",
  Nome = "Nome Original",
Email = "original@email.com",
  DataCriacao = DateTime.UtcNow,
    Ativo = true
};
        await _repository.AddAsync(usuario);

   // Act
        usuario.Nome = "Nome Atualizado";
        usuario.Email = "atualizado@email.com";
  await _repository.UpdateAsync(usuario);
        var usuarioAtualizado = await _repository.GetByIdAsync(usuario.Id);

  // Assert
        usuarioAtualizado.Should().NotBeNull();
    usuarioAtualizado!.Nome.Should().Be("Nome Atualizado");
    usuarioAtualizado.Email.Should().Be("atualizado@email.com");
    }

    [Fact]
    public async Task UpdateAsync_DevePermitirAtualizarUltimoAcesso()
    {
        // Arrange
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
   Login = "usuario_acesso",
      SenhaHash = "hash",
    Nome = "Teste Acesso",
   Email = "acesso@email.com",
            DataCriacao = DateTime.UtcNow,
          UltimoAcesso = null,
            Ativo = true
        };
  await _repository.AddAsync(usuario);

      // Act
        usuario.UltimoAcesso = DateTime.UtcNow;
      await _repository.UpdateAsync(usuario);
        var usuarioAtualizado = await _repository.GetByIdAsync(usuario.Id);

        // Assert
        usuarioAtualizado.Should().NotBeNull();
        usuarioAtualizado!.UltimoAcesso.Should().NotBeNull();
        usuarioAtualizado.UltimoAcesso.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

 public void Dispose()
    {
        _context.Database.EnsureDeleted();
 _context.Dispose();
GC.SuppressFinalize(this);
    }
}

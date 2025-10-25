using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SSBJr.TecAgro.Inventario.Domain.Entities;
using SSBJr.TecAgro.Inventario.Domain.Events;
using SSBJr.TecAgro.Inventario.Domain.Repositories;
using SSBJr.TecAgro.Inventario.Server.Controllers;

namespace SSBJr.TecAgro.Inventario.Server.Tests.Controllers;

public class ProdutosControllerTests
{
    private readonly Mock<IProdutoRepository> _mockRepository;
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<ProdutosController>> _mockLogger;
    private readonly ProdutosController _controller;

    public ProdutosControllerTests()
    {
        _mockRepository = new Mock<IProdutoRepository>();
_mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<ProdutosController>>();
_controller = new ProdutosController(_mockRepository.Object, _mockMediator.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAll_DeveRetornarOkComListaDeProdutos()
    {
        // Arrange
        var produtos = new List<Produto>
        {
      new Produto { Id = Guid.NewGuid(), Nome = "Produto 1" },
            new Produto { Id = Guid.NewGuid(), Nome = "Produto 2" }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(produtos);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
   var returnedProdutos = okResult.Value.Should().BeAssignableTo<IEnumerable<Produto>>().Subject;
  returnedProdutos.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_DeveRetornar500QuandoOcorrerErro()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("Erro no banco"));

  // Act
        var result = await _controller.GetAll();

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
   statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetById_DeveRetornarOkComProdutoQuandoExistir()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var produto = new Produto { Id = produtoId, Nome = "Produto Teste" };
        _mockRepository.Setup(r => r.GetByIdAsync(produtoId)).ReturnsAsync(produto);

  // Act
        var result = await _controller.GetById(produtoId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProduto = okResult.Value.Should().BeOfType<Produto>().Subject;
     returnedProduto.Id.Should().Be(produtoId);
        returnedProduto.Nome.Should().Be("Produto Teste");
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFoundQuandoNaoExistir()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
     _mockRepository.Setup(r => r.GetByIdAsync(produtoId)).ReturnsAsync((Produto?)null);

     // Act
var result = await _controller.GetById(produtoId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Search_DeveRetornarOkComProdutosEncontrados()
    {
// Arrange
        var termo = "Teste";
        var produtos = new List<Produto>
        {
            new Produto { Id = Guid.NewGuid(), Nome = "Produto Teste 1" },
       new Produto { Id = Guid.NewGuid(), Nome = "Produto Teste 2" }
        };
        _mockRepository.Setup(r => r.SearchAsync(termo)).ReturnsAsync(produtos);

        // Act
        var result = await _controller.Search(termo);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProdutos = okResult.Value.Should().BeAssignableTo<IEnumerable<Produto>>().Subject;
    returnedProdutos.Should().HaveCount(2);
    }

    [Fact]
    public async Task Search_DeveRetornarBadRequestQuandoTermoVazio()
    {
      // Act
   var result = await _controller.Search("");

      // Assert
 result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

[Fact]
  public async Task Create_DeveCriarProdutoComSucessoERetornarCreated()
 {
        // Arrange
     var produto = new Produto
     {
            Nome = "Novo Produto",
        Descricao = "Descrição"
   };
        
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Produto>())).Returns(Task.CompletedTask);
        _mockMediator.Setup(m => m.Publish(It.IsAny<ProdutoCriadoEvent>(), default)).Returns(Task.CompletedTask);

 // Act
        var result = await _controller.Create(produto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(ProdutosController.GetById));
        var createdProduto = createdResult.Value.Should().BeOfType<Produto>().Subject;
      createdProduto.Id.Should().NotBe(Guid.Empty);
  createdProduto.DataCadastro.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        createdProduto.StatusSincronizacao.Should().Be(StatusSincronizacao.Sincronizado);
        
 _mockRepository.Verify(r => r.AddAsync(It.IsAny<Produto>()), Times.Once);
        _mockMediator.Verify(m => m.Publish(It.IsAny<ProdutoCriadoEvent>(), default), Times.Once);
    }

    [Fact]
    public async Task Update_DeveAtualizarProdutoComSucessoERetornarNoContent()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
    var produtoExistente = new Produto { Id = produtoId, Nome = "Produto Original" };
        var produtoAtualizado = new Produto { Id = produtoId, Nome = "Produto Atualizado" };
        
    _mockRepository.Setup(r => r.GetByIdAsync(produtoId)).ReturnsAsync(produtoExistente);
      _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Produto>())).Returns(Task.CompletedTask);
        _mockMediator.Setup(m => m.Publish(It.IsAny<ProdutoAtualizadoEvent>(), default)).Returns(Task.CompletedTask);

    // Act
  var result = await _controller.Update(produtoId, produtoAtualizado);

        // Assert
        result.Should().BeOfType<NoContentResult>();
   _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Produto>()), Times.Once);
     _mockMediator.Verify(m => m.Publish(It.IsAny<ProdutoAtualizadoEvent>(), default), Times.Once);
    }

    [Fact]
    public async Task Update_DeveRetornarBadRequestQuandoIdNaoCorresponder()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
var produto = new Produto { Id = Guid.NewGuid(), Nome = "Produto" };

        // Act
        var result = await _controller.Update(produtoId, produto);

      // Assert
   result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_DeveRetornarNotFoundQuandoProdutoNaoExistir()
    {
        // Arrange
    var produtoId = Guid.NewGuid();
        var produto = new Produto { Id = produtoId, Nome = "Produto" };
   _mockRepository.Setup(r => r.GetByIdAsync(produtoId)).ReturnsAsync((Produto?)null);

        // Act
        var result = await _controller.Update(produtoId, produto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_DeveDeletarProdutoComSucessoERetornarNoContent()
  {
 // Arrange
        var produtoId = Guid.NewGuid();
      var produto = new Produto { Id = produtoId, Nome = "Produto" };
        _mockRepository.Setup(r => r.GetByIdAsync(produtoId)).ReturnsAsync(produto);
        _mockRepository.Setup(r => r.DeleteAsync(produtoId)).Returns(Task.CompletedTask);

        // Act
   var result = await _controller.Delete(produtoId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockRepository.Verify(r => r.DeleteAsync(produtoId), Times.Once);
    }

    [Fact]
    public async Task Delete_DeveRetornarNotFoundQuandoProdutoNaoExistir()
    {
        // Arrange
  var produtoId = Guid.NewGuid();
   _mockRepository.Setup(r => r.GetByIdAsync(produtoId)).ReturnsAsync((Produto?)null);

        // Act
   var result = await _controller.Delete(produtoId);

   // Assert
    result.Should().BeOfType<NotFoundResult>();
    }
}

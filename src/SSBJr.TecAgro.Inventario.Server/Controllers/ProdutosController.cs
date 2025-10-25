using Microsoft.AspNetCore.Mvc;
using SSBJr.TecAgro.Inventario.Domain.Entities;
using SSBJr.TecAgro.Inventario.Domain.Repositories;
using MediatR;
using SSBJr.TecAgro.Inventario.Domain.Events;

namespace SSBJr.TecAgro.Inventario.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<ProdutosController> _logger;

    public ProdutosController(
      IProdutoRepository produtoRepository,
        IMediator mediator,
        ILogger<ProdutosController> logger)
    {
   _produtoRepository = produtoRepository;
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> GetAll()
    {
        try
   {
            var produtos = await _produtoRepository.GetAllAsync();
            return Ok(produtos);
        }
    catch (Exception ex)
        {
  _logger.LogError(ex, "Erro ao buscar produtos");
            return StatusCode(500, "Erro ao buscar produtos");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Produto>> GetById(Guid id)
    {
      try
        {
   var produto = await _produtoRepository.GetByIdAsync(id);
      
       if (produto == null)
                return NotFound();

    return Ok(produto);
  }
        catch (Exception ex)
        {
      _logger.LogError(ex, "Erro ao buscar produto {ProdutoId}", id);
        return StatusCode(500, "Erro ao buscar produto");
   }
    }

    [HttpGet("search")]
 public async Task<ActionResult<IEnumerable<Produto>>> Search([FromQuery] string termo)
    {
        try
        {
     if (string.IsNullOrWhiteSpace(termo))
            return BadRequest("Termo de busca é obrigatório");

    var produtos = await _produtoRepository.SearchAsync(termo);
            return Ok(produtos);
      }
    catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao pesquisar produtos com termo: {Termo}", termo);
            return StatusCode(500, "Erro ao pesquisar produtos");
  }
    }

    [HttpPost]
    public async Task<ActionResult<Produto>> Create([FromBody] Produto produto)
    {
        try
        {
     if (produto.Id == Guid.Empty)
   produto.Id = Guid.NewGuid();

    produto.DataCadastro = DateTime.UtcNow;
          produto.DataAtualizacao = DateTime.UtcNow;
            produto.StatusSincronizacao = StatusSincronizacao.Sincronizado;

        await _produtoRepository.AddAsync(produto);
 await _mediator.Publish(new ProdutoCriadoEvent(produto.Id, produto.Nome, produto.DataCadastro));

            _logger.LogInformation("Produto {ProdutoId} criado com sucesso", produto.Id);
        
    return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
      }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produto");
            return StatusCode(500, "Erro ao criar produto");
     }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] Produto produto)
    {
     try
   {
      if (id != produto.Id)
      return BadRequest("ID do produto não corresponde");

  var produtoExistente = await _produtoRepository.GetByIdAsync(id);
  if (produtoExistente == null)
      return NotFound();

    produto.DataAtualizacao = DateTime.UtcNow;
     await _produtoRepository.UpdateAsync(produto);
    await _mediator.Publish(new ProdutoAtualizadoEvent(produto.Id, produto.DataAtualizacao));

            _logger.LogInformation("Produto {ProdutoId} atualizado com sucesso", produto.Id);

         return NoContent();
        }
        catch (Exception ex)
        {
   _logger.LogError(ex, "Erro ao atualizar produto {ProdutoId}", id);
     return StatusCode(500, "Erro ao atualizar produto");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
   var produto = await _produtoRepository.GetByIdAsync(id);
          if (produto == null)
   return NotFound();

            await _produtoRepository.DeleteAsync(id);

       _logger.LogInformation("Produto {ProdutoId} deletado com sucesso", id);

     return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar produto {ProdutoId}", id);
            return StatusCode(500, "Erro ao deletar produto");
   }
    }
}

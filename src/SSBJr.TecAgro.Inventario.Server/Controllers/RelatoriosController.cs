using Microsoft.AspNetCore.Mvc;
using SSBJr.TecAgro.Inventario.Domain.Repositories;

namespace SSBJr.TecAgro.Inventario.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RelatoriosController : ControllerBase
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly ILogger<RelatoriosController> _logger;

    public RelatoriosController(
  IProdutoRepository produtoRepository,
   ILogger<RelatoriosController> logger)
  {
        _produtoRepository = produtoRepository;
        _logger = logger;
    }

    [HttpGet("resumo")]
    public async Task<ActionResult<RelatorioResumo>> GetResumo()
    {
    try
 {
            var produtos = await _produtoRepository.GetAllAsync();
            var pendentes = await _produtoRepository.GetPendentesAsync();

         var resumo = new RelatorioResumo
       {
  TotalProdutos = await _produtoRepository.CountAsync(),
 ProdutosPendentes = pendentes.Count(),
    ValorTotalEstoque = produtos.Sum(p => p.ValorAquisicao * p.QuantidadeEstoque),
  QuantidadeTotalEstoque = produtos.Sum(p => p.QuantidadeEstoque),
       ProdutosBaixoEstoque = produtos.Count(p => p.QuantidadeEstoque < 10),
ProdutosPorCategoria = produtos.GroupBy(p => p.Categoria)
         .ToDictionary(g => string.IsNullOrEmpty(g.Key) ? "Sem Categoria" : g.Key, g => g.Count())
   };

    return Ok(resumo);
        }
        catch (Exception ex)
        {
  _logger.LogError(ex, "Erro ao gerar resumo");
            return StatusCode(500, "Erro ao gerar resumo");
        }
    }

    [HttpGet("estoque-baixo")]
    public async Task<ActionResult> GetProdutosEstoqueBaixo([FromQuery] decimal limite = 10)
    {
        try
    {
    var produtos = await _produtoRepository.GetAllAsync();
       var produtosBaixoEstoque = produtos.Where(p => p.QuantidadeEstoque < limite);

      return Ok(produtosBaixoEstoque);
    }
   catch (Exception ex)
   {
  _logger.LogError(ex, "Erro ao buscar produtos com estoque baixo");
  return StatusCode(500, "Erro ao buscar produtos");
        }
    }

 [HttpGet("por-categoria")]
    public async Task<ActionResult> GetProdutosPorCategoria()
    {
      try
        {
       var produtos = await _produtoRepository.GetAllAsync();
   var porCategoria = produtos.GroupBy(p => string.IsNullOrEmpty(p.Categoria) ? "Sem Categoria" : p.Categoria)
        .Select(g => new
    {
      Categoria = g.Key,
         Quantidade = g.Count(),
        ValorTotal = g.Sum(p => p.ValorAquisicao * p.QuantidadeEstoque)
});

   return Ok(porCategoria);
        }
 catch (Exception ex)
     {
    _logger.LogError(ex, "Erro ao agrupar produtos por categoria");
  return StatusCode(500, "Erro ao processar relatório");
        }
    }
}

public class RelatorioResumo
{
    public int TotalProdutos { get; set; }
    public int ProdutosPendentes { get; set; }
    public decimal ValorTotalEstoque { get; set; }
    public decimal QuantidadeTotalEstoque { get; set; }
    public int ProdutosBaixoEstoque { get; set; }
 public Dictionary<string, int> ProdutosPorCategoria { get; set; } = new();
}

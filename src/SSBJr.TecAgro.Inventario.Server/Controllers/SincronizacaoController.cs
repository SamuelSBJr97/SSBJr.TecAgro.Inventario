using Microsoft.AspNetCore.Mvc;
using SSBJr.TecAgro.Inventario.Domain.Repositories;
using SSBJr.TecAgro.Inventario.Domain.Services;
using SSBJr.TecAgro.Inventario.Domain.Entities;

namespace SSBJr.TecAgro.Inventario.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SincronizacaoController : ControllerBase
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly ILogSincronizacaoRepository _logRepository;
 private readonly ISincronizacaoService _sincronizacaoService;
    private readonly ILogger<SincronizacaoController> _logger;

    public SincronizacaoController(
        IProdutoRepository produtoRepository,
        ILogSincronizacaoRepository logRepository,
    ISincronizacaoService sincronizacaoService,
        ILogger<SincronizacaoController> logger)
    {
        _produtoRepository = produtoRepository;
        _logRepository = logRepository;
      _sincronizacaoService = sincronizacaoService;
    _logger = logger;
    }

    [HttpGet("status")]
    public async Task<ActionResult> GetStatus()
    {
        try
     {
            var pendentes = await _produtoRepository.GetPendentesAsync();
            var isOnline = await _sincronizacaoService.VerificarConectividadeAsync();

     return Ok(new
     {
     Online = isOnline,
     ProdutosPendentes = pendentes.Count(),
    UltimaVerificacao = DateTime.UtcNow
      });
        }
        catch (Exception ex)
    {
    _logger.LogError(ex, "Erro ao verificar status de sincronização");
        return StatusCode(500, "Erro ao verificar status");
        }
 }

    [HttpPost("sincronizar-produto/{id}")]
    public async Task<ActionResult> SincronizarProduto(Guid id)
    {
 try
  {
 var produto = await _produtoRepository.GetByIdAsync(id);
            
       if (produto == null)
    return NotFound();

            var sucesso = await _sincronizacaoService.SincronizarProdutoAsync(produto);

  if (sucesso)
            {
         return Ok(new { Mensagem = "Produto sincronizado com sucesso" });
  }
   else
         {
    return BadRequest(new { Mensagem = "Falha na sincronização", Erro = produto.ErroSincronizacao });
         }
    }
        catch (Exception ex)
        {
 _logger.LogError(ex, "Erro ao sincronizar produto {ProdutoId}", id);
    return StatusCode(500, "Erro ao sincronizar produto");
        }
    }

    [HttpPost("sincronizar-pendentes")]
  public async Task<ActionResult> SincronizarPendentes()
    {
        try
        {
      await _sincronizacaoService.SincronizarPendentesAsync();
 
    var pendentes = await _produtoRepository.GetPendentesAsync();
    
            return Ok(new 
     { 
    Mensagem = "Sincronização concluída",
           PendentesRestantes = pendentes.Count()
  });
      }
   catch (Exception ex)
        {
       _logger.LogError(ex, "Erro ao sincronizar produtos pendentes");
            return StatusCode(500, "Erro ao sincronizar");
        }
    }

    [HttpGet("logs/{produtoId}")]
    public async Task<ActionResult> GetLogs(Guid produtoId)
    {
        try
        {
          var logs = await _logRepository.GetByProdutoIdAsync(produtoId);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar logs do produto {ProdutoId}", produtoId);
  return StatusCode(500, "Erro ao buscar logs");
        }
    }

    [HttpGet("logs-recentes")]
    public async Task<ActionResult> GetLogsRecentes([FromQuery] int quantidade = 100)
    {
      try
        {
        var logs = await _logRepository.GetRecentesAsync(quantidade);
   return Ok(logs);
        }
        catch (Exception ex)
        {
      _logger.LogError(ex, "Erro ao buscar logs recentes");
  return StatusCode(500, "Erro ao buscar logs");
     }
    }
}

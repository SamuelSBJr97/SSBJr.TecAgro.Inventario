using System.Net.Http.Json;
using System.Net.NetworkInformation;
using MediatR;
using Serilog;
using SSBJr.TecAgro.Inventario.Domain.Entities;
using SSBJr.TecAgro.Inventario.Domain.Events;
using SSBJr.TecAgro.Inventario.Domain.Repositories;
using SSBJr.TecAgro.Inventario.Domain.Services;

namespace SSBJr.TecAgro.Inventario.Infrastructure.Services;

public class SincronizacaoService : ISincronizacaoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly ILogSincronizacaoRepository _logRepository;
    private readonly IArmazenamentoService _armazenamentoService;
 private readonly IMediator _mediator;
    private readonly HttpClient _httpClient;
    private readonly string _serverUrl;

    public SincronizacaoService(
        IProdutoRepository produtoRepository,
   ILogSincronizacaoRepository logRepository,
        IArmazenamentoService armazenamentoService,
        IMediator mediator,
        HttpClient httpClient,
        string serverUrl)
    {
        _produtoRepository = produtoRepository;
        _logRepository = logRepository;
 _armazenamentoService = armazenamentoService;
        _mediator = mediator;
        _httpClient = httpClient;
        _serverUrl = serverUrl;
    }

    public async Task<bool> VerificarConectividadeAsync()
  {
        try
        {
            var ping = new Ping();
      var reply = await ping.SendPingAsync("8.8.8.8", 3000);
    return reply.Status == IPStatus.Success;
        }
   catch
        {
            return false;
    }
  }

    public async Task<bool> SincronizarProdutoAsync(Produto produto)
    {
 try
        {
          if (!await VerificarConectividadeAsync())
   {
      Log.Warning("Sem conectividade. Produto {ProdutoId} permanece pendente.", produto.Id);
      return false;
            }

            produto.StatusSincronizacao = StatusSincronizacao.EmProcessamento;
          await _produtoRepository.UpdateAsync(produto);

      // Enviar produto para o servidor
            var response = await _httpClient.PostAsJsonAsync($"{_serverUrl}/api/produtos", produto);
   
            if (response.IsSuccessStatusCode)
            {
    produto.StatusSincronizacao = StatusSincronizacao.Sincronizado;
         produto.ErroSincronizacao = null;
         await _produtoRepository.UpdateAsync(produto);

              await _logRepository.AddAsync(new LogSincronizacao
  {
  Id = Guid.NewGuid(),
     ProdutoId = produto.Id,
               DataHora = DateTime.UtcNow,
  Status = StatusSincronizacao.Sincronizado,
      Mensagem = "Sincronização bem-sucedida"
     });

    await _mediator.Publish(new ProdutoSincronizadoEvent(produto.Id, DateTime.UtcNow));

      Log.Information("Produto {ProdutoId} sincronizado com sucesso.", produto.Id);
   return true;
     }
            else
         {
     var erro = await response.Content.ReadAsStringAsync();
    produto.StatusSincronizacao = StatusSincronizacao.Erro;
 produto.ErroSincronizacao = erro;
   await _produtoRepository.UpdateAsync(produto);

     await _logRepository.AddAsync(new LogSincronizacao
  {
       Id = Guid.NewGuid(),
        ProdutoId = produto.Id,
            DataHora = DateTime.UtcNow,
       Status = StatusSincronizacao.Erro,
    Mensagem = "Erro na sincronização",
  Detalhes = erro
      });

   await _mediator.Publish(new ErroSincronizacaoEvent(produto.Id, erro, DateTime.UtcNow));

     Log.Error("Erro ao sincronizar produto {ProdutoId}: {Erro}", produto.Id, erro);
      return false;
      }
        }
  catch (Exception ex)
        {
            produto.StatusSincronizacao = StatusSincronizacao.Erro;
         produto.ErroSincronizacao = ex.Message;
        await _produtoRepository.UpdateAsync(produto);

    await _logRepository.AddAsync(new LogSincronizacao
  {
    Id = Guid.NewGuid(),
     ProdutoId = produto.Id,
          DataHora = DateTime.UtcNow,
   Status = StatusSincronizacao.Erro,
     Mensagem = "Exceção durante sincronização",
    Detalhes = ex.ToString()
      });

Log.Error(ex, "Exceção ao sincronizar produto {ProdutoId}", produto.Id);
   return false;
        }
    }

    public async Task SincronizarPendentesAsync()
    {
    try
        {
            if (!await VerificarConectividadeAsync())
       {
   Log.Information("Sem conectividade para sincronização automática.");
          return;
         }

     var produtosPendentes = await _produtoRepository.GetPendentesAsync();
            
 Log.Information("Iniciando sincronização de {Count} produtos pendentes.", produtosPendentes.Count());

            foreach (var produto in produtosPendentes)
  {
        await SincronizarProdutoAsync(produto);
 await Task.Delay(500); // Evitar sobrecarga
}

            Log.Information("Sincronização automática concluída.");
        }
  catch (Exception ex)
        {
  Log.Error(ex, "Erro na sincronização automática de pendentes");
        }
    }
}

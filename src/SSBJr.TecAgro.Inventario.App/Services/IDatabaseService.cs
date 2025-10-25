using SSBJr.TecAgro.Inventario.Domain.Entities;

namespace SSBJr.TecAgro.Inventario.App.Services;

public interface IDatabaseService
{
    // Produtos
    Task<List<Produto>> GetProdutosAsync();
 Task<Produto?> GetProdutoAsync(Guid id);
    Task<int> SaveProdutoAsync(Produto produto);
    Task<int> DeleteProdutoAsync(Produto produto);
    Task<List<Produto>> SearchProdutosAsync(string searchText);
 Task<List<Produto>> GetProdutosPendentesAsync();
    
    // Logs
    Task<int> SaveLogAsync(LogSincronizacao log);
  Task<List<LogSincronizacao>> GetLogsAsync();
    Task<List<LogSincronizacao>> GetLogsByProdutoAsync(Guid produtoId);
}

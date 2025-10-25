using SSBJr.TecAgro.Inventario.Domain.Entities;

namespace SSBJr.TecAgro.Inventario.Domain.Repositories;

public interface ILogSincronizacaoRepository
{
    Task AddAsync(LogSincronizacao log);
 Task<IEnumerable<LogSincronizacao>> GetByProdutoIdAsync(Guid produtoId);
    Task<IEnumerable<LogSincronizacao>> GetRecentesAsync(int quantidade = 100);
}

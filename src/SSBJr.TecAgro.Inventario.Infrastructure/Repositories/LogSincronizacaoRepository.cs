using Microsoft.EntityFrameworkCore;
using SSBJr.TecAgro.Inventario.Domain.Entities;
using SSBJr.TecAgro.Inventario.Domain.Repositories;
using SSBJr.TecAgro.Inventario.Infrastructure.Data;

namespace SSBJr.TecAgro.Inventario.Infrastructure.Repositories;

public class LogSincronizacaoRepository : ILogSincronizacaoRepository
{
    private readonly InventarioDbContext _context;

    public LogSincronizacaoRepository(InventarioDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(LogSincronizacao log)
    {
        await _context.LogsSincronizacao.AddAsync(log);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<LogSincronizacao>> GetByProdutoIdAsync(Guid produtoId)
    {
        return await _context.LogsSincronizacao
  .Where(l => l.ProdutoId == produtoId)
 .OrderByDescending(l => l.DataHora)
    .ToListAsync();
    }

    public async Task<IEnumerable<LogSincronizacao>> GetRecentesAsync(int quantidade = 100)
    {
        return await _context.LogsSincronizacao
     .OrderByDescending(l => l.DataHora)
       .Take(quantidade)
 .ToListAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using SSBJr.TecAgro.Inventario.Domain.Entities;
using SSBJr.TecAgro.Inventario.Domain.Repositories;
using SSBJr.TecAgro.Inventario.Infrastructure.Data;

namespace SSBJr.TecAgro.Inventario.Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly InventarioDbContext _context;

    public ProdutoRepository(InventarioDbContext context)
    {
      _context = context;
  }

    public async Task<Produto?> GetByIdAsync(Guid id)
  {
        return await _context.Produtos.FindAsync(id);
    }

    public async Task<IEnumerable<Produto>> GetAllAsync()
    {
        return await _context.Produtos
 .Where(p => p.Ativo)
            .OrderByDescending(p => p.DataAtualizacao)
            .ToListAsync();
    }

    public async Task<IEnumerable<Produto>> GetPendentesAsync()
    {
        return await _context.Produtos
     .Where(p => p.StatusSincronizacao == StatusSincronizacao.Pendente)
            .ToListAsync();
    }

    public async Task<IEnumerable<Produto>> SearchAsync(string termo)
    {
 return await _context.Produtos
            .Where(p => p.Ativo && 
               (p.Nome.Contains(termo) || 
            p.Descricao.Contains(termo) ||
       p.CodigoFiscal.Contains(termo) ||
   p.SKU.Contains(termo) ||
  p.Categoria.Contains(termo)))
            .OrderBy(p => p.Nome)
      .ToListAsync();
    }

    public async Task AddAsync(Produto produto)
 {
        await _context.Produtos.AddAsync(produto);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Produto produto)
    {
        _context.Produtos.Update(produto);
    await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var produto = await GetByIdAsync(id);
        if (produto != null)
     {
  produto.Ativo = false;
        await UpdateAsync(produto);
 }
    }

    public async Task<int> CountAsync()
    {
 return await _context.Produtos.CountAsync(p => p.Ativo);
    }
}

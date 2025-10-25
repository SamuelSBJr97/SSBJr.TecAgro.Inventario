using SSBJr.TecAgro.Inventario.Domain.Entities;

namespace SSBJr.TecAgro.Inventario.Domain.Repositories;

public interface IProdutoRepository
{
    Task<Produto?> GetByIdAsync(Guid id);
    Task<IEnumerable<Produto>> GetAllAsync();
    Task<IEnumerable<Produto>> GetPendentesAsync();
  Task<IEnumerable<Produto>> SearchAsync(string termo);
    Task AddAsync(Produto produto);
    Task UpdateAsync(Produto produto);
    Task DeleteAsync(Guid id);
    Task<int> CountAsync();
}

using SSBJr.TecAgro.Inventario.Domain.Entities;

namespace SSBJr.TecAgro.Inventario.Domain.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(Guid id);
    Task<Usuario?> GetByLoginAsync(string login);
    Task AddAsync(Usuario usuario);
    Task UpdateAsync(Usuario usuario);
}

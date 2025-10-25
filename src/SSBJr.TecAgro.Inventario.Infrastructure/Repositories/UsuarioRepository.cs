using Microsoft.EntityFrameworkCore;
using SSBJr.TecAgro.Inventario.Domain.Entities;
using SSBJr.TecAgro.Inventario.Domain.Repositories;
using SSBJr.TecAgro.Inventario.Infrastructure.Data;

namespace SSBJr.TecAgro.Inventario.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly InventarioDbContext _context;

    public UsuarioRepository(InventarioDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByIdAsync(Guid id)
    {
  return await _context.Usuarios.FindAsync(id);
    }

    public async Task<Usuario?> GetByLoginAsync(string login)
    {
        return await _context.Usuarios
    .FirstOrDefaultAsync(u => u.Login == login && u.Ativo);
    }

    public async Task AddAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
     await _context.SaveChangesAsync();
    }
}

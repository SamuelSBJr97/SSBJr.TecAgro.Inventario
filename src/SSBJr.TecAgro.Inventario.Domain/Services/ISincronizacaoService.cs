using SSBJr.TecAgro.Inventario.Domain.Entities;

namespace SSBJr.TecAgro.Inventario.Domain.Services;

public interface ISincronizacaoService
{
    Task<bool> SincronizarProdutoAsync(Produto produto);
    Task<bool> VerificarConectividadeAsync();
    Task SincronizarPendentesAsync();
}

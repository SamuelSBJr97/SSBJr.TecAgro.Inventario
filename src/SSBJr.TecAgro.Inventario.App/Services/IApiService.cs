using SSBJr.TecAgro.Inventario.Domain.Entities;

namespace SSBJr.TecAgro.Inventario.App.Services;

public interface IApiService
{
    // Produtos
    Task<IEnumerable<Produto>> GetProdutosAsync();
    Task<Produto?> GetProdutoByIdAsync(Guid id);
    Task<IEnumerable<Produto>> SearchProdutosAsync(string termo);
    Task<Produto> CreateProdutoAsync(Produto produto);
    Task UpdateProdutoAsync(Produto produto);
    Task DeleteProdutoAsync(Guid id);
    Task<int> GetProdutosCountAsync();
    
    // Autenticação
    Task<string?> LoginAsync(string login, string senha);
    Task<bool> ValidateTokenAsync(string token);
    
    // Sincronização
    Task<bool> SincronizarProdutoAsync(Produto produto);
    Task SincronizarPendentesAsync();
    Task<bool> VerificarConectividadeAsync();
}

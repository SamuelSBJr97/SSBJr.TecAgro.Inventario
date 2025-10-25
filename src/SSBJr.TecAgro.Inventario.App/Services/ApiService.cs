using System.Net.Http.Json;
using System.Net.Http.Headers;
using SSBJr.TecAgro.Inventario.Domain.Entities;

namespace SSBJr.TecAgro.Inventario.App.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
 private readonly IPreferencesService _preferencesService;
    private string? _token;

    public ApiService(HttpClient httpClient, IPreferencesService preferencesService)
    {
   _httpClient = httpClient;
        _preferencesService = preferencesService;
        _token = _preferencesService.GetToken();
        
        if (!string.IsNullOrEmpty(_token))
        {
       _httpClient.DefaultRequestHeaders.Authorization = 
   new AuthenticationHeaderValue("Bearer", _token);
        }
    }

  #region Produtos

    public async Task<IEnumerable<Produto>> GetProdutosAsync()
{
        try
        {
            var response = await _httpClient.GetAsync("api/produtos");
     response.EnsureSuccessStatusCode();
   return await response.Content.ReadFromJsonAsync<IEnumerable<Produto>>() ?? [];
     }
        catch (Exception ex)
        {
     Console.WriteLine($"Erro ao buscar produtos: {ex.Message}");
   return [];
}
    }

    public async Task<Produto?> GetProdutoByIdAsync(Guid id)
    {
    try
        {
    var response = await _httpClient.GetAsync($"api/produtos/{id}");
response.EnsureSuccessStatusCode();
    return await response.Content.ReadFromJsonAsync<Produto>();
    }
      catch (Exception ex)
 {
        Console.WriteLine($"Erro ao buscar produto: {ex.Message}");
            return null;
}
  }

    public async Task<IEnumerable<Produto>> SearchProdutosAsync(string termo)
    {
        try
      {
       var response = await _httpClient.GetAsync($"api/produtos/search?termo={Uri.EscapeDataString(termo)}");
         response.EnsureSuccessStatusCode();
      return await response.Content.ReadFromJsonAsync<IEnumerable<Produto>>() ?? [];
        }
        catch (Exception ex)
        {
Console.WriteLine($"Erro ao pesquisar produtos: {ex.Message}");
  return [];
    }
    }

    public async Task<Produto> CreateProdutoAsync(Produto produto)
    {
    try
      {
            var response = await _httpClient.PostAsJsonAsync("api/produtos", produto);
      response.EnsureSuccessStatusCode();
 return await response.Content.ReadFromJsonAsync<Produto>() ?? produto;
        }
 catch (Exception ex)
        {
       Console.WriteLine($"Erro ao criar produto: {ex.Message}");
            throw;
   }
    }

 public async Task UpdateProdutoAsync(Produto produto)
    {
      try
        {
       var response = await _httpClient.PutAsJsonAsync($"api/produtos/{produto.Id}", produto);
     response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
    {
            Console.WriteLine($"Erro ao atualizar produto: {ex.Message}");
    throw;
        }
    }

 public async Task DeleteProdutoAsync(Guid id)
    {
     try
        {
   var response = await _httpClient.DeleteAsync($"api/produtos/{id}");
      response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
     Console.WriteLine($"Erro ao deletar produto: {ex.Message}");
            throw;
 }
    }

public async Task<int> GetProdutosCountAsync()
    {
  try
        {
     var produtos = await GetProdutosAsync();
    return produtos.Count();
        }
        catch
        {
      return 0;
        }
    }

    #endregion

    #region Autenticação

  public async Task<string?> LoginAsync(string login, string senha)
    {
   try
        {
     var response = await _httpClient.PostAsJsonAsync("api/autenticacao/login", new { login, senha });
            
        if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
         _token = result?.Token;
        
       if (!string.IsNullOrEmpty(_token))
   {
     _preferencesService.SaveToken(_token);
         _httpClient.DefaultRequestHeaders.Authorization = 
           new AuthenticationHeaderValue("Bearer", _token);
    }
          
       return _token;
  }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao fazer login: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
 {
            var response = await _httpClient.PostAsJsonAsync("api/autenticacao/validate", new { token });
          return response.IsSuccessStatusCode;
        }
        catch
        {
      return false;
        }
    }

 #endregion

    #region Sincronização

    public async Task<bool> SincronizarProdutoAsync(Produto produto)
    {
        try
   {
      var response = await _httpClient.PostAsJsonAsync("api/produtos", produto);
     return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
 Console.WriteLine($"Erro ao sincronizar produto: {ex.Message}");
return false;
 }
    }

    public async Task SincronizarPendentesAsync()
    {
     // Esta funcionalidade será implementada através do serviço local
        await Task.CompletedTask;
    }

    public async Task<bool> VerificarConectividadeAsync()
    {
        try
    {
  var response = await _httpClient.GetAsync("api/produtos");
        return response.IsSuccessStatusCode;
     }
        catch
        {
     return false;
    }
    }

    #endregion

    private class LoginResponse
    {
     public string Token { get; set; } = string.Empty;
    }
}

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SSBJr.TecAgro.Inventario.App.Services;
using SSBJr.TecAgro.Inventario.Domain.Entities;

namespace SSBJr.TecAgro.Inventario.App.ViewModels;

public partial class ProdutosViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<Produto> produtos = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private int totalProdutos;

    [ObservableProperty]
    private int produtosPendentes;

    [ObservableProperty]
    private bool isOnline;

    public ProdutosViewModel(
  IDatabaseService databaseService,
   IApiService apiService)
    {
        _databaseService = databaseService;
        _apiService = apiService;
        Title = "Inventário";
    }

    [RelayCommand]
    private async Task LoadProdutosAsync()
    {
 if (IsBusy) return;

        try
        {
   IsBusy = true;
   
            var produtosList = string.IsNullOrWhiteSpace(SearchText)
    ? await _databaseService.GetProdutosAsync()
  : await _databaseService.SearchProdutosAsync(SearchText);

       Produtos.Clear();
  foreach (var produto in produtosList)
      {
    Produtos.Add(produto);
         }

 TotalProdutos = produtosList.Count;
  var pendentes = await _databaseService.GetProdutosPendentesAsync();
   ProdutosPendentes = pendentes.Count;
            
       IsOnline = await _apiService.VerificarConectividadeAsync();
 }
      catch (Exception ex)
   {
            await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar produtos: {ex.Message}", "OK");
        }
        finally
      {
            IsBusy = false;
          IsRefreshing = false;
     }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
 await LoadProdutosAsync();
    }

    [RelayCommand]
    private async Task NovoProdutoAsync()
    {
        await Shell.Current.GoToAsync("detalheproduto");
    }

    [RelayCommand]
    private async Task EditarProdutoAsync(Produto produto)
    {
        if (produto == null) return;
        
   await Shell.Current.GoToAsync($"detalheproduto?id={produto.Id}");
    }

    [RelayCommand]
  private async Task DeletarProdutoAsync(Produto produto)
    {
  if (produto == null) return;

        var confirm = await Shell.Current.DisplayAlert(
  "Confirmar", 
            $"Deseja realmente deletar o produto '{produto.Nome}'?", 
            "Sim", 
        "Não");

   if (!confirm) return;

        try
        {
            IsBusy = true;
       await _databaseService.DeleteProdutoAsync(produto);
      
            // Tentar sincronizar com o servidor
 if (await _apiService.VerificarConectividadeAsync())
            {
await _apiService.DeleteProdutoAsync(produto.Id);
            }

            await LoadProdutosAsync();
       await Shell.Current.DisplayAlert("Sucesso", "Produto deletado com sucesso!", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", $"Erro ao deletar produto: {ex.Message}", "OK");
        }
        finally
        {
    IsBusy = false;
        }
    }

  [RelayCommand]
    private async Task SincronizarAsync()
    {
    if (IsBusy) return;

        try
        {
    IsBusy = true;

   IsOnline = await _apiService.VerificarConectividadeAsync();
            
            if (!IsOnline)
  {
          await Shell.Current.DisplayAlert("Aviso", "Sem conexão com a internet", "OK");
     return;
        }

    var pendentes = await _databaseService.GetProdutosPendentesAsync();
            
         var sucesso = 0;
            var falhas = 0;

            foreach (var produto in pendentes)
       {
   var resultado = await _apiService.SincronizarProdutoAsync(produto);
       if (resultado)
        {
        produto.StatusSincronizacao = StatusSincronizacao.Sincronizado;
        produto.ErroSincronizacao = null;
   await _databaseService.SaveProdutoAsync(produto);
            sucesso++;
         }
 else
          {
                falhas++;
        }
   }

        await LoadProdutosAsync();

  await Shell.Current.DisplayAlert("Sincronização", 
      $"Sincronização concluída!\nSucesso: {sucesso}\nFalhas: {falhas}", "OK");
    }
        catch (Exception ex)
        {
   await Shell.Current.DisplayAlert("Erro", $"Erro na sincronização: {ex.Message}", "OK");
        }
        finally
    {
            IsBusy = false;
        }
    }
}

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SSBJr.TecAgro.Inventario.Domain.Entities;
using SSBJr.TecAgro.Inventario.Domain.Repositories;
using SSBJr.TecAgro.Inventario.Domain.Services;

namespace SSBJr.TecAgro.Inventario.App.ViewModels;

public partial class ProdutosViewModel : BaseViewModel
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly ISincronizacaoService _sincronizacaoService;

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
  IProdutoRepository produtoRepository,
        ISincronizacaoService sincronizacaoService)
    {
        _produtoRepository = produtoRepository;
        _sincronizacaoService = sincronizacaoService;
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
       ? await _produtoRepository.GetAllAsync()
      : await _produtoRepository.SearchAsync(SearchText);

            Produtos.Clear();
       foreach (var produto in produtosList)
            {
       Produtos.Add(produto);
            }

         TotalProdutos = await _produtoRepository.CountAsync();
       var pendentes = await _produtoRepository.GetPendentesAsync();
            ProdutosPendentes = pendentes.Count();
    
   IsOnline = await _sincronizacaoService.VerificarConectividadeAsync();
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
    private async Task SincronizarAsync()
    {
        if (IsBusy) return;

        try
   {
            IsBusy = true;

            IsOnline = await _sincronizacaoService.VerificarConectividadeAsync();
            
  if (!IsOnline)
      {
      await Shell.Current.DisplayAlert("Aviso", "Sem conexão com a internet", "OK");
    return;
        }

   await _sincronizacaoService.SincronizarPendentesAsync();
    await LoadProdutosAsync();

            await Shell.Current.DisplayAlert("Sucesso", "Sincronização concluída!", "OK");
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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SSBJr.TecAgro.Inventario.App.Services;
using SSBJr.TecAgro.Inventario.Domain.Entities;
using System.Collections.ObjectModel;

namespace SSBJr.TecAgro.Inventario.App.ViewModels;

public partial class RelatoriosViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;

    [ObservableProperty]
    private ObservableCollection<Produto> _produtos = [];

    [ObservableProperty]
    private int _totalProdutos;

    [ObservableProperty]
    private decimal _valorTotalEstoque;

    [ObservableProperty]
    private decimal _quantidadeTotalEstoque;

 [ObservableProperty]
    private int _produtosAtivos;

    [ObservableProperty]
    private int _produtosBaixoEstoque;

    [ObservableProperty]
    private Dictionary<string, int> _produtosPorCategoria = [];

    [ObservableProperty]
    private DateTime _dataRelatorio;

    public RelatoriosViewModel(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
        Title = "Relatórios";
        DataRelatorio = DateTime.Now;
    }

    [RelayCommand]
    async Task CarregarRelatorioAsync()
 {
   if (IsBusy) return;

 IsBusy = true;

        try
   {
            var todosProdutos = await _databaseService.GetProdutosAsync();
        
   Produtos.Clear();
            foreach (var produto in todosProdutos)
            {
     Produtos.Add(produto);
       }

 TotalProdutos = todosProdutos.Count;
ProdutosAtivos = todosProdutos.Count(p => p.Ativo);
   ValorTotalEstoque = todosProdutos.Sum(p => p.ValorAquisicao * p.QuantidadeEstoque);
   QuantidadeTotalEstoque = todosProdutos.Sum(p => p.QuantidadeEstoque);
            ProdutosBaixoEstoque = todosProdutos.Count(p => p.QuantidadeEstoque < 10);

       // Produtos por categoria
 ProdutosPorCategoria = todosProdutos
        .GroupBy(p => string.IsNullOrEmpty(p.Categoria) ? "Sem Categoria" : p.Categoria)
     .ToDictionary(g => g.Key, g => g.Count());
  }
        catch (Exception ex)
 {
     await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar relatório: {ex.Message}", "OK");
  }
   finally
        {
     IsBusy = false;
        }
    }

    [RelayCommand]
    async Task ExportarRelatorioAsync()
    {
        if (IsBusy) return;

        try
        {
          IsBusy = true;

        var csv = GerarCSV();
     var fileName = $"relatorio_inventario_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
       var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            await File.WriteAllTextAsync(filePath, csv);

     await Shell.Current.DisplayAlert("Sucesso", 
           $"Relatório exportado com sucesso!\nLocal: {filePath}", "OK");

    // Opcionalmente, compartilhar o arquivo
   await Share.RequestAsync(new ShareFileRequest
          {
    Title = "Compartilhar Relatório",
           File = new ShareFile(filePath)
            });
 }
        catch (Exception ex)
        {
  await Shell.Current.DisplayAlert("Erro", $"Erro ao exportar relatório: {ex.Message}", "OK");
        }
   finally
   {
     IsBusy = false;
        }
    }

    private string GerarCSV()
    {
   var sb = new System.Text.StringBuilder();
        
   // Header
      sb.AppendLine("Nome,Descrição,Código Fiscal,SKU,Categoria,Quantidade,Unidade,Valor Aquisição,Valor Revenda,Localização");
        
        // Data
        foreach (var produto in Produtos)
        {
        sb.AppendLine($"\"{produto.Nome}\",\"{produto.Descricao}\",\"{produto.CodigoFiscal}\",\"{produto.SKU}\",\"{produto.Categoria}\",{produto.QuantidadeEstoque},\"{produto.UnidadeMedida}\",{produto.ValorAquisicao},{produto.ValorRevenda},\"{produto.Localizacao}\"");
        }

   return sb.ToString();
    }
}

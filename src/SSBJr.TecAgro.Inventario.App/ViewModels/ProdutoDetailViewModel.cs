using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SSBJr.TecAgro.Inventario.App.Services;
using SSBJr.TecAgro.Inventario.Domain.Entities;
using System.Collections.ObjectModel;

namespace SSBJr.TecAgro.Inventario.App.ViewModels;

public partial class ProdutoDetailViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
 private readonly IApiService _apiService;

    [ObservableProperty]
    private Produto _produto = new();

    [ObservableProperty]
    private bool _isNew;

    [ObservableProperty]
 private ObservableCollection<string> _fotos = [];

    [ObservableProperty]
    private ObservableCollection<string> _categorias = [
        "Fertilizantes",
   "Defensivos",
    "Sementes",
   "Equipamentos",
  "Ferramentas",
    "Insumos",
        "Ração Animal",
        "Medicamentos Veterinários",
        "Outros"
  ];

    [ObservableProperty]
    private ObservableCollection<string> _unidadesMedida = [
   "UN",
  "KG",
        "L",
   "M",
"M²",
  "M³",
      "CX",
        "SC",
    "FD",
    "TON"
    ];

    public ProdutoDetailViewModel(IDatabaseService databaseService, IApiService apiService)
    {
        _databaseService = databaseService;
    _apiService = apiService;
   Title = "Detalhes do Produto";
 }

    public async Task LoadProdutoAsync(Guid? produtoId)
    {
  IsBusy = true;

  try
    {
     if (produtoId.HasValue && produtoId != Guid.Empty)
     {
       var produto = await _databaseService.GetProdutoAsync(produtoId.Value);
    if (produto != null)
{
       Produto = produto;
       Fotos = new ObservableCollection<string>(produto.Fotos);
   IsNew = false;
   Title = "Editar Produto";
        }
    }
       else
      {
    Produto = new Produto
   {
   Id = Guid.NewGuid(),
       DataCadastro = DateTime.UtcNow,
DataAtualizacao = DateTime.UtcNow,
 StatusSincronizacao = StatusSincronizacao.Pendente,
Ativo = true
       };
  IsNew = true;
       Title = "Novo Produto";
        }
   }
     catch (Exception ex)
      {
     await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar produto: {ex.Message}", "OK");
  }
        finally
  {
   IsBusy = false;
        }
    }

    [RelayCommand]
    async Task SaveAsync()
    {
    if (IsBusy) return;

   if (string.IsNullOrWhiteSpace(Produto.Nome))
   {
     await Shell.Current.DisplayAlert("Validação", "Nome é obrigatório", "OK");
 return;
}

     IsBusy = true;

  try
        {
  Produto.Fotos = Fotos.ToList();
   Produto.DataAtualizacao = DateTime.UtcNow;
    Produto.StatusSincronizacao = StatusSincronizacao.Pendente;

      await _databaseService.SaveProdutoAsync(Produto);

   // Tentar sincronizar com o servidor
      if (await _apiService.VerificarConectividadeAsync())
    {
  await _apiService.SincronizarProdutoAsync(Produto);
         }

   await Shell.Current.DisplayAlert("Sucesso", "Produto salvo com sucesso!", "OK");
       await Shell.Current.GoToAsync("..");
  }
    catch (Exception ex)
      {
await Shell.Current.DisplayAlert("Erro", $"Erro ao salvar produto: {ex.Message}", "OK");
        }
        finally
 {
  IsBusy = false;
      }
  }

 [RelayCommand]
    async Task CancelAsync()
    {
   await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task AdicionarFotoAsync()
    {
        try
    {
var result = await MediaPicker.PickPhotoAsync();
if (result != null)
       {
          var stream = await result.OpenReadAsync();
 var fileName = $"{Guid.NewGuid()}{Path.GetExtension(result.FileName)}";
    var filePath = Path.Combine(FileSystem.AppDataDirectory, "fotos", fileName);
    
 Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
      
   using var fileStream = File.Create(filePath);
 await stream.CopyToAsync(fileStream);
     
        Fotos.Add(filePath);
   }
        }
      catch (Exception ex)
        {
       await Shell.Current.DisplayAlert("Erro", $"Erro ao adicionar foto: {ex.Message}", "OK");
   }
    }

 [RelayCommand]
    async Task RemoverFotoAsync(string foto)
    {
   var confirm = await Shell.Current.DisplayAlert("Confirmar", "Deseja remover esta foto?", "Sim", "Não");
   if (confirm)
      {
     Fotos.Remove(foto);
     try
       {
        if (File.Exists(foto))
 {
   File.Delete(foto);
}
  }
     catch
       {
         // Ignorar erros ao deletar arquivo
       }
        }
    }

    [RelayCommand]
 async Task VisualizarFotoAsync(string foto)
    {
   // Implementar visualização de foto em tela cheia
     await Task.CompletedTask;
    }
}

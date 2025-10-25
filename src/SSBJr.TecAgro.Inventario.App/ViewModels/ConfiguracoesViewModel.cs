using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SSBJr.TecAgro.Inventario.App.Services;

namespace SSBJr.TecAgro.Inventario.App.ViewModels;

public partial class ConfiguracoesViewModel : ObservableObject
{
    private readonly IPreferencesService _preferencesService;

    [ObservableProperty]
    private string _serverUrl = string.Empty;

    [ObservableProperty]
    private string _plataformaAtual = string.Empty;

    [ObservableProperty]
    private string _versaoApp = "1.0.0";

    public ConfiguracoesViewModel(IPreferencesService preferencesService)
    {
        _preferencesService = preferencesService;
        
        // Carregar URL atual
        _serverUrl = _preferencesService.GetServerUrl();
        
 // Detectar plataforma
        _plataformaAtual = DetectarPlataforma();
    }

    [RelayCommand]
    private async Task SalvarConfiguracoes()
    {
        try
        {
// Validar URL
            if (string.IsNullOrWhiteSpace(_serverUrl))
            {
     await Shell.Current.DisplayAlert("Erro", "URL do servidor nao pode estar vazia", "OK");
         return;
      }

  if (!Uri.TryCreate(_serverUrl, UriKind.Absolute, out var uri))
  {
    await Shell.Current.DisplayAlert("Erro", "URL invalida", "OK");
        return;
          }

    // Salvar URL
            _preferencesService.SaveServerUrl(_serverUrl);

            await Shell.Current.DisplayAlert("Sucesso", "Configuracoes salvas com sucesso!", "OK");
        }
        catch (Exception ex)
        {
         await Shell.Current.DisplayAlert("Erro", $"Erro ao salvar configuracoes: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private void RestaurarPadrao()
    {
        ServerUrl = GetDefaultUrlForPlatform();
  }

    [RelayCommand]
    private async Task TestarConexao()
  {
        try
        {
            using var httpClient = new HttpClient();
    httpClient.Timeout = TimeSpan.FromSeconds(5);

            var response = await httpClient.GetAsync($"{_serverUrl}/swagger/index.html");

          if (response.IsSuccessStatusCode)
            {
         await Shell.Current.DisplayAlert("Sucesso", "Conexao com o servidor bem-sucedida!", "OK");
            }
  else
    {
   await Shell.Current.DisplayAlert("Aviso", $"Servidor respondeu com status: {response.StatusCode}", "OK");
       }
        }
catch (HttpRequestException)
        {
            await Shell.Current.DisplayAlert("Erro", "Nao foi possivel conectar ao servidor. Verifique a URL e se o servidor esta rodando.", "OK");
        }
        catch (TaskCanceledException)
   {
     await Shell.Current.DisplayAlert("Erro", "Timeout ao conectar. O servidor pode estar lento ou indisponivel.", "OK");
        }
        catch (Exception ex)
  {
            await Shell.Current.DisplayAlert("Erro", $"Erro ao testar conexao: {ex.Message}", "OK");
        }
    }

    private string DetectarPlataforma()
    {
#if ANDROID
        return "Android";
#elif IOS
        return "iOS";
#elif WINDOWS
   return "Windows";
#elif MACCATALYST
    return "macOS";
#else
        return "Desconhecida";
#endif
  }

    private string GetDefaultUrlForPlatform()
    {
#if ANDROID
        return "http://10.0.2.2:5000";
#elif IOS
        return "http://localhost:5000";
#elif WINDOWS
        return "http://localhost:5000";
#elif MACCATALYST
        return "http://localhost:5000";
#else
   return "http://localhost:5000";
#endif
    }
}

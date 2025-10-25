using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SSBJr.TecAgro.Inventario.App.Services;

namespace SSBJr.TecAgro.Inventario.App.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly IPreferencesService _preferencesService;

  [ObservableProperty]
    private string _login = string.Empty;

    [ObservableProperty]
    private string _senha = string.Empty;

    [ObservableProperty]
    private string _serverUrl;

    [ObservableProperty]
  private bool _rememberMe = true;

    public LoginViewModel(IApiService apiService, IPreferencesService preferencesService)
    {
        _apiService = apiService;
        _preferencesService = preferencesService;
        _serverUrl = preferencesService.GetServerUrl();
        Title = "Login";
    }

    [RelayCommand]
    async Task LoginAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Senha))
    {
            await Shell.Current.DisplayAlert("Validação", "Login e senha são obrigatórios", "OK");
            return;
        }

        IsBusy = true;

        try
        {
     _preferencesService.SaveServerUrl(ServerUrl);

      var token = await _apiService.LoginAsync(Login, Senha);

  if (!string.IsNullOrEmpty(token))
        {
        if (RememberMe)
   {
        _preferencesService.SaveToken(token);
            }

      await Shell.Current.DisplayAlert("Sucesso", "Login realizado com sucesso!", "OK");
                await Shell.Current.GoToAsync("///produtos");
  }
            else
 {
      await Shell.Current.DisplayAlert("Erro", "Login ou senha inválidos", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", $"Erro ao fazer login: {ex.Message}", "OK");
        }
   finally
        {
            IsBusy = false;
        }
    }
}

using Microsoft.Extensions.Logging;
using SSBJr.TecAgro.Inventario.App.Services;
using SSBJr.TecAgro.Inventario.App.ViewModels;
using SSBJr.TecAgro.Inventario.App.Views;

namespace SSBJr.TecAgro.Inventario.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Registrar Serviços
		builder.Services.AddSingleton<IPreferencesService, PreferencesService>();
		builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
		
		builder.Services.AddHttpClient<IApiService, ApiService>(client =>
		{
			var preferencesService = new PreferencesService();
			var serverUrl = preferencesService.GetServerUrl();
			client.BaseAddress = new Uri(serverUrl);
			client.Timeout = TimeSpan.FromSeconds(30);
		});

		// Registrar ViewModels
		builder.Services.AddTransient<ProdutosViewModel>();
		builder.Services.AddTransient<ProdutoDetailViewModel>();
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<RelatoriosViewModel>();
		builder.Services.AddTransient<ConfiguracoesViewModel>();

		// Registrar Pages
		builder.Services.AddTransient<ProdutosPage>();
		builder.Services.AddTransient<ProdutoDetailPage>();
		builder.Services.AddTransient<ConfiguracoesPage>();

		return builder.Build();
	}
}

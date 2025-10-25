using SSBJr.TecAgro.Inventario.App.ViewModels;

namespace SSBJr.TecAgro.Inventario.App.Views;

public partial class ConfiguracoesPage : ContentPage
{
	public ConfiguracoesPage(ConfiguracoesViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

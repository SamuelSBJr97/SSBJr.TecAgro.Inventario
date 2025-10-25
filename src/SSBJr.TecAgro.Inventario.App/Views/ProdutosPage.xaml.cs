using SSBJr.TecAgro.Inventario.App.ViewModels;

namespace SSBJr.TecAgro.Inventario.App.Views;

public partial class ProdutosPage : ContentPage
{
	public ProdutosPage(ProdutosViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		
 if (BindingContext is ProdutosViewModel vm)
		{
			await vm.LoadProdutosCommand.ExecuteAsync(null);
		}
	}
}

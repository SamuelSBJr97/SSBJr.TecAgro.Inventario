using SSBJr.TecAgro.Inventario.App.ViewModels;

namespace SSBJr.TecAgro.Inventario.App.Views;

[QueryProperty(nameof(ProdutoId), "id")]
public partial class ProdutoDetailPage : ContentPage
{
	private readonly ProdutoDetailViewModel _viewModel;
	public string? ProdutoId { get; set; }

	public ProdutoDetailPage(ProdutoDetailViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		
		Guid? id = null;
		if (Guid.TryParse(ProdutoId, out var parsedId))
		{
			id = parsedId;
		}
		
		await _viewModel.LoadProdutoAsync(id);
	}
}

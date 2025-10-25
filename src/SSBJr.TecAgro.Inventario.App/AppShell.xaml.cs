using SSBJr.TecAgro.Inventario.App.Views;

namespace SSBJr.TecAgro.Inventario.App;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		// Registrar rotas para navegação
		Routing.RegisterRoute("detalheproduto", typeof(ProdutoDetailPage));
	}
}

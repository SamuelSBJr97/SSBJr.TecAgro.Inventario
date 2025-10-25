using CommunityToolkit.Mvvm.ComponentModel;

namespace SSBJr.TecAgro.Inventario.App.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private bool isRefreshing;
}

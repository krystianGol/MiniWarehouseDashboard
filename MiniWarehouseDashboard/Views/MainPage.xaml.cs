using MiniWarehouseDashboard.ViewModels;

namespace MiniWarehouseDashboard.Views;

public partial class MainPage : ContentPage
{
	public MainPage(DashboardViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is DashboardViewModel vm)
        {
            vm.LoadDataCommand.Execute(null);
        }
    }
}

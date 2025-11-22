using System.Windows.Input;

namespace MiniWarehouseDashboard.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly ICsvDataService _dataService;
    private bool _isBusy;
    private double _lowStockPercentage;

    public ObservableCollection<WarehouseItem> Items { get; } = new();
    public ObservableCollection<WarehouseItem> TopItems { get; } = new();

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public double LowStockPercentage
    {
        get => _lowStockPercentage;
        set => SetProperty(ref _lowStockPercentage, value);
    }

    public ICommand LoadDataCommand { get; }

    public DashboardViewModel(ICsvDataService dataService)
    {
        _dataService = dataService;
        LoadDataCommand = new Command(async () => await LoadDataAsync());
    }

    private async Task LoadDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var items = await _dataService.GetItemsAsync();

            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }

            // Calculate Top 5 by Quantity
            TopItems.Clear();
            var top5 = items.OrderByDescending(i => i.Quantity).Take(5);
            foreach (var item in top5)
            {
                TopItems.Add(item);
            }

            // Calculate Low Stock Percentage
            if (items.Any())
            {
                var lowStockCount = items.Count(i => i.IsLowStock);
                LowStockPercentage = (double)lowStockCount / items.Count * 100;
            }
            else
            {
                LowStockPercentage = 0;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}

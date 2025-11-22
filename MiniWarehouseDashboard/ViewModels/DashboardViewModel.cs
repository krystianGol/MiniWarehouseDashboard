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
    public ICommand ImportCsvCommand { get; }

    public DashboardViewModel(ICsvDataService dataService)
    {
        _dataService = dataService;
        LoadDataCommand = new Command(async () => await LoadDataAsync());
        ImportCsvCommand = new Command(async () => await ImportCsvAsync());
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

            // Update Top 5 chart
            UpdateTopItems();

            // Recalculate Low Stock Percentage
            UpdateLowStockPercentage();
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

    private async Task ImportCsvAsync()
    {
        if (IsBusy) return;

        try
        {
            // Open file picker for CSV files
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "text/csv", "text/comma-separated-values" } },
                    { DevicePlatform.iOS, new[] { "public.comma-separated-values-text" } },
                    { DevicePlatform.WinUI, new[] { ".csv" } },
                    { DevicePlatform.macOS, new[] { "csv" } },
                });

            var options = new PickOptions
            {
                PickerTitle = "Select CSV File",
                FileTypes = customFileType
            };

            var result = await FilePicker.Default.PickAsync(options);
            
            if (result == null)
                return; // User cancelled

            IsBusy = true;

            // Open and read the CSV file
            using var stream = await result.OpenReadAsync();
            var importedItems = await _dataService.ImportCsvAsync(stream);

            // Add imported items to existing collection
            foreach (var item in importedItems)
            {
                Items.Add(item);
            }

            // Update Top 5 chart
            UpdateTopItems();

            // Recalculate Low Stock Percentage
            UpdateLowStockPercentage();

            // Show success message
            await Application.Current.MainPage.DisplayAlert(
                "Success", 
                $"Successfully imported {importedItems.Count} items from CSV.", 
                "OK");
        }
        catch (InvalidDataException ex)
        {
            // Show user-friendly error message for CSV format issues
            await Application.Current.MainPage.DisplayAlert(
                "Import Error", 
                $"Invalid CSV format:\n{ex.Message}", 
                "OK");
        }
        catch (Exception ex)
        {
            // Show generic error message
            await Application.Current.MainPage.DisplayAlert(
                "Error", 
                $"Failed to import CSV file:\n{ex.Message}", 
                "OK");
            System.Diagnostics.Debug.WriteLine($"Error importing CSV: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void UpdateTopItems()
    {
        TopItems.Clear();
        var top5 = Items.OrderByDescending(i => i.Quantity).Take(5);
        foreach (var item in top5)
        {
            TopItems.Add(item);
        }
    }

    private void UpdateLowStockPercentage()
    {
        if (Items.Any())
        {
            var lowStockCount = Items.Count(i => i.IsLowStock);
            LowStockPercentage = (double)lowStockCount / Items.Count * 100;
        }
        else
        {
            LowStockPercentage = 0;
        }
    }
}

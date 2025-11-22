using System.Windows.Input;

namespace MiniWarehouseDashboard.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly ICsvDataService _dataService;
    private bool _isBusy;
    private double _lowStockPercentage;
    private string _searchProductName = string.Empty;
    private string _searchCategory = string.Empty;
    private string _minQuantity = string.Empty;
    private string _maxQuantity = string.Empty;
    private int _lowStockFilterIndex = 0;

    public ObservableCollection<WarehouseItem> Items { get; } = new();
    public ObservableCollection<WarehouseItem> FilteredItems { get; } = new();
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

    public string SearchProductName
    {
        get => _searchProductName;
        set
        {
            if (SetProperty(ref _searchProductName, value))
                ApplyFilters();
        }
    }

    public string SearchCategory
    {
        get => _searchCategory;
        set
        {
            if (SetProperty(ref _searchCategory, value))
                ApplyFilters();
        }
    }

    public string MinQuantity
    {
        get => _minQuantity;
        set
        {
            if (SetProperty(ref _minQuantity, value))
                ApplyFilters();
        }
    }

    public string MaxQuantity
    {
        get => _maxQuantity;
        set
        {
            if (SetProperty(ref _maxQuantity, value))
                ApplyFilters();
        }
    }

    public int LowStockFilterIndex
    {
        get => _lowStockFilterIndex;
        set
        {
            if (SetProperty(ref _lowStockFilterIndex, value))
                ApplyFilters();
        }
    }

    public ICommand LoadDataCommand { get; }
    public ICommand ImportCsvCommand { get; }
    public ICommand ClearFiltersCommand { get; }

    public DashboardViewModel(ICsvDataService dataService)
    {
        _dataService = dataService;
        LoadDataCommand = new Command(async () => await LoadDataAsync());
        ImportCsvCommand = new Command(async () => await ImportCsvAsync());
        ClearFiltersCommand = new Command(ClearFilters);
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

            UpdateTopItems();

            UpdateLowStockPercentage();

            ApplyFilters();
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
                return;

            IsBusy = true;

            using var stream = await result.OpenReadAsync();
            var importedItems = await _dataService.ImportCsvAsync(stream);

            foreach (var item in importedItems)
            {
                Items.Add(item);
            }

            ApplyFilters();

            await Application.Current.MainPage.DisplayAlert(
                "Success", 
                $"Successfully imported {importedItems.Count} items from CSV.", 
                "OK");
        }
        catch (InvalidDataException ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Import Error", 
                $"Invalid CSV format:\n{ex.Message}", 
                "OK");
        }
        catch (Exception ex)
        {
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
        var top5 = FilteredItems.OrderByDescending(i => i.Quantity).Take(5);
        foreach (var item in top5)
        {
            TopItems.Add(item);
        }
    }

    private void UpdateLowStockPercentage()
    {
        if (FilteredItems.Any())
        {
            var lowStockCount = FilteredItems.Count(i => i.IsLowStock);
            LowStockPercentage = (double)lowStockCount / FilteredItems.Count * 100;
        }
        else
        {
            LowStockPercentage = 0;
        }
    }

    private void ApplyFilters()
    {
        FilteredItems.Clear();

        var filtered = Items.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchProductName))
        {
            filtered = filtered.Where(i => i.Name.Contains(SearchProductName, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(SearchCategory))
        {
            filtered = filtered.Where(i => i.Category.Contains(SearchCategory, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(MinQuantity) && int.TryParse(MinQuantity, out int minQty))
        {
            filtered = filtered.Where(i => i.Quantity >= minQty);
        }

        if (!string.IsNullOrWhiteSpace(MaxQuantity) && int.TryParse(MaxQuantity, out int maxQty))
        {
            filtered = filtered.Where(i => i.Quantity <= maxQty);
        }

        if (LowStockFilterIndex == 1)
        {
            filtered = filtered.Where(i => i.IsLowStock);
        }
        else if (LowStockFilterIndex == 2)
        {
            filtered = filtered.Where(i => !i.IsLowStock);
        }

        foreach (var item in filtered)
        {
            FilteredItems.Add(item);
        }

        UpdateTopItems();
        UpdateLowStockPercentage();
    }

    private void ClearFilters()
    {
        SearchProductName = string.Empty;
        SearchCategory = string.Empty;
        MinQuantity = string.Empty;
        MaxQuantity = string.Empty;
        LowStockFilterIndex = 0;
    }
}

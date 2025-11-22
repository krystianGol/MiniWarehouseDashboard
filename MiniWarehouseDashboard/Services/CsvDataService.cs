namespace MiniWarehouseDashboard.Services;

public class CsvDataService : ICsvDataService
{
    private readonly ICsvParserService _parserService;

    public CsvDataService(ICsvParserService parserService)
    {
        _parserService = parserService;
    }

    public async Task<List<WarehouseItem>> GetItemsAsync()
    {
        var items = new List<WarehouseItem>();

        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("inventory.csv");
            using var reader = new StreamReader(stream);

            // Skip header
            await reader.ReadLineAsync();

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var values = line.Split(',');
                if (values.Length >= 4)
                {
                    if (int.TryParse(values[2].Trim(), out int quantity) && 
                        int.TryParse(values[3].Trim(), out int minStock))
                    {
                        items.Add(new WarehouseItem
                        {
                            Name = values[0].Trim(),
                            Category = values[1].Trim(),
                            Quantity = quantity,
                            MinStock = minStock
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error reading CSV: {ex.Message}");
        }

        return items;
    }

    public async Task<List<WarehouseItem>> ImportCsvAsync(Stream csvStream)
    {
        return await _parserService.ParseCsvAsync(csvStream);
    }
}

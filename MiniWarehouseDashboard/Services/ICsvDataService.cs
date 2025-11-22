namespace MiniWarehouseDashboard.Services;

public interface ICsvDataService
{
    Task<List<WarehouseItem>> GetItemsAsync();
    Task<List<WarehouseItem>> ImportCsvAsync(Stream csvStream);
}

namespace MiniWarehouseDashboard.Services;

public interface ICsvDataService
{
    Task<List<WarehouseItem>> GetItemsAsync();
}

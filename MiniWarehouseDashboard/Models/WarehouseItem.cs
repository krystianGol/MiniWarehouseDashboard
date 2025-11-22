namespace MiniWarehouseDashboard.Models;

public class WarehouseItem
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int MinStock { get; set; }

    // Helper property to determine if stock is low
    public bool IsLowStock => Quantity < MinStock;
}

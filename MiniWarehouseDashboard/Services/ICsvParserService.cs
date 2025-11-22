namespace MiniWarehouseDashboard.Services;

public interface ICsvParserService
{
    /// <summary>
    /// Parses a CSV stream and returns a list of WarehouseItem objects.
    /// Expected CSV format: Product, Category, Qty, Low
    /// </summary>
    /// <param name="csvStream">The CSV file stream to parse</param>
    /// <returns>List of parsed WarehouseItem objects</returns>
    /// <exception cref="InvalidDataException">Thrown when CSV format is invalid</exception>
    Task<List<WarehouseItem>> ParseCsvAsync(Stream csvStream);
}

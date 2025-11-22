namespace MiniWarehouseDashboard.Services;

public class CsvParserService : ICsvParserService
{
    public async Task<List<WarehouseItem>> ParseCsvAsync(Stream csvStream)
    {
        var items = new List<WarehouseItem>();

        try
        {
            using var reader = new StreamReader(csvStream);
            
            // Read and validate header
            var headerLine = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(headerLine))
            {
                throw new InvalidDataException("CSV file is empty or has no header.");
            }

            var headers = headerLine.Split(',').Select(h => h.Trim()).ToArray();
            
            // Validate required columns exist
            var productIndex = Array.FindIndex(headers, h => h.Equals("Product", StringComparison.OrdinalIgnoreCase));
            var categoryIndex = Array.FindIndex(headers, h => h.Equals("Category", StringComparison.OrdinalIgnoreCase));
            var qtyIndex = Array.FindIndex(headers, h => h.Equals("Qty", StringComparison.OrdinalIgnoreCase));
            var lowIndex = Array.FindIndex(headers, h => h.Equals("Low", StringComparison.OrdinalIgnoreCase));

            if (productIndex == -1 || categoryIndex == -1 || qtyIndex == -1 || lowIndex == -1)
            {
                throw new InvalidDataException(
                    "CSV file must contain the following columns: Product, Category, Qty, Low. " +
                    $"Found columns: {string.Join(", ", headers)}");
            }

            // Parse data rows
            int lineNumber = 1;
            while (!reader.EndOfStream)
            {
                lineNumber++;
                var line = await reader.ReadLineAsync();
                
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var values = line.Split(',').Select(v => v.Trim()).ToArray();
                
                if (values.Length < headers.Length)
                {
                    System.Diagnostics.Debug.WriteLine($"Warning: Line {lineNumber} has fewer columns than expected. Skipping.");
                    continue;
                }

                try
                {
                    // Parse and validate numeric fields
                    if (!int.TryParse(values[qtyIndex], out int quantity))
                    {
                        throw new InvalidDataException($"Line {lineNumber}: Invalid quantity value '{values[qtyIndex]}'. Must be a number.");
                    }

                    if (!int.TryParse(values[lowIndex], out int minStock))
                    {
                        throw new InvalidDataException($"Line {lineNumber}: Invalid minimum stock value '{values[lowIndex]}'. Must be a number.");
                    }

                    // Create warehouse item
                    items.Add(new WarehouseItem
                    {
                        Name = values[productIndex],
                        Category = values[categoryIndex],
                        Quantity = quantity,
                        MinStock = minStock
                    });
                }
                catch (Exception ex) when (ex is not InvalidDataException)
                {
                    System.Diagnostics.Debug.WriteLine($"Error parsing line {lineNumber}: {ex.Message}");
                    throw new InvalidDataException($"Error parsing line {lineNumber}: {ex.Message}", ex);
                }
            }

            if (items.Count == 0)
            {
                throw new InvalidDataException("CSV file contains no valid data rows.");
            }

            return items;
        }
        catch (InvalidDataException)
        {
            throw; // Re-throw validation errors as-is
        }
        catch (Exception ex)
        {
            throw new InvalidDataException($"Error reading CSV file: {ex.Message}", ex);
        }
    }
}

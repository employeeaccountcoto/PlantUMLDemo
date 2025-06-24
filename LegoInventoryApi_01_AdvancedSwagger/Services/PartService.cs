using LegoInventoryApi.Models;
using System.Globalization;

namespace LegoInventoryApi.Services
{
    public class PartService
    {
        private readonly ILogger<PartService> _logger;
        private readonly string _csvFilePath;
        private List<Part>? _parts;

        public PartService(IWebHostEnvironment webHostEnvironment, ILogger<PartService> logger)
        {
            _logger = logger;
            _csvFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "parts.csv");
        }

        public async Task<IEnumerable<Part>> GetAllPartsAsync()
        {
            if (_parts == null)
            {
                await LoadPartsAsync();
            }
            return _parts ?? Enumerable.Empty<Part>();
        }

        public async Task<Part?> GetPartByPartNumAsync(string partNum)
        {
            if (_parts == null)
            {
                await LoadPartsAsync();
            }
            return _parts?.FirstOrDefault(p => p.PartNum == partNum);
        }

        private async Task LoadPartsAsync()
        {
            _parts = new List<Part>();
            
            if (!File.Exists(_csvFilePath))
            {
                _logger.LogError("CSV file not found: {FilePath}", _csvFilePath);
                return;
            }

            try
            {
                var lines = await File.ReadAllLinesAsync(_csvFilePath);
                
                // Skip the header row
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var part = ParsePartFromCsvLine(line);
                    if (part != null)
                    {
                        _parts.Add(part);
                    }
                }
                
                _logger.LogInformation("Loaded {Count} parts from the CSV file", _parts.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading parts from the CSV file");
                _parts = null;
            }
        }

        private Part? ParsePartFromCsvLine(string line)
        {
            // Handle quoted strings containing commas
            List<string> fields = new List<string>();
            bool inQuotes = false;
            int startIndex = 0;

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (line[i] == ',' && !inQuotes)
                {
                    fields.Add(line.Substring(startIndex, i - startIndex).Trim('"'));
                    startIndex = i + 1;
                }
            }

            // Add the last field
            fields.Add(line.Substring(startIndex).Trim('"'));

            if (fields.Count < 4)
            {
                _logger.LogWarning("Invalid CSV line: {Line}", line);
                return null;
            }

            try
            {
                return new Part
                {
                    PartNum = fields[0],
                    Name = fields[1],
                    PartCategoryId = int.Parse(fields[2], CultureInfo.InvariantCulture),
                    PartMaterial = fields[3]
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing part from CSV line: {Line}", line);
                return null;
            }
        }
    }
}

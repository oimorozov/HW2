using System.Globalization;
using Domain.Enums;

namespace Domain.Import;

public class CsvImporter : ImporterBase
{
    public CsvImporter(Domain.DomainFactory.IDomainFactory factory) : base(factory)
    {
    }

    protected override IEnumerable<ImportDataItem> Parse(string content)
    {
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        var startIndex = 0;
        if (lines.Length > 0 && lines[0].Contains("account_name", StringComparison.OrdinalIgnoreCase))
        {
            startIndex = 1;
        }

        for (int i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var parts = line.Split(',');
            if (parts.Length < 6) continue;

            yield return new ImportDataItem
            {
                AccountName = parts[0].Trim(),
                CategoryName = parts[1].Trim(),
                CategoryType = parts[2].Trim() == "Income" ? CategoryType.Income : CategoryType.Expense,
                OperationType = parts[3].Trim() == "Income" ? OperationType.Income : OperationType.Expense,
                Amount = decimal.Parse(parts[4].Trim(), CultureInfo.InvariantCulture),
                Date = DateTime.Parse(parts[5].Trim()),
                Description = parts.Length > 6 ? parts[6].Trim() : null
            };
        }
    }
}


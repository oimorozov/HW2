using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Enums;

namespace Domain.Import;

public class YamlImporter : ImporterBase
{
    public YamlImporter(Domain.DomainFactory.IDomainFactory factory) : base(factory)
    {
    }

    protected override IEnumerable<ImportDataItem> Parse(string content)
    {   
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var currentItem = new ImportDataItem();
        var itemStarted = false;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            if (trimmed == "-" || trimmed.StartsWith("- "))
            {
                if (itemStarted)
                {
                    yield return currentItem;
                }
                currentItem = new ImportDataItem();
                itemStarted = true;
                continue;
            }

            if (trimmed.StartsWith("account_name:"))
                currentItem.AccountName = ExtractValue(trimmed);
            else if (trimmed.StartsWith("category_name:"))
                currentItem.CategoryName = ExtractValue(trimmed);
            else if (trimmed.StartsWith("category_type:"))
                currentItem.CategoryType = ExtractValue(trimmed) == "Income" ? CategoryType.Income : CategoryType.Expense;
            else if (trimmed.StartsWith("operation_type:"))
                currentItem.OperationType = ExtractValue(trimmed) == "Income" ? OperationType.Income : OperationType.Expense;
            else if (trimmed.StartsWith("amount:"))
                currentItem.Amount = decimal.Parse(ExtractValue(trimmed));
            else if (trimmed.StartsWith("date:"))
                currentItem.Date = DateTime.Parse(ExtractValue(trimmed));
            else if (trimmed.StartsWith("description:"))
                currentItem.Description = ExtractValue(trimmed);
        }

        if (itemStarted)
        {
            yield return currentItem;
        }
    }

    private string ExtractValue(string line)
    {
        var colonIndex = line.IndexOf(':');
        if (colonIndex == -1) return string.Empty;
        return line.Substring(colonIndex + 1).Trim();
    }
}


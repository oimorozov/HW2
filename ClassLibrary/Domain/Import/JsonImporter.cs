using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Enums;

namespace Domain.Import;

public class JsonImporter : ImporterBase
{
    public JsonImporter(Domain.DomainFactory.IDomainFactory factory) : base(factory)
    {
    }

    protected override IEnumerable<ImportDataItem> Parse(string content)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var jsonData = JsonSerializer.Deserialize<List<JsonOperationDto>>(content, options);
        if (jsonData == null)
            yield break;

        foreach (var item in jsonData)
        {
            yield return new ImportDataItem
            {
                AccountName = item.AccountName,
                CategoryName = item.CategoryName,
                CategoryType = item.CategoryType == "Income" ? CategoryType.Income : CategoryType.Expense,
                OperationType = item.OperationType == "Income" ? OperationType.Income : OperationType.Expense,
                Amount = item.Amount,
                Date = DateTime.Parse(item.Date),
                Description = item.Description
            };
        }
    }

    private class JsonOperationDto
    {
        [JsonPropertyName("account_name")]
        public string AccountName { get; set; } = string.Empty;
        
        [JsonPropertyName("category_name")]
        public string CategoryName { get; set; } = string.Empty;
        
        [JsonPropertyName("category_type")]
        public string CategoryType { get; set; } = string.Empty;
        
        [JsonPropertyName("operation_type")]
        public string OperationType { get; set; } = string.Empty;
        
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        
        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;
        
        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}


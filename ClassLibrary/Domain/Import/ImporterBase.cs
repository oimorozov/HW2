using Domain.DomainFactory;
using Domain.BankAccount;
using Domain.Category;
using Domain.Operation;
using Domain.Enums;

namespace Domain.Import;

public abstract class ImporterBase
{
    protected readonly IDomainFactory _factory;

    protected ImporterBase(IDomainFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }
    public ImportResult Import(string filePath, Func<string, Domain.BankAccount.BankAccount> getOrCreateAccount, Func<string, CategoryType, Domain.Category.Category> getOrCreateCategory)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        var fileContent = File.ReadAllText(filePath);
        var parsedData = Parse(fileContent);
        
        var result = new ImportResult();
        
        foreach (var item in parsedData)
        {
            try
            {
                ProcessItem(item, getOrCreateAccount, getOrCreateCategory, result);
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Error processing item: {ex.Message}");
            }
        }
        
        return result;
    }

    protected abstract IEnumerable<ImportDataItem> Parse(string content);

    private void ProcessItem(ImportDataItem data, Func<string, Domain.BankAccount.BankAccount> getOrCreateAccount, Func<string, CategoryType, Domain.Category.Category> getOrCreateCategory, ImportResult result)
    {
        var account = getOrCreateAccount(data.AccountName);

        var category = getOrCreateCategory(data.CategoryName, data.CategoryType);
        
        var operation = _factory.CreateOperation(
            data.OperationType,
            account,
            new Domain.Money.Money(data.Amount),
            data.Date,
            category,
            data.Description);

        result.ImportedOperations.Add(operation);
        result.SuccessCount++;
    }

    protected class ImportDataItem
    {
        public string AccountName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public CategoryType CategoryType { get; set; }
        public OperationType OperationType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
    }

    public class ImportResult
    {
        public List<Domain.Operation.Operation> ImportedOperations { get; } = new();
        public List<string> Errors { get; } = new();
        public int SuccessCount { get; set; }
    }
}


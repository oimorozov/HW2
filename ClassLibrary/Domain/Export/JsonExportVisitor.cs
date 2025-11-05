using Domain.BankAccount;
using Domain.Category;
using Domain.Operation;
using Domain.Enums;

namespace Domain.Export;

public interface IExportVisitor
{
    void Visit(Domain.BankAccount.BankAccount account);
    void Visit(Domain.Category.Category category);
    void Visit(Domain.Operation.Operation operation);
    string Build();
}

public class JsonExportVisitor : IExportVisitor
{
    private readonly List<object> _data = new();

    public void Visit(Domain.BankAccount.BankAccount account)
    {
        _data.Add(new
        {
            type = "account",
            id = account.Id,
            name = account.Name,
            balance = account.Balance.Value
        });
    }

    public void Visit(Domain.Category.Category category)
    {
        _data.Add(new
        {
            type = "category",
            id = category.Id,
            name = category.Name,
            category_type = category.Type.ToString()
        });
    }

    public void Visit(Domain.Operation.Operation operation)
    {
        _data.Add(new
        {
            type = "operation",
            id = operation.Id,
            operation_type = operation.Type.ToString(),
            account_id = operation.BankAccountId.Id,
            account_name = operation.BankAccountId.Name,
            category_id = operation.CategoryId.Id,
            category_name = operation.CategoryId.Name,
            amount = operation.Amount.Value,
            date = operation.Date.ToString("yyyy-MM-dd"),
            description = operation.Description
        });
    }

    public string Build()
    {
        return System.Text.Json.JsonSerializer.Serialize(_data, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
}


using Domain.BankAccount;
using Domain.Category;
using Domain.Operation;

namespace Domain.Export;

public class YamlExportVisitor : IExportVisitor
{
    private readonly System.Text.StringBuilder _yaml = new();
    private int _operationIndex = 0;

    public void Visit(Domain.BankAccount.BankAccount account)
    {
        _yaml.AppendLine("accounts:");
        _yaml.AppendLine($"  - id: {account.Id}");
        _yaml.AppendLine($"    name: {account.Name}");
        _yaml.AppendLine($"    balance: {account.Balance.Value}");
        _yaml.AppendLine();
    }

    public void Visit(Domain.Category.Category category)
    {
        if (!_yaml.ToString().Contains("categories:"))
        {
            _yaml.AppendLine("categories:");
        }
        _yaml.AppendLine($"  - id: {category.Id}");
        _yaml.AppendLine($"    name: {category.Name}");
        _yaml.AppendLine($"    type: {category.Type}");
        _yaml.AppendLine();
    }

    public void Visit(Domain.Operation.Operation operation)
    {
        if (_operationIndex == 0)
        {
            _yaml.AppendLine("operations:");
        }
        _yaml.AppendLine($"  - id: {operation.Id}");
        _yaml.AppendLine($"    operation_type: {operation.Type}");
        _yaml.AppendLine($"    account_id: {operation.BankAccountId.Id}");
        _yaml.AppendLine($"    account_name: {operation.BankAccountId.Name}");
        _yaml.AppendLine($"    category_id: {operation.CategoryId.Id}");
        _yaml.AppendLine($"    category_name: {operation.CategoryId.Name}");
        _yaml.AppendLine($"    amount: {operation.Amount.Value}");
        _yaml.AppendLine($"    date: {operation.Date:yyyy-MM-dd}");
        if (!string.IsNullOrEmpty(operation.Description))
        {
            _yaml.AppendLine($"    description: {operation.Description}");
        }
        _yaml.AppendLine();
        _operationIndex++;
    }

    public string Build()
    {
        return _yaml.ToString();
    }
}


using Domain.BankAccount;
using Domain.Category;
using Domain.Operation;

namespace Domain.Export;

public class CsvExportVisitor : IExportVisitor
{
    private readonly System.Text.StringBuilder _csv = new();
    private bool _headerWritten = false;

    public void Visit(Domain.BankAccount.BankAccount account)
    {
        if (!_headerWritten)
        {
            _csv.AppendLine("Type,Id,Name,Balance");
            _headerWritten = true;
        }
        _csv.AppendLine($"Account,{account.Id},{account.Name},{account.Balance.Value}");
    }

    public void Visit(Domain.Category.Category category)
    {
        if (!_headerWritten)
        {
            _csv.AppendLine("Type,Id,Name,CategoryType");
            _headerWritten = true;
        }
        _csv.AppendLine($"Category,{category.Id},{category.Name},{category.Type}");
    }

    public void Visit(Domain.Operation.Operation operation)
    {
        if (!_headerWritten)
        {
            _csv.AppendLine("Type,Id,OperationType,AccountId,AccountName,CategoryId,CategoryName,Amount,Date,Description");
            _headerWritten = true;
        }
        _csv.AppendLine($"Operation,{operation.Id},{operation.Type},{operation.BankAccountId.Id},{operation.BankAccountId.Name},{operation.CategoryId.Id},{operation.CategoryId.Name},{operation.Amount.Value},{operation.Date:yyyy-MM-dd},{operation.Description ?? ""}");
    }

    public string Build()
    {
        return _csv.ToString();
    }
}


using Domain.BankAccount;
using Domain.Category;
using Domain.Operation;

namespace Domain.Export;

public class ExportFacade
{
    private readonly Func<IEnumerable<Domain.BankAccount.BankAccount>> _getAccounts;
    private readonly Func<IEnumerable<Domain.Category.Category>> _getCategories;
    private readonly Func<IEnumerable<Domain.Operation.Operation>> _getOperations;

    public ExportFacade(
        Func<IEnumerable<Domain.BankAccount.BankAccount>> getAccounts,
        Func<IEnumerable<Domain.Category.Category>> getCategories,
        Func<IEnumerable<Domain.Operation.Operation>> getOperations)
    {
        _getAccounts = getAccounts ?? throw new ArgumentNullException(nameof(getAccounts));
        _getCategories = getCategories ?? throw new ArgumentNullException(nameof(getCategories));
        _getOperations = getOperations ?? throw new ArgumentNullException(nameof(getOperations));
    }

    public string Export(IExportVisitor visitor)
    {
        foreach (var account in _getAccounts())
        {
            visitor.Visit(account);
        }

        foreach (var category in _getCategories())
        {
            visitor.Visit(category);
        }

        foreach (var operation in _getOperations())
        {
            visitor.Visit(operation);
        }

        return visitor.Build();
    }

    public void ExportToFile(string filePath, IExportVisitor visitor)
    {
        var content = Export(visitor);
        File.WriteAllText(filePath, content);
    }
}


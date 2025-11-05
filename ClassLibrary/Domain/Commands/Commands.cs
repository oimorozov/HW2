using Domain.BankAccount;
using Domain.Category;
using Domain.Operation;
using Domain.Enums;

namespace Domain.Commands;

public class CreateAccountCommand : ICommand
{
    private readonly Domain.DomainFactory.IDomainFactory _factory;
    private readonly string _name;
    private readonly decimal _initialBalance;
    private readonly Action<Domain.BankAccount.BankAccount> _onCreated;

    public Domain.BankAccount.BankAccount? CreatedAccount { get; private set; }

    public CreateAccountCommand(
        Domain.DomainFactory.IDomainFactory factory,
        string name,
        decimal initialBalance,
        Action<Domain.BankAccount.BankAccount> onCreated)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _name = name;
        _initialBalance = initialBalance;
        _onCreated = onCreated ?? throw new ArgumentNullException(nameof(onCreated));
    }

    public void Execute()
    {
        CreatedAccount = _factory.CreateBankAccount(_name, _initialBalance);
        _onCreated(CreatedAccount);
    }
}

public class CreateCategoryCommand : ICommand
{
    private readonly Domain.DomainFactory.IDomainFactory _factory;
    private readonly string _name;
    private readonly CategoryType _type;
    private readonly Action<Domain.Category.Category> _onCreated;

    public Domain.Category.Category? CreatedCategory { get; private set; }

    public CreateCategoryCommand(
        Domain.DomainFactory.IDomainFactory factory,
        string name,
        CategoryType type,
        Action<Domain.Category.Category> onCreated)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _name = name;
        _type = type;
        _onCreated = onCreated ?? throw new ArgumentNullException(nameof(onCreated));
    }

    public void Execute()
    {
        CreatedCategory = _factory.CreateCategory(_name, _type);
        _onCreated(CreatedCategory);
    }
}

public class AddOperationCommand : ICommand
{
    private readonly Domain.DomainFactory.IDomainFactory _factory;
    private readonly OperationType _type;
    private readonly Domain.BankAccount.BankAccount _account;
    private readonly Domain.Category.Category _category;
    private readonly Domain.Money.Money _amount;
    private readonly DateTime _date;
    private readonly string? _description;
    private readonly Action<Domain.Operation.Operation> _onCreated;

    public Domain.Operation.Operation? CreatedOperation { get; private set; }

    public AddOperationCommand(
        Domain.DomainFactory.IDomainFactory factory,
        OperationType type,
        Domain.BankAccount.BankAccount account,
        Domain.Category.Category category,
        Domain.Money.Money amount,
        DateTime date,
        Action<Domain.Operation.Operation> onCreated,
        string? description = null)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _type = type;
        _account = account ?? throw new ArgumentNullException(nameof(account));
        _category = category ?? throw new ArgumentNullException(nameof(category));
        _amount = amount;
        _date = date;
        _description = description;
        _onCreated = onCreated ?? throw new ArgumentNullException(nameof(onCreated));
    }

    public void Execute()
    {
        CreatedOperation = _factory.CreateOperation(_type, _account, _amount, _date, _category, _description);
        _onCreated(CreatedOperation);
    }
}

public class RecalculateBalanceCommand : ICommand
{
    private readonly Domain.BankAccount.BankAccount _account;
    private readonly IEnumerable<Domain.Operation.Operation> _operations;
    private readonly Func<Guid, Domain.Category.Category> _getCategory;

    public RecalculateBalanceCommand(
        Domain.BankAccount.BankAccount account,
        IEnumerable<Domain.Operation.Operation> operations,
        Func<Guid, Domain.Category.Category> getCategory)
    {
        _account = account ?? throw new ArgumentNullException(nameof(account));
        _operations = operations ?? throw new ArgumentNullException(nameof(operations));
        _getCategory = getCategory ?? throw new ArgumentNullException(nameof(getCategory));
    }

    public void Execute()
    {
        var accountOperations = _operations
            .Where(o => o.BankAccountId.Id == _account.Id)
            .OrderBy(o => o.Date)
            .ToList();

        decimal balance = 0;
        foreach (var operation in accountOperations)
        {
            var category = _getCategory(operation.CategoryId.Id);
            
            if (operation.Type == OperationType.Income)
                balance += operation.Amount.Value;
            else
                balance -= operation.Amount.Value;
        }
    }
}


using Domain.BankAccount;
using Domain.Category;
using Domain.Operation;
using Domain.Commands;

namespace Domain.BalanceRecalculation;

public class RecalculateBalanceCommand : ICommand
{
    private readonly Domain.BankAccount.BankAccount _account;
    private readonly IEnumerable<Domain.Operation.Operation> _operations;
    private readonly Func<Guid, Domain.Category.Category> _getCategory;
    private readonly IBalanceRecalculationStrategy _strategy;

    public RecalculationResult? Result { get; private set; }

    public RecalculateBalanceCommand(
        Domain.BankAccount.BankAccount account,
        IEnumerable<Domain.Operation.Operation> operations,
        Func<Guid, Domain.Category.Category> getCategory,
        IBalanceRecalculationStrategy strategy)
    {
        _account = account ?? throw new ArgumentNullException(nameof(account));
        _operations = operations ?? throw new ArgumentNullException(nameof(operations));
        _getCategory = getCategory ?? throw new ArgumentNullException(nameof(getCategory));
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public void Execute()
    {
        Result = _strategy.Recalculate(_account, _operations, _getCategory);
    }
}

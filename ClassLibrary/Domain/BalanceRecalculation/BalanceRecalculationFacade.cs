using Domain.BankAccount;
using Domain.Category;
using Domain.Operation;
using Domain.Commands;

namespace Domain.BalanceRecalculation;

public class BalanceRecalculationFacade
{
    private readonly Func<IEnumerable<Domain.Operation.Operation>> _getOperations;
    private readonly Func<Guid, Domain.Category.Category> _getCategory;
    private readonly Action<Domain.BankAccount.BankAccount, decimal> _updateBalance;

    public BalanceRecalculationFacade(
        Func<IEnumerable<Domain.Operation.Operation>> getOperations,
        Func<Guid, Domain.Category.Category> getCategory,
        Action<Domain.BankAccount.BankAccount, decimal>? updateBalance = null)
    {
        _getOperations = getOperations ?? throw new ArgumentNullException(nameof(getOperations));
        _getCategory = getCategory ?? throw new ArgumentNullException(nameof(getCategory));
        _updateBalance = updateBalance ?? ((account, balance) => account.UpdateBalance(balance));
    }

    public RecalculationResult AutomaticRecalculate(Domain.BankAccount.BankAccount account)
    {
        var strategy = new AutomaticRecalculationStrategy();
        var operations = _getOperations();
        
        var command = new RecalculateBalanceCommand(account, operations, _getCategory, strategy);
        command.Execute();
        
        var result = command.Result!;
        
        if (result.IsConsistent || result.Errors.Count == 0)
        {
            _updateBalance(account, result.NewBalance);
        }
        
        return result;
    }

    public RecalculationResult ManualRecalculate(Domain.BankAccount.BankAccount account, decimal manualBalance)
    {
        var strategy = new ManualRecalculationStrategy(manualBalance);
        var operations = _getOperations();
        
        var command = new RecalculateBalanceCommand(account, operations, _getCategory, strategy);
        command.Execute();
        
        var result = command.Result!;
        
        _updateBalance(account, result.NewBalance);
        
        return result;
    }

    public RecalculationResult ValidateAndRecalculate(Domain.BankAccount.BankAccount account)
    {
        var strategy = new ValidationRecalculationStrategy();
        var operations = _getOperations();
        
        var command = new RecalculateBalanceCommand(account, operations, _getCategory, strategy);
        command.Execute();
        
        return command.Result!;
    }
    public RecalculationResult RecalculateWithStrategy(
        Domain.BankAccount.BankAccount account,
        IBalanceRecalculationStrategy strategy)
    {
        var operations = _getOperations();
        
        var command = new RecalculateBalanceCommand(account, operations, _getCategory, strategy);
        command.Execute();
        
        return command.Result!;
    }

    public bool CheckConsistency(Domain.BankAccount.BankAccount account)
    {
        var result = AutomaticRecalculate(account);
        return result.IsConsistent;
    }

    public decimal GetBalanceDifference(Domain.BankAccount.BankAccount account)
    {
        var strategy = new AutomaticRecalculationStrategy();
        var operations = _getOperations();
        
        var command = new RecalculateBalanceCommand(account, operations, _getCategory, strategy);
        command.Execute();
        
        return command.Result!.Difference;
    }
}


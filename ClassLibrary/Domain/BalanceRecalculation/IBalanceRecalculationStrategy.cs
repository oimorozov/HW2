using Domain.BankAccount;
using Domain.Category;
using Domain.Operation;
using Domain.Enums;

namespace Domain.BalanceRecalculation;

public interface IBalanceRecalculationStrategy
{
    RecalculationResult Recalculate(Domain.BankAccount.BankAccount account, IEnumerable<Domain.Operation.Operation> operations, Func<Guid, Domain.Category.Category> getCategory);

    string Name { get; }
}

public class RecalculationResult
{
    public decimal OldBalance { get; set; }
    public decimal NewBalance { get; set; }
    public decimal Difference { get; set; }
    public bool IsConsistent { get; set; }
    public List<string> Errors { get; } = new();
    public int OperationsProcessed { get; set; }
}

using Domain.Enums;

namespace Domain.Analytics;

public class BalanceDifferenceStrategy : IAnalyticsStrategy
{
    public string Name => "Разница доходов и расходов";

    public object Analyze(IEnumerable<Domain.Operation.Operation> operations)
    {
        var income = operations
            .Where(o => o.Type == OperationType.Income)
            .Sum(o => o.Amount.Value);
        
        var expense = operations
            .Where(o => o.Type == OperationType.Expense)
            .Sum(o => o.Amount.Value);
        
        return new BalanceDifferenceResult
        {
            Income = income,
            Expense = expense,
            Difference = income - expense
        };
    }

    public class BalanceDifferenceResult
    {
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal Difference { get; set; }
    }
}


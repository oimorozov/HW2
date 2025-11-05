using Domain.Enums;

namespace Domain.Analytics;

public class AverageAmountStrategy : IAnalyticsStrategy
{
    public string Name => "Средние суммы доходов и расходов";

    public object Analyze(IEnumerable<Domain.Operation.Operation> operations)
    {
        var incomeOperations = operations.Where(o => o.Type == OperationType.Income).ToList();
        var expenseOperations = operations.Where(o => o.Type == OperationType.Expense).ToList();

        return new AverageAmountResult
        {
            AverageIncome = incomeOperations.Any() 
                ? incomeOperations.Average(o => o.Amount.Value) 
                : 0,
            AverageExpense = expenseOperations.Any() 
                ? expenseOperations.Average(o => o.Amount.Value) 
                : 0,
            IncomeCount = incomeOperations.Count,
            ExpenseCount = expenseOperations.Count
        };
    }

    public class AverageAmountResult
    {
        public decimal AverageIncome { get; set; }
        public decimal AverageExpense { get; set; }
        public int IncomeCount { get; set; }
        public int ExpenseCount { get; set; }
    }
}


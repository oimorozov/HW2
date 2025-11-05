using Domain.Enums;

namespace Domain.Analytics;

public class CategoryGroupingStrategy : IAnalyticsStrategy
{
    public string Name => "Группировка по категориям";

    public object Analyze(IEnumerable<Domain.Operation.Operation> operations)
    {
        var grouped = operations
            .GroupBy(o => o.CategoryId.Name)
            .Select(g => new CategoryGroupResult
            {
                CategoryName = g.Key,
                Income = g.Where(o => o.Type == OperationType.Income).Sum(o => o.Amount.Value),
                Expense = g.Where(o => o.Type == OperationType.Expense).Sum(o => o.Amount.Value),
                Total = g.Where(o => o.Type == OperationType.Income).Sum(o => o.Amount.Value) -
                       g.Where(o => o.Type == OperationType.Expense).Sum(o => o.Amount.Value),
                OperationCount = g.Count()
            })
            .ToList();

        return grouped;
    }

    public class CategoryGroupResult
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal Total { get; set; }
        public int OperationCount { get; set; }
    }
}


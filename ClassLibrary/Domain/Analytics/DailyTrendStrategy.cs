using Domain.Enums;

namespace Domain.Analytics;

public class DailyTrendStrategy : IAnalyticsStrategy
{
    public string Name => "Динамика операций по дням";

    public object Analyze(IEnumerable<Domain.Operation.Operation> operations)
    {
        var dailyTrend = operations
            .GroupBy(o => o.Date.Date)
            .Select(g => new DailyTrendResult
            {
                Date = g.Key,
                Income = g.Where(o => o.Type == OperationType.Income).Sum(o => o.Amount.Value),
                Expense = g.Where(o => o.Type == OperationType.Expense).Sum(o => o.Amount.Value),
                NetAmount = g.Where(o => o.Type == OperationType.Income).Sum(o => o.Amount.Value) -
                           g.Where(o => o.Type == OperationType.Expense).Sum(o => o.Amount.Value),
                OperationCount = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToList();

        return dailyTrend;
    }

    public class DailyTrendResult
    {
        public DateTime Date { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal NetAmount { get; set; }
        public int OperationCount { get; set; }
    }
}


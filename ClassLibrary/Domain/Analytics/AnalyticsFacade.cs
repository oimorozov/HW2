using Domain.Enums;

namespace Domain.Analytics;
public class AnalyticsContext
{
    private readonly IAnalyticsStrategy _strategy;

    public AnalyticsContext(IAnalyticsStrategy strategy)
    {
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public object ExecuteAnalysis(IEnumerable<Domain.Operation.Operation> operations)
    {
        return _strategy.Analyze(operations);
    }

    public string GetStrategyName() => _strategy.Name;
}

public class AnalyticsFacade
{
    private readonly Func<IEnumerable<Domain.Operation.Operation>> _getOperations;

    public AnalyticsFacade(Func<IEnumerable<Domain.Operation.Operation>> getOperations)
    {
        _getOperations = getOperations ?? throw new ArgumentNullException(nameof(getOperations));
    }

    public object Analyze(IAnalyticsStrategy strategy, DateTime? startDate = null, DateTime? endDate = null)
    {
        var operations = _getOperations();
        
        if (startDate.HasValue || endDate.HasValue)
        {
            operations = operations.Where(o => 
                (!startDate.HasValue || o.Date >= startDate.Value) &&
                (!endDate.HasValue || o.Date <= endDate.Value));
        }

        var context = new AnalyticsContext(strategy);
        return context.ExecuteAnalysis(operations);
    }

    public BalanceDifferenceStrategy.BalanceDifferenceResult CalculateBalanceDifference(DateTime? startDate = null, DateTime? endDate = null)
    {
        var strategy = new BalanceDifferenceStrategy();
        return (BalanceDifferenceStrategy.BalanceDifferenceResult)Analyze(strategy, startDate, endDate);
    }

    public List<CategoryGroupingStrategy.CategoryGroupResult> GroupByCategories(DateTime? startDate = null, DateTime? endDate = null)
    {
        var strategy = new CategoryGroupingStrategy();
        return (List<CategoryGroupingStrategy.CategoryGroupResult>)Analyze(strategy, startDate, endDate);
    }

    public AverageAmountStrategy.AverageAmountResult GetAverageAmounts(DateTime? startDate = null, DateTime? endDate = null)
    {
        var strategy = new AverageAmountStrategy();
        return (AverageAmountStrategy.AverageAmountResult)Analyze(strategy, startDate, endDate);
    }

    public List<DailyTrendStrategy.DailyTrendResult> GetDailyTrend(DateTime? startDate = null, DateTime? endDate = null)
    {
        var strategy = new DailyTrendStrategy();
        return (List<DailyTrendStrategy.DailyTrendResult>)Analyze(strategy, startDate, endDate);
    }
}


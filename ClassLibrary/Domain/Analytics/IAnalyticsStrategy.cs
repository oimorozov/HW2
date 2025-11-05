using Domain.Enums;

namespace Domain.Analytics;
public interface IAnalyticsStrategy
{
    object Analyze(IEnumerable<Domain.Operation.Operation> operations);
    string Name { get; }
}


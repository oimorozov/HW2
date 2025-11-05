using System.Diagnostics;

namespace Domain.Commands;

public class CommandFacade
{
    private readonly Domain.DomainFactory.IDomainFactory _factory;
    private readonly List<TimeSpan> _executionTimes = new();

    public CommandFacade(Domain.DomainFactory.IDomainFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public void ExecuteTimed(ICommand command, Action<TimeSpan>? onTimeMeasured = null)
    {
        var timedCommand = new TimedCommand(command, elapsed =>
        {
            _executionTimes.Add(elapsed);
            onTimeMeasured?.Invoke(elapsed);
        });
        
        timedCommand.Execute();
    }

    public void ExecuteTimedAndLogged(ICommand command, Action<string> logger, Action<TimeSpan>? onTimeMeasured = null)
    {
        var loggedCommand = new LoggedCommand(command, logger);
        var timedCommand = new TimedCommand(loggedCommand, elapsed =>
        {
            _executionTimes.Add(elapsed);
            onTimeMeasured?.Invoke(elapsed);
        });
        
        timedCommand.Execute();
    }

    public ExecutionStatistics GetStatistics()
    {
        if (!_executionTimes.Any())
            return new ExecutionStatistics();

        return new ExecutionStatistics
        {
            TotalExecutions = _executionTimes.Count,
            TotalTime = TimeSpan.FromMilliseconds(_executionTimes.Sum(t => t.TotalMilliseconds)),
            AverageTime = TimeSpan.FromMilliseconds(_executionTimes.Average(t => t.TotalMilliseconds)),
            MinTime = _executionTimes.Min(),
            MaxTime = _executionTimes.Max()
        };
    }
    public void ClearStatistics()
    {
        _executionTimes.Clear();
    }

    public class ExecutionStatistics
    {
        public int TotalExecutions { get; set; }
        public TimeSpan TotalTime { get; set; }
        public TimeSpan AverageTime { get; set; }
        public TimeSpan MinTime { get; set; }
        public TimeSpan MaxTime { get; set; }
    }
}


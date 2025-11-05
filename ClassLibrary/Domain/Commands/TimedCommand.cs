using System.Diagnostics;

namespace Domain.Commands;

public class TimedCommand : ICommand
{
    private readonly ICommand _inner;
    private readonly Action<TimeSpan> _onComplete;

    public TimedCommand(ICommand inner, Action<TimeSpan> onComplete)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _onComplete = onComplete ?? throw new ArgumentNullException(nameof(onComplete));
    }

    public void Execute()
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            _inner.Execute();
        }
        finally
        {
            stopwatch.Stop();
            _onComplete(stopwatch.Elapsed);
        }
    }
}

public class LoggedCommand : ICommand
{
    private readonly ICommand _inner;
    private readonly Action<string> _logger;

    public LoggedCommand(ICommand inner, Action<string> logger)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Execute()
    {
        _logger($"Начало выполнения команды: {_inner.GetType().Name}");
        try
        {
            _inner.Execute();
            _logger($"Успешное завершение команды: {_inner.GetType().Name}");
        }
        catch (Exception ex)
        {
            _logger($"Ошибка выполнения команды {_inner.GetType().Name}: {ex.Message}");
            throw;
        }
    }
}


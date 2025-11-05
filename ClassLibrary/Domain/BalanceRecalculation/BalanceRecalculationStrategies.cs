using Domain.BankAccount;
using Domain.Category;
using Domain.Operation;
using Domain.Enums;

namespace Domain.BalanceRecalculation;

public class AutomaticRecalculationStrategy : IBalanceRecalculationStrategy
{
    public string Name => "Автоматический пересчет";

    public RecalculationResult Recalculate(
        Domain.BankAccount.BankAccount account,
        IEnumerable<Domain.Operation.Operation> operations,
        Func<Guid, Domain.Category.Category> getCategory)
    {
        var result = new RecalculationResult
        {
            OldBalance = account.Balance.Value
        };

        var accountOperations = operations
            .Where(o => o.BankAccountId.Id == account.Id)
            .OrderBy(o => o.Date)
            .ToList();

        decimal calculatedBalance = 0;
        
        foreach (var operation in accountOperations)
        {
            try
            {
                var category = getCategory(operation.CategoryId.Id);
                
                if ((operation.Type == OperationType.Income && category.Type != CategoryType.Income) ||
                    (operation.Type == OperationType.Expense && category.Type != CategoryType.Expense))
                {
                    result.Errors.Add($"Несоответствие типов операции {operation.Id} и категории {category.Id}");
                    continue;
                }

                if (operation.Type == OperationType.Income)
                    calculatedBalance += operation.Amount.Value;
                else
                    calculatedBalance -= operation.Amount.Value;

                result.OperationsProcessed++;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Ошибка обработки операции {operation.Id}: {ex.Message}");
            }
        }

        result.NewBalance = calculatedBalance;
        result.Difference = calculatedBalance - result.OldBalance;
        result.IsConsistent = Math.Abs(result.Difference) < 0.01m;

        return result;
    }
}

public class ManualRecalculationStrategy : IBalanceRecalculationStrategy
{
    private readonly decimal _manualBalance;

    public ManualRecalculationStrategy(decimal manualBalance)
    {
        _manualBalance = manualBalance;
    }

    public string Name => "Ручной пересчет";

    public RecalculationResult Recalculate(
        Domain.BankAccount.BankAccount account,
        IEnumerable<Domain.Operation.Operation> operations,
        Func<Guid, Domain.Category.Category> getCategory)
    {
        var result = new RecalculationResult
        {
            OldBalance = account.Balance.Value,
            NewBalance = _manualBalance,
            Difference = _manualBalance - account.Balance.Value,
            IsConsistent = Math.Abs(_manualBalance - account.Balance.Value) < 0.01m,
            OperationsProcessed = operations.Count(o => o.BankAccountId.Id == account.Id)
        };

        var accountOperations = operations
            .Where(o => o.BankAccountId.Id == account.Id)
            .ToList();

        decimal calculatedBalance = 0;
        foreach (var operation in accountOperations)
        {
            try
            {
                var category = getCategory(operation.CategoryId.Id);
                
                if ((operation.Type == OperationType.Income && category.Type != CategoryType.Income) ||
                    (operation.Type == OperationType.Expense && category.Type != CategoryType.Expense))
                {
                    result.Errors.Add($"Несоответствие типов операции {operation.Id} и категории {category.Id}");
                }

                if (operation.Type == OperationType.Income)
                    calculatedBalance += operation.Amount.Value;
                else
                    calculatedBalance -= operation.Amount.Value;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Ошибка обработки операции {operation.Id}: {ex.Message}");
            }
        }

        if (Math.Abs(calculatedBalance - _manualBalance) > 0.01m)
        {
            result.Errors.Add($"Внимание: Ручной баланс ({_manualBalance}) не совпадает с рассчитанным ({calculatedBalance:F2})");
        }

        return result;
    }
}

public class ValidationRecalculationStrategy : IBalanceRecalculationStrategy
{
    public string Name => "Пересчет с валидацией";

    public RecalculationResult Recalculate(
        Domain.BankAccount.BankAccount account,
        IEnumerable<Domain.Operation.Operation> operations,
        Func<Guid, Domain.Category.Category> getCategory)
    {
        var result = new RecalculationResult
        {
            OldBalance = account.Balance.Value
        };

        var accountOperations = operations
            .Where(o => o.BankAccountId.Id == account.Id)
            .OrderBy(o => o.Date)
            .ToList();

        decimal calculatedBalance = 0;
        var processedOperations = new HashSet<Guid>();

        foreach (var operation in accountOperations)
        {
            try
            {
                if (processedOperations.Contains(operation.Id))
                {
                    result.Errors.Add($"Дубликат операции {operation.Id}");
                    continue;
                }
                processedOperations.Add(operation.Id);

                var category = getCategory(operation.CategoryId.Id);
                
                if ((operation.Type == OperationType.Income && category.Type != CategoryType.Income) ||
                    (operation.Type == OperationType.Expense && category.Type != CategoryType.Expense))
                {
                    result.Errors.Add($"Несоответствие типов операции {operation.Id} и категории {category.Id}");
                    continue;
                }

                if (operation.Amount.Value <= 0)
                {
                    result.Errors.Add($"Операция {operation.Id} имеет неположительную сумму");
                    continue;
                }

                if (operation.Type == OperationType.Income)
                    calculatedBalance += operation.Amount.Value;
                else
                {
                    calculatedBalance -= operation.Amount.Value;

                    if (calculatedBalance < 0)
                    {
                        result.Errors.Add($"После операции {operation.Id} баланс становится отрицательным: {calculatedBalance:F2}");
                    }
                }

                result.OperationsProcessed++;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Ошибка обработки операции {operation.Id}: {ex.Message}");
            }
        }

        result.NewBalance = calculatedBalance;
        result.Difference = calculatedBalance - result.OldBalance;
        result.IsConsistent = Math.Abs(result.Difference) < 0.01m && result.Errors.Count == 0;

        return result;
    }
}


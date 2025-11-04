using Domain.Enums;

namespace Domain.DomainFactory;

public interface IDomainFactory
{
    Domain.BankAccount.BankAccount CreateBankAccount(string name, decimal value = 0);
    Domain.Category.Category CreateCategory(string name, CategoryType type);
    Domain.Operation.Operation CreateOperation(OperationType type, Domain.BankAccount.BankAccount bankAccountId, Domain.Money.Money amount, DateTime date, Domain.Category.Category categoryId, string? description = null);
}

public class DomainFactory : IDomainFactory
{
    public Domain.BankAccount.BankAccount CreateBankAccount(string name, decimal value = 0)
    {
        return new Domain.BankAccount.BankAccount(name, value);
    }
    public Domain.Category.Category CreateCategory(string name, CategoryType type)
    {
        return new Domain.Category.Category(name, type);
    }
    public Domain.Operation.Operation CreateOperation(OperationType type, Domain.BankAccount.BankAccount bankAccountId, Domain.Money.Money amount, DateTime date, Domain.Category.Category categoryId, string? description = null)
    {
        return new Domain.Operation.Operation(type, bankAccountId, amount, date, categoryId, description);
    }
}
using Domain.Enums;

namespace Domain.Operation;

public class Operation
{
    public Guid Id { get; private set;}
    public OperationType Type { get; private set;}
    public Domain.BankAccount.BankAccount BankAccountId { get; private set;}
    public Domain.Money.Money Amount { get; private set;}
    public DateTime Date { get; private set;}
    public string? Description { get; private set;}
    public Domain.Category.Category CategoryId { get; private set;}
    public Operation(OperationType type, Domain.BankAccount.BankAccount bankAccountId, Domain.Money.Money amount, DateTime date, Domain.Category.Category categoryId, string? description = null)
    {
        Id = Guid.NewGuid();
        Type = type;
        BankAccountId = bankAccountId;
        Amount = amount;
        Date = date;
        Description = description;
        CategoryId = categoryId;
    }

}
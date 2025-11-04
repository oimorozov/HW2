namespace Domain.BankAccount;

public class BankAccount
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Domain.Money.Money Balance { get; private set; }
    public BankAccount(string name, decimal value = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Bank account name cannot be empty.", nameof(name));
        if (value < 0)
            throw new ArgumentException("Initial balance cannot be negative.", nameof(value));
        Id = Guid.NewGuid();
        Name = name;
        Balance = new Domain.Money.Money(value);
    }

    public void UpdateBalance(decimal newBalance)
    {
        if (newBalance < 0)
            throw new ArgumentException("Balance cannot be negative.", nameof(newBalance));
        Balance = new Domain.Money.Money(newBalance);
    }
}

namespace Domain.Money;

public struct Money
{
    private decimal _value;
    
    public decimal Value => _value;
    
    public Money(decimal value)
    {
        _value = value;
    }
}
using Domain.Enums;

namespace Domain.Category;

public class Category
{
    public Guid Id{ get; private set;}
    public string Name { get; private set;}
    public CategoryType Type { get; private set;}
    public Category(string name, CategoryType type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty.", nameof(name));
        Id = Guid.NewGuid();
        Name = name;
        Type = type;
    }
}
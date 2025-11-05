using Domain.DomainFactory;
using Domain.BankAccount;
using Domain.Category;
using Domain.Operation;
using Domain.Enums;
using Domain.Commands;
using Domain.Analytics;
using Domain.Import;
using Domain.Export;
using Domain.BalanceRecalculation;

namespace ConsoleApp;

class Program
{
    private static List<BankAccount> accounts = new();
    private static List<Category> categories = new();
    private static List<Operation> operations = new();

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Модуль учета финансов\n");

        var container = new DIContainer();
        ConfigureServices(container);
        
        var factory = container.Get<IDomainFactory>();
        var commandFacade = container.Get<CommandFacade>();
        var analyticsFacade = container.Get<AnalyticsFacade>();
        var exportFacade = container.Get<ExportFacade>();
        
        Console.WriteLine("1. Создание счетов и категорий:");
        Console.WriteLine("-----------------------------------");
        
        var createAccountCommand = new CreateAccountCommand(factory, "Основной счет", 1000, account => accounts.Add(account));
        commandFacade.ExecuteTimed(createAccountCommand, elapsed => 
            Console.WriteLine($"   Время выполнения: {elapsed.TotalMilliseconds:F2} мс"));
        
        var account = createAccountCommand.CreatedAccount!;
        Console.WriteLine($"   Создан счет: {account.Name}, Баланс: {account.Balance.Value:F2} руб.");

        var salaryCategory = new CreateCategoryCommand(factory, "Зарплата", CategoryType.Income, cat => categories.Add(cat));
        salaryCategory.Execute();
        var cafeCategory = new CreateCategoryCommand(factory, "Кафе", CategoryType.Expense, cat => categories.Add(cat));
        cafeCategory.Execute();
        var healthCategory = new CreateCategoryCommand(factory, "Здоровье", CategoryType.Expense, cat => categories.Add(cat));
        healthCategory.Execute();
        
        Console.WriteLine($"   Созданы категории: Зарплата, Кафе, Здоровье\n");

        Console.WriteLine("2. Добавление операций:");
        Console.WriteLine("-----------------------------------");
        
        var addIncomeCommand = new AddOperationCommand(
            factory,
            OperationType.Income,
            account,
            categories[0],
            new Domain.Money.Money(50000),
            DateTime.Now.AddDays(-20),
            op => operations.Add(op),
            "Зарплата за март");
        commandFacade.ExecuteTimed(addIncomeCommand, elapsed =>
            Console.WriteLine($"   Время выполнения: {elapsed.TotalMilliseconds:F2} мс"));
        Console.WriteLine($"   Добавлен доход: Зарплата за март - 50000 руб.");

        var op1 = new AddOperationCommand(factory, OperationType.Expense, account, categories[1], new Domain.Money.Money(500), DateTime.Now.AddDays(-15), op => operations.Add(op), "Обед в кафе");
        op1.Execute();
        Console.WriteLine($"   Добавлен расход: Обед в кафе - 500 руб.");
        
        var op2 = new AddOperationCommand(factory, OperationType.Expense, account, categories[1], new Domain.Money.Money(300), DateTime.Now.AddDays(-10), op => operations.Add(op), "Кофе");
        op2.Execute();
        Console.WriteLine($"   Добавлен расход: Кофе - 300 руб.");
        
        var op3 = new AddOperationCommand(factory, OperationType.Expense, account, categories[2], new Domain.Money.Money(1500), DateTime.Now.AddDays(-5), op => operations.Add(op), "Визит к врачу");
        op3.Execute();
        Console.WriteLine($"   Добавлен расход: Визит к врачу - 1500 руб.\n");

        Console.WriteLine($"3. Текущий баланс счета: {account.Balance.Value:F2} руб.\n");

        Console.WriteLine("4. Аналитика за последние 30 дней:");
        Console.WriteLine("-----------------------------------");
        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now;
        
        var balanceDiff = analyticsFacade.CalculateBalanceDifference(startDate, endDate);
        Console.WriteLine($"   Разница доходов и расходов: {balanceDiff.Difference:F2} руб.");
        Console.WriteLine($"   Всего доходов: {balanceDiff.Income:F2} руб.");
        Console.WriteLine($"   Всего расходов: {balanceDiff.Expense:F2} руб.");

        var grouped = analyticsFacade.GroupByCategories(startDate, endDate);
        Console.WriteLine("\n   Группировка по категориям:");
        foreach (var group in grouped)
        {
            Console.WriteLine($"     {group.CategoryName}: {group.Total:F2} руб. (операций: {group.OperationCount})");
        }
        Console.WriteLine();

        Console.WriteLine("5. Экспорт данных:");
        Console.WriteLine("-----------------------------------");
        var jsonVisitor = new JsonExportVisitor();
        exportFacade.ExportToFile("export.json", jsonVisitor);
        Console.WriteLine("   Данные экспортированы в export.json");
        
        var csvVisitor = new CsvExportVisitor();
        exportFacade.ExportToFile("export.csv", csvVisitor);
        Console.WriteLine("   Данные экспортированы в export.csv\n");

        Console.WriteLine("6. Пересчет баланса:");
        Console.WriteLine("-----------------------------------");
        var recalculationFacade = container.Get<BalanceRecalculationFacade>();
        var recalcResult = recalculationFacade.AutomaticRecalculate(account);
        Console.WriteLine($"   Старый баланс: {recalcResult.OldBalance:F2} руб.");
        Console.WriteLine($"   Новый баланс: {recalcResult.NewBalance:F2} руб.");
        Console.WriteLine($"   Разница: {recalcResult.Difference:F2} руб.");
        Console.WriteLine($"   Консистентен: {recalcResult.IsConsistent}");
        Console.WriteLine($"   Обработано операций: {recalcResult.OperationsProcessed}\n");
    }

    static void ConfigureServices(DIContainer container)
    {
        var factory = new DomainFactory();
        container.Register<IDomainFactory>(factory);
        container.Register<CommandFacade>(new CommandFacade(factory));
        
        container.Register<AnalyticsFacade>(new AnalyticsFacade(() => operations));
        container.Register<ExportFacade>(new ExportFacade(
            () => accounts,
            () => categories,
            () => operations));
        container.Register<BalanceRecalculationFacade>(new BalanceRecalculationFacade(
            () => operations,
            id => categories.First(c => c.Id == id)));
    }
}

class DIContainer
{
    private readonly Dictionary<Type, object> _services = new();

    public void Register<T>(T instance)
    {
        _services[typeof(T)] = instance!;
    }

    public T Get<T>()
    {
        if (_services.TryGetValue(typeof(T), out var service))
            return (T)service;
        throw new InvalidOperationException($"Service {typeof(T).Name} not registered");
    }
}

# HW2

**Фабричный метод**
- `ClassLibrary/domain/DomainFactory.cs`: `IDomainFactory`, `DomainFactory`

**Фасад**
- `ClassLibrary/domain/Analytics/AnalyticsFacade.cs`: `AnalyticsFacade`
- `ClassLibrary/domain/Export/ExportFacade.cs`: `ExportFacade`
- `ClassLibrary/domain/Commands/CommandFacade.cs`: `CommandFacade`
- `ClassLibrary/domain/BalanceRecalculation/BalanceRecalculationFacade.cs`: `BalanceRecalculationFacade`

**Декоратор**
- `ClassLibrary/domain/Commands/TimedCommand.cs`: `TimedCommand`
- `ClassLibrary/domain/Commands/TimedCommand.cs`: `LoggedCommand`

**Посетитель**
- `ClassLibrary/domain/Export/IExportVisitor.cs`: `IExportVisitor`
- `ClassLibrary/domain/Export/JsonExportVisitor.cs`: `JsonExportVisitor`
- `ClassLibrary/domain/Export/CsvExportVisitor.cs`: `CsvExportVisitor`
- `ClassLibrary/domain/Export/YamlExportVisitor.cs`: `YamlExportVisitor`

**Стратегия**
- `ClassLibrary/domain/Analytics/IAnalyticsStrategy.cs`: `IAnalyticsStrategy`
- `ClassLibrary/domain/Analytics/BalanceDifferenceStrategy.cs`: `BalanceDifferenceStrategy`
- `ClassLibrary/domain/Analytics/CategoryGroupingStrategy.cs`: `CategoryGroupingStrategy`
- `ClassLibrary/domain/Analytics/AverageAmountStrategy.cs`: `AverageAmountStrategy`
- `ClassLibrary/domain/Analytics/DailyTrendStrategy.cs`: `DailyTrendStrategy`
- `ClassLibrary/domain/Analytics/AnalyticsFacade.cs`: `AnalyticsContext`
- `ClassLibrary/domain/BalanceRecalculation/IBalanceRecalculationStrategy.cs`: `IBalanceRecalculationStrategy`
- `ClassLibrary/domain/BalanceRecalculation/BalanceRecalculationStrategies.cs`: `AutomaticRecalculationStrategy`, `ManualRecalculationStrategy`, `ValidationRecalculationStrategy`

**Команда**
- `ClassLibrary/domain/Commands/ICommand.cs`: `ICommand`
- `ClassLibrary/domain/Commands/Commands.cs`: `CreateAccountCommand`, `CreateCategoryCommand`, `AddOperationCommand`
- `ClassLibrary/domain/BalanceRecalculation/RecalculateBalanceCommand.cs`: `RecalculateBalanceCommand`

**Шаблонный метод**
- `ClassLibrary/domain/Import/ImporterBase.cs`: `ImporterBase` - `Import` и абстрактным методом `Parse`

**Единая ответственность**
  - `DomainFactory.cs`: создание доменных объектов
  - `BankAccount.cs`: представление банковского счета
  - `Category.cs`: представление категории 
  - и тд

**Open/Closed**
  - Пример: `IAnalyticsStrategy` - можно добавить новый функционал

**Лисков**
- Пример: Все реализации `IAnalyticsStrategy` взаимозаменяемы в `AnalyticsFacade`

**Интерфейсы**
  - Пример: `IDomainFactory` - только создание объектов

**DIP**
  - `CommandFacade` зависит от `IDomainFactory`, а не от конкретной реализации и тд

**Высокая связность**
  - `BankAccount.cs`: свойства и методы счета логически связаны
  - `Category.cs`: свойства категории логически связаны
  - `Operation.cs`: свойства операции логически связаны


**Низкая связанность**
  - Доменные модели (`BankAccount`, `Category`, `Operation`) не зависят от фасадов
  - Фасады зависят от абстракций (интерфейсов), а не от конкретных реализаций

**Создатель**
- `DomainFactory` создает доменные объекты (`BankAccount`, `Category`, `Operation`)
- Команды создают объекты через `IDomainFactory`
- Импортеры создают операции через `IDomainFactory`

**DI-контейнер**
- `ConsoleApp/Program.cs`: `DIContainer` - DI-контейнер для регистрации и получения сервисов
- `ConsoleApp/Program.cs`: `ConfigureServices` - настройка зависимостей

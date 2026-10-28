namespace EmployeesApp;

public class Manager : Employee
{
    public const double BonusPerSubordinate = 2000;

    private int _subordinatesCount;

    public int SubordinatesCount
    {
        get => _subordinatesCount;
        set => _subordinatesCount = value >= 0
            ? value
            : throw new ArgumentException("Кількість підлеглих не може бути від'ємною.");
    }

    public Manager(string fullName, double baseSalary, int subordinatesCount)
        : base(fullName, baseSalary)
    {
        SubordinatesCount = subordinatesCount;
    }

    public override string Position => "Менеджер";

    public override double CalculateSalary() => BaseSalary + SubordinatesCount * BonusPerSubordinate;

    protected override string GetDetails() => $"підлеглих: {SubordinatesCount}";
}

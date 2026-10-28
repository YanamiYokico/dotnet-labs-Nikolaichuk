namespace EmployeesApp;

public class Developer : Employee
{
    public DeveloperLevel Level { get; set; }

    public Developer(string fullName, double baseSalary, DeveloperLevel level)
        : base(fullName, baseSalary)
    {
        Level = level;
    }

    // Перевантажений конструктор: базова ставка за замовчуванням
    public Developer(string fullName, DeveloperLevel level)
        : this(fullName, DefaultBaseSalary, level)
    {
    }

    // Фіксована надбавка залежно від рівня
    public double LevelBonus => Level switch
    {
        DeveloperLevel.Junior => 0,
        DeveloperLevel.Middle => 5000,
        DeveloperLevel.Senior => 10000,
        _ => 0
    };

    public override string Position => "Розробник";

    public override double CalculateSalary() => BaseSalary + LevelBonus;

    protected override string GetDetails() => $"рівень: {Level}";
}

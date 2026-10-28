namespace EmployeesApp;

// Базовий клас співробітника
public class Employee : IComparable<Employee>
{
    public const double DefaultBaseSalary = 20000;

    private string _fullName = "";
    private double _baseSalary;

    public string FullName
    {
        get => _fullName;
        set => _fullName = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("ПІБ співробітника не може бути порожнім.")
            : value.Trim();
    }

    // Валідація в set: некоректне значення не потрапить в об'єкт жодним шляхом
    public double BaseSalary
    {
        get => _baseSalary;
        set => _baseSalary = value > 0
            ? value
            : throw new InvalidSalaryException(
                $"Базова ставка має бути додатною (отримано {value:F2}).");
    }

    public Employee(string fullName, double baseSalary)
    {
        FullName = fullName;
        BaseSalary = baseSalary;
    }

    // Перевантажений конструктор: базова ставка за замовчуванням
    public Employee(string fullName) : this(fullName, DefaultBaseSalary)
    {
    }

    public virtual string Position => "Співробітник";

    // Базова реалізація: зарплата дорівнює базовій ставці
    public virtual double CalculateSalary() => BaseSalary;

    // Додаткова інформація, специфічна для посади
    protected virtual string GetDetails() => "-";

    public virtual void PrintInfo()
    {
        Console.WriteLine($"{FullName,-22} {Position,-12} {BaseSalary,10:F2} "
                          + $"{GetDetails(),-18} {CalculateSalary(),10:F2}");
    }

    // Природний порядок — за фактичною зарплатою, а не за базовою ставкою
    public int CompareTo(Employee? other)
    {
        if (other is null)
        {
            return 1;
        }

        return CalculateSalary().CompareTo(other.CalculateSalary());
    }
}

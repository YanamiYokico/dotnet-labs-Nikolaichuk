using System.Globalization;
using System.Text;

namespace EmployeesApp;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        // Колекція базового типу, що фактично зберігає похідні об'єкти (заповнена наперед у коді)
        List<Employee> employees = new()
        {
            new Manager("Коваленко Олег", 30000, 4),
            new Developer("Шевчук Анна", 25000, DeveloperLevel.Senior),
            new Developer("Бондар Ігор", DeveloperLevel.Junior),
            new Employee("Мельник Світлана", 18000),
            new Developer("Ткачук Максим", 24000, DeveloperLevel.Middle)
        };

        bool isRunning = true;

        while (isRunning)
        {
            PrintMenu();
            int choice = ReadInt("Ваш вибір: ", 0, 5);

            isRunning = choice switch
            {
                1 => HandleAddEmployee(employees),
                2 => HandleShowAll(employees),
                3 => HandleShowPayroll(employees),
                4 => HandleShowSorted(employees),
                5 => HandleChangeBaseSalary(employees),
                0 => false,
                _ => true // недосяжно: ReadInt обмежує ввід діапазоном 0-5
            };
        }

        Console.WriteLine("Роботу завершено.");
    }

    static void PrintMenu()
    {
        Console.WriteLine("\n=== Співробітники компанії ===");
        Console.WriteLine("0. Вихід");
        Console.WriteLine("1. Додати співробітника");
        Console.WriteLine("2. Показати список співробітників із зарплатою");
        Console.WriteLine("3. Показати сумарний фонд заробітної плати");
        Console.WriteLine("4. Показати співробітників за зарплатою (від більшої до меншої)");
        Console.WriteLine("5. Змінити базову ставку співробітника");
    }

    // === Методи для пунктів меню ===

    static bool HandleAddEmployee(List<Employee> employees)
    {
        Console.WriteLine("Посада: 1 - співробітник, 2 - менеджер, 3 - розробник");
        int position = ReadInt("Оберіть посаду: ", 1, 3);
        string fullName = ReadText("ПІБ: ");
        double baseSalary = ReadDouble("Базова ставка, грн: ");

        try
        {
            Employee employee = position switch
            {
                2 => new Manager(fullName, baseSalary, ReadInt("Кількість підлеглих: ")),
                3 => new Developer(fullName, baseSalary, ReadLevel()),
                _ => new Employee(fullName, baseSalary)
            };

            employees.Add(employee);
            Console.WriteLine($"Додано: {employee.FullName} ({employee.Position}), "
                              + $"зарплата {employee.CalculateSalary():F2} грн.");
        }
        catch (InvalidSalaryException ex)
        {
            Console.WriteLine($"Помилка зарплати: {ex.Message} Співробітника не додано.");
        }
        catch (ArgumentException ex) // порожнє ПІБ або від'ємна кількість підлеглих
        {
            Console.WriteLine($"Помилка вхідних даних: {ex.Message} Співробітника не додано.");
        }

        return true;
    }

    static bool HandleShowAll(List<Employee> employees)
    {
        if (employees.Count == 0)
        {
            Console.WriteLine("Немає даних.");
            return true;
        }

        PrintTable(employees);
        return true;
    }

    static bool HandleShowPayroll(List<Employee> employees)
    {
        double total = 0;

        // Поліморфізм: для кожного об'єкта викликається власна реалізація CalculateSalary()
        foreach (Employee employee in employees)
        {
            total += employee.CalculateSalary();
        }

        Console.WriteLine($"Співробітників: {employees.Count}, "
                          + $"фонд заробітної плати: {total:F2} грн.");
        return true;
    }

    static bool HandleShowSorted(List<Employee> employees)
    {
        if (employees.Count == 0)
        {
            Console.WriteLine("Немає даних.");
            return true;
        }

        List<Employee> sorted = new(employees);
        // За спаданням зарплати (CompareTo з IComparable<Employee>)
        sorted.Sort((a, b) => b.CompareTo(a));
        PrintTable(sorted);
        return true;
    }

    // Граничний випадок: спроба встановити від'ємну або нульову ставку через властивість
    static bool HandleChangeBaseSalary(List<Employee> employees)
    {
        if (employees.Count == 0)
        {
            Console.WriteLine("Немає даних.");
            return true;
        }

        for (int i = 0; i < employees.Count; i++)
        {
            Employee e = employees[i];
            Console.WriteLine($"{i + 1}. {e.FullName} — ставка {e.BaseSalary:F2} грн");
        }

        int index = ReadInt("Номер співробітника: ", 1, employees.Count) - 1;
        double newSalary = ReadDouble("Нова базова ставка, грн: ");
        Employee selected = employees[index];

        try
        {
            selected.BaseSalary = newSalary;
            Console.WriteLine("Ставку змінено.");
        }
        catch (InvalidSalaryException ex)
        {
            Console.WriteLine($"Помилка зарплати: {ex.Message} Ставку не змінено.");
        }
        finally
        {
            Console.WriteLine($"Поточна ставка {selected.FullName}: {selected.BaseSalary:F2} грн.");
        }

        return true;
    }

    static void PrintTable(List<Employee> employees)
    {
        Console.WriteLine($"{"ПІБ",-22} {"Посада",-12} {"Ставка",10} {"Деталі",-18} {"Зарплата",10}");

        foreach (Employee employee in employees)
        {
            employee.PrintInfo();
        }
    }

    // === Допоміжні методи введення з валідацією ===

    static DeveloperLevel ReadLevel()
    {
        int level = ReadInt("Рівень (1 - Junior, 2 - Middle, 3 - Senior): ", 1, 3);
        return (DeveloperLevel)(level - 1);
    }

    static string ReadText(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? "";
    }

    static double ReadDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim().Replace(',', '.');

            bool isNumber = double.TryParse(input, NumberStyles.Float,
                                            CultureInfo.InvariantCulture, out double value);

            if (isNumber && double.IsFinite(value))
            {
                return value;
            }

            Console.WriteLine("Некоректне значення. Введіть число.");
        }
    }

    static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int value) && value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine($"Некоректне значення. Введіть ціле число від {min} до {max}.");
        }
    }
}

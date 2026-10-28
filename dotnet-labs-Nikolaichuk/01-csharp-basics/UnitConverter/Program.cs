using System.Globalization;
using System.Text;

namespace UnitConverter;

class Program
{
    // Константи перерахунку (використовуються і в прямій, і в зворотній формулі)
    const double KmPerMile = 1.609344;
    const double KgPerPound = 0.45359237;

    // Абсолютний нуль — нижня межа допустимої температури
    const double AbsoluteZeroCelsius = -273.15;
    const double AbsoluteZeroFahrenheit = -459.67;

    // Тексти попереджень для граничних випадків
    const string DistanceWarning = "відстань не може бути від'ємною";
    const string MassWarning = "маса не може бути від'ємною";
    const string CelsiusWarning = "температура не може бути нижчою за -273.15 °C";
    const string FahrenheitWarning = "температура не може бути нижчою за -459.67 °F";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        // Крапка як десятковий роздільник у виводі незалежно від мови системи
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        bool isRunning = true;

        while (isRunning)
        {
            PrintMenu();
            int choice = ReadInt("Ваш вибір: ", 0, 6);

            isRunning = choice switch
            {
                1 => HandleConversion("км", "милі", KmToMiles, 0, DistanceWarning),
                2 => HandleConversion("милі", "км", MilesToKm, 0, DistanceWarning),
                3 => HandleConversion("кг", "фунти", KgToPounds, 0, MassWarning),
                4 => HandleConversion("фунти", "кг", PoundsToKg, 0, MassWarning),
                5 => HandleConversion("°C", "°F", CelsiusToFahrenheit,
                                      AbsoluteZeroCelsius, CelsiusWarning),
                6 => HandleConversion("°F", "°C", FahrenheitToCelsius,
                                      AbsoluteZeroFahrenheit, FahrenheitWarning),
                0 => false,
                _ => true // недосяжно: ReadInt обмежує ввід діапазоном 0-6
            };
        }

        Console.WriteLine("Роботу завершено.");
    }

    static void PrintMenu()
    {
        Console.WriteLine("\n=== Конвертер величин ===");
        Console.WriteLine("0. Вихід");
        Console.WriteLine("1. км -> милі");
        Console.WriteLine("2. милі -> км");
        Console.WriteLine("3. кг -> фунти");
        Console.WriteLine("4. фунти -> кг");
        Console.WriteLine("5. °C -> °F");
        Console.WriteLine("6. °F -> °C");
    }

    // Один пункт меню: введення значення, перевірка граничного випадку, конвертація та вивід
    static bool HandleConversion(string fromUnit, string toUnit, Func<double, double> convert,
                                 double minValue, string warning)
    {
        double value = ReadDouble($"Введіть значення ({fromUnit}): ");

        if (value < minValue)
        {
            Console.WriteLine($"Попередження: {warning}. Конвертацію не виконано.");
            return true;
        }

        double result = convert(value);
        Console.WriteLine($"{value} {fromUnit} = {result:F2} {toUnit}");
        return true;
    }

    // === Методи конвертації: параметр — вхідне значення, повернене значення — результат ===

    static double KmToMiles(double km) => km / KmPerMile;

    static double MilesToKm(double miles) => miles * KmPerMile;

    static double KgToPounds(double kg) => kg / KgPerPound;

    static double PoundsToKg(double pounds) => pounds * KgPerPound;

    static double CelsiusToFahrenheit(double celsius) => celsius * 9 / 5 + 32;

    static double FahrenheitToCelsius(double fahrenheit) => (fahrenheit - 32) * 5 / 9;

    // === Допоміжні методи введення з валідацією ===

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

            Console.WriteLine("Некоректне значення. Введіть число (наприклад, 12.5).");
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

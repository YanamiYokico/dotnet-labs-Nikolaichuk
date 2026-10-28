namespace EmployeesApp;

// Власний виняток: некоректне значення зарплати (від'ємне або нульове)
public class InvalidSalaryException : Exception
{
    public InvalidSalaryException() { }

    public InvalidSalaryException(string message) : base(message) { }

    public InvalidSalaryException(string message, Exception inner) : base(message, inner) { }
}

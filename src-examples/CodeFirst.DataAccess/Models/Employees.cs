namespace CodeFirst.DataAccess.Models;

public class Employees
{
    public int EmployeeId { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public float? Salary { get; set; }        // yeah, salary might be nullable, why not :)
    public string City { get; set; }

    public Employees()
    {
    }

    public Employees(int employeeId, string firstname, string lastname, float? salary, string city)
    {
        EmployeeId = employeeId;
        Firstname = firstname;
        Lastname = lastname;
        Salary = salary;
        City = city;
    }
}
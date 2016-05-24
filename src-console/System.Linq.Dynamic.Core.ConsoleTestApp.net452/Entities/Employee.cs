namespace System.Linq.Dynamic.Core.ConsoleTestApp.net452.Entities
{
    public partial class Employee
    {
        public long Id { get; set; }

        public int EmployeeNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime HireDate { get; set; }

        public long? CompanyId { get; set; }

        public long? CountryId { get; set; }

        public long? FunctionId { get; set; }

        public long? SubFunctionId { get; set; }

        public int? Assigned { get; set; }

        public virtual Company Company { get; set; }

        public virtual Country Country { get; set; }

        public virtual Function Function { get; set; }

        public virtual SubFunction SubFunction { get; set; }
    }
}

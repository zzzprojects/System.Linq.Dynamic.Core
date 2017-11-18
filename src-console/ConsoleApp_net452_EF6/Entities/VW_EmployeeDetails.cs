using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace System.Linq.Dynamic.Core.ConsoleTestApp.net452.Entities
{
    public partial class ViewEmployeeDetails
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmployeeNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime HireDate { get; set; }

        public long? CompanyId { get; set; }

        public long? CountryId { get; set; }

        public long? FunctionId { get; set; }

        public long? SubFunctionId { get; set; }

        public int? Assigned { get; set; }

        public bool? IsManager { get; set; }

        public string FullName { get; set; }

        public bool? IsAssigned { get; set; }

        public string CountryCode { get; set; }

        public string CountryName { get; set; }
    }
}

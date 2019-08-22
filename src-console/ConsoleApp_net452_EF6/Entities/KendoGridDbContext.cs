using System.Data.Common;
using System.Data.Entity;
using System.Linq.Dynamic.Core.ConsoleTestApp.net452.Entities;

namespace ConsoleApp_net452_EF6.Entities
{
    public class KendoGridDbContext : DbContext
    {
        public KendoGridDbContext(DbConnection connection)
            : base(connection, false)
        {
        }

        public KendoGridDbContext()
            : base("name=KendoGrid")
        {
        }

        public virtual DbSet<Company> KendoGridCompany { get; set; }
        public virtual DbSet<Country> KendoGridCountry { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Function> KendoGridFunction { get; set; }
        public virtual DbSet<MainCompany> KendoGridMainCompany { get; set; }
        public virtual DbSet<OU> KendoGridOu { get; set; }
        public virtual DbSet<Product> KendoGridProduct { get; set; }
        public virtual DbSet<Role> KendoGridRole { get; set; }
        public virtual DbSet<SubFunction> KendoGridSubFunction { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ViewEmployeeDetails> ViewEmployeeDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .ToTable("KendoGrid_Employee");

            modelBuilder.Entity<Company>()
                .HasMany(e => e.KendoGridEmployee)
                .WithOptional(e => e.Company)
                .HasForeignKey(e => e.CompanyId);

            modelBuilder.Entity<Country>()
                .HasMany(e => e.KendoGridEmployee)
                .WithOptional(e => e.Country)
                .HasForeignKey(e => e.CountryId);

            modelBuilder.Entity<Function>()
                .HasMany(e => e.KendoGridEmployee)
                .WithOptional(e => e.Function)
                .HasForeignKey(e => e.FunctionId);

            modelBuilder.Entity<Function>()
                .HasMany(e => e.KendoGridSubFunction)
                .WithOptional(e => e.Function)
                .HasForeignKey(e => e.FunctionId);

            modelBuilder.Entity<MainCompany>()
                .HasMany(e => e.KendoGridCompany)
                .WithOptional(e => e.MainCompany)
                .HasForeignKey(e => e.MainCompanyId);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.User)
                .WithMany(e => e.Roles)
                .Map(m => m.ToTable("KendoGrid_UserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<SubFunction>()
                .HasMany(e => e.Employee)
                .WithOptional(e => e.SubFunction)
                .HasForeignKey(e => e.SubFunctionId);
        }
    }
}

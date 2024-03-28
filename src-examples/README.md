# Code First with Entity Framework

Simple example of Entity Framework Core Code-First approach.

## Required packages and settings

Nuget Packages:
- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.Tools`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Npgsql.EntityFrameworkCore.PostgreSQL`

Settings:
- Environment variable: `SQLSERVER_MOVIES_LOCAL_CONNSTR`
- Environment variable: `POSTGRES_MOVIES_LOCAL_CONNSTR`

## Entity Framework CLI commands

Postgres:
- `dotnet-ef database update --project src-examples\CodeFirst.DataAccess --context PostgresDbContext`
- `dotnet-ef database drop --project src-examples\CodeFirst.DataAccess --context PostgresDbContext`

MS SQL Server:
- `dotnet-ef database update --project src-examples\CodeFirst.DataAccess --context SqlServerDbContext`
- `dotnet-ef database drop --project src-examples\CodeFirst.DataAccess --context PostgresDbContext`

## How to code first?

- Set proper environmental variable for connection string for SQL Server database. For localhost it is: 

	`"Data Source=DESKTOP-RN0NICT;Initial Catalog=MoviesCodeFirst;Integrated Security=true;"`

- Set proper environmental variable for connection string for Postgre SQL database. For localhost it is: 

	`"Server=localhost;User Id=postgres;Password=postgres;Database=MoviesCodeFirst;"`

- Build a relations between models, for instance, `one-to-one`, `one-to-many`, `many-to-many` according to the database diagram. 
	Refer to
  - https://www.learnentityframeworkcore.com/configuration/one-to-one-relationship-configuration
  - https://www.learnentityframeworkcore.com/relationships/managing-one-to-many-relationships
  - https://www.learnentityframeworkcore.com/configuration/many-to-many-relationship-configuration

- Create proper configurations of models, using Fluent API. Configuration should implement `IEntityTypeConfiguration<T>` interface from Entity Framework namespace
- Create `BaseContext` inherrit it from `DbContext` in Entity Framework namespace, add related sets `BDSet<T>`, where `T` -- models.
- Create `SqlServerContext`, inherrit it from `BaseContext` and override methods `OnConfiguring` and `OnModelCreating`.
- Create `PostgreSqlContext`, inherrit it from `BaseContext` and override methods `OnConfiguring` and `OnModelCreating`.
- Create migration for `SqlServerContext`, use CLI command: 

	`dotnet-ef migrations add SqlServerInitialMigration --project CodeFirst.DataAccess --context SqlServerDbContext`
	
- Create migration for `PostgreSqlContext`, use CLI command: 

	`dotnet-ef migrations add PostgreSqlInitialMigration --project CodeFirst.DataAccess --context PostgresDbContext`
	
- Update SQL Server database, use CLI command: 

	`dotnet-ef database update --project CodeFirst.DataAccess --context SqlServerDbContext`

- Update Postgre SQL database, use CLI command: 

	`dotnet-ef database update --project CodeFirst.DataAccess --context PostgresDbContext`

- Write unit tests for `SqlServerDbContext`
- Write unit tests for `PostgresDbContext`
- Create Generic repository interface
- Implement Generic repository interface
- Test Generic Repositories

## Notes

To get MS SQL connection string proceed:
- Go to visual studio - Server explorer - Data connections
- Right click on data connections - Add new connection
- Type there your server name
- Click add connection
- Then in properties of such connection you'd get connection string
using System.Diagnostics.CodeAnalysis;
using System.Linq.Dynamic.Core;
using System.Reflection;
using Serilog;
using Serilog.Exceptions;

AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;

Log.Logger = new LoggerConfiguration()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
    .CreateLogger();

var customers = new List<Customer>
{
    new(Guid.NewGuid(), [
        new Order(Guid.NewGuid()),
        new Order(Guid.NewGuid()),
        new Order(Guid.NewGuid()),
        new Order(Guid.NewGuid())
    ]),
    new(Guid.NewGuid(), [
        new Order(Guid.NewGuid()),
        new Order(Guid.NewGuid())
    ])
};

var result = customers
    .AsQueryable()
    .Where("Orders.Count >= @0", 3)
    .OrderBy("Orders.Count")
    .ToList();

Log.Information("Found {Count} customers: {@Customers}", result.Count, result);

return;

static Assembly? CurrentDomainOnAssemblyResolve(object? sender, ResolveEventArgs resolveEventArgs)
{
    Log.Warning("Attempted to resolve assembly {Name} by {RequestingAssembly}", resolveEventArgs.Name, resolveEventArgs.RequestingAssembly?.GetName().Name);

    return null;
}

[SuppressMessage("Design", "CA1050:Declare types in namespaces")]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global")]
public record Order(Guid Id);

[SuppressMessage("Design", "CA1050:Declare types in namespaces")]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global")]
public record Customer(Guid Id, List<Order> Orders);
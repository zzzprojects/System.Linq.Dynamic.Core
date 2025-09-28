namespace System.Linq.Dynamic.Core.Tests.Helpers.Models;

public class ModelWithEnum
{
    public string Name { get; set; } = null!;

    public TestEnum TestEnum { get; set; }

    public TestEnum? TestEnumNullable { get; set; }
}
using System.ComponentModel;
using System.Globalization;
using System.Linq.Dynamic.Core;

namespace ConsoleAppSelfContained;

internal class Program
{
    static void Main(string[] args)
    {
        //System.ComponentModel.TypeConverter tc = new CustomDateTimeConverter();
        //tc.ConvertFrom("2023-01-01");

        Console.WriteLine("Write user exclusion func");
        var userExclusionFuncStr = "u => u.Name.ToUpper().Contains(\"TEST\")";
        Console.WriteLine(userExclusionFuncStr);

        var exp = DynamicExpressionParser.ParseLambda<User, bool>(ParsingConfig.Default, false, userExclusionFuncStr);
        Func<User, bool>? userExclusionFunc = exp.Compile();

        var user1 = new User() { Name = "foo" };
        var user2 = new User() { Name = "test" };

        Console.WriteLine($"Is user 1 excluded: {userExclusionFunc(user1)}");
        Console.WriteLine($"Is user 2 excluded: {userExclusionFunc(user2)}");
    }
}

internal class CustomDateTimeConverter : DateTimeOffsetConverter
{
    /// <summary>
    /// Converts the specified object to a <see cref="DateTime"></see>.
    /// </summary>
    /// <param name="context">The date format context.</param>
    /// <param name="culture">The date culture.</param>
    /// <param name="value">The object to be converted.</param>
    /// <returns>A <see cref="Nullable{DateTime}"></see> that represents the specified object.</returns>
    /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
#if NET6_0_OR_GREATER
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
#else
    public override object? ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
#endif
    {
        var dateTimeOffset = base.ConvertFrom(context, culture, value) as DateTimeOffset?;

        return dateTimeOffset?.UtcDateTime;
    }
}

class User
{
    public string Name { get; set; } = string.Empty;
}
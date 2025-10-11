using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Dynamic.Core.Parser.SupportedMethods;

internal class MethodData(MethodBase methodBase, ParameterInfo[] parameters)
{
    public MethodBase MethodBase => methodBase;

    public ParameterInfo[] Parameters => parameters;

    public Expression[] Args { get; set; } = [];
}
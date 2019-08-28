using Fasterflect;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Dynamic.Core.Parser.SupportedMethods
{
    internal class MethodData
    {
        public MethodData()
        {
        }

        public MethodData(MethodBase m)
        {
            this.MethodBase = m;
            this.Parameters = m.GetParameters();
            this.NonOptionalParameterCount = this.Parameters.Count(i => i.IsOptional == false);
        }

        public MethodBase MethodBase { get; set; }

        public ParameterInfo[] Parameters { get; set; }

        public Type[] GenericArguments { get; set; }

        public int NonOptionalParameterCount { get; set; }

        public Expression[] Args { get; set; }

        public bool ExtensionMethod { get; set; }
    }
}

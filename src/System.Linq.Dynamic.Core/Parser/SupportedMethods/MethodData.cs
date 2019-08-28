using System.Collections.Generic;
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
            this.MethodGenericsResolved = !m.GetGenericArguments().Any();
        }

        public MethodBase MethodBase { get; set; }

        public ParameterInfo[] Parameters { get; set; }

        public Dictionary<string, Type> GenericArguments { get; set; } = new Dictionary<string, Type>();

        public int NonOptionalParameterCount { get; set; }

        public List<Expression> Args { get; set; } = new List<Expression>();

        public bool ExtensionMethod { get; set; }

        public bool MethodGenericsResolved { get; set; }
    }
}

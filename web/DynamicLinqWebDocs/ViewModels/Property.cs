using System.Collections.Generic;
using System.Text;
using DynamicLinqWebDocs.Models;

namespace DynamicLinqWebDocs.ViewModels
{
    public class Property
    {
        public string Namespace { get; set; }

        public string Class { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ValueType { get; set; }

        public string ValueTypeDescription { get; set; }
        
        public string Remarks { get; set; }

        public bool IsStatic { get; set; }

        public IList<Example> Examples { get; set; }

        public Frameworks Frameworks { get; set; }

        public PropertyAccessors Accessors { get; set; }

        public string GenerateCSharpSyntaxCode()
        {
            var sb = new StringBuilder();

            sb.Append("public");
            if (IsStatic) sb.Append(" static");

            sb.AppendFormat(" {0}", ValueType);

            sb.AppendFormat(" {0} {{ ", Name);

            if ((Accessors & PropertyAccessors.Get) > 0) sb.AppendFormat("get; ");
            if ((Accessors & PropertyAccessors.Set) > 0) sb.AppendFormat("set; ");

            sb.AppendFormat("}}");

            return sb.ToString();
        }
    }
}
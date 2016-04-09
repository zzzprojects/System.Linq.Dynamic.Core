using System.Collections.Generic;
using System.Text;
using DynamicLinqWebDocs.Models;

namespace DynamicLinqWebDocs.ViewModels
{
    public class Method
    {
        public string Namespace { get; set; }

        public string Class { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IList<Argument> Arguments { get; set; }

        public bool HasParamsArgument { get; set; }

        public string ReturnType { get; set; }

        public string ReturnDescription { get; set; }
        
        public string Remarks { get; set; }

        public bool IsStatic { get; set; }

        public bool IsExtensionMethod { get; set; }

        public IList<Example> Examples { get; set; }

        public Frameworks Frameworks { get; set; }

        public string GenerateCSharpSyntaxCode()
        {
            var sb = new StringBuilder();

            sb.Append("public");
            if (IsStatic || IsExtensionMethod) sb.Append(" static");

            if (ReturnType != null)
                sb.AppendFormat(" {0}", ReturnType);
            else
                sb.Append(" void");

            sb.AppendFormat(" {0}", Name);

            if (Arguments != null && Arguments.Count > 0)
            {
                var last = Arguments[Arguments.Count - 1];

                sb.Append("(");

                bool isFirst = true;

                foreach (var arg in Arguments)
                {
                    if (!isFirst) sb.Append(",");

                    sb.Append("\n\t");

                    if (isFirst && IsExtensionMethod) sb.Append("this ");
                    if (arg == last && HasParamsArgument) sb.Append("params ");

                    sb.AppendFormat("{0} {1}", arg.Type, arg.Name);

                    isFirst = false;
                }

                sb.Append("\n)");
            }
            else
            {
                sb.Append("()");
            }


            return sb.ToString();
        }
    }
}
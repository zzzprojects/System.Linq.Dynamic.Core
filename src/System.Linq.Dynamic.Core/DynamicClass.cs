using System.Reflection;
using System.Text;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Provides a base class for dynamic objects created by using the <see cref="DynamicQueryable.Select(IQueryable,string,object[])"/> 
    /// method. For internal use only.
    /// </summary>
    public abstract class DynamicClass
    {
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            PropertyInfo[] props = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var sb = new StringBuilder();

            //sb.AppendFormat("{0} in {1} : ", GetType().FullName, GetType().GetTypeInfo().Assembly.FullName);

            sb.Append("{");
            for (int i = 0; i < props.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.AppendFormat("{0}={1}", props[i].Name, props[i].GetValue(this, null));
            }
            sb.Append("}");

            return sb.ToString();
        }
    }
}
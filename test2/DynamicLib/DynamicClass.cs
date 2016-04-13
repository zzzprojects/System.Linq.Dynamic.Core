using System.Reflection;
using System.Text;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Provides a base class for dynamic objects created by using the <see cref="DynamicQueryable.Select(IQueryable,string,object[])"/> method. For internal use only.
    /// </summary>
    public abstract class DynamicClass
    {
        public string Test { get; set; }

        public T GetDynamicProperty<T>(string propertyName)
        {
            var type = GetType();
            var propInfo = type.GetProperty(propertyName);

            return (T)propInfo.GetValue(this, null);
        }

        public void SetDynamicProperty<T>(string propertyName, T value)
        {
            var type = GetType();
            var propInfo = type.GetProperty(propertyName);

            propInfo.SetValue(this, value, null);
        }
    }
    public abstract class DynamicClassT<T>
    {
        public T Test { get; set; }
    }
}
using System.Reflection;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Provides a base class for dynamic objects created by using the <see cref="DynamicQueryable.Select(IQueryable,string,object[])"/> method. For internal use only.
    /// </summary>
    public abstract class DynamicClass
    {
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

        //protected bool Equals(DynamicClass other)
        //{
        //    var properties = GetType().GetProperties();
        //    foreach (var property in properties)
        //    {
        //        var thisValue = property.GetValue(this, null);
        //        var otherValue = property.GetValue(other, null);

        //        if (thisValue != null && !thisValue.Equals(otherValue))
        //            return false;

        //        if (otherValue != null && !otherValue.Equals(thisValue))
        //            return false;
        //    }

        //    return true;
        //}

        //public override bool Equals(object obj)
        //{
        //    if (ReferenceEquals(null, obj)) return false;
        //    if (ReferenceEquals(this, obj)) return true;
        //    if (obj.GetType() != this.GetType()) return false;

        //    return Equals((DynamicClass)obj);
        //}
    }
}
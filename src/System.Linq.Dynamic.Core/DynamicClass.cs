#if WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD
using System.Reflection;
#endif

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Provides a base class for dynamic objects.
    /// 
    /// In addition to the methods defined here, the following items are added using reflection:
    /// - default constructor
    /// - constructor with all the properties as parameters (if not linq-to-entities)
    /// - all properties (also with getter and setters)
    /// - ToString() method
    /// - Equals() method
    /// - GetHashCode() method
    /// </summary>
    public abstract class DynamicClass
    {
        /// <summary>
        /// Gets the dynamic property by name.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>T</returns>
        public T GetDynamicProperty<T>(string propertyName)
        {
            var type = GetType();
            var propInfo = type.GetProperty(propertyName);

            return (T)propInfo.GetValue(this, null);
        }

        /// <summary>
        /// Gets the dynamic property by name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>value</returns>
        public object GetDynamicProperty(string propertyName)
        {
            return GetDynamicProperty<object>(propertyName);
        }

        /// <summary>
        /// Sets the dynamic property by name.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public void SetDynamicProperty<T>(string propertyName, T value)
        {
            var type = GetType();
            var propInfo = type.GetProperty(propertyName);

            propInfo.SetValue(this, value, null);
        }

        /// <summary>
        /// Sets the dynamic property by name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public void SetDynamicProperty(string propertyName, object value)
        {
            var type = GetType();
            var propInfo = type.GetProperty(propertyName);

            propInfo.SetValue(this, value, null);
        }
    }
}
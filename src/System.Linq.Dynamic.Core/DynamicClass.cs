#if UAP10_0
using System.Collections.Generic;
using System.Dynamic;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Provides a base class for dynamic objects for UAP10_0.
    /// </summary>
    public class DynamicClass : DynamicObject
    {
        readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        public DynamicClass(
            KeyValuePair<string, object> _1
            )
        {
            _properties.Add(_1.Key, _1.Value);
        }

        public DynamicClass(
            KeyValuePair<string, object> _1,
            KeyValuePair<string, object> _2
            )
        {
            _properties.Add(_1.Key, _1.Value);
            _properties.Add(_2.Key, _2.Value);
        }

        public DynamicClass(
            KeyValuePair<string, object> _1,
            KeyValuePair<string, object> _2,
            KeyValuePair<string, object> _3
            )
        {
            _properties.Add(_1.Key, _1.Value);
            _properties.Add(_2.Key, _2.Value);
            _properties.Add(_3.Key, _3.Value);
        }

        public DynamicClass(
            KeyValuePair<string, object> _1,
            KeyValuePair<string, object> _2,
            KeyValuePair<string, object> _3,
            KeyValuePair<string, object> _4
            )
        {
            _properties.Add(_1.Key, _1.Value);
            _properties.Add(_2.Key, _2.Value);
            _properties.Add(_3.Key, _3.Value);
            _properties.Add(_4.Key, _4.Value);
        }

        public DynamicClass(
            KeyValuePair<string, object> _1,
            KeyValuePair<string, object> _2,
            KeyValuePair<string, object> _3,
            KeyValuePair<string, object> _4,
            KeyValuePair<string, object> _5
            )
        {
            _properties.Add(_1.Key, _1.Value);
            _properties.Add(_2.Key, _2.Value);
            _properties.Add(_3.Key, _3.Value);
            _properties.Add(_4.Key, _4.Value);
            _properties.Add(_5.Key, _5.Value);
        }

        public DynamicClass(
            KeyValuePair<string, object> _1,
            KeyValuePair<string, object> _2,
            KeyValuePair<string, object> _3,
            KeyValuePair<string, object> _4,
            KeyValuePair<string, object> _5,
            KeyValuePair<string, object> _6
            )
        {
            _properties.Add(_1.Key, _1.Value);
            _properties.Add(_2.Key, _2.Value);
            _properties.Add(_3.Key, _3.Value);
            _properties.Add(_4.Key, _4.Value);
            _properties.Add(_5.Key, _5.Value);
            _properties.Add(_6.Key, _6.Value);
        }

        public DynamicClass(
            KeyValuePair<string, object> _1,
            KeyValuePair<string, object> _2,
            KeyValuePair<string, object> _3,
            KeyValuePair<string, object> _4,
            KeyValuePair<string, object> _5,
            KeyValuePair<string, object> _6,
            KeyValuePair<string, object> _7
           )
        {
            _properties.Add(_1.Key, _1.Value);
            _properties.Add(_2.Key, _2.Value);
            _properties.Add(_3.Key, _3.Value);
            _properties.Add(_4.Key, _4.Value);
            _properties.Add(_5.Key, _5.Value);
            _properties.Add(_6.Key, _6.Value);
            _properties.Add(_7.Key, _7.Value);
        }

        public DynamicClass(
            KeyValuePair<string, object> _1,
            KeyValuePair<string, object> _2,
            KeyValuePair<string, object> _3,
            KeyValuePair<string, object> _4,
            KeyValuePair<string, object> _5,
            KeyValuePair<string, object> _6,
            KeyValuePair<string, object> _7,
            KeyValuePair<string, object> _8
           )
        {
            _properties.Add(_1.Key, _1.Value);
            _properties.Add(_2.Key, _2.Value);
            _properties.Add(_3.Key, _3.Value);
            _properties.Add(_4.Key, _4.Value);
            _properties.Add(_5.Key, _5.Value);
            _properties.Add(_6.Key, _6.Value);
            _properties.Add(_7.Key, _7.Value);
            _properties.Add(_8.Key, _8.Value);
        }

        public DynamicClass(
            KeyValuePair<string, object> _1,
            KeyValuePair<string, object> _2,
            KeyValuePair<string, object> _3,
            KeyValuePair<string, object> _4,
            KeyValuePair<string, object> _5,
            KeyValuePair<string, object> _6,
            KeyValuePair<string, object> _7,
            KeyValuePair<string, object> _8,
            KeyValuePair<string, object> _9
           )
        {
            _properties.Add(_1.Key, _1.Value);
            _properties.Add(_2.Key, _2.Value);
            _properties.Add(_3.Key, _3.Value);
            _properties.Add(_4.Key, _4.Value);
            _properties.Add(_5.Key, _5.Value);
            _properties.Add(_6.Key, _6.Value);
            _properties.Add(_7.Key, _7.Value);
            _properties.Add(_8.Key, _8.Value);
            _properties.Add(_9.Key, _9.Value);
        }

        public object this[string name]
        {
            get
            {
                object result;
                if (_properties.TryGetValue(name, out result))
                    return result;

                return null;
            }
            set
            {
                if (_properties.ContainsKey(name))
                    _properties[name] = value;
                else
                    _properties.Add(name, value);
            }
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _properties.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = binder.Name;
            _properties.TryGetValue(name, out result);

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var name = binder.Name;
            if (_properties.ContainsKey(name))
                _properties[name] = value;
            else
                _properties.Add(name, value);

            return true;
        }
    }
}
#else
#if WINDOWS_APP || DOTNET5_1 || NETSTANDARD
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
#endif
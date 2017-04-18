using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if UAP10_0
using System.Dynamic;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Provides a base class for dynamic objects for UAP10_0.
    /// </summary>
    public class DynamicClass : DynamicObject
    {
        readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicClass"/> class.
        /// </summary>
        /// <param name="propertylist">The propertylist.</param>
        public DynamicClass(params KeyValuePair<string, object>[] propertylist)
        {
            foreach (var kvp in propertylist)
            {
                _properties.Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="object"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="object"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns>Value from the property.</returns>
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

        /// <summary>
        /// Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>
        /// A sequence that contains dynamic member names.
        /// </returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _properties.Keys;
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = binder.Name;
            _properties.TryGetValue(name, out result);

            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, the <paramref name="value" /> is "Test".</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
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
# elif NET35
namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Provides a base class for dynamic objects for Net 3.5
    /// </summary>
    public abstract class DynamicClass
    {
        /// <summary>
        /// Gets the dynamic property by name.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>T</returns>
        public T GetDynamicPropertyValue<T>(string propertyName)
        {
            var type = GetType();
            var propInfo = type.GetProperty(propertyName);

            return (T)propInfo.GetValue(this, null);
        }

        /// <summary>
        /// Gets the dynamic property value by name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>value</returns>
        public object GetDynamicPropertyValue(string propertyName)
        {
            return GetDynamicPropertyValue<object>(propertyName);
        }

        /// <summary>
        /// Sets the dynamic property value by name.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public void SetDynamicPropertyValue<T>(string propertyName, T value)
        {
            var type = GetType();
            var propInfo = type.GetProperty(propertyName);

            propInfo.SetValue(this, value, null);
        }

        /// <summary>
        /// Sets the dynamic property value by name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public void SetDynamicPropertyValue(string propertyName, object value)
        {
            SetDynamicPropertyValue<object>(propertyName, value);
        }
    }
}
#else
using System.Dynamic;

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
    public abstract class DynamicClass : DynamicObject
    {
        private Dictionary<string, object> _propertiesDictionary;

        private Dictionary<string, object> _properties
        {
            get
            {
                if (_propertiesDictionary == null)
                {
                    _propertiesDictionary = new Dictionary<string, object>();

                    foreach (PropertyInfo pi in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        int parameters = pi.GetIndexParameters().Length;
                        if (parameters > 0)
                        {
                            // The property is an indexer, skip this.
                            continue;
                        }

                        _propertiesDictionary.Add(pi.Name, pi.GetValue(this, null));
                    }
                }

                return _propertiesDictionary;
            }
        }

        /// <summary>
        /// Gets the dynamic property by name.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>T</returns>
        public T GetDynamicPropertyValue<T>(string propertyName)
        {
            var type = GetType();
            var propInfo = type.GetProperty(propertyName);

            return (T)propInfo.GetValue(this, null);
        }

        /// <summary>
        /// Gets the dynamic property value by name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>value</returns>
        public object GetDynamicPropertyValue(string propertyName)
        {
            return GetDynamicPropertyValue<object>(propertyName);
        }

        /// <summary>
        /// Sets the dynamic property value by name.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public void SetDynamicPropertyValue<T>(string propertyName, T value)
        {
            var type = GetType();
            var propInfo = type.GetProperty(propertyName);

            propInfo.SetValue(this, value, null);
        }

        /// <summary>
        /// Sets the dynamic property value by name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public void SetDynamicPropertyValue(string propertyName, object value)
        {
            SetDynamicPropertyValue<object>(propertyName, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="object"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="object"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns>Value from the property.</returns>
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

        /// <summary>
        /// Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>
        /// A sequence that contains dynamic member names.
        /// </returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _properties.Keys;
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name;
            _properties.TryGetValue(name, out result);

            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, the <paramref name="value" /> is "Test".</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string name = binder.Name;
            if (_properties.ContainsKey(name))
                _properties[name] = value;
            else
                _properties.Add(name, value);

            return true;
        }
    }
}
#endif
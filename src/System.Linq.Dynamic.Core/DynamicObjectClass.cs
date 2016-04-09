#if !(NET35)
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// A DynamicClass as Replacement for creating classes via Reflection Emit, wich is not supported in WinRT
    /// </summary>
    public class DynamicObjectClass : DynamicObject
    {
        readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        public DynamicObjectClass(KeyValuePair<string, object> _1)
        {
            _properties.Add(_1.Key, _1.Value);
        }

        public DynamicObjectClass(KeyValuePair<string, object> _1, KeyValuePair<string, object> _2)
        {
            _properties.Add(_1.Key, _1.Value);
            _properties.Add(_2.Key, _2.Value);
        }

        public DynamicObjectClass(KeyValuePair<string, object> _1, KeyValuePair<string, object> _2, KeyValuePair<string, object> _3)
        {
            _properties.Add(_1.Key, _1.Value);
            _properties.Add(_2.Key, _2.Value);
            _properties.Add(_3.Key, _3.Value);
        }

        public DynamicObjectClass(KeyValuePair<string, object> _1, KeyValuePair<string, object> _2, KeyValuePair<string, object> _3, KeyValuePair<string, object> _4)
        {
            _properties.Add(_1.Key, _1.Value);
            _properties.Add(_2.Key, _2.Value);
            _properties.Add(_3.Key, _3.Value);
            _properties.Add(_4.Key, _4.Value);
        }

        public DynamicObjectClass(KeyValuePair<string, object> _1, KeyValuePair<string, object> _2, KeyValuePair<string, object> _3, KeyValuePair<string, object> _4, KeyValuePair<string, object> _5)
        {
            _properties.Add(_1.Key, _1.Value);
            _properties.Add(_2.Key, _2.Value);
            _properties.Add(_3.Key, _3.Value);
            _properties.Add(_4.Key, _4.Value);
            _properties.Add(_5.Key, _5.Value);
        }

        public DynamicObjectClass(KeyValuePair<string, object> _1, KeyValuePair<string, object> _2, KeyValuePair<string, object> _3, KeyValuePair<string, object> _4, KeyValuePair<string, object> _5, KeyValuePair<string, object> _6)
        {
            _properties.Add(_1.Key, _1.Value);
            _properties.Add(_2.Key, _2.Value);
            _properties.Add(_3.Key, _3.Value);
            _properties.Add(_4.Key, _4.Value);
            _properties.Add(_5.Key, _5.Value);
            _properties.Add(_6.Key, _6.Value);
        }

        public DynamicObjectClass(KeyValuePair<string, object> _1, KeyValuePair<string, object> _2, KeyValuePair<string, object> _3, KeyValuePair<string, object> _4, KeyValuePair<string, object> _5, KeyValuePair<string, object> _6, KeyValuePair<string, object> _7)
        {
            _properties.Add(_1.Key, _1.Value);
            _properties.Add(_2.Key, _2.Value);
            _properties.Add(_3.Key, _3.Value);
            _properties.Add(_4.Key, _4.Value);
            _properties.Add(_5.Key, _5.Value);
            _properties.Add(_6.Key, _6.Value);
            _properties.Add(_7.Key, _7.Value);
        }

        public DynamicObjectClass(KeyValuePair<string, object> _1, KeyValuePair<string, object> _2, KeyValuePair<string, object> _3, KeyValuePair<string, object> _4, KeyValuePair<string, object> _5, KeyValuePair<string, object> _6, KeyValuePair<string, object> _7, KeyValuePair<string, object> _8)
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

        public DynamicObjectClass(KeyValuePair<string, object> _1, KeyValuePair<string, object> _2, KeyValuePair<string, object> _3, KeyValuePair<string, object> _4, KeyValuePair<string, object> _5, KeyValuePair<string, object> _6, KeyValuePair<string, object> _7, KeyValuePair<string, object> _8, KeyValuePair<string, object> _9)
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

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            foreach (var prp in _properties)
            {
                if (stringBuilder.Length > 1)
                    stringBuilder.Append(", ");
                stringBuilder.Append(prp.Key);
                stringBuilder.Append("=");
                stringBuilder.Append(prp.Value);
            }
            stringBuilder.Append("}");
            return (stringBuilder).ToString();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;

            var other = obj as DynamicObjectClass;

            if (other == null) return false;

            if (_properties.Count != other._properties.Count)
                return false;
            return _properties.Keys.All(x => this[x].Equals(other[x]));
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
#endif
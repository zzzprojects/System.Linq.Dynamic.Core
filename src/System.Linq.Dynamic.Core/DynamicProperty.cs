namespace System.Linq.Dynamic.Core
{
    public class DynamicProperty
    {
        public DynamicProperty(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }

        public Type Type { get; }
    }
}
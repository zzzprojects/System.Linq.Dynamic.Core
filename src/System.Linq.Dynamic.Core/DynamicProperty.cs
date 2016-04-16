namespace System.Linq.Dynamic.Core
{
    internal class DynamicProperty
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
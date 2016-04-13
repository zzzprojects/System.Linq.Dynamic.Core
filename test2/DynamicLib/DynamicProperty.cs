namespace System.Linq.Dynamic.Core
{
    public class DynamicProperty
    {
        string name;
        Type type;

        public DynamicProperty(string name, Type type)
        {
            this.name = name;
            this.type = type;
        }

        public string Name
        {
            get { return name; }
        }

        public Type Type
        {
            get { return type; }
        }
    }
}

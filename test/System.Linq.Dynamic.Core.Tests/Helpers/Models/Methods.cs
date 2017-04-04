namespace System.Linq.Dynamic.Core.Tests.Helpers.Models
{
    public class Methods
    {
        public bool Method1(int value)
        {
            return value == 1;
        }

        public bool Method2(object value)
        {
            return value != null && (int)value == 1;
        }

        public bool Method3(object value)
        {
            Item item = value as Item;
            return item != null && item.Value == 1;
        }

        public class Item
        {
            public int Value { get; set; }
        }
    }
}

using System.Linq.Dynamic.Core;

namespace WasmDynamicLinq.Pages;

public partial class Home
{
    protected override void OnInitialized()
    {
        base.OnInitialized();

        var o = new Order();
        dynamic od = o;
        Console.WriteLine(od.OrderId);

        var props = new DynamicProperty[] { new("Name", typeof(string)), new("Birthday", typeof(DateTime)) };
        var type = DynamicClassFactory.CreateType(props);
        var dynamicClass1 = (DynamicClass)Activator.CreateInstance(type, false)!;
        dynamicClass1.SetDynamicPropertyValue("Name", "Albert");
        dynamicClass1.SetDynamicPropertyValue("Birthday", new DateTime(1879, 3, 14));

        Console.WriteLine(dynamicClass1.GetHashCode());
        Console.WriteLine(dynamicClass1);

        var dynamicClass2 = (DynamicClass)Activator.CreateInstance(type, false)!;
        dynamicClass2.SetDynamicPropertyValue("Name", "Albert");
        dynamicClass2.SetDynamicPropertyValue("Birthday", new DateTime(1879, 3, 14));

        Console.WriteLine(dynamicClass2.GetHashCode());
        Console.WriteLine(dynamicClass2);

        Console.WriteLine("EQUALS ??? = " + dynamicClass1.Equals(dynamicClass2));
        Console.WriteLine("EQUALS == " + (dynamicClass1 == dynamicClass2));

        foreach (var f in type.GetFields())
        {
            f.SetValue(dynamicClass1, "Field Reflection Value");
            var fieldValue = f.GetValue(dynamicClass1);
            Console.WriteLine($"Field {f.Name} Reflection Value = {fieldValue}");
            Console.WriteLine(((dynamic)dynamicClass1).Name__Field);
        }
        
        var orders = new List<Order>
        {
            new Order
            {
                OrderId = 1,
                CustomerName = "Customer A",
                OrderItems =
                [
                    new OrderItem { ItemId = 101, ItemName = "Item 1" },
                    new OrderItem { ItemId = 102, ItemName = "Item 2" }
                ]
            },
            new Order
            {
                OrderId = 2,
                CustomerName = "Customer B",
                OrderItems =
                [
                    new OrderItem { ItemId = 103, ItemName = "Item 3" },
                    new OrderItem { ItemId = 104, ItemName = "Item 4" }
                ]
            }
        };

        var allOrderItemsDynamic = orders.AsQueryable()
            .Select("new (OrderId as Id, OrderItems.Where(w => true) as Children, null as Empty)")
            .ToDynamicList<DynamicClass>();

        foreach (var element in allOrderItemsDynamic)
        {
            object elementAsObj = element;

            var idReflection = elementAsObj.GetType().GetProperty("Id")!.GetValue(elementAsObj);
            Console.WriteLine("Reflection = " + idReflection);

            Console.WriteLine("DynamicClass = " + element["Id"]);

            Console.WriteLine("dynamic = " + ((dynamic)element).Id);
        }
    }

    public class Order
    {
        public int OrderId { get; set; }

        public string CustomerName { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }

    public class OrderItem
    {
        public int ItemId { get; set; }

        public string ItemName { get; set; }
    }
}
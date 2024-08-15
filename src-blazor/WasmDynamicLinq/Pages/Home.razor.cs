using System.Linq.Dynamic.Core;

namespace WasmDynamicLinq.Pages;

public partial class Home
{
    protected override void OnInitialized()
    {
        base.OnInitialized();

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
            Console.WriteLine("idReflection = " + idReflection);

            Console.WriteLine("dynamic = " + element["Id"]);
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
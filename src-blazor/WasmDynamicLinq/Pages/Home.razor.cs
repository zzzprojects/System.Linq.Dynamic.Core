using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Reflection.Emit;

namespace WasmDynamicLinq.Pages;

public partial class Home
{
    public class ExampleBase { }

    private void Test()
    {
        // Define a dynamic assembly and module
        AssemblyName assemblyName = new AssemblyName("DynamicAssembly");
        AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

        // Define a public class named "DynamicType"
        TypeBuilder typeBuilder = moduleBuilder.DefineType("DynamicType", TypeAttributes.Public);

        // Define a public field of type string named "MyField"
        typeBuilder.DefineField("MyField", typeof(string), FieldAttributes.Public);

        //typeBuilder.DefineGenericParameters("MyField");

        // Create the type
        Type dynamicType = typeBuilder.CreateType();
        //if (types.Length > 0)
        {
            //dynamicType = dynamicType.MakeGenericType(typeof(string));
        }

        // Create an instance of the dynamic type
        dynamic dynamicObject = Activator.CreateInstance(dynamicType)!;
        
        // Set the value of the field using reflection
        FieldInfo fieldInfo = dynamicType.GetField("MyField", BindingFlags.Public | BindingFlags.Instance)!;
        fieldInfo.SetValue(dynamicObject, "Hello, World!");

        // Output the value
        Console.WriteLine("Test Field Value: " + (string)fieldInfo.GetValue(dynamicObject));
        Console.WriteLine("Test Field Value Dynamic: " + dynamicObject.MyField);
    }

    private void TestChatGPT()
    {
        // Define a dynamic assembly and module
        AssemblyName assemblyName = new AssemblyName("DynamicAssembly");
        AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

        // Define a public generic class named "DynamicType`1" with one generic type parameter
        TypeBuilder typeBuilder = moduleBuilder.DefineType("DynamicType`1", TypeAttributes.Public);

        // Define a generic parameter "T"
        GenericTypeParameterBuilder genericParameter = typeBuilder.DefineGenericParameters("T")[0];

        // Define a public field of type T named "MyField"
        FieldBuilder fieldBuilder = typeBuilder.DefineField("MyField", genericParameter, FieldAttributes.Public);

        // Create the generic type
        Type dynamicType = typeBuilder.CreateType();

        // Make a closed generic type of "DynamicType<string>"
        Type closedGenericType = dynamicType.MakeGenericType(typeof(string));

        // Create an instance of the closed generic type
        dynamic dynamicObject = Activator.CreateInstance(closedGenericType)!;

        // Set the value of the field directly
        FieldInfo fieldInfo = closedGenericType.GetField("MyField", BindingFlags.Public | BindingFlags.Instance)!;
        fieldInfo.SetValue(dynamicObject, "Hello, World!");

        // Get the value of the field directly
        string fieldValue = (string)fieldInfo.GetValue(dynamicObject);

        // Output the value
        Console.WriteLine("Field Value: " + fieldValue);
        Console.WriteLine("Test Field Value Dynamic: " + dynamicObject.MyField);
    }

    private void TestFail()
    {
        // Define a dynamic assembly and module
        AssemblyName assemblyName = new AssemblyName("DynamicAssembly");
        AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

        // Define a public class named "DynamicType"
        TypeBuilder typeBuilder = moduleBuilder.DefineType("DynamicType", TypeAttributes.Public);

        // Define a public field of type string named "MyField"
        typeBuilder.DefineField("MyField", typeof(string), FieldAttributes.Public);

        // Define Generic Parameter
        typeBuilder.DefineGenericParameters("T0");

        // Create the type
        Type dynamicType = typeBuilder.CreateType();

        // Make it generic
        dynamicType = dynamicType.MakeGenericType(typeof(string));

        // Create an instance of the dynamic type
        dynamic dynamicObject = Activator.CreateInstance(dynamicType)!;

        // Set the value of the field using reflection
        FieldInfo fieldInfo = dynamicType.GetField("MyField", BindingFlags.Public | BindingFlags.Instance)!;
        fieldInfo.SetValue(dynamicObject, "Hello, World!"); // this throws exception

        // Output the value
        Console.WriteLine("Test Field Value: " + (string)fieldInfo.GetValue(dynamicObject));
        Console.WriteLine("Test Field Value Dynamic: " + dynamicObject.MyField);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Test();
        //TestChatGPT();
        //TestFail();

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
            break;
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

            // Fail
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
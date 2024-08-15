using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Util;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace WasmDynamicLinq.Pages;

public partial class Home
{
    public class ExampleBase { }

    private void Test()
    {
        AppDomain myDomain = AppDomain.CurrentDomain;
        AssemblyName myAsmName = new AssemblyName("GenericEmitExample1");
        AssemblyBuilder myAssembly = AssemblyBuilder.DefineDynamicAssembly(myAsmName, AssemblyBuilderAccess.RunAndCollect);

        ModuleBuilder myModule = myAssembly.DefineDynamicModule(myAsmName.Name!);

        Type baseType = typeof(ExampleBase);

        TypeBuilder myType = myModule.DefineType("Sample", TypeAttributes.Public);

        Console.WriteLine("Type 'Sample' is generic: {0}", myType.IsGenericType);

        string[] typeParamNames = { "TFirst", "TSecond" };
        GenericTypeParameterBuilder[] typeParams = myType.DefineGenericParameters(typeParamNames);

        GenericTypeParameterBuilder TFirst = typeParams[0];
        GenericTypeParameterBuilder TSecond = typeParams[1];

        Console.WriteLine("Type 'Sample' is generic: {0}", myType.IsGenericType);

        // The following code adds a private field named ExampleField,
        // of type TFirst.
        FieldBuilder exField =
            myType.DefineField("ExampleField", TFirst,
                FieldAttributes.Private);

        // Define a static method that takes an array of TFirst and
        // returns a List<TFirst> containing all the elements of
        // the array. To define this method it is necessary to create
        // the type List<TFirst> by calling MakeGenericType on the
        // generic type definition, List<T>. (The T is omitted with
        // the typeof operator when you get the generic type
        // definition.) The parameter type is created by using the
        // MakeArrayType method.
        //
        Type listOf = typeof(List<>);
        Type listOfTFirst = listOf.MakeGenericType(TFirst);
        Type[] mParamTypes = { TFirst.MakeArrayType() };

        MethodBuilder exMethod =
            myType.DefineMethod("ExampleMethod",
                MethodAttributes.Public | MethodAttributes.Static,
                listOfTFirst,
                mParamTypes);

        // Emit the method body.
        // The method body consists of just three opcodes, to load
        // the input array onto the execution stack, to call the
        // List<TFirst> constructor that takes IEnumerable<TFirst>,
        // which does all the work of putting the input elements into
        // the list, and to return, leaving the list on the stack. The
        // hard work is getting the constructor.
        //
        // The GetConstructor method is not supported on a
        // GenericTypeParameterBuilder, so it is not possible to get
        // the constructor of List<TFirst> directly. There are two
        // steps, first getting the constructor of List<T> and then
        // calling a method that converts it to the corresponding
        // constructor of List<TFirst>.
        //
        // The constructor needed here is the one that takes an
        // IEnumerable<T>. Note, however, that this is not the
        // generic type definition of IEnumerable<T>; instead, the
        // T from List<T> must be substituted for the T of
        // IEnumerable<T>. (This seems confusing only because both
        // types have type parameters named T. That is why this example
        // uses the somewhat silly names TFirst and TSecond.) To get
        // the type of the constructor argument, take the generic
        // type definition IEnumerable<T> (expressed as
        // IEnumerable<> when you use the typeof operator) and
        // call MakeGenericType with the first generic type parameter
        // of List<T>. The constructor argument list must be passed
        // as an array, with just one argument in this case.
        //
        // Now it is possible to get the constructor of List<T>,
        // using GetConstructor on the generic type definition. To get
        // the constructor of List<TFirst>, pass List<TFirst> and
        // the constructor from List<T> to the static
        // TypeBuilder.GetConstructor method.
        //
        ILGenerator ilgen = exMethod.GetILGenerator();

        Type ienumOf = typeof(IEnumerable<>);
        Type TfromListOf = listOf.GetGenericArguments()[0];
        Type ienumOfT = ienumOf.MakeGenericType(TfromListOf);
        Type[] ctorArgs = { ienumOfT };

        ConstructorInfo ctorPrep = listOf.GetConstructor(ctorArgs);
        ConstructorInfo ctor =
            TypeBuilder.GetConstructor(listOfTFirst, ctorPrep);

        ilgen.Emit(OpCodes.Ldarg_0);
        ilgen.Emit(OpCodes.Newobj, ctor);
        ilgen.Emit(OpCodes.Ret);

        // Create the type and save the assembly.
        Type finished = myType.CreateType();
        //myAssembly.Save(myAsmName.Name + ".dll");


    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        // Test();

        var o = new Order();
        dynamic od = o;
        Console.WriteLine(od.OrderId);

        var props = new DynamicProperty[] { new("Name", typeof(string)), new("Birthday", typeof(DateTime)) };
        var type = DynamicClassFactory.CreateType(props);
        var dynamicClass = (DynamicClass)Activator.CreateInstance(type, false)!;
        dynamicClass.SetDynamicPropertyValue("Name", "Albert");
        dynamicClass.SetDynamicPropertyValue("Birthday", new DateTime(1879, 3, 14));

        Console.WriteLine(dynamicClass.GetHashCode());
        Console.WriteLine(dynamicClass);

        foreach (var f in type.GetFields())
        {
            f.SetValue(dynamicClass, "Field Reflection Value");
            var fieldValue = f.GetValue(dynamicClass);
            Console.WriteLine("Field Reflection = " + fieldValue);
            Console.WriteLine(((dynamic)dynamicClass).Name__Field);
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
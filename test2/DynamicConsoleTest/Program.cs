using DynamicLib;
//using Kendo.Mvc.Infrastructure.Implementation;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace DynamicConsoleTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //TestEqualsOld();
            TestEqualsNew();
        }

        //private static void TestEqualsNewTel()
        //{
        //    var properties = new List<DynamicProperty>();
        //    properties.Add(new DynamicProperty("x", typeof(string)));

        //    var t = ClassFactoryTel.Instance.GetDynamicClass(properties);
        //    dynamic x0 = (dynamic)Activator.CreateInstance(t);
        //    var hc = x0.GetHashCode();
        //    string res = x0.get_x();

        //    x0.x = "ddd";

        //    var x1 = (dynamic)Activator.CreateInstance(t);

        //    x1.x = "ddd";

        //    int h0 = x0.GetHashCode();
        //    int h1 = x1.GetHashCode();
        //    Console.WriteLine("hashcode_0 = {0} , hashcode_1 = {1} : eq = {2} ?", h0, h1, h0 == h1);

        //    bool eq = x0 == x1;
        //    Console.WriteLine("bool eq = x0 == x1 ===> {0}", eq);

        //    int u = 0;
        //}

        public class X
        {
            public int Test { get; set; }
        }
       

        private static void TestEqualsNew()
        {
            var props = typeof (X).GetProperties();
            var prop = props[0];
            var methodForSet = prop.GetSetMethod();
            var parameterInfos = methodForSet.GetParameters();
            var parameterInfo = parameterInfos[0];
            var customAttrs = parameterInfo.GetCustomAttributes(false);

            int uyyyy = 0;

            var a1 = new { s = "1234" };
            var a2 = new { s = "1234" };
            Console.WriteLine("bool eq = a1 Equals a2 ===> {0}", a1.Equals(a2));

            var properties = new List<DynamicProperty>();
            properties.Add(new DynamicProperty("x", typeof(string)));
            properties.Add(new DynamicProperty("i", typeof(int)));

            var t = DynamicClassFactory.CreateType(properties);

            var x1 = (dynamic)Activator.CreateInstance(t);
            var index = t.GetProperty("i").GetIndexParameters();

            var method = t.GetMethod("get_i");

            ParameterInfo[] parameters = method.GetParameters();

            var m = t.GetProperty("i").GetGetMethod();

            MethodInfo miSetter = t.GetProperty("i").GetSetMethod();
            var pars = miSetter.GetParameters();
            foreach (ParameterInfo par in pars)
            {
                var ooo = par.GetCustomAttributes(false);
                int uuuuu = 9;
            }

            int retValueI = (int)t.GetProperty("i").GetGetMethod().Invoke(x1, null);

            x1.SetDynamicProperty("i", 10);
            int retValue2 = x1.GetDynamicProperty<int>("i");
            int iii = x1.i;

            // set 
            x1.i = 999;


            dynamic x0 = t.GetConstructor(new Type[] { typeof(string), typeof(int) }).Invoke(new object[] { "inuuu", 140 });
            var hc = x0.GetHashCode();

            
            

            string retValue = (string)t.GetProperty("x").GetGetMethod().Invoke(x0, null);
            
            string resss = x0.x;
           

            x0.x = "ddd";

            //var x1 = (dynamic) Activator.CreateInstance(t);
            
            x1.x = "ddd";

            int h0 = x0.GetHashCode();
            int h1 = x1.GetHashCode();
            Console.WriteLine("hashcode_0 = {0} , hashcode_1 = {1} : eq = {2} ?", h0, h1, h0 == h1);

            bool eq = x0 == x1;
            Console.WriteLine("bool eq = x0 == x1 ===> {0}", eq);

            bool eq2 = x0.Equals(x1);
            Console.WriteLine("bool eq = x0 Equals x1 ===> {0}", eq2);

            int u = 0;
        }

        private static void TestEqualsOld()
        {
            var properties = new Dictionary<string, Type>();
            properties.Add("x", typeof(string));

            var objs = Factory.Create(properties, 2);
            var x0 = objs[0];
            var x1 = objs[0];
            x0._x = "ddd";
            x1._x = "ddd";

            int h0 = x0.GetHashCode();
            int h1 = x1.GetHashCode();
            Console.WriteLine("hashcode_0 = {0} , hashcode_1 = {1} : eq = {2} ?", h0, h1, h0 == h1);

            bool eq = x0 == x1;
            Console.WriteLine("bool eq = x0 == x1 ===> {0}", eq);

            int u = 0;

            var properties2 = new Dictionary<string, Type>();
            properties2.Add("x0", x0.GetType());
            var combinedObjs = Factory.Create(properties2, 2);

            combinedObjs[0].x0 = (dynamic) x0;
            combinedObjs[1].x0 = (dynamic) x0;

            bool be = combinedObjs[0] == combinedObjs[1];

            int t = 9;
        }

        private static void TestAccess()
        {
            var properties = new Dictionary<string, Type>();
            properties.Add("x", typeof(string));

            var obj = Factory.Create(properties, 1)[0];

            obj._x = "ddd"; // accessing backing field directly works
            string x_ = obj._x; // works
            Console.WriteLine("x_=[{0}]", x_);

            obj.x = "err"; // does not work

            string x = obj.x; // does not work
            Console.WriteLine(x);
        }
    }
}
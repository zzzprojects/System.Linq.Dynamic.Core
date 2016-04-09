using DynamicLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DynamicConsoleTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
var list = new Dictionary<string, Type>();
list.Add("x", typeof(string));

var obj = Fact.Create(list);

obj._x = "ddd"; // accessing backing field directly works
string x_ = obj._x; // works
Console.WriteLine("x_=[{0}]", x_);

obj.x = "err"; // does not work

string x = obj.x; // does not work
Console.WriteLine(x);
        }
    }
}
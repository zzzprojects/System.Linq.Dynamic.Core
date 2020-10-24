using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace Z.Dynamic.Core.Lab
{
	class Program
	{
        public class Foo
        {
            public string? Bar { get; set; }
            public string? Baz() => "zzz";
        }

        static void Main(string[] args)
        {
            var expression = "np(Bar.Length)";

            var results = (new List<Foo>() { new Foo()}).AsQueryable().Select("np(Baz().Length)").ToDynamicList();

  


            Request_LambdaAddFunc.Execute();

            //var data = new List<object> {
            //    new { ItemCode = "AAAA", Flag = true, SoNo="aaa",JobNo="JNO01" } ,
            //    new { ItemCode = "AAAA", Flag = true, SoNo="aaa",JobNo="JNO02" } ,
            //    new { ItemCode = "AAAA", Flag = false, SoNo="aaa",JobNo="JNO03" } ,
            //    new { ItemCode = "BBBB", Flag = true, SoNo="bbb",JobNo="JNO04" },
            //    new { ItemCode = "BBBB", Flag = true, SoNo="bbb",JobNo="JNO05" } ,
            //    new { ItemCode = "BBBB", Flag = true, SoNo="ccc",JobNo="JNO06" } ,
            //};
            //var jsonString = JsonConvert.SerializeObject(data);
            //var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonString);

            //var groupList = list.AsQueryable().GroupBy("new (ItemCode, Flag)").ToDynamicList();

            ////var data = new List<object> {
            ////    new { ItemCode = "AAAA", Flag = true, SoNo="aaa",JobNo="JNO01" } ,
            ////    new { ItemCode = "AAAA", Flag = true, SoNo="aaa",JobNo="JNO02" } ,
            ////    new { ItemCode = "AAAA", Flag = false, SoNo="aaa",JobNo="JNO03" } ,
            ////    new { ItemCode = "BBBB", Flag = true, SoNo="bbb",JobNo="JNO04" },
            ////    new { ItemCode = "BBBB", Flag = true, SoNo="bbb",JobNo="JNO05" } ,
            ////    new { ItemCode = "BBBB", Flag = true, SoNo="ccc",JobNo="JNO06" } ,
            ////};
            ////var jsonString = JsonConvert.SerializeObject(data);
            ////var list = JsonConvert.DeserializeObject<List<object>>(jsonString).ToList();
            ////var groupList = list.Select("new (ItemCode, Flag)");

            //Request_Dictionary.Execute();
        }
	}
}

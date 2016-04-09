using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicLib
{
    public static class Fact
    {
        public static dynamic Create(IDictionary<string, Type> properties)
        {
            AssemblyName name = new AssemblyName("Test.DynamicClasses, Version=1.0.0.0");

            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndCollect);
            
            var module = assembly.DefineDynamicModule("Test.DynamicClasses");

            string typeName = "DynamicClass1";
            TypeBuilder tb = module.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Public, typeof(DynamicClass));
            GenerateProperties(tb, properties);

            Type result = tb.CreateTypeInfo().AsType();

            return Activator.CreateInstance(result);
        }

        private static FieldInfo[] GenerateProperties(TypeBuilder tb, IDictionary<string, Type> properties)
        {
            const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            FieldInfo[] fields = new FieldBuilder[properties.Count];
            int i = 0;
            foreach (string key in properties.Keys)
            {
                string name = key;
                Type type = properties[key];
                FieldBuilder fb = tb.DefineField("_" + key, type, FieldAttributes.Public);
                PropertyBuilder pb = tb.DefineProperty(key, PropertyAttributes.HasDefault, type, null);
                
                MethodBuilder mbGet = tb.DefineMethod("get_" + name, getSetAttr, type, Type.EmptyTypes);
                ILGenerator genGet = mbGet.GetILGenerator(8);
                genGet.Emit(OpCodes.Ldarg_0);
                genGet.Emit(OpCodes.Ldfld, fb);
                genGet.Emit(OpCodes.Ret);
                pb.SetGetMethod(mbGet);

                MethodBuilder mbSet = tb.DefineMethod("set_" + name, getSetAttr, null, new Type[] { type });
                ILGenerator genSet = mbSet.GetILGenerator(8);
                genSet.Emit(OpCodes.Ldarg_0);
                genSet.Emit(OpCodes.Ldarg_1);
                genSet.Emit(OpCodes.Stfld, fb);
                genSet.Emit(OpCodes.Ret);
                pb.SetSetMethod(mbSet);

                fields[i] = fb;
            }

            return fields;
        }
    }
}

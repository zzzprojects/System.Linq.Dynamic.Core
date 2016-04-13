using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicLib
{
    public static class Factory
    {
        public static List<dynamic> Create(IDictionary<string, Type> properties, int count)
        {
            AssemblyName name = new AssemblyName("Test.DynamicClasses, Version=1.0.0.0");

            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndCollect);
            
            var module = assembly.DefineDynamicModule("Test.DynamicClasses");

            string typeName = "DynamicClass1";
            TypeBuilder tb = module.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Public, typeof(DynamicClass));
            var fields = GenerateProperties(tb, properties);
            GenerateEquals(tb, fields);
            GenerateGetHashCode(tb, fields);

            Type result = tb.CreateTypeInfo().AsType();

            var l = new List<dynamic>();
            for (int i = 0; i < count; i++)
            {
                l.Add(Activator.CreateInstance(result));
            }

            return l;
        }

        private static void GenerateEquals(TypeBuilder tb, FieldInfo[] fields)
        {
            MethodBuilder mb = tb.DefineMethod("Equals",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(bool), new Type[] { typeof(object) });
            ILGenerator gen = mb.GetILGenerator();
            LocalBuilder other = gen.DeclareLocal(tb.GetType());
            Label next = gen.DefineLabel();
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Isinst, tb.GetType());
            gen.Emit(OpCodes.Stloc, other);
            gen.Emit(OpCodes.Ldloc, other);
            gen.Emit(OpCodes.Brtrue_S, next);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ret);
            gen.MarkLabel(next);
            foreach (FieldInfo field in fields)
            {
                Type ft = field.FieldType;
                Type ct = typeof(EqualityComparer<>).MakeGenericType(ft);
                next = gen.DefineLabel();
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.Emit(OpCodes.Ldloc, other);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("Equals", new Type[] { ft, ft }), null);
                gen.Emit(OpCodes.Brtrue_S, next);
                gen.Emit(OpCodes.Ldc_I4_0);
                gen.Emit(OpCodes.Ret);
                gen.MarkLabel(next);
            }
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Ret);
        }

        private static void GenerateGetHashCode(TypeBuilder tb, FieldInfo[] fields)
        {
            MethodBuilder mb = tb.DefineMethod("GetHashCode",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(int), Type.EmptyTypes);
            ILGenerator gen = mb.GetILGenerator();
            gen.Emit(OpCodes.Ldc_I4_0);
            foreach (FieldInfo field in fields)
            {
                Type ft = field.FieldType;
                Type ct = typeof(EqualityComparer<>).MakeGenericType(ft);
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("GetHashCode", new Type[] { ft }), null);
                gen.Emit(OpCodes.Xor);
            }
            gen.Emit(OpCodes.Ret);
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

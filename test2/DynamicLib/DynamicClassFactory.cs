using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using ReflectionBridge.Extensions;

//,
namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// http://stackoverflow.com/questions/29413942/c-sharp-anonymous-object-with-properties-from-dictionary
    /// </summary>
    public static class DynamicClassFactory
    {
        private static readonly ConcurrentDictionary<string, Type> GeneratedTypes = new ConcurrentDictionary<string, Type>();

        private static readonly ModuleBuilder ModuleBuilder;

        // Some objects we cache
        private static readonly CustomAttributeBuilder CompilerGeneratedAttributeBuilder = new CustomAttributeBuilder(typeof(CompilerGeneratedAttribute).GetConstructor(Type.EmptyTypes), new object[0]);
        private static readonly CustomAttributeBuilder DebuggerBrowsableAttributeBuilder = new CustomAttributeBuilder(typeof(DebuggerBrowsableAttribute).GetConstructor(new[] { typeof(DebuggerBrowsableState) }), new object[] { DebuggerBrowsableState.Never });
        private static readonly CustomAttributeBuilder DebuggerHiddenAttributeBuilder = new CustomAttributeBuilder(typeof(DebuggerHiddenAttribute).GetConstructor(Type.EmptyTypes), new object[0]);

        private static readonly ConstructorInfo ObjectCtor = typeof(object).GetConstructor(Type.EmptyTypes);
#if DNXCORE50
        private static readonly MethodInfo ObjectToString = typeof(object).GetMethod("ToString", BindingFlags.Instance | BindingFlags.Public);
#else
        private static readonly MethodInfo ObjectToString = typeof(object).GetMethod("ToString", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);
#endif

        private static readonly ConstructorInfo StringBuilderCtor = typeof(StringBuilder).GetConstructor(Type.EmptyTypes);
#if DNXCORE50
        private static readonly MethodInfo StringBuilderAppendString = typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) });
        private static readonly MethodInfo StringBuilderAppendObject = typeof(StringBuilder).GetMethod("Append", new[] { typeof(object) });
#else
        private static readonly MethodInfo StringBuilderAppendString = typeof(StringBuilder).GetMethod("Append", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string) }, null);
        private static readonly MethodInfo StringBuilderAppendObject = typeof(StringBuilder).GetMethod("Append", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(object) }, null);
#endif

        private static readonly Type EqualityComparer = typeof(EqualityComparer<>);

#if DNXCORE50
        private static readonly Type EqualityComparerGenericArgument = TypeExtensions.GetGenericArguments(EqualityComparer)[0];
        private static readonly MethodInfo EqualityComparerDefault = EqualityComparer.GetMethod("get_Default", BindingFlags.Static | BindingFlags.Public);
        private static readonly MethodInfo EqualityComparerEquals = EqualityComparer.GetMethod("Equals", new[] { EqualityComparerGenericArgument, EqualityComparerGenericArgument });
        private static readonly MethodInfo EqualityComparerGetHashCode = EqualityComparer.GetMethod("GetHashCode", new[] { EqualityComparerGenericArgument });
#else
        private static readonly Type EqualityComparerGenericArgument = EqualityComparer.GetGenericArguments()[0];
        private static readonly MethodInfo EqualityComparerDefault = EqualityComparer.GetMethod("get_Default", BindingFlags.Static | BindingFlags.Public, null, Type.EmptyTypes, null);
        private static readonly MethodInfo EqualityComparerEquals = EqualityComparer.GetMethod("Equals", BindingFlags.Instance | BindingFlags.Public, null, new[] { EqualityComparerGenericArgument, EqualityComparerGenericArgument }, null);
        private static readonly MethodInfo EqualityComparerGetHashCode = EqualityComparer.GetMethod("GetHashCode", BindingFlags.Instance | BindingFlags.Public, null, new[] { EqualityComparerGenericArgument }, null);
#endif

        private static int _index = -1;

        static DynamicClassFactory()
        {
            AssemblyName assemblyName = new AssemblyName("System.Linq.Dynamic.Core.DynamicClasses, Version=1.0.0.0");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

            ModuleBuilder = assemblyBuilder.DefineDynamicModule("System.Linq.Dynamic.Core.DynamicClasses");
        }

        /// <summary>
        /// CreateType
        /// </summary>
        /// <param name="properties">The DynamicProperties</param>
        /// <returns>Type</returns>
        public static Type CreateType(IList<DynamicProperty> properties)
        {
            //Check.HasNoNulls(properties, nameof(properties));

            var types = properties.Select(p => p.Type).ToArray();
            var names = properties.Select(p => p.Name).ToArray();

            // Anonymous classes are generics based. The generic classes
            // are distinguished by number of parameters and name of 
            // parameters. The specific types of the parameters are the 
            // generic arguments. We recreate this by creating a fullName
            // composed of all the property names, separated by a "|"
            string fullName = string.Join("|", names.Select(Escape).ToArray());

            Type type;

            if (!GeneratedTypes.TryGetValue(fullName, out type))
            {
                // We create only a single class at a time, through this lock
                // Note that this is a variant of the double-checked locking.
                // It is safe because we are using a thread safe class.
                lock (GeneratedTypes)
                {
                    if (!GeneratedTypes.TryGetValue(fullName, out type))
                    {
                        int index = Interlocked.Increment(ref _index);

                        string name = names.Length != 0 ? $"<>f__AnonymousType{index}`{names.Length}" : $"<>f__AnonymousType{index}";

                        TypeBuilder tb = ModuleBuilder.DefineType(name, TypeAttributes.AnsiClass | TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoLayout | TypeAttributes.BeforeFieldInit, typeof(DynamicClass));
                        tb.SetCustomAttribute(CompilerGeneratedAttributeBuilder);

                        GenericTypeParameterBuilder[] generics;

                        if (names.Length != 0)
                        {
                            string[] genericNames = names.Select(genericName => $"<{genericName}>j__TPar").ToArray();
                            generics = tb.DefineGenericParameters(genericNames);

                            foreach (GenericTypeParameterBuilder b in generics)
                            {
                                b.SetCustomAttribute(CompilerGeneratedAttributeBuilder);
                                var attr = b.GenericParameterAttributes;

                                int y = 0;
                            }
                        }
                        else
                        {
                            generics = new GenericTypeParameterBuilder[0];
                        }

                        // .ctor default
                        ConstructorBuilder constructorDef = tb.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.HasThis, Type.EmptyTypes);
                        constructorDef.SetCustomAttribute(DebuggerHiddenAttributeBuilder);

                        ILGenerator ilgeneratorConstructorDef = constructorDef.GetILGenerator();
                        ilgeneratorConstructorDef.Emit(OpCodes.Ldarg_0);
                        ilgeneratorConstructorDef.Emit(OpCodes.Call, ObjectCtor);
                        ilgeneratorConstructorDef.Emit(OpCodes.Ret);

                        // .ctor
                        ConstructorBuilder constructor = tb.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.HasThis, generics);
                        constructor.SetCustomAttribute(DebuggerHiddenAttributeBuilder);

                        ILGenerator ilgeneratorConstructor = constructor.GetILGenerator();
                        ilgeneratorConstructor.Emit(OpCodes.Ldarg_0);
                        ilgeneratorConstructor.Emit(OpCodes.Call, ObjectCtor);

                        var fields = new FieldBuilder[names.Length];

                        // There are two for cycles because we want to have all the getter methods before all the other methods
                        for (int i = 0; i < names.Length; i++)
                        {
                            // field
                            fields[i] = tb.DefineField($"<{names[i]}>i__Field", generics[i], FieldAttributes.Private | FieldAttributes.InitOnly);
                            fields[i].SetCustomAttribute(DebuggerBrowsableAttributeBuilder);

                            // .ctor
                            constructor.DefineParameter(i + 1, ParameterAttributes.None, names[i]);
                            ilgeneratorConstructor.Emit(OpCodes.Ldarg_0);

                            if (i == 0)
                            {
                                ilgeneratorConstructor.Emit(OpCodes.Ldarg_1);
                            }
                            else if (i == 1)
                            {
                                ilgeneratorConstructor.Emit(OpCodes.Ldarg_2);
                            }
                            else if (i == 2)
                            {
                                ilgeneratorConstructor.Emit(OpCodes.Ldarg_3);
                            }
                            else if (i < 255)
                            {
                                ilgeneratorConstructor.Emit(OpCodes.Ldarg_S, (byte)(i + 1));
                            }
                            else
                            {
                                // Ldarg uses a ushort, but the Emit only accepts short, so we use a unchecked(...), cast to short and let the CLR interpret it as ushort.
                                ilgeneratorConstructor.Emit(OpCodes.Ldarg, unchecked((short)(i + 1)));
                            }

                            ilgeneratorConstructor.Emit(OpCodes.Stfld, fields[i]);

                            PropertyBuilder property = tb.DefineProperty(names[i], PropertyAttributes.None, CallingConventions.Standard | CallingConventions.HasThis, generics[i], Type.EmptyTypes);
                            property.SetCustomAttribute(CompilerGeneratedAttributeBuilder);

                            // getter
                            MethodBuilder getter = tb.DefineMethod($"get_{names[i]}", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, CallingConventions.Standard | CallingConventions.HasThis, generics[i], new Type[0]);
                            getter.SetCustomAttribute(CompilerGeneratedAttributeBuilder);


                            //MethodKindEnum.PropAccessor
                            ILGenerator ilgeneratorGetter = getter.GetILGenerator();
                            //ilgeneratorGetter.Emit(OpCodes.Ldc_I4, 5);
                            ilgeneratorGetter.Emit(OpCodes.Ldarg_0);
                            ilgeneratorGetter.Emit(OpCodes.Ldfld, fields[i]);
                            ilgeneratorGetter.Emit(OpCodes.Ret);
                            property.SetGetMethod(getter);

                            // setter
                            MethodBuilder setter = tb.DefineMethod($"set_{names[i]}", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, CallingConventions.Standard | CallingConventions.HasThis, null, new[] { generics[i] });
                            setter.SetCustomAttribute(CompilerGeneratedAttributeBuilder);
                            setter.DefineParameter(1, ParameterAttributes.In, generics[i].Name);


                            ILGenerator ilgeneratorSetter = setter.GetILGenerator();
                            ilgeneratorSetter.Emit(OpCodes.Ldarg_0);
                            ilgeneratorSetter.Emit(OpCodes.Ldarg_1);
                            ilgeneratorSetter.Emit(OpCodes.Stfld, fields[i]);
                            ilgeneratorSetter.Emit(OpCodes.Ret);
                            property.SetSetMethod(setter);
                        }

                        // ToString()
                        MethodBuilder toString = tb.DefineMethod("ToString", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.Standard | CallingConventions.HasThis, typeof(string), Type.EmptyTypes);
                        toString.SetCustomAttribute(DebuggerHiddenAttributeBuilder);
                        ILGenerator ilgeneratorToString = toString.GetILGenerator();
                        ilgeneratorToString.DeclareLocal(typeof(StringBuilder));
                        ilgeneratorToString.Emit(OpCodes.Newobj, StringBuilderCtor);
                        ilgeneratorToString.Emit(OpCodes.Stloc_0);

                        // Equals
                        MethodBuilder equals = tb.DefineMethod("Equals", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.Standard | CallingConventions.HasThis, typeof(bool), new[] { typeof(object) });
                        equals.SetCustomAttribute(DebuggerHiddenAttributeBuilder);
                        equals.DefineParameter(1, ParameterAttributes.None, "value");
                        ILGenerator ilgeneratorEquals = equals.GetILGenerator();
                        ilgeneratorEquals.DeclareLocal(tb);
                        ilgeneratorEquals.Emit(OpCodes.Ldarg_1);
                        ilgeneratorEquals.Emit(OpCodes.Isinst, tb);
                        ilgeneratorEquals.Emit(OpCodes.Stloc_0);
                        ilgeneratorEquals.Emit(OpCodes.Ldloc_0);

                        Label equalsLabel = ilgeneratorEquals.DefineLabel();

                        // GetHashCode()
                        MethodBuilder getHashCode = tb.DefineMethod("GetHashCode", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.Standard | CallingConventions.HasThis, typeof(int), Type.EmptyTypes);
                        getHashCode.SetCustomAttribute(DebuggerHiddenAttributeBuilder);
                        ILGenerator ilgeneratorGetHashCode = getHashCode.GetILGenerator();
                        ilgeneratorGetHashCode.DeclareLocal(typeof(int));

                        if (names.Length == 0)
                        {
                            ilgeneratorGetHashCode.Emit(OpCodes.Ldc_I4_0);
                        }
                        else
                        {
                            // As done by Roslyn
                            // Note that initHash can vary, because string.GetHashCode() isn't "stable" for different compilation of the code
                            int initHash = 0;

                            for (int i = 0; i < names.Length; i++)
                            {
                                initHash = unchecked(initHash * (-1521134295) + fields[i].Name.GetHashCode());
                            }

                            // Note that the CSC seems to generate a different seed for every anonymous class
                            ilgeneratorGetHashCode.Emit(OpCodes.Ldc_I4, initHash);
                        }

                        for (int i = 0; i < names.Length; i++)
                        {
#if DNXCORE50
                            Type equalityComparerT = EqualityComparer.MakeGenericType(generics[i].AsType());
#else
                            Type equalityComparerT = EqualityComparer.MakeGenericType(generics[i]);
#endif
                            // Equals()
                            MethodInfo equalityComparerTDefault = TypeBuilder.GetMethod(equalityComparerT, EqualityComparerDefault);
                            MethodInfo equalityComparerTEquals = TypeBuilder.GetMethod(equalityComparerT, EqualityComparerEquals);
                            ilgeneratorEquals.Emit(OpCodes.Brfalse_S, equalsLabel);
                            ilgeneratorEquals.Emit(OpCodes.Call, equalityComparerTDefault);
                            ilgeneratorEquals.Emit(OpCodes.Ldarg_0);
                            ilgeneratorEquals.Emit(OpCodes.Ldfld, fields[i]);
                            ilgeneratorEquals.Emit(OpCodes.Ldloc_0);
                            ilgeneratorEquals.Emit(OpCodes.Ldfld, fields[i]);
                            ilgeneratorEquals.Emit(OpCodes.Callvirt, equalityComparerTEquals);

                            // GetHashCode();
                            MethodInfo equalityComparerTGetHashCode = TypeBuilder.GetMethod(equalityComparerT, EqualityComparerGetHashCode);
                            ilgeneratorGetHashCode.Emit(OpCodes.Stloc_0);
                            ilgeneratorGetHashCode.Emit(OpCodes.Ldc_I4, -1521134295);
                            ilgeneratorGetHashCode.Emit(OpCodes.Ldloc_0);
                            ilgeneratorGetHashCode.Emit(OpCodes.Mul);
                            ilgeneratorGetHashCode.Emit(OpCodes.Call, equalityComparerTDefault);
                            ilgeneratorGetHashCode.Emit(OpCodes.Ldarg_0);
                            ilgeneratorGetHashCode.Emit(OpCodes.Ldfld, fields[i]);
                            ilgeneratorGetHashCode.Emit(OpCodes.Callvirt, equalityComparerTGetHashCode);
                            ilgeneratorGetHashCode.Emit(OpCodes.Add);

                            // ToString();
                            ilgeneratorToString.Emit(OpCodes.Ldloc_0);
                            ilgeneratorToString.Emit(OpCodes.Ldstr, i == 0 ? $"{{ {names[i]} = " : $", {names[i]} = ");
                            ilgeneratorToString.Emit(OpCodes.Callvirt, StringBuilderAppendString);
                            ilgeneratorToString.Emit(OpCodes.Pop);
                            ilgeneratorToString.Emit(OpCodes.Ldloc_0);
                            ilgeneratorToString.Emit(OpCodes.Ldarg_0);
                            ilgeneratorToString.Emit(OpCodes.Ldfld, fields[i]);
#if DNXCORE50
                            ilgeneratorToString.Emit(OpCodes.Box, generics[i].AsType());
#else
                            ilgeneratorToString.Emit(OpCodes.Box, generics[i]);
#endif
                            ilgeneratorToString.Emit(OpCodes.Callvirt, StringBuilderAppendObject);
                            ilgeneratorToString.Emit(OpCodes.Pop);
                        }

                        // .ctor
                        ilgeneratorConstructor.Emit(OpCodes.Ret);

                        // Equals()
                        if (names.Length == 0)
                        {
                            ilgeneratorEquals.Emit(OpCodes.Ldnull);
                            ilgeneratorEquals.Emit(OpCodes.Ceq);
                            ilgeneratorEquals.Emit(OpCodes.Ldc_I4_0);
                            ilgeneratorEquals.Emit(OpCodes.Ceq);
                        }
                        else
                        {
                            ilgeneratorEquals.Emit(OpCodes.Ret);
                            ilgeneratorEquals.MarkLabel(equalsLabel);
                            ilgeneratorEquals.Emit(OpCodes.Ldc_I4_0);
                        }

                        ilgeneratorEquals.Emit(OpCodes.Ret);

                        // GetHashCode()
                        ilgeneratorGetHashCode.Emit(OpCodes.Stloc_0);
                        ilgeneratorGetHashCode.Emit(OpCodes.Ldloc_0);
                        ilgeneratorGetHashCode.Emit(OpCodes.Ret);

                        // ToString()
                        ilgeneratorToString.Emit(OpCodes.Ldloc_0);
                        ilgeneratorToString.Emit(OpCodes.Ldstr, names.Length == 0 ? "{ }" : " }");
                        ilgeneratorToString.Emit(OpCodes.Callvirt, StringBuilderAppendString);
                        ilgeneratorToString.Emit(OpCodes.Pop);
                        ilgeneratorToString.Emit(OpCodes.Ldloc_0);
                        ilgeneratorToString.Emit(OpCodes.Callvirt, ObjectToString);
                        ilgeneratorToString.Emit(OpCodes.Ret);

#if DNXCORE50
                        type = tb.CreateTypeInfo().AsType();
#else
                        type = tb.CreateType();
#endif

                        type = GeneratedTypes.GetOrAdd(fullName, type);
                    }
                }
            }

            if (types.Length != 0)
            {
                type = type.MakeGenericType(types);
            }

            var props = type.GetProperties();
            foreach (var prop in props)
            {
                var setM = prop.GetSetMethod();
                var parameters = setM.GetParameters();
                foreach (var parameter in parameters)
                {
                    var x = parameter.GetCustomAttributes(false);

                    
                    if (x == null)
                    {
                        
                    }
                }
            }

            return type;
        }

        private static string Escape(string str)
        {
            // We escape the \ with \\, so that we can safely escape the "|" (that we use as a separator) with "\|"
            str = str.Replace(@"\", @"\\");
            str = str.Replace(@"|", @"\|");
            return str;
        }
    }
}
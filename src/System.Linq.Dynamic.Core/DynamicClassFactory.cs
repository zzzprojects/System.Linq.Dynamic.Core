#if !(UAP10_0)
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Dynamic.Core.Validation;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
#if WINDOWS_APP
using System.Linq;
#endif

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// A factory to create dynamic classes, based on <see href="http://stackoverflow.com/questions/29413942/c-sharp-anonymous-object-with-properties-from-dictionary" />.
    /// </summary>
    public static class DynamicClassFactory
    {
        // EmptyTypes is used to indicate that we are looking for someting without any parameters. 
        private static readonly Type[] EmptyTypes = new Type[0];

        private static readonly ConcurrentDictionary<string, Type> GeneratedTypes = new ConcurrentDictionary<string, Type>();

        private static readonly ModuleBuilder ModuleBuilder;

        // Some objects we cache
        private static readonly CustomAttributeBuilder CompilerGeneratedAttributeBuilder = new CustomAttributeBuilder(typeof(CompilerGeneratedAttribute).GetConstructor(EmptyTypes), new object[0]);
        private static readonly CustomAttributeBuilder DebuggerBrowsableAttributeBuilder = new CustomAttributeBuilder(typeof(DebuggerBrowsableAttribute).GetConstructor(new[] { typeof(DebuggerBrowsableState) }), new object[] { DebuggerBrowsableState.Never });
        private static readonly CustomAttributeBuilder DebuggerHiddenAttributeBuilder = new CustomAttributeBuilder(typeof(DebuggerHiddenAttribute).GetConstructor(EmptyTypes), new object[0]);

        private static readonly ConstructorInfo ObjectCtor = typeof(object).GetConstructor(EmptyTypes);
#if WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD
        private static readonly MethodInfo ObjectToString = typeof(object).GetMethod("ToString", BindingFlags.Instance | BindingFlags.Public);
#else
        private static readonly MethodInfo ObjectToString = typeof(object).GetMethod("ToString", BindingFlags.Instance | BindingFlags.Public, null, EmptyTypes, null);
#endif

        private static readonly ConstructorInfo StringBuilderCtor = typeof(StringBuilder).GetConstructor(EmptyTypes);
#if WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD
        private static readonly MethodInfo StringBuilderAppendString = typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) });
        private static readonly MethodInfo StringBuilderAppendObject = typeof(StringBuilder).GetMethod("Append", new[] { typeof(object) });
#else
        private static readonly MethodInfo StringBuilderAppendString = typeof(StringBuilder).GetMethod("Append", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string) }, null);
        private static readonly MethodInfo StringBuilderAppendObject = typeof(StringBuilder).GetMethod("Append", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(object) }, null);
#endif

        private static readonly Type EqualityComparer = typeof(EqualityComparer<>);
        private static readonly Type EqualityComparerGenericArgument = EqualityComparer.GetGenericArguments()[0];
#if WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD
        private static readonly MethodInfo EqualityComparerDefault = EqualityComparer.GetMethod("get_Default", BindingFlags.Static | BindingFlags.Public);
        private static readonly MethodInfo EqualityComparerEquals = EqualityComparer.GetMethod("Equals", new[] { EqualityComparerGenericArgument, EqualityComparerGenericArgument });
        private static readonly MethodInfo EqualityComparerGetHashCode = EqualityComparer.GetMethod("GetHashCode", new[] { EqualityComparerGenericArgument });
#else
        private static readonly MethodInfo EqualityComparerDefault = EqualityComparer.GetMethod("get_Default", BindingFlags.Static | BindingFlags.Public, null, EmptyTypes, null);
        private static readonly MethodInfo EqualityComparerEquals = EqualityComparer.GetMethod("Equals", BindingFlags.Instance | BindingFlags.Public, null, new[] { EqualityComparerGenericArgument, EqualityComparerGenericArgument }, null);
        private static readonly MethodInfo EqualityComparerGetHashCode = EqualityComparer.GetMethod("GetHashCode", BindingFlags.Instance | BindingFlags.Public, null, new[] { EqualityComparerGenericArgument }, null);
#endif

        private static int _index = -1;

        /// <summary>
        /// The AssemblyName
        /// </summary>
        public static string DynamicAssemblyName = "System.Linq.Dynamic.Core.DynamicClasses, Version=1.0.0.0";

        /// <summary>
        /// Initializes the <see cref="DynamicClassFactory"/> class.
        /// </summary>
        static DynamicClassFactory()
        {
            var assemblyName = new AssemblyName(DynamicAssemblyName);
            var assemblyBuilder = AssemblyBuilderFactory.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

            ModuleBuilder = assemblyBuilder.DefineDynamicModule("System.Linq.Dynamic.Core.DynamicClasses");
        }

        /// <summary>
        /// The CreateType method creates a new data class with a given set of public properties and returns the System.Type object for the newly created class. If a data class with an identical sequence of properties has already been created, the System.Type object for this class is returned.        
        /// Data classes implement private instance variables and read/write property accessors for the specified properties.Data classes also override the Equals and GetHashCode members to implement by-value equality.
        /// Data classes are created in an in-memory assembly in the current application domain. All data classes inherit from <see cref="DynamicClass"/> and are given automatically generated names that should be considered private (the names will be unique within the application domain but not across multiple invocations of the application). Note that once created, a data class stays in memory for the lifetime of the current application domain. There is currently no way to unload a dynamically created data class.
        /// The dynamic expression parser uses the CreateClass methods to generate classes from data object initializers. This feature in turn is often used with the dynamic Select method to create projections.
        /// </summary>
        /// <param name="properties">The DynamicProperties</param>
        /// <param name="createParameterCtor">Create a constructor with parameters. Default set to true. Note that for Linq-to-Database objects, this needs to be set to false.</param>
        /// <returns>Type</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// DynamicProperty[] props = new DynamicProperty[] { new DynamicProperty("Name", typeof(string)), new DynamicProperty("Birthday", typeof(DateTime)) };
        /// Type type = DynamicClassFactory.CreateType(props);
        /// DynamicClass dynamicClass = Activator.CreateInstance(type) as DynamicClass;
        /// dynamicClass.SetDynamicProperty("Name", "Albert");
        /// dynamicClass.SetDynamicProperty("Birthday", new DateTime(1879, 3, 14));
        /// Console.WriteLine(dynamicClass);
        /// ]]>
        /// </code>
        /// </example>
        public static Type CreateType([NotNull] IList<DynamicProperty> properties, bool createParameterCtor = true)
        {
            Check.HasNoNulls(properties, nameof(properties));

            Type[] types = properties.Select(p => p.Type).ToArray();
            string[] names = properties.Select(p => p.Name).ToArray();

            string key = GenerateKey(properties, createParameterCtor);

            Type type;
            if (!GeneratedTypes.TryGetValue(key, out type))
            {
                // We create only a single class at a time, through this lock
                // Note that this is a variant of the double-checked locking.
                // It is safe because we are using a thread safe class.
                lock (GeneratedTypes)
                {
                    if (!GeneratedTypes.TryGetValue(key, out type))
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
                            }
                        }
                        else
                        {
                            generics = new GenericTypeParameterBuilder[0];
                        }

                        var fields = new FieldBuilder[names.Length];

                        // There are two for cycles because we want to have all the getter methods before all the other methods
                        for (int i = 0; i < names.Length; i++)
                        {
                            // field
                            fields[i] = tb.DefineField($"<{names[i]}>i__Field", generics[i].AsType(), FieldAttributes.Private | FieldAttributes.InitOnly);
                            fields[i].SetCustomAttribute(DebuggerBrowsableAttributeBuilder);

                            PropertyBuilder property = tb.DefineProperty(names[i], PropertyAttributes.None, CallingConventions.HasThis, generics[i].AsType(), EmptyTypes);

                            // getter
                            MethodBuilder getter = tb.DefineMethod($"get_{names[i]}", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, CallingConventions.HasThis, generics[i].AsType(), null);
                            getter.SetCustomAttribute(CompilerGeneratedAttributeBuilder);
                            ILGenerator ilgeneratorGetter = getter.GetILGenerator();
                            ilgeneratorGetter.Emit(OpCodes.Ldarg_0);
                            ilgeneratorGetter.Emit(OpCodes.Ldfld, fields[i]);
                            ilgeneratorGetter.Emit(OpCodes.Ret);
                            property.SetGetMethod(getter);

                            // setter
                            MethodBuilder setter = tb.DefineMethod($"set_{names[i]}", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, CallingConventions.HasThis, null, new[] { generics[i].AsType() });
                            setter.SetCustomAttribute(CompilerGeneratedAttributeBuilder);

                            // workaround for https://github.com/dotnet/corefx/issues/7792
                            setter.DefineParameter(1, ParameterAttributes.In, generics[i].Name);

                            ILGenerator ilgeneratorSetter = setter.GetILGenerator();
                            ilgeneratorSetter.Emit(OpCodes.Ldarg_0);
                            ilgeneratorSetter.Emit(OpCodes.Ldarg_1);
                            ilgeneratorSetter.Emit(OpCodes.Stfld, fields[i]);
                            ilgeneratorSetter.Emit(OpCodes.Ret);
                            property.SetSetMethod(setter);
                        }

                        // ToString()
                        MethodBuilder toString = tb.DefineMethod("ToString", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.HasThis, typeof(string), EmptyTypes);
                        toString.SetCustomAttribute(DebuggerHiddenAttributeBuilder);
                        ILGenerator ilgeneratorToString = toString.GetILGenerator();
                        ilgeneratorToString.DeclareLocal(typeof(StringBuilder));
                        ilgeneratorToString.Emit(OpCodes.Newobj, StringBuilderCtor);
                        ilgeneratorToString.Emit(OpCodes.Stloc_0);

                        // Equals
                        MethodBuilder equals = tb.DefineMethod("Equals", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.HasThis, typeof(bool), new[] { typeof(object) });
                        equals.SetCustomAttribute(DebuggerHiddenAttributeBuilder);
                        equals.DefineParameter(1, ParameterAttributes.In, "value");

                        ILGenerator ilgeneratorEquals = equals.GetILGenerator();
                        ilgeneratorEquals.DeclareLocal(tb.AsType());
                        ilgeneratorEquals.Emit(OpCodes.Ldarg_1);
                        ilgeneratorEquals.Emit(OpCodes.Isinst, tb.AsType());
                        ilgeneratorEquals.Emit(OpCodes.Stloc_0);
                        ilgeneratorEquals.Emit(OpCodes.Ldloc_0);

                        Label equalsLabel = ilgeneratorEquals.DefineLabel();

                        // GetHashCode()
                        MethodBuilder getHashCode = tb.DefineMethod("GetHashCode", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.HasThis, typeof(int), EmptyTypes);
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
                            Type equalityComparerT = EqualityComparer.MakeGenericType(generics[i].AsType());

                            // Equals()
                            MethodInfo equalityComparerTDefault = TypeBuilder.GetMethod(equalityComparerT, EqualityComparerDefault);
                            MethodInfo equalityComparerTEquals = TypeBuilder.GetMethod(equalityComparerT, EqualityComparerEquals);

                            // Illegal one-byte branch at position: 9. Requested branch was: 143.
                            // So replace OpCodes.Brfalse_S to OpCodes.Brfalse
                            ilgeneratorEquals.Emit(OpCodes.Brfalse, equalsLabel);
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
                            ilgeneratorToString.Emit(OpCodes.Box, generics[i].AsType());
                            ilgeneratorToString.Emit(OpCodes.Callvirt, StringBuilderAppendObject);
                            ilgeneratorToString.Emit(OpCodes.Pop);
                        }

                        if (createParameterCtor)
                        {
                            // .ctor default
                            ConstructorBuilder constructorDef = tb.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.HasThis, EmptyTypes);
                            constructorDef.SetCustomAttribute(DebuggerHiddenAttributeBuilder);

                            ILGenerator ilgeneratorConstructorDef = constructorDef.GetILGenerator();
                            ilgeneratorConstructorDef.Emit(OpCodes.Ldarg_0);
                            ilgeneratorConstructorDef.Emit(OpCodes.Call, ObjectCtor);
                            ilgeneratorConstructorDef.Emit(OpCodes.Ret);

                            // .ctor with params
                            ConstructorBuilder constructor = tb.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.HasThis, generics.Select(p => p.AsType()).ToArray());
                            constructor.SetCustomAttribute(DebuggerHiddenAttributeBuilder);

                            ILGenerator ilgeneratorConstructor = constructor.GetILGenerator();
                            ilgeneratorConstructor.Emit(OpCodes.Ldarg_0);
                            ilgeneratorConstructor.Emit(OpCodes.Call, ObjectCtor);

                            for (int i = 0; i < names.Length; i++)
                            {
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
                            }

                            ilgeneratorConstructor.Emit(OpCodes.Ret);
                        }

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

                        type = tb.CreateType();

                        type = GeneratedTypes.GetOrAdd(key, type);
                    }
                }
            }

            if (types.Length != 0)
            {
                type = type.MakeGenericType(types);
            }

            return type;
        }

        /// <summary>
        /// Generates the key.
        /// Anonymous classes are generics based. The generic classes are distinguished by number of parameters and name of parameters. The specific types of the parameters are the generic arguments.
        /// </summary>
        /// <param name="dynamicProperties">The dynamic propertys.</param>
        /// <param name="createParameterCtor">if set to <c>true</c> [create parameter ctor].</param>
        /// <returns></returns>
        private static string GenerateKey(IEnumerable<DynamicProperty> dynamicProperties, bool createParameterCtor)
        {
            // We recreate this by creating a fullName composed of all the property names and types, separated by a "|".
            // And append and extra field depending on createParameterCtor.
            return string.Format("{0}_{1}", string.Join("|", dynamicProperties.Select(p => Escape(p.Name) + "~" + p.Type.FullName).ToArray()), createParameterCtor ? "c" : string.Empty);
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
#endif

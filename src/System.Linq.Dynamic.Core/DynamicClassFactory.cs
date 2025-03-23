#if !UAP10_0
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Dynamic.Core.Validation;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace System.Linq.Dynamic.Core;

/// <summary>
/// A factory to create dynamic classes, based on <see href="http://stackoverflow.com/questions/29413942/c-sharp-anonymous-object-with-properties-from-dictionary" />.
/// </summary>
public static class DynamicClassFactory
{
    private const string DynamicAssemblyName = "System.Linq.Dynamic.Core.DynamicClasses, Version=1.0.0.0";
    private const string DynamicModuleName = "System.Linq.Dynamic.Core.DynamicClasses";

    // Some objects we cache
    private static readonly CustomAttributeBuilder CompilerGeneratedAttributeBuilder = new(typeof(CompilerGeneratedAttribute).GetConstructor(Type.EmptyTypes)!, []);
    private static readonly CustomAttributeBuilder DebuggerBrowsableAttributeBuilder = new(typeof(DebuggerBrowsableAttribute).GetConstructor([typeof(DebuggerBrowsableState)])!, [DebuggerBrowsableState.Never]);
    private static readonly CustomAttributeBuilder DebuggerHiddenAttributeBuilder = new(typeof(DebuggerHiddenAttribute).GetConstructor(Type.EmptyTypes)!, []);

    private static readonly ConstructorInfo ObjectCtor = typeof(object).GetConstructor(Type.EmptyTypes)!;

    private static readonly ConstructorInfo StringBuilderCtor = typeof(StringBuilder).GetConstructor(Type.EmptyTypes)!;
#if UAP10_0 || NETSTANDARD
    private static readonly MethodInfo StringBuilderAppendString = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), [typeof(string)])!;
    private static readonly MethodInfo StringBuilderAppendObject = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), [typeof(object)])!;
#else
    private static readonly MethodInfo StringBuilderAppendString = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), BindingFlags.Instance | BindingFlags.Public, null, [typeof(string)], null)!;
    private static readonly MethodInfo StringBuilderAppendObject = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), BindingFlags.Instance | BindingFlags.Public, null, [typeof(object)], null)!;
#endif

    private static readonly Type EqualityComparer = typeof(EqualityComparer<>);

    private static readonly ConcurrentDictionary<string, Type> GeneratedTypes = new();
    private static readonly ModuleBuilder ModuleBuilder;

    private static int _index = -1;

    /// <summary>
    /// Initializes the <see cref="DynamicClassFactory"/> class.
    /// </summary>
    static DynamicClassFactory()
    {
        var assemblyName = new AssemblyName(DynamicAssemblyName);
        var assemblyBuilder = AssemblyBuilderFactory.DefineDynamicAssembly
        (
            assemblyName,
#if NET35
            AssemblyBuilderAccess.Run
#else
            AssemblyBuilderAccess.RunAndCollect
#endif
        );

        ModuleBuilder = assemblyBuilder.DefineDynamicModule(DynamicModuleName);
    }

    /// <summary>
    /// Create a GenericComparerType based on the GenericType and an instance of a <see cref="IComparer"/>.
    /// </summary>
    /// <param name="comparerGenericType">The GenericType</param>
    /// <param name="comparerType">The <see cref="IComparer"/> instance</param>
    /// <returns>Type</returns>
    public static Type CreateGenericComparerType(Type comparerGenericType, Type comparerType)
    {
        Check.NotNull(comparerGenericType);
        Check.NotNull(comparerType);

        var key = $"{comparerGenericType.FullName}_{comparerType.FullName}";

        // ReSharper disable once InconsistentlySynchronizedField
        if (!GeneratedTypes.TryGetValue(key, out var type))
        {
            // We create only a single class at a time, through this lock
            // Note that this is a variant of the double-checked locking.
            // It is safe because we are using a thread safe class.
            lock (GeneratedTypes)
            {
                if (!GeneratedTypes.TryGetValue(key, out type))
                {
                    var compareMethodGeneric = comparerGenericType.GetMethod("Compare")!;
                    var compareMethod = typeof(IComparer).GetMethod("Compare")!;
                    var compareCtor = comparerType.GetConstructor(Type.EmptyTypes)!;
                    var genericType = comparerGenericType.GetGenericArguments()[0];

                    var typeBuilder = ModuleBuilder.DefineType(key, TypeAttributes.AnsiClass | TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoLayout, typeof(object));
                    typeBuilder.AddInterfaceImplementation(comparerGenericType);

                    var fieldBuilder = typeBuilder.DefineField("_c", typeof(IComparer), FieldAttributes.Private | FieldAttributes.InitOnly);

                    var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.HasThis, Type.EmptyTypes);
                    var constructorIL = constructorBuilder.GetILGenerator();
                    constructorIL.Emit(OpCodes.Ldarg_0);
                    constructorIL.Emit(OpCodes.Call, ObjectCtor);
                    constructorIL.Emit(OpCodes.Ldarg_0);
                    constructorIL.Emit(OpCodes.Newobj, compareCtor);
                    constructorIL.Emit(OpCodes.Stfld, fieldBuilder);
                    constructorIL.Emit(OpCodes.Ret);

                    var methodBuilder = typeBuilder.DefineMethod(
                        compareMethodGeneric.Name,
                        compareMethodGeneric.Attributes & ~MethodAttributes.Abstract,
                        compareMethodGeneric.CallingConvention,
                        compareMethodGeneric.ReturnType,
                        compareMethodGeneric.GetParameters().Select(p => p.ParameterType).ToArray()
                    );
                    var methodBuilderIL = methodBuilder.GetILGenerator();
                    methodBuilderIL.Emit(OpCodes.Ldarg_0);
                    methodBuilderIL.Emit(OpCodes.Ldfld, fieldBuilder);
                    methodBuilderIL.Emit(OpCodes.Ldarg_1);
                    methodBuilderIL.Emit(OpCodes.Box, genericType);
                    methodBuilderIL.Emit(OpCodes.Ldarg_2);
                    methodBuilderIL.Emit(OpCodes.Box, genericType);
                    methodBuilderIL.Emit(OpCodes.Callvirt, compareMethod);
                    methodBuilderIL.Emit(OpCodes.Ret);

                    return GeneratedTypes.GetOrAdd(key, typeBuilder.CreateType());
                }
            }
        }

        return type;
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
    /// DynamicClass dynamicClass = (DynamicClass) Activator.CreateInstance(type)!;
    /// dynamicClass.SetDynamicPropertyValue("Name", "Albert");
    /// dynamicClass.SetDynamicPropertyValue("Birthday", new DateTime(1879, 3, 14));
    /// ]]>
    /// </code>
    /// </example>
    public static Type CreateType(IList<DynamicProperty> properties, bool createParameterCtor = true)
    {
        Check.HasNoNulls(properties);

        var key = GenerateKey(properties, createParameterCtor);

        // ReSharper disable once InconsistentlySynchronizedField
        if (!GeneratedTypes.TryGetValue(key, out var type))
        {
            // We create only a single class at a time, through this lock.
            // Note that this is a variant of the double-checked locking.
            // It is safe because we are using a thread safe class.
            lock (GeneratedTypes)
            {
                return GeneratedTypes.GetOrAdd(key, _ => EmitType(properties, createParameterCtor));
            }
        }

        return type;
    }

    /// <summary>
    /// Create an instance of a <see cref="DynamicClass"/> based on a list of <see cref="DynamicPropertyWithValue"/>.
    /// </summary>
    /// <param name="dynamicPropertiesWithValue">The dynamic properties including the value you want to set in the generated instance.</param>
    /// <param name="createParameterCtor">Create a constructor with parameters. Default set to true. Note that for Linq-to-Database objects, this needs to be set to false.</param>
    /// <returns>Instance of a <see cref="DynamicClass"/>.</returns>
    public static DynamicClass CreateInstance(IList<DynamicPropertyWithValue> dynamicPropertiesWithValue, bool createParameterCtor = true)
    {
        var type = CreateType(dynamicPropertiesWithValue.Cast<DynamicProperty>().ToArray(), createParameterCtor);
        var dynamicClass = (DynamicClass)Activator.CreateInstance(type)!;
        foreach (var dynamicPropertyWithValue in dynamicPropertiesWithValue.Where(p => p.Value != null))
        {
            dynamicClass.SetDynamicPropertyValue(dynamicPropertyWithValue.Name, dynamicPropertyWithValue.Value!);
        }

        return dynamicClass;
    }

    private static Type EmitType(IList<DynamicProperty> properties, bool createParameterCtor)
    {
        var typeIndex = Interlocked.Increment(ref _index);
        var typeName = properties.Any() ? $"<>f__AnonymousType{typeIndex}`{properties.Count}" : $"<>f__AnonymousType{typeIndex}";

        var typeBuilder = ModuleBuilder.DefineType(typeName, TypeAttributes.AnsiClass | TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoLayout | TypeAttributes.BeforeFieldInit, typeof(DynamicClass));
        typeBuilder.SetCustomAttribute(CompilerGeneratedAttributeBuilder);

        var fieldBuilders = new FieldBuilder[properties.Count];

        // There are two for-loops because we want to have all the getter methods before all the other methods
        for (int i = 0; i < properties.Count; i++)
        {
            var fieldName = properties[i].Name;
            var fieldType = properties[i].Type;

            // field
            fieldBuilders[i] = typeBuilder.DefineField($"<{fieldName}>i__Field", fieldType, FieldAttributes.Private | FieldAttributes.InitOnly);
            fieldBuilders[i].SetCustomAttribute(DebuggerBrowsableAttributeBuilder);

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(fieldName, PropertyAttributes.None, CallingConventions.HasThis, fieldType, Type.EmptyTypes);

            // getter
            MethodBuilder getter = typeBuilder.DefineMethod($"get_{fieldName}", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, CallingConventions.HasThis, fieldType, null);
            getter.SetCustomAttribute(CompilerGeneratedAttributeBuilder);

            ILGenerator ilgeneratorGetter = getter.GetILGenerator();
            ilgeneratorGetter.Emit(OpCodes.Ldarg_0);
            ilgeneratorGetter.Emit(OpCodes.Ldfld, fieldBuilders[i]);
            ilgeneratorGetter.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(getter);

            // setter
            MethodBuilder setter = typeBuilder.DefineMethod($"set_{fieldName}", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, CallingConventions.HasThis, null, [fieldType]);
            setter.SetCustomAttribute(CompilerGeneratedAttributeBuilder);

            // workaround for https://github.com/dotnet/corefx/issues/7792
            setter.DefineParameter(1, ParameterAttributes.In, properties[i].Name);

            ILGenerator ilgeneratorSetter = setter.GetILGenerator();
            ilgeneratorSetter.Emit(OpCodes.Ldarg_0);
            ilgeneratorSetter.Emit(OpCodes.Ldarg_1);
            ilgeneratorSetter.Emit(OpCodes.Stfld, fieldBuilders[i]);
            ilgeneratorSetter.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(setter);
        }

        // ToString()
        MethodBuilder toString = typeBuilder.DefineMethod(nameof(ToString), MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.HasThis, typeof(string), Type.EmptyTypes);
        toString.SetCustomAttribute(DebuggerHiddenAttributeBuilder);

        ILGenerator ilgeneratorToString = toString.GetILGenerator();
        ilgeneratorToString.DeclareLocal(typeof(StringBuilder));
        ilgeneratorToString.Emit(OpCodes.Newobj, StringBuilderCtor);
        ilgeneratorToString.Emit(OpCodes.Stloc_0);

        // Equals
        MethodBuilder equals = typeBuilder.DefineMethod(nameof(Equals), MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.HasThis, typeof(bool), [typeof(object)]);
        equals.DefineParameter(1, ParameterAttributes.In, "value");
        equals.SetCustomAttribute(DebuggerHiddenAttributeBuilder);

        ILGenerator ilgeneratorEquals = equals.GetILGenerator();
        ilgeneratorEquals.DeclareLocal(typeBuilder.AsType());
        ilgeneratorEquals.Emit(OpCodes.Ldarg_1);
        ilgeneratorEquals.Emit(OpCodes.Isinst, typeBuilder.AsType());
        ilgeneratorEquals.Emit(OpCodes.Stloc_0);
        ilgeneratorEquals.Emit(OpCodes.Ldloc_0);

        // GetHashCode()
        MethodBuilder getHashCode = typeBuilder.DefineMethod(nameof(GetHashCode), MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.HasThis, typeof(int), Type.EmptyTypes);
        getHashCode.SetCustomAttribute(DebuggerHiddenAttributeBuilder);

        ILGenerator ilgeneratorGetHashCode = getHashCode.GetILGenerator();
        ilgeneratorGetHashCode.DeclareLocal(typeof(int));

        if (properties.Count == 0)
        {
            ilgeneratorGetHashCode.Emit(OpCodes.Ldc_I4_0);
        }
        else
        {
            // As done by Roslyn
            // Note that initHash can vary, because string.GetHashCode() isn't "stable" for different compilation of the code
            int initHash = 0;

            for (int i = 0; i < properties.Count; i++)
            {
                initHash = unchecked(initHash * -1521134295 + fieldBuilders[i].Name.GetHashCode());
            }

            // Note that the CSC seems to generate a different seed for every anonymous class
            ilgeneratorGetHashCode.Emit(OpCodes.Ldc_I4, initHash);
        }

        Label equalsLabel = ilgeneratorEquals.DefineLabel();

        for (var i = 0; i < properties.Count; i++)
        {
            var fieldName = properties[i].Name;
            var fieldType = properties[i].Type;
            var equalityComparerT = EqualityComparer.MakeGenericType(fieldType);

            // Equals()
            MethodInfo equalityComparerTDefault = equalityComparerT.GetMethod("get_Default", BindingFlags.Static | BindingFlags.Public)!;
            MethodInfo equalityComparerTEquals = equalityComparerT.GetMethod(nameof(EqualityComparer.Equals), BindingFlags.Instance | BindingFlags.Public, null, [fieldType, fieldType], null)!;

            // Illegal one-byte branch at position: 9. Requested branch was: 143.
            // So replace OpCodes.Brfalse_S to OpCodes.Brfalse
            ilgeneratorEquals.Emit(OpCodes.Brfalse, equalsLabel);
            ilgeneratorEquals.Emit(OpCodes.Call, equalityComparerTDefault);
            ilgeneratorEquals.Emit(OpCodes.Ldarg_0);
            ilgeneratorEquals.Emit(OpCodes.Ldfld, fieldBuilders[i]);
            ilgeneratorEquals.Emit(OpCodes.Ldloc_0);
            ilgeneratorEquals.Emit(OpCodes.Ldfld, fieldBuilders[i]);
            ilgeneratorEquals.Emit(OpCodes.Callvirt, equalityComparerTEquals);

            // GetHashCode();
            MethodInfo equalityComparerTGetHashCode = equalityComparerT.GetMethod(nameof(EqualityComparer.GetHashCode), BindingFlags.Instance | BindingFlags.Public, null, [fieldType], null)!;
            ilgeneratorGetHashCode.Emit(OpCodes.Stloc_0);
            ilgeneratorGetHashCode.Emit(OpCodes.Ldc_I4, -1521134295);
            ilgeneratorGetHashCode.Emit(OpCodes.Ldloc_0);
            ilgeneratorGetHashCode.Emit(OpCodes.Mul);
            ilgeneratorGetHashCode.Emit(OpCodes.Call, equalityComparerTDefault);
            ilgeneratorGetHashCode.Emit(OpCodes.Ldarg_0);
            ilgeneratorGetHashCode.Emit(OpCodes.Ldfld, fieldBuilders[i]);
            ilgeneratorGetHashCode.Emit(OpCodes.Callvirt, equalityComparerTGetHashCode);
            ilgeneratorGetHashCode.Emit(OpCodes.Add);

            // ToString();
            ilgeneratorToString.Emit(OpCodes.Ldloc_0);
            ilgeneratorToString.Emit(OpCodes.Ldstr, i == 0 ? $"{{ {fieldName} = " : $", {fieldName} = ");
            ilgeneratorToString.Emit(OpCodes.Callvirt, StringBuilderAppendString);
            ilgeneratorToString.Emit(OpCodes.Pop);
            ilgeneratorToString.Emit(OpCodes.Ldloc_0);
            ilgeneratorToString.Emit(OpCodes.Ldarg_0);
            ilgeneratorToString.Emit(OpCodes.Ldfld, fieldBuilders[i]);
            ilgeneratorToString.Emit(OpCodes.Box, properties[i].Type);
            ilgeneratorToString.Emit(OpCodes.Callvirt, StringBuilderAppendObject);
            ilgeneratorToString.Emit(OpCodes.Pop);
        }

        // Only create the default and with params constructor when there are any params.
        // Otherwise default constructor is not needed because it matches the default
        // one provided by the runtime when no constructor is present
        if (createParameterCtor && properties.Any())
        {
            // .ctor default
            ConstructorBuilder constructorDef = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.HasThis, Type.EmptyTypes);
            constructorDef.SetCustomAttribute(DebuggerHiddenAttributeBuilder);

            ILGenerator ilgeneratorConstructorDef = constructorDef.GetILGenerator();
            ilgeneratorConstructorDef.Emit(OpCodes.Ldarg_0);
            ilgeneratorConstructorDef.Emit(OpCodes.Call, ObjectCtor);
            ilgeneratorConstructorDef.Emit(OpCodes.Ret);

            // .ctor with params
            var types = properties.Select(p => p.Type).ToArray();
            ConstructorBuilder constructor = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.HasThis, types);
            constructor.SetCustomAttribute(DebuggerHiddenAttributeBuilder);

            ILGenerator ilgeneratorConstructor = constructor.GetILGenerator();
            ilgeneratorConstructor.Emit(OpCodes.Ldarg_0);
            ilgeneratorConstructor.Emit(OpCodes.Call, ObjectCtor);

            for (var i = 0; i < properties.Count; i++)
            {
                constructor.DefineParameter(i + 1, ParameterAttributes.None, properties[i].Name);
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
                    // Ldarg uses an ushort, but the Emit only accepts short, so we use a unchecked(...), cast to short and let the CLR interpret it as ushort.
                    ilgeneratorConstructor.Emit(OpCodes.Ldarg, unchecked((short)(i + 1)));
                }

                ilgeneratorConstructor.Emit(OpCodes.Stfld, fieldBuilders[i]);
            }

            ilgeneratorConstructor.Emit(OpCodes.Ret);
        }

        // Equals()
        if (properties.Count == 0)
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
        ilgeneratorToString.Emit(OpCodes.Ldstr, properties.Count == 0 ? "{ }" : " }");
        ilgeneratorToString.Emit(OpCodes.Callvirt, StringBuilderAppendString);
        ilgeneratorToString.Emit(OpCodes.Pop);
        ilgeneratorToString.Emit(OpCodes.Ldloc_0);
        ilgeneratorToString.Emit(OpCodes.Callvirt, PredefinedMethodsHelper.ObjectToString);
        ilgeneratorToString.Emit(OpCodes.Ret);

        EmitEqualityOperators(typeBuilder, equals);

        return typeBuilder.CreateType();
    }

    private static void EmitEqualityOperators(TypeBuilder typeBuilder, MethodBuilder equals)
    {
        // Define the '==' operator
        MethodBuilder equalityOperator = typeBuilder.DefineMethod(
            "op_Equality",
            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
            typeof(bool),
            [typeBuilder.AsType(), typeBuilder.AsType()]);

        ILGenerator ilgeneratorEqualityOperator = equalityOperator.GetILGenerator();

        // if (left == null || right == null) return ReferenceEquals(left, right);
        Label endLabel = ilgeneratorEqualityOperator.DefineLabel();
        ilgeneratorEqualityOperator.Emit(OpCodes.Ldarg_0);
        ilgeneratorEqualityOperator.Emit(OpCodes.Brfalse_S, endLabel);
        ilgeneratorEqualityOperator.Emit(OpCodes.Ldarg_1);
        ilgeneratorEqualityOperator.Emit(OpCodes.Brfalse_S, endLabel);

        // return left.Equals(right);
        ilgeneratorEqualityOperator.Emit(OpCodes.Ldarg_0);
        ilgeneratorEqualityOperator.Emit(OpCodes.Ldarg_1);
        ilgeneratorEqualityOperator.Emit(OpCodes.Callvirt, equals);
        ilgeneratorEqualityOperator.Emit(OpCodes.Ret);

        // Return false if one is null
        ilgeneratorEqualityOperator.MarkLabel(endLabel);
        ilgeneratorEqualityOperator.Emit(OpCodes.Ldarg_0);
        ilgeneratorEqualityOperator.Emit(OpCodes.Ldarg_1);
        ilgeneratorEqualityOperator.Emit(OpCodes.Call, typeof(object).GetMethod("ReferenceEquals")!);
        ilgeneratorEqualityOperator.Emit(OpCodes.Ret);

        // Define the '!=' operator
        MethodBuilder inequalityOperator = typeBuilder.DefineMethod(
            "op_Inequality",
            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
            typeof(bool),
            [typeBuilder.AsType(), typeBuilder.AsType()]);

        ILGenerator ilNeq = inequalityOperator.GetILGenerator();

        // return !(left == right);
        ilNeq.Emit(OpCodes.Ldarg_0);
        ilNeq.Emit(OpCodes.Ldarg_1);
        ilNeq.Emit(OpCodes.Call, equalityOperator);
        ilNeq.Emit(OpCodes.Ldc_I4_0);
        ilNeq.Emit(OpCodes.Ceq);
        ilNeq.Emit(OpCodes.Ret);
    }

    /// <summary>
    /// Generates the key.
    /// Anonymous classes are generics based. The generic classes are distinguished by number of parameters and name of parameters.
    /// The specific types of the parameters are the generic arguments.
    /// </summary>
    /// <param name="dynamicProperties">The dynamic properties.</param>
    /// <param name="createParameterCtor">if set to <c>true</c> [create parameter ctor].</param>
    /// <returns></returns>
    private static string GenerateKey(IEnumerable<DynamicProperty> dynamicProperties, bool createParameterCtor)
    {
        // We recreate this by creating a fullName composed of all the property names and types, separated by a "|".
        // And append and extra field depending on createParameterCtor.
        return $"{string.Join("|", dynamicProperties.Select(p => Escape(p.Name) + "~" + p.Type.FullName).ToArray())}_{(createParameterCtor ? "c" : string.Empty)}";
    }

    private static string Escape(string str)
    {
        // We escape the \ with \\, so that we can safely escape the "|" (that we use as a separator) with "\|"
        str = str.Replace(@"\", @"\\");
        str = str.Replace(@"|", @"\|");
        return str;
    }

    /// <summary>
    /// Used for unit-testing
    /// </summary>
    internal static void ClearGeneratedTypes()
    {
        lock (GeneratedTypes)
        {
            GeneratedTypes.Clear();
        }
    }
}
#endif
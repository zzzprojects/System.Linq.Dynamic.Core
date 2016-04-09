using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace System.Linq.Dynamic.Core
{
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "There is only ever one instance of this class, and it should never be destroyed except on AppDomain termination.")]
    internal class ClassFactory
    {
        public static readonly ClassFactory Instance = new ClassFactory();

        private ModuleBuilder _module;
        private Dictionary<Signature, Type> _classes;
        private int _classCount;
#if SILVERLIGHT
        ReaderWriterLock _rwLock;
#else
        private ReaderWriterLockSlim _rwLock;
#endif

        private ClassFactory()
        {
            AssemblyName name = new AssemblyName("System.Linq.Dynamic.Core.DynamicClasses, Version=1.0.0.0");
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
#if ENABLE_LINQ_PARTIAL_TRUST
            new ReflectionPermission(PermissionState.Unrestricted).Assert();
#endif
            try
            {
                _module = assembly.DefineDynamicModule("System.Linq.Dynamic.Core.DynamicClasses");
            }
            finally
            {
#if ENABLE_LINQ_PARTIAL_TRUST
                PermissionSet.RevertAssert();
#endif
            }
            _classes = new Dictionary<Signature, Type>();
#if SILVERLIGHT
            _rwLock = new ReaderWriterLock();
#else
            _rwLock = new ReaderWriterLockSlim();
#endif
        }

        public Type GetDynamicClass(IEnumerable<DynamicProperty> properties)
        {
            Signature signature = new Signature(properties);

#if SILVERLIGHT || NET20
            _rwLock.AcquireReaderLock(Timeout.Infinite);
#else
            _rwLock.EnterReadLock();
#endif

            try
            {
                Type type;

                if (_classes.TryGetValue(signature, out type)) return type;
            }
            finally
            {
#if SILVERLIGHT
                 _rwLock.ReleaseReaderLock();
#else
                _rwLock.ExitReadLock();
#endif
            }

            return CreateDynamicClass(signature);
        }

        private Type CreateDynamicClass(Signature signature)
        {
#if SILVERLIGHT || NET20
            _rwLock.UpgradeToWriterLock(Timeout.Infinite);
#else
            _rwLock.EnterWriteLock();
#endif

            try
            {
                Type type;

                //do a final check to make sure the type hasn't been generated.
                if (_classes.TryGetValue(signature, out type))
                    return type;

                string typeName = "DynamicClass" + (_classCount + 1);
#if ENABLE_LINQ_PARTIAL_TRUST
                new ReflectionPermission(PermissionState.Unrestricted).Assert();
#endif
                try
                {
                    TypeBuilder tb = _module.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Public, typeof(DynamicClass));
                    FieldInfo[] fields = GenerateProperties(tb, signature.properties);
                    GenerateEquals(tb, fields);
                    GenerateGetHashCode(tb, fields);

                    Type result = tb.CreateType();
                    _classCount++;

                    _classes.Add(signature, result);

                    return result;
                }
                finally
                {
#if ENABLE_LINQ_PARTIAL_TRUST
                    PermissionSet.RevertAssert();
#endif
                }
            }
            finally
            {
#if SILVERLIGHT
                _rwLock.DowngradeToReaderLock();
#else
                _rwLock.ExitWriteLock();
#endif
            }
        }

        private static FieldInfo[] GenerateProperties(TypeBuilder tb, DynamicProperty[] properties)
        {
            MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
            FieldInfo[] fields = new FieldBuilder[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                DynamicProperty dp = properties[i];
                FieldBuilder fb = tb.DefineField("_" + dp.Name, dp.Type, FieldAttributes.Private);
                PropertyBuilder pb = tb.DefineProperty(dp.Name, PropertyAttributes.HasDefault, dp.Type, null);

                MethodBuilder mbGet = tb.DefineMethod("get_" + dp.Name, getSetAttr, dp.Type, Type.EmptyTypes);
                ILGenerator genGet = mbGet.GetILGenerator();
                genGet.Emit(OpCodes.Ldarg_0);
                genGet.Emit(OpCodes.Ldfld, fb);
                genGet.Emit(OpCodes.Ret);
                pb.SetGetMethod(mbGet);

                MethodBuilder mbSet = tb.DefineMethod("set_" + dp.Name, getSetAttr, null, new Type[] { dp.Type });
                ILGenerator genSet = mbSet.GetILGenerator();
                genSet.Emit(OpCodes.Ldarg_0);
                genSet.Emit(OpCodes.Ldarg_1);
                genSet.Emit(OpCodes.Stfld, fb);
                genSet.Emit(OpCodes.Ret);
                pb.SetSetMethod(mbSet);

                fields[i] = fb;
            }

            return fields;
        }

        private static void GenerateEquals(TypeBuilder tb, FieldInfo[] fields)
        {
            MethodBuilder mb = tb.DefineMethod("Equals",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(bool), new Type[] { typeof(object) });
            ILGenerator gen = mb.GetILGenerator();
            LocalBuilder other = gen.DeclareLocal(tb);
            Label next = gen.DefineLabel();
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Isinst, tb);
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


        private class Signature : IEquatable<Signature>
        {
            public DynamicProperty[] properties;
            public int hashCode;

            public Signature(IEnumerable<DynamicProperty> properties)
            {
                this.properties = properties.ToArray();
                hashCode = 0;
                foreach (DynamicProperty p in properties)
                {
                    hashCode ^= p.Name.GetHashCode() ^ p.Type.GetHashCode();
                }
            }

            public override int GetHashCode()
            {
                return hashCode;
            }

            public override bool Equals(object obj)
            {
                var other = obj as Signature;

                if (other != null) return Equals(other);

                return false;
            }

            public bool Equals(Signature other)
            {
                if (other == null) return false;
                if (properties.Length != other.properties.Length) return false;

                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].Name != other.properties[i].Name ||
                        properties[i].Type != other.properties[i].Type) return false;
                }
                return true;
            }
        }
    }
}

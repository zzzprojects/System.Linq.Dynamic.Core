//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Microsoft.Dnx.Compilation;
//using Microsoft.Dnx.Compilation.CSharp;
//using Microsoft.Extensions.PlatformAbstractions;
//using System;
//using System.Collections.Generic;
//using System.Collections.Immutable;
//using System.IO;
//using System.Linq;
//using System.Linq.Dynamic.Core;
//using System.Reflection;
//using System.Threading;

//namespace Kendo.Mvc.Infrastructure.Implementation
//{
//    public class ClassFactoryTel
//    {
//        public static readonly ClassFactoryTel Instance;

//        private readonly IAssemblyLoadContext loader;

//        private int classCount;

//        private readonly ReaderWriterLockSlim rwLock;

//        private static string TO_STRING_METHOD_TEMPLATE;

//        static ClassFactoryTel()
//        {
//            ClassFactoryTel.Instance = new ClassFactoryTel((IAssemblyLoadContextAccessor)CallContextServiceLocator.Locator.ServiceProvider.GetService(typeof(IAssemblyLoadContextAccessor)));
//            ClassFactoryTel.TO_STRING_METHOD_TEMPLATE = "public override string ToString() {var props = this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public); var sb = new System.Text.StringBuilder();sb.Append(\"{\"); for (int i = 0; i < props.Length; i++) {     if (i > 0) sb.Append(\", \");     sb.Append(props[i].Name);     sb.Append(\"=\");    sb.Append(props[i].GetValue(this, null));} sb.Append(\"}\"); return sb.ToString(); }";
//        }

//        private ClassFactoryTel(IAssemblyLoadContextAccessor loaderAccessor)
//        {
//            this.loader = loaderAccessor.GetLoadContext(IntrospectionExtensions.GetTypeInfo(typeof(ClassFactoryTel)).Assembly);
//            this.rwLock = new ReaderWriterLockSlim();
//        }

//        public Type GetDynamicClass(IEnumerable<DynamicProperty> properties)
//        {
//            string typeName = "DynamicClass" + (this.classCount + 1);
//            CompilationUnitSyntax compilationUnitSyntax = this.DeclareCompilationUnit().AddMembers(new MemberDeclarationSyntax[]
//            {
//                this.DeclareClass(typeName).AddMembers(Enumerable.ToArray<PropertyDeclarationSyntax>(Enumerable.Select<DynamicProperty, PropertyDeclarationSyntax>(properties, new Func<DynamicProperty, PropertyDeclarationSyntax>(this.DeclareDynamicProperty)))).AddMembers(new MemberDeclarationSyntax[]
//                {
//                    this.DeclareToStringMethod()
//                })
//            });
//            CSharpCompilation compilation = this.CreateCompilation(compilationUnitSyntax.SyntaxTree);
//            this.IncrementClassCounter();
//            return this.EmitType(typeName, compilation);
//        }

//        private Type EmitType(string typeName, CSharpCompilation compilation)
//        {
//            Type type;
//            using (MemoryStream memoryStream = new MemoryStream())
//            {
//                if (!compilation.Emit(memoryStream, null, null, null, null, null, null, default(CancellationToken)).Success)
//                {
//                    throw new Exception("Unable to build type" + typeName);
//                }
//                memoryStream.Seek(0L, 0);
//                type = this.loader.LoadStream(memoryStream, null).GetType(typeName);
//            }
//            return type;
//        }

//        private void IncrementClassCounter()
//        {
//            this.rwLock.EnterWriteLock();
//            try
//            {
//                this.classCount++;
//            }
//            finally
//            {
//                this.rwLock.ExitWriteLock();
//            }
//        }

//        private ClassDeclarationSyntax DeclareClass(string typeName)
//        {
//            return SyntaxFactory.ClassDeclaration(typeName).AddModifiers(new SyntaxToken[]
//            {
//                SyntaxFactory.Token(SyntaxKind.PublicKeyword)
//            });
//        }

//        private CompilationUnitSyntax DeclareCompilationUnit()
//        {
//            return SyntaxFactory.CompilationUnit().AddUsings(new UsingDirectiveSyntax[]
//            {
//                SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System"))
//            });
//        }

//        private CSharpCompilation CreateCompilation(SyntaxTree syntaxTree)
//        {
//            string arg_53_0 = "DynamicClasses" + this.classCount;
//            CSharpCompilationOptions cSharpCompilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, null, null, null, null, 0, false, false, null, null, default(ImmutableArray<byte>), default(bool?), 0, 0, 4, null, true, false, null, null, null, null, null);
//            return CSharpCompilation.Create(arg_53_0, new SyntaxTree[]
//            {
//                syntaxTree
//            }, this.GetReferences(), cSharpCompilationOptions);
//        }

//        private IEnumerable<MetadataReference> GetReferences()
//        {
//            List<MetadataReference> list = new List<MetadataReference>();
//            LibraryExport export = ((ILibraryExporter)CallContextServiceLocator.Locator.ServiceProvider.GetService(typeof(ILibraryExporter))).GetExport("System");
//            if (export != null)
//            {
//                using (IEnumerator<IMetadataReference> enumerator = export.MetadataReferences.GetEnumerator())
//                {
//                    while (enumerator.MoveNext())
//                    {
//                        IMetadataReference current = enumerator.Current;
//                        IRoslynMetadataReference roslynMetadataReference = current as IRoslynMetadataReference;
//                        if (roslynMetadataReference != null)
//                        {
//                            list.Add(roslynMetadataReference.MetadataReference);
//                            break;
//                        }
//                        IMetadataFileReference metadataFileReference = current as IMetadataFileReference;
//                        if (metadataFileReference != null)
//                        {
//                            AssemblyMetadata assemblyMetadata = AssemblyMetadata.CreateFromStream(File.OpenRead(metadataFileReference.Path), false);
//                            list.Add(assemblyMetadata.GetReference(null, default(ImmutableArray<string>), false, null, null));
//                            break;
//                        }
//                    }
//                }
//            }

//            if (list.Count() == 0)
//            {
//                throw new InvalidOperationException("Unable to create MetadataReference");
//            }
//            return list;
//        }

//        private PropertyDeclarationSyntax DeclareDynamicProperty(DynamicProperty property)
//        {
//            return SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(property.Type.FullName, 0, true), property.Name).WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword))).WithAccessorList(SyntaxFactory.AccessorList(SyntaxFactory.List<AccessorDeclarationSyntax>(new AccessorDeclarationSyntax[]
//            {
//                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, null).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
//                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration, null).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
//            })));
//        }

//        private MethodDeclarationSyntax DeclareToStringMethod()
//        {
//            return Enumerable.First<SyntaxNode>(CSharpSyntaxTree.ParseText(ClassFactoryTel.TO_STRING_METHOD_TEMPLATE, null, "", null, default(CancellationToken)).GetRoot(default(CancellationToken)).ChildNodes()) as MethodDeclarationSyntax;
//        }
//    }
//}

// Description: C# Eval Function | Evaluate, Compile and Execute C# code and expression at runtime. 
// Website & Documentation: https://github.com/zzzprojects/Eval-Expression.NET
// Forum & Issues: https://github.com/zzzprojects/Eval-Expression.NET/issues
// License: https://github.com/zzzprojects/Eval-Expression.NET/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2017. All rights reserved.

using System.Collections.Generic;

namespace System.Linq.Dynamic.Core
{
    internal static class Dynamic
    {
        internal static object DynamicIndex(object obj, string name)
        {
            // CAUTION: This method is called via reflection, so even with 0 reference, the method is used
            // var method = typeof(Dynamic).GetMethod("DynamicIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            // return Expression.Call(null, method, instance, Expression.Constant(id));

            // TBD: If we want to really support Expando Object for properties & method
            // - We will need to add a reference to: Microsoft.CSharp.dll
            // - Copy source from Z.Expressions.Eval\Z.Expressions.Compiler.Shared\CodeCompiler\CSharp\Compiler\Dynamic\DynamicIndexer.cs and other files

            // At this moment, this is only a very quick fix for issue: https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/397
            // To replace DynamicGetMemberBinder.cs old logic that was caching the result

            var dictionary = obj as IDictionary<string, object>;
            if (dictionary == null)
            {
                throw new InvalidOperationException("Target object is not an ExpandoObject");
            }

            return dictionary[name];
        }
    }
}

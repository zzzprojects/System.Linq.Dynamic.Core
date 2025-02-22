﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Dynamic.Core.Config;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Dynamic.Core.Util.Cache;

namespace System.Linq.Dynamic.Core;

/// <summary>
/// Configuration class for System.Linq.Dynamic.Core.
/// </summary>
public class ParsingConfig
{
    private IDynamicLinqCustomTypeProvider? _customTypeProvider;
    private IExpressionPromoter? _expressionPromoter;
    private IQueryableAnalyzer? _queryableAnalyzer;

    /// <summary>
    /// Default ParsingConfig
    /// </summary>
    public static ParsingConfig Default { get; } = new();

    /// <summary>
    /// Default ParsingConfig for EntityFramework Core 2.1 and higher
    /// </summary>
    public static ParsingConfig DefaultEFCore21 { get; } = new()
    {
        EvaluateGroupByAtDatabase = true
    };

    /// <summary>
    /// Default ParsingConfig for CosmosDb
    /// </summary>
    public static ParsingConfig DefaultCosmosDb { get; } = new()
    {
        RenameEmptyParameterExpressionNames = true
    };

    /// <summary>
    /// Defines if the resolution should be case-sensitive for:
    /// - fields and properties
    /// - (extension) methods
    /// - constant expressions ("null", "true", "false")
    /// - keywords ("it", "parent", "root")
    /// - functions ("as", "cast", "iif", "is", "isnull", "new", "np")
    /// - operator aliases ("eq", "equal", "ne", "notequal", "neq", "lt", "LessThan", "le", "LessThanEqual", "gt", "GreaterThan", "ge", "GreaterThanEqual", "and", "AndAlso", "or", "OrElse", "not", "mod")
    ///
    /// Default value is <c>false</c>.
    /// </summary>
    public bool IsCaseSensitive { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="IDynamicLinqCustomTypeProvider"/>.
    /// </summary>
    public IDynamicLinqCustomTypeProvider? CustomTypeProvider
    {
        get
        {
#if !(UAP10_0 || NETSTANDARD)
            // Only use DefaultDynamicLinqCustomTypeProvider for full .NET Framework and .NET Core App 2.x and higher.
            return _customTypeProvider ??= new DefaultDynamicLinqCustomTypeProvider(this);
#else
            return _customTypeProvider;
#endif
        }
        set
        {
            _customTypeProvider = value;
        }
    }

    /// <summary>
    /// Sets the CustomTypeProvider to <see cref="DefaultDynamicLinqCustomTypeProvider"/>.
    /// </summary>
    /// <param name="cacheCustomTypes">Defines whether to cache the CustomTypes (including extension methods) which are found in the Application Domain. Default set to <c>true</c>.</param>
    public void UseDefaultDynamicLinqCustomTypeProvider(bool cacheCustomTypes = true)
    {
        _customTypeProvider = new DefaultDynamicLinqCustomTypeProvider(this, cacheCustomTypes);
    }

    /// <summary>
    /// Sets the CustomTypeProvider to <see cref="DefaultDynamicLinqCustomTypeProvider"/>.
    /// </summary>
    /// <param name="cacheCustomTypes">Defines whether to cache the CustomTypes (including extension methods) which are found in the Application Domain. Default set to <c>true</c>.</param>
    /// <param name="additionalTypes">A list of additional types (without the DynamicLinqTypeAttribute annotation) which should also be resolved.</param>
    public void UseDefaultDynamicLinqCustomTypeProvider(IList<Type> additionalTypes, bool cacheCustomTypes = true)
    {
        _customTypeProvider = new DefaultDynamicLinqCustomTypeProvider(this, additionalTypes, cacheCustomTypes);
    }

    /// <summary>
    /// Load additional assemblies from the current domain base directory.
    /// Note: only used when full .NET Framework and .NET Core App 2.x and higher.
    ///
    /// Default value is <c>false</c>.
    /// </summary>
    public bool LoadAdditionalAssembliesFromCurrentDomainBaseDirectory { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="IExpressionPromoter"/>.
    /// </summary>
    public IExpressionPromoter ExpressionPromoter
    {
        get => _expressionPromoter ??= new ExpressionPromoter(this);
        set
        {
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (_expressionPromoter != value)
            {
                _expressionPromoter = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="IQueryableAnalyzer"/>.
    /// </summary>
    public IQueryableAnalyzer QueryableAnalyzer
    {
        get
        {
            return _queryableAnalyzer ??= new DefaultQueryableAnalyzer();
        }
        set
        {
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (_queryableAnalyzer != value)
            {
                _queryableAnalyzer = value;
            }
        }
    }

    /// <summary>
    /// Determines if the context keywords (it, parent, and root) are valid and usable inside a Dynamic Linq string expression.  
    /// Does not affect the usability of the equivalent context symbols ($, ^ and ~).
    /// 
    /// Default value is <c>false</c>.
    /// </summary>
    public bool AreContextKeywordsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the EntityFramework version supports evaluating GroupBy at database level.
    /// See https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-2.1#linq-groupby-translation
    /// Remark: when this setting is set to <c>true</c>, make sure to supply this ParsingConfig as first parameter on the extension methods.
    ///
    /// Default value is <c>false</c>.
    /// </summary>
    public bool EvaluateGroupByAtDatabase { get; set; }

    /// <summary>
    /// Use Parameterized Names in generated dynamic SQL query.
    /// See https://github.com/graeme-hill/gblog/blob/master/source_content/articles/2014.139_entity-framework-dynamic-queries-and-parameterization.mkd
    ///
    /// Default value is <c>false</c>.
    /// </summary>
    public bool UseParameterizedNamesInDynamicQuery { get; set; }

    /// <summary>
    /// Allows the New() keyword to evaluate any available Type.
    ///
    /// Default value is <c>false</c>.
    /// </summary>
    public bool AllowNewToEvaluateAnyType { get; set; }

    /// <summary>
    /// Renames the (Typed)ParameterExpression empty Name to the correct supplied name from `it`.
    ///
    /// Default value is <c>false</c>.
    /// </summary>
    public bool RenameParameterExpression { get; set; }

    /// <summary>
    /// Prevents any System.Linq.Expressions.ParameterExpression.Name value from being empty by substituting a random 16 character word.
    /// 
    /// Default value is <c>false</c>.
    /// </summary>
    public bool RenameEmptyParameterExpressionNames { get; set; }

    /// <summary>
    /// By default, when a member is not found in a type and the type has a string based index accessor it will be parsed as an index accessor.
    /// Use this flag to disable this behaviour and have parsing fail when parsing an expression where a member access on a non-existing member happens.
    ///
    /// Default value is <c>false</c>.
    /// </summary>
    public bool DisableMemberAccessToIndexAccessorFallback { get; set; }

    /// <summary>
    /// By default, finding types by a simple name is not supported.
    /// Use this flag to use the CustomTypeProvider to resolve types by a simple name like "Employee" instead of "MyDatabase.Entities.Employee".
    /// Note that a first matching type is returned and this functionality needs to scan all types from all assemblies, so use with caution.
    /// 
    /// Default value is <c>false</c>.
    /// </summary>
    public bool ResolveTypesBySimpleName { get; set; }

    /// <summary>
    /// Support enumeration-types from the System namespace in mscorlib. An example could be "StringComparison".
    /// 
    /// Default value is <c>true</c>.
    /// </summary>
    public bool SupportEnumerationsFromSystemNamespace { get; set; } = true;

    /// <summary>
    /// By default, a DateTime (like 'Fri, 10 May 2019 11:03:17 GMT') is parsed as local time.
    /// Use this flag to parse all DateTime strings as UTC.
    ///
    /// Default value is <c>false</c>.
    /// </summary>
    public bool DateTimeIsParsedAsUTC { get; set; }

    /// <summary>
    /// The number parsing culture.
    ///
    /// Default value is CultureInfo.InvariantCulture
    /// </summary>
    public CultureInfo? NumberParseCulture { get; set; } = CultureInfo.InvariantCulture;

    /// <summary>
    /// Additional TypeConverters
    /// </summary>
    public IDictionary<Type, TypeConverter>? TypeConverters { get; set; }

    /// <summary>
    /// When using the NullPropagating function np(...), use a "default value" for non-nullable value types instead of "null value".
    /// 
    /// Default value is <c>false</c>.
    /// </summary>
    public bool NullPropagatingUseDefaultValueForNonNullableValueTypes { get; set; }

    /// <summary>
    /// Support casting to a full qualified type using a string (double-quoted value).
    /// <code>
    /// var result = queryable.Select($"\"System.DateTime\"(LastUpdate)");
    /// </code>
    /// 
    /// Default value is <c>true</c>.
    /// </summary>
    public bool SupportCastingToFullyQualifiedTypeAsString { get; set; } = true;

    /// <summary>
    /// When the type and property have the same name the parser takes the property instead of type when this setting is set to <c>true</c>.
    /// This setting is also used for calling ExtensionMethods.
    ///
    /// Default value is <c>true</c>.
    /// </summary>
    public bool PrioritizePropertyOrFieldOverTheType { get; set; } = true;

    /// <summary>
    /// Support a "." in a property-name. Used in the 'new (a.b as a.b)' syntax.
    /// 
    /// Default value is <c>false</c>.
    /// </summary>
    public bool SupportDotInPropertyNames { get; set; }

    /// <summary>
    /// Disallows the New() keyword to be used to construct a class.
    ///
    /// Default value is <c>false</c>.
    /// </summary>
    public bool DisallowNewKeyword { get; set; }

    /// <summary>
    /// Caches constant expressions to enhance performance. Periodic cleanup is performed to manage cache size, governed by this configuration.
    /// </summary>
    public CacheConfig? ConstantExpressionCacheConfig { get; set; }

    /// <summary>
    /// Converts typeof(object) to the correct type to allow comparison (Equal, NotEqual, GreaterThan, GreaterThanEqual, LessThan and LessThanEqual).
    ///
    /// Default value is <c>false</c>.
    ///
    /// When set to <c>true</c>, the following code will work correct:
    /// <example>
    /// <code>
    /// <![CDATA[
    /// class Person
    /// {
    ///     public string Name { get; set; }
    ///     public object Age { get; set; }
    /// }
    ///
    /// var persons = new[]
    /// {
    ///     new Person { Name = "Foo", Age = 99 },
    ///     new Person { Name = "Bar", Age = 33 }
    /// }.AsQueryable();
    ///
    /// var config = new ParsingConfig
    /// {
    ///     ConvertObjectToSupportComparison = true
    /// };
    /// 
    /// var results = persons.Where(config, "Age > 50").ToList();
    /// ]]>
    /// </code>
    /// </example>
    /// </summary>
    public bool ConvertObjectToSupportComparison { get; set; }

    /// <summary>
    /// Defines the type of string literal parsing that will be performed.
    /// Default value is <c>StringLiteralParsingType.Default</c>.
    /// </summary>
    public StringLiteralParsingType StringLiteralParsing { get; set; } = StringLiteralParsingType.Default;

    /// <summary>
    /// When set to <c>true</c>, the parser will restrict the OrderBy and ThenBy methods to only allow properties or fields. If set to <c>false</c>, any expression is allowed.
    ///
    /// Default value is <c>true</c>.
    /// </summary>
    public bool RestrictOrderByToPropertyOrField { get; set; } = true;

    /// <summary>
    /// When set to <c>true</c>, the parser will allow the use of the Equals(object obj), Equals(object objA, object objB), ReferenceEquals(object objA, object objB) and ToString() methods on the <see cref="object"/> type.
    ///
    /// Default value is <c>false</c>.
    /// </summary>
    public bool AllowEqualsAndToStringMethodsOnObject { get; set; }
}
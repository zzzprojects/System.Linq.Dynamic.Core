# v1.4.0 (31 May 2024)
- [#807](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/807) - PredefinedTypesHelper: Fix loading types from System.Data.Entity and EntityFramework [bug] contributed by [StefH](https://github.com/StefH)
- [#790](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/790) - .NET 8 (and possible all non-NETSTANDARD and higher versions) attempt to load EF 6 types [bug]
- [#812](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/812) - &gt;= 1.3.3 Slow First Dynamic Select

# v1.3.14 (03 May 2024)
- [#804](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/804) - Fix calling methods on a constant string [bug] contributed by [StefH](https://github.com/StefH)
- [#806](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/806) - Support implicit boolean operator for logical operations (And, Or) [feature] contributed by [StefH](https://github.com/StefH)
- [#808](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/808) - Fix calling Average without any arguments [bug] contributed by [StefH](https://github.com/StefH)
- [#809](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/809) - Add unit test: Where_Dynamic_ConcatString [test] contributed by [StefH](https://github.com/StefH)
- [#811](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/811) - Add extra unit test for calling methods on a string property [test] contributed by [StefH](https://github.com/StefH)
- [#626](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/626) - '&amp;&amp;' and '||' operators incompatible between operand type 'bool' and type with implicit conversation to 'bool' [bug]
- [#633](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/633) - Fail to GroupBy Select Where Average [bug]
- [#783](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/783) - Performance Issue with EF Core 8.0 and Microsoft.EntityFrameworkCore.DynamicLinq Package [duplicate]
- [#803](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/803) - Updating to 1.3.13 - No property or field 'XXXXX' exists in type 'Char' [bug]
- [#810](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/810) - Issue with Where() method when filtering multiple string properties while using ToLower() [bug]

# v1.3.13 (25 April 2024)
- [#802](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/802) - Fix DynamicExpressionParser for IQueryable [bug] contributed by [StefH](https://github.com/StefH)
- [#801](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/801) - ExpressionParser finds Types from IEnumerable instead of IQueryable (breaks Code) (worked in 1.3.9) [bug]

# v1.3.12 (18 April 2024)
- [#800](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/800) - Fix String.Equals together with other condition [bug] contributed by [StefH](https://github.com/StefH)
- [#799](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/799) - Error with 'Equals' together with other parameters [bug]

# v1.3.11 (17 April 2024)
- [#785](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/785) - Add CodeFirst examples for .NET 6 and 8 [test] contributed by [StefH](https://github.com/StefH)
- [#795](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/795) - Fix Aggregate methods Average and Sum [bug] contributed by [StefH](https://github.com/StefH)
- [#796](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/796) - Fixed ParseArgumentList when using lambda filter on list of dictionaries [bug] contributed by [StefH](https://github.com/StefH)
- [#798](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/798) - Fix when using LINQ methods like &quot;Any&quot; on a string [bug] contributed by [StefH](https://github.com/StefH)
- [#415](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/415) - Problem with Query over JObject, Filter with SubProperty not returning the correct value [bug]
- [#440](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/440) - `it` isn't interpreted as the inner parameter when using `All` or `Any` [bug]
- [#793](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/793) - Failure in ParseArgumentList when using lambda filter on list of dictionaries [bug]
- [#794](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/794) - DynamicExpressionParser.ParseLambda regressions in v1.3.10 [bug]

# v1.3.10 (03 March 2024)
- [#774](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/774) - Fixed calling property on nested static class [bug] contributed by [StefH](https://github.com/StefH)
- [#777](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/777) - Keep original type from subquery [bug] contributed by [StefH](https://github.com/StefH)
- [#772](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/772) - Static helper class with name as property doesn't work after updating from v.1.0.8.18 to 1.3.8 [bug]
- [#775](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/775) - Value typed property from subquery are casted as `object` [bug]
- [#779](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/779) - try to call function inside select [bug]

# v1.3.9 (29 February 2024)
- [#761](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/761) - Add unit tests for Any [test] contributed by [StefH](https://github.com/StefH)
- [#765](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/765) - Introduce Sliding Cache to Constant Expression Helper [feature] contributed by [TWhidden](https://github.com/TWhidden)
- [#770](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/770) - Enhancements to SlidingCache&lt;&gt; for Improved Thread Safety and Performance [feature] contributed by [TWhidden](https://github.com/TWhidden)
- [#780](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/780) - Add RadzenDataGrid.BlazorApp example [test] contributed by [StefH](https://github.com/StefH)
- [#764](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/764) - Possible Memory Leak with ConstantExpressionHelper [bug]
- [#769](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/769) - Enhancements to SlidingCache&lt;&gt; for Improved Thread Safety and Performance [bug]

# v1.3.8 (20 January 2024)
- [#758](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/758) - Update function argument parsing for strings [bug] contributed by [StefH](https://github.com/StefH)
- [#760](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/760) - Update function argument parsing for strings (part 2) [bug] contributed by [StefH](https://github.com/StefH)
- [#757](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/757) - ParseException after update from v.1.0.8.18 to 1.0.9.0 or any newer (till 1.3.7) [bug]
- [#759](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/759) - Still yet ParseException after update from v.1.0.8.18 to 1.0.9.0 or any newer (till 1.3.7) [bug]

# v1.3.7 (20 November 2023)
- [#756](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/756) - Add support for .NET 8 + Microsoft.EntityFrameworkCore 8 [feature] contributed by [StefH](https://github.com/StefH)

# v1.3.6 (19 November 2023)
- [#750](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/750) - DynamicLinqTypeAttribute can now also be defined on an interface [feature] contributed by [StefH](https://github.com/StefH)
- [#755](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/755) - Fix KeywordsHelper [bug] contributed by [StefH](https://github.com/StefH)
- [#745](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/745) - The `DynamicLinqType` attribute can not be added to interfaces.  [refactor]
- [#754](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/754) - Parameters of lambdas in the supplied expression have lower priority than defined types, even when casing differs [bug]

# v1.3.5 (21 September 2023)
- [#728](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/728) - added symbol check when checking for identifier [feature] contributed by [abbasc52](https://github.com/abbasc52)
- [#730](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/730) - Add PackageReadme to NuGet [feature] contributed by [StefH](https://github.com/StefH)
- [#731](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/731) - Add logic to convert any array to object array. [feature] contributed by [StefH](https://github.com/StefH)
- [#732](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/732) - Add config setting: DisallowNewKeyword [feature] contributed by [StefH](https://github.com/StefH)
- [#739](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/739) - Fix AddSignatures and SubtractSignatures [bug] contributed by [StefH](https://github.com/StefH)
- [#743](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/743) - Add support for out keyword [feature] contributed by [StefH](https://github.com/StefH)
- [#744](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/744) - Add some more tests for Max [test] contributed by [StefH](https://github.com/StefH)
- [#389](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/389) -  ExpressionPromoter.Promote Limitations to Detect plausible method matches [feature]
- [#727](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/727) - Type from registered type has higher preference than input parameter expression with same name [feature]
- [#737](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/737) - Exception when trying to add/subtract types 'DateTime' and 'TimeSpan' [bug]
- [#741](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/741) - out keyword is not working with dynamic linq [feature]

# v1.3.4 (03 August 2023)
- [#723](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/723) - Handle UseParameterizedNamesInDynamicQuery when parsing dynamic OfType function contributed by [yonguelink](https://github.com/yonguelink)
- [#722](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/722) - UseParameterizedNamesInDynamicQuery set to true breaks `OfType` method call

# v1.3.3 (24 June 2023)
- [#686](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/686) - Add support for casting a string to another type (e.g. int) for Linq2Objects [feature] contributed by [StefH](https://github.com/StefH)
- [#703](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/703) -  Update logic for AbstractDynamicLinqCustomTypeProvider.GetAssemblyTypesWithDynamicLinqTypeAttribute [feature] contributed by [StefH](https://github.com/StefH)
- [#704](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/704) - Fix Average to support nullable types [bug] contributed by [StefH](https://github.com/StefH)
- [#714](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/714) - Add support for methods on Enum [feature] contributed by [StefH](https://github.com/StefH)
- [#717](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/717) - Extend ParseException with InnerException and StackTrace [feature] contributed by [StefH](https://github.com/StefH)
- [#719](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/719) - SupportDotInPropertyNames [feature] contributed by [StefH](https://github.com/StefH)
- [#720](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/720) - Fixed result value for bitwise operators when using enums [bug] contributed by [StefH](https://github.com/StefH)
- [#695](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/695) - Flags enum with bitwise operator returns as int; should be the enum type [bug]
- [#696](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/696) - Methods on type 'Enum' are not accessible. [bug]
- [#700](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/700) - Dynamic Average does not support nullable types [bug]
- [#716](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/716) - ParseException ToString() does not contain stack trace [feature]

# v1.3.2 (01 April 2023)
- [#679](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/679) - Add some tests for DynamicClass with System.Text.Json [test] contributed by [StefH](https://github.com/StefH)
- [#685](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/685) - Add test for DynamicExpressionParser string with a dot [test] contributed by [StefH](https://github.com/StefH)
- [#687](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/687) - Fix parsing a string literal with a dot [bug] contributed by [StefH](https://github.com/StefH)
- [#691](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/691) - Add internal Clear method to DynamicClassFactory for unit-testing. [test] contributed by [StefH](https://github.com/StefH)
- [#693](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/693) - UnitTests: add and use SkipIfGitHubActionsAttribute [test] contributed by [StefH](https://github.com/StefH)
- [#694](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/694) - Add extra unit test for OfType [test] contributed by [StefH](https://github.com/StefH)
- [#683](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/683) - DynamicExpression parser can't parse when there is a dot in a string [bug]
- [#692](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/692) - OfType(string) fails with &quot;'.' or '(' or string literal expected&quot; when AllowNewToEvaluateAnyType is true on version 1.2.24 and above

# v1.3.1 (06 March 2023)
- [#680](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/680) - Bump MongoDB.Driver from 2.4.4 to 2.19.0 in /test/EntityFramework.DynamicLinq.Tests [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#681](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/681) - Bump MongoDB.Driver from 2.4.4 to 2.19.0 in /test/EntityFramework.DynamicLinq.Tests.net452 [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#684](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/684) - Add more try-catch logic to DefaultAssemblyHelper and AbstractDynamicLinqCustomTypeProvider [feature] contributed by [StefH](https://github.com/StefH)

# v1.3.0 (03 March 2023)
- [#669](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/669) - Methods should only be callable on predefined types [security] contributed by [StefH](https://github.com/StefH)
- [#671](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/671) - Add support for DateOnly and TimeOnly [feature] contributed by [StefH](https://github.com/StefH)
- [#672](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/672) - Fixed ExpressionParser when WrappedValue-string is used for equals-operator for Enum [bug] contributed by [StefH](https://github.com/StefH)
- [#676](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/676) - Change default setting for PrioritizePropertyOrFieldOverTheType to 'true'. [bug] contributed by [StefH](https://github.com/StefH)
- [#660](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/660) - Question about security [security]
- [#667](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/667) - DateOnly support [feature]
- [#668](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/668) - With UseParameterizedNamesInDynamicQuery, can't compare enum type with String [bug]
- [#675](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/675) - Expression does not parse any more 1.2.25 (works in 1.2.24) [bug]

# v1.2.25 (05 February 2023)
- [#664](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/664) - Add config setting for PrioritizePropertyOrFieldOverTheType  [feature] contributed by [StefH](https://github.com/StefH)
- [#665](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/665) - Update AbstractDynamicLinqCustomTypeProvider to exclude null types [bug] contributed by [StefH](https://github.com/StefH)
- [#666](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/666) -  Fixed ExpressionParser when WrappedValue-string is used for equals-operator contributed by [StefH](https://github.com/StefH)
- [#645](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/645) - With UseParameterizedNamesInDynamicQuery, can't  compare DateTimeOffset with String [bug]
- [#662](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/662) - System.Linq.Dynamic.Core.Exceptions.ParseException : No applicable method 'DateTime' exists in type [feature]

# v1.2.24 (18 December 2022)
- [#621](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/621) - Fix Join on inherited class [bug] contributed by [StefH](https://github.com/StefH)
- [#646](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/646) - Add more unittests for issue 645 [feature] contributed by [StefH](https://github.com/StefH)
- [#647](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/647) - Support nullable notation &quot;xxx?&quot; in As expression [feature] contributed by [StefH](https://github.com/StefH)
- [#649](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/649) - Fix DynamicExpressionParser and ExpressionPromoter to support LambdExpression [bug] contributed by [StefH](https://github.com/StefH)
- [#653](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/653) - Add support to cast to a fully qualified type [feature] contributed by [StefH](https://github.com/StefH)
- [#614](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/614) - Join problem with inherited entities [bug]
- [#652](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/652) - Implement casting to fully qualified type [feature]

# v1.2.23 (12 November 2022)
- [#644](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/644) - Add support for .NET 7 and EF Core 7 [feature] contributed by [StefH](https://github.com/StefH)

# v1.2.22 (06 November 2022)
- [#642](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/642) - Rename extension method &quot;AsEnumerable&quot; to &quot;AsDynamicEnumerable&quot;. [feature] contributed by [StefH](https://github.com/StefH)
- [#304](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/304) - System.Linq.Dynamic.Core.DynamicQueryableExtensions.AsEnumerable conflicts with System.Linq.Enumerable.AsEnumerable [bug]

# v1.2.21 (28 October 2022)
- [#627](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/627) - Use PackageIcon + upgrade JetBrains.Annotations [feature] contributed by [StefH](https://github.com/StefH)
- [#630](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/630) - Fix MethodFinder to return topmost implementation of the virtual method [feature] contributed by [jogibear9988](https://github.com/jogibear9988)
- [#631](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/631) - Init field only on first execution [bug] contributed by [BBreiden](https://github.com/BBreiden)
- [#636](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/636) - Fix nullable issues [bug] contributed by [StefH](https://github.com/StefH)
- [#637](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/637) - Fixed accessing dynamic properties by index [bug] contributed by [StefH](https://github.com/StefH)
- [#641](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/641) - Add unit test for DynamicClass SerializeToJson [feature] contributed by [StefH](https://github.com/StefH)
- [#580](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/580) - C# Expressions use Base Class Virtual Methodinfo [bug]
- [#629](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/629) - Error accessing values in DynamicClass - fails on second attempt [bug]
- [#634](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/634) - After Upgrade NuGet-Package to 1.2.20 accessing dynamic properties by index fails [bug]
- [#635](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/635) - Compile errors due to missing nullable reference annotations on APIs that accept null [bug]
- [#640](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/640) - NullReferenceException during json serialize of DynamicClass [bug]

# v1.2.20 (03 September 2022)
- [#619](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/619) - Fix generic comparer type [bug] contributed by [StefH](https://github.com/StefH)
- [#620](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/620) - ToDynamicListAsync uses IAsyncEnumerable (if applicable) [feature] contributed by [StefH](https://github.com/StefH)
- [#622](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/622) - Add unit tests for &quot;As acting on property&quot; [feature, test] contributed by [StefH](https://github.com/StefH)
- [#625](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/625) - Add unit test for Where with empty string [test] contributed by [StefH](https://github.com/StefH)
- [#617](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/617) - &quot;Failed to compare two elements in the array.&quot; System.InvalidOperationException (V1.2.19 compared to V1.2.18) [feature]

# v1.2.19 (26 June 2022)
- [#579](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/579) - Support Struct for DynamicLinqTypeAttribute [feature] contributed by [StefH](https://github.com/StefH)
- [#583](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/583) - Fix example 'ConsoleApp_netcore2.1_EF2.1.1' [bug] contributed by [StefH](https://github.com/StefH)
- [#584](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/584) - Add more OrderBy unittests [feature] contributed by [StefH](https://github.com/StefH)
- [#585](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/585) - Fix SelectMany when using JArray [bug] contributed by [StefH](https://github.com/StefH)
- [#587](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/587) - Fixed: Parenthesis around an &quot;In&quot; expression raise an exception [bug] contributed by [cambirch](https://github.com/cambirch)
- [#590](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/590) - Bump jQuery from 2.2.2 to 3.0.0 in /web/DynamicLinqWebDocs [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#592](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/592) - Bump Microsoft.NETCore.UniversalWindowsPlatform from 6.0.1 to 6.0.6 in /test-xamarin/WindowsUniversalTestApp16299 [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#594](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/594) - Bump Newtonsoft.Json from 10.0.3 to 13.0.1 in /src-console/ConsoleAppEF2.1 [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#599](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/599) - Bump Newtonsoft.Json from 8.0.3 to 13.0.1 in /web/DynamicLinqWebDocs [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#609](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/609) - Bump Newtonsoft.Json from 10.0.3 to 13.0.1 in /src-console/ConsoleAppEF6_InMemory [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#612](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/612) - Update dependencies (Newtonsoft.Json and more) [dependencies] contributed by [StefH](https://github.com/StefH)
- [#511](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/511) - Strange SelectMany behaviour using JSON [bug]
- [#548](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/548) - IComparer&lt;T&gt; not supported for OrderBy? [feature]
- [#581](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/581) - ConsoleApp_netcore2.1_EF2.1.1 fails to run with exception [bug]
- [#586](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/586) - Parenthesis around an &quot;In&quot; expression raise an exception [bug]

# v1.2.18 (03 March 2022)
- [#573](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/573) - Updated 'new' command (finding a constructor with exact argument-types and same order) [feature] contributed by [StefH](https://github.com/StefH)
- [#574](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/574) - Bump bootstrap from 3.3.6 to 3.4.1 in /web/DynamicLinqWebDocs [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#576](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/576) - fix enums could not be cast to decimal [feature] contributed by [jogibear9988](https://github.com/jogibear9988)
- [#571](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/571) - ParseException: 'Type 'System.IO.DirectoryInfo' not found' [bug]

# v1.2.17 (21 February 2022)
- [#569](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/569) - Fix logic for indexer when parameter-type differs [bug] contributed by [StefH](https://github.com/StefH)

# v1.2.16 (19 February 2022)
- [#562](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/562) - Make TextParser public [feature] contributed by [StefH](https://github.com/StefH)
- [#563](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/563) - GitHub Actions: use actions/checkout@v2 [security] contributed by [StefH](https://github.com/StefH)
- [#567](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/567) - Add support for property for Is,As and Cast [feature] contributed by [StefH](https://github.com/StefH)

# v1.2.15 (18 December 2021)
- [#554](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/554) - Support for parsing Binary Integer [feature] contributed by [StefH](https://github.com/StefH)
- [#557](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/557) - bind expressions to correct constructor parameter [feature] contributed by [jonasdaniels](https://github.com/jonasdaniels)
- [#558](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/558) - Allow assembly to be collected [bug] contributed by [hemirunner426](https://github.com/hemirunner426)
- [#561](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/561) - Fix NET35 build + add unit test for OrderBy with IComparer [bug] contributed by [StefH](https://github.com/StefH)
- [#552](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/552) - Missing parse from binary literal to integer [bug]

# v1.2.14 (09 November 2021)
- [#551](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/551) - Use Microsoft.EntityFrameworkCore 6.0.0 [feature] contributed by [StefH](https://github.com/StefH)

# v1.2.13 (09 October 2021)
- [#517](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/517) - support for generic (static) methods, thus also support for extension&#8230; [feature] contributed by [dogguts](https://github.com/dogguts)
- [#546](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/546) - Fix StringParser to handle (escaped) newline correctly [bug] contributed by [StefH](https://github.com/StefH)
- [#547](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/547) - Set DebugType to portable for 'Release' builds [feature] contributed by [StefH](https://github.com/StefH)
- [#544](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/544) - 'System.Linq.Dynamic.Core.pdb' is using a deprecated debug information level [feature]
- [#545](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/545) - Unable to escape backslashes correctly [bug]

# v1.2.12 (31 July 2021)
- [#535](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/535) - Fix DynamicQueryableExtensions for .NET 6 [bug] contributed by [StefH](https://github.com/StefH)
- [#493](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/493) - System.Linq.Queryable breaking changes in .NET 6  [bug]

# v1.2.11 (19 July 2021)
- [#518](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/518) - Fix exception in DynamicClassFactory.CreateType when using Blazor WebAssembly [bug] contributed by [StefH](https://github.com/StefH)
- [#522](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/522) - Fix logic for np(...) : always add source object and use configurable default value for non-nullable value-types [bug] contributed by [StefH](https://github.com/StefH)
- [#524](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/524) - Parse in operator before comparisons [bug] contributed by [igitur](https://github.com/igitur)
- [#530](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/530) - Add support of Interpolated strings. contributed by [yangzhongke](https://github.com/yangzhongke)
- [#532](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/532) - Fix NumberParsing for double with exponent [bug] contributed by [StefH](https://github.com/StefH)
- [#516](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/516) - Unable to use DynamicClassFactory.CreateType from Blazor WebAssembly [bug]
- [#520](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/520) - np operator ignores single member access when no default value passed in
- [#527](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/527) - Do you have any plan to support interpolated String? [feature]
- [#531](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/531) - Number parsing rejecting doubles in scientific notation [bug]

# v1.2.10 (31 May 2021)
- [#476](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/476) - Add IDynamicLinqCustomTypeProvider contributed by [StefH](https://github.com/StefH)
- [#495](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/495) - Fix ContainsKey in IReadOnlyDictionary&lt;,&gt; [bug] contributed by [StefH](https://github.com/StefH)
- [#496](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/496) - Fixed selecting int property into enum property [bug] contributed by [StefH](https://github.com/StefH)
- [#506](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/506) - Add Concat, Union, Except and Intersect [feature] contributed by [StefH](https://github.com/StefH)
- [#508](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/508) - Create EF6 preview NuGet [feature] contributed by [StefH](https://github.com/StefH)
- [#509](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/509) - Fix np(...) with UnaryExpression [bug] contributed by [StefH](https://github.com/StefH)
- [#510](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/510) - Fix FindMethod for extension methods [bug] contributed by [StefH](https://github.com/StefH)
- [#514](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/514) - Fix Enum [bug] contributed by [StefH](https://github.com/StefH)
- [#438](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/438) - Typo in IDynamicLinkCustomTypeProvider name [bug]
- [#452](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/452) - Filter properties of a derived class on a list of base class objects [bug]
- [#490](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/490) - Selecting int property into enum throws an exception [bug]
- [#494](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/494) - No applicable aggregate method 'ContainsKey(String)' exists [bug]
- [#497](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/497) - Extending with extensions methods (DynamicLinqType attribute) [bug]
- [#499](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/499) - Feature: Support for Concat and optionally Union, Except, Intersect [feature]
- [#513](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/513) - Where throws exception when property name doesn't match enum name [bug]

# v1.2.9 (26 March 2021)
- [#485](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/485) - Add TypeConverters to config [feature] contributed by [StefH](https://github.com/StefH)
- [#488](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/488) - If args count is 0 -&gt; parametereless method is better than method with parameters [bug] contributed by [AndriiZ](https://github.com/AndriiZ)
- [#477](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/477) - How to use Dynamic LINQ with custom types (i.e NodaTime) ?  [feature]
- [#487](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/487) - DynamicExpressionParser.ParseLambda can not parse 'TrimEnd' string method [bug]

# v1.2.8 (13 February 2021)
- [#455](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/455) - Ensure action delegate allows call to void methods contributed by [glopesdev](https://github.com/glopesdev)
- [#480](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/480) - Fix DynamicIndex implementation [bug] contributed by [StefH](https://github.com/StefH)
- [#481](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/481) - Xamarin fix Enum [bug] contributed by [StefH](https://github.com/StefH)
- [#448](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/448) - Dynamic.DynamicIndex is exposed in the expression [bug]
- [#479](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/479) - Xamarin.Forms - DynamicExpressionParser.ParseLambda fails when comparing enum properties by their int value [bug]

# v1.2.7 (26 December 2020)
- [#462](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/462) - Add PatchVersion [feature] contributed by [StefH](https://github.com/StefH)
- [#463](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/463) - Add extension method Where&lt;TSource&gt;(...) with LambdaExpression  [feature] contributed by [StefH](https://github.com/StefH)
- [#464](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/464) - NullPropagation operator: support nullable DateTime contributed by [StefH](https://github.com/StefH)
- [#466](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/466) - Fix Android issue (Could not load the file 'System.Private.Corelib') [bug] contributed by [StefH](https://github.com/StefH)
- [#467](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/467) - Support 'System.Type' in As, Is, Cast and OfType [feature] contributed by [StefH](https://github.com/StefH)
- [#470](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/470) - Add EF 5 NuGet package [feature] contributed by [StefH](https://github.com/StefH)
- [#424](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/424) - How to GroupBy Nullable DateTime Year? [bug]
- [#459](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/459) - Allow `as` and `is` to use instances of `System.Type` [feature]
- [#465](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/465) - Crash on Android (regression in 1.2.6) [bug]
- [#468](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/468) - net5.0 OrderBy problem [bug]
- [#473](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/473) - Exception in System.Linq.Dynamic.Core.Parser.EnumerationsFromMscorlib after update to 1.2.6 [bug]

# v1.2.6 (22 November 2020)
- [#443](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/443) - Fix MethodCallExpression when using NullPropagating (np) contributed by [StefH](https://github.com/StefH)
- [#445](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/445) - Add GitHub action for ci build + unit tests contributed by [StefH](https://github.com/StefH)
- [#446](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/446) - Remove MyGet links from Readme.md contributed by [StefH](https://github.com/StefH)
- [#447](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/447) - Fix Unit tests for net452 and net461 contributed by [StefH](https://github.com/StefH)
- [#449](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/449) - Fix DateTime constructor using ticks [bug] contributed by [StefH](https://github.com/StefH)
- [#450](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/450) - Support the enum UriKind [feature] contributed by [StefH](https://github.com/StefH)
- [#284](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/284) - String(Null) raises Ambiguous error [bug]
- [#432](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/432) - Clarify error message when using np with instance methods [bug]
- [#439](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/439) - Question: DateTime constructor using ticks [bug]
- [#442](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/442) - UriKind is not recognized in Uri constructor [bug]

# v1.2.5 (24 October 2020)

# v1.2.4 (19 October 2020)
- [#429](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/429) - save contributed by [Lempireqc](https://github.com/Lempireqc)

# v1.2.3 (11 October 2020)
- [#428](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/428) - Add support for IQueryable.Min and .Max contributed by [gregfullman](https://github.com/gregfullman)
- [#230](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/230) - Request for contribution
- [#403](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/403) - Expression is missing an 'as' clause [bug]
- [#406](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/406) - Expression parameter should be case sensitive
- [#411](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/411) - Can't apply the library while using reflection emit with dynamic linq
- [#414](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/414) - Contains search in an Enum
- [#416](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/416) - &quot;Target object is not an ExpandoObject&quot; exception being thrown when using GroupBy
- [#417](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/417) - Expression is missing an 'as' clause
- [#418](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/418) - Dynamic is not playing nice with EF+
- [#420](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/420) - How to set global date format conversion&#65311;not used utc
- [#423](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/423) - Produce Dynamic LINQ strings from expression trees

# v1.2.2 (19 August 2020)
- [#409](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/409) - save contributed by [Lempireqc](https://github.com/Lempireqc)

# v1.2.1 (08 August 2020)
- [#399](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/399) - Use parameterized names in dynamic query contributed by [ascott18](https://github.com/ascott18)
- [#391](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/391) - SumAsync and CountAsync (maybe more) do not work with EF Core 3.1
- [#404](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/404) - Add support to Max and Min functions

# v1.2.0 (27 July 2020)
- [#402](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/402) - Ef core 3x support contributed by [JonathanMagnan](https://github.com/JonathanMagnan)
- [#386](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/386) - Question: Generic and Param based Custom Type Methods Support
- [#397](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/397) - Dictionary parameter issue
- [#400](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/400) - [Question] OrderBy does not work with nullable navigation properties 

# v1.1.8 (12 July 2020)
- [#398](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/398) - Just add simple test contributed by [Lempireqc](https://github.com/Lempireqc)
- [#393](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/393) - Generate SQL Server Select Statement from a dynamically created table.
- [#394](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/394) - OrderByDynamic: Value cannot be null. (Parameter 'type')  
- [#395](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/395) - SQL between and query
- [#396](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/396) - No property or field 'DynamicFunctions' exists in type 'Log'

# v1.1.7 (06 July 2020)

# v1.1.6 (05 July 2020)
- [#384](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/384) - Request: Please implement ContainsKey for dictionary type 

# v1.1.5 (15 June 2020)
- [#390](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/390) - Fixed loading &quot;Microsoft.EntityFrameworkCore.DynamicLinq.DynamicFunctions&quot; [bug] contributed by [StefH](https://github.com/StefH)
- [#388](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/388) - Version from the dll is 0.0.0.0 (microsoft.entityframeworkcore.dynamiclinq) [bug]

# v1.1.3 (15 June 2020)
- [#351](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/351) - Case insensitive GroupBy() [feature]
- [#363](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/363) - OrderBy with non-english letters give different ordering result on string vs key selector [feature]
- [#382](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/382) - Call nullable method [bug]
- [#385](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/385) - Select Issue in EFCore 3 Update

# v1.1.2 (31 May 2020)
- [#380](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/380) - save contributed by [Lempireqc](https://github.com/Lempireqc)
- [#381](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/381) - save contributed by [Lempireqc](https://github.com/Lempireqc)
- [#383](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/383) - Fixed : calling methods which return a nullable [bug] contributed by [StefH](https://github.com/StefH)
- [#378](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/378) - Versioning Issue in Latest NUGET packages [bug]

# v1.1.1 (14 May 2020)

# 1.1.0.0 (25 April 2020)
- [#326](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/326) - Fixes for parsing escaped / quoted strings [bug] contributed by [StefH](https://github.com/StefH)
- [#307](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/307) - Found problem with backslashes parsing [bug]

# 1.0.24.0 (16 April 2020)
- [#367](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/367) - Azure Pipelines: fix Build (coverlet), use new vmImage and update NuGet dependencies for UnitTests [bug] contributed by [StefH](https://github.com/StefH)
- [#368](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/368) - Support MethodCalls in NullPropagation function : np(...) [feature] contributed by [StefH](https://github.com/StefH)
- [#370](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/370) - Add ValidatedNotNullAttribute (for SonarQube) [refactor] contributed by [StefH](https://github.com/StefH)
- [#366](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/366) - Null propagation cannot be used for primitive type lists (string) [feature]

# 1.0.23.0 (26 March 2020)
- [#357](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/357) - Prioritize property or field over the type / Fix find for static property or field contributed by [konzen](https://github.com/konzen)
- [#360](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/360) - Support for Blazor webassembly [feature] contributed by [julienGrd](https://github.com/julienGrd)
- [#355](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/355) - Parser issue - NullReferenceException [bug]
- [#358](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/358) - [Blazor webassembly] library not working with linker disabled [feature]

# 1.0.22.0 (18 March 2020)
- [#352](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/352) - Nested Cosmos Db compatibility contributed by [countincognito](https://github.com/countincognito)
- [#354](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/354) - Fix correctly type cast of nulls (and other constants) contributed by [rockResolve](https://github.com/rockResolve)
- [#339](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/339) - Error creating Null string [bug]

# 1.0.21.0 (29 February 2020)
- [#340](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/340) - Better error message in case property or field is not present in new() [feature] contributed by [StefH](https://github.com/StefH)
- [#342](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/342) - Fix np(...) logic when default value is supplied [bug] contributed by [StefH](https://github.com/StefH)
- [#343](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/343) - DocFx [feature] contributed by [StefH](https://github.com/StefH)
- [#349](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/349) - Update PagedResult logic [feature] contributed by [StefH](https://github.com/StefH)
- [#353](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/353) - Remove option for 'UseDynamicObjectClassForAnonymousTypes' [bug] contributed by [StefH](https://github.com/StefH)
- [#164](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/164) - Issue: The option `UseDynamicObjectClassForAnonymousTypes` does not work correctly [bug]
- [#337](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/337) - np (NullPropagation) throws NullReferenceException with property on .NET Core 3.1 [bug]

# 1.0.20.0 (11 January 2020)
- [#262](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/262) - Z.EntityFramework.Classic contributed by [StefH](https://github.com/StefH)
- [#286](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/286) - Do not generate IIF(...) when np(...) is used for a single expression [feature] contributed by [StefH](https://github.com/StefH)
- [#309](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/309) - Null propagation for methods [bug] contributed by [StefH](https://github.com/StefH)
- [#321](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/321) - LongCount contributed by [StefH](https://github.com/StefH)
- [#323](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/323) - ParseNumber using CultureInfo from configuration [feature] contributed by [StefH](https://github.com/StefH)
- [#329](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/329) - Fixed ToDynamicArrayAsync + ToDynamicListAsync (add type) [bug] contributed by [StefH](https://github.com/StefH)
- [#336](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/336) - Add EF3.1 example [feature] contributed by [StefH](https://github.com/StefH)
- [#338](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/338) - Fix np(...) [bug] contributed by [StefH](https://github.com/StefH)
- [#302](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/302) - np (NullPropagation) throws NullReferenceException with methods [bug]
- [#311](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/311) - Please support LongCount() [feature]
- [#320](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/320) - TypeHelper#ParseNumber TryParse does not use InvariantCulture [bug]
- [#327](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/327) - Incorrect ToDynamicListAsync(this IEnumerable source, Type type) and ToDynamicArrayAsync(this IEnumerable source, Type type) behavior [bug]

# 1.0.19.0 (29 August 2019)
- [#277](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/277) - DateTimeIsParsedAsUTC [feature] contributed by [StefH](https://github.com/StefH)
- [#281](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/281) - Support for AndAlso and OrElse [feature] contributed by [StefH](https://github.com/StefH)
- [#285](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/285) - Fix certain cases where implicit conversions aren't correctly detected when parsing comparison operators [bug] contributed by [alexweav](https://github.com/alexweav)
- [#287](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/287) - Ensure that one-way implicit conversions also work for value types contributed by [alexweav](https://github.com/alexweav)
- [#290](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/290) - Added SumAsync contributed by [wertzui](https://github.com/wertzui)
- [#292](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/292) - Add ConsoleApp using EF6 Effort contributed by [StefH](https://github.com/StefH)
- [#297](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/297) - Fix for #294 contributed by [david-garcia-garcia](https://github.com/david-garcia-garcia)
- [#298](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/298) - Add 'All', 'Average', 'AverageAsync' and update 'Sum' [feature] contributed by [StefH](https://github.com/StefH)
- [#299](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/299) - Add more PredefinedOperatorAliases [feature] contributed by [StefH](https://github.com/StefH)
- [#268](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/268) - Timezone conversion [feature]
- [#279](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/279) - Support .NET Expression string operators (AndAlso &amp; OrElse) [feature]
- [#294](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/294) - Context lost in object initializer [bug]

# 1.0.18.0 (02 July 2019)
- [#278](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/278) - Nuget System.Linq.Dynamic.Core 1.0.17 - Incorrect version [bug]

# 1.0.17.0 (14 June 2019)
- [#276](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/276) - op_Compare also for single &quot;equals&quot; token [bug] contributed by [nothrow](https://github.com/nothrow)

# 1.0.16.0 (06 June 2019)
- [#275](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/275) - Support Enumerations from System Namespace (e.g. StringComparison) [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.15.0 (20 May 2019)
- [#273](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/273) - Adding support for overloaded op_Equality contributed by [nothrow](https://github.com/nothrow)
- [#272](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/272) - Allow comparing via overloaded equality operator [feature]

# 1.0.14.0 (14 May 2019)
- [#270](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/270) - Fix for np() opererator for Nullable (e.g. DateTime) contributed by [StefH](https://github.com/StefH)
- [#105](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/105) - Feature: Support for EF Core 2.0's EF.Functions.Like() [feature]
- [#269](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/269) - np() opererator fails for (Nullable) DateTime [bug]

# 1.0.13.0 (03 May 2019)
- [#264](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/264) - Fix escape characters parsing [bug] contributed by [StefH](https://github.com/StefH)
- [#266](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/266) - Make ExpressionPromoter public + Fix issue with null constant expression compare [bug] contributed by [david-garcia-garcia](https://github.com/david-garcia-garcia)
- [#163](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/163) - Issue: Using escaped strings is not working correctly [bug]
- [#240](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/240) - Question: What is the proper way to construct a dynamic query for EF Core using DateTime or Nullable DateTime?

# 1.0.12.0 (26 March 2019)
- [#260](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/260) - Fix for Nullable Enum filter contributed by [StefH](https://github.com/StefH)
- [#258](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/258) - Filter by enum column with parameters throws exception [bug]

# 1.0.11.0 (28 February 2019)
- [#249](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/249) - Add support for OfType, Is, As and Cast [feature] contributed by [StefH](https://github.com/StefH)
- [#250](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/250) - Wrap all constant expressions to fix Parameterized SQL (#247) [bug, feature] contributed by [StefH](https://github.com/StefH)
- [#255](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/255) - Fix SonarScanner in build [bug] contributed by [StefH](https://github.com/StefH)
- [#247](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/247) - Parameterized SQL doesn't work for Contains, StartsWith, and EndsWith 
- [#248](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/248) - Add IQueryable.OfType support to ExpressionParser.
- [#252](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/252) - Implement ResolveTypesBySimpleName [feature]

# 1.0.10.0 (05 February 2019)
- [#223](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/223) - Add 'np(...)' Null Propagating function [feature] contributed by [StefH](https://github.com/StefH)
- [#98](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/98) - Feature: Add the &quot;?.&quot; operator (null-conditional operator) to support navigation properties with null values [feature]
- [#182](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/182) - Error when navigation property which named &quot;Parent&quot;
- [#243](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/243) - EF Core 2.2 - cannot use Where operator

# 1.0.9.2 (10 January 2019)
- [#239](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/239) - SingleOrDefaultAsync [feature] contributed by [StefH](https://github.com/StefH)
- [#238](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/238) - Missing SingleOrDefaultAsync for EntityFrameworkCore [feature]

# 1.0.9.1 (07 January 2019)
- [#210](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/210) - Set up CI with Azure Pipelines contributed by [azure-pipelines[bot]](https://github.com/apps/azure-pipelines)
- [#211](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/211) - ParameterExpressionRenamer contributed by [StefH](https://github.com/StefH)
- [#212](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/212) - Make ExpressionPromoter plugable contributed by [david-garcia-garcia](https://github.com/david-garcia-garcia)
- [#213](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/213) - Generating Parameterized SQL (by sspekinc) contributed by [StefH](https://github.com/StefH)
- [#214](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/214) - UseParameterizedNamesInDynamicQuery=false contributed by [StefH](https://github.com/StefH)
- [#216](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/216) - Add sourcelink contributed by [StefH](https://github.com/StefH)
- [#217](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/217) - Use GitHubReleaseNotes contributed by [StefH](https://github.com/StefH)
- [#218](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/218) - Codecov integration contributed by [StefH](https://github.com/StefH)
- [#221](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/221) - Add docs folder for hosting documentation pages on github. [feature] contributed by [StefH](https://github.com/StefH)
- [#222](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/222) - GenerateConditional will cast to nullable valuetype if needed [feature] contributed by [StefH](https://github.com/StefH)
- [#228](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/228) - Issue215 [bug, feature] contributed by [david-garcia-garcia](https://github.com/david-garcia-garcia)
- [#229](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/229) - Override is linq to objects [feature] contributed by [david-garcia-garcia](https://github.com/david-garcia-garcia)
- [#231](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/231) - Make ParsingConfig mandatory contributed by [StefH](https://github.com/StefH)
- [#237](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/237) - Performance Fix [bug] contributed by [StefH](https://github.com/StefH)
- [#71](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/71) - Issue: Increase code-coverage [feature]
- [#119](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/119) - Feature: How to keep parameter input name of query [feature]
- [#152](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/152) - Multiple assemblies with equivalent identity have been imported
- [#179](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/179) - Feature: Implement SourceLink
- [#184](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/184) - Feature: Generate Parameterized SQL [feature]
- [#204](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/204) - Cannot GroupJoin when source is Linq-To-Entities
- [#209](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/209) - Feature: use Azure Pipelines for building
- [#215](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/215) - Issue: DynamicClassFactory fails to create dynamic type without properties [bug]
- [#236](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/236) - Massive performance hit when upgrading from 1.0.8.18 to 1.0.9 [bug]

# 1.0.9.0 (19 October 2018)
- [#208](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/208) - Fix New() support for Type + Fix GroupJoin() not working when using Linq-To-Entities (2) contributed by [StefH](https://github.com/StefH)
- [#136](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/136) - Expressions on dynamic objects
- [#173](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/173) - Error when trying to access an object declared on another lambda
- [#199](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/199) - Feature: Add EvaluateGroupByAtDatabase logic to Join and GroupJoin
- [#205](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/205) - Documentation of supported operations
- [#206](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/206) - Issue: new() expression cannot handle complex types

# 1.0.8.18 (04 September 2018)
- [#201](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/201) - Fix Parsing Config not passed down to expression parser in JOIN contributed by [david-garcia-garcia](https://github.com/david-garcia-garcia)
- [#165](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/165) - Consider fit the rule of AnonymousTypes for EFCore2.1? [feature]
- [#202](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/202) - Feature: support Explicit cast Operator [feature]

# 1.0.8.17 (27 August 2018)
- [#200](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/200) - Fix for parsing Guid and string in the same condition contributed by [OlegNadymov](https://github.com/OlegNadymov)
- [#191](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/191) - Feature: re-enable support for uap10

# 1.0.8.16 (19 August 2018)
- [#198](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/198) - re-enable UAP10 support contributed by [StefH](https://github.com/StefH)

# 1.0.8.15 (17 August 2018)
- [#197](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/197) - Added EvaluateGroupByAtDatabase For EF Core 2.1 contributed by [StefH](https://github.com/StefH)

# 1.0.8.14 (14 August 2018)
- [#190](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/190) - Add SonarCloud (#186) contributed by [StefH](https://github.com/StefH)
- [#193](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/193) -  Fix for ParseLambda with itType and resultType: correct order of arguments contributed by [OlegNadymov](https://github.com/OlegNadymov)
- [#195](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/195) - Fix the problem with inner double quotes contributed by [OlegNadymov](https://github.com/OlegNadymov)
- [#186](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/186) - Feature: include SonarCloud code checks [feature]
- [#187](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/187) - Add custom static classes for parsing

# 1.0.8.12 (27 July 2018)
- [#177](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/177) - Feature: Remove built-in references from netstandard2.0 target contributed by [hazzik](https://github.com/hazzik)
- [#189](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/189) - Fix conversion from a non-nullable value type to the nullable value type contributed by [StefH](https://github.com/StefH)
- [#178](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/178) - Question: No generic method 'Contains' on type 'System.Linq.Enumerable
- [#180](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/180) - Question: GroupBy fails for field named SHORT
- [#188](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/188) - Issue: Implicitly conversion from a non-nullable value type to the nullable form of that value type is broken?

# 1.0.8.11 (06 June 2018)
- [#172](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/172) - Issue: DynamicQueryableExtensions.OrderBy extension method not using ParsingConfig parameter

# 1.0.8.10 (05 June 2018)
- [#143](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/143) - Question : How to orderby an attribute of a List
- [#170](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/170) - Question: Support to build Expressions besides LambdaExpressions

# 1.0.8.9 (26 May 2018)
- [#166](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/166) - Feature: Added support for implicit type conversions contributed by [arjenvrh](https://github.com/arjenvrh)

# 1.0.8.8 (21 May 2018)
- [#168](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/168) - Fixed ConstantExpressionHelper.cs (#167) contributed by [StefH](https://github.com/StefH)
- [#167](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/167) - Issue: Memory leak in `ConstantExpressionHelper.cs` [bug]

# 1.0.8.7 (09 May 2018)
- [#156](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/156) - Fix parsing config contributed by [jogibear9988](https://github.com/jogibear9988)

# 1.0.8.6 (28 April 2018)
- [#158](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/158) - Fix157 contributed by [jogibear9988](https://github.com/jogibear9988)
- [#157](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/157) - Issue : SkipWhile Method not found in mono [bug]
- [#161](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/161) - Error (1.0.8.3 to 1.0.8.4 on EF 6.2) &quot;No generic method 'OrderBy' on type System.Linq.Queryable&quot;

# 1.0.8.5 (27 April 2018)

# 1.0.8.4 (25 April 2018)
- [#159](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/159) - Performance fix (#153) contributed by [StefH](https://github.com/StefH)
- [#151](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/151) - Parse Query Syntax like Code
- [#153](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/153) - Issue: Performance while working with EF core

# 1.0.8.3 (30 March 2018)
- [#137](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/137) - Feature: Add support for querying a IQueryable&lt;dynamic&gt; contributed by [NickDarvey](https://github.com/NickDarvey)
- [#150](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/150) - Feature: Support Binary &amp; For String and Int [feature] contributed by [jogibear9988](https://github.com/jogibear9988)
- [#139](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/139) - Question: How to get related entities only one field
- [#141](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/141) - Question: Is there TryParseLambda

# 1.0.8.2 (09 January 2018)
- [#138](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/138) - Solved issue 130 contributed by [StefH](https://github.com/StefH)
- [#130](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/130) - BUG: Dynamic new in Where() causes NRE

# 1.0.8.1 (05 January 2018)
- [#135](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/135) - Add DbGeography to predefined types to allow advanced spatial queries. contributed by [czielin](https://github.com/czielin)
- [#95](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/95) - Has no assembly version number.
- [#126](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/126) - How to make a request with Collate in order to get Accent Insensitive results?
- [#129](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/129) - ToDynamicList/ToDynamicArray cannot actually cast to specified type in .net core 2 [bug]
- [#131](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/131) - Adding LIKE operator for EF6
- [#132](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/132) - System.Linq.Dynamic.Core.Exceptions.ParseException in IQueryable&lt;object&gt; filled with anonymous type
- [#133](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/133) - Possibility to parse an Expression&lt;T, bool&gt; to a valid expression string 
- [#134](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/134) - Accessing DbGeography methods/properties

# 1.0.8.0 (16 December 2017)
- [#127](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/127) - Refactored Parser contributed by [StefH](https://github.com/StefH)

# 1.0.7.13 (29 November 2017)
- [#117](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/117) -  New features contributed by [jogibear9988](https://github.com/jogibear9988)
- [#123](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/123) - appveyor contributed by [StefH](https://github.com/StefH)
- [#114](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/114) - Dynamic Linq Query not usable with ORM Provider [bug]
- [#120](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/120) - Error with parsing
- [#122](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/122) - Join with int list
- [#124](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/124) -  OrderBy produces error.
- [#125](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/125) - Not compatable with dotnet Core 2. [bug]

# 1.0.7.12 (09 November 2017)
- [#115](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/115) - Dynamic Linq Query not usable with ORM Provider (fix for #114) contributed by [jogibear9988](https://github.com/jogibear9988)
- [#116](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/116) - Bugfix DynamicLinq when using IQueryable contributed by [jogibear9988](https://github.com/jogibear9988)
- [#108](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/108) - [Question] Nullable property inside Join statement
- [#109](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/109) - Not able to build with VS2017
- [#111](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/111) - [Bug] Incorrect Nullable&lt;&gt; parsing [bug]
- [#112](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/112) - Support NETStandard 2.0 [feature]
- [#113](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/113) - .pdb is missing in nuget [bug]

# 1.0.7.10 (27 October 2017)
- [#8](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/8) - Remove useless dependences [feature] contributed by [yyjdelete](https://github.com/yyjdelete)
- [#37](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/37) - Support strings as Enum Parameter Objects contributed by [jogibear9988](https://github.com/jogibear9988)
- [#38](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/38) - Support more comparisons with strings contributed by [jogibear9988](https://github.com/jogibear9988)
- [#39](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/39) - Exception friendly Type loading contributed by [jogibear9988](https://github.com/jogibear9988)
- [#47](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/47) - * Add unit test and fix public methods access. contributed by [jotab123](https://github.com/jotab123)
- [#55](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/55) - Fix Nullable Enums from String contributed by [jogibear9988](https://github.com/jogibear9988)
- [#56](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/56) - Create .editorconfig contributed by [jogibear9988](https://github.com/jogibear9988)
- [#68](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/68) - Work on #66 -&gt; Should work now. Tests will follow on VS2017 support! contributed by [jogibear9988](https://github.com/jogibear9988)
- [#69](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/69) - Fix - when method has object parameter and ValueType value is passed into this method, result is exception in System.Dynamic.Utils.ExpressionUtils.ValidateOneArgument. contributed by [DavidCizek](https://github.com/DavidCizek)
- [#76](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/76) - Fix - shift operators work only for int, short, ushort, byte, sbyte. contributed by [DavidCizek](https://github.com/DavidCizek)
- [#77](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/77) - New features: Hexadecimal integers and array initializers contributed by [DavidCizek](https://github.com/DavidCizek)
- [#78](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/78) - New feature: GroupJoin [feature] contributed by [ghost](https://github.com/ghost)
- [#80](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/80) - [Feature] Usage of cached Lambda Expressions contributed by [jogibear9988](https://github.com/jogibear9988)
- [#85](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/85) - [Fix] Guid? == null comparison contributed by [jogibear9988](https://github.com/jogibear9988)
- [#86](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/86) - [Fix] Fixed null in Parameter and added functionality Binary And and Or with different Types contributed by [jogibear9988](https://github.com/jogibear9988)
- [#92](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/92) - [Feature] Adds support for decimal qualifiers. Resolves #91 contributed by [pferraris](https://github.com/pferraris)
- [#93](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/93) - [Bug] Fix uap10 build in appveyor contributed by [StefH](https://github.com/StefH)
- [#99](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/99) - Added DynamicEnumerable Async extension methods contributed by [StefH](https://github.com/StefH)
- [#100](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/100) - Feature: NullPropagation operator contributed by [StefH](https://github.com/StefH)
- [#103](https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/103) - support group by with 2 parameters, add tolist contributed by [jogibear9988](https://github.com/jogibear9988)
- [#1](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/1) - SymbolTable.DoesMethodHaveParameterArray throws exception when accessing a dynamic created property
- [#2](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/2) - UnitTest : GroupByAndSelect_TestDynamicSelectMember fails [bug]
- [#3](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/3) - Add &quot;SelectMany&quot; [feature]
- [#4](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/4) - Illegal one-byte branch at position: 9. Requested branch was: 143
- [#5](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/5) - Only parameterless constructors and initializers are supported in LINQ to Entities [bug]
- [#6](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/6) - Add support for dotnet5.4 framework [feature]
- [#7](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/7) - Add SelectMany with resultSelector [feature]
- [#9](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/9) - Add Null-coalescing operator support [feature]
- [#10](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/10) - Support explicit integer qualifiers [feature]
- [#11](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/11) - IN does not support negative and parse of negative integers with qualifiers.  [bug]
- [#12](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/12) - parsing negative float or double with qualifier [feature]
- [#13](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/13) - Add isnull sql function &quot;isnull(a, b)&quot; [feature]
- [#14](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/14) - Ampersand can be used both as logical And or as vb-like concatenation operator [feature]
- [#15](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/15) - Add Skip, Take to ExpressionParser [feature]
- [#16](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/16) - Add Paging support [feature]
- [#17](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/17) - Windows 10 uwp support
- [#18](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/18) - SelectMany over an Array throws System.IndexOutOfRangeException [bug]
- [#19](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/19) - NotEqual filter not working with DateTime [bug]
- [#20](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/20) - Can't install using nuget in Asp.Net 4.0 Web Pages project
- [#21](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/21) - Question: why is Distinct not supported? [feature]
- [#22](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/22) - DynamicExpression accessibility [feature]
- [#23](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/23) - Cannot work with property which in base class. [bug]
- [#24](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/24) - FirstOrDefaultAsync method is missing
- [#25](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/25) - DynamicExpression gone in version 1.0.3.4
- [#26](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/26) - Calling ToString on a nullable column throws error
- [#27](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/27) - UWP version
- [#28](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/28) - Dynamic Queries seem to lose &quot;Include()&quot;s
- [#29](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/29) - An another project ?
- [#30](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/30) - Move to .NET Core RTM
- [#31](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/31) - Group by multiple columns? [bug]
- [#32](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/32) - When same dynamic class is first used in Linq2Entities, it's reused for Linq2Sql [bug]
- [#33](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/33) - Package 1.0.6.3 install fails for UWP App [bug]
- [#36](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/36) - CreateClass Equivalent? [feature]
- [#40](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/40) - Add strong naming from library [feature]
- [#42](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/42) - Microsoft.EntityFrameworkCore.DynamicLinq - ToListAsync()?
- [#43](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/43) - Join with dependent subquery?
- [#44](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/44) - Casting a int to a nullable int will throw an error when using linq to entities&quot;Only parameterless constructors and initializers are supported in LINQ to Entities&quot;
- [#45](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/45) - Take() and Skip() lose ElementType [bug]
- [#46](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/46) - Methods on type are not accessible error
- [#48](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/48) - Add an overload to the &quot;ToDynamicList&quot; method which accepts a Type [feature]
- [#49](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/49) - .Contains(&quot;&quot;) operation Exception
- [#50](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/50) - Add functionality to optimize your queries using Linq.Expression.Optimizer [feature]
- [#51](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/51) - [Question] How can I format a datetime (nullable) field value in select?
- [#52](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/52) - Can I convert int to string type?
- [#57](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/57) - Issue finding indexer [bug]
- [#60](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/60) - Issue with nested Calls
- [#62](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/62) - OrderBy Chaining [bug, feature]
- [#63](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/63) - Syntax IN dont work with Enums
- [#65](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/65) - Support embedded quotes in string literal [feature]
- [#66](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/66) - Is there Way to enter a Complex query
- [#67](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/67) - Convert Project to VS2017 [feature]
- [#70](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/70) - Move all tests into 1 test project [feature]
- [#72](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/72) - [Bug] Re-enable support for uap10.0 [bug]
- [#73](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/73) - [Feature] Extend OrderBy functionality [feature]
- [#74](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/74) - Join on nullable and not nullable type throws exception [bug]
- [#75](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/75) - GroupBy clause add an &quot;Item&quot; property when projects the query
- [#79](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/79) - DynamicExpressionParser does not allow empty parameter lists. [bug]
- [#81](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/81) - [Question] Create new nuget?
- [#82](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/82) - [Feature] Add DefaultIfEmpty [feature]
- [#84](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/84) - [Bug] DynamicClassFactory not caching generated types [bug]
- [#90](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/90) - [Bug] ParseIntegerLiteral Int16 [bug]
- [#91](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/91) - [Bug] Support for decimal qualifiers 'M' &amp; 'm' [bug]
- [#94](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/94) - [Bug] ParseException: Operator '==' incompatible with operand types 'ObjectId' and 'ObjectId' [bug]
- [#96](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/96) - Async support for ToDynamicList() [feature]
- [#102](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/102) - Aggregate method does not work with Average function
- [#104](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/104) - Add PDB to nuget package [feature]
- [#106](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/106) - Using both System.Linq and System.Linq.Dynamic.Core 
- [#107](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/107) - Type conversions generated in cases where they're not needed. [bug]


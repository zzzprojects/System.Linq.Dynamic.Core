# 1.0.7.5 (28 juni 2017)

 - [#72](https://github.com/StefH/System.Linq.Dynamic.Core/issues/72) - [Bug] Re-enable support for uap10.0 +fix

Commits: a02f3d64fa...4bfd7a5628


# 1.0.7.4 (27 juni 2017) (27 juni 2017)

 - [#92](https://github.com/StefH/System.Linq.Dynamic.Core/pull/92) - [Feature] Adds support for decimal qualifiers. Resolves #91 contributed by Pablo Ferraris ([pferraris](https://github.com/pferraris))
 - [#91](https://github.com/StefH/System.Linq.Dynamic.Core/issues/91) - [Bug] Support for decimal qualifiers 'M' & 'm' +fix
 - [#90](https://github.com/StefH/System.Linq.Dynamic.Core/issues/90) - [Bug] ParseIntegerLiteral Int16 +fix
 - [#89](https://github.com/StefH/System.Linq.Dynamic.Core/issues/89) - [Question] System.Linq.Dynamic.Core.Exceptions.ParseException: 'No 'it' is in scope'
 - [#88](https://github.com/StefH/System.Linq.Dynamic.Core/issues/88) - [Question] FileNotFoundException: Could not load file or assembly 'System.Linq.Dynamic.Core'
 - [#87](https://github.com/StefH/System.Linq.Dynamic.Core/issues/87) - [Question] Memory leak in Join method
 - [#84](https://github.com/StefH/System.Linq.Dynamic.Core/issues/84) - [Bug] DynamicClassFactory not caching generated types +fix
 - [#83](https://github.com/StefH/System.Linq.Dynamic.Core/issues/83) - [Bug] Problem with DynamicExpressionParser.ParseLambda & external ParameterExpression

Commits: a02f3d64fa...ab249d498a


# 1.0.7.3 (01 juni 2017) (01 juni 2017)

 - [#86](https://github.com/StefH/System.Linq.Dynamic.Core/pull/86) - [Fix] Fixed null in Parameter and added functionality Binary And and Or with different Types contributed by Jochen Kühner ([jogibear9988](https://github.com/jogibear9988))

Commits: a02f3d64fa...a1096799c2


# 1.0.7.2 (01 juni 2017) (01 juni 2017)

 - [#86](https://github.com/StefH/System.Linq.Dynamic.Core/pull/86) - [Fix] Fixed null in Parameter and added functionality Binary And and Or with different Types contributed by Jochen Kühner ([jogibear9988](https://github.com/jogibear9988))

Commits: a02f3d64fa...35bdf2c0c7


# 1.0.7.1 (31 mei 2017) (31 mei 2017)

 - [#85](https://github.com/StefH/System.Linq.Dynamic.Core/pull/85) - [Fix] Guid? == null comparison contributed by Jochen Kühner ([jogibear9988](https://github.com/jogibear9988))
 - [#82](https://github.com/StefH/System.Linq.Dynamic.Core/issues/82) - [Feature] Add DefaultIfEmpty +enhancement
 - [#81](https://github.com/StefH/System.Linq.Dynamic.Core/issues/81) - [Question] Create new nuget?
 - [#80](https://github.com/StefH/System.Linq.Dynamic.Core/pull/80) - [Feature] Usage of cached Lambda Expressions contributed by Jochen Kühner ([jogibear9988](https://github.com/jogibear9988))

Commits: a02f3d64fa...4252212620


# 1.0.7.0 (17 mei 2017) (17 mei 2017)

 - [#80](https://github.com/StefH/System.Linq.Dynamic.Core/pull/80) - [Feature] Usage of cached Lambda Expressions contributed by Jochen Kühner ([jogibear9988](https://github.com/jogibear9988))
 - [#79](https://github.com/StefH/System.Linq.Dynamic.Core/issues/79) - DynamicExpressionParser does not allow empty parameter lists. +fix
 - [#78](https://github.com/StefH/System.Linq.Dynamic.Core/pull/78) - New feature: GroupJoin contributed by ([Maschmi](https://github.com/Maschmi)) +enhancement
 - [#77](https://github.com/StefH/System.Linq.Dynamic.Core/pull/77) - New features: Hexadecimal integers and array initializers contributed by David Cizek ([DavidCizek](https://github.com/DavidCizek))
 - [#76](https://github.com/StefH/System.Linq.Dynamic.Core/pull/76) - Fix - shift operators work only for int, short, ushort, byte, sbyte. contributed by David Cizek ([DavidCizek](https://github.com/DavidCizek))
 - [#75](https://github.com/StefH/System.Linq.Dynamic.Core/issues/75) - GroupBy clause add an "Item" property when projects the query
 - [#74](https://github.com/StefH/System.Linq.Dynamic.Core/issues/74) - Join on nullable and not nullable type throws exception +fix
 - [#73](https://github.com/StefH/System.Linq.Dynamic.Core/issues/73) - [Feature] Extend OrderBy functionality +enhancement
 - [#70](https://github.com/StefH/System.Linq.Dynamic.Core/issues/70) - Move all tests into 1 test project +enhancement
 - [#67](https://github.com/StefH/System.Linq.Dynamic.Core/issues/67) - Convert Project to VS2017 +enhancement
 - [#66](https://github.com/StefH/System.Linq.Dynamic.Core/issues/66) - Is there Way to enter a Complex query
 - [#63](https://github.com/StefH/System.Linq.Dynamic.Core/issues/63) - Syntax IN dont work with Enums
 - [#58](https://github.com/StefH/System.Linq.Dynamic.Core/issues/58) - Parse Lambda
 - [#49](https://github.com/StefH/System.Linq.Dynamic.Core/issues/49) - .Contains("") operation Exception
 - [#44](https://github.com/StefH/System.Linq.Dynamic.Core/issues/44) - Casting a int to a nullable int will throw an error when using linq to entities"Only parameterless constructors and initializers are supported in LINQ to Entities"

Commits: a02f3d64fa...c21b1be15c


# 1.0.6.13 (08 april 2017)

 - [#68](https://github.com/StefH/System.Linq.Dynamic.Core/pull/68) - Work on #66 -> Should work now. Tests will follow on VS2017 support! contributed by Jochen Kühner ([jogibear9988](https://github.com/jogibear9988))

Commits: a02f3d64fa...9fa29d7b3b


# vNext

 - [#69](https://github.com/StefH/System.Linq.Dynamic.Core/pull/69) - Fix - when method has object parameter and ValueType value is passed into this method, result is exception in System.Dynamic.Utils.ExpressionUtils.ValidateOneArgument. contributed by David Cizek ([DavidCizek](https://github.com/DavidCizek))
 - [#65](https://github.com/StefH/System.Linq.Dynamic.Core/issues/65) - Support embedded quotes in string literal +enhancement
 - [#62](https://github.com/StefH/System.Linq.Dynamic.Core/issues/62) - OrderBy Chaining +fix
 - [#61](https://github.com/StefH/System.Linq.Dynamic.Core/issues/61) - String[].Contains(String) expression translated to first array element contains
 - [#60](https://github.com/StefH/System.Linq.Dynamic.Core/issues/60) - Issue with nested Calls
 - [#57](https://github.com/StefH/System.Linq.Dynamic.Core/issues/57) - Issue finding indexer +fix
 - [#56](https://github.com/StefH/System.Linq.Dynamic.Core/pull/56) - Create .editorconfig contributed by Jochen Kühner ([jogibear9988](https://github.com/jogibear9988))
 - [#55](https://github.com/StefH/System.Linq.Dynamic.Core/pull/55) - Fix Nullable Enums from String contributed by Jochen Kühner ([jogibear9988](https://github.com/jogibear9988))
 - [#52](https://github.com/StefH/System.Linq.Dynamic.Core/issues/52) - Can I convert int to string type?
 - [#50](https://github.com/StefH/System.Linq.Dynamic.Core/issues/50) - Add functionality to optimize your queries using Linq.Expression.Optimizer +enhancement
 - [#48](https://github.com/StefH/System.Linq.Dynamic.Core/issues/48) - Add an overload to the "ToDynamicList" method which accepts a Type +enhancement
 - [#47](https://github.com/StefH/System.Linq.Dynamic.Core/pull/47) - * Add unit test and fix public methods access. contributed by ([jotab123](https://github.com/jotab123))
 - [#46](https://github.com/StefH/System.Linq.Dynamic.Core/issues/46) - Methods on type are not accessible error
 - [#45](https://github.com/StefH/System.Linq.Dynamic.Core/issues/45) - Take() and Skip() lose ElementType +fix
 - [#43](https://github.com/StefH/System.Linq.Dynamic.Core/issues/43) - Join with dependent subquery?
 - [#42](https://github.com/StefH/System.Linq.Dynamic.Core/issues/42) - Microsoft.EntityFrameworkCore.DynamicLinq - ToListAsync()?
 - [#41](https://github.com/StefH/System.Linq.Dynamic.Core/pull/41) - Separation of tokenization logic contributed by ([arespr](https://github.com/arespr))
 - [#40](https://github.com/StefH/System.Linq.Dynamic.Core/issues/40) - Add strong naming from library +enhancement
 - [#39](https://github.com/StefH/System.Linq.Dynamic.Core/pull/39) - Exception friendly Type loading contributed by Jochen Kühner ([jogibear9988](https://github.com/jogibear9988))
 - [#38](https://github.com/StefH/System.Linq.Dynamic.Core/pull/38) - Support more comparisons with strings contributed by Jochen Kühner ([jogibear9988](https://github.com/jogibear9988))
 - [#37](https://github.com/StefH/System.Linq.Dynamic.Core/pull/37) - Support strings as Enum Parameter Objects contributed by Jochen Kühner ([jogibear9988](https://github.com/jogibear9988))
 - [#36](https://github.com/StefH/System.Linq.Dynamic.Core/issues/36) - CreateClass Equivalent? +enhancement
 - [#35](https://github.com/StefH/System.Linq.Dynamic.Core/issues/35) - Compatibility with System.Linq.Dynamic.Library 
 - [#34](https://github.com/StefH/System.Linq.Dynamic.Core/issues/34) - Support for netcoreapp1.0?
 - [#32](https://github.com/StefH/System.Linq.Dynamic.Core/issues/32) - When same dynamic class is first used in Linq2Entities, it's reused for Linq2Sql +fix
 - [#31](https://github.com/StefH/System.Linq.Dynamic.Core/issues/31) - Group by multiple columns? +fix
 - [#30](https://github.com/StefH/System.Linq.Dynamic.Core/issues/30) - Move to .NET Core RTM
 - [#29](https://github.com/StefH/System.Linq.Dynamic.Core/issues/29) - An another project ?
 - [#28](https://github.com/StefH/System.Linq.Dynamic.Core/issues/28) - Dynamic Queries seem to lose "Include()"s
 - [#27](https://github.com/StefH/System.Linq.Dynamic.Core/issues/27) - UWP version
 - [#26](https://github.com/StefH/System.Linq.Dynamic.Core/issues/26) - Calling ToString on a nullable column throws error
 - [#25](https://github.com/StefH/System.Linq.Dynamic.Core/issues/25) - DynamicExpression gone in version 1.0.3.4
 - [#24](https://github.com/StefH/System.Linq.Dynamic.Core/issues/24) - FirstOrDefaultAsync method is missing
 - [#23](https://github.com/StefH/System.Linq.Dynamic.Core/issues/23) - Cannot work with property which in base class. +fix
 - [#22](https://github.com/StefH/System.Linq.Dynamic.Core/issues/22) - DynamicExpression accessibility +enhancement
 - [#21](https://github.com/StefH/System.Linq.Dynamic.Core/issues/21) - Question: why is Distinct not supported? +enhancement
 - [#20](https://github.com/StefH/System.Linq.Dynamic.Core/issues/20) - Can't install using nuget in Asp.Net 4.0 Web Pages project
 - [#19](https://github.com/StefH/System.Linq.Dynamic.Core/issues/19) - NotEqual filter not working with DateTime +fix
 - [#18](https://github.com/StefH/System.Linq.Dynamic.Core/issues/18) - SelectMany over an Array throws System.IndexOutOfRangeException +fix
 - [#17](https://github.com/StefH/System.Linq.Dynamic.Core/issues/17) - Windows 10 uwp support
 - [#16](https://github.com/StefH/System.Linq.Dynamic.Core/issues/16) - Add Paging support +enhancement
 - [#15](https://github.com/StefH/System.Linq.Dynamic.Core/issues/15) - Add Skip, Take to ExpressionParser +enhancement
 - [#14](https://github.com/StefH/System.Linq.Dynamic.Core/issues/14) - Ampersand can be used both as logical And or as vb-like concatenation operator +enhancement
 - [#13](https://github.com/StefH/System.Linq.Dynamic.Core/issues/13) - Add isnull sql function "isnull(a, b)" +enhancement
 - [#12](https://github.com/StefH/System.Linq.Dynamic.Core/issues/12) - parsing negative float or double with qualifier +enhancement
 - [#11](https://github.com/StefH/System.Linq.Dynamic.Core/issues/11) - IN does not support negative and parse of negative integers with qualifiers. +fix
 - [#10](https://github.com/StefH/System.Linq.Dynamic.Core/issues/10) - Support explicit integer qualifiers +enhancement
 - [#9](https://github.com/StefH/System.Linq.Dynamic.Core/issues/9) - Add Null-coalescing operator support +enhancement
 - [#8](https://github.com/StefH/System.Linq.Dynamic.Core/pull/8) - Remove useless dependences contributed by SilverFox ([yyjdelete](https://github.com/yyjdelete)) +enhancement
 - [#7](https://github.com/StefH/System.Linq.Dynamic.Core/issues/7) - Add SelectMany with resultSelector +enhancement
 - [#6](https://github.com/StefH/System.Linq.Dynamic.Core/issues/6) - Add support for dotnet5.4 framework +enhancement
 - [#5](https://github.com/StefH/System.Linq.Dynamic.Core/issues/5) - Only parameterless constructors and initializers are supported in LINQ to Entities +fix
 - [#4](https://github.com/StefH/System.Linq.Dynamic.Core/issues/4) - Illegal one-byte branch at position: 9. Requested branch was: 143
 - [#3](https://github.com/StefH/System.Linq.Dynamic.Core/issues/3) - Add "SelectMany" +enhancement
 - [#2](https://github.com/StefH/System.Linq.Dynamic.Core/issues/2) - UnitTest : GroupByAndSelect_TestDynamicSelectMember fails +fix
 - [#1](https://github.com/StefH/System.Linq.Dynamic.Core/issues/1) - SymbolTable.DoesMethodHaveParameterArray throws exception when accessing a dynamic created property

Commits: a02f3d64fa...a32f8327e3

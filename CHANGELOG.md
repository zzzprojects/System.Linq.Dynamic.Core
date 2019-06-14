# 1.0.17.0 (14 June 2019)
- [#276](https://github.com/StefH/System.Linq.Dynamic.Core/pull/276) - op_Compare also for single &quot;equals&quot; token [bug] contributed by [nothrow](https://github.com/nothrow)
- [#274](https://github.com/StefH/System.Linq.Dynamic.Core/issues/274) - Question: Extension method that returns Expression&lt;Func&lt;T, bool&gt;&gt; ? [question]

# 1.0.16.0 (06 June 2019)
- [#275](https://github.com/StefH/System.Linq.Dynamic.Core/pull/275) - Support Enumerations from System Namespace (e.g. StringComparison) [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.15.0 (20 May 2019)
- [#273](https://github.com/StefH/System.Linq.Dynamic.Core/pull/273) - Adding support for overloaded op_Equality contributed by [nothrow](https://github.com/nothrow)
- [#272](https://github.com/StefH/System.Linq.Dynamic.Core/issues/272) - Allow comparing via overloaded equality operator [feature]

# 1.0.14.0 (14 May 2019)
- [#270](https://github.com/StefH/System.Linq.Dynamic.Core/pull/270) - Fix for np() opererator for Nullable (e.g. DateTime) contributed by [StefH](https://github.com/StefH)
- [#105](https://github.com/StefH/System.Linq.Dynamic.Core/issues/105) - Feature: Support for EF Core 2.0's EF.Functions.Like() [feature]
- [#265](https://github.com/StefH/System.Linq.Dynamic.Core/issues/265) - Linq CreateQuery throws exception [question]
- [#267](https://github.com/StefH/System.Linq.Dynamic.Core/issues/267) - UseParameterizedNamesInDynamicQuery=true is ignored if predicate in Where clause has Contains() expression [question]
- [#269](https://github.com/StefH/System.Linq.Dynamic.Core/issues/269) - np() opererator fails for (Nullable) DateTime [bug]

# 1.0.13.0 (03 May 2019)
- [#264](https://github.com/StefH/System.Linq.Dynamic.Core/pull/264) - Fix escape characters parsing [bug] contributed by [StefH](https://github.com/StefH)
- [#266](https://github.com/StefH/System.Linq.Dynamic.Core/pull/266) - Make ExpressionPromoter public + Fix issue with null constant expression compare [bug] contributed by [david-garcia-garcia](https://github.com/david-garcia-garcia)
- [#148](https://github.com/StefH/System.Linq.Dynamic.Core/issues/148) - Question: Using where filter it.Contains(@0) fails where it.Contains(\&quot;keep\&quot;) works [question]
- [#154](https://github.com/StefH/System.Linq.Dynamic.Core/issues/154) - Remark: Semantic Versioning... [wontfix]
- [#163](https://github.com/StefH/System.Linq.Dynamic.Core/issues/163) - Issue: Using escaped strings is not working correctly [bug]
- [#240](https://github.com/StefH/System.Linq.Dynamic.Core/issues/240) - Question: What is the proper way to construct a dynamic query for EF Core using DateTime or Nullable DateTime?
- [#241](https://github.com/StefH/System.Linq.Dynamic.Core/issues/241) - Question: Is it possible to project without flattening the result? [question]
- [#244](https://github.com/StefH/System.Linq.Dynamic.Core/issues/244) - Possible to add 'Compare' Extension method [wontfix]
- [#246](https://github.com/StefH/System.Linq.Dynamic.Core/issues/246) - Exception: Argument expression is not valid [question]
- [#261](https://github.com/StefH/System.Linq.Dynamic.Core/issues/261) - How can I use Where(&quot;...&quot;) after calling ToDynamicArray [question]
- [#263](https://github.com/StefH/System.Linq.Dynamic.Core/issues/263) - Have a WhereOr method [question]

# 1.0.12.0 (26 March 2019)
- [#260](https://github.com/StefH/System.Linq.Dynamic.Core/pull/260) - Fix for Nullable Enum filter contributed by [StefH](https://github.com/StefH)
- [#258](https://github.com/StefH/System.Linq.Dynamic.Core/issues/258) - Filter by enum column with parameters throws exception [bug]

# 1.0.11.0 (28 February 2019)
- [#249](https://github.com/StefH/System.Linq.Dynamic.Core/pull/249) - Add support for OfType, Is, As and Cast [feature] contributed by [StefH](https://github.com/StefH)
- [#250](https://github.com/StefH/System.Linq.Dynamic.Core/pull/250) - Wrap all constant expressions to fix Parameterized SQL (#247) [bug, feature] contributed by [StefH](https://github.com/StefH)
- [#251](https://github.com/StefH/System.Linq.Dynamic.Core/pull/251) - Add NetCoreApp target &amp; include DefaultDynamicLinqCustomTypeProvider [feature] contributed by [StefH](https://github.com/StefH)
- [#253](https://github.com/StefH/System.Linq.Dynamic.Core/pull/253) - OfType Function contributed by [StefH](https://github.com/StefH)
- [#254](https://github.com/StefH/System.Linq.Dynamic.Core/pull/254) - Resolve types by simple name #252 [feature] contributed by [StefH](https://github.com/StefH)
- [#255](https://github.com/StefH/System.Linq.Dynamic.Core/pull/255) - Fix SonarScanner in build [bug] contributed by [StefH](https://github.com/StefH)
- [#245](https://github.com/StefH/System.Linq.Dynamic.Core/issues/245) - Question: Is it possible to validate expression before apply it? [question]
- [#247](https://github.com/StefH/System.Linq.Dynamic.Core/issues/247) - Parameterized SQL doesn't work for Contains, StartsWith, and EndsWith 
- [#248](https://github.com/StefH/System.Linq.Dynamic.Core/issues/248) - Add IQueryable.OfType support to ExpressionParser.
- [#252](https://github.com/StefH/System.Linq.Dynamic.Core/issues/252) - Implement ResolveTypesBySimpleName [feature]

# 1.0.10.0 (05 February 2019)
- [#223](https://github.com/StefH/System.Linq.Dynamic.Core/pull/223) - Add 'np(...)' Null Propagating function [feature] contributed by [StefH](https://github.com/StefH)
- [#98](https://github.com/StefH/System.Linq.Dynamic.Core/issues/98) - Feature: Add the &quot;?.&quot; operator (null-conditional operator) to support navigation properties with null values [feature, question]
- [#182](https://github.com/StefH/System.Linq.Dynamic.Core/issues/182) - Error when navigation property which named &quot;Parent&quot;
- [#243](https://github.com/StefH/System.Linq.Dynamic.Core/issues/243) - EF Core 2.2 - cannot use Where operator

# 1.0.9.2 (10 January 2019)
- [#239](https://github.com/StefH/System.Linq.Dynamic.Core/pull/239) - SingleOrDefaultAsync [feature] contributed by [StefH](https://github.com/StefH)
- [#238](https://github.com/StefH/System.Linq.Dynamic.Core/issues/238) - Missing SingleOrDefaultAsync for EntityFrameworkCore [feature]

# 1.0.9.1 (07 January 2019)
- [#210](https://github.com/StefH/System.Linq.Dynamic.Core/pull/210) - Set up CI with Azure Pipelines contributed by [azure-pipelines[bot]](https://github.com/apps/azure-pipelines)
- [#211](https://github.com/StefH/System.Linq.Dynamic.Core/pull/211) - ParameterExpressionRenamer contributed by [StefH](https://github.com/StefH)
- [#212](https://github.com/StefH/System.Linq.Dynamic.Core/pull/212) - Make ExpressionPromoter plugable contributed by [david-garcia-garcia](https://github.com/david-garcia-garcia)
- [#213](https://github.com/StefH/System.Linq.Dynamic.Core/pull/213) - Generating Parameterized SQL (by sspekinc) contributed by [StefH](https://github.com/StefH)
- [#214](https://github.com/StefH/System.Linq.Dynamic.Core/pull/214) - UseParameterizedNamesInDynamicQuery=false contributed by [StefH](https://github.com/StefH)
- [#216](https://github.com/StefH/System.Linq.Dynamic.Core/pull/216) - Add sourcelink contributed by [StefH](https://github.com/StefH)
- [#217](https://github.com/StefH/System.Linq.Dynamic.Core/pull/217) - Use GitHubReleaseNotes contributed by [StefH](https://github.com/StefH)
- [#218](https://github.com/StefH/System.Linq.Dynamic.Core/pull/218) - Codecov integration contributed by [StefH](https://github.com/StefH)
- [#221](https://github.com/StefH/System.Linq.Dynamic.Core/pull/221) - Add docs folder for hosting documentation pages on github. [feature] contributed by [StefH](https://github.com/StefH)
- [#222](https://github.com/StefH/System.Linq.Dynamic.Core/pull/222) - GenerateConditional will cast to nullable valuetype if needed [feature] contributed by [StefH](https://github.com/StefH)
- [#228](https://github.com/StefH/System.Linq.Dynamic.Core/pull/228) - Issue215 [bug, feature] contributed by [david-garcia-garcia](https://github.com/david-garcia-garcia)
- [#229](https://github.com/StefH/System.Linq.Dynamic.Core/pull/229) - Override is linq to objects [feature] contributed by [david-garcia-garcia](https://github.com/david-garcia-garcia)
- [#231](https://github.com/StefH/System.Linq.Dynamic.Core/pull/231) - Make ParsingConfig mandatory contributed by [StefH](https://github.com/StefH)
- [#237](https://github.com/StefH/System.Linq.Dynamic.Core/pull/237) - Performance Fix [bug] contributed by [StefH](https://github.com/StefH)
- [#71](https://github.com/StefH/System.Linq.Dynamic.Core/issues/71) - Issue: Increase code-coverage [feature]
- [#119](https://github.com/StefH/System.Linq.Dynamic.Core/issues/119) - Feature: How to keep parameter input name of query [feature]
- [#144](https://github.com/StefH/System.Linq.Dynamic.Core/issues/144) - Question: Static / Constant properties and Enums not supported as parameters in method calls [question]
- [#145](https://github.com/StefH/System.Linq.Dynamic.Core/issues/145) - Question : Performance and 'System.IO.FileNotFoundException' in System.Private.CoreLib.dll
- [#152](https://github.com/StefH/System.Linq.Dynamic.Core/issues/152) - Multiple assemblies with equivalent identity have been imported
- [#174](https://github.com/StefH/System.Linq.Dynamic.Core/issues/174) - Including more methods from System.Math? [question]
- [#179](https://github.com/StefH/System.Linq.Dynamic.Core/issues/179) - Feature: Implement SourceLink
- [#183](https://github.com/StefH/System.Linq.Dynamic.Core/issues/183) - Question: Issue with DateTime Field [question]
- [#184](https://github.com/StefH/System.Linq.Dynamic.Core/issues/184) - Feature: Generate Parameterized SQL [feature]
- [#204](https://github.com/StefH/System.Linq.Dynamic.Core/issues/204) - Cannot GroupJoin when source is Linq-To-Entities
- [#209](https://github.com/StefH/System.Linq.Dynamic.Core/issues/209) - Feature: use Azure Pipelines for building
- [#215](https://github.com/StefH/System.Linq.Dynamic.Core/issues/215) - Issue: DynamicClassFactory fails to create dynamic type without properties [bug]
- [#219](https://github.com/StefH/System.Linq.Dynamic.Core/issues/219) - ParsingConfig.DefaultEFCore21 doesn't group on database [question]
- [#220](https://github.com/StefH/System.Linq.Dynamic.Core/issues/220) - Update wiki about  Null-coalescing operator [question]
- [#227](https://github.com/StefH/System.Linq.Dynamic.Core/issues/227) - Question: Is it possible to pass CustomAttribute to property using new(...) syntax?  [question]
- [#232](https://github.com/StefH/System.Linq.Dynamic.Core/issues/232) - Extensionpoint for changing identifier token text [question, wontfix]
- [#234](https://github.com/StefH/System.Linq.Dynamic.Core/issues/234) - Does this support Json data
- [#236](https://github.com/StefH/System.Linq.Dynamic.Core/issues/236) - Massive performance hit when upgrading from 1.0.8.18 to 1.0.9

# 1.0.9.0 (19 October 2018)
- [#208](https://github.com/StefH/System.Linq.Dynamic.Core/pull/208) - Fix New() support for Type + Fix GroupJoin() not working when using Linq-To-Entities (2) contributed by [StefH](https://github.com/StefH)
- [#136](https://github.com/StefH/System.Linq.Dynamic.Core/issues/136) - Expressions on dynamic objects
- [#147](https://github.com/StefH/System.Linq.Dynamic.Core/issues/147) - Question: Making some queries dynamic possible
- [#173](https://github.com/StefH/System.Linq.Dynamic.Core/issues/173) - Error when trying to access an object declared on another lambda
- [#181](https://github.com/StefH/System.Linq.Dynamic.Core/issues/181) - Question: QueryValidator
- [#199](https://github.com/StefH/System.Linq.Dynamic.Core/issues/199) - Feature: Add EvaluateGroupByAtDatabase logic to Join and GroupJoin
- [#203](https://github.com/StefH/System.Linq.Dynamic.Core/issues/203) - How to query complex entities
- [#205](https://github.com/StefH/System.Linq.Dynamic.Core/issues/205) - Documentation of supported operations
- [#206](https://github.com/StefH/System.Linq.Dynamic.Core/issues/206) - Issue: new() expression cannot handle complex types

# 1.0.8.18 (04 September 2018)
- [#201](https://github.com/StefH/System.Linq.Dynamic.Core/pull/201) - Fix Parsing Config not passed down to expression parser in JOIN contributed by [david-garcia-garcia](https://github.com/david-garcia-garcia)
- [#165](https://github.com/StefH/System.Linq.Dynamic.Core/issues/165) - Consider fit the rule of AnonymousTypes for EFCore2.1? [feature]
- [#202](https://github.com/StefH/System.Linq.Dynamic.Core/issues/202) - Feature: support Explicit cast Operator [feature]

# 1.0.8.17 (27 August 2018)
- [#200](https://github.com/StefH/System.Linq.Dynamic.Core/pull/200) - Fix for parsing Guid and string in the same condition contributed by [OlegNadymov](https://github.com/OlegNadymov)
- [#191](https://github.com/StefH/System.Linq.Dynamic.Core/issues/191) - Feature: re-enable support for uap10

# 1.0.8.16 (19 August 2018)
- [#198](https://github.com/StefH/System.Linq.Dynamic.Core/pull/198) - re-enable UAP10 support contributed by [StefH](https://github.com/StefH)

# 1.0.8.15 (17 August 2018)
- [#197](https://github.com/StefH/System.Linq.Dynamic.Core/pull/197) - Added EvaluateGroupByAtDatabase For EF Core 2.1 contributed by [StefH](https://github.com/StefH)
- [#196](https://github.com/StefH/System.Linq.Dynamic.Core/issues/196) - Question: Not applicable aggregate method 'Any' exists [question]

# 1.0.8.14 (14 August 2018)
- [#190](https://github.com/StefH/System.Linq.Dynamic.Core/pull/190) - Add SonarCloud (#186) contributed by [StefH](https://github.com/StefH)
- [#193](https://github.com/StefH/System.Linq.Dynamic.Core/pull/193) -  Fix for ParseLambda with itType and resultType: correct order of arguments contributed by [OlegNadymov](https://github.com/OlegNadymov)
- [#195](https://github.com/StefH/System.Linq.Dynamic.Core/pull/195) - Fix the problem with inner double quotes contributed by [OlegNadymov](https://github.com/OlegNadymov)
- [#186](https://github.com/StefH/System.Linq.Dynamic.Core/issues/186) - Feature: include SonarCloud code checks [feature]
- [#187](https://github.com/StefH/System.Linq.Dynamic.Core/issues/187) - Add custom static classes for parsing

# 1.0.8.12 (27 July 2018)
- [#177](https://github.com/StefH/System.Linq.Dynamic.Core/pull/177) - Feature: Remove built-in references from netstandard2.0 target contributed by [hazzik](https://github.com/hazzik)
- [#189](https://github.com/StefH/System.Linq.Dynamic.Core/pull/189) - Fix conversion from a non-nullable value type to the nullable value type contributed by [StefH](https://github.com/StefH)
- [#53](https://github.com/StefH/System.Linq.Dynamic.Core/issues/53) - Question: External Methods call [question]
- [#178](https://github.com/StefH/System.Linq.Dynamic.Core/issues/178) - Question: No generic method 'Contains' on type 'System.Linq.Enumerable
- [#180](https://github.com/StefH/System.Linq.Dynamic.Core/issues/180) - Question: GroupBy fails for field named SHORT
- [#188](https://github.com/StefH/System.Linq.Dynamic.Core/issues/188) - Issue: Implicitly conversion from a non-nullable value type to the nullable form of that value type is broken?

# 1.0.8.11 (06 June 2018)
- [#172](https://github.com/StefH/System.Linq.Dynamic.Core/issues/172) - Issue: DynamicQueryableExtensions.OrderBy extension method not using ParsingConfig parameter

# 1.0.8.10 (05 June 2018)
- [#143](https://github.com/StefH/System.Linq.Dynamic.Core/issues/143) - Question : How to orderby an attribute of a List
- [#155](https://github.com/StefH/System.Linq.Dynamic.Core/issues/155) - Question: OrderByDescending availability [question]
- [#170](https://github.com/StefH/System.Linq.Dynamic.Core/issues/170) - Question: Support to build Expressions besides LambdaExpressions

# 1.0.8.9 (26 May 2018)
- [#166](https://github.com/StefH/System.Linq.Dynamic.Core/pull/166) - Feature: Added support for implicit type conversions contributed by [arjenvrh](https://github.com/arjenvrh)

# 1.0.8.8 (21 May 2018)
- [#168](https://github.com/StefH/System.Linq.Dynamic.Core/pull/168) - Fixed ConstantExpressionHelper.cs (#167) contributed by [StefH](https://github.com/StefH)
- [#167](https://github.com/StefH/System.Linq.Dynamic.Core/issues/167) - Issue: Memory leak in `ConstantExpressionHelper.cs` [bug]

# 1.0.8.7 (09 May 2018)
- [#156](https://github.com/StefH/System.Linq.Dynamic.Core/pull/156) - Fix parsing config contributed by [jogibear9988](https://github.com/jogibear9988)

# 1.0.8.6 (28 April 2018)
- [#158](https://github.com/StefH/System.Linq.Dynamic.Core/pull/158) - Fix157 contributed by [jogibear9988](https://github.com/jogibear9988)
- [#157](https://github.com/StefH/System.Linq.Dynamic.Core/issues/157) - Issue : SkipWhile Method not found in mono [bug]
- [#161](https://github.com/StefH/System.Linq.Dynamic.Core/issues/161) - Error (1.0.8.3 to 1.0.8.4 on EF 6.2) &quot;No generic method 'OrderBy' on type System.Linq.Queryable&quot;

# 1.0.8.5 (27 April 2018)
- [#160](https://github.com/StefH/System.Linq.Dynamic.Core/issues/160) - Question: Generic ParseLambda method [question]

# 1.0.8.4 (25 April 2018)
- [#159](https://github.com/StefH/System.Linq.Dynamic.Core/pull/159) - Performance fix (#153) contributed by [StefH](https://github.com/StefH)
- [#151](https://github.com/StefH/System.Linq.Dynamic.Core/issues/151) - Parse Query Syntax like Code
- [#153](https://github.com/StefH/System.Linq.Dynamic.Core/issues/153) - Issue: Performance while working with EF core

# 1.0.8.3 (30 March 2018)
- [#137](https://github.com/StefH/System.Linq.Dynamic.Core/pull/137) - Feature: Add support for querying a IQueryable&lt;dynamic&gt; contributed by [NickDarvey](https://github.com/NickDarvey)
- [#150](https://github.com/StefH/System.Linq.Dynamic.Core/pull/150) - Feature: Support Binary &amp; For String and Int [feature] contributed by [jogibear9988](https://github.com/jogibear9988)
- [#139](https://github.com/StefH/System.Linq.Dynamic.Core/issues/139) - Question: How to get related entities only one field [help wanted]
- [#140](https://github.com/StefH/System.Linq.Dynamic.Core/issues/140) - Question: Possible to write this using Dynamic Linq? [question]
- [#141](https://github.com/StefH/System.Linq.Dynamic.Core/issues/141) - Question: Is there TryParseLambda
- [#142](https://github.com/StefH/System.Linq.Dynamic.Core/issues/142) - Question: GroupJoin issue [question]
- [#146](https://github.com/StefH/System.Linq.Dynamic.Core/issues/146) - Question: where should support question be posted (here or SO) [question]

# 1.0.8.2 (09 January 2018)
- [#138](https://github.com/StefH/System.Linq.Dynamic.Core/pull/138) - Solved issue 130 contributed by [StefH](https://github.com/StefH)
- [#130](https://github.com/StefH/System.Linq.Dynamic.Core/issues/130) - BUG: Dynamic new in Where() causes NRE

# 1.0.8.1 (05 January 2018)
- [#135](https://github.com/StefH/System.Linq.Dynamic.Core/pull/135) - Add DbGeography to predefined types to allow advanced spatial queries. contributed by [czielin](https://github.com/czielin)
- [#59](https://github.com/StefH/System.Linq.Dynamic.Core/issues/59) - Cant compile DynamicLinqWebDocs [question]
- [#95](https://github.com/StefH/System.Linq.Dynamic.Core/issues/95) - Has no assembly version number.
- [#126](https://github.com/StefH/System.Linq.Dynamic.Core/issues/126) - How to make a request with Collate in order to get Accent Insensitive results?
- [#129](https://github.com/StefH/System.Linq.Dynamic.Core/issues/129) - ToDynamicList/ToDynamicArray cannot actually cast to specified type in .net core 2 [bug]
- [#131](https://github.com/StefH/System.Linq.Dynamic.Core/issues/131) - Adding LIKE operator for EF6
- [#132](https://github.com/StefH/System.Linq.Dynamic.Core/issues/132) - System.Linq.Dynamic.Core.Exceptions.ParseException in IQueryable&lt;object&gt; filled with anonymous type
- [#133](https://github.com/StefH/System.Linq.Dynamic.Core/issues/133) - Possibility to parse an Expression&lt;T, bool&gt; to a valid expression string 
- [#134](https://github.com/StefH/System.Linq.Dynamic.Core/issues/134) - Accessing DbGeography methods/properties

# 1.0.8.0 (16 December 2017)
- [#127](https://github.com/StefH/System.Linq.Dynamic.Core/pull/127) - Refactored Parser contributed by [StefH](https://github.com/StefH)

# 1.0.7.13 (29 November 2017)
- [#117](https://github.com/StefH/System.Linq.Dynamic.Core/pull/117) -  New features contributed by [jogibear9988](https://github.com/jogibear9988)
- [#123](https://github.com/StefH/System.Linq.Dynamic.Core/pull/123) - appveyor contributed by [StefH](https://github.com/StefH)
- [#114](https://github.com/StefH/System.Linq.Dynamic.Core/issues/114) - Dynamic Linq Query not usable with ORM Provider [bug]
- [#120](https://github.com/StefH/System.Linq.Dynamic.Core/issues/120) - Error with parsing
- [#121](https://github.com/StefH/System.Linq.Dynamic.Core/issues/121) - [Question] Count() method [question]
- [#122](https://github.com/StefH/System.Linq.Dynamic.Core/issues/122) - Join with int list
- [#124](https://github.com/StefH/System.Linq.Dynamic.Core/issues/124) -  OrderBy produces error.
- [#125](https://github.com/StefH/System.Linq.Dynamic.Core/issues/125) - Not compatable with dotnet Core 2. [bug]

# 1.0.7.12 (09 November 2017)
- [#115](https://github.com/StefH/System.Linq.Dynamic.Core/pull/115) - Dynamic Linq Query not usable with ORM Provider (fix for #114) contributed by [jogibear9988](https://github.com/jogibear9988)
- [#116](https://github.com/StefH/System.Linq.Dynamic.Core/pull/116) - Bugfix DynamicLinq when using IQueryable contributed by [jogibear9988](https://github.com/jogibear9988)
- [#108](https://github.com/StefH/System.Linq.Dynamic.Core/issues/108) - [Question] Nullable property inside Join statement
- [#109](https://github.com/StefH/System.Linq.Dynamic.Core/issues/109) - Not able to build with VS2017
- [#111](https://github.com/StefH/System.Linq.Dynamic.Core/issues/111) - [Bug] Incorrect Nullable&lt;&gt; parsing [bug, question]
- [#112](https://github.com/StefH/System.Linq.Dynamic.Core/issues/112) - Support NETStandard 2.0 [feature]
- [#113](https://github.com/StefH/System.Linq.Dynamic.Core/issues/113) - .pdb is missing in nuget [bug]

# 1.0.7.10 (27 October 2017)
- [#8](https://github.com/StefH/System.Linq.Dynamic.Core/pull/8) - Remove useless dependences [feature] contributed by [yyjdelete](https://github.com/yyjdelete)
- [#37](https://github.com/StefH/System.Linq.Dynamic.Core/pull/37) - Support strings as Enum Parameter Objects contributed by [jogibear9988](https://github.com/jogibear9988)
- [#38](https://github.com/StefH/System.Linq.Dynamic.Core/pull/38) - Support more comparisons with strings contributed by [jogibear9988](https://github.com/jogibear9988)
- [#39](https://github.com/StefH/System.Linq.Dynamic.Core/pull/39) - Exception friendly Type loading contributed by [jogibear9988](https://github.com/jogibear9988)
- [#47](https://github.com/StefH/System.Linq.Dynamic.Core/pull/47) - * Add unit test and fix public methods access. contributed by [jotab123](https://github.com/jotab123)
- [#55](https://github.com/StefH/System.Linq.Dynamic.Core/pull/55) - Fix Nullable Enums from String contributed by [jogibear9988](https://github.com/jogibear9988)
- [#56](https://github.com/StefH/System.Linq.Dynamic.Core/pull/56) - Create .editorconfig contributed by [jogibear9988](https://github.com/jogibear9988)
- [#68](https://github.com/StefH/System.Linq.Dynamic.Core/pull/68) - Work on #66 -&gt; Should work now. Tests will follow on VS2017 support! contributed by [jogibear9988](https://github.com/jogibear9988)
- [#69](https://github.com/StefH/System.Linq.Dynamic.Core/pull/69) - Fix - when method has object parameter and ValueType value is passed into this method, result is exception in System.Dynamic.Utils.ExpressionUtils.ValidateOneArgument. contributed by [DavidCizek](https://github.com/DavidCizek)
- [#76](https://github.com/StefH/System.Linq.Dynamic.Core/pull/76) - Fix - shift operators work only for int, short, ushort, byte, sbyte. contributed by [DavidCizek](https://github.com/DavidCizek)
- [#77](https://github.com/StefH/System.Linq.Dynamic.Core/pull/77) - New features: Hexadecimal integers and array initializers contributed by [DavidCizek](https://github.com/DavidCizek)
- [#78](https://github.com/StefH/System.Linq.Dynamic.Core/pull/78) - New feature: GroupJoin [feature] contributed by [ghost](https://github.com/ghost)
- [#80](https://github.com/StefH/System.Linq.Dynamic.Core/pull/80) - [Feature] Usage of cached Lambda Expressions contributed by [jogibear9988](https://github.com/jogibear9988)
- [#85](https://github.com/StefH/System.Linq.Dynamic.Core/pull/85) - [Fix] Guid? == null comparison contributed by [jogibear9988](https://github.com/jogibear9988)
- [#86](https://github.com/StefH/System.Linq.Dynamic.Core/pull/86) - [Fix] Fixed null in Parameter and added functionality Binary And and Or with different Types contributed by [jogibear9988](https://github.com/jogibear9988)
- [#92](https://github.com/StefH/System.Linq.Dynamic.Core/pull/92) - [Feature] Adds support for decimal qualifiers. Resolves #91 contributed by [pferraris](https://github.com/pferraris)
- [#93](https://github.com/StefH/System.Linq.Dynamic.Core/pull/93) - [Bug] Fix uap10 build in appveyor contributed by [StefH](https://github.com/StefH)
- [#99](https://github.com/StefH/System.Linq.Dynamic.Core/pull/99) - Added DynamicEnumerable Async extension methods contributed by [StefH](https://github.com/StefH)
- [#100](https://github.com/StefH/System.Linq.Dynamic.Core/pull/100) - Feature: NullPropagation operator contributed by [StefH](https://github.com/StefH)
- [#103](https://github.com/StefH/System.Linq.Dynamic.Core/pull/103) - support group by with 2 parameters, add tolist contributed by [jogibear9988](https://github.com/jogibear9988)
- [#1](https://github.com/StefH/System.Linq.Dynamic.Core/issues/1) - SymbolTable.DoesMethodHaveParameterArray throws exception when accessing a dynamic created property
- [#2](https://github.com/StefH/System.Linq.Dynamic.Core/issues/2) - UnitTest : GroupByAndSelect_TestDynamicSelectMember fails [bug]
- [#3](https://github.com/StefH/System.Linq.Dynamic.Core/issues/3) - Add &quot;SelectMany&quot; [feature]
- [#4](https://github.com/StefH/System.Linq.Dynamic.Core/issues/4) - Illegal one-byte branch at position: 9. Requested branch was: 143
- [#5](https://github.com/StefH/System.Linq.Dynamic.Core/issues/5) - Only parameterless constructors and initializers are supported in LINQ to Entities [bug]
- [#6](https://github.com/StefH/System.Linq.Dynamic.Core/issues/6) - Add support for dotnet5.4 framework [feature]
- [#7](https://github.com/StefH/System.Linq.Dynamic.Core/issues/7) - Add SelectMany with resultSelector [feature]
- [#9](https://github.com/StefH/System.Linq.Dynamic.Core/issues/9) - Add Null-coalescing operator support [feature]
- [#10](https://github.com/StefH/System.Linq.Dynamic.Core/issues/10) - Support explicit integer qualifiers [feature]
- [#11](https://github.com/StefH/System.Linq.Dynamic.Core/issues/11) - IN does not support negative and parse of negative integers with qualifiers.  [bug]
- [#12](https://github.com/StefH/System.Linq.Dynamic.Core/issues/12) - parsing negative float or double with qualifier [feature]
- [#13](https://github.com/StefH/System.Linq.Dynamic.Core/issues/13) - Add isnull sql function &quot;isnull(a, b)&quot; [feature]
- [#14](https://github.com/StefH/System.Linq.Dynamic.Core/issues/14) - Ampersand can be used both as logical And or as vb-like concatenation operator [feature]
- [#15](https://github.com/StefH/System.Linq.Dynamic.Core/issues/15) - Add Skip, Take to ExpressionParser [feature]
- [#16](https://github.com/StefH/System.Linq.Dynamic.Core/issues/16) - Add Paging support [feature]
- [#17](https://github.com/StefH/System.Linq.Dynamic.Core/issues/17) - Windows 10 uwp support [help wanted]
- [#18](https://github.com/StefH/System.Linq.Dynamic.Core/issues/18) - SelectMany over an Array throws System.IndexOutOfRangeException [bug]
- [#19](https://github.com/StefH/System.Linq.Dynamic.Core/issues/19) - NotEqual filter not working with DateTime [bug]
- [#20](https://github.com/StefH/System.Linq.Dynamic.Core/issues/20) - Can't install using nuget in Asp.Net 4.0 Web Pages project
- [#21](https://github.com/StefH/System.Linq.Dynamic.Core/issues/21) - Question: why is Distinct not supported? [feature]
- [#22](https://github.com/StefH/System.Linq.Dynamic.Core/issues/22) - DynamicExpression accessibility [feature]
- [#23](https://github.com/StefH/System.Linq.Dynamic.Core/issues/23) - Cannot work with property which in base class. [bug]
- [#24](https://github.com/StefH/System.Linq.Dynamic.Core/issues/24) - FirstOrDefaultAsync method is missing
- [#25](https://github.com/StefH/System.Linq.Dynamic.Core/issues/25) - DynamicExpression gone in version 1.0.3.4
- [#26](https://github.com/StefH/System.Linq.Dynamic.Core/issues/26) - Calling ToString on a nullable column throws error
- [#27](https://github.com/StefH/System.Linq.Dynamic.Core/issues/27) - UWP version
- [#28](https://github.com/StefH/System.Linq.Dynamic.Core/issues/28) - Dynamic Queries seem to lose &quot;Include()&quot;s
- [#29](https://github.com/StefH/System.Linq.Dynamic.Core/issues/29) - An another project ?
- [#30](https://github.com/StefH/System.Linq.Dynamic.Core/issues/30) - Move to .NET Core RTM
- [#31](https://github.com/StefH/System.Linq.Dynamic.Core/issues/31) - Group by multiple columns? [bug]
- [#32](https://github.com/StefH/System.Linq.Dynamic.Core/issues/32) - When same dynamic class is first used in Linq2Entities, it's reused for Linq2Sql [bug]
- [#33](https://github.com/StefH/System.Linq.Dynamic.Core/issues/33) - Package 1.0.6.3 install fails for UWP App [bug]
- [#34](https://github.com/StefH/System.Linq.Dynamic.Core/issues/34) - Support for netcoreapp1.0? [question]
- [#35](https://github.com/StefH/System.Linq.Dynamic.Core/issues/35) - Compatibility with System.Linq.Dynamic.Library  [question]
- [#36](https://github.com/StefH/System.Linq.Dynamic.Core/issues/36) - CreateClass Equivalent? [feature]
- [#40](https://github.com/StefH/System.Linq.Dynamic.Core/issues/40) - Add strong naming from library [feature]
- [#42](https://github.com/StefH/System.Linq.Dynamic.Core/issues/42) - Microsoft.EntityFrameworkCore.DynamicLinq - ToListAsync()?
- [#43](https://github.com/StefH/System.Linq.Dynamic.Core/issues/43) - Join with dependent subquery?
- [#44](https://github.com/StefH/System.Linq.Dynamic.Core/issues/44) - Casting a int to a nullable int will throw an error when using linq to entities&quot;Only parameterless constructors and initializers are supported in LINQ to Entities&quot;
- [#45](https://github.com/StefH/System.Linq.Dynamic.Core/issues/45) - Take() and Skip() lose ElementType [bug]
- [#46](https://github.com/StefH/System.Linq.Dynamic.Core/issues/46) - Methods on type are not accessible error
- [#48](https://github.com/StefH/System.Linq.Dynamic.Core/issues/48) - Add an overload to the &quot;ToDynamicList&quot; method which accepts a Type [feature]
- [#49](https://github.com/StefH/System.Linq.Dynamic.Core/issues/49) - .Contains(&quot;&quot;) operation Exception [help wanted]
- [#50](https://github.com/StefH/System.Linq.Dynamic.Core/issues/50) - Add functionality to optimize your queries using Linq.Expression.Optimizer [feature]
- [#51](https://github.com/StefH/System.Linq.Dynamic.Core/issues/51) - [Question] How can I format a datetime (nullable) field value in select? [help wanted]
- [#52](https://github.com/StefH/System.Linq.Dynamic.Core/issues/52) - Can I convert int to string type?
- [#57](https://github.com/StefH/System.Linq.Dynamic.Core/issues/57) - Issue finding indexer [bug]
- [#58](https://github.com/StefH/System.Linq.Dynamic.Core/issues/58) - Parse Lambda [question]
- [#60](https://github.com/StefH/System.Linq.Dynamic.Core/issues/60) - Issue with nested Calls
- [#61](https://github.com/StefH/System.Linq.Dynamic.Core/issues/61) - String[].Contains(String) expression translated to first array element contains [invalid]
- [#62](https://github.com/StefH/System.Linq.Dynamic.Core/issues/62) - OrderBy Chaining [bug, feature]
- [#63](https://github.com/StefH/System.Linq.Dynamic.Core/issues/63) - Syntax IN dont work with Enums
- [#65](https://github.com/StefH/System.Linq.Dynamic.Core/issues/65) - Support embedded quotes in string literal [feature]
- [#66](https://github.com/StefH/System.Linq.Dynamic.Core/issues/66) - Is there Way to enter a Complex query
- [#67](https://github.com/StefH/System.Linq.Dynamic.Core/issues/67) - Convert Project to VS2017 [feature]
- [#70](https://github.com/StefH/System.Linq.Dynamic.Core/issues/70) - Move all tests into 1 test project [feature]
- [#72](https://github.com/StefH/System.Linq.Dynamic.Core/issues/72) - [Bug] Re-enable support for uap10.0 [bug]
- [#73](https://github.com/StefH/System.Linq.Dynamic.Core/issues/73) - [Feature] Extend OrderBy functionality [feature]
- [#74](https://github.com/StefH/System.Linq.Dynamic.Core/issues/74) - Join on nullable and not nullable type throws exception [bug]
- [#75](https://github.com/StefH/System.Linq.Dynamic.Core/issues/75) - GroupBy clause add an &quot;Item&quot; property when projects the query
- [#79](https://github.com/StefH/System.Linq.Dynamic.Core/issues/79) - DynamicExpressionParser does not allow empty parameter lists. [bug]
- [#81](https://github.com/StefH/System.Linq.Dynamic.Core/issues/81) - [Question] Create new nuget?
- [#82](https://github.com/StefH/System.Linq.Dynamic.Core/issues/82) - [Feature] Add DefaultIfEmpty [feature]
- [#83](https://github.com/StefH/System.Linq.Dynamic.Core/issues/83) - [Bug] Problem with DynamicExpressionParser.ParseLambda &amp; external ParameterExpression [question]
- [#84](https://github.com/StefH/System.Linq.Dynamic.Core/issues/84) - [Bug] DynamicClassFactory not caching generated types [bug]
- [#87](https://github.com/StefH/System.Linq.Dynamic.Core/issues/87) - [Question] Memory leak in Join method [question]
- [#88](https://github.com/StefH/System.Linq.Dynamic.Core/issues/88) - [Question] FileNotFoundException: Could not load file or assembly 'System.Linq.Dynamic.Core' [question]
- [#89](https://github.com/StefH/System.Linq.Dynamic.Core/issues/89) - [Question] System.Linq.Dynamic.Core.Exceptions.ParseException: 'No 'it' is in scope' [question]
- [#90](https://github.com/StefH/System.Linq.Dynamic.Core/issues/90) - [Bug] ParseIntegerLiteral Int16 [bug]
- [#91](https://github.com/StefH/System.Linq.Dynamic.Core/issues/91) - [Bug] Support for decimal qualifiers 'M' &amp; 'm' [bug]
- [#94](https://github.com/StefH/System.Linq.Dynamic.Core/issues/94) - [Bug] ParseException: Operator '==' incompatible with operand types 'ObjectId' and 'ObjectId' [bug, question]
- [#96](https://github.com/StefH/System.Linq.Dynamic.Core/issues/96) - Async support for ToDynamicList() [feature]
- [#97](https://github.com/StefH/System.Linq.Dynamic.Core/issues/97) - Dynamic Select with string Concatenation [question]
- [#101](https://github.com/StefH/System.Linq.Dynamic.Core/issues/101) - Question: OrderBy does not work with navigation properties [question]
- [#102](https://github.com/StefH/System.Linq.Dynamic.Core/issues/102) - Aggregate method does not work with Average function
- [#104](https://github.com/StefH/System.Linq.Dynamic.Core/issues/104) - Add PDB to nuget package [feature]
- [#106](https://github.com/StefH/System.Linq.Dynamic.Core/issues/106) - Using both System.Linq and System.Linq.Dynamic.Core 
- [#107](https://github.com/StefH/System.Linq.Dynamic.Core/issues/107) - Type conversions generated in cases where they're not needed. [bug]


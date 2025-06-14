# System.Linq.Dynamic.Core
This is a **.NET Core / Standard port** of the Microsoft assembly for the .Net 4.0 Dynamic language functionality.

---

## Overview
With this library it's possible to write Dynamic LINQ queries (string based) on an `IQueryable`:
``` c#
var query = db.Customers
    .Where("City == @0 and Orders.Count >= @1", "London", 10)
    .OrderBy("CompanyName")
    .Select("new(CompanyName as Name, Phone)");
```

Interpolated strings are supported on .NET 4.6(and above), .NET Core 2.1(and above), .NET Standard 1.3(and above) and UAP 10.0(and above).
For example:
``` csharp
string cityName = "London";
int c = 10;
db.Customers.WhereInterpolated($"City == {cityName} and Orders.Count >= {c}");
```

---

## Sponsors

ZZZ Projects owns and maintains **System.Linq.Dynamic.Core** as part of our [mission](https://zzzprojects.com/mission) to add value to the .NET community

Through [Entity Framework Extensions](https://entityframework-extensions.net/?utm_source=zzzprojects&utm_medium=systemlinqdynamiccore) and [Dapper Plus](https://dapper-plus.net/?utm_source=zzzprojects&utm_medium=systemlinqdynamiccore), we actively sponsor and help key open-source libraries grow.

[![Entity Framework Extensions](https://raw.githubusercontent.com/zzzprojects/System.Linq.Dynamic.Core/master/entity-framework-extensions-sponsor.png)](https://entityframework-extensions.net/bulk-insert?utm_source=zzzprojects&utm_medium=systemlinqdynamiccore)

[![Dapper Plus](https://raw.githubusercontent.com/zzzprojects/System.Linq.Dynamic.Core/master/dapper-plus-sponsor.png)](https://dapper-plus.net/bulk-insert?utm_source=zzzprojects&utm_medium=systemlinqdynamiccore)

---

## :exclamation: Breaking changes

### v1.3.0
A breaking change is introduced in this version which is related to calling methods on classes.
Due to security reasons, it's now only allowed to call methods on the standard predefined classes like (`bool`, `int`, `string` ...).
If you want to call a method on an own custom class, annotate that class with the [DynamicLinqType](https://dynamic-linq.net/advanced-extending#dynamiclinqtype-attribute).
Example:
``` c#
[DynamicLinqType]
public class MyCustomClass
{
    public int GetAge(int x) => x;
}
```
If it's not possible to add that attribute, you need to implement a custom [CustomTypeProvider](https://dynamic-linq.net/advanced-configuration#customtypeprovider) and set this to the `ParsingConfig` and provide that config to all dynamic calls.
Or provide a list of additional types in the [DefaultDynamicLinqCustomTypeProvider.cs](https://github.com/zzzprojects/System.Linq.Dynamic.Core/blob/master/src/System.Linq.Dynamic.Core/CustomTypeProviders/DefaultDynamicLinqCustomTypeProvider.cs).

### v1.6.0
#### Change 1
It's not allowed anymore to call any methods on the `object` type. By default also the `ToString` and `Equals` methods are not allowed.
This is done to mitigate the risk of calling methods on the `object` type which could lead to security issues (CVE-2024-51417).
To allow these methods set `AllowEqualsAndToStringMethodsOnObject` to `true` in the `ParsingConfig` and provide that config to all dynamic calls.

#### Change 2
By default the `RestrictOrderByToPropertyOrField` is now set to `true` in the `ParsingConfig`. 
Which means that only properties and fields can be used in the `OrderBy` / `ThenBy`.
This is done to mitigate the risk of calling methods or other expressions in the `OrderBy` / `ThenBy` which could lead to security issues.
To allow these methods set `RestrictOrderByToPropertyOrField` to `false` in the `ParsingConfig` and provide that config to all dynamic calls.

#### Change 3
The `DefaultDynamicLinqCustomTypeProvider` has been changed to only return types which have the `[DynamicLinqType]` attribute applied.
If it's not possible to add that attribute, you need to implement a custom [CustomTypeProvider](https://dynamic-linq.net/advanced-configuration#customtypeprovider) and set this to the `ParsingConfig` and provide that config to all dynamic calls.
Or provide a list of additional types in the [DefaultDynamicLinqCustomTypeProvider.cs](https://github.com/zzzprojects/System.Linq.Dynamic.Core/blob/master/src/System.Linq.Dynamic.Core/CustomTypeProviders/DefaultDynamicLinqCustomTypeProvider.cs).

---

## Useful links
- [Website](https://dynamic-linq.net)
- [Documentation](https://dynamic-linq.net/overview)
- [Online examples](https://dynamic-linq.net/online-examples)
- [NuGet](https://www.nuget.org/packages/System.Linq.Dynamic.Core)

## Info
| | |
| --- | --- |
| ***Project*** | &nbsp; |
| &nbsp;&nbsp;**Chat** | [![Gitter](https://img.shields.io/gitter/room/system-linq-dynamic-core/Lobby.svg)](https://gitter.im/system-linq-dynamic-core/Lobby) |
| &nbsp;&nbsp;**Issues** | [![GitHub issues](https://img.shields.io/github/issues/StefH/System.Linq.Dynamic.Core.svg)](https://github.com/StefH/System.Linq.Dynamic.Core/issues) |
| | |
| ***Quality*** | &nbsp; |
| &nbsp;&nbsp;**CI Workflow** | ![CI Workflow](https://github.com/zzzprojects/System.Linq.Dynamic.Core/actions/workflows/ci.yml/badge.svg) |
| &nbsp;&nbsp;**SonarCloud** | [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=zzzprojects_System.Linq.Dynamic.Core&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=zzzprojects_System.Linq.Dynamic.Core) |
| |
| ***NuGet*** | &nbsp; |
| &nbsp;&nbsp;**System.Linq.Dynamic.Core** | [![NuGet](https://img.shields.io/nuget/v/System.Linq.Dynamic.Core)](https://www.nuget.org/packages/System.Linq.Dynamic.Core) |
| &nbsp;&nbsp;**EntityFramework.DynamicLinq** | [![NuGet](https://img.shields.io/nuget/v/EntityFramework.DynamicLinq)](https://www.nuget.org/packages/EntityFramework.DynamicLinq) |
| &nbsp;&nbsp;**Microsoft.EntityFrameworkCore.DynamicLinq** | [![NuGet](https://img.shields.io/nuget/v/Microsoft.EntityFrameworkCore.DynamicLinq)](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.DynamicLinq) |
| &nbsp;&nbsp;**Z.EntityFramework.Classic.DynamicLinq** | [![NuGet](https://img.shields.io/nuget/v/Z.EntityFramework.Classic.DynamicLinq)](https://www.nuget.org/packages/Z.EntityFramework.Classic.DynamicLinq) |
| &nbsp;&nbsp;**Z.DynamicLinq.SystemTextJson** | [![NuGet](https://img.shields.io/nuget/v/Z.DynamicLinq.SystemTextJson)](https://www.nuget.org/packages/Z.DynamicLinq.SystemTextJson) |
| &nbsp;&nbsp;**Z.DynamicLinq.NewtonsoftJson** | [![NuGet](https://img.shields.io/nuget/v/Z.DynamicLinq.NewtonsoftJson)](https://www.nuget.org/packages/Z.DynamicLinq.NewtonsoftJson) |

## Development Details

### Frameworks
The following frameworks are supported:
- net35, net40, net45, net46 and up
- netstandard1.3, netstandard2.0 and netstandard2.1
- netcoreapp3.1, net5.0, net6.0, net7.0, net8.0 and net9.0
- uap10.0

### Fork details
This fork takes the basic library to a new level. Also adds unit tests to help ensure that it works properly.

Some background:
I forked from https://github.com/NArnott/System.Linq.Dynamic and added some more functionality there.<br>My fork is still visible on github [https://github.com/StefH/System.Linq.Dynamic], however I decided to start a new project + NuGet to avoid confusion and create the project according to the new VS2017 + .NET Core rules / standards.

However, currently there are multiple nuget packages and projects available:

| Project | NuGet | Author | Comment |
| ------- | ----- | ------ | ------- |
| [kahanu/System.Linq.Dynamic][2a] | [System.Linq.Dynamic][2b] | @kahanu | - |
| [kavun/System.Linq.Dynamic.3.5][3a] | [System.Linq.Dynamic.3.5/][3b] | @kavun | only 3.5 and VB.NET |
| [NArnott/System.Linq.Dynamic][4a] | [System.Linq.Dynamic.Library][4b]  | @NArnott | removed from github + nuget ? |
| [dynamiclinq.codeplex][5a] | - | dialectsoftware | - |
| [dynamic-linq][6a] | - | scottgu | - |

[2a]: https://github.com/kahanu/System.Linq.Dynamic
[2b]: https://www.nuget.org/packages/System.Linq.Dynamic
[3a]: https://github.com/kavun/System.Linq.Dynamic.3.5
[3b]: https://www.nuget.org/packages/System.Linq.Dynamic.3.5/
[4a]: https://github.com/NArnott/System.Linq.Dynamic
[4b]: https://www.nuget.org/packages/System.Linq.Dynamic.Library
[5a]: https://dynamiclinq.codeplex.com/
[6a]: http://weblogs.asp.net/scottgu/dynamic-linq-part-1-using-the-linq-dynamic-query-library

[doc-api]: http://zzzprojects.github.io/System.Linq.Dynamic.Core
[doc-wiki]: https://github.com/zzzprojects/System.Linq.Dynamic.Core/wiki/Dynamic-Expressions

## Contribute

The best way to contribute is by **spreading the word** about the library:

 - Blog it
 - Comment it
 - Star it
 - Share it
 
A **HUGE THANKS** for your help.

## More Projects

- Projects:
   - [EntityFramework Extensions](https://entityframework-extensions.net/)
   - [Dapper Plus](https://dapper-plus.net/)
   - [C# Eval Expression](https://eval-expression.net/)
- Learn Websites
   - [Learn EF Core](https://www.learnentityframeworkcore.com/)
   - [Learn Dapper](https://www.learndapper.com/)
- Online Tools:
   - [.NET Fiddle](https://dotnetfiddle.net/)
   - [SQL Fiddle](https://sqlfiddle.com/)
   - [ZZZ Code AI](https://zzzcode.ai/)
- and much more!

To view all our free and paid projects, visit our website [ZZZ Projects](https://zzzprojects.com/).

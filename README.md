# System.Linq.Dynamic.Core
This is a **.NET Core / Standard port** of the Microsoft assembly for the .Net 4.0 Dynamic language functionality.

# Overview
With this library it's possible to write Dynamic LINQ queries (string based) on an `IQueryable`:
``` c#
var query = db.Customers
    .Where("City == @0 and Orders.Count >= @1", "London", 10)
    .OrderBy("CompanyName")
    .Select("new(CompanyName as Name, Phone)");
```

# How to use
There are several documentation resources:
- [Getting Started](https://dynamic-linq.net/overview) : a website to get started with basic and advanced usage.
- [API Documentation][doc-api] : a low-level API description website with some code samples

# Info
| | |
| --- | --- |
| ***Project*** | &nbsp; |
| &nbsp;&nbsp;**Chat** | [![Gitter](https://img.shields.io/gitter/room/system-linq-dynamic-core/Lobby.svg)](https://gitter.im/system-linq-dynamic-core/Lobby) |
| &nbsp;&nbsp;**Issues** | [![GitHub issues](https://img.shields.io/github/issues/StefH/System.Linq.Dynamic.Core.svg)](https://github.com/StefH/System.Linq.Dynamic.Core/issues) |
| | |
| ***Quality*** | &nbsp; |
| &nbsp;&nbsp;**Main workflow** | ![Main workflow](https://github.com/zzzprojects/System.Linq.Dynamic.Core/workflows/Main%20workflow/badge.svg) |
| |
| ***NuGet*** | &nbsp; |
| &nbsp;&nbsp;**System.Linq.Dynamic.Core** | [![NuGet](https://buildstats.info/nuget/System.Linq.Dynamic.Core)](https://www.nuget.org/packages/System.Linq.Dynamic.Core) |
| &nbsp;&nbsp;**EntityFramework.DynamicLinq** | [![NuGet](https://buildstats.info/nuget/EntityFramework.DynamicLinq)](https://www.nuget.org/packages/EntityFramework.DynamicLinq) |
| &nbsp;&nbsp;**Microsoft.EntityFrameworkCore.DynamicLinq** | [![NuGet](https://buildstats.info/nuget/Microsoft.EntityFrameworkCore.DynamicLinq)](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.DynamicLinq) |
| &nbsp;&nbsp;**Z.EntityFramework.Classic.DynamicLinq** | [![NuGet](https://buildstats.info/nuget/Z.EntityFramework.Classic.DynamicLinq)](https://www.nuget.org/packages/Z.EntityFramework.Classic.DynamicLinq) |
| ***MyGet (preview)*** | &nbsp; |
| &nbsp;&nbsp;**System.Linq.Dynamic.Core** | [![MyGet](https://buildstats.info/myget/system-linq-dynamic-core/System.Linq.Dynamic.Core)](https://www.myget.org/feed/system-linq-dynamic-core/package/nuget/System.Linq.Dynamic.Core) |
| &nbsp;&nbsp;**EntityFramework.DynamicLinq** | [![MyGet](https://buildstats.info/myget/system-linq-dynamic-core/EntityFramework.DynamicLinq)](https://www.myget.org/feed/system-linq-dynamic-core/package/nuget/EntityFramework.DynamicLinq) |
| &nbsp;&nbsp;**Microsoft.EntityFrameworkCore.DynamicLinq** | [![MyGet](https://buildstats.info/myget/system-linq-dynamic-core/Microsoft.EntityFrameworkCore.DynamicLinq)](https://www.myget.org/feed/system-linq-dynamic-core/package/nuget/Microsoft.EntityFrameworkCore.DynamicLinq) |
| &nbsp;&nbsp;**Z.EntityFramework.Classic.DynamicLinq** | [![MyGet](https://buildstats.info/myget/system-linq-dynamic-core/Z.EntityFramework.Classic.DynamicLinq)](https://www.myget.org/feed/system-linq-dynamic-core/package/nuget/Z.EntityFramework.Classic.DynamicLinq) |

# Development Details

## Frameworks
The following frameworks are supported:
- net35, net40, net45, net46 and up
- netstandard1.3 & netstandard2.0
- uap10.0 (UAP 10.0.14393.0)

## Fork details
This fork takes the basic library to a new level. Contains XML Documentation and examples on how to use it. Also adds unit testing to help ensure that it works properly.

Some background:
I forked from https://github.com/NArnott/System.Linq.Dynamic and added some more functionality there.<br>My fork is still visible on github [https://github.com/StefH/System.Linq.Dynamic], however I decided to start a new project + nuget to avoid confusion and create the project according to the new VS2017 + .NET Core rules / standards.

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

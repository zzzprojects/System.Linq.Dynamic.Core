# System.Linq.Dynamic.Core
This is a **.NET Core / Standard port** of the Microsoft assembly for the .Net 4.0 Dynamic language functionality.

# Overview
With this library it's possible to write Dynamic LINQ queries (string based) on an `IQueryable`:
```csharp
var query = db.Customers
    .Where("City == @0 and Orders.Count >= @1", "London", 10)
    .OrderBy("CompanyName")
    .Select("new(CompanyName as Name, Phone)");
```

# How to use
There are several documentation resources:
- [Wiki][doc-wiki] : an overview from the basic functionality  (correctness ~ 80%)
- [API Documentation][doc-api] : a low-level API description website with some code samples
- DynamicLinqWebDocs (TODO)

# Info
| | |
| --- | --- |
| ***Project*** | &nbsp; |
| &nbsp;&nbsp;**Chat** | [![Gitter](https://img.shields.io/gitter/room/system-linq-dynamic-core/Lobby.svg)](https://gitter.im/system-linq-dynamic-core/Lobby) |
| &nbsp;&nbsp;**Issues** | [![GitHub issues](https://img.shields.io/github/issues/StefH/System.Linq.Dynamic.Core.svg)](https://github.com/StefH/System.Linq.Dynamic.Core/issues) |
| | |
| ***Quality*** | &nbsp; |
| &nbsp;&nbsp;**Build** | [![Build status](https://ci.appveyor.com/api/projects/status/0c4v2bsvdqd57600?svg=true)](https://ci.appveyor.com/project/StefH/system-linq-dynamic-core) |
| &nbsp;&nbsp;**CodeFactor** | [![CodeFactor](https://www.codefactor.io/repository/github/stefh/system.linq.dynamic.core/badge)](https://www.codefactor.io/repository/github/stefh/system.linq.dynamic.core)
| &nbsp;&nbsp;**Sonar Quality Gate** | [![Sonar Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=system.linq.dynamic.core&metric=alert_status)](https://sonarcloud.io/project/issues?id=system.linq.dynamic.core) |
| &nbsp;&nbsp;**Sonar Bugs** | [![Sonar Bugs](https://sonarcloud.io/api/project_badges/measure?project=system.linq.dynamic.core&metric=bugs)](https://sonarcloud.io/project/issues?id=system.linq.dynamic.core&resolved=false&types=BUG) |
| &nbsp;&nbsp;**Sonar Code Smells** | [![Sonar Code Smells](https://sonarcloud.io/api/project_badges/measure?project=system.linq.dynamic.core&metric=code_smells)](https://sonarcloud.io/project/issues?id=system.linq.dynamic.core&resolved=false&types=CODE_SMELL) |
| &nbsp;&nbsp;**Sonar Coverage** | [![Sonar Coverage](https://sonarcloud.io/api/project_badges/measure?project=system.linq.dynamic.core&metric=coverage)](https://sonarcloud.io/component_measures?id=system.linq.dynamic.core&metric=coverage) |
| &nbsp;&nbsp;**Codecov** | [![codecov](https://codecov.io/gh/StefH/System.Linq.Dynamic.Core/branch/master/graph/badge.svg)](https://codecov.io/gh/StefH/System.Linq.Dynamic.Core) |
| &nbsp;&nbsp;**Coveralls** | [![Coverage Status](https://coveralls.io/repos/github/StefH/System.Linq.Dynamic.Core/badge.svg?branch=master)](https://coveralls.io/github/StefH/System.Linq.Dynamic.Core?branch=master) |
| |
| ***Nuget*** | &nbsp; |
| &nbsp;&nbsp;**System.Linq.Dynamic.Core** | [![NuGet Badge](https://buildstats.info/nuget/System.Linq.Dynamic.Core)](https://www.nuget.org/packages/System.Linq.Dynamic.Core) |
| &nbsp;&nbsp;**EntityFramework.DynamicLinq** | [![NuGet Badge](https://buildstats.info/nuget/EntityFramework.DynamicLinq)](https://www.nuget.org/packages/EntityFramework.DynamicLinq) |
| &nbsp;&nbsp;**Microsoft.EntityFrameworkCore.DynamicLinq** | [![NuGet Badge](https://buildstats.info/nuget/Microsoft.EntityFrameworkCore.DynamicLinq)](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.DynamicLinq) |


# Development Details

## Frameworks
The following frameworks are supported:
- net35
- net40
- net45 and up
- netstandard1.3
- netstandard2.0
- uap10.0

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

[doc-api]: https://system-linq-dynamic-core.azurewebsites.net
[doc-wiki]: https://github.com/StefH/System.Linq.Dynamic.Core/wiki/Dynamic-Expressions

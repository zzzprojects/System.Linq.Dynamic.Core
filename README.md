# System.Linq.Dynamic.Core

[![Build status](https://ci.appveyor.com/api/projects/status/0c4v2bsvdqd57600?svg=true)](https://ci.appveyor.com/project/StefH/system-linq-dynamic-core)
[![codecov](https://codecov.io/gh/StefH/System.Linq.Dynamic.Core/branch/master/graph/badge.svg)](https://codecov.io/gh/StefH/System.Linq.Dynamic.Core)
[![Coverage Status](https://coveralls.io/repos/github/StefH/System.Linq.Dynamic.Core/badge.svg?branch=master)](https://coveralls.io/github/StefH/System.Linq.Dynamic.Core?branch=master)
[![GitHub issues](https://img.shields.io/github/issues/StefH/System.Linq.Dynamic.Core.svg)](https://github.com/StefH/System.Linq.Dynamic.Core/issues)
[![GitHub stars](https://img.shields.io/github/stars/StefH/System.Linq.Dynamic.Core.svg)](https://github.com/StefH/System.Linq.Dynamic.Core/stargazers)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/StefH/System.Linq.Dynamic.Core/master/LICENSE)

| Project | NuGet |
| ------- | ----- |
| System.Linq.Dynamic.Core | [![NuGet Badge](https://buildstats.info/nuget/System.Linq.Dynamic.Core)](https://www.nuget.org/packages/System.Linq.Dynamic.Core) |
| EntityFramework.DynamicLinq | [![NuGet Badge](https://buildstats.info/nuget/EntityFramework.DynamicLinq)](https://www.nuget.org/packages/EntityFramework.DynamicLinq) |
| Microsoft.EntityFrameworkCore.DynamicLinq | [![NuGet Badge](https://buildstats.info/nuget/Microsoft.EntityFrameworkCore.DynamicLinq)](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.DynamicLinq) |

This is a **.NET Core/Standard port** of the Microsoft assembly for the .Net 4.0 Dynamic language functionality.


# Functionality
With this library it's possible to write Dynamic LINQ queries (string based), example:
```csharp
var query = db.Customers
    .Where("City == @0 and Orders.Count >= @1", "London", 10)
    .OrderBy("CompanyName")
    .Select("new(CompanyName as Name, Phone)");
```

See the [Wiki][2] and [API Documentation][1] for more code examples and usage details.


# Development Details

## Frameworks
The following frameworks are supported:
- net35
- net40
- net45 and up
- netstandard1.3
- uap10.0

## Fork details
This fork takes the basic library to a new level. Contains XML Documentation and examples on how to use it. Also adds unit testing to help ensure that it works properly.

Some background:
I forked from https://github.com/NArnott/System.Linq.Dynamic and added some more functionality there.<br>My fork is still visible on github [https://github.com/StefH/System.Linq.Dynamic], however I decided to start a new project + nuget to avoid confusion and create the project according to the new VS2017 + .NET Core rules / standards.

However, currently there are multiple nuget packages and project available:

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

[1]: https://system-linq-dynamic-core.azurewebsites.net
[2]: https://github.com/StefH/System.Linq.Dynamic.Core/wiki/Dynamic-Expressions

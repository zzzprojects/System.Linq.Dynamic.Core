# System.Linq.Dynamic.Core

[![Build status](https://ci.appveyor.com/api/projects/status/0c4v2bsvdqd57600?svg=true)](https://ci.appveyor.com/project/StefH/system-linq-dynamic-core)
[![Version](https://img.shields.io/nuget/v/System.Linq.Dynamic.Core.svg)](https://www.nuget.org/packages/System.Linq.Dynamic.Core)

This is a **.NET Core port** of the the Microsoft assembly for the .Net 4.0 Dynamic language functionality.

The following frameworks are supported:
- net35
- net40
- net45x
- net46x
- dnx451
- netstandard1.3
- netcore45
- netcore451
- netcore50
- uap10.0
- SilverLight 5.0

<br>
This fork takes the basic library to a new level. Contains XML Documentation and examples on how to use it. Also adds unit testing to help ensure that it works properly.

<br>
Some background:
I forked from https://github.com/NArnott/System.Linq.Dynamic and added some more functionality there.<br>My fork is still visible on github [https://github.com/StefH/System.Linq.Dynamic], however I decided to start a new project + nuget to avoid confusion and create the project according to the new VS2015 + dotnet Core standards.





However, currently there are multiple nuget packages and project available:

| Project | NuGet | Author | Comment |
| ------- | ----- | ------ | ------- |
| [kahanu/System.Linq.Dynamic][2a] | [System.Linq.Dynamic][2b] | @kahanu | - |
| [kavun/System.Linq.Dynamic.3.5][3a] | [System.Linq.Dynamic.3.5/][3b] | @kavun | only 3.5 and VB.NET |
| [StefH/System.Linq.Dynamic][4a] | [System.Linq.Dynamic.Library][4b]  | @NArnott | removed from github + nuget ? |
| [davidfowl/DynamicLinq][5a] | - | @davidfowl | - |

So some investigation is needed to compare functionality from all these projects and mine.
An idea would be to analyse all the issues reported on these projects, and add unit-tests in my project to see if these issues are fixed or still need a fix.

Suggestions and comments are welcome.

Documentation can be found [here][1].

[2a]: https://github.com/kahanu/System.Linq.Dynamic
[2b]: https://www.nuget.org/packages/System.Linq.Dynamic
[3a]: https://github.com/kavun/System.Linq.Dynamic.3.5
[3b]: https://www.nuget.org/packages/System.Linq.Dynamic.3.5/
[4a]: https://github.com/StefH/System.Linq.Dynamic
[4b]: https://www.nuget.org/packages/System.Linq.Dynamic.Library
[5a]: https://github.com/davidfowl/DynamicLinq


[1]: https://system-linq-dynamic-core.azurewebsites.net

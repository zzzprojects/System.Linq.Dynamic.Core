rem https://www.appveyor.com/blog/2017/03/17/codecov/

%USERPROFILE%\.nuget\packages\opencover\4.6.519\tools\OpenCover.Console.exe -target:dotnet.exe -targetargs:"test test\System.Linq.Dynamic.Core.Tests\System.Linq.Dynamic.Core.Tests.csproj --no-build --framework netcoreapp1.1" -filter:"+[EntityFramework.DynamicLinq]* +[Microsoft.EntityFrameworkCore.DynamicLinq]* +[System.Linq.Dynamic.Core]* -[*Tests*]* -[System.Linq.Dynamic.Core]System.Linq.Dynamic.Core.Validation.*" -nodefaultfilters -output:coverage.xml -register:user -oldStyle

%USERPROFILE%\.nuget\packages\ReportGenerator\2.5.6\tools\ReportGenerator.exe -reports:"coverage.xml" -targetdir:"report"

start report\index.htm

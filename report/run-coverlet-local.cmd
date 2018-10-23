set buildType=azure-pipelines-ci
dotnet test ..\test\System.Linq.Dynamic.Core.Tests\System.Linq.Dynamic.Core.Tests.csproj -c Debug -f netcoreapp2.1 /p:CollectCoverage=true /p:CoverletOutputFormat=\"opencover,lcov\" /p:CoverletOutput="../../report/"

%USERPROFILE%\.nuget\packages\ReportGenerator\3.1.2\tools\ReportGenerator.exe -reports:"coverage.opencover.xml" -targetdir:"coverlet"

start coverlet\index.htm

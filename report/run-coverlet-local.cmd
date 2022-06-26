rem Run this in a CommandPrompt, not in PowerShell.

SET buildType=azure-pipelines-ci
SET testproject=..\test\System.Linq.Dynamic.Core.Tests.Net6\System.Linq.Dynamic.Core.Tests.Net6.csproj

del coverage.info
del coverage.opencover.xml
del /q coverlet\*.*

dotnet clean %testproject%

dotnet build %testproject% -c Debug -f net6.0

dotnet test %testproject% -c Debug -f net6.0 --logger trx /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput="../../report/"

%USERPROFILE%\.nuget\packages\ReportGenerator\4.8.13\tools\net5.0\ReportGenerator.exe -reports:"coverage.opencover.xml" -targetdir:"coverlet"

start coverlet\index.htm

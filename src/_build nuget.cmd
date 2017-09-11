dotnet pack -c Release System.Linq.Dynamic.Core\System.Linq.Dynamic.Core.csproj --include-symbols
dotnet pack -c Release EntityFramework.DynamicLinq\EntityFramework.DynamicLinq.csproj --include-symbols
dotnet pack -c Release Microsoft.EntityFrameworkCore.DynamicLinq\Microsoft.EntityFrameworkCore.DynamicLinq.csproj --include-symbols
pause

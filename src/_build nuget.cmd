dotnet restore
dotnet pack -c Release System.Linq.Dynamic.Core\project.json
dotnet pack -c Release EntityFramework.DynamicLinq\project.json
dotnet pack -c Release Microsoft.EntityFrameworkCore.DynamicLinq\project.json
pause
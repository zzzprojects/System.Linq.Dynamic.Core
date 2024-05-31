if (Test-Path -Path ".\output") {
    Remove-Item -Force -Recurse -Path ".\output\"
}
else {
    New-Item -Path ".\output" -ItemType Directory
}


Write-Host "Build solution..."

dotnet build .\LinqCoreDemo.sln --nologo --configuration "Debug"

Write-Host "Publish projects..."
dotnet publish .\src\Demo.Plugin\Demo.Plugin.csproj --nologo --no-build --configuration "Debug" --output .\output
dotnet publish .\src\Demo.Host\Demo.Host.csproj --nologo --no-build --configuration "Debug" --output .\output

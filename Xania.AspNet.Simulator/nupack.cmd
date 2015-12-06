msbuild

msbuild ..\Xania.AspNet.Http\Xania.AspNet.Http.csproj /p:TargetFrameworkVersion=v4.5

.\.nuget\nuget.exe pack -IncludeReferencedProjects .\Xania.AspNet.Simulator.csproj
pause

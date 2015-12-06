msbuild

msbuild Xania.AspNet.Simulator.csproj /p:Configuration=NET45 /p:TargetFrameworkVersion=v4.5 /p:DefineConstants="NET45;TRACE" /p:OutputPath=bin\DebugNet45\

.\.nuget\nuget.exe pack -IncludeReferencedProjects .\Xania.AspNet.Simulator.csproj
pause

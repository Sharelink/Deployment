cd D:\Source\Git\bahamut\AuthenticationServer\src\AuthenticationServer
D:
dotnet restore
dotnet publish -f netcoreapp1.0 -r ubuntu.14.04-x64 -c Release -o D:\Source\Git\bahamut\Deployment\apps\Auth
pause

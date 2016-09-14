dotnet /home/deployment/Deployment/apps/Auth/AuthenticationServer.dll --config /etc/bahamut/auth/auth.json

dotnet /home/deployment/Deployment/apps/Chicago/Chicago.dll --config /etc/bahamut/chicago/config.json

dotnet /home/deployment/Deployment/apps/Fire/FireServer.dll --config /etc/bahamut/fire/fire.json

dotnet /home/deployment/Deployment/apps/Toronto/TorontoAPIServer.dll --server.urls http://127.0.0.1:8088

dotnet /home/deployment/Deployment/apps/Vege/VessageRESTfulServer.dll --config /etc/bahamut/vege/vessage.json

dotnet /home/deployment/Deployment/apps/Wellington/Wellington.dll --server.urls http://127.0.0.1:8080
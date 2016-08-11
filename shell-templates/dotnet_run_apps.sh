mono /home/deployment/Deployment/apps/Auth/AuthenticationServer.exe --server.urls http://127.0.0.1:8086

mono /home/deployment/Deployment/apps/Chicago/Chicago.exe --config /etc/bahamut/chicago/config.json

mono /home/deployment/Deployment/apps/Fire/FireServer.exe --server.urls http://127.0.0.1:8089

mono /home/deployment/Deployment/apps/Toronto/TorontoAPIServer.exe --server.urls http://127.0.0.1:8088

mono /home/deployment/Deployment/apps/Vege/VessageRESTfulServer.exe --config /etc/bahamut/vege/vessage.json

dotnet /home/deployment/Deployment/apps/Wellington/Wellington.dll --server.urls http://127.0.0.1:8080
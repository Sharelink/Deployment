chmod +x /home/deployment/Deployment/apps/AuthenticationServer/approot/web
export MONO_THREADS_PER_CPU=67
/home/deployment/Deployment/apps/AuthenticationServer/approot/web --server.urls http://auth.sharelink.online:8086 &
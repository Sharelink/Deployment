chmod +x /home/deployment/Deployment/apps/Toronto/approot/web
export MONO_THREADS_PER_CPU=1000
/home/deployment/Deployment/apps/Toronto/approot/web --server.urls http://api.sharelink.online:8088 &
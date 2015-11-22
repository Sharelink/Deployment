chmod +x /home/deployment/Deployment/apps/TorontoWebsite/approot/web
export MONO_THREADS_PER_CPU=80
/home/deployment/Deployment/apps/TorontoWebsite/approot/web --server.urls http://www.sharelink.online:8080 &
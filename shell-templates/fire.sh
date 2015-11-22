chmod +x ~/Deployment/apps/BahamutFire/approot/web
export MONO_THREADS_PER_CPU=100
~/Deployment/apps/BahamutFire/approot/web --server.urls http://file.sharelink.online:8089 &
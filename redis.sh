 cd /usr/tmp
 wget http://download.redis.io/releases/redis-3.0.5.tar.gz
 tar -zxf redis-3.0.5.tar.gz
 cd redis-3.0.5
 yum -y install tcl
 yum -y install gcc gcc-c++ libstdc++-devel
 make PREFIX=/usr/local/redis install
 mkdir /usr/local/redis/data
 cd utils
 ./install_server.sh
 #configurate server config
 #/usr/local/redis/bin/redis-server
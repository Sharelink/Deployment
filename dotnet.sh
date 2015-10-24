#install dnvm
sudo yum -y install unzip
curl -sSL https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.sh | DNX_BRANCH=dev sh && source ~/.dnx/dnvm/dnvm.sh

#install mono
yum -y install gcc gcc-c++ bison pkgconfig glib2-devel gettext make libpng-devel libjpeg-devel libtiff-devel libexif-devel giflib-devel libX11-devel freetype-devel fontconfig-devel  cairo-devel
wget http://download.mono-project.com/sources/libgdiplus/libgdiplus-3.12.tar.gz
tar zxvf libgdiplus-3.12.tar.gz
cd libgdiplus-3.12
./configure --prefix=/usr
make
make install

wget http://download.mono-project.com/sources/mono/mono-4.0.4.1.tar.bz2
tar -jxvf mono-4.0.4.1.tar.bz2
cd mono-4.0.4
./configure --prefix=/usr
make
make install

#fix dnu restore fail issue
export MONO_THREADS_PER_CPU=2000

#lib64/libuv for kestral
sudo yum install automake libtool wget
wget http://dist.libuv.org/dist/v1.4.2/libuv-v1.4.2.tar.gz
tar -zxf libuv-v1.4.2.tar.gz
cd libuv-v1.4.2
sudo sh autogen.sh
sudo ./configure
sudo make
sudo make check
sudo make install
ln -s /usr/lib64/libdl.so.2 /usr/lib64/libdl
ln -s /usr/local/lib/libuv.so /usr/lib64/libuv.so.1
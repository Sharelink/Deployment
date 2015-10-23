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

wget http://download.mono-project.com/sources/mono/mono-4.2.1.60.tar.bz2
tar -jxvf mono-4.2.1.60.tar.bz2
cd mono-4.2.1
./configure --prefix=/usr
make
make install
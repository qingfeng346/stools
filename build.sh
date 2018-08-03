cd szip
version=1.0.0
dotnet publish -c release -o ../build/szip-$version-windows-x64 -r win7-x64
dotnet publish -c release -o ../build/szip-$version-macos-x64 -r osx.10.11-x64
dotnet publish -c release -o ../build/szip-$version-centos-x64 -r centos.7-x64
dotnet publish -c release -o ../build/szip-$version-ubuntu-x64 -r ubuntu.14.04-x64
dotnet publish -c release -o ../build/szip-$version-debian-x64 -r debian.8-x64
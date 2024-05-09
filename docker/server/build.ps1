Remove-Item -Path "./stools" -Force -Recurse
Copy-Item -Path "../../stools/stools/src/" -Destination "./stools/src/" -Recurse -Force
Copy-Item -Path "../../stools/stools/stools.csproj" -Destination "./stools/stools.csproj" -Force
docker build -t stools .
docker save -o ../stools.tar stools
Remove-Item -Path "./stools/src" -Force -Recurse
Copy-Item -Path "../../stools/stools/stools.csproj" -Destination "./stools/stools.csproj" -Force
Copy-Item -Path "../../stools/stools/src/" -Destination "./stools/src/" -Recurse -Force
docker build -t stools .
docker save -o ../stools.tar stools
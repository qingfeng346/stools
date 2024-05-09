cd ../client
# npm run build
cd ../docker
Remove-Item -Path ./server -Force -Recurse
Remove-Item -Path ./command -Force -Recurse
Remove-Item -Path ./stools -Force -Recurse

Copy-Item -Path ../server/client/ -Destination ./server/client/ -Recurse -Force
Copy-Item -Path ../server/src/ -Destination ./server/src/ -Recurse -Force
Copy-Item -Path ../server/index.js -Destination ./server/index.js -Recurse -Force
Copy-Item -Path ../server/package.json -Destination ./server/package.json -Recurse -Force

Copy-Item -Path ../../stools/stools/src/ -Destination ./stools/src/ -Recurse -Force
Copy-Item -Path ../../stools/stools/stools.csproj -Destination ./stools/stools.csproj -Force

Copy-Item -Path ../command/src/ -Destination ./command/src/ -Recurse -Force
Copy-Item -Path ../command/index.js -Destination ./command/index.js -Recurse -Force
Copy-Item -Path ../command/package.json -Destination ./command/package.json -Recurse -Force
# docker build -t stools .
# docker save -o ../stools.tar stools
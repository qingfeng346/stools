$version = "0.0.1"
dotnet publish ./ImageViewer/ImageViewer.csproj -c release -o ./bin/win-x64 --self-contained -r win-x64 -p:AssemblyVersion=$version -p:FileVersion=$version -p:PublishSingleFile=true
$version = "1.0.0"

$today = Get-Date
$date = $today.ToString('yyyy-MM-dd')
Remove-Item ./bin/* -Force -Recurse

Set-Location ./szip
$fileData = @"
namespace szip {
    public static class Version {
        public const string version = "$version";
        public const string date = "$date";
    }
}
"@
$fileData | Out-File -Encoding utf8 ./src/Version.cs

dotnet publish -c release -o ../bin/szip-win-x64 -r win-x64
dotnet publish -c release -o ../bin/szip-osx-x64 -r osx-x64
dotnet publish -c release -o ../bin/szip-linux-x64 -r linux-x64


Compress-Archive ../bin/szip-win-x64 ../bin/szip-$version-win-x64.zip -Force
Compress-Archive ../bin/szip-osx-x64 ../bin/szip-$version-osx-x64.zip -Force
Compress-Archive ../bin/szip-linux-x64 ../bin/szip-$version-linux-x64.zip -Force

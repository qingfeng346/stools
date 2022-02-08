$version="1.0.0"
$name="stools"


$today = Get-Date
$date=$today.ToString('yyyy-MM-dd')
$fileData=@"
namespace Scorpio.stools {
    public static class Version {
        public const string version = "$version";
        public const string date = "$date";
    }
}
"@
$fileData | Out-File -Encoding utf8 ../stools/stools/src/Version.cs

Remove-Item ../bin/* -Force -Recurse
$platforms = @("win-x86", "win-x64", "win-arm", "win-arm64", "linux-x64", "linux-musl-x64", "linux-arm", "linux-arm64", "osx-x64", "osx-arm64")
# $platforms = @()
$aipPath = ".\Install.aip"
foreach ($platform in $platforms) {
    Write-Host "正在打包 $platform 版本..."
    $pathName = "$name-$platform"
    dotnet publish ../stools/stools/stools.csproj -c release -o ../bin/$pathName -r $platform --self-contained -p:AssemblyVersion=$version | Out-Null
    Copy-Item -Path "../stools/stools/bash/*" -Destination ../bin/$pathName/
    Write-Host "正在压缩 $platform ..."
    $fileName = "$name-$version-$platform"
    Compress-Archive ../bin/$pathName/* ../bin/$fileName.zip -Force
    if ($IsWindows -and (($platform -eq "win-x86") -or ($platform -eq "win-x64"))) {
        Write-Host "正在生成安装包 $platform ..."
        git checkout $aipPath
        Get-ChildItem ..\bin\$pathName\ | ForEach-Object -Process{
            if($_ -is [System.IO.FileInfo]) {
                AdvancedInstaller.com /edit $aipPath /AddFile APPDIR $_.FullName
            }
        }
        AdvancedInstaller.com /edit $aipPath /SetVersion $version
        AdvancedInstaller.com /edit $aipPath /SetPackageName ..\bin\$fileName.msi -buildname DefaultBuild
        if ($platform -eq "win-x86") {
            AdvancedInstaller.com /edit $aipPath /SetPackageType x86 -buildname DefaultBuild
        } elseif ($platform -eq "win-x64") {
            AdvancedInstaller.com /edit $aipPath /SetPackageType x64 -buildname DefaultBuild
        }
        AdvancedInstaller.com /build $aipPath -buildslist DefaultBuild
        git checkout $aipPath
    }
}


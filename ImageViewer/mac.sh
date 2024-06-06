#!/bin/bash
version="0.0.1"
dotnet publish ./ImageViewer/ImageViewer.csproj -c release -o ./bin/osx-x64 --self-contained -r osx-x64 -p:AssemblyVersion=$version -p:FileVersion=$version

#!/bin/bash
APP_NAME="./ImageViewer.app"
PUBLISH_OUTPUT_DIRECTORY="./bin/osx-x64/."
# PUBLISH_OUTPUT_DIRECTORY should point to the output directory of your dotnet publish command.
# One example is /path/to/your/csproj/bin/Release/netcoreapp3.1/osx-x64/publish/.
# If you want to change output directories, add `--output /my/directory/path` to your `dotnet publish` command.
INFO_PLIST="./Info.plist"
ICON_FILE="./icon.icns"

if [ -d "$APP_NAME" ]
then
    rm -rf "$APP_NAME"
fi

mkdir "$APP_NAME"

mkdir "$APP_NAME/Contents"
mkdir "$APP_NAME/Contents/MacOS"
mkdir "$APP_NAME/Contents/Resources"

cp "$INFO_PLIST" "$APP_NAME/Contents/Info.plist"
cp "$ICON_FILE" "$APP_NAME/Contents/Resources/$ICON_FILE"
cp -a "$PUBLISH_OUTPUT_DIRECTORY" "$APP_NAME/Contents/MacOS"

ENTITLEMENTS="./MyAppEntitlements.entitlements"
SIGNING_IDENTITY="Apple Development: linyuan.yang@centurygame.com (J58CW2UZN9)" # matches Keychain Access certificate name

find "$APP_NAME/Contents/MacOS/"|while read fname; do
    if [[ -f $fname ]]; then
        echo "[INFO] Signing $fname"
        codesign --force --timestamp --options=runtime --entitlements "$ENTITLEMENTS" --sign "$SIGNING_IDENTITY" "$fname"
    fi
done

echo "[INFO] Signing app file"

codesign --force --timestamp --options=runtime --entitlements "$ENTITLEMENTS" --sign "$SIGNING_IDENTITY" "$APP_NAME"
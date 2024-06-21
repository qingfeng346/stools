# dotnet publish -p:PublishProfile=./Jellyfin.Plugin.MyMetadata/Properties/PublishProfiles/FolderProfile.pubxml
dotnet build
Copy-Item ./Jellyfin.Plugin.MyMetadata/bin/Debug/net8.0/Jellyfin.Plugin.MyMetadata.dll -Destination "/Users/da/Library/Application Support/jellyfin/plugins/MyMetadata/Jellyfin.Plugin.MyMetadata.dll"
Copy-Item ./Jellyfin.Plugin.MyMetadata/bin/Debug/net8.0/Jellyfin.Plugin.MyMetadata.pdb -Destination "/Users/da/Library/Application Support/jellyfin/plugins/MyMetadata/Jellyfin.Plugin.MyMetadata.pdb"
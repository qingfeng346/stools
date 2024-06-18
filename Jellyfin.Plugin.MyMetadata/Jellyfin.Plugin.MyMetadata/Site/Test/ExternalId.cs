﻿using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.MyMetadata.Service.Test {
    public class ExternalId : IExternalId
    {
        public string ProviderName => "MyMetadata";
        public string Key => "MyMetadataID";
        public ExternalIdMediaType? Type => null;
        public string UrlFormatString => $"UrlFormatString";
        public bool Supports(IHasProviderIds item)
        {
            return item is Movie || item is Video;
        }
    }
}

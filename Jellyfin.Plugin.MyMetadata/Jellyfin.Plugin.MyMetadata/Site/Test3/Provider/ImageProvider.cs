﻿using Microsoft.Extensions.Logging;
namespace Jellyfin.Plugin.MyMetadata.Service.Test3 {
    public class ImageProvider : ImageProvider<TestHttpService> {
        public override string Name => Config.ProviderName;
        public override string ProviderID => Config.ProviderID;
        public ImageProvider(ILogger<ImageProvider> logger, TestHttpService httpService) : base(logger, httpService) { }
    }
}

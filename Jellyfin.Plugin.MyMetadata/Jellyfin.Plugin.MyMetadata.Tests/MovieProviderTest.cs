using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Jellyfin.Plugin.MyMetadata.Service.Test;

using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;

using Xunit;
using Xunit.Abstractions;

namespace Jellyfin.Plugin.MyMetadata.Tests
{
    public class MovieProviderTest
    {
        private readonly MovieProvider _provider;
        public MovieProviderTest(ITestOutputHelper output)
        {
            var serviceProvider = ServiceUtils.BuildServiceProvider<MovieProvider>(output);
            _provider = serviceProvider.GetService<MovieProvider>();
        }

        [Fact]
        public async Task TestGetSearchResults()
        {
            // Test 1: search metadata.
            //MovieInfo info = new MovieInfo() {
            //    Name = "我吃西红柿",
            //};

            //var result = await _provider.GetSearchResults(info, CancellationToken.None);
            //Assert.NotEmpty(result);
            //string doubanId = result.FirstOrDefault()?.GetProviderId(BaseProvider.ProviderID);
            //int? year = result.FirstOrDefault()?.ProductionYear;
            //Assert.Equal("1851857", doubanId);
            //Assert.Equal(2008, year);

            //// Test 2: Already has provider Id.
            //info.SetProviderId(BaseProvider.ProviderID, "1851857");
            //result = await _provider.GetSearchResults(info, CancellationToken.None);
            //Assert.Single(result);
            //doubanId = result.FirstOrDefault()?.GetProviderId(BaseProvider.ProviderID);
            //year = result.FirstOrDefault()?.ProductionYear;
            //Assert.Equal("1851857", doubanId);
            //Assert.Equal(2008, year);
        }

        [Fact]
        public async Task TestGetMetadata()
        {
            // Test 1: Normal case.
            MovieInfo info = new MovieInfo()
            {
                Name = "我吃西红柿"
            };
            var meta = await _provider.GetMetadata(info, CancellationToken.None);
            //Assert.True(meta.HasMetadata);
            //Assert.Equal("源代码", meta.Item.Name);
            //Assert.Equal("3075287", meta.Item.GetProviderId(Config.ProviderID));
            //Assert.Equal(DateTime.Parse("2011-08-30"), meta.Item.PremiereDate);

            // Test 2: Already has provider Id.
            //info = new MovieInfo()
            //{
            //    Name = "Source Code"
            //};
            //info.SetProviderId(Config.ProviderID, "1851857");
            //meta = await _provider.GetMetadata(info, CancellationToken.None);
            //Assert.True(meta.HasMetadata);
            //Assert.Equal("蝙蝠侠：黑暗骑士", meta.Item.Name);

            //// Test 2: Not movie type.
            //info = new MovieInfo()
            //{
            //    Name = "大秦帝国"
            //};
            //meta = await _provider.GetMetadata(info, CancellationToken.None);
            //Assert.False(meta.HasMetadata);
        }
    }
}

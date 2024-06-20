using Microsoft.Extensions.DependencyInjection;
using Jellyfin.Plugin.MyMetadata.Service.Test;
using MediaBrowser.Controller.Providers;
using Xunit.Abstractions;
namespace Jellyfin.Plugin.MyMetadata.Tests {
    public class MovieProviderTest {
        private readonly MovieProvider _provider;
        private ITestOutputHelper output;
        public MovieProviderTest(ITestOutputHelper output) {
            this.output = output;
            var serviceProvider = ServiceUtils.BuildServiceProvider<MovieProvider>(output);
            _provider = serviceProvider.GetService<MovieProvider>();
        }
        [Fact]
        public async Task TestGetMetadata() {
            // output.WriteLine("1111111111111111111");
            // System.Diagnostics.Debug.WriteLine("123123123");
            // throw new Exception("123123123");
            // // Test 1: Normal case.
            MovieInfo info = new MovieInfo() {
                Name = "111",
                Path = "JUX-236.mp4"
            };
            await _provider.GetMetadata(info, CancellationToken.None);
        }
    }
}

using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using Microsoft.Extensions.Logging;
namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class PersonProvider<T> : BaseProvider<PersonProvider<T>, T, Person, PersonLookupInfo> where T : HttpService {
        public PersonProvider(ILogger<PersonProvider<T>> logger, T httpService) : base(logger, httpService) { }
        protected override async Task<MetadataResult<Person>> GetMetadata_impl(PersonLookupInfo info, CancellationToken cancellationToken) {
            var id = info.GetProviderId(ProviderID);
            var name = Path.GetFileNameWithoutExtension(info.Path);
            var personId = await httpService.GetPersonIdByName(id, info.Name, info.Path, cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrEmpty(personId))
                throw new Exception("personId is null");
            var personInfo = await httpService.GetPersonMetadataAsync(personId, cancellationToken).ConfigureAwait(false);
            if (personInfo == null)
                throw new Exception("personInfo is null");
            if (personInfo.HasMetadata)
                info.SetProviderId(ProviderID, personId);
            return personInfo;
        }
    }
}

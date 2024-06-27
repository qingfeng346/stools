const ProviderTest = require('./ProviderTest');
class ProviderManager {
    constructor() {
        this.providers = []
        this.providers.push(ProviderTest)
    }
    async GetMovieInfo(name) {
        for (let provider of this.providers) {
            try {
                let info = await provider.GetMovieInfo(name)
                if (info != null)
                    return info
            } catch (err) {

            }
        }
    }
    async GetPersonInfo(name) {
        for (let provider of this.providers) {
            try {
                let info = await provider.GetPersonInfo(name)
                if (info != null)
                    return info
            } catch (err) {

            }
        }
    }
}
module.exports = new ProviderManager()
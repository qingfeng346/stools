const { logger } = require('weimingcommons');
const ProviderTest = require('./ProviderTest');
const ProviderTest2 = require('./ProviderTest2');
const ProviderTest3 = require('./ProviderTest3');
class ProviderManager {
    constructor() {
        this.providers = []
        this.providers.push(ProviderTest)
        this.providers.push(ProviderTest3)
        // this.providers.push(ProviderTest2)
    }
    async GetMovieInfo(name) {
        for (let provider of this.providers) {
            try {
                let info = await provider.GetMovieInfo(name)
                if (info != null)
                    return info
            } catch (err) {
                logger.error(`provider is error : ${err.message}`)
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
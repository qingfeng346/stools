const { logger } = require('weimingcommons');
const ProviderTest = require('./ProviderTest');
const ProviderTest2 = require('./ProviderTest2');
const ProviderTest3 = require('./ProviderTest3');
const ProviderTest4 = require('./ProviderTest4');
class ProviderManager {
    constructor() {
        this.providers = []
        this.providers.push(ProviderTest)
        this.providers.push(ProviderTest2)
        this.providers.push(ProviderTest3)
        this.providers.push(ProviderTest4)
    }
    GetProvider(name) {
        for (let provider of this.providers) {
            if (provider.constructor.name == name) {
                return provider
            }
        }
        return null
    }
    async GetMovieInfo(name) {
        for (let provider of this.providers) {
            try {
                let info = await provider.GetMovieInfo(name)
                if (info != null) {
                    return info
                }
            } catch (err) {
                logger.error(`provider:${provider.constructor.name} GetMovieInfo is error : ${err.message}\n${err.stack}`)
            }
        }
    }
    async ParseMovieInfo(name, type, content) {
        return await this.GetProvider(type).ParseMovieInfo(name, content)
    }
    async GetPersonInfo(name) {
        for (let provider of this.providers) {
            try {
                let info = await provider.GetPersonInfo(name)
                if (info != null) {
                    if (info.imageUrl == null) info.imageUrl = `/assets/images/150x150.png`
                    return info
                }
            } catch (err) {
                logger.error(`provider:${provider.constructor.name} GetPersonInfo is error : ${err.message}\n${err.stack}`)
            }
        }
    }
    async ParsePersonInfo(name, type, content) {
        return await this.GetProvider(type).ParsePersonInfo(name, content)
    }
}
module.exports = new ProviderManager()
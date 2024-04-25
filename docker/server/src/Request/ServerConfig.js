const { RequestCode } = require('../code')
const message = require('../message')
const database = require('../database')
class ServerConfig {
    async init() {
        message.register(RequestCode.GetConfigList, this.OnGetConfigList.bind(this))
        message.register(RequestCode.GetConfig, this.OnGetConfig.bind(this))
        message.register(RequestCode.SetConfig, this.OnSetConfig.bind(this))
        message.register(RequestCode.DelConfig, this.OnDelConfig.bind(this))
    }
    async GetConfigString(key) {
        return (await database.config.findOrCreate({ defaults: { value: "{}" }, where: { key: key } }))[0].dataValues.value
    }
    async OnGetConfigList() {
        return await database.config.findAll({ attributes: ['key'] })
    }
    async OnGetConfig(data) {
        return await this.GetConfigString(data.key)
    }
    async OnSetConfig(data) {
        let key = data.key
        let value = data.value
        if (typeof(value) != "string") {
            value = JSON.stringify(value)
        }
        await database.config.upsert({ key: key, value: value, }, { where: { key: key } })
    }
    async OnDelConfig(data) {
        await database.config.destroy({ where: { key: data.key } })
    }
    
}
module.exports = new ServerConfig()
const { RequestCode } = require('../code')
const message = require('../message')
const database = require('../database')
class ServerConfig {
    async init() {
        message.register(RequestCode.GetConfigList, this.OnGetConfigList.bind(this))
        message.register(RequestCode.GetConfig, this.OnGetConfig.bind(this))
        message.register(RequestCode.SetConfig, this.OnSetConfig.bind(this))
        message.register(RequestCode.DelConfig, this.OnDelConfig.bind(this))

        message.register(RequestCode.GetCommandList, this.OnGetCommandList.bind(this))
        message.register(RequestCode.GetCommand, this.GetCommand.bind(this))
        message.register(RequestCode.SetCommand, this.SetCommand.bind(this))
        message.register(RequestCode.DelCommand, this.DelCommand.bind(this))
    }
    async OnGetConfigList() {
        return await database.config.findAll({ attributes: ['key'] })
    }
    async OnGetConfig(data) {
        return (await database.config.findOrCreate({ defaults: { value: "{}" }, where: { key: data.key } }))[0].dataValues.value
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
    async OnGetCommandList() {
        return await database.command.findAll({ attributes: ['name', 'info'] })
    }
    async GetCommand() {
        return await database.command.findAll({ attributes: ['name', 'info'] })
    }
    async SetCommand() {
        return await database.command.findAll({ attributes: ['name', 'info'] })
    }
    async DelCommand() {
        return await database.command.findAll({ attributes: ['name', 'info'] })
    }
}
module.exports = new ServerConfig()
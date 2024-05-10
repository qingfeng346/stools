const { RequestCode, ConfigType } = require('../code')
const message = require('../message')
const database = require('../database')
class ServerConfig {
    async init() {
        message.register(RequestCode.GetConfigList, this.OnGetConfigList.bind(this))
        message.register(RequestCode.GetConfig, this.OnGetConfig.bind(this))
        message.register(RequestCode.SetConfig, this.OnSetConfig.bind(this))
        message.register(RequestCode.DelConfig, this.OnDelConfig.bind(this))

        message.register(RequestCode.GetCommandList, this.OnGetCommandList.bind(this))
        message.register(RequestCode.GetCommand, this.OnGetCommand.bind(this))
        message.register(RequestCode.SetCommand, this.OnSetCommand.bind(this))
        message.register(RequestCode.DelCommand, this.OnDelCommand.bind(this))

        message.register(RequestCode.GetStorage, this.OnGetStorage.bind(this))
        message.register(RequestCode.SetStorage, this.OnSetStorage.bind(this))
        message.register(RequestCode.DelStorage, this.OnDelStorage.bind(this))
    }
    async GetConfig(name) {
        return JSON.parse((await database.config.findOrCreate({ defaults: { value: "{}" }, where: { name: name } }))[0].dataValues.value)
    }
    async GetBuildConfig() {
        return await this.GetConfig(ConfigType.BuildConfig)
    }
    async GetCommand(name) {
        return await database.command.findOne({ where: { name: name } })
    }

    async OnGetConfigList() {
        return await database.config.findAll({ attributes: ['name'] })
    }
    async OnGetConfig(data) {
        return await this.GetConfig(data.name)
    }
    async OnSetConfig(data) {
        let name = data.name
        let value = data.value
        if (typeof(value) != "string") {
            value = JSON.stringify(value)
        }
        await database.config.upsert({ name: name, value: value, }, { where: { name: name } })
    }
    async OnDelConfig(data) {
        await database.config.destroy({ where: { name: data.name } })
    }
    async OnGetCommandList() {
        return await database.command.findAll({ attributes: ['name', 'info'] })
    }
    async OnGetCommand(data) {
        return await this.GetCommand(data.name)
    }
    async OnSetCommand(data) {
        let info = data.info
        let content = data.content
        let execute = data.execute
        let operate = data.operate
        if (typeof(info) != "string") { info = JSON.stringify(info) }
        if (typeof(content) != "string") { content = JSON.stringify(content) }
        if (typeof(execute) != "string") { execute = JSON.stringify(execute) }
        if (typeof(operate) != "string") { operate = JSON.stringify(operate) }
        await database.command.upsert({ name: data.name, info: info, content: content, execute: execute, operate: operate }, { where: { name: data.name } });
    }
    async OnDelCommand(data) {
        await database.command.destroy({ where: { name: data.name } })
    }

    async OnGetStorage(data) {
        return await database.storage.findOne({where: { name: data.name }})
    }
    async OnSetStorage(data) {
        await database.storage.upsert({ name: data.name, value: data.value, }, { where: { name: data.name } })
    }
    async OnDelStorage(data) {
        await database.storage.destroy({ where: { name: data.name } })
    }
}
module.exports = new ServerConfig()
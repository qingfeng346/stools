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
        return await database.config.findAll({ attributes: ['name'] })
    }
    async OnGetConfig(data) {
        return (await database.config.findOrCreate({ defaults: { value: "{}" }, where: { name: data.name } }))[0].dataValues.value
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
    async GetCommand(data) {
        return await database.command.findOne({ where: { name: data.name } })
    }
    async SetCommand(data) {
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
    async DelCommand(data) {
        await database.command.destroy({ where: { name: data.name } })
    }
}
module.exports = new ServerConfig()
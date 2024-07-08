import { RequestCode } from '../code.js'
import message from '../message.js'
import database from '../database.js'
class ServerConfig {
    async init() {
        message.register(RequestCode.GetConfigList, this.OnGetConfigList.bind(this))
        message.register(RequestCode.GetConfig, this.OnGetConfig.bind(this))
        message.register(RequestCode.SetConfig, this.OnSetConfig.bind(this))
        message.register(RequestCode.DelConfig, this.OnDelConfig.bind(this))

        message.register(RequestCode.GetStorage, this.OnGetStorage.bind(this))
        message.register(RequestCode.SetStorage, this.OnSetStorage.bind(this))
        message.register(RequestCode.DelStorage, this.OnDelStorage.bind(this))

        message.register(RequestCode.SyncDatabase, this.OnSyncDatabase.bind(this))
    }
    async GetConfig(name) {
        return JSON.parse((await database.config.findOrCreate({ defaults: { value: "{}" }, where: { name: name } }))[0].dataValues.value)
    }
    //获取config列表
    async OnGetConfigList() {
        return await database.config.findAll({ attributes: ['name'] })
    }
    //获取config
    async OnGetConfig(data) {
        return await this.GetConfig(data.name)
    }
    //设置config
    async OnSetConfig(data) {
        let name = data.name
        let value = data.value
        if (typeof(value) != "string") {
            value = JSON.stringify(value)
        }
        await database.config.upsert({ name: name, value: value, }, { where: { name: name } })
    }
    //删除config
    async OnDelConfig(data) {
        await database.config.destroy({ where: { name: data.name } })
    }

    //获取storage
    async OnGetStorage(data) {
        return await database.storage.findOne({where: { name: data.name }})
    }
    //设置config
    async OnSetStorage(data) {
        await database.storage.upsert({ name: data.name, value: data.value, }, { where: { name: data.name } })
    }
    //删除storage
    async OnDelStorage(data) {
        await database.storage.destroy({ where: { name: data.name } })
    }
    //更新数据库
    async OnSyncDatabase() {
        await database.sync()
    }
}
export default new ServerConfig()
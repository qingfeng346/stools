const ProviderManager = require("../Provider/ProviderManager")
const database = require("../database")
class ActorManager {
    constructor() {
        this.pendingIds = []
    }
    async GetActorInfoByName(name) {
        let value = (await database.actor.findOrCreate({ where: { name: name } }))[0].dataValues
        if (!value.isInfo) {
            this.UpdatePersonInfo(value.id)
        }
        return value
    }
    async GetActorInfoById(id) {
        let value = await database.actor.findOne({ where: { id: id } })
        if (!value.isInfo) {
            this.UpdatePersonInfo(value.id)
        }
        return value
    }
    UpdatePersonInfo(id) {
        if (this.pendingIds.indexOf(id) < 0) {
            this.pendingIds.push(id)
        }
    }
    async update() {
        if (this.pendingIds.length > 0) {
            await this.RefreshInfo(this.pendingIds.pop())
        }
    }
    async RefreshInfo(id) {
        let value = await database.actor.findOne({ where: { id: id } })
        if (value == null) {
            throw new Error(`找不到ActorId:${id}`)
        }
        let personInfo = await ProviderManager.GetPersonInfo(value.name)
        value = value.dataValues
        value.isInfo = true
        if (personInfo != null) {
            value.desc = personInfo.desc
            value.imageUrl = personInfo.imageUrl
        }
        await database.actor.update(value, { where: {id: id}})
    }
}
module.exports = new ActorManager()
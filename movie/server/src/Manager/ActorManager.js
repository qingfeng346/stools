const ProviderManager = require("../Provider/ProviderManager")
const database = require("../database")
const ImageManager = require("./ImageManager")
class ActorManager {
    constructor() {
        this.pendingIds = []
        this.pendingParse = []
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
    ParseMovieInfo(id, type, content) {
        this.pendingParse.push({id: id, type: type, content: content})
    }
    async update() {
        if (this.pendingIds.length > 0) {
            await this.RefreshInfo(this.pendingIds.pop())
        } else if (this.pendingParse.length > 0) {
            let data = this.pendingParse.pop()
            await this.RefreshInfo(data.id, data.type, data.content)
        }
    }
    async RefreshInfo(id, type, content) {
        let value = await database.actor.findOne({ where: { id: id } })
        if (value == null) {
            throw new Error(`找不到ActorId:${id}`)
        }
        let personInfo = null
        if (type == null || content == null) {
            personInfo = await ProviderManager.GetPersonInfo(value.name)
        } else {
            personInfo = await ProviderManager.ParsePersonInfo(value.name, type, content)
        }
        value = value.dataValues
        value.isInfo = true
        value.imageUrl = (await ImageManager.GetImageInfoByUrl(personInfo?.imageUrl ?? utils.getDefaultImage(150,150)))?.id
        if (personInfo != null) {
            value.desc = personInfo.desc
        }
        await database.actor.update(value, { where: {id: id}})
    }
}
module.exports = new ActorManager()
const database = require("../database")
class ActorManager {
    constructor() {
        this.pendingIds = []
    }
    async GetActorInfoByName(name) {
        let value = (await database.actor.findOrCreate({ where: { name: name } }))[0].dataValues
        if (!value.isInfo) {
            if (this.pendingIds.indexOf(value.id) < 0) {
                this.pendingIds.push(value.id)
            }
        }
        return value
    }
    async GetActorInfoById(id) {
        let value = await database.actor.findOne({ where: { id: id } })
        if (!value.isInfo) {
            if (this.pendingIds.indexOf(value.id) < 0) {
                this.pendingIds.push(value.id)
            }
        }
        return value
    }
    async update() {
        while (this.pendingIds.length > 0) {
            await this.RefreshInfo(this.pendingIds.pop())
        }
    }
    async RefreshInfo(id) {
        let value = await database.actor.findOne({ where: { id: id } })
        if (value == null) {
            throw new Error(`找不到ActorId:${id}`)
        }
        value = value.dataValues
        value.isInfo = true
        value.title = "title"
        value.desc = "desc"
        console.log(value)
        await database.actor.update(value, { where: {id: id}})
    }
}
module.exports = new ActorManager()
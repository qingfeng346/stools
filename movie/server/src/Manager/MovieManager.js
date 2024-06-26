const database = require("../database")
const ActorManager = require("./ActorManager")
const TagManager = require("./TagManager")
class MovieManager {
    constructor() {
        this.pendingIds = []
    }
    async GetMovieInfoByPath(path) {
        let value = (await database.movie.findOrCreate({ where: { path: path } }))[0].dataValues
        if (!value.isInfo) {
            if (this.pendingIds.indexOf(value.id) < 0) {
                this.pendingIds.push(value.id)
            }
        }
        return value
    }
    async GetMovieInfoById(id) {
        let value = await database.movie.findOne({ where: { id: id } })
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
        let value = await database.movie.findOne({ where: { id: id } })
        if (value == null) {
            throw new Error(`找不到MovieId:${id}`)
        }
        value = value.dataValues
        value.isInfo = true
        value.title = "title"
        value.desc = "desc"
        value.actors = []
        value.actors.push((await ActorManager.GetActorInfoByName("123")).id)
        value.actors.push((await ActorManager.GetActorInfoByName("456")).id)
        value.tags = []
        value.tags.push(await TagManager.GetTagId("123"))
        value.tags.push(await TagManager.GetTagId("456"))
        console.log(value)
        await database.movie.update(value, { where: {id: id}})
    }
}
module.exports = new MovieManager()
const { FileUtil, logger } = require("weimingcommons")
const database = require("../database")
const ActorManager = require("./ActorManager")
const TagManager = require("./TagManager")
const { QueryTypes } = require("sequelize")
class MovieManager {
    constructor() {
        this.pendingIds = []
    }
    async UpdateFileList() {
        let values = await database.movie.findAll()
        let allFiles = FileUtil.GetFiles("./media", true)
        if (allFiles == null) return
        for (let file of allFiles) {
            if (file.endsWith(".mp4") ||
                file.endsWith(".mkv") ||
                file.endsWith(".avi")) {
                await this.GetMovieInfoByPath(file)
            }
        }
        for (let value of values) {
            if (allFiles.indexOf(value.path) < 0) {
                logger.info(`文件 : ${value.path} 已删除`)
                await database.movie.destroy({ where: {id : value.id}})
            }
        }
        let result = await this.GetAllMovieInfosByActor(3)
        console.log(result)
        let a = 100
    }
    async GetMovieInfoByPath(path) {
        let value = (await database.movie.findOrCreate({ where: { path: path } }))[0].dataValues
        if (!value.isInfo) {
            logger.info(`添加新文件:${path}`)
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
        await database.movie.update(value, { where: {id: id}})
    }
    async GetAllMovieInfos() {
        await database.movie.findAll()
    }
    async GetAllMovieInfosByActor(id) {
        return await database.sequelize.query(`select * from \`movie\` where exists (select 1 from json_each(actors) where value = ${id})`, { type: QueryTypes.SELECT })
    }
    async GetAllMovieInfosByTag(id) {
        return await database.sequelize.query(`select * from \`movie\` where exists (select 1 from json_each(tags) where value = ${id})`, { type: QueryTypes.SELECT })
    }
}
module.exports = new MovieManager()